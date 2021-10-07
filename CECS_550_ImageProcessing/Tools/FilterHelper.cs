using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;
using MosaicMason.DataTypes;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace MosaicMason.Filters
{
    public static class FilterHelper
    {
        public static int RedChannelDiff(Color color1, Color color2, bool absoluteValue)
        {
            return (absoluteValue) ? Math.Abs(color2.R - color1.R) : color2.R - color1.R;
        }

        public static int GreenChannelDiff(Color color1, Color color2, bool absoluteValue)
        {
            return (absoluteValue) ? Math.Abs(color2.G - color1.G) : color2.G - color1.G;
        }

        public static int BlueChannelDiff(Color color1, Color color2, bool absoluteValue)
        {
            return (absoluteValue) ? Math.Abs(color2.B - color1.B) : color2.B - color1.B;
        }

        public static int TotalColorDiff(Color color1, Color color2)
        {
            return TotalColorDiff(color1, color2, false);
        }

        public static int TotalColorDiff(Color color1, Color color2, bool compareAsGrayscale)
        {
            if (!compareAsGrayscale)
            {
                return RedChannelDiff(color1, color2, true)
                    + GreenChannelDiff(color1, color2, true)
                    + BlueChannelDiff(color1, color2, true);
            }
            else
            {
                color1 = Color.FromArgb((byte)((color1.R + color1.G + color1.B) / 3), 0, 0);
                color2 = Color.FromArgb((byte)((color2.R + color2.G + color2.B) / 3), 0, 0);
                return RedChannelDiff(color1, color2, true);
            }
        }

        public static Color GetRegionAverage(Bitmap image, double startX, double startY, double width, double height)
        {
            double avgR = 0,
                  avgG = 0,
                  avgB = 0;
            Color pixelColor;
           
            int x1 = (int)startX;
            double tmpX = startX + width;
            int x2 = (tmpX % 1 > 0 && tmpX + 1 < image.Width) ? (int)(tmpX + 1) : (int)tmpX;
            int y1 = (int)startY;
            double tmpY = startY + height;
            int y2 = (tmpY % 1 > 0 && tmpY + 1 < image.Height) ? (int)(tmpY + 1) : (int)tmpY;
            double numOfPixels = 0;
            for (int y = y1; y < y2; y++)
            {
                for (int x = x1; x < x2; x++)
                {
                    double factor = 1;
                    if (x == x1)
                    {
                        factor = 1 - (startX % 1);
                    }
                    else if (x == x2)
                    {
                        factor = (startX + width) % 1;
                    }
                    pixelColor = image.GetPixel(x, y);
                    avgR += pixelColor.R * factor;
                    avgG += pixelColor.G * factor;
                    avgB += pixelColor.B * factor;
                    numOfPixels += factor;
                }
            }
            if (numOfPixels > 0)
            {
                avgR = Math.Min(avgR / numOfPixels, 255);
                avgG = Math.Min(avgG / numOfPixels, 255);
                avgB = Math.Min(avgB / numOfPixels, 255);
            }
            return Color.FromArgb((int)Math.Round(avgR), (int)Math.Round(avgG), (int)Math.Round(avgB));
        }

        public static Bitmap GenerateSolutionImage(SortedList<float, int>[] solutionSet, 
            int xCells, int yCells, Dimensions outputDimensions, bool isGrayscale, ProgressBar progressBar)
        {
            Bitmap output = new Bitmap(outputDimensions.width, outputDimensions.height);
            Bitmap image;
            int width = outputDimensions.width;
            int height = outputDimensions.height;
            float boxX = (1.0f * width) / xCells;
            float boxY = (1.0f * height) / yCells;
            float xLoc = 0;
            float yLoc = 0;
            int id, imageMatchId, xActual, yActual;
            IList<int> SortedImageIds;
            List<Point> renderedLocations = new List<Point>();
            List<Point> tmpLocs;

            int currWidth, currHeight;
            for (int x = 0; x < xCells; x++)
            {
                for (int y = 0; y < yCells; y++)
                {
                    if (!PointListContains(renderedLocations, new Point(x, y)))
                    {
                        id = 0;
                        SortedImageIds = solutionSet[x * yCells + y].Values;
                        do
                        {
                            imageMatchId = SortedImageIds[id++];
                            image = XIR.Instance.GetBitmapFromImageId(imageMatchId);
                        } while (image == null);
                        xLoc = boxX * x;
                        yLoc = boxY * y;
                        xActual = (int)Math.Round(xLoc);
                        yActual = (int)Math.Round(yLoc);
                        currWidth = (xLoc + boxX >= width) ? width - xActual - 1 : (int)Math.Round(boxX + xLoc) - xActual;
                        currHeight = (yLoc + boxY >= height) ? height - yActual - 1 : (int)Math.Round(boxY + yLoc) - yActual;
                        image = new Bitmap(image, new Size(currWidth, currHeight));
                        image = (isGrayscale) ? ConvertImageToGrayscale(image) : image;
                        tmpLocs = FilterHelper.CopyImageToAllLocations(solutionSet, renderedLocations, image, 
                            new Point(x, y), xCells, yCells, outputDimensions, XIR.Instance[imageMatchId], output);
                        renderedLocations.AddRange(tmpLocs);
                        image.Dispose();
                        progressBar.Invoke((ThreadStart)delegate
                        {
                            progressBar.Value += (tmpLocs.Count + progressBar.Value <= progressBar.Maximum) ? tmpLocs.Count : 0;
                        });
                    }
                    
                }
            }
            return output;
        }

        public static List<Point> CopyImageToAllLocations(SortedList<float, int>[] solutionSet, 
            List<Point> alreadyRendered, Bitmap bmp, Point start, int xCells, int yCells, 
            Dimensions outputDimensions, XIRImage image, Bitmap destination)
        {
            int currentID, targetID = image.id;
            int width = outputDimensions.width;
            int height = outputDimensions.height;
            float boxX = (1.0f * width) / xCells;
            float boxY = (1.0f * height) / yCells;
            int xActual, yActual;
            Size currSize = new Size();
            float xLoc, yLoc;
            List<Point> renderedLocations = new List<Point>(0);
            int avgGrayDiff;
            for (int x = start.X; x < xCells; x++)
            {
                for (int y = start.Y; y < yCells; y++)
                {
                    currentID = solutionSet[x * yCells + y].Values[0];
                    avgGrayDiff = XIR.Instance.AverageGrayDiffArray[x * yCells + y];
                    if (targetID == currentID && !alreadyRendered.Contains(new Point(x,y)))
                    {
                        xLoc = boxX * x;
                        yLoc = boxY * y;
                        xActual = (int)Math.Round(xLoc);
                        yActual = (int)Math.Round(yLoc);
                        currSize.Width = (xLoc + boxX >= width) ? width - xActual - 1 : (int)Math.Round(boxX + xLoc) - xActual;
                        currSize.Height = (yLoc + boxY >= height) ? height - yActual - 1 : (int)Math.Round(boxY + yLoc) - yActual;
                        if (bmp.Size == currSize)
                        {
                            CopyImageInto(destination, bmp, xActual, yActual, avgGrayDiff);
                            renderedLocations.Add(new Point(x, y));
                        }
                    }
                }
            }
            return renderedLocations;
        }

        public static void CopyImageInto(Bitmap destination, Bitmap source, int xLoc, int yLoc, int avgGrayDiff)
        {
            Color c;
            int r, g, b;
            for (int i = source.Width - 1; i >= 0; i--)
            {
                for (int j = source.Height - 1; j >= 0; j--)
                {
                    c = source.GetPixel(i, j);
                    r = c.R - avgGrayDiff;
                    r = (r > 0) ? Math.Min(r, 255) : 0;
                    g = c.G - avgGrayDiff;
                    g = (g > 0) ? Math.Min(g, 255) : 0;
                    b = c.B - avgGrayDiff;
                    b = (b > 0) ? Math.Min(b, 255) : 0;
                    destination.SetPixel(xLoc + i, yLoc + j, Color.FromArgb(r,g,b));
                }
            }
        }

        public static Bitmap ConvertImageToGrayscale(Bitmap image)
        {
            byte val;
            Color color;
            for (int y = image.Height - 1; y >= 0; y--)
            {
                for (int x = image.Width - 1; x >= 0; x--)
                {
                    color = image.GetPixel(x, y);
                    val = (byte)((color.R + color.G + color.B) / 3);
                    image.SetPixel(x, y, Color.FromArgb(val, val, val));
                }
            }
            return image;
        }

        private static bool PointListContains(List<Point> pointList, Point point)
        {
            foreach (Point tmpPoint in pointList)
            {
                if (tmpPoint.X == point.X && tmpPoint.Y == point.Y)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
