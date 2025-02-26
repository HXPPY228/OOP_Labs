using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    public class Circle : Shape
    {
        public int Radius { get; set; }

        public Circle(int x, int y, int radius) : base(x, y, '*')
        {
            Radius = radius;
        }

        public override void Draw(char[,] canvas)
        {
            for (int i = Y - Radius; i <= Y + Radius; i++)
            {
                for (int j = X - Radius; j <= X + Radius; j++)
                {
                    if (IsInsideCircle(i, j) && IsWithinCanvas(i, j, canvas))
                    {
                        canvas[i, j] = Color;
                    }
                }
            }
        }

        private bool IsInsideCircle(int i, int j)
        {
            return (i - Y) * (i - Y) + (j - X) * (j - X) <= Radius * Radius;
        }

        private bool IsWithinCanvas(int i, int j, char[,] canvas)
        {
            return i >= 0 && i < canvas.GetLength(0) && j >= 0 && j < canvas.GetLength(1);
        }
    }
}
