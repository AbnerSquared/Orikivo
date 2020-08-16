using System.Drawing;

namespace Orikivo.Drawing
{
    public class SolidLayer : DrawableLayer
    {
        public Color Color { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        protected override Bitmap GetBaseImage()
            => ImageHelper.CreateRgbBitmap(Color, Width, Height);
    }
}
