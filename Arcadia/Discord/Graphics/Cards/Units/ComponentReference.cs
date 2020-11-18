using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    public class ComponentReference
    {
        internal int Width { get; set; }

        internal int Height { get; set; }

        internal int PaddingWidth { get;set; }

        internal int PaddingHeight { get; set; }

        internal void Update(int width, int height, Padding padding)
        {
            Width = width;
            Height = height;
            PaddingWidth = padding.Width;
            PaddingHeight = padding.Height;
        }
    }
}
