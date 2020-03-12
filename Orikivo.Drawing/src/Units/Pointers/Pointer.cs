using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Orikivo.Drawing
{

    // used to help draw things, might be scrapped
    public class Pointer
    {
        public Pointer(int leftPadding = 0, int topPadding = 0, int? maxWidth = null, int? maxHeight = null)
        {
            _leftPadding = leftPadding;
            _topPadding = topPadding;
            _maxWidth = maxWidth;
            _maxHeight = maxHeight;
            
        }

        private int _leftPadding = 0;
        private int _topPadding = 0;
        private int? _maxWidth = null;
        private int? _maxHeight = null;

        // a list of all the times X was reset.
        public List<int> Rows { get; } = new List<int>();

        // the largest row
        public (int X, int Y) LastPos { get; private set; } = (0,0);

        public Point Pos => new Point(X, Y);

        public int Width
        {
            get
            {
                int[] rows = new int[Rows.Count + 1];

                for (int i = 0; i < Rows.Count; i++)
                    rows[i] = Rows[i];

                rows[Rows.Count] = X;

                return rows.OrderByDescending(x => x).First();
            }
        }

        public int Height { get; private set; }
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public void MoveX(int len)
        {
            LastPos = (X, LastPos.Y);
            /* handle resetting; think of a typewriter
            if (_maxWidth < X + len)
            {
                ResetX();
                X += len;
            }
            */

            X += len;
            //Console.WriteLine($"Pointer.ShiftX: {len}");
        }
        public void MoveY(int len)
        {
            LastPos = (LastPos.X, Y);
            Y += len;
            //Console.WriteLine($"Pointer.ShiftY: {len}");
            if (Y > Height)
                Height = Y;
        }

        public void ResetX()
        {
            Rows.Add(X);
            //Console.WriteLine($"Pointer.Rows.Add: {X}");
            X = _leftPadding;
        }

        public void ResetY()
            => Y = _topPadding;

        public Size Size => new Size(Width, Height);

    }
}
