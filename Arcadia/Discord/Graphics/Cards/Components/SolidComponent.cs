using System;
using System.Drawing;
using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    public class SolidComponent : CardComponent
    {
        public SolidComponent(ComponentInfo info, FillInfo fill, FillInfo outlineFill = null)
        {
            Info = info;
            Fill = fill;
            OutlineFill = outlineFill;
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
                Source = CreateSolid(Fill, OutlineFill, width, height)
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

        private static Bitmap CreateSolid(FillInfo fill, FillInfo outline, int width, int height)
        {
            if (width == 0 || height == 0)
                throw new Exception("Cannot create a solid with a width or height of 0");

            Color primary = fill.Primary.HasValue ? (Color)fill.Palette[fill.Primary.Value] : Color.Empty;
            Color secondary = fill.Secondary.HasValue ? (Color)fill.Palette[fill.Secondary.Value] : Color.Empty;

            Bitmap content = fill.Mode switch
            {
                FillMode.Solid => ImageHelper.CreateSolid(primary, width, height),
                FillMode.Gradient => ImageHelper.CreateGradient(fill.Palette, width, height, fill.Direction),
                FillMode.Bar => ImageHelper.CreateProgressBar(primary, secondary, width, height, fill.FillPercent.GetValueOrDefault(0), fill.Direction),
                _ => throw new Exception("Cannot initialize a solid component with the enclosing fill mode")
            };

            if (outline == null || outline.Mode == FillMode.None)
            {
                return content;
            }

            Bitmap result = ImageHelper.Pad(content, new Padding(1), true);

            GammaPalette outlinePalette = outline.Palette ?? fill.Palette;
            Color outlinePrimary = outline.Primary.HasValue ? (Color)outlinePalette[outline.Primary.Value] : Color.Empty;
            Color outlineSecondary = outline.Secondary.HasValue ? (Color)outlinePalette[outline.Secondary.Value] : Color.Empty;

            if (outline.Mode == FillMode.Solid)
            {
                // Dispose of content in this case, as the image is entirely redrawn
                Bitmap r = ImageHelper.DrawOutline(result, 1, outlinePrimary);
                result.Dispose();
                return r;
            }

            if (outline.Mode == FillMode.Gradient)
            {
                using Bitmap gradient = ImageHelper.CreateGradient(outlinePalette, result.Width, result.Height, outline.Direction);
                using Bitmap outlineContent = ImageHelper.DrawOutline(result, 1, gradient, true);

                using System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(result);
                ImageHelper.ClipAndDrawImage(g, outlineContent, 0, 0);
                return result;
            }

            if (outline.Mode == FillMode.Bar)
            {
                using Bitmap bar = ImageHelper.CreateProgressBar(outlinePrimary,
                    outlineSecondary, result.Width, result.Height,
                    outline.FillPercent ?? fill.FillPercent.GetValueOrDefault(0),
                    outline.Direction);

                using Bitmap outlineContent = ImageHelper.DrawOutline(result, 1, bar, true);
                using System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(result);

                ImageHelper.ClipAndDrawImage(g, outlineContent, 0, 0);
                return result;
            }

            throw new Exception("An invalid fill mode was specified for an outline");
        }
    }
}
