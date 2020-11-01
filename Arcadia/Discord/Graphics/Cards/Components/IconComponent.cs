using System.Drawing;
using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    public class IconComponent : CardComponent
    {
        public IconComponent(ComponentInfo info, FillInfo fill)
        {
            Info = info;
            Fill = fill;
        }

        public SheetInfo Sheet { get; set; }

        /// <inheritdoc />
        protected override DrawableLayer Build()
        {
            var sheet = new Sheet(Sheet.Path, Sheet.CropWidth, Sheet.CropHeight);
            Bitmap sprite = sheet.GetSprite(Sheet.Index);
            Bitmap icon = ImageHelper.SetColorMap(sprite, GammaPalette.Default, Fill.Palette);

            if (Sheet.AutoTrim)
                icon = ImageHelper.Trim(icon, true);

            return new BitmapLayer(icon);
        }
    }
}
