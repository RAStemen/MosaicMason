using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CECS_550_ImageProcessing.Profiles
{
    public class RGB
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public RGB(Color color)
        {
            R = color.R;
            G = color.G;
            B = color.B;
            A = color.A;
        }

        public Color ToColor()
        {
            return Color.FromArgb(A, R, G, B);
        }

        public static implicit operator Color(RGB x)
        {
            return x.ToColor();
        }

        public static implicit operator RGB(Color x)
        {
            return new RGB(x);
        }
    }
}
