using Discord;
using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    public class ImageComponent : ICardComponent
    {
        public ImageComponent(ComponentInfo info, FillInfo fill)
        {
            Info = info;
            Fill = fill;
        }

        public ComponentInfo Info { get; }

        public FillInfo Fill { get; }

        public string Url;

        public int Size;

        public Color? BackgroundColor;

        public Color? FramePrimaryColor;

        public Color? FrameSecondaryColor;

        public string BorderId;

        public void Draw(Drawable card, ref Cursor cursor, ref ComponentReference previous)
        {
            // Get the layer offsets
            int offsetX = CardInfo.GetOffsetX(Info, cursor, previous);
            int offsetY = CardInfo.GetOffsetY(Info, cursor, previous);

            var layer = new BitmapLayer
            {
                Source = ImageHelper.ForceLumenPalette(ImageHelper.GetHttpImage(Url), Fill.Palette),
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
