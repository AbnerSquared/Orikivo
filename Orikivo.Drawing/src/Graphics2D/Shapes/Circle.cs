using System;

namespace Orikivo.Drawing.Graphics2D
{
    // TODO: Apply Union, Intersect, and Except methods
    public class Circle
    {
        public Circle(float x, float y, float radius)
        {
            Origin = new Vector2(x, y);

            if (radius <= 0.0f)
                throw new ArgumentException("The radius specified must be greater than 0.");


            Radius = radius;
        }

        public Circle(Vector2 origin, float radius)
        {
            Origin = origin;

            if (radius <= 0.0f)
                throw new ArgumentException("The radius specified must be greater than 0.");


            Radius = radius;
        }

        public Vector2 Origin { get; protected set; }
        public float Radius { get; protected set; }

        public float Diameter => 2 * Radius;
        public float Circumference => GetLength();

        // returns the length (circumference) of this circle.
        private float GetLength()
        {
            return 2 * CalcF.Pi * Radius;
        }

        public float GetArea()
        {
            return CalcF.Pi * MathF.Pow(Radius, 2);
        }

        public float GetArcLength(AngleF angle)
            => angle.Radians * Radius;

        // This focuses on a portion of a circle (a segement from an angle), and checks
        // if it's within that portion.
        // The angle determines where the arc is going to be drawn
        // The arc determines the length of the sector at that angle
        // The p is the point you are checking on the sector.
        public bool SectorContains(AngleF angle, AngleF arc, Vector2 p)
        {
            throw new NotImplementedException();
        }

        public void Offset(Vector2 offset)
            => Origin.Offset(offset);

        public void Offset(float x, float y)
            => Origin.Offset(x, y);

        // returns true if the point is IN or ON the circle.
        public bool Contains(Vector2 p)
        {
            return MathF.Pow(p.X - Origin.X, 2) + MathF.Pow(p.Y - Origin.Y, 2) <= MathF.Pow(Radius, 2);
        }
    }
}
