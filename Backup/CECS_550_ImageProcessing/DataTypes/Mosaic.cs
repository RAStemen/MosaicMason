using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CECS_550_ImageProcessing.DataTypes
{
    public class Mosaic
    {
        //Basic mosaic data
        public int Columns { get; private set; }
        public int Rows { get; private set; }
        public float AspectRatio { get; private set; }
        public bool Grayscale { get; private set; }
        public int CellSubdivisionX { get; private set; }
        public int CellSubdivisionY { get; private set; }
        public Size OutputSize { get; set; }

        public Dictionary<int, XIRImage>[] SolutionSet { get; private set; }


        public Mosaic(int columns, int rows, float aspectRatio, 
            bool grayscale, int cellSubdivisionX, int cellSubdivisionY)
        {
            SolutionSet = new Dictionary<int, XIRImage>[columns * rows];
            for (int i = columns * rows - 1; i >= 0; i--)
            {
                SolutionSet[i] = new Dictionary<int, XIRImage>();
            }
            Columns = columns;
            Rows = rows;
            AspectRatio = aspectRatio;
            Grayscale = grayscale;
            CellSubdivisionX = cellSubdivisionX;
            CellSubdivisionY = cellSubdivisionY;
        }
    }
}
