using System;
using System.Drawing;
using System.Linq;
using Orikivo.Drawing;
using Orikivo.Drawing.Graphics2D;
using Orikivo.Desync;

namespace Orikivo
{
    /// <summary>
    /// Handles all of the rendering processes for Orikivo.
    /// </summary>
    public class GraphicsService : IDisposable
    {
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
                PaletteType.NeonRed => GammaPalette.NeonRed,
                PaletteType.Glass => GammaPalette.Glass,
                PaletteType.Default => GammaPalette.Default,
                _ => GammaPalette.Default
            };

        public static Bitmap GetBitmap(Grid<Color> pixels, int scale = 1)
        {
            Bitmap image = ImageEditor.CreateRgbBitmap(pixels.Values);

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

        public Bitmap DrawCard(CardDetails details, CardProperties properties = null)
        {
            properties ??= CardProperties.Default;


            var palette = GetPalette(properties.Palette);
            var defaultProperties = new ImageProperties { Matte = null, Padding = Padding.Empty };
            FontFace delton = GetFont(FontType.Delton);
            FontFace minic = GetFont(FontType.Minic);
            FontFace foxtrot = GetFont(FontType.Foxtrot);

            var iconSheet = new Sheet(@"../assets/icons/levels.png", 6, 6);
            var coinSheet = new Sheet(@"../assets/icons/coins.png", 8, 8);

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

            var cursor = new Cursor();

            // AVATAR
            if (!properties.Deny.HasFlag(CardDeny.Avatar))
            {
                var avatar = new BitmapLayer
                {
                    Source = ImageEditor.ForcePalette(ImageHelper.GetHttpImage(details.AvatarUrl), palette),
                    Offset = new Point(cursor.X, cursor.Y)
                };
                avatar.Properties.Padding = new Padding(right: 2);

                cursor.X += avatar.Source.Width + avatar.Properties.Padding.Width;
                card.AddLayer(avatar);
            }
           
            // USERNAME
            if (!properties.Deny.HasFlag(CardDeny.Username))
            {
                var usernameGamma = properties.Gamma[CardComponentType.Username] ?? Gamma.Max;
                var username = new BitmapLayer
                {
                    Source = DrawText(GetString(properties.Casing, details.Name), GetFont(properties.Font), usernameGamma, defaultProperties),
                    Offset = new Point(cursor.X, cursor.Y)
                };

                username.Properties.Padding = new Padding(bottom: 2);

                cursor.Y += username.Source.Height + username.Properties.Padding.Height;
                card.AddLayer(username);
            }

            // ACTIVITY
            if (!properties.Deny.HasFlag(CardDeny.Activity))
            {
                var activityGamma = properties.Gamma[CardComponentType.Activity] ?? Gamma.Max;
                var activity = new BitmapLayer
                {
                    Source = DrawText(details.Program ?? details.Status.ToString(), minic, activityGamma, defaultProperties),
                    Offset = new Point(cursor.X, cursor.Y)
                };

                card.AddLayer(activity);
            }

            if (!(properties.Border == 0))
            {
                var borderGamma = properties.Gamma[CardComponentType.Border] ?? Gamma.Max;
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


            return card.BuildAndDispose();
        }

        public void Dispose()
        {
            _text.Dispose();
        }
    }
}
