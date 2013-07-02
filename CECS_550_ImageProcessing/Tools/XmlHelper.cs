using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CECS_550_ImageProcessing
{
    public static class XmlHelper
    {
        public static string ColorToRGBString(Color color)
        {
            int argb = color.ToArgb();
            int r = (argb >> 16) & 0xff;
            int g = (argb >> 8) & 0xff;
            int b = argb & 0xff;
            return r.ToString("X").PadLeft(2, '0') + g.ToString("X").PadLeft(2, '0') + b.ToString("X").PadLeft(2, '0');
        }

        public static Color RGBStringToColor(string rgb)
        {
            int r = int.Parse(rgb.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            int g = int.Parse(rgb.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            int b = int.Parse(rgb.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return Color.FromArgb(r, g, b);
        }

        public static int[] RGBStringToIntArray(string rgb)
        {
            int r = int.Parse(rgb.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            int g = int.Parse(rgb.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            int b = int.Parse(rgb.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new int[]{r, g, b};
        }
    }
}
