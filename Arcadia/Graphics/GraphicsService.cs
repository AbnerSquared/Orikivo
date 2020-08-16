using System;
using System.Drawing;
using Orikivo.Drawing;
using Orikivo;
using System.Diagnostics;

namespace Arcadia.Graphics
{
    // What is the type of component being specified?
    // In what position is this component drawn?
    // if -1, it is inherited from the image crop
    // What is the padding for this component?
    // What is the horizontal offset of this component's origin?
    // What is the vertical offset of this component's origin?
    // Will the cursor be offset by this component's width or height once drawn?
    // Will the specified offset that was set be permanent or temporary?
    // Will the specified offset by added with the cursor offset or replaced?
    // GraphicsService should draw each component in increasing priority

    /* CardLayout.Default
     * Width = 192
     * Height = 32
     * Padding = 2
     * Margin = 2
     * Origin = (0, 0)
     * CanTrim = true [Do not trim the card if it cannot be trimmed]
     *
     * Border
     * Thickness = 2
     *
     * Avatar
     * Type = ComponentType.Image
     * Priority = 0
     * Width = 32
     * Height = 32
     * Padding = Right: 2
     * Margin = 0
     * CursorOffset = CursorOffset.X
     *
     * Name
     * Type = ComponentType.Text
     * Priority = 1
     * Width = Font.WidthOf(Details.Name)
     * Height = Font.CharHeight
     * Padding = Bottom: 2
     * CursorOffset = CursorOffset.Y
     *
     * Activity
     * Type = ComponentType.Text
     * Priority = 2
     * Width = Font.WidthOf(Details.Activity)
     * Height = Font.CharHeight
     * Padding = Bottom: 2
     * CursorOffset = CursorOffset.Y
     *
     * Level/ (Group)
     *
     * Icon
     * Type = ComponentType.Icon
     * Priority = 3
     * Reference = @"../assets/icons/levels.png"
     * ReferencePointer = (1, 1)
     * Width = 6
     * Height = 6
     * Padding = Right: 1
     * CursorOffset = CursorOffset.X
     * 
     * Counter
     * Priority = 4
     * Type = ComponentType.Counter
     * Width = Font.WidthOf(Details.Level)
     * Height = Font.CharHeight
     * Padding = Right: 5, Bottom: 1
     * CursorOffset = CursorOffset.None
     * CanShowSuffix = false
     *
     * Exp
     * Type = ComponentType.Bar
     * Priority = 5
     * Width = Counter.Width
     * Height = 2
     * OffsetY = Counter.Height + Counter.Padding.Height
     * OffsetHandling = OffsetHandling.Additive
     * CursorOffset = CursorOffset.X
     *
     * Money/ (Group)
     *
     * Icon
     * Type = ComponentType.Icon
     * Priority = 6
     * Reference = @"../assets/icons/coins.png"
     * ReferencePointer = (1, 1)
     * Width = 8
     * Height = 8
     * Padding = Right: 2
     * CursorOffset = CursorOffset.X
     * CursorOffsetHandling = OffsetHandling.Additive
     * CursorOffsetUsage = OffsetUsage.Temporary
     * CursorOffsetY = -1 (temporary set the cursor offset of y -1, then place it back once done)
     *
     * Counter
     * Type = ComponentType.Counter
     * Priority = 7
     * Width = Font.WidthOf(text)
     * Height = Font.CharHeight
     * Padding = Right: 1
     * CursorOffset = CursorOffset.X
     * CanShowSuffix = true
     * MaxLength = 3
     *
     * THE SUFFIX WILL BE HANDLED IN GRAPHICS_SERVICE
     * Suffix
     * Type = ComponentType.Icon
     * Priority = 8
     * Reference = @"../assets/icons/suffixes.png"
     * ReferencePointer = (1, NumberGroup)
     * Width = 6
     * Height = 6
     * CursorOffset = CursorOffset.None
     */

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
                FontType.Delton => JsonHandler.Load<FontFace>(@"../assets/fonts/delton.json"),
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
                _ => GammaPalette.Default
            };

        public static bool CanGroup(CardComponent component)
        {
            return component switch
            {
                CardComponent.Level => true,
                CardComponent.Money => true,
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

        private void DrawComponent(CardInfo info)
        {


            int previousWidth = 0;
            int previousHeight = 0;
            int previousPadWidth = 0;
            int previousPadHeight = 0;
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
            if (!properties.Deny.HasFlag(CardDeny.Avatar))
            {
                var imageProperties = DrawableProperties.Default;
                imageProperties.Padding = new Padding(right: 2);

                //card.AddLayer(DrawImage(ref cursor, ImageHelper.GetHttpImage(details.AvatarUrl), palette, imageProperties));
                var avatar = new BitmapLayer
                {
                    Source = ImageHelper.ForcePalette(ImageHelper.GetHttpImage(details.AvatarUrl), palette),
                    Offset = cursor
                };
                avatar.Properties.Padding = new Padding(right: 2);

                cursor.X += avatar.Source.Width + avatar.Properties.Padding.Width;
                card.AddLayer(avatar);
            }
           
            // USERNAME
            if (!properties.Deny.HasFlag(CardDeny.Username))
            {
                var usernameGamma = properties.Gamma[CardComponent.Username] ?? Gamma.Max;
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
            if (!properties.Deny.HasFlag(CardDeny.Activity))
            {
                var activityGamma = properties.Gamma[CardComponent.Activity] ?? Gamma.Max;
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
            if (!properties.Deny.HasFlag(CardDeny.Level))
            {
                var iconSheet = new Sheet(@"../assets/icons/levels.png", 6, 6);
                
                // ICON
                var levelGamma = properties.Gamma[CardComponent.Level] ?? Gamma.Max;
                var icon = new BitmapLayer
                {
                    Source = ImageHelper.Trim(ImageHelper.SetColorMap(iconSheet.GetSprite(1, 1), GammaPalette.Default, Palette), true),
                    Offset = cursor
                };
                icon.Properties.Padding = new Padding(right: 1);

                cursor.X += icon.Source.Width + icon.Properties.Padding.Width;
                card.AddLayer(icon);

                // COUNTER
                int level = ExpConvert.AsLevel(details.Exp);
                ulong currentExp = ExpConvert.AsExp(level);
                ulong nextExp = ExpConvert.AsExp(level + 1);

                var counter = new BitmapLayer
                {
                    Source = DrawText(level.ToString(), GetFont(FontType.Delton), levelGamma, defaultProperties),
                    Offset = cursor
                };
                counter.Properties.Padding = new Padding(right: 5, bottom: 1);

                card.AddLayer(counter);

                // EXP
                if (!properties.Deny.HasFlag(CardDeny.Exp))
                {
                    var fillGamma = properties.Gamma[CardComponent.Exp] ?? Gamma.Max;
                    var emptyGamma = properties.Gamma[CardComponent.Bar] ?? Gamma.Standard;

                    var toNextLevel = RangeF.Convert(currentExp, nextExp, 0, 1, details.Exp);

                    int counterHeight = counter.Source.Height + counter.Properties.Padding.Height;
                    var exp = new BitmapLayer
                    {
                        // TODO: 
                        Source = ImageHelper.CreateProgressBar(palette[emptyGamma], palette[fillGamma],
                            counter.Source.Width, 2, toNextLevel),
                        Offset = new Coordinate(cursor.X, cursor.Y + counterHeight)
                    };

                    card.AddLayer(exp);
                }

                cursor.X += counter.Source.Width + counter.Properties.Padding.Width;
            }

            // MONEY
            if (!properties.Deny.HasFlag(CardDeny.Money))
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

                ulong money = inDebt ? details.Debt - details.Balance : details.Balance - details.Debt;
                string balance = Format.Condense(money, out NumberGroup suffix);

                string text = $"{(inDebt ? "-" : "")}{balance}";

                // COUNTER
                // CardComponent: Name of all colorable components
                var moneyGamma = properties.Gamma[CardComponent.Money] ?? Gamma.Max;

                // TODO: Dim color for debt
                var counter = new BitmapLayer(DrawText(text, GetFont(FontType.Delton), moneyGamma, defaultProperties));
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
        public Bitmap SetProgressMask(Bitmap image, Color foreground, Color background, float progress, Direction direction = Direction.Right)
        {
            Bitmap bar = ImageHelper.CreateProgressBar(background, foreground, image.Width, image.Height, progress, direction);

            Grid<float> mask = ImageHelper.GetOpacityMask(image);

            return ImageHelper.SetOpacityMask(bar, mask);
        }

        public void Dispose()
        {
            _text.Dispose();
        }
    }
}
