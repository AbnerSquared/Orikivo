using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    public sealed class ImageComponent : CardComponent
    {
        public ImageComponent(ComponentInfo info, FillInfo fill)
        {
            Info = info;
            Fill = fill;
        }

        public string Url { get; set; }

        /// <inheritdoc />
        protected override DrawableLayer Build()
        {
            return new BitmapLayer
            {
                Source = ImageHelper.ForceLumenPalette(ImageHelper.GetHttpImage(Url), Fill.Palette)
            };
        }
    }
}
