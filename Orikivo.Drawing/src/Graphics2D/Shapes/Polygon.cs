namespace Orikivo.Drawing.Graphics2D
{
    /// <summary>
    /// Represents a base polygon that has assured defineable points.
    /// </summary>
    public abstract class Polygon
    {
        public const int RequiredVertices = 3;

        //public Polygon(params Vector2[] points)
        //{
         //   if (points.Length < 3)
         //       throw new System.ArgumentException("There must be at least three vertices for a shape to form.");
        //}

        public Vector2[] Points { get; protected set; }

        public void Offset(Vector2 offset)
            => Offset(offset.X, offset.Y);

        public void Offset(float x, float y)
        {
            for (int i = 0; i < Points.Length; i++)
            {
                Points[i].Offset(x, y);
            }
        }
    }
}
