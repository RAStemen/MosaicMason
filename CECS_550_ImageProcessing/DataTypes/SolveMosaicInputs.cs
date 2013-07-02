using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CECS_550_ImageProcessing.DataTypes
{
    class SolveMosaicInputs
    {
        public XIRImage image;
        public XIRImage[] images;
        public float regionAspectRatio;
        public int columns;
        public int regionCellWidth;
        public int regionCellHeight;
        public int numMatchesToReturn;
        public bool isGrayscale;

        public SolveMosaicInputs()
        {

        }
    }
}
