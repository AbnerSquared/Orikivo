namespace Orikivo.Drawing
{
    /// <summary>
    /// Represents a rectangular floating-point region.
    /// </summary>
    public struct RegionF
    {
        public static bool Contains(float x, float y, float width, float height, float u, float v)
            => u <= x
            && v <= y
            && u < x + width
            && v < y + height;

        public RegionF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public float X { get; set; }

        public float Y { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

        public Vector2 Size => new Vector2(Width, Height);

        public Vector2 Position => new Vector2(X, Y);

        public Vector2 Origin => new Vector2(X + (Width / 2), Y + (Height / 2));

        public float Left => X;

        public float Top => Y;

        public float Right => X + Width;

        public float Bottom => Y + Height;

        public bool Contains(float x, float y)
            => Contains(X, Y, Width, Height, x, y);

        public bool Contains(Vector2 p)
            => Contains(p.X, p.Y);
    }
}
