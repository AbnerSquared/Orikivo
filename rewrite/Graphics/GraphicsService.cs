using System;
using System.Drawing;
using System.Linq;
using Orikivo.Drawing;
using Orikivo.Drawing.Graphics2D;
using Orikivo.Unstable;

namespace Orikivo
{
    public class CardFormatter
    {
        public bool AutoSize { get; set; } = false;
        public bool UpperCaseName { get; set; } = false;

        public FontType Font { get; set; } = FontType.Foxtrot;
        public PaletteType Palette { get; set; } = PaletteType.Default;
    }

    /// <summary>
    /// Handles all of the rendering processes for Orikivo.
    /// </summary>
    public class GraphicsService : IDisposable
    {
        private readonly GraphicsWriter _graphics;

        public void Dispose()
        {
            _graphics.Dispose();
        }

        public GraphicsService()
        {
            PixelGraphicsConfig config = new PixelGraphicsConfig { CharMap = GetDefaultCharMap() };

            _graphics = new GraphicsWriter(config);

            _graphics.SetFont(GetFont(FontType.Orikos));
            _graphics.SetPalette(GammaPalette.Default);
        }

        public static char[][][][] GetDefaultCharMap()
            => OriJsonHandler.Load<char[][][][]>(@"../assets/char_map.json", new JsonCharArrayConverter());

