using System;

namespace Orikivo.Drawing.Graphics2D
{
    // TODO: Apply Union and Except methods
    public class CircleF
    {
        public CircleF(float x, float y, float radius)
        {
            Origin = new Vector2(x, y);

            if (radius == 0)
                throw new ArgumentException("The radius specified must a value that is not 0.");

            Radius = radius;
        }

        public CircleF(Vector2 origin, float radius)
        {
            Origin = origin;

            if (radius == 0)
                throw new ArgumentException("The radius specified must a value that is not 0.");

            Radius = MathF.Abs(radius);
        }

        /// <summary>
        /// Represents the center point of the <see cref="CircleF"/>.
        /// </summary>
        public Vector2 Origin { get; protected set; }

        public float Radius { get; protected set; }

        public float GetDiameter()
            => 2 * Radius;

        public float GetCircumference()
            => 2 * CalcF.Pi * Radius;

        public float GetArea()
            => CalcF.Pi * MathF.Pow(Radius, 2);

        public RegionF GetBoundingBox()
        {
            float xMin = Origin.X - Radius;
            float yMin = Origin.Y - Radius;
            float xMax = Origin.X + Radius;
            float yMax = Origin.Y + Radius;

            return new RegionF(xMin, yMin, xMax, yMax);
        }

        public Line LineFromDiameter()
            => new Line(PointFromAngle(AngleF.Right),
                PointFromAngle(AngleF.Left));

        public Line LineFromDiameter(AngleF offset)
        {
            Vector2 a = PointFromAngle(offset);
            Vector2 b = PointFromAngle(-offset);
            return new Line(a, b);
        }

        public Vector2 PointFromAngle(AngleF angle)
            => new Vector2(Origin.X + Radius * MathF.Cos(angle.Radians),
                Origin.Y + Radius * MathF.Sin(angle.Radians));

        public Line LineFromAngle(AngleF angle)
            => new Line(Origin, PointFromAngle(angle));

        public float GetArcLength(AngleF theta)
            => Radius * theta.Radians;

        public SectorF GetSector(AngleF length)
            => new SectorF(Origin, Radius, length);

        public SectorF GetSector(AngleF offset, AngleF theta)
            => new SectorF(Origin, Radius, offset, theta);

        //public bool SectorContains(AngleF offset, AngleF length, Vector2 p)
        //    => throw new NotImplementedException();

        //public bool SectorContains(AngleF length, Vector2 p)
        //    => throw new NotImplementedException();

        public bool Intersects(CircleF circle)
        {
            float dist = DistanceFromOrigin(circle.Origin);
            return dist <= Radius;
        }

        public bool Intersects(RegionF region)
        {
            var quad = new Quad(region);

            return region.Contains(Origin)
            || Intersects(quad.GetLeftLine())
            || Intersects(quad.GetTopLine())
            || Intersects(quad.GetRightLine())
            || Intersects(quad.GetBottomLine());
        }

        public bool Intersects(Line line)
        {
            Line perpendicular = line.GetPerpendicular(Origin);

            if (0 <= perpendicular.GetLength() && perpendicular.GetLength() <= Radius)
                return true;

            return Contains(line.A) || Contains(line.B);
        }

        public float DistanceFromOrigin(Vector2 p)
            => CalcF.Distance(Origin, p);

        public Line LineFromOrigin(Vector2 p)
            => new Line(Origin, p);

        public float DistanceFrom(Vector2 p)
        {
            float fromOrigin = CalcF.Distance(Origin, p);

            if (fromOrigin >= Radius)
                fromOrigin -= Radius;

            return fromOrigin;
        }

        public void Offset(Vector2 offset)
            => Origin.Offset(offset);

        public void Offset(float x, float y)
            => Origin.Offset(x, y);

        public bool Contains(Vector2 p)
            => MathF.Pow(p.X - Origin.X, 2) + MathF.Pow(p.Y - Origin.Y, 2) <= MathF.Pow(Radius, 2);
    }
}
