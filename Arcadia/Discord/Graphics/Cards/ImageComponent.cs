using Discord;
using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    public class ImageComponent : ICardComponent
    {
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

        }
    }
}