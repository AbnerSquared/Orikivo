using System.Drawing;
using Orikivo;
using Orikivo.Drawing;
using Orikivo.Text;

namespace Arcadia.Graphics
{
    public class TextComponent : CardComponent
    {
        public TextComponent(ComponentInfo info, FillInfo fill)
        {
            Info = info;
            Fill = fill;
        }

        public TextInfo Text { get; set; }

        /// <inheritdoc />
        protected override DrawableLayer Build()
        {
            using var graphics = new GraphicsService();
            Bitmap text = graphics.DrawText(Text.Content, Text.Font, Fill.Primary, Fill.Palette);
            return new BitmapLayer(text);
        }
    }
}
