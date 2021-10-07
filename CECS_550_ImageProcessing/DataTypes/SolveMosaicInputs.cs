using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MosaicMason.DataTypes
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
