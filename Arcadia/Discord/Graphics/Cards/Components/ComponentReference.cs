using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    public class ComponentReference
    {
        public int Width { get; internal set; }

        public int Height { get; internal set; }

        public int PaddingWidth { get; internal set; }

        public int PaddingHeight { get; internal set; }

        internal void Update(int width, int height, Padding padding)
        {
            Width = width;
            Height = height;
            PaddingWidth = padding.Width;
            PaddingHeight = padding.Height;
        }
    }
}
