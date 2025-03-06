using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    public class Triangle : Shape
    {
        public int Height { get; set; }

        public Triangle(int x, int y, int height, char color) : base(x, y, color)
        {
            Height = height;
        }

        public override void Draw(char[,] canvas)
        {
            for (int i = Y; i < Y + Height; i++)
            {
                int widthAtRow = 2 * (i - Y + 1) - 1;
                int startX = X - (widthAtRow / 2);

                for (int j = startX; j < startX + widthAtRow; j++)
                {
                    if (IsWithinCanvas(i, j, canvas))
                    {
                        canvas[i, j] = Color;
                    }
                }
            }
        }

        public override Shape Clone()
        {
            return new Triangle(this.X, this.Y, this.Height, this.Color);
        }

        private bool IsWithinCanvas(int i, int j, char[,] canvas)
        {
            return i >= 0 && i < canvas.GetLength(0) && j >= 0 && j < canvas.GetLength(1);
        }
    }
}
