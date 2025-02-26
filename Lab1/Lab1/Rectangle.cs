using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    public class Rectangle : Shape
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Rectangle(int x, int y, int width, int height) : base(x, y, '#')
        {
            Width = width;
            Height = height;
        }

        public override void Draw(char[,] canvas)
        {
            for (int i = Y; i < Y + Height; i++)
            {
                for (int j = X; j < X + Width; j++)
                {
                    if (IsWithinCanvas(i, j, canvas))
                    {
                        canvas[i, j] = Color;
                    }
                }
            }
        }

        private bool IsWithinCanvas(int i, int j, char[,] canvas)
        {
            return i >= 0 && i < canvas.GetLength(0) && j >= 0 && j < canvas.GetLength(1);
        }
    }
}
