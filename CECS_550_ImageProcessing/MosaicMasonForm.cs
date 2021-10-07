using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using MosaicMason.Filters;
using MosaicMason.DataTypes;
using System.Collections;
using System.Threading;
using MosaicMason.Profiles;

namespace MosaicMason
{
    public partial class MosaicMasonForm : Form
    {
        Bitmap image1;

        string XIRURL;
        int imageIdIndex = 0;
        string masterImageAddress;
        string[] candidateImagePool;
        int imagesPerColumn = 32;
        int subdivLevel = 3;
        int validCandidateImages = 0;
        int xCells, yCells;
        bool isRunning = true;
        bool grayscaleMosaic = false;
        Size maxPictureBoxDimensions = new Size(640, 480);
        bool useCUDA = false;

        EColorProfile CurrentColorProfile = EColorProfile.HSV;

        //This is the mosaic itself
        SortedList<float, int>[] solutionSet;

        //Custom forms
        private SettingsForm settingsForm;
        private ProgressBarForm progressForm;
        private AboutForm aboutForm;

        public MosaicMasonForm()
        {
            InitializeComponent();

            XIRURL = Path.GetDirectoryName(Application.ExecutablePath) + "\\repository.xir";
            CreateProgressBarForm();
        }

        private void smiButton_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Images (*.bmp)|*.bmp;*.jpg;*.jpeg;*.png;*.gif";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    masterImageAddress = ofd.FileName.ToLower();
                    AddImageToLibraryIfNew(masterImageAddress);

                    image1 = new Bitmap(masterImageAddress, true);
                    UpdateStatusStrip((1.0f * image1.Width) / image1.Height);
                    
                    // Set the PictureBox to display the image.

                    Size newSize = XIRTools.CalculateNewSize(maxPictureBoxDimensions, image1.Size);
                    pictureBox1.Image = new Bitmap(image1, newSize);
                    pictureBox1.Left = (maxPictureBoxDimensions.Width / 2) - (newSize.Width / 2) + 10;
                    pictureBox1.Size = newSize;
                    scpButton.Enabled = true;
                    createMosaicButton.Enabled = validCandidateImages > 0;

