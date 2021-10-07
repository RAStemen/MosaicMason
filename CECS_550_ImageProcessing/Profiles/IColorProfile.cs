using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MosaicMason.Profiles
{
    public enum EColorProfile
    { 
        RGB,
        HSV,
        HSL
    }


    public interface IColorProfile
    {
        float GetChannelDiff(IColorProfile other, int channelIndex);


        float GetTotalDiff(IColorProfile other);

        float GetTotalDiffFromColor(Color other);


        Color ToColor();

        float this[int index] { get; set; }


    }

    public static class ColorProfileUtils
    {
        public static ColorProfile FromColor<T>(Color color) where T : ColorProfile
        {
            Type t = typeof(T);
            if(t == typeof(RGB))
            {
                return (ColorProfile)new RGB(color);
            }
            else if(t == typeof(HSV))
            {
                return (ColorProfile)new HSV(color);
            }
            throw new Exception("Unhandled ColorProfile!");
        }
    }

    public class ColorProfile : IColorProfile
    {
        protected float[] Data = new float[4];
        public ColorProfile(Color color) { }

        public virtual float this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public virtual float GetChannelDiff(IColorProfile other, int channelIndex)
        {
            throw new NotImplementedException();
        }

        public virtual float GetTotalDiff(IColorProfile other)
        {
            throw new NotImplementedException();
        }

        public virtual float GetTotalDiffFromColor(Color other)
        {
            throw new NotImplementedException();
        }

        public virtual Color ToColor()
        {
            throw new NotImplementedException();
        }
    }
}
