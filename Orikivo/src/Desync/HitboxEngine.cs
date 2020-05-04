using Orikivo.Drawing;
using Orikivo.Drawing.Graphics2D;

namespace Orikivo.Desync
{
    /// <summary>
    /// Handles all methods relating to 2D hitbox and ray collision.
    /// </summary>
    internal static class HitboxEngine
    {
        internal static Line GetLineFromOriginToRegion(CircleF circle, Region region)
        {
            Line fromOrigin = circle.LineFromOrigin(region.Perimeter.Origin);
            var quad = new Quad(region.Perimeter);
            Vector2? intersection = fromOrigin.GetClosestIntersection(quad);

            if (intersection.HasValue)
                fromOrigin.B = intersection.Value;

            return fromOrigin;
        }
    }
}
