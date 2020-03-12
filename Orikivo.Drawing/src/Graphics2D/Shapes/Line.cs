using System;

namespace Orikivo.Drawing.Graphics2D
{
    /// <summary>
    /// Represents two <see cref="Vector2"/> points.
    /// </summary>
    public class Line
    {
        public Line(Vector2 a, float length, float angle)
        {
            Points = new Vector2[2];

            angle = angle % 360.00f;

            Vector2 b = new Vector2(a.X + (length * MathF.Cos(CalcF.Radians(angle))),
                a.Y + (length * MathF.Sin(CalcF.Radians(angle))));

            A = a;
            B = b;
        }

        public Line(Vector2 a, Vector2 b)
        {
            Points = new Vector2[2];

            A = a;
            B = b;
        }

        public Vector2[] Points { get; protected set; }

        public Vector2 A
        {
            get => Points[0];
            set => Points[0] = value;
        }

        public Vector2 B
        {
            get => Points[1];
            set => Points[1] = value;
        }

        public float Length => CalcF.Distance(A, B);
    }
}
