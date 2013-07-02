using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;
using CECS_550_ImageProcessing.DataTypes;
using System.Drawing.Imaging;

namespace CECS_550_ImageProcessing.Filters
{
    public static class PixelationFilter
    {
        public static XIRPixelationTechnique Filter(Bitmap image, int subdivisionLevel, float aspectRatio)
        {
            int width = image.Width;
            int height = image.Height;
            double boxX = (1.0 * width) / subdivisionLevel;
            double boxY = (1.0 * height) / subdivisionLevel;
            double tempWidth;
            double tempHeight;
            double xLoc, yLoc;
            Color newColor;
            StringBuilder colorArray = new StringBuilder();
            Bitmap icon = Scale(image, subdivisionLevel, subdivisionLevel);//new Bitmap(image, subdivisionLevel, subdivisionLevel);
            //int accumulatedError = 0;
            for (int x = 0; x < subdivisionLevel; x++)
            {
                for (int y = 0; y < subdivisionLevel; y++)
                {
                    xLoc = boxX * x;
                    yLoc = boxY * y;
                    tempWidth = (xLoc + boxX >= width) ? width - xLoc - 1 : boxX;
                    tempHeight = (yLoc + boxY >= height) ? height - yLoc - 1 : boxY;
                    newColor = icon.GetPixel(x, y);
                    //Color color1 = FilterHelper.GetRegionAverage(image, xLoc, yLoc, tempWidth, tempHeight);
                    //accumulatedError += Math.Abs(color1.R - newColor.R) + Math.Abs(color1.G - newColor.G) + Math.Abs(color1.B - newColor.B);
                    colorArray.Append(XmlHelper.ColorToRGBString(newColor));
                }
            }
            icon.Dispose();
            return new XIRPixelationTechnique( subdivisionLevel, aspectRatio, colorArray.ToString() );
        }

        public static Bitmap Scale(Bitmap Bitmap, int width, int height)
        {
            Bitmap scaledBitmap = new Bitmap(width, height);

            // Scale the bitmap in high quality mode.
            using (Graphics gr = Graphics.FromImage(scaledBitmap))
            {
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                gr.DrawImage(Bitmap, new Rectangle(0, 0, width, height), new Rectangle(0, 0, Bitmap.Width, Bitmap.Height), GraphicsUnit.Pixel);
            }

            // Copy original Bitmap's EXIF tags to new bitmap.
            foreach (PropertyItem propertyItem in Bitmap.PropertyItems)
            {
                scaledBitmap.SetPropertyItem(propertyItem);
            }

            return scaledBitmap;
        }
    }
}
