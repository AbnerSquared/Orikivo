using System;
using System.Drawing;
using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    public sealed class TextComponent : CardComponent
    {
        public TextComponent(ComponentInfo info, FillInfo fill, FillInfo outline = null)
        {
            Info = info;
            Fill = fill;
            Outline = outline;
        }

        public TextInfo Text { get; set; }

        /// <inheritdoc />
        protected override DrawableLayer Build()
        {
            return new BitmapLayer(CreateText(Text, Fill, Outline));
        }

        private static Bitmap CreateText(TextInfo text, FillInfo fill, FillInfo outline)
        {
            using var graphics = new GraphicsService();

            if (fill.Mode == FillMode.Solid && (outline == null || outline.Mode == FillMode.None))
                return graphics.DrawText(text.Content, text.Font, fill.Primary ?? Gamma.Max, fill.Palette);

            Color primary = fill.Primary.HasValue ? (Color)fill.Palette[fill.Primary.Value] : Color.Empty;
            Color secondary = fill.Secondary.HasValue ? (Color)fill.Palette[fill.Secondary.Value] : Color.Empty;

            var properties = new ImageProperties();

            if (outline != null && outline.Mode != FillMode.None)
                properties.Padding = new Padding(1);

            using Bitmap mask = graphics.DrawText(text.Content, text.Font, Gamma.Max, GammaPalette.Default, properties);

            Bitmap content = fill.Mode switch
            {
                FillMode.Solid => graphics.DrawText(text.Content, text.Font, primary, properties),
                FillMode.Gradient => graphics.SetGradientMask(mask, fill.Palette, fill.Direction),
                FillMode.Bar => graphics.SetBarMask(mask,
                primary,
                secondary,
                fill.FillPercent.GetValueOrDefault(0),
                fill.Direction,
                MaskingMode.Clamp),
                _ => throw new Exception("An invalid fill mode was specified for a text component")
            };

            // If there is no outline fill information specified, don't do anything in this case
            if (outline == null || outline.Mode == FillMode.None)
                return content;

            GammaPalette outlinePalette = outline.Palette ?? fill.Palette;
            Color outlinePrimary = outline.Primary.HasValue ? (Color)outlinePalette[outline.Primary.Value] : Color.Empty;
            Color outlineSecondary = outline.Secondary.HasValue ? (Color)outlinePalette[outline.Secondary.Value] : Color.Empty;

            if (outline.Mode == FillMode.Solid)
            {
                // Dispose of content in this case, as the image is entirely redrawn
                Bitmap result = ImageHelper.DrawOutline(content, 1, outlinePrimary);
                content.Dispose();
                return result;
            }

            if (outline.Mode == FillMode.Gradient)
            {
                using Bitmap gradient = ImageHelper.CreateGradient(outlinePalette, content.Width, content.Height, outline.Direction);
                using Bitmap outlineContent = ImageHelper.DrawOutline(mask, 1, gradient, true);

                using var g = System.Drawing.Graphics.FromImage(content);
                ImageHelper.ClipAndDrawImage(g, outlineContent, 0, 0);
                return content;
                //content.Dispose();
                //return outlineContent;
            }

            if (outline.Mode == FillMode.Bar)
            {
                using Bitmap bar = ImageHelper.CreateProgressBar(outlinePrimary,
                    outlineSecondary,
                    content.Width,
                    content.Height,
                    outline.FillPercent ?? fill.FillPercent.GetValueOrDefault(0),
                    outline.Direction);

                using Bitmap outlineContent = ImageHelper.DrawOutline(mask, 1, bar, true);
                using var g = System.Drawing.Graphics.FromImage(content);
                ImageHelper.ClipAndDrawImage(g, outlineContent, 0, 0);
                return content;
                //content.Dispose();
                //return outlineContent;
            }

            throw new Exception("An invalid fill mode was specified for an outline");
        }
    }
}
