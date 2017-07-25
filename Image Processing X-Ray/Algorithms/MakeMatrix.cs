using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Image_Processing_X_Ray.Algorithms
{
    class RectangleMatrix
    {
        #region Methods
        public static byte[,] MakeMatrix(byte width)
        {
            byte[,] tempMatrix = new byte[width, width];
            for(byte j=0;j<width;j++)
            {
                for(byte i=0;i<width;i++)
                {
                    tempMatrix[i, j] = 1;
                }
            }
            return tempMatrix;
        }
        #endregion
    }
}
