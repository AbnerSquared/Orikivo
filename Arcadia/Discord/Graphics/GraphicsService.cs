using System;
using System.Drawing;
using Orikivo.Drawing;
using Orikivo;
using System.Diagnostics;
using Orikivo.Framework;
using Orikivo.Text;

namespace Arcadia.Graphics
{
    /// <summary>
    /// Handles all of the rendering processes for Orikivo.
    /// </summary>
    public class GraphicsService : IDisposable
    {
        // TODO: Use the Minic FontFace to utilize a way to draw sub/superscript on larger fonts.
        private readonly TextFactory _text;

        public GraphicsService()
        {
            _text = new TextFactory(GetDefaultCharMap());
            _text.SetFont(GetFont(FontType.Orikos));
            Palette = GetPalette(PaletteType.Default);
        }

        public static char[][][][] GetDefaultCharMap()
            => JsonHandler.Load<char[][][][]>(@"../assets/char_map.json", new JsonCharArrayConverter());

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

        public static bool CanGroup(CardGroup component)
        {
            return component switch
            {
                CardGroup.Level => true,
                CardGroup.Money => true,
                _ => false
            };
        }

        public static Bitmap GetBitmap(Grid<Color> pixels, int scale = 1)
        {
            Bitmap image = ImageHelper.CreateRgbBitmap(pixels.Values);

            if (scale > 1)
            {
                // A cap is placed to prevent abuse.
                scale = scale > 5 ? 5 : scale;
                image = ImageHelper.Scale(image, scale, scale);
            }

            return image;
        }

        public GammaPalette Palette { get; set; }

        public void SetPalette(PaletteType palette)
            => Palette = GetPalette(palette);

        public void SetPalette(GammaPalette palette)
            => Palette = palette;

        public Bitmap DrawText(string content, ImageProperties properties = null)
            => _text.DrawText(content, properties);

        public Bitmap DrawText(string content, FontFace font, ImageProperties properties = null)
            => _text.DrawText(content, font, properties);

        public Bitmap DrawText(string content, FontType font, ImageProperties properties = null)
            => _text.DrawText(content, GetFont(font), properties);

        public Bitmap DrawText(string content, Color color, ImageProperties properties = null)
            => _text.DrawText(content, color, properties);

        public Bitmap DrawText(string content, Gamma gamma, ImageProperties properties = null)
            => _text.DrawText(content, Palette[gamma], properties);

        public Bitmap DrawText(string content, Gamma gamma, GammaPalette palette, ImageProperties properties = null)
            => _text.DrawText(content, palette[gamma], properties);

        public Bitmap DrawText(string content, Gamma gamma, PaletteType palette, ImageProperties properties = null)
            => _text.DrawText(content, GetPalette(palette)[gamma], properties);

        public Bitmap DrawText(string content, FontFace font, Color color, ImageProperties properties = null)
            => _text.DrawText(content, font, color, properties);

        public Bitmap DrawText(string content, FontFace font, Gamma gamma, ImageProperties properties = null)
            => _text.DrawText(content, font, Palette[gamma], properties);

        public Bitmap DrawText(string content, FontFace font, Gamma gamma, GammaPalette palette, ImageProperties properties = null)
            => _text.DrawText(content, font, palette[gamma], properties);

        public Bitmap DrawText(string content, FontFace font, Gamma gamma, PaletteType palette, ImageProperties properties = null)
            => _text.DrawText(content, font, GetPalette(palette)[gamma], properties);

        public Bitmap DrawText(string content, FontType font, Color color, ImageProperties properties = null)
            => _text.DrawText(content, GetFont(font), color, properties);

        public Bitmap DrawText(string content, FontType font, Gamma gamma, ImageProperties properties = null)
            => _text.DrawText(content, GetFont(font), Palette[gamma], properties);

        public Bitmap DrawText(string content, FontType font, Gamma gamma, GammaPalette palette, ImageProperties properties = null)
            => _text.DrawText(content, GetFont(font), palette[gamma], properties);

