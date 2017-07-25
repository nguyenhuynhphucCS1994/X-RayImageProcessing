using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Image_Processing_X_Ray.DataStructures
{
    class Matrix
    {
        #region Properties
        
        public int Width;
        public int Height;
        public int[,] Values;
        
        #endregion

        #region Constructors
        
        public Matrix(int width,int height)
        {
            this.Width = width;
            this.Height = height;
            this.Values = new int[width, height];
        }

        #endregion

        #region Methods
        //x::width
        //y::height
        public int Get(int x,int y)
        {
            return this.Values[x,y];
        }

        public void Set(int x,int y,int value)
        {
            this.Values[x, y] = value;
        }

        public enum Direction
        {
             N, S, E, W, NE, NW, SE, SW
        }
        public Cell GetNeighbor(int x,int y ,Direction direction)
        {
            int neighborX = -1;
            int neighborY = -1;
            switch(direction)
            {
                case Direction.NW: neighborX = x - 1; neighborY = y - 1; break;
                case Direction.N: neighborX = x; neighborY = y - 1; break;
                case Direction.NE: neighborX = x + 1; neighborY = y - 1; break;
                case Direction.E: neighborX = x + 1; neighborY = y; break;
                case Direction.SE: neighborX = x + 1; neighborY = y + 1; break;
                case Direction.S: neighborX = x; neighborY = y + 1; break;
                case Direction.SW: neighborX = x - 1; neighborY = y + 1; break;
                case Direction.W: neighborX = x - 1; neighborY = y; break;
            }
            if (neighborX >= 0 && neighborX < this.Width)
            {
                if (neighborY >= 0 && neighborY < this.Height)
                {
                    return new Cell(neighborX, neighborY, this.Values[neighborX, neighborY]);
                }
            }
            return null;
        }

        public List<Cell> GetNeighbors(int x,int y)
        {
            List<Cell> neighbors = new List<Cell>();

            neighbors.Add(GetNeighbor(x, y, Direction.N));
            neighbors.Add(GetNeighbor(x, y, Direction.W));
            neighbors.Add(GetNeighbor(x, y, Direction.NW));
            neighbors.Add(GetNeighbor(x, y, Direction.NE));
            neighbors.Add(GetNeighbor(x, y, Direction.E));
            neighbors.Add(GetNeighbor(x, y, Direction.SE));
            neighbors.Add(GetNeighbor(x, y, Direction.S));
            neighbors.Add(GetNeighbor(x, y, Direction.SW));

            return neighbors.Where(cell => cell != null).ToList();
        }
        #endregion
    }
}
