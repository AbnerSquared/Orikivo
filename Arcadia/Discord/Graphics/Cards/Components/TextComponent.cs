using Orikivo;
using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    public class TextComponent : ICardComponent
    {
        public ComponentInfo Info { get; }
        public FillInfo Fill { get; }

        public string Content;

        public FontFace Font;

        public Casing Casing;

        public Padding Padding;

        public void Draw(Drawable card, ref Cursor cursor, ref ComponentReference previous)
        {

        }
    }
}