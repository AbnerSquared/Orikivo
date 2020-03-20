using System;
using System.Drawing;

namespace Orikivo.Drawing
{
    public static class OriginUtils
    {
        public static int GetMidpoint(int length)
        {
            return (int)Math.Floor((double)(length / 2));
        }

        public static Point GetOrigin(Size size, OriginAnchor anchor)
        {
            return anchor switch
            {
                OriginAnchor.TopLeft => new Point(0, 0),
                OriginAnchor.Top => new Point(GetMidpoint(size.Width), 0),
                OriginAnchor.TopRight => new Point(size.Width, 0),

                OriginAnchor.Left => new Point(0, GetMidpoint(size.Height)),
                OriginAnchor.Center => new Point(GetMidpoint(size.Width), GetMidpoint(size.Height)),
                OriginAnchor.Right => new Point(size.Width, GetMidpoint(size.Height)),

                OriginAnchor.BottomLeft => new Point(0, size.Height),
                OriginAnchor.Bottom => new Point(GetMidpoint(size.Width), size.Height),
                OriginAnchor.BottomRight => new Point(size.Width, size.Height),

                _ => new Point(0, 0)
            };
        }
    }
}