        public Bitmap DrawText(string content, FontType font, Gamma gamma, PaletteType palette, ImageProperties properties = null)
            => _text.DrawText(content, GetFont(font), GetPalette(palette)[gamma], properties);

        public Bitmap DrawCard(CardDetails details, PaletteType palette)
        {
            var properties = CardProperties.Default;
            properties.Palette = palette;

            return DrawCard(details, properties);
        }

        private static string GetString(Casing casing, string value)
            => casing switch
            {
                Casing.Upper => value.ToUpper(),
                Casing.Lower => value.ToLower(),
                _ => value
            };

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

            Logger.Debug($"Card generated in {Format.RawCounter(stopwatch.ElapsedMilliseconds)}.");

            return result;
        }

        public Bitmap DrawCard(CardDetails details, CardProperties properties = null)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            properties ??= CardProperties.Default;

            var palette = properties.PaletteOverride ?? GetPalette(properties.Palette);
            Palette = palette;
            var defaultProperties = new ImageProperties
            {
                Matte = null,
                Padding = Padding.Empty
            };

            // 150, 200 with 2px padding
            var card = new Drawable(192, 32)
            {
                Properties = new DrawableProperties
                {
                    Palette = palette,
                    Padding = properties.Padding,
                    Margin = properties.Padding
                }.WithScale(properties.Scale)
            };

            var cursor = new Cursor(0, 0);

            // AVATAR
            if (!properties.Deny.HasFlag(CardGroup.Avatar))
            {
                var imageProperties = DrawableProperties.Default;
                imageProperties.Padding = new Padding(right: 2);

                //card.AddLayer(DrawImage(ref cursor, ImageHelper.GetHttpImage(details.AvatarUrl), palette, imageProperties));
                var avatar = new BitmapLayer
                {
                    Source = ImageHelper.ForceLumenPalette(ImageHelper.GetHttpImage(details.AvatarUrl), palette),
                    Offset = cursor
                };
                avatar.Properties.Padding = new Padding(right: 2);

                cursor.X += avatar.Source.Width + avatar.Properties.Padding.Width;
                card.AddLayer(avatar);
            }

            // USERNAME
            if (!properties.Deny.HasFlag(CardGroup.Name))
            {
                var usernameGamma = properties.Gamma[CardGroup.Name] ?? Gamma.Max;
                var username = new BitmapLayer
                {
                    Source = DrawText(GetString(properties.Casing, details.Name), GetFont(properties.Font), usernameGamma, defaultProperties),
                    Offset = cursor
                };
                username.Properties.Padding = new Padding(bottom: 2);

                cursor.Y += username.Source.Height + username.Properties.Padding.Height;
                card.AddLayer(username);
            }

            // ACTIVITY
            if (!properties.Deny.HasFlag(CardGroup.Activity))
            {
                var activityGamma = properties.Gamma[CardGroup.Activity] ?? Gamma.Max;
                var activity = new BitmapLayer
                {
                    Source = DrawText(details.Program ?? details.Status.ToString(), GetFont(FontType.Minic), activityGamma, defaultProperties),
                    Offset = cursor
                };
                activity.Properties.Padding = new Padding(bottom: 2);

                cursor.Y += activity.Source.Height + activity.Properties.Padding.Height;
                card.AddLayer(activity);
            }

