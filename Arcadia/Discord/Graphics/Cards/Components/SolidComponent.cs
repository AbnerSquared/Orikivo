using System;
using System.Drawing;
using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    public class SolidComponent : CardComponent
    {
        public SolidComponent(ComponentInfo info, FillInfo fill)
        {
            Info = info;
            Fill = fill;
        }

        // For now, this is not handled here, as it needs information from other sources
        /// <inheritdoc />
        protected override DrawableLayer Build()
        {
            return null;
        }

        /// <inheritdoc />
        public override void Draw(ref Drawable card, ref Cursor cursor, ref ComponentReference previous)
        {
            if (Fill == null || Info == null)
                throw new Exception("Expected both component fill info and base info to be specified");

            int width = CardInfo.GetWidth(Info, cursor, previous);
            int height = CardInfo.GetHeight(Info, cursor, previous);

            DrawableLayer layer = new BitmapLayer
            {
                Source = CreateSolid(Fill, width, height)
            };

            int offsetX = CardInfo.GetOffsetX(Info, cursor, previous);
            int offsetY = CardInfo.GetOffsetY(Info, cursor, previous);

            layer.Offset = new Coordinate(offsetX, offsetY);
            layer.Properties.Padding = Info.Padding;

            if (Info.CursorOffset.HasFlag(CursorOffset.X))
                cursor.X += layer.GetBaseWidth() + layer.Properties.Padding.Width;

            if (Info.CursorOffset.HasFlag(CursorOffset.Y))
                cursor.Y += layer.GetBaseHeight() + layer.Properties.Padding.Height;

            previous.Update(layer.GetBaseWidth(), layer.GetBaseHeight(), layer.Properties.Padding);
            card.AddLayer(layer);
        }

        private static Bitmap CreateSolid(FillInfo fill, int width, int height)
        {
            if (width == 0 || height == 0)
                throw new Exception("Cannot create a solid with a width or height of 0");

            return fill.Usage switch
            {
                FillMode.Solid => ImageHelper.CreateSolid(fill.Palette[fill.Primary], width, height),
                FillMode.Gradient => ImageHelper.CreateGradient(fill.Palette, width, height, fill.Direction),
                FillMode.Bar => ImageHelper.CreateProgressBar(fill.Palette[fill.Primary], fill.Palette[fill.Secondary ?? Gamma.Standard], width, height, fill.FillPercent.GetValueOrDefault(0), fill.Direction),
                _ => throw new Exception("Cannot initialize a solid component with the enclosing fill mode")
            };
        }
    }
}
