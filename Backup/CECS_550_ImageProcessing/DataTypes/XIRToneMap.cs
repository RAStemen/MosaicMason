using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;

namespace CECS_550_ImageProcessing.DataTypes
{
    class XIRToneMap
    {
        public byte AverageGray { get; private set; }
        public int SubdivisionLevel { private set; get; }
        public float AspectRatio { private set; get; }
        public byte[] ToneMapArray { private set; get; }


        public XIRToneMap(string colorArray, int subdivisionLevel, float aspectRatio)
        {
            SubdivisionLevel = subdivisionLevel;
            AspectRatio = aspectRatio;
            CalculateAverageGray(colorArray);
            BuildToneMapArray(colorArray, AverageGray);
        }

        public XIRToneMap(XIRPixelationTechnique pixTeq)
        {
            SubdivisionLevel = pixTeq.subdivisionLevel;
            AspectRatio = pixTeq.aspectRatio;
            string colors = pixTeq.colorArray;
            CalculateAverageGray(colors);
            BuildToneMapArray(colors, AverageGray);
        }

        private void CalculateAverageGray(string colors)
        {
            Color color;
            long total = 0;
            long count = 0;
            for (int i = 0; i < colors.Length; i += 6)
            {
                color = XmlHelper.RGBStringToColor(colors.Substring(i, 6));
                total += color.R + color.G + color.B;
                count += 3;
            }
            AverageGray = (byte)(total / count);//(colors.Length / 2));
        }

        private void BuildToneMapArray(string colors, int AverageGray)
        {
            Color color;
            StringBuilder toneMapBuilder = new StringBuilder();
            ToneMapArray = new byte[2* (colors.Length / 3)];
            int toneMapIndex = 0;
            int r, g, b;
            for (int i = 0; i < colors.Length; i += 6)
            {
                color = XmlHelper.RGBStringToColor(colors.Substring(i, 6));
                r = color.R - AverageGray;
                g = color.G - AverageGray;
                b = color.B - AverageGray;
                color = Color.FromArgb(Math.Abs(r), Math.Abs(g), Math.Abs(b));
                toneMapBuilder.Append(GetNegativeCode(r, g, b));
                toneMapBuilder.Append(XmlHelper.ColorToRGBString(color));
                ToneMapArray[toneMapIndex++] = (byte)GetNegativeCode(r, g, b);
                ToneMapArray[toneMapIndex++] = (byte)Math.Abs(r);
                ToneMapArray[toneMapIndex++] = (byte)Math.Abs(g);
                ToneMapArray[toneMapIndex++] = (byte)Math.Abs(b);
            }
        }

        public int GetNegativeCode(int r, int g, int b)
        {
            int toReturn = (r < 0)? 4 : 0;
            toReturn += (g < 0) ? 2 : 0;
            toReturn += (b < 0) ? 1 : 0;
            return toReturn;
        }

        public XmlElement toXmlElement()
        {
            XmlElement toneMapElement = XIR.Instance.XML.CreateElement("toneMap");
            toneMapElement.SetAttribute("subdivLevel", SubdivisionLevel.ToString());
            toneMapElement.SetAttribute("aspectRatio", AspectRatio.ToString());
            toneMapElement.SetAttribute("averageGray", AverageGray.ToString());
            XmlElement toneMapArrayElement = XIR.Instance.XML.CreateElement("toneMapArray");
            toneMapArrayElement.InnerText = ToneMapArray.ToString();
            toneMapElement.AppendChild(toneMapArrayElement);
            return toneMapElement;
        }

        public int CalculateDifference(XIRToneMap map)
        {
            if (ToneMapArray.Length != map.ToneMapArray.Length)
            {
                throw new Exception("Both XIRToneMaps must have the save amount of data!");
            }
            int r1, r2, g1, g2, b1, b2, code1, code2, totalDiff = 0;

            for (int i = 0; i < ToneMapArray.Length; i += 4)
            {
                code1 = ToneMapArray[i];
                r1 = ((code1 >> 2 & 0x1) == 1) ? -ToneMapArray[i + 1] : ToneMapArray[i + 1];
                g1 = ((code1 >> 1 & 0x1) == 1) ? -ToneMapArray[i + 2] : ToneMapArray[i + 2];
                b1 = ((code1 & 0x1) == 1) ? -ToneMapArray[i + 3] : ToneMapArray[i + 3];

                code2 = map.ToneMapArray[i];
                r2 = ((code2 >> 2 & 0x1) == 1) ? -map.ToneMapArray[i + 1] : map.ToneMapArray[i + 1];
                g2 = ((code2 >> 1 & 0x1) == 1) ? -map.ToneMapArray[i + 2] : map.ToneMapArray[i + 2];
                b2 = ((code2 & 0x1) == 1) ? -map.ToneMapArray[i + 3] : map.ToneMapArray[i + 3];

                totalDiff += Math.Abs(r2 - r1) + Math.Abs(g2 - g1) + Math.Abs(b2 - b1);
            }
            return totalDiff;
        }
    }
}
