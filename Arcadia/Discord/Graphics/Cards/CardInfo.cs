using System.Collections.Generic;
using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    public class CardInfo
    {
        public ImageScale Scale { get; internal set; }

        public bool Trim { get; internal set; }

        public int Width { get; internal set; }

        public int Height { get; internal set; }

        public Padding Padding { get; internal set; } = Padding.Empty;

        public Padding Margin { get; internal set; } = Padding.Empty;

        public int CursorOriginX { get; internal set; }

        public int CursorOriginY { get; internal set; }

        public List<CardComponent> Components { get; internal set; }

        public GammaPalette Palette { get; internal set; }

        // If null, the border is not allowed
        public BorderInfo Border { get; internal set; }

        public FillInfo BackgroundFill { get; internal set; }

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

        public static int GetWidth(ComponentInfo info, Cursor cursor, ComponentReference previous)
        {
            int width = info.BaseWidth;

            if (info.SizeHandling == SizeHandling.Set)
            {
                width = info.MaxWidth < 0 ? 0 : info.MaxWidth;
                return width;
            }

            if (info.PreviousInherit.HasFlag(SizeInherit.Width))
                width += previous.Width;

            if (info.PreviousInherit.HasFlag(SizeInherit.PaddingWidth))
                width += previous.PaddingWidth;

            return width;
        }

        public static int GetHeight(ComponentInfo info, Cursor cursor, ComponentReference previous)
        {
            int height = info.BaseHeight;

            if (info.SizeHandling == SizeHandling.Set)
            {
                height = info.MaxHeight < 0 ? 0 : info.MaxHeight;
                return height;
            }

            if (info.PreviousInherit.HasFlag(SizeInherit.Height))
                height += previous.Height;

            if (info.PreviousInherit.HasFlag(SizeInherit.PaddingHeight))
                height += previous.PaddingHeight;

            return height;
        }
    }
}
