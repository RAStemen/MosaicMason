using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using CECS_550_ImageProcessing.DataTypes;
using System.IO;

namespace CECS_550_ImageProcessing
{
    static class XIRTools
    {
        public static int ImagesWithSameAspectRatioCount(float aspectRatio, String directory)
        {
            XmlNodeList images = XIR.Instance.XML.GetElementsByTagName("image");
            float aspectRatio1;
            int count = 0;

            for (int i = images.Count - 1; i >= 0; i--)
            {
                aspectRatio1 = float.Parse(((XmlElement)images[i]).GetAttribute("aspectRatio"));

                //We allow a small amount of error because float.Parse() is not 100% accurate
                if (Math.Abs(aspectRatio - aspectRatio1) < 0.005f)
                {
                    count++;
                }
            }
            return count;
        }

        public static Size CalculateNewSize(Size maxDimensions, Size currentDimensions)
        {
            float maxAspectRatio = (1.0f * maxDimensions.Width) / maxDimensions.Height;
            float currAspectRatio = (1.0f * currentDimensions.Width) / currentDimensions.Height;
            Size toReturn = new Size();
            if (currAspectRatio < maxAspectRatio)
            {
                toReturn.Height = maxDimensions.Height;
                toReturn.Width = (int)Math.Round(toReturn.Height * currAspectRatio);
            }
            else
            {
                toReturn.Width = maxDimensions.Width;
                toReturn.Height = (int)Math.Round(toReturn.Width / currAspectRatio);
            }
            return toReturn;
        }
    }

        
}
