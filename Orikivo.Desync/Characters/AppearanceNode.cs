using Orikivo.Drawing;
using System.Drawing;

namespace Orikivo.Desync
{
    // a basic sprite with an offset
    public class AppearanceNode
    {
        public AppearanceNode() { }

        public AppearanceNode(Sprite sprite, int? offsetX = null, int? offsetY = null)
        {
            Value = sprite;
            if (offsetX == null && offsetY == null)
                HasOffset = false;

            OffsetX = offsetX ?? 0;
            OffsetY = offsetY ?? 0;
            HasOffset = true;
        }

        public bool HasOffset { get; private set; }
        public Sprite Value { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }

        public Point GetOffset()
            => new Point(OffsetX, OffsetY);
    }
}
