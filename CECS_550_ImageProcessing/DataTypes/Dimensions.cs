using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CECS_550_ImageProcessing.DataTypes
{
    public struct Dimensions
    {
        public int width;
        public int height;

        public Dimensions(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }
}
