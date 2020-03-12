using System.Drawing;

namespace Orikivo.Drawing
{
    public class SolidLayer : DrawableLayer
    {
        public Color Color { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        protected override Bitmap GetBaseImage()
            => GraphicsUtils.CreateRgbBitmap(Color, Width, Height);
    }
}