        public static FontFace GetFont(FontType type)
            => type switch
            {
                FontType.Orikos => OriJsonHandler.Load<FontFace>(@"../assets/fonts/orikos.json"),
                FontType.Monori => OriJsonHandler.Load<FontFace>(@"../assets/fonts/monori.json"),
                FontType.Minic => OriJsonHandler.Load<FontFace>(@"../assets/fonts/minic.json"),
                FontType.Delton => OriJsonHandler.Load<FontFace>(@"../assets/fonts/delton.json"),
                FontType.Foxtrot => OriJsonHandler.Load<FontFace>(@"../assets/fonts/foxtrot.json"),
                _ => OriJsonHandler.Load<FontFace>(@"../assets/fonts/orikos.json"),
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

        public void SetPalette(PaletteType palette)
            => _graphics.SetPalette(GetPalette(palette));

        public void SetPalette(GammaPalette palette)
            => _graphics.SetPalette(palette);

        public Bitmap DrawString(string content, CanvasOptions options = null)
            => _graphics.DrawString(content, options);

        public Bitmap DrawString(string content, FontFace font, CanvasOptions options = null)
            => _graphics.DrawString(content, font, options);

        public Bitmap DrawString(string content, Color color, CanvasOptions options = null)
            => _graphics.DrawString(content, color, options);

        public Bitmap DrawString(string content, Gamma gamma, CanvasOptions options = null)
            => _graphics.DrawString(content, _graphics.Palette[gamma], options);

        public Bitmap DrawString(string content, Gamma gamma, GammaPalette palette, CanvasOptions options = null)
            => _graphics.DrawString(content, palette[gamma], options);

        public Bitmap DrawString(string content, Gamma gamma, PaletteType palette, CanvasOptions options = null)
            => _graphics.DrawString(content, GetPalette(palette)[gamma], options);

        public Bitmap DrawString(string content, FontFace font, Color color, CanvasOptions options = null)
            => _graphics.DrawString(content, font, color, options);

        public Bitmap DrawString(string content, FontFace font, Gamma gamma, CanvasOptions options = null)
            => _graphics.DrawString(content, font, _graphics.Palette[gamma], options);

        public Bitmap DrawString(string content, FontFace font, Gamma gamma, GammaPalette palette, CanvasOptions options = null)
            => _graphics.DrawString(content, font, palette[gamma], options);

        public Bitmap DrawString(string content, FontFace font, Gamma gamma, PaletteType palette, CanvasOptions options = null)
            => _graphics.DrawString(content, font, GetPalette(palette)[gamma], options);

        public Bitmap DrawCard(CardDetails details, PaletteType palette)
            => DrawCard(details, GetPalette(palette));

        public Bitmap DrawCard(CardDetails details, GammaPalette palette, bool autoResize = false, bool allCaps = false)
        {
            int level = ExpConvert.AsLevel(details.Exp);
            ulong nextExp = ExpConvert.AsExp(level + 1);
            ulong currentExp = ExpConvert.AsExp(level);

            CanvasOptions textConfig = new CanvasOptions { BackgroundColor = null, Padding = Padding.Empty };
            FontFace delton = GetFont(FontType.Delton);
            FontFace minic = GetFont(FontType.Minic);
            FontFace foxtrot = GetFont(FontType.Foxtrot);

            string iconSheetPath = @"../assets/icons/levels.png";
            Sheet iconSheet = new Sheet(iconSheetPath, 6, 6);
            string coinSheetPath = @"../assets/icons/coins.png";
            Sheet coinSheet = new Sheet(coinSheetPath, 8, 8);

            Console.WriteLine("Creating card base...");
            Drawable card = new Drawable(196, 40); // 150, 200 with 2px padding
            card.Scale = ImageScale.Medium;
            card.Padding = new Padding(2);
            card.Palette = palette;

            Console.WriteLine($"Drawing avatar...");
            #region Avatar
            // AVATAR

            BitmapLayer avatar = new BitmapLayer
            {
                Source = GraphicsUtils.SetPalette(BitmapHandler.GetHttpImage(details.AvatarUrl), palette),
                Offset = new Point(4, 4),
                Padding = new Padding(right: 2)
            };
            #endregion

            Console.WriteLine($"Drawing name...");
            #region Name
            // USERNAME
            BitmapLayer username = new BitmapLayer
            {
                Source = DrawString(allCaps ? details.Name.ToUpper() : details.Name, foxtrot, textConfig),
                Offset = new Point(
                    avatar.Offset.X + avatar.Source.Width + avatar.Padding.Width,
                    avatar.Offset.Y),
                Padding = new Padding(bottom: 2)
            };

            Console.WriteLine($"{12 * 9} || {username.Source.Width} + {username.Offset.X} + {username.Padding.Width}");
            #endregion

            Console.WriteLine($"Drawing state...");
            #region State
            // STATE
            BitmapLayer activity = new BitmapLayer
            {
                Source = DrawString(details.Program ?? details.Status.ToString(), minic, textConfig),
                Offset = new Point(
                    username.Offset.X,
                    username.Offset.Y + username.Source.Height + username.Padding.Height),
                Padding = new Padding(bottom: 2)
            };
            #endregion

            Console.WriteLine($"Drawing level icon...");
            #region Level Icon
            // LEVEL ICON
            BitmapLayer levelIcon = new BitmapLayer
            {
                Source = BitmapHandler.AutoCrop(iconSheet.GetSprite(1, 1), true),
                Offset = new Point(
                    username.Offset.X,
                    activity.Offset.Y + activity.Source.Height + activity.Padding.Height),
                Padding = new Padding(right: 1)
            };
            #endregion

            Console.WriteLine($"Drawing level counter...");
            #region Level Display
            // LEVEL COUNTER
            BitmapLayer levelCounter = new BitmapLayer
            {
                Source = DrawString(level.ToString(), delton, textConfig),
                Offset = new Point(
                    levelIcon.Offset.X + levelIcon.Source.Width + levelIcon.Padding.Width,
                    levelIcon.Offset.Y),
                Padding = new Padding(right: 5, bottom: 1)
            };
            #endregion

            Console.WriteLine($"Drawing experience bar...");
            #region EXP
            // EXP
            BitmapLayer expBar = new BitmapLayer
            {
                Source = _graphics.DrawFillable(GammaPalette.Default[Gamma.Standard],
                GammaPalette.Default[Gamma.Max], levelCounter.Source.Width, 2,
                RangeF.Convert(currentExp, nextExp, 0.0f, 1.0f, details.Exp), AngleF.Right),

                Offset = new Point(
                    levelCounter.Offset.X,
                    levelCounter.Offset.Y + levelCounter.Source.Height + levelCounter.Padding.Height)
            };
            #endregion

            Console.WriteLine($"Drawing coin icon...");
            #region Coin Icon
            // COIN ICON
            BitmapLayer coinIcon = new BitmapLayer
            {
                Source = BitmapHandler.AutoCrop(coinSheet.GetSprite(1, 1), true),

                Offset = new Point(
                    levelCounter.Offset.X + levelCounter.Source.Width + levelCounter.Padding.Width,
                    activity.Offset.Y + activity.Source.Height + activity.Padding.Height),

                Padding = new Padding(right: 2)
            };
            #endregion

            Console.WriteLine($"Drawing money counter...");
            #region Balance Display

            Calculator.MinusRem(details.Balance, details.Debt);
            bool inDebt = details.Debt > details.Balance;
            ulong balance = inDebt ? details.Debt - details.Balance : details.Balance - details.Debt;
            string bal = OriFormat.GetShortValue(balance, out PlaceValue value);
            // BALANCE COUNTER
            BitmapLayer moneyCounter = new BitmapLayer
            {
                Source = DrawString($"{(inDebt ? "-" : "")}{bal}", delton, inDebt ? Gamma.Bright : Gamma.Max, GammaPalette.Default, textConfig),

                Offset = new Point(
                    coinIcon.Offset.X + coinIcon.Source.Width + coinIcon.Padding.Width,
                    coinIcon.Offset.Y),

                Padding = new Padding(right: 1)
            };
            #endregion

            // TODO: Split the card drawing process into one method for each layer,
            // from which it can then utilize custom loadouts and such.

            card.AddLayer(avatar);
            card.AddLayer(username);
            card.AddLayer(activity);

            card.AddLayer(levelIcon);
            card.AddLayer(levelCounter);
            card.AddLayer(expBar);

            card.AddLayer(coinIcon);
            card.AddLayer(moneyCounter);

            Console.WriteLine($"Drawing border...");
            #region Border
            SolidLayer borderLeft = new SolidLayer
            {
                Color = GammaPalette.Default[Gamma.Max],
                Offset = new Point(0, 0),
                Width = 2,
                Height = 40
            };

            SolidLayer borderRight = new SolidLayer
            {
                Color = GammaPalette.Default[Gamma.Max],
                Offset = new Point(194, 0),
                Width = 2,
                Height = 40
            };

            SolidLayer borderTop = new SolidLayer
            {
                Color = GammaPalette.Default[Gamma.Max],
                Offset = new Point(2, 0),
                Width = 192,
                Height = 2
            };

            SolidLayer borderBottom = new SolidLayer
            {
                Color = GammaPalette.Default[Gamma.Max],
                Offset = new Point(2, 38),
                Width = 192,
                Height = 2
            };

            if (autoResize)
            {
                LayerSizeData widest = card.Layers
                    .Where(x => x is BitmapLayer)
                    .Select(x => new LayerSizeData { OffsetX = x.Offset.X, OffsetY = x.Offset.Y, Padding = x.Padding, SourceWidth = (x as BitmapLayer).Source.Width, SourceHeight = (x as BitmapLayer).Source.Height })
                    .OrderByDescending(x => x.Width).FirstOrDefault();

                LayerSizeData tallest = card.Layers
                    .Where(x => x is BitmapLayer)
                    .Select(x => new LayerSizeData { OffsetX = x.Offset.X, OffsetY = x.Offset.Y, Padding = x.Padding, SourceWidth = (x as BitmapLayer).Source.Width, SourceHeight = (x as BitmapLayer).Source.Height })
                    .OrderByDescending(x => x.Height).FirstOrDefault();

                Console.WriteLine($"Widest => {widest.OffsetX} + {widest.SourceWidth} + {widest.Padding.Width}\nTallest => {tallest.OffsetY} + {tallest.SourceHeight} + {tallest.Padding.Height}");

                // l border => new Point(0, 0)
                // r border => new Point(4 + widest.Width + 2, 0)
                // t border => new Point(2, 0)
                // b border => new Point(2, tallest.Height + 2)

                int lrHeight = tallest.Height + 4;
                int tbWidth = widest.Width;

                borderLeft.Height = lrHeight;

                borderRight.Offset = new Point(tbWidth + 2, 0);
                borderRight.Height = lrHeight;

                borderTop.Width = tbWidth;

                borderBottom.Offset = new Point(2, tallest.Height + 2);
                borderBottom.Width = tbWidth;

                card.Viewport = new Size(4 + widest.Width, lrHeight);
            }

            // BORDER
            
            #endregion


            Console.WriteLine($"Finalizing...");
            #region Finalization
            card.AddLayer(borderLeft);
            card.AddLayer(borderRight);
            card.AddLayer(borderTop);
            card.AddLayer(borderBottom);

            


            // SUFFIX OPTIONAL
            if (value > PlaceValue.H)
            {
                string suffixSheetPath = @"../assets/icons/suffixes.png";
                Sheet suffixSheet = new Sheet(suffixSheetPath, 6, 6);

                // BALANCE VALUE SUFFIX
                BitmapLayer valueSuffix = new BitmapLayer
                {
                    Source = BitmapHandler.AutoCrop(suffixSheet.GetSprite(1, (int)value), true),
                    Offset = new Point(
                        moneyCounter.Offset.X + moneyCounter.Source.Width + moneyCounter.Padding.Width,
                        moneyCounter.Offset.Y)
                };

                card.AddLayer(valueSuffix);
            }

            #endregion

            return card.BuildAndDispose();
        }
    }
}
