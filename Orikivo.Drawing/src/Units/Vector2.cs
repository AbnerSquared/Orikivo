using Orikivo.Drawing.Graphics2D;
using System;

namespace Orikivo.Drawing
{
    // TODO: Implement operators.
    public struct Vector2
    {
        public static Vector2 Zero = new Vector2(0);

        public static Vector2 One = new Vector2(1);

        public static Vector2 Add(Vector2 a, Vector2 b)
            => new Vector2(a.X + b.X, a.Y + b.Y);

        public static Vector2 Add(Vector2 a, float b)
            => new Vector2(a.X + b, a.Y + b);

        public static Vector2 Subtract(Vector2 a, Vector2 b)
            => new Vector2(b.X - a.X, b.Y - a.Y);

        public static Vector2 Subtract(Vector2 a, float b)
            => new Vector2(a.X - b, a.Y - b);

        public static Vector2 Multiply(Vector2 a, Vector2 b)
            => new Vector2(a.X * b.X, a.Y * b.Y);

        public static Vector2 MultiplyXY(Vector2 v, float b)
            => new Vector2(v.X * b, v.Y * b);

        public static Vector2 Multiply(Vector2 v, float scalar)
        {
            float magnitude = v.GetLength() * scalar;
            return new Vector2(magnitude, v.GetDirection());
        }

        public static Vector2 Divide(Vector2 a, Vector2 b)
            => new Vector2(a.X / b.X, a.Y / b.Y);

        public static Vector2 DivideXY(Vector2 a, float v)
            => new Vector2(a.X / v, a.Y / v);

        public static Vector2 Round(Vector2 v)
            => new Vector2(MathF.Round(v.X), MathF.Round(v.Y));

        public static Vector2 Floor(Vector2 v)
            => new Vector2(MathF.Floor(v.X), MathF.Floor(v.Y));

        public static Vector2 Ceiling(Vector2 v)
            => new Vector2(MathF.Ceiling(v.X), MathF.Ceiling(v.Y));

        public static Vector2 Min(Vector2 a, Vector2 b)
        {
            float x = a.GetLength();
            float y = b.GetLength();
            float min = MathF.Min(x, y);

            if (min == x)
                return a;

            return b;
        }

        public static Vector2 Min(Vector2 a, Vector2 b, params Vector2[] rest)
        {
            Vector2 min = Min(a, b);

            foreach (Vector2 v in rest)
                min = Min(min, v);

            return min;
        }

        public static Vector2 Max(Vector2 a, Vector2 b)
        {
            float x = a.GetLength();
            float y = b.GetLength();
            float max = MathF.Max(x, y);

            if (max == x)
                return a;

            return b;
        }

        public static Vector2 Max(Vector2 a, Vector2 b, params Vector2[] rest)
        {
            Vector2 max = Max(a, b);

            foreach (Vector2 v in rest)
                max = Max(max, v);

            return max;
        }

        public static Vector2 Negate(Vector2 v)
            => new Vector2(-v.X, -v.Y);

        public static float Dot(Vector2 a, Vector2 b)
            => a.X * b.X + a.Y * b.Y;

        public static Vector2 Normalize(Vector2 v)
        {
            float magnitude = v.GetLength();
            return new Vector2(v.X / magnitude, v.Y / magnitude);
        }

        public Vector2(Vector2 p1, Vector2 p2)
        {
            X = p2.X - p1.X;
            Y = p2.Y - p1.Y;
        }

        public Vector2(float x = 0, float y = 0)
        {
            X = x;
            Y = y;
        }

        public Vector2(float xy)
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

        public void OffsetRotate(AngleF offset)
        {
            X = X * MathF.Cos(offset.Radians) - (Y * MathF.Sin(offset.Radians));
            Y = Y * MathF.Sin(offset.Radians) + (Y * MathF.Cos(offset.Radians));
        }

        public Vector2 GetNormal()
            => CalcF.Rotate(X, Y, AngleF.Up);

        public float GetLength()
            => CalcF.Hypotenuse(X, Y);

        public Line ToLine()
            => new Line(0, 0, X, Y);

        public AngleF GetDirection()
            => MathF.Atan(Y / X);
    }
}
