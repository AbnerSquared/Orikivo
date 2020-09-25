using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    public class IconComponent : ICardComponent
    {
        public ComponentInfo Info { get; }
        public FillInfo Fill { get; }

        public string ReferencePath { get; set; }
        public int ReferenceIndex { get; set; }
        public int CropWidth { get; set; }
        public int CropHeight { get; set; }

        public void Draw(Drawable card, ref Cursor cursor, ref ComponentReference previous)
        {
            // Get the layer offsets
            int offsetX = CardInfo.GetOffsetX(Info, cursor, previous);
            int offsetY = CardInfo.GetOffsetY(Info, cursor, previous);

            // Get the image source
            var sheet = new Sheet(ReferencePath, CropWidth, CropHeight);
            var sprite = sheet.GetSprite(ReferenceIndex);
            var icon = ImageHelper.SetColorMap(sprite, GammaPalette.Default, Fill.Palette);

            // Build the new layer
            var layer = new BitmapLayer(icon)
            {
                Offset = new Coordinate(offsetX, offsetY)
            };

            layer.Properties.Padding = Info.Padding;

            // Try to offset the cursor
            if (Info.CursorOffset.HasFlag(CursorOffset.X))
                cursor.X += layer.Source.Width + layer.Properties.Padding.Width;

            if (Info.CursorOffset.HasFlag(CursorOffset.Y))
                cursor.Y += layer.Source.Height + layer.Properties.Padding.Height;

            // Finally, update the component reference
            previous.Update(layer.Source.Width, layer.Source.Height, layer.Properties.Padding);

            // Add the layer to the card
            card.AddLayer(layer);
        }
    }
}