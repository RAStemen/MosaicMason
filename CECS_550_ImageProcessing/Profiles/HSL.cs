using System;
using System.Drawing;

namespace MosaicMason.Profiles
{
    public class HSL : ColorProfile, IColorProfile
    {
        public float H { get { return Data[0]; } set { Data[0] = value; } }
        public float S { get { return Data[1]; } set { Data[1] = value; } }
        public float L { get { return Data[2]; } set { Data[2] = value; } }
        public float A { get { return Data[3]; } set { Data[3] = value; } }

        public override float this[int index] 
        {
            get => Data[index];
            set => Data[index] = value;
        }

        public HSL(Color color) : base(color)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));
            float chroma = (max - min) / 255.0f;

            H = color.GetHue() / 360.0f;
            L = ((min + max) / 2.0f) / 255.0f;
            S = (L == 0 || L == 1) ? 0 : chroma / (1.0f - Math.Abs(2 * L - 1));
            A = color.A;
        }

        public override Color ToColor()
        {
            float C = (1.0f - Math.Abs(2.0f * L - 1)) * S;

            float scaledH = H * 360.0f;

            float HPrime = scaledH / 60.0f;
            int X = Convert.ToInt32(C * (1.0f - Math.Abs(HPrime % 2 - 1)) * 255);
            int m = Convert.ToInt32((L - C / 2) * 255);

            int c1 = Convert.ToInt32(C * 255) + m;
            int x1 = X + m;
            int alpha = (int)Math.Round(A);
            switch((int)HPrime)
            {
                case 0:
                    return Color.FromArgb(alpha, c1, x1, m);
                case 1:
                    return Color.FromArgb(alpha, x1, c1, m);
                case 2:
                    return Color.FromArgb(alpha, m, c1, x1);
                case 3:
                    return Color.FromArgb(alpha, m, x1, c1);
                case 4:
                    return Color.FromArgb(alpha, x1, m, c1);
                default:
                    return Color.FromArgb(alpha, c1, m, x1);
            }
        }

        public static float[] weights = new float[]{ 2.0f, 1.0f, 1.0f, 0.0f };

        public override float GetChannelDiff(IColorProfile other, int channelIndex)
        {
            float diff = Math.Abs(this[channelIndex] - other[channelIndex]);
            if (channelIndex == 0 && diff > 0.5f)
            {
                diff = 1.0f - diff;
            }

            return diff * weights[channelIndex];
        }

        public override float GetTotalDiffFromColor(Color other)
        {
            return GetTotalDiff((HSV)other);
        }

        public override float GetTotalDiff(IColorProfile other)
        {
            float diff = 0.0f;
            for (int i = 0; i < Data.Length - 1; ++i)
            {
                diff += GetChannelDiff(other, i) * 255;
            }
            return diff;
        }



        public static implicit operator Color(HSL x)
        {
            return x.ToColor();
        }

        public static implicit operator HSL(Color x)
        {
            return new HSL(x);
        }
    }
}