                    UpdateOutputDimensions();
                }
            }
            catch (ArgumentException err)
            {
                MessageBox.Show("There was an error.  " +
                    "Check the path to the image file. \r\nERROR:" + err.Message + " " + err.StackTrace);
            }
        }

        private void UpdateStatusStrip(float aspectRatio)
        {
            validCandidateImages = XIRTools.ImagesWithSameAspectRatioCount(aspectRatio, null) - 1;
            CandidateImageValueLabel.ForeColor = (validCandidateImages < 100) ? Color.Red : Color.Black;
            CandidateImageValueLabel.Text = "" + validCandidateImages;
            int length = Math.Min(aspectRatio.ToString().Length, 5);
            aspectRatioValueLabel.Text = aspectRatio.ToString().Substring(0, length);
        }

        private void UpdateStatusStrip(string masterImageAddress)
        {
            XIRImage image = XIR.Instance.GetXIRImageByAddress(masterImageAddress);
            UpdateStatusStrip(image.aspectRatio);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void setRegionToColor(Bitmap image, Color color, int startX,
            int startY, int width, int height)
        {
            for (int y = startY + height; y >= startY; y--)
            {
                for (int x = startX + width; x >= startX; x--)
                {
                    image.SetPixel(x, y, color);
                }
            }
        }

        private void createMosaicButton_Click(object sender, EventArgs e)
        {
            bool shouldMakeMosaic = true;
            if (validCandidateImages < 50)
            {
                string message = "You only have " + validCandidateImages 
                    + " valid candidate images to make your mosaic out of.  "
                    + "It is reccomended that you use a minimum of 50 images.  " 
                    + "Do you want to continue anyhow?";
                shouldMakeMosaic = MessageBox.Show(message, "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes;
            }
            if (shouldMakeMosaic)
            {
                const int maxVal = 3;
                for (int i = 1; i < maxVal; ++i)
                {
                    HSL.weights[0] = i * 1.0f;
                    createMosaicButton.Enabled = false;
                    pictureBox1.Visible = true;

                    //Thread pixelationThread = new System.Threading.Thread(doPixelationAnalysis);
                    //pixelationThread.Start();
                    doPixelationAnalysis();

                    XIRImage image = XIR.Instance.GetXIRImageByAddress(masterImageAddress);
                    int imageId = image.id;
                    float aspectRatio = image.aspectRatio;

                    MosaicInputs mosaicInputs = new MosaicInputs();
                    mosaicInputs.ImageId = imageId;
                    mosaicInputs.AspectRatio = aspectRatio;
                    mosaicInputs.Columns = imagesPerColumn * subdivLevel;
                    mosaicInputs.RegionWidth = subdivLevel;
                    mosaicInputs.RegionHeight = subdivLevel;
                    mosaicInputs.ImagesToKeep = 45;
                    mosaicInputs.Grayscale = grayscaleCheckBox.Checked;
                    grayscaleMosaic = mosaicInputs.Grayscale;
                    mosaicInputs.NumThreads = (settingsForm != null) ? settingsForm.numThreads : 1;

                    //Thread mosaicThread = new Thread(FindImageCellMatchLocations);
                    //mosaicThread.Start(mosaicInputs);
                    FindImageCellMatchLocations(mosaicInputs);
                    saveToolStripMenuItem.Enabled = true;
                    createMosaicButton.Enabled = true;

                    //float aspectRatio = XIR.Instance.GetXIRImageByAddress(masterImageAddress).aspectRatio;

                    
                }
            }
        }

        public void doPixelationAnalysis()
        {
            XIRImage image = XIR.Instance.GetXIRImageByAddress(masterImageAddress);
            float aspectRatio;
            if (image == null)
            {
                Bitmap tmp = new Bitmap(masterImageAddress);
                aspectRatio = (tmp.Width * 1.0f) / tmp.Height;
                tmp.Dispose();
            }
            else
            {
                aspectRatio = image.aspectRatio;
            }
            doPixelationAnalysis(masterImageAddress, imagesPerColumn * subdivLevel, aspectRatio);
        }

        public void doPixelationAnalysis(string imageAddress, int subdivisionLevel, float aspectRatio)
        {
            XIRImage xirImage = XIR.Instance.GetXIRImageByAddress(imageAddress);
            Bitmap image = new Bitmap(imageAddress, true);
            bool imageTagIsValid = (xirImage != null) ? ImageStillValid(imageAddress) : false;
            if (xirImage != null && !imageTagIsValid)
            {
                XIR.Instance.CleanRepository();
            }
            if (xirImage == null)
            {
                DateTime lastWriteTime = File.GetLastWriteTime(imageAddress);
                float aspectRatio1 = (1.0f * image.Width) / image.Height;
                xirImage = CreateBasicImageNode(imageAddress, aspectRatio1, lastWriteTime,
                    image.Width, image.Height, true);
                XIR.Instance.Add(xirImage.id, xirImage);
            }
            if (!xirImage.PixelationTechniqueExists(aspectRatio, subdivisionLevel))
            {
                xirImage.pixTechs.Add(PixelationFilter.Filter(image, subdivisionLevel, aspectRatio));
                XIR.Instance[xirImage.id] = xirImage;
            }
            image.Dispose();
        }

        public void LoadXIR()
        {
            XIR.Instance.Load(XIRURL);
        }

        public XIRImage CreateBasicImageNode(string imageAddress, bool validate)
        {
            Bitmap image = new Bitmap(imageAddress);
            DateTime lastWriteTime = File.GetLastWriteTime(imageAddress);
            float aspectRatio = (1.0f * image.Width) / image.Height;
            int width = image.Width;
            int height = image.Height;
            image.Dispose();
            return CreateBasicImageNode(imageAddress, aspectRatio, lastWriteTime,
                width, height, validate);
        }

        public XIRImage CreateBasicImageNode(string address, float aspectRatio, 
            DateTime lastModifiedDate, int width, int height, bool validate)
        {
            XIRImage image = XIR.Instance.GetXIRImageByAddress(address);
            bool imageTagIsValid = (image != null) ? ImageStillValid(address) : false;
            if (image != null && !imageTagIsValid)
            {
                //((XmlElement)xir.GetElementsByTagName("images")[0]).RemoveChild(imageElement);
                //imageElement = null;
            }
            if (image == null || !validate)
            {
                while (XIR.Instance.Keys.Contains(imageIdIndex))
                {
                    imageIdIndex++;
                }
                image =  new XIRImage(imageIdIndex, aspectRatio, address, 
                    lastModifiedDate.ToString(), new Dimensions(width, height));
                //XIR.Instance.Add(image.id, image);
                imageIdIndex++;
                
            }
            return image;
        }

        public bool ImageStillValid(string address)
        {
            XIRImage image = XIR.Instance.GetXIRImageByAddress(address);
            if (image == null)
                throw new Exception("Selected image has not been analyzed yet!");
            string lastModifiedTime1 = File.GetLastWriteTime(address).ToString();
            return lastModifiedTime1.CompareTo(image.modified) == 0;
        }  

        public int ImageCellToRegionDifference(string masterColorArray, int rowStride, int x, int y, 
            int width, int height, string cellColorArray, bool compareAsGrayscale)
        {
            int index;
            int totalDifference = 0;
            int masterColorArraySize = masterColorArray.Length / 6;
            int height1 = masterColorArraySize / rowStride;

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    index = 6 * ((i + x) + ((y + j) * rowStride));
                    Color masterColor = XmlHelper.RGBStringToColor(masterColorArray.Substring(index, 6));

                    HSL tmp1 = (HSL)masterColor;
                    Color tmp2 = tmp1.ToColor();
                    if(tmp2 != masterColor)
                    {
                        tmp1.H += 0;
                    }

                    index = 6 * (i + (j * width));
                    Color cellColor = XmlHelper.RGBStringToColor(cellColorArray.Substring(index, 6));
                    ColorProfile colorProfile = null;
                    switch (CurrentColorProfile)
                    {
                        case EColorProfile.RGB:
                            colorProfile = new RGB(cellColor);
                            break;
                        case EColorProfile.HSV:
                            colorProfile = new HSV(cellColor);
                            break;
                        case EColorProfile.HSL:
                            colorProfile = new HSL(cellColor);
                            break;
                        default:
                            throw new Exception("Unhandled color mode.");
                    }
                    //ColorProfile colorProfile = ColorProfileUtils.FromColor<T>(cellColor);
                    totalDifference += (int)Math.Round(colorProfile.GetTotalDiffFromColor(masterColor), 0);
                }
            }
            return totalDifference;
        }



        private void FindImageCellMatchLocations(object parameter)
        {

            MosaicInputs mosaicInputs = (MosaicInputs)parameter;
            int numImages, totalImages = XIR.Instance.Count;
            XIRImage image = XIR.Instance[mosaicInputs.ImageId];

            //Setting up the solution set
            solutionSet = new SortedList<float, int>[mosaicInputs.Columns * mosaicInputs.Columns];
            XIR.Instance.AverageGrayDiffArray = new int[mosaicInputs.Columns * mosaicInputs.Columns];
            for (int i = 0; i < solutionSet.Length; i++)
            {
                XIR.Instance.AverageGrayDiffArray[i] = 0;
                solutionSet[i] = new SortedList<float, int>();
                solutionSet[i].Capacity = mosaicInputs.ImagesToKeep;
            }
            xCells = mosaicInputs.Columns / mosaicInputs.RegionWidth;
            ///TODO: fix the yCells calculation if we use any other cell count other than s^2 
            yCells = mosaicInputs.Columns / mosaicInputs.RegionHeight;

            int iterCount = 3;
            int stepSize = 2;
            for (int l = 0; l < iterCount; ++l)
            {
                HSV.weights[2] = stepSize * l + 1.0f;
                for (int k = 0; k < iterCount; ++k)
                {
                    HSV.weights[1] = stepSize * k + 1.0f;
                    for (int j = 0; j < iterCount; ++j)
                    {
                        HSV.weights[0] = stepSize * j + 1.0f;

                        if (j == k && k == l && j > 0)
                        {
                            continue;
                        }

                        Invoke((ThreadStart)delegate
                        {
                            progressForm.progressBar1.Minimum = 0;
                            progressForm.progressBar1.Maximum = XIR.Instance.Count;
                            progressForm.progressBar1.Value = 0;
                            progressForm.label1.Text = "Calculating Solution";
                            progressForm.Visible = true;
                        });

                        //building our image arrays
                        //int imagesPerThread = totalImages / mosaicInputs.NumThreads;
                        //int index = 0;
                        //XIRImage[][] imagesArray = new XIRImage[mosaicInputs.NumThreads][];
                        //Thread[] solveMosaicThreads = new Thread[mosaicInputs.NumThreads];
                        //for (int thread = 0; thread < mosaicInputs.NumThreads; thread++)
                        //{
                        //    numImages = (thread == 0) ? imagesPerThread + (totalImages % mosaicInputs.NumThreads) : imagesPerThread;
                        //    imagesArray[thread] = new XIRImage[numImages];

                        //    for (int i = numImages - 1; i >= 0; i--)
                        //    {
                        //        int key = XIR.Instance.Keys.ElementAt(index++);
                        //        imagesArray[thread][i] = XIR.Instance[key];
                        //    }
                        //    solveMosaicThreads[thread] = new Thread(SolveMosaic);
                        //    SolveMosaicInputs inputs = new SolveMosaicInputs();
                        //    inputs.image = image;
                        //    inputs.images = imagesArray[thread];
                        //    inputs.regionAspectRatio = mosaicInputs.AspectRatio;
                        //    inputs.columns = mosaicInputs.Columns;
                        //    inputs.regionCellWidth = mosaicInputs.RegionWidth;
                        //    inputs.regionCellHeight = mosaicInputs.RegionHeight;
                        //    inputs.numMatchesToReturn = mosaicInputs.ImagesToKeep;
                        //    inputs.isGrayscale = mosaicInputs.Grayscale;

                        //    solveMosaicThreads[thread].Start(inputs);
                        //}
                        //int doneCount;

                        //do
                        //{
                        //    doneCount = 0;
                        //    for (int i = 0; i < solveMosaicThreads.Length; i++)
                        //    {
                        //        if (solveMosaicThreads[i].ThreadState == ThreadState.Aborted
                        //            || solveMosaicThreads[i].ThreadState == ThreadState.Stopped)
                        //        {
                        //            doneCount++;
                        //        }

                        //        //Let the main thread sleep so we don't waste cycles.
                        //        Thread.Sleep(50);
                        //    }
                        //} while (doneCount != solveMosaicThreads.Length);

                        //building our image arrays


                        SolveMosaicInputs inputs = new SolveMosaicInputs();
                        inputs.image = image;
                        inputs.images = XIR.Instance.Values.ToArray();
                        inputs.regionAspectRatio = mosaicInputs.AspectRatio;
                        inputs.columns = mosaicInputs.Columns;
                        inputs.regionCellWidth = mosaicInputs.RegionWidth;
                        inputs.regionCellHeight = mosaicInputs.RegionHeight;
                        inputs.numMatchesToReturn = mosaicInputs.ImagesToKeep;
                        inputs.isGrayscale = mosaicInputs.Grayscale;

                        SolveMosaic(inputs);



                        //Rendering the images to the screen.
                        RenderMosaicPreview(mosaicInputs.Grayscale);


                        /// NOTE: everything below this should get deleted when done tuning parameters.

                        Invoke((ThreadStart)delegate
                        {
                            progressForm.progressBar1.Value = 0;
                        });

                        

                        OutputOptions options = new OutputOptions();
                        options.width = (settingsForm != null) ? settingsForm.outputWidth : (int)Math.Round(2000 * 1.0f);
                        options.height = (settingsForm != null) ? settingsForm.outputHeight : 2000;
                        options.grayscale = grayscaleMosaic;
                        String values = "";
                        for (int x = 0; x < 3; ++x)
                        {
                            values += $"_{HSV.weights[x].ToString("00")}";
                        }
                        options.address = $"G:\\icons\\output\\pizzaMan_HSV{values}.bmp";

                        //Thread saveThread = new Thread(GenerateAndSaveSolutionImage);
                        //saveThread.IsBackground = true;
                        //saveThread.Start((object)options);

                        Image outputImage = FilterHelper.GenerateSolutionImage(solutionSet, xCells, yCells,
                        new Dimensions(options.width, options.height), options.grayscale,
                        progressForm.progressBar1);
                        outputImage.Save(options.address);
                        outputImage.Dispose();


                        //GenerateAndSaveSolutionImage(options);
                        for (int i = 0; i < solutionSet.Length; i++)
                        {
                            XIR.Instance.AverageGrayDiffArray[i] = 0;
                            solutionSet[i] = new SortedList<float, int>();
                            solutionSet[i].Capacity = mosaicInputs.ImagesToKeep;
                        }
                    }
                }
            }

            //Update the XIR file with any new info
            //XIR.Instance.Save();
        }

        private void SolveMosaic(Object inputs)
        {
            SolveMosaicInputs msi = (SolveMosaicInputs)inputs;
            SolveMosaic(msi.image, msi.images, msi.regionAspectRatio, msi.columns, 
                msi.regionCellWidth, msi.regionCellHeight, msi.numMatchesToReturn, msi.isGrayscale);
        }

        /// <summary>
        /// This function is used to find the best matches for a specific master image cell set.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="images"></param>
        /// <param name="regionAspectRatio"></param>
        /// <param name="columns"></param>
        /// <param name="regionCellWidth"></param>
        /// <param name="regionCellHeight"></param>
        /// <param name="numMatchesToReturn"></param>
        /// <param name="isGrayscale"></param>
        private void SolveMosaic(XIRImage image, XIRImage[] images, float regionAspectRatio, int columns, 
           int regionCellWidth, int regionCellHeight, int numMatchesToReturn, bool isGrayscale)
        { 
            float cellAspectRatio;
            int miWidthInCells, miHeightInCells, ciWidthInCells, ciHeightInCells, width, height;
            String masterColorArray;
            XIRPixelationTechnique pixTech;
            Dimensions dimensions;

            // Handling a problem that occurs when the master image is to small.
            image = XIR.Instance.GetXIRImageByAddress(masterImageAddress);
            dimensions = image.dimensions;
            width = dimensions.width;
            height = dimensions.height;
            cellAspectRatio = image.aspectRatio;
            int masterId = image.id;

            pixTech = image.GetMatchingPixelationTechnique(regionAspectRatio, columns);
            if (pixTech == null)
            {
                doPixelationAnalysis(image.address, columns, regionAspectRatio);
                image = XIR.Instance.GetXIRImageByAddress(image.address);
                pixTech = image.GetMatchingPixelationTechnique(regionAspectRatio, columns);
            }
            miWidthInCells = columns;
            cellAspectRatio = image.aspectRatio;
            miHeightInCells = columns;
            masterColorArray = pixTech.colorArray;

            #region CUDA variables
            List<string> candidateImages = new List<string>();
            List<int> candidateIDs = new List<int>();
            #endregion 

            TimeSpan span = TimeSpan.Zero;

            // Checking the quality of each match.
            foreach (XIRImage image1 in images)
            {
                Invoke((ThreadStart)delegate
                {
                    if (progressForm.progressBar1.Value < progressForm.progressBar1.Maximum)
                    {
                        progressForm.progressBar1.Value++;
                    }
                });

                image = image1;
                int cellId = image.id;
                if (cellId != masterId)
                {
                    cellAspectRatio = image.aspectRatio;
                    dimensions = image.dimensions;
                    width = dimensions.width;
                    pixTech = image.GetMatchingPixelationTechnique(cellAspectRatio, subdivLevel);
                    float aspectDiff = Math.Abs(regionAspectRatio - cellAspectRatio);
                    if (aspectDiff < 0.005f && pixTech == null)
                    {
                        doPixelationAnalysis(image.address, subdivLevel, cellAspectRatio);
                        //image = XIR.Instance.GetXIRImageByAddress(image.address);
                        pixTech = image.GetMatchingPixelationTechnique(regionAspectRatio, subdivLevel);
                    }
                    if (aspectDiff < 0.005f && pixTech != null)
                    {
                        width = image.dimensions.width;
                        height = image.dimensions.height;
                        ciWidthInCells = subdivLevel;//(int)Math.Round((1.0f * width) / subdivLevel);
                        ciHeightInCells = subdivLevel;// (int)Math.Round(height / (cellWidth / cellAspectRatio));
                        if (ciWidthInCells == regionCellWidth && ciHeightInCells == regionCellHeight)
                        {
                            #region CUDA variable setup
                            if (useCUDA)
                            {
                                candidateImages.Add(pixTech.colorArray);
                                candidateIDs.Add(cellId);
                            }
                            #endregion
                            
                            #region CPU based cell fitness calculation
                            // Checking every cell location for its fit.
                            if (!useCUDA)
                            {
                                for (int y = 0; y < miHeightInCells && isRunning; y += regionCellHeight)
                                {
                                    for (int x = 0; x < miWidthInCells && isRunning; x += regionCellWidth)
                                    {
                                        DateTime start = DateTime.Now;
                                        int diff = ImageCellToRegionDifference(masterColorArray, miWidthInCells,
                                            x, y, regionCellWidth, regionCellHeight, pixTech.colorArray, isGrayscale);
                                        span += DateTime.Now - start;

                                        //TEST TEST TEST TEST TEST TEST TEST TEST TEST
                                        //string colorArray = "";
                                        //int index;
                                        //for (int i = 0; i < regionCellWidth; i++)
                                        //{
                                        //    for (int j = 0; j < regionCellHeight; j++)
                                        //    {
                                        //        index = 6 * ((i + x) + ((y + j) * miWidthInCells));
                                        //        colorArray += masterColorArray.Substring(index, 6);
                                        //    }
                                        //}
                                        //XIRToneMap tmpToneMap1 = new XIRToneMap(colorArray, subdivLevel, regionAspectRatio);
                                        //XIRToneMap tmpToneMap2 = new XIRToneMap(pixTech);
                                        //int diff2 = tmpToneMap1.CalculateDifference(tmpToneMap2);
                                        //diff = diff2;// Math.Min(diff, diff2);
                                        //TEST TEST TEST TEST TEST TEST TEST TEST TEST

                                        int tmpX = x / regionCellWidth;
                                        int tmpY = y / regionCellHeight;
                                        lock (((ICollection)solutionSet[tmpY * xCells + tmpX]).SyncRoot)
                                        {
                                            //XIR.Instance.AverageGrayDiffArray[tmpY * xCells + tmpX] = (int)tmpToneMap2.AverageGray - (int)tmpToneMap1.AverageGray;

                                            AddToSolution(solutionSet[tmpY * xCells + tmpX], cellId, diff);
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
            }

            
            Console.WriteLine(span.ToString());

            #region CUDA calculate solution
            //DANGER DANGER THIS CAN BLUE SCREEN THE COMPUTER!
            if (useCUDA)
            {
                int[] solutionData = Cuda.CudaProcessor.CUDA_SolveMosaic(masterColorArray, candidateImages, subdivLevel, miWidthInCells / subdivLevel);
                for (int x = miWidthInCells / subdivLevel - 1; x >= 0; x--)
                {
                    for (int y = miWidthInCells / subdivLevel - 1; y >= 0; y--)
                    {
                        float minValue = float.MaxValue;
                        int tmpIndex, id = 0, minIndex = 0;
                        for (int n = 0; n < candidateImages.Count; n++)
                        {
                            tmpIndex = (((y * (miWidthInCells / subdivLevel)) + x) * candidateImages.Count) + n;
                            if (solutionData[tmpIndex] < minValue)
                            {
                                minValue = solutionData[tmpIndex];
                                minIndex = n;
                                id = candidateIDs[n];
                            }
                        }
                        AddToSolution(solutionSet[(y * (miWidthInCells / subdivLevel)) + x], id, minValue);
                    }
                }
            }
            #endregion

        }

        private void RenderMosaicPreview(bool isGrayscale)
        {
            Invoke((ThreadStart)delegate
            {
                progressForm.label1.Text = "Rendering Solution Preview";
                progressForm.progressBar1.Minimum = 0;
                progressForm.progressBar1.Maximum = xCells * yCells;
                progressForm.progressBar1.Value = 0;
            });
            Invoke((ThreadStart)delegate
           {
               pictureBox1.Image = FilterHelper.GenerateSolutionImage(solutionSet,
                       xCells, yCells, new Dimensions(pictureBox1.Width, pictureBox1.Height),
                       isGrayscale, progressForm.progressBar1);
           });
            Invoke((ThreadStart)delegate
            {
                progressForm.Visible = false;
            });
        }

        private void AddToSolution(SortedList<float, int> solution, int cellId, float diff)
        {
            float value = 1.0f * diff;

            while (solution.ContainsKey(value))
            {
                value += 0.01f;
            }
            solution.Add(value, cellId);
        }        

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadXIR();
        }

        private void scpButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Images (*.bmp;*.jpg;*.jpeg;*.png;*.gif)|*.bmp;*.jpg;*.jpeg;*.png;*.gif";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                candidateImagePool = ofd.FileNames;
                ProgressBar pb = progressForm.progressBar1;
                pb.Maximum = candidateImagePool.Length;
                pb.Minimum = 0;
                pb.Value = 0;
                progressForm.label1.Text = "Adding images to candidate pool.";
                progressForm.Visible = true;
                bool anyImagesAdded = false;
                
                Invoke((ThreadStart)delegate
                {
                    for (int i = 0; i < candidateImagePool.Length; i++)
                    {
                        anyImagesAdded = AddImageToLibraryIfNew(candidateImagePool[i]) || anyImagesAdded;
                        pb.Value++;
                    }
                });
                if (anyImagesAdded)
                {
                    XIR.Instance.Save();
                }
                progressForm.Visible = false;
                UpdateStatusStrip(masterImageAddress);
                createMosaicButton.Enabled = validCandidateImages > 1;
            }
            
        }

        private bool AddImageToLibraryIfNew(string address)
        {
            XIRImage image = XIR.Instance.GetXIRImageByAddress(address);
            if (image == null)
            {
                image = CreateBasicImageNode(address, false);
                XIR.Instance.Add(image.id, image);
                return true;
            }
            return false;
        }

        private void GenerateAndSaveSolutionImage(object parameters)
        {
                OutputOptions options = (OutputOptions)parameters;
                Invoke((ThreadStart)delegate
                {
                    progressForm.label1.Text = "Rendering Final Output Mosaic";
                    progressForm.progressBar1.Minimum = 0;
                    progressForm.progressBar1.Maximum = xCells * yCells;
                    progressForm.progressBar1.Value = 0;
                    progressForm.Visible = true;
                });
                Invoke((ThreadStart)delegate
                {
                    FilterHelper.GenerateSolutionImage(solutionSet, xCells, yCells,
                        new Dimensions(options.width, options.height), options.grayscale,
                        progressForm.progressBar1).Save(options.address);
                });
                //FilterHelper.GenerateSolutionImage(solutionSet, xCells, yCells,
                //        new Dimensions(options.width, options.height), options.grayscale,
                //        progressForm.progressBar1).Save(options.address);

                Invoke((ThreadStart)delegate
                {
                    progressForm.Visible = false;
                });
        }

        private void advancedOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (settingsForm == null)
            {
                settingsForm = new SettingsForm();
            }
            if (!settingsForm.outputWidthTextBox.Enabled)
            {
                UpdateOutputDimensions();
            }
            if (masterImageAddress != null)
            {
                float aspectRatio = XIR.Instance.GetXIRImageByAddress(masterImageAddress).aspectRatio;
                settingsForm.aspectRatio = aspectRatio;
            }
            settingsForm.Visible = true;
        }

        private void UpdateOutputDimensions()
        {
            if  (masterImageAddress != null && settingsForm != null)
                {
                    Bitmap tmp = new Bitmap(masterImageAddress, true);
                    int w = tmp.Width;
                    int h = tmp.Height;
                    settingsForm.InitializeOutputFields(w, h);
                    tmp.Dispose();
                }
        }

        private void CreateProgressBarForm()
        {
            if (progressForm == null)
            {
                progressForm = new ProgressBarForm();
                UpdateProgressFormLocation();
            }
            progressForm.Visible = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (settingsForm != null)
            {
                settingsForm.Dispose();
            }
            if (progressForm != null)
            {
                progressForm.Dispose();
            }
            if (aboutForm != null)
            {
                aboutForm.Dispose();
            }
            isRunning = false;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Images (*.bmp)|*.bmp;*.jpg;*.jpeg;*.png;*.gif";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                float aspectRatio = XIR.Instance.GetXIRImageByAddress(masterImageAddress).aspectRatio;
                Thread saveThread = new Thread(GenerateAndSaveSolutionImage);
                OutputOptions options = new OutputOptions();
                options.width = (settingsForm != null) ? settingsForm.outputWidth : (int)Math.Round(2000 * aspectRatio);
                options.height = (settingsForm != null) ? settingsForm.outputHeight : 2000;
                options.grayscale = grayscaleMosaic;
                options.address = sfd.FileName;

                saveThread.IsBackground = true;
                saveThread.Start((object)options);
            }
            sfd.Dispose();
        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void maskedTextBox2_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UpdateProgressFormLocation(object sender, DragEventArgs e)
        {
            UpdateProgressFormLocation();
        }

        private void UpdateProgressFormLocation()
        {
            if (progressForm != null)
            {
                progressForm.Left = 15 + this.Left;
                progressForm.Top = this.Top + pictureBox1.Top + (pictureBox1.Height / 2) - (progressForm.Height / 2);
                progressForm.Focus();
            }
        }

        private void maskedTextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                imagesPerColumn = int.Parse(imagesPerColumnTextBox1.Text);
            }
            catch (FormatException)
            {

            }
        }

        private void maskedTextBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                subdivLevel = int.Parse(subdivisionLevelTextBox2.Text);
            }
            catch (FormatException)
            {

            }
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            UpdateProgressFormLocation();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (progressForm != null && progressForm.Visible == true)
            {
                UpdateProgressFormLocation();
            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            if (progressForm != null && progressForm.Visible == true)
            {
                UpdateProgressFormLocation();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (aboutForm == null)
            {
                aboutForm = new AboutForm();                
            }
            aboutForm.Visible = true;
        }

        private void CUDA_CheckedChanged(object sender, EventArgs e)
        {
            useCUDA = !useCUDA;
        }

    }   
}
