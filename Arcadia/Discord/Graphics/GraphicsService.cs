using System;
using System.Drawing;
using Orikivo.Drawing;
using Orikivo;
using System.Diagnostics;
using Orikivo.Framework;

namespace Arcadia.Graphics
{
    /// <summary>
    /// Handles all of the core rendering methods for Arcadia.
    /// </summary>
    public class GraphicsService : IDisposable
    {
        // TODO: Use the Minic FontFace to utilize a way to draw sub/superscript on larger fonts.
        private readonly TextFactory _text;

        public GraphicsService()
        {
            _text = new TextFactory(GetDefaultCharMap());
            _text.SetFont(GetFont(FontType.Orikos));
        }

        public static char[][][][] GetDefaultCharMap()
            => JsonHandler.Load<char[][][][]>(@"../assets/char_map.json", new JsonCharArrayConverter());

        public static CardLayout GetLayout(LayoutType type)
        {
            return type switch
            {
                LayoutType.Default => CardLayout.Default,
                LayoutType.Micro => CardLayout.Micro,
                _ => throw new Exception("Unknown layout type specified")
            };
        }

        public static FontFace GetFont(FontType type)
            => type switch
            {
                FontType.Orikos => JsonHandler.Load<FontFace>(@"../assets/fonts/orikos.json"),
                FontType.Monori => JsonHandler.Load<FontFace>(@"../assets/fonts/monori.json"),
                FontType.Minic => JsonHandler.Load<FontFace>(@"../assets/fonts/minic.json"),
                FontType.Delta => JsonHandler.Load<FontFace>(@"../assets/fonts/delton.json"),
                FontType.Foxtrot => JsonHandler.Load<FontFace>(@"../assets/fonts/foxtrot.json"),
                _ => JsonHandler.Load<FontFace>(@"../assets/fonts/orikos.json"),
            };

        public static GammaPalette GetPalette(PaletteType type)
            => type switch
            {
                PaletteType.GammaGreen => GammaPalette.GammaGreen,
                PaletteType.Crimson => GammaPalette.NeonRed,
                PaletteType.Glass => GammaPalette.Glass,
                PaletteType.Default => GammaPalette.Default,
                PaletteType.Wumpite => GammaPalette.Wumpite,
                PaletteType.Lemon => GammaPalette.Lemon,
                PaletteType.Amber => GammaPalette.Amber,
                PaletteType.Taffy => GammaPalette.Bubblegum,
                PaletteType.Oceanic => GammaPalette.Oceanic,
                PaletteType.Polarity => GammaPalette.Polarity,
                PaletteType.Chocolate => GammaPalette.Chocolate,
                _ => GammaPalette.Default
            };

        public Bitmap DrawText(string content, FontFace font, Color color, ImageProperties properties = null)
            => _text.DrawText(content, font, color, properties);

        public Bitmap DrawText(string content, FontFace font, Gamma gamma, GammaPalette palette, ImageProperties properties = null)
            => _text.DrawText(content, font, palette[gamma], properties);

        public Bitmap DrawText(string content, FontFace font, Gamma gamma, PaletteType palette, ImageProperties properties = null)
            => _text.DrawText(content, font, GetPalette(palette)[gamma], properties);

        public Bitmap DrawText(string content, FontType font, Color color, ImageProperties properties = null)
            => _text.DrawText(content, GetFont(font), color, properties);

        public Bitmap DrawText(string content, FontType font, Gamma gamma, GammaPalette palette, ImageProperties properties = null)
            => _text.DrawText(content, GetFont(font), palette[gamma], properties);

        public Bitmap DrawText(string content, FontType font, Gamma gamma, PaletteType palette, ImageProperties properties = null)
            => _text.DrawText(content, GetFont(font), GetPalette(palette)[gamma], properties);

        public Bitmap DrawCard(CardInfo info, CardGroup deniedGroups)
        {
            var stopwatch = Stopwatch.StartNew();

            var card = new Drawable(info.Width, info.Height)
            {
                Properties = new DrawableProperties
                {
                    Palette = info.Palette,
                    Padding = info.Padding,
                    Margin = info.Margin
                }.WithScale(info.Scale)
            };

            if (info.Border != null && info.Border.Thickness > 0)
            {
                card.Border = new Border
                {
                    Allow = info.Border.Allowed,
                    Thickness = info.Border.Thickness,
                    Edge = info.Border.Edge,
                    Color = info.Border.Fill?.Palette[info.Border.Fill.Primary ?? Gamma.Max] ?? info.Palette[Gamma.Max]
                };
            }

            var cursor = new Cursor(info.CursorOriginX, info.CursorOriginY);
            var previous = new ComponentReference();

            foreach (CardComponent component in info.Components)
            {
                if (!deniedGroups.HasFlag(component.Info.Group))
                    component.Draw(ref card, ref cursor, ref previous);
            }

            if (info.Trim)
                card.Trim();

            Bitmap result = card.BuildAndDispose();
            stopwatch.Stop();

            Logger.Debug($"Card generated in {stopwatch.ElapsedMilliseconds} milliseconds.");

            return result;
        }

        // Creates a progress bar for an image of the same proportions, and applies the original opacity mask of the initial image to the progress bar
        public Bitmap SetBarMask(Bitmap mask, Color foreground, Color background, float progress, Direction direction = Direction.Right, MaskingMode mode = MaskingMode.Set)
        {
            Bitmap bar = ImageHelper.CreateProgressBar(foreground, background, mask.Width, mask.Height, progress, direction);
            return SetOpacityMask(mask, bar, mode);
        }

        public Bitmap SetGradientMask(Bitmap mask, GammaPalette palette, Direction direction = Direction.Right, GradientColorHandling colorHandling = GradientColorHandling.Snap, MaskingMode mode = MaskingMode.Set)
        {
            Bitmap gradient = ImageHelper.CreateGradient(palette, mask.Width, mask.Height, direction, colorHandling);
            return SetOpacityMask(mask, gradient, mode);
        }

        public Bitmap SetOpacityMask(Bitmap reference, Bitmap target, MaskingMode mode = MaskingMode.Set)
        {
            Grid<float> mask = ImageHelper.GetOpacityMask(reference);
            return ImageHelper.SetOpacityMask(target, mask, mode);
        }

        public void Dispose()
        {
            _text.Dispose();
        }
    }
}
