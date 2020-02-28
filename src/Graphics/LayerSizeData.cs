using Orikivo.Drawing;

namespace Orikivo
{
    // Keeps track of how large a layer is, without keeping any bitmap data.
    public class LayerSizeData
    {
        public int OffsetX;
        public int OffsetY;
        public int SourceWidth;
        public int SourceHeight;
        public Padding Padding;

        public int Width => OffsetX + SourceWidth + Padding.Width;
        public int Height => OffsetY + SourceHeight + Padding.Height;
    }
}
