using System.Drawing;

namespace Orikivo.Drawing.Graphics2D
{
    public static class CanvasExtensions
    {
        public static void DrawLine(this Canvas c, Point a, Point b, Color color)
            => c.DrawLine(a.X, a.Y, b.X, b.Y, color);

        public static void DrawCircle(this Canvas c, Point origin, int radius, Color color)
            => c.DrawCircle(origin.X, origin.Y, radius, color);

        public static void DrawRectangle(this Canvas c, Point point, Size size, Color color)
            => c.DrawRectangle(point.X, point.Y, size.Width, size.Height, color);
    }
}
