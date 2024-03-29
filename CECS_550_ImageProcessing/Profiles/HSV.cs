﻿using System;
using System.Drawing;

namespace MosaicMason.Profiles
{
    public class HSV : ColorProfile, IColorProfile
    {
        public float H { get { return Data[0]; } set { Data[0] = value; } }
        public float S { get { return Data[1]; } set { Data[1] = value; } }
        public float V { get { return Data[2]; } set { Data[2] = value; } }
        public float A { get { return Data[3]; } set { Data[3] = value; } }

        public override float this[int index] 
        {
            get => Data[index];
            set => Data[index] = value;
        }

        public HSV(Color color) : base(color)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            H = color.GetHue() / 360.0f;
            S = (max == 0.0f) ? 0 : 1.0f - (1.0f * min / max);
            V = max / 255.0f;
            A = color.A;
        }

        public override Color ToColor()
        {
            float scaledH = H * 360;
            int hi = Convert.ToInt32(Math.Floor(scaledH / 60)) % 6;
            double f = scaledH / 60 - Math.Floor(scaledH / 60);

            float scaledV = V * 255;
            int v = Convert.ToInt32(scaledV);
            int p = Convert.ToInt32(scaledV * (1 - S));
            int q = Convert.ToInt32(scaledV * (1 - f * S));
            int t = Convert.ToInt32(scaledV * (1 - (1 - f) * S));
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

        public static float[] weights = new float[] { 2.0f, 1.0f, 1.0f, 0.0f };

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
                diff += GetChannelDiff(other, i);
            }
            return diff;
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
