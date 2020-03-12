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

        /// <summary>
        /// Returns the rectangle shape that contains the <see cref="Quad"/>.
        /// </summary>
        public Quad GetBoundingBox()
        {
            float xMin = CalcF.Min(TopLeft.X, TopRight.X, BottomLeft.X, BottomRight.X);
            float xMax = CalcF.Max(TopLeft.X, TopRight.X, BottomLeft.X, BottomRight.X);
            float yMin = CalcF.Min(TopLeft.Y, TopRight.Y, BottomLeft.Y, BottomRight.Y);
            float yMax = CalcF.Max(TopLeft.Y, TopRight.Y, BottomLeft.Y, BottomRight.Y);

            // Might need to be reversed, depending on right-hand rule
            Vector2 topLeft = new Vector2(xMin, yMin);
            Vector2 topRight = new Vector2(xMax, yMin);
            Vector2 bottomLeft = new Vector2(xMin, yMax);
            Vector2 bottomRight = new Vector2(xMax, yMax);

            return new Quad(topLeft, topRight, bottomLeft, bottomRight);
        }
    }
}
