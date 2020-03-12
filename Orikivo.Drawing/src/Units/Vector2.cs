using System;

namespace Orikivo.Drawing
{
    public struct Vector2
    {
        public static Vector2 Zero = new Vector2(0.0f, 0.0f);

        public static Vector2 One = new Vector2(1.00f, 1.00f);

        public static Vector2 Subtract(Vector2 a, Vector2 b)
            => new Vector2(b.X - a.X, b.Y - a.Y);

        public static Vector2 Multiply(Vector2 a, Vector2 b)
            => new Vector2(a.X * b.X, a.Y * b.Y);

        public static Vector2 Round(Vector2 v)
            => new Vector2(MathF.Round(v.X), MathF.Round(v.Y));

        public Vector2(float x = 0.00f, float y = 0.00f)
        {
            X = x;
            Y = y;
        }

        public Vector2(float xy = 0.00f)
        {
            X = Y = xy;
        }

        public float X { get; set; }

        public float Y { get; set; }

        public void Offset(Vector2 offset)
        {
            X += offset.X;
            Y += offset.Y;
        }

        public void Offset(float x, float y)
        {
            X += x;
            Y += y;
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
            => Subtract(a, b);
    }
}
