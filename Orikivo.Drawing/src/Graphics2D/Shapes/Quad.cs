using System.Collections.Generic;

namespace Orikivo.Drawing.Graphics2D
{
    public class Quad : Polygon
    {
        public Quad(float x, float y, float width, float height)
        {
            Points = new Vector2[4];

            TopLeft = new Vector2(x, y);
            TopRight = new Vector2(x + width, y);
            BottomLeft = new Vector2(x, y + height);
            BottomRight = new Vector2(x + width, y + height);
        }

        public Quad(RegionF region)
        {
            Points = new Vector2[4];
            TopLeft = new Vector2(region.X, region.Y);
            TopRight = new Vector2(region.Right, region.Y);
            BottomLeft = new Vector2(region.X, region.Bottom);
            BottomRight = new Vector2(region.Right, region.Bottom);
        }

        public Quad(Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight)
        {
            Points = new Vector2[4];

            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
        }

        public Vector2 TopLeft
        {
            get => Points[0];
            set => Points[0] = value;
        }

        public Vector2 TopRight
        {
            get => Points[1];
            set => Points[1] = value;
        }

        public Vector2 BottomLeft
        {
            get => Points[2];
            set => Points[2] = value;
        }

        public Vector2 BottomRight
        {
            get => Points[3];
            set => Points[3] = value;
        }

        public Line GetLeftLine()
            => new Line(TopLeft, BottomLeft);

        public Line GetRightLine()
            => new Line(TopRight, BottomRight);

        public Line GetTopLine()
            => new Line(TopLeft, TopRight);

        public Line GetBottomLine()
            => new Line(BottomRight, BottomLeft);

        public IEnumerable<Line> GetLines()
        {
            return new Line[]
            {
                GetLeftLine(),
                GetRightLine(),
                GetTopLine(),
                GetBottomLine()
            };
        }

        /// <summary>
        /// Returns the <see cref="RegionF"/> that represents the containing boundaries for the <see cref="Quad"/>.
        /// </summary>
        public RegionF GetBoundingBox()
        {
            float xMin = CalcF.Min(TopLeft.X, TopRight.X, BottomLeft.X, BottomRight.X);
            float xMax = CalcF.Max(TopLeft.X, TopRight.X, BottomLeft.X, BottomRight.X);
            float yMin = CalcF.Min(TopLeft.Y, TopRight.Y, BottomLeft.Y, BottomRight.Y);
            float yMax = CalcF.Max(TopLeft.Y, TopRight.Y, BottomLeft.Y, BottomRight.Y);

            return new RegionF(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        public bool Contains(Vector2 p)
        {
            var ap = new Vector2(TopLeft, p);
            var ab = new Vector2(TopLeft, TopRight);
            var ad = new Vector2(TopLeft, BottomRight);

            return Vector2.Dot(ap, ab) <= Vector2.Dot(ab, ab)
                && 0 <= Vector2.Dot(ap, ad)
                && Vector2.Dot(ap, ad) <= Vector2.Dot(ad, ad);
        }
    }
}
