using System.Collections.Generic;

namespace Orikivo.Drawing
{
    // TODO: Remove usage of this class, make only work in PixelGraphics.
    public class TextBox
    {
        public TextBox(string text, FontFace font)
        {
            Text = text;

        }

        internal TextBox(string text, Padding padding, int width, int height, List<CharObject> chars)
        {
            Text = text;
            Padding = padding;
            Width = width;
            Height = height;
            Chars = chars;
        }

        public string Text { get; }

        public bool Disposed { get; private set; } = false;

        public Padding Padding { get; }

        public int BitmapWidth => Padding.Width + Width;

        public int BitmapHeight => Padding.Height + Height;

        public int Width { get; }

        public int Height { get; }

        public List<CharObject> Chars { get; }
    }
}
