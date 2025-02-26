using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    public class Background
    {
        public char Color { get; set; }

        public Background(char color = ' ')
        {
            Color = color;
        }

        public void Apply(char[,] canvas)
        {
            for (int i = 0; i < canvas.GetLength(0); i++)
            {
                for (int j = 0; j < canvas.GetLength(1); j++)
                {
                    canvas[i, j] = Color;
                }
            }
        }
    }
}
