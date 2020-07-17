using System;

namespace Orikivo.Drawing
{
    public struct Region
    {
        public static bool Contains(int x, int y, int width, int height, int u, int v)
            => u <= x
            && v <= y
            && u < x + width
            && v < y + height;

        public static bool Excludes(int x, int y, int width, int height, int u, int v)
            => (u < x || u > (x + width)) && (v < y || v > (y + height));

        public Region(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int Left => X;

        public int Top => Y;

        public int Right => X + Width;

        public int Bottom => Y + Height;

        public Unit Size => new Unit(Width, Height);
        public Coordinate Position => new Coordinate(X, Y);

        // NOTE: Using the floor method is consistent, as opposed to rounding.
        public Coordinate Midpoint => new Coordinate(X + (int)Math.Floor((double)Width / 2), Y + (int)Math.Floor((double)Height / 2));

        public bool Contains(int x, int y)
            => Contains(X, Y, Width, Height, x, y);
    }
}