            // LEVEL
            if (!properties.Deny.HasFlag(CardGroup.Level))
            {
                var iconSheet = new Sheet(@"../assets/icons/levels.png", 6, 6);

                // ICON
                var levelGamma = properties.Gamma[CardGroup.Level] ?? Gamma.Max;
                var icon = new BitmapLayer
                {
                    Source = ImageHelper.Trim(ImageHelper.SetColorMap(iconSheet.GetSprite(1, 1), GammaPalette.Default, Palette), true),
                    Offset = cursor
                };
                icon.Properties.Padding = new Padding(right: 1);

                cursor.X += icon.Source.Width + icon.Properties.Padding.Width;
                card.AddLayer(icon);

                // COUNTER
                int level = ExpConvert.AsLevel(details.Exp, details.Ascent);
                ulong currentExp = ExpConvert.AsExp(level, details.Ascent);
                ulong nextExp = ExpConvert.AsExp(level + 1, details.Ascent);

                var counter = new BitmapLayer
                {
                    Source = DrawText(level.ToString(), GetFont(FontType.Delta), levelGamma, defaultProperties),
                    Offset = cursor
                };
                counter.Properties.Padding = new Padding(right: 5, bottom: 1);

                card.AddLayer(counter);

                // EXP
                if (!properties.Deny.HasFlag(CardGroup.Exp))
                {
                    var fillGamma = properties.Gamma[CardGroup.Exp] ?? Gamma.Max;
                    var emptyGamma = Gamma.Standard;

                    var toNextLevel = RangeF.Convert(currentExp, nextExp, 0, 1, details.Exp);

                    int counterHeight = counter.Source.Height + counter.Properties.Padding.Height;
                    var exp = new BitmapLayer
                    {
                        Source = ImageHelper.CreateProgressBar(palette[emptyGamma], palette[fillGamma],
                            counter.Source.Width, 2, toNextLevel),
                        Offset = new Coordinate(cursor.X, cursor.Y + counterHeight)
                    };

                    card.AddLayer(exp);
                }

                cursor.X += counter.Source.Width + counter.Properties.Padding.Width;
            }

            // MONEY
            if (!properties.Deny.HasFlag(CardGroup.Money))
            {
                var coinSheet = new Sheet(@"../assets/icons/coins.png", 8, 8);
                cursor.Y -= 1;
                // ICON
                var icon = new BitmapLayer(ImageHelper.Trim(ImageHelper.SetColorMap(coinSheet.GetSprite(1, 1), GammaPalette.Default, Palette), true));
                icon.Offset = cursor;
                icon.Properties.Padding = new Padding(right: 2);

                cursor.X += icon.Source.Width + icon.Properties.Padding.Width;
                card.AddLayer(icon);
                cursor.Y += 1;

                bool inDebt = details.Debt > details.Balance;

                long money = inDebt ? details.Debt - details.Balance : details.Balance - details.Debt;
                string balance = Format.Condense(money, out NumberGroup suffix);

                string text = $"{(inDebt ? "-" : "")}{balance}";

                // COUNTER
                // CardComponent: Name of all colorable components
                var moneyGamma = properties.Gamma[CardGroup.Money] ?? Gamma.Max;

                // TODO: Dim color for debt
                var counter = new BitmapLayer(DrawText(text, GetFont(FontType.Delta), moneyGamma, defaultProperties));
                counter.Offset = cursor;

                counter.Properties.Padding = new Padding(right: 1);

                cursor.X += counter.Source.Width + counter.Properties.Padding.Width;
                card.AddLayer(counter);

                // SUFFIX (OPTIONAL)
                if (suffix > NumberGroup.H)
                {
                    var suffixSheet = new Sheet(@"../assets/icons/suffixes.png", 6, 6);

                    var counterSuffix = new BitmapLayer
                    {
                        Source = ImageHelper.SetColorMap(suffixSheet.GetSprite(1, (int)suffix), GammaPalette.Default, Palette),
                        Offset = cursor
                    };

                    card.AddLayer(counterSuffix);
                }
            }

            if (properties.Border != 0)
            {
                var borderGamma = Gamma.Max;
                card.Border = new Border
                {
                    Allow = properties.Border,
                    Color = palette[borderGamma],
                    Edge = BorderEdge.Outside,
                    Thickness = 2
                };
            }

            if (properties.Trim)
                card.Trim();

            var result = card.BuildAndDispose();

            stopwatch.Stop();

            Console.WriteLine($"Card generated in {Format.RawCounter(stopwatch.ElapsedMilliseconds)}.");

            return result;
        }

        public DrawableLayer DrawImage(ref Cursor cursor, Bitmap image, GammaPalette palette, DrawableProperties properties = null)
        {
            var layer = new BitmapLayer(ImageHelper.ForcePalette(image, palette))
            {
                Properties = properties ?? DrawableProperties.Default,
                Offset = cursor
            };

            cursor.X += image.Width + layer.Properties.Padding.Width;
            return layer;
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
