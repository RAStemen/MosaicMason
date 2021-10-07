using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MosaicMason.Profiles
{
    public class RGB : ColorProfile, IColorProfile
    {
        
        public float R { get { return Data[0]; } set { Data[0] = value; } }
        public float G { get { return Data[1]; } set { Data[1] = value; } }
        public float B { get { return Data[2]; } set { Data[2] = value; } }
        public float A { get { return Data[3]; } set { Data[3] = value; } }

        public override float this[int index] 
        { 
            get => Data[index]; 
            set => Data[index] = value; 
        }

        public RGB(Color color) : base(color)
        {
            R = color.R;
            G = color.G;
            B = color.B;
            A = color.A;
        }

        public override Color ToColor()
        {
            byte a =  (byte)Math.Round(A * 255, 0);
            byte r = (byte)Math.Round(R * 255, 0);
            byte g = (byte)Math.Round(G * 255, 0);
            byte b = (byte)Math.Round(B * 255, 0);
            return Color.FromArgb(a, r, g, b);
        }

        public override float GetChannelDiff(IColorProfile other, int channelIndex)
        {
            return Math.Abs(this[channelIndex] - other[channelIndex]);
        }

        public override float GetTotalDiff(IColorProfile other)
        {
            float diff = 0.0f;
            for(int i = 0; i < Data.Length - 1; ++i)
            {
                diff += GetChannelDiff(other, i);
            }
            return diff;
        }

        public override float GetTotalDiffFromColor(Color other)
        {
            return GetTotalDiff((RGB)other);
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
