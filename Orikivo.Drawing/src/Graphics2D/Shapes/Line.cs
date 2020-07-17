using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Drawing.Graphics2D
{
    /// <summary>
    /// Represents two <see cref="Vector2"/> points.
    /// </summary>
    public class Line
    {
        public Line(float ax, float ay, float bx, float by)
        {
            Points = new Vector2[2];
            A = new Vector2(ax, ay);
            B = new Vector2(bx, by);
        }

        public Line(Vector2 a, float length, AngleF direction)
        {
            Points = new Vector2[2];

            Vector2 b = new Vector2(a.X + (length * MathF.Cos(direction.Radians)),
                a.Y + (length * MathF.Sin(direction.Radians)));

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

        public float DeltaX => B.X - A.X;

        public float DeltaY => B.Y - A.X;

        public float Slope => DeltaY / DeltaX;

        public float GetLength()
            => CalcF.Distance(A, B);

        public AngleF GetDirection()
            => MathF.Atan(Slope);

        public Vector2 ToVector2()
            => CalcF.PolarToParametric(GetLength(), GetDirection());

        public Vector2 GetMidpoint()
            => new Vector2((A.X + B.X) / 2, (A.Y + B.Y) / 2);

        public float GetY(float x)
            => IsHorizontal() ? 0
            : IsVertical() ? float.NaN
            : Slope * (x - A.X) + A.Y;

        public Line GetPerpendicular(Vector2 p)
        {
            float x = (p.Y - A.Y + (Slope * A.X) + (p.X / Slope)) / (Slope + (1 / Slope));
            return new Line(p, new Vector2(x, GetY(x)));
        }

        public void RotateAround(Vector2 p, AngleF angle)
        {
            A = CalcF.RotateAround(p, A, angle);
            B = CalcF.RotateAround(p, B, angle);
        }

        public void RotateFromCenter(AngleF angle)
        {
            A = CalcF.Rotate(A, angle);
            B = CalcF.Rotate(B, angle);
        }

        public void RotateFromA(AngleF angle)
        {
            A = CalcF.Rotate(A, angle);
        }

        public void RotateFromB(AngleF angle)
        {
            B = CalcF.Rotate(B, angle);
        }

        public Vector2? GetIntersection(Line b)
        {
            float x = GetIntersectX(b);

            if (GetY(x) != b.GetY(x))
                return null;

            return new Vector2(x, GetY(x));
        }

        public IEnumerable<Vector2> GetIntersections(Quad q)
        {
            foreach (Line line in q.GetLines())
            {
                if (Intersects(line))
                    yield return GetIntersection(line).Value;
            }
        }

        public Vector2? GetClosestIntersection(Quad q)
            => GetClosestIntersectionFromPoint(A, q);

        public Vector2? GetClosestIntersection(RegionF region)
            => GetClosestIntersectionFromPoint(A, new Quad(region));

        public Vector2? GetClosestIntersectionFromPoint(Vector2 p, Quad q)
        {
            var intersections = GetIntersections(q);

            if (intersections?.Count() == 0)
                return null;

            if (intersections.Count() == 1)
                return intersections.First();

            return intersections.OrderBy(x => CalcF.Distance(p, x)).FirstOrDefault();
        }

        public bool IsHorizontal()
            => A.Y == B.Y;

        public bool IsVertical()
            => A.X == B.X;

        public bool Intersects(Line b)
        {
            float x = GetIntersectX(b);
            return GetY(x) == b.GetY(x);
        }

        public bool Contains(Vector2 p)
            => IsHorizontal() ? p.Y == A.Y
            : IsVertical() ? p.X == A.X
            : p.Y == GetY(p.X);

        public bool SegmentContains(Vector2 p)
            => ((RangeF.Contains(A.X, B.X, p.X) && RangeF.Contains(A.Y, B.Y, p.Y))
            || (RangeF.Contains(B.X, A.X, p.X) && RangeF.Contains(B.Y, A.Y, p.Y)))
            && Contains(p);

        private float GetIntersectX(Line b)
            => (b.A.Y - A.Y + (Slope * A.X) - (b.Slope * b.A.X)) / (Slope - b.Slope);
    }
}
