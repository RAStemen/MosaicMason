using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CECS_550_ImageProcessing.Profiles
{
    public class HSV
    {
        public float H;
        public float S;
        public float V;
        public float A;

        public HSV(Color color)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            H = color.GetHue();
            S = (max == 0.0f) ? 0 : 1.0f - (1.0f * min / max);
            V = max / 255.0f;
            A = color.A;
        }

        public Color ToColor()
        {
            int hi = Convert.ToInt32(Math.Floor(H / 60)) % 6;
            double f = H / 60 - Math.Floor(H / 60);

            V *= 255;
            int v = Convert.ToInt32(V);
            int p = Convert.ToInt32(V * (1 - S));
            int q = Convert.ToInt32(V * (1 - f * S));
            int t = Convert.ToInt32(V * (1 - (1 - f) * S));
            int alpha = (int)Math.Round(A);
            switch(hi)
            {
                case 0:
                    return Color.FromArgb(alpha, v, t, p);
                case 1:
                    return Color.FromArgb(alpha, q, v, p);
                case 2:
                    return Color.FromArgb(alpha, p, v, t);
                case 3:
                    return Color.FromArgb(alpha, p, q, v);
                case 4:
                    return Color.FromArgb(alpha, t, p, v);
                default:
                    return Color.FromArgb(alpha, v, p, q);
            }
        }

        public static implicit operator Color(HSV x)
        {
            return x.ToColor();
        }

        public static implicit operator HSV(Color x)
        {
            return new HSV(x);
        }
    }
}
