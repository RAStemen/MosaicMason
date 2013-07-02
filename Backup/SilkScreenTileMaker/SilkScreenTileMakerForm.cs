using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

namespace SilkScreenTileMaker
{
    public partial class SilkScreenTileMakerForm : Form
    {
        private static byte HIGH_COLOR = 0xFF;
        private static byte LOW_COLOR = 0x00;

        private int subdivisions = 3;
        private int ImageSize = 36;
        private string directory = "";

        public SilkScreenTileMakerForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (subdivisions > 3)
            {
                if (MessageBox.Show("Warning: this will create " + Math.Pow(2,subdivisions * subdivisions) + " image icons.  Proceed?", "Warning!", MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    return;
                }
            }
            try
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.Description = "Select the directory that you wish to create your icons in.";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    directory = fbd.SelectedPath;
                    
                }
                fbd.Dispose();
            }
            catch (ArgumentException err)
            {
                MessageBox.Show("There was an error.  " +
                    "Check the path.\r\nERROR:" + err.Message + " " + err.StackTrace);
            }
            CreateSilkScreenTiles();
        }

        private void CreateSilkScreenTiles()
        {
            int arraySize = subdivisions * subdivisions;
            string appPath = Path.GetDirectoryName(Application.ExecutablePath);
            string name;
            int totalImages = (int)Math.Pow(2, arraySize);
            int fileNameLength = totalImages.ToString().Length + 4;
            for (int i = totalImages - 1; i >= 0; i--)
            {
                byte[] data = IntToBitmap(i, HIGH_COLOR, LOW_COLOR, arraySize);
                name = i + ".png";
                ByteArrayToBitmap(data, subdivisions, ImageSize).Save(directory + "\\" + name.PadLeft(fileNameLength, '0'), ImageFormat.Png);
            }
        }

        private byte[] IntToBitmap(int number, byte highColor, byte lowColor, int arraySize)
        {
            byte[] bitmap = new byte[arraySize];
            int val;
            for (int i = arraySize - 1; i >= 0; i--)
            {
                val = (number >> i) & 01;
                bitmap[i] = (val == 1) ? highColor : lowColor;
            }
            return bitmap;
        }

        private Bitmap ByteArrayToBitmap(byte[] data, int rowStride, int sideLength)
        {
            Bitmap bitmap = new Bitmap(sideLength, sideLength);
            float cellLength = (1.0f * sideLength) / rowStride;
            int xActual, yActual;
            int tempWidth, tempHeight;
            int i = 0, j = 0;
            Color newColor;
            byte color;
            // Loop through the images pixels to reset color.
            for (float x = 0; x < sideLength; x += cellLength)
            {
                j = 0;
                for (float y = 0; y < sideLength; y += cellLength)
                {
                    xActual = (int)Math.Round(x);
                    yActual = (int)Math.Round(y);
                    tempWidth = (x + cellLength >= sideLength) ? sideLength - xActual - 1 : (int)Math.Round(cellLength + x) - xActual;
                    tempHeight = (y + cellLength >= sideLength) ? sideLength - yActual - 1 : (int)Math.Round(cellLength + y) - yActual;
                    if ((tempHeight != 0) && (tempWidth != 0))
                    {
                        color = data[j * rowStride + i];
                        newColor = Color.FromArgb(color, color, color);
                        SetRegionToColor(bitmap, newColor, xActual, yActual, tempWidth, tempHeight);
                    }
                    j++;
                }
                i++;
            }
            return bitmap;
        }

        private void SetRegionToColor(Bitmap image, Color color, int startX, int startY, int width, int height)
        {
            for (int y = startY + height - 1; y >= startY; y--)
            {
                for (int x = startX + width - 1; x >= startX; x--)
                {
                    image.SetPixel(x, y, color);
                }
            }
        }

        private void maskedTextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                ImageSize = Int16.Parse(maskedTextBox1.Text);
            }
            catch (Exception err)
            {
                // we don't really care what we catch here
            }
        }

        private void maskedTextBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                subdivisions = Int16.Parse(maskedTextBox2.Text);
            }
            catch (Exception err)
            {
                // we don't really care what we catch here
            }
        }
    }
}
