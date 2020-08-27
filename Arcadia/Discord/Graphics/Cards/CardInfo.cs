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

        public bool CanTrim { get; }
        public int Width { get; }
        public int Height { get; }
        public Padding Padding { get; }
        public Padding Margin { get; }
        public int CursorOriginX { get; }
        public int CursorOriginY { get; }
        public List<ICardComponent> Components { get; }
        public GammaPalette BasePalette { get; }
        public int BorderThickness { get; }
        public BorderAllow BorderAllow { get; }
        public FillInfo BorderFill { get; }

        public string BackgroundUrl { get; }
        public bool TileBackground { get; }
        public FillInfo BackgroundFill { get; }
    }
}