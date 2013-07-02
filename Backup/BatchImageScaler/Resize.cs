using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Drawing.Imaging;

namespace BatchImageScaler
{
    public partial class Resize : Form
    {
        int maxDimension = 640;
        string[] targetImages;
        int numImagesToScale;


        public Resize()
        {
            InitializeComponent();
        }

        private void selectImagesButton_Click(object sender, EventArgs e)
        {
             OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Images (*.bmp;*.jpg;*.jpeg;*.png;*.gif)|*.bmp;*.jpg;*.jpeg;*.png;*.gif";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                targetImages = ofd.FileNames;
                if (targetImages.Length > 0)
                {
                    numImagesToScale = targetImages.Length;
                    progressBar.Maximum = numImagesToScale;
                    resizeButton.Enabled = true;
                }
            }
        }

        private void resizeButton_Click(object sender, EventArgs e)
        {
            progressBar.Value = 0;
            Thread resizeThread = new Thread(DoBatchResize);
            resizeThread.Start();
        }

        private void DoBatchResize()
        {
            int width, height, insertLoc;
            Bitmap image;
            float aspectRatio;
            string address, directory;
            RotateFlipType rotation = RotateFlipType.RotateNoneFlipNone;
            Invoke((ThreadStart)delegate
            {
                rotation = getSelectedRotateFlipType();
            });
            

            //ImageCodecInfo jpegCodec = GetImageEncoderByExtension("JPG");

            for (int i = targetImages.Length - 1; i >= 0; i--)
            {
                address = targetImages[i];
                if (File.Exists(address))
                {
                    image = new Bitmap(targetImages[i], true);
                    aspectRatio = (1.0f * image.Width) / image.Height;

                    if (aspectRatio > 1)
                    {
                        width = maxDimension;
                        height = (int)Math.Round(width / aspectRatio);
                    }
                    else
                    {
                        height = maxDimension;
                        width = (int)Math.Round(height * aspectRatio);
                    }

                    insertLoc = address.LastIndexOf('\\') + 1;
                    directory = address.Substring(0, insertLoc) + "scaled_" + maxDimension + "\\";
                    address = address.Insert(insertLoc, "scaled_" + maxDimension + "\\");
                    address = address.Substring(0,address.LastIndexOf('.')) + ".png";

                   
                    Bitmap image2 = new Bitmap(image, width, height);
                    image.Dispose();
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    image2.RotateFlip(rotation);
                    image2.Save(address, ImageFormat.Jpeg);
                    image2.Dispose();
                    Invoke((ThreadStart)delegate
                    {
                        progressBar.Value++;
                        progressLabel.Text = progressBar.Value + " of " + numImagesToScale;
                    });
                }
            }
        }

        private ImageCodecInfo GetImageEncoderByExtension(string ext){
            ext = ext.ToUpper();
            ImageCodecInfo[] imgCodecs = ImageCodecInfo.GetImageEncoders();
            for (int i = 0; i < imgCodecs.Length; i++)
            {
                if (ext.Contains(imgCodecs[i].FilenameExtension))
                {
                    return imgCodecs[i];
                }
            }
            return null;
        }

        private RotateFlipType getSelectedRotateFlipType()
        {
            if (radioButton1.Checked)
            {
                return RotateFlipType.RotateNoneFlipNone;
            }
            else if (radioButton2.Checked)
            {
                return RotateFlipType.Rotate90FlipNone;
            }
            else if (radioButton3.Checked)
            {
                return RotateFlipType.Rotate180FlipNone;
            }
            else
            {
                return RotateFlipType.Rotate270FlipNone;
            }
        }

        private void maxDimTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                maxDimension = int.Parse(maxDimTextBox.Text);
            }
            catch { }
        }
    }
}
