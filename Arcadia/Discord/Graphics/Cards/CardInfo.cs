using System.Collections.Generic;
using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    public class CardInfo
    {
        public static int GetOffsetX(ComponentInfo info, Cursor cursor, ComponentReference previous)
        {
            int x = 0;

            if (info.OffsetHandling == OffsetHandling.Additive)
                x = cursor.X;

            if (info.OffsetX != 0)
                x += info.OffsetX;

            if (info.PreviousOffsetInherit.HasFlag(SizeInherit.Width))
                x += previous.Width;

            if (info.PreviousOffsetInherit.HasFlag(SizeInherit.PaddingWidth))
                x += previous.PaddingWidth;

            return x;
        }

        public static int GetOffsetY(ComponentInfo info, Cursor cursor, ComponentReference previous)
        {
            int y = 0;

            if (info.OffsetHandling == OffsetHandling.Additive)
                y = cursor.Y;

            if (info.OffsetY != 0)
                y += info.OffsetY;

            if (info.PreviousOffsetInherit.HasFlag(SizeInherit.Height))
                y += previous.Height;

            if (info.PreviousOffsetInherit.HasFlag(SizeInherit.PaddingHeight))
                y += previous.PaddingHeight;

            return y;
        }

        public bool CanTrim { get; internal set; }
        public int Width { get; internal set; }
        public int Height { get; internal set; }
        public Padding Padding { get; internal set; }
        public Padding Margin { get; internal set; }
        public int CursorOriginX { get; internal set; }
        public int CursorOriginY { get; internal set; }
        public List<ICardComponent> Components { get; internal set; }
        public GammaPalette BasePalette { get; internal set; }
        public int BorderThickness { get; internal set; }
        public BorderAllow BorderAllow { get; internal set; }
        public FillInfo BorderFill { get; internal set; }
        public string BackgroundUrl { get; internal set; }
        public bool TileBackground { get; internal set; }
        public FillInfo BackgroundFill { get; internal set; }
    }
}
