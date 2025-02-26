using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    public abstract class Shape
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char Color { get; protected set; }

        protected Shape(int x, int y, char color)
        {
            X = x;
            Y = y;
            Color = color;
        }

        public abstract void Draw(char[,] canvas);
    }
}
