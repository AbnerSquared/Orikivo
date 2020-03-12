using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Drawing
{
    // used to help draw things, might be scrapped
    public struct Cursor
    {
        public Cursor(int x, int y, int seekLength = 1, int? maxWidth = null, int? maxHeight = null)
        {
            X = _spawnX = x;
            Y = _spawnY = y;
            SeekLength = seekLength;
            Rows = new List<(int, int)>();
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
            CurrentHeight = 0;
        }

        private readonly int _spawnX;
        private readonly int _spawnY;

        public int X { get; private set; }
        public int Y { get; private set; }

        public List<(int Length, int Height)> Rows { get; private set; }

        public int RowCount => Rows.Count;

        public int CurrentHeight { get; private set; }
        
        public int SeekLength { get; set; }

        public int Width => GetWidth();

        public int Height => Rows.Select(x => x.Height).Sum() + CurrentHeight;

        private int GetWidth()
        {
            int curX = X;
            return Rows.Where(x => x.Length > curX).Select(x => x.Length).OrderByDescending(x => x).FirstOrNull() ?? X;
        }

        public void Seek(int length, int height)
        {
            int seekLength = length * SeekLength;

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
                    if (height > CurrentHeight)
                        CurrentHeight = height;

                    Break();

                    X += nextLength;
                    if (height > CurrentHeight)
                        CurrentHeight = height;

                    return;
                }
            }

            X += length;
            if (height > CurrentHeight)
                CurrentHeight = height;
        }

        /// <summary>
        /// Breaks the existing cursor row, and initializes a new empty row.
        /// </summary>
        public void Break()
        {
            Rows.Add((X, CurrentHeight));
            X = _spawnX;
            CurrentHeight = 0;
        }

        public int? MaxWidth { get; }
        public int? MaxHeight { get; }
    }
}
