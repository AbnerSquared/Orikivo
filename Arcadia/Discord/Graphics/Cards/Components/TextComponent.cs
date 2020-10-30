using Orikivo;
using Orikivo.Drawing;
using Orikivo.Text;

namespace Arcadia.Graphics
{
    public class TextComponent : ICardComponent
    {
        public TextComponent(ComponentInfo info, FillInfo fill)
        {
            Info = info;
            Fill = fill;
        }

        public ComponentInfo Info { get; }
        public FillInfo Fill { get; }

        public string Content { get; set; }

        public FontFace Font { get; set; }

        public Casing Casing { get; set; } = Casing.Any;

        public void Draw(Drawable card, ref Cursor cursor, ref ComponentReference previous)
        {
            // Get the layer offsets
            int offsetX = CardInfo.GetOffsetX(Info, cursor, previous);
            int offsetY = CardInfo.GetOffsetY(Info, cursor, previous);

            using var graphics = new GraphicsService();

            var text = graphics.DrawText(Content.ToString(Casing), Font, Fill.Primary, Fill.Palette);

            // Build the new layer
            var layer = new BitmapLayer(text)
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