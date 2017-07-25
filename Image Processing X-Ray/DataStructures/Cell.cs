using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Image_Processing_X_Ray.DataStructures
{
    class Cell
    {
        #region Properties

        public int X;
        public int Y;
        public int Value;

        #endregion

        #region Constructors

        public Cell()
        {
            this.X = 0;
            this.Y = 0;
            this.Value = 0;
        }

        public Cell(int x,int y)
            :this()
        {
            this.X = x;
            this.Y = y;
        }

        public Cell(int x,int y,int value)
        {
            this.X = x;
            this.Y = y;
            this.Value = value;
        }
        #endregion
    }
}
