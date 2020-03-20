using System;

namespace Orikivo.Drawing.Graphics2D
{
    /// <summary>
    /// Represents a partial <see cref="CircleF"/>.
    /// </summary>
    public class SectorF
    {
        public SectorF(Vector2 origin, float radius, AngleF length)
        {
            Origin = origin;
            Radius = radius;
            Offset = AngleF.Right;
            OffsetEnd = length;
        }

        public SectorF(Vector2 origin, float radius, AngleF offset, AngleF length)
        {
            Origin = origin;
            Radius = radius;
            Offset = offset;
            OffsetEnd = length + offset;
        }

        /// <summary>
        /// Represents the center point of the <see cref="SectorF"/>.
        /// </summary>
        public Vector2 Origin { get; set; }

        public float Radius { get; set; }

        /// <summary>
        /// Represents the degree at which the <see cref="SectorF"/> starts.
        /// </summary>
        public AngleF Offset { get; set; }

        /// <summary>
        /// Represents the degree at which the <see cref="SectorF"/> ends.
        /// </summary>
        public AngleF OffsetEnd { get; set; }

        /// <summary>
        /// Returns the actual degree of the <see cref="SectorF"/>.
        /// </summary>
        public AngleF GetTheta()
            => OffsetEnd - Offset;

        public float GetPerimeter()
            => GetArcLength() + (2 * Radius);

        public float GetArea()
            => MathF.Pow(Radius, GetTheta());

        public float GetArcLength()
            => Radius * GetTheta().Radians;

        /// <summary>
        /// Returns the <see cref="CircleF"/> that contains the <see cref="SectorF"/>.
        /// </summary>
        public CircleF GetCircle()
            => new CircleF(Origin, Radius);
    }
}
