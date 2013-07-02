using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CECS_550_ImageProcessing.DataTypes
{
    public struct MosaicInputs
    {
        public int ImageId;
        public float AspectRatio;
        public int Columns;
        public int RegionWidth; 
        public int RegionHeight;
        public int ImagesToKeep;
        public bool Grayscale;
        public int NumThreads;
        public XIRImage[] images;
    }
}
