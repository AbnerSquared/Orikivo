using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Drawing
{
    public struct TextCursor
    {
        public TextCursor(int x, int y, int seekLength = 1, int? maxWidth = null, int? maxHeight = null)
        {
            X = _spawnX = x;
            Y = _spawnY = y;
            SeekLength = seekLength;
            Rows = new List<Unit>();
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
            CurrentHeight = 0;
        }

        private readonly int _spawnX;
        private readonly int _spawnY;

        public int X { get; private set; }
        public int Y { get; private set; }

        public List<Unit> Rows { get; private set; }

        public int RowCount => Rows.Count;

        public int CurrentHeight { get; private set; }
        
        public int SeekLength { get; set; }

        public int Width => GetWidth();

        public int Height => GetHeight();

        public int? MaxWidth { get; }

        public int? MaxHeight { get; }

        private int GetWidth()
        {
            int curX = X;
            return Rows.Where(x => x.Width > curX).Select(x => x.Width).OrderByDescending(x => x).FirstOrNull() ?? X;
        }

        private int GetHeight()
        {
            return Rows.Select(x => x.Height).Sum() + CurrentHeight;
        }

        public void Move(int x = 0, int y = 0)
        {
            if (x == 0 && y == 0)
                return;

            int seekLength = x * SeekLength;

            if (MaxWidth.HasValue)
            {
                if (X + seekLength >= MaxWidth.Value)
                {
                    int remLength = (MaxWidth.Value - X);
                    int fits = Math.DivRem(remLength, SeekLength, out int rem);

                    if (rem != 0)
                        fits++;

                    int fittableLength = fits * SeekLength;

                    int nextLength = seekLength - fittableLength;

                    X += fittableLength;
                    if (y > CurrentHeight)
                        CurrentHeight = y;

                    Break();

                    X += nextLength;
                    if (y > CurrentHeight)
                        CurrentHeight = y;

                    return;
                }
            }

            X += x;
            if (y > CurrentHeight)
                CurrentHeight = y;
        }

        /// <summary>
        /// Breaks the existing cursor row, and initializes a new empty row.
        /// </summary>
        public void Break()
        {
            Rows.Add(new Unit(X, CurrentHeight));
            X = _spawnX;
            CurrentHeight = 0;
        }
    }
}
