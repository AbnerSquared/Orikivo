using System;
using System.Drawing;

namespace Orikivo
{
    public class PixelRenderingOptions
    {
        public PixelRenderingOptions(OldAccount a)
        {
            Scale = PixelScale.Set(2); // The scale of the final image.
            PPU = 8; // The amount of pixels per unit. // 4, 6, 8 are the specific.
            Font = FontManager.FontMap.GetFont(a.Card.FontId);
            Palette = a.Card.Schema.Palette; // The color palette the image is rendered in.
            Padding = 0;
        }

        public PixelRenderingOptions(OldAccount a, int padding)
        {
            Scale = PixelScale.Set(2);
            PPU = 8;
            Font = FontManager.FontMap.GetFont(a.Card.FontId);
            Palette = a.Card.Schema.Palette;
            Padding = padding;
        }

        public int Padding { get; } = 0;
        public int PPU { get; set; } = 8;
        public int Scale { get; }
        public FontFace Font { get; set; }
        public FontType FontType { get; } = FontType.Default;
        public Color[] Palette { get; }
    }
}