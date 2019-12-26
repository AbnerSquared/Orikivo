using System;
using System.Drawing;
using System.Linq;
using Discord.WebSocket;
using Orikivo.Drawing;

namespace Orikivo
{
    // Handles all rendering and such.
    /// <summary>
    /// Handles all of the rendering processes for Orikivo.
    /// </summary>
    public class GraphicsService : IDisposable
    {
        private readonly PixelGraphics _graphics;

        public void Dispose()
        {
            _graphics.Dispose();
        }

        public GraphicsService()
        {
            FontFace font = OriJsonHandler.Load<FontFace>(@"../assets/fonts/orikos.json");
            char[][][][] charMap = OriJsonHandler.Load<char[][][][]>(@"../assets/char_map.json", new JsonCharArrayConverter());
            PixelGraphicsConfig config = new PixelGraphicsConfig { CharMap = charMap, Colors = GammaColorMap.Default };

            _graphics = new PixelGraphics(config);

            _graphics.SetFont(font);
        }

        public Bitmap DrawString(string content, GammaColorMap colors = null, CanvasOptions options = null, FontFace font = null)
        {
            colors ??= GammaColorMap.Default;
            options ??= new CanvasOptions { Padding = Padding.Empty, BackgroundColor = null };

            _graphics.Colors = colors;

            return _graphics.DrawString(content, font ?? _graphics.CurrentFont, options);
        }

        public CardDetails GetCardDetails(OriUser user, SocketUser socket)
        {
            CardDetails info = new CardDetails
            {
                Activity = socket.Activity?.Name,
                Ascent = 0,
                Status = socket.Status,
                Name = user.Name,
                AvatarUrl = socket.GetAvatarUrl(Discord.ImageFormat.Png, 32),
                Exp = user.Exp,
                Level = ExpConvert.AsLevel(user.Exp),
                NextLevelExp = ExpConvert.AsExp(ExpConvert.AsLevel(user.Exp) + 1),
                CurrentLevelExp = ExpConvert.AsExp(ExpConvert.AsLevel(user.Exp))
            };

            return info;
        }

        public Bitmap DrawCard(CardDetails details, bool autoResize = false, bool useFoxtrot = true, bool allCaps = false)
        {
            Console.WriteLine("Importing fonts...");
            CanvasOptions textConfig = new CanvasOptions { BackgroundColor = null, Padding = Padding.Empty };
            FontFace delton = OriJsonHandler.Load<FontFace>(@"../assets/fonts/delton.json");
            FontFace minic = OriJsonHandler.Load<FontFace>(@"../assets/fonts/minic.json");
            FontFace foxtrot = OriJsonHandler.Load<FontFace>(@"../assets/fonts/foxtrot.json");

            string iconSheetPath = @"../assets/icons/levels.png";
            Sheet iconSheet = new Sheet(iconSheetPath, 6, 6);
            string coinSheetPath = @"../assets/icons/coins.png";
            Sheet coinSheet = new Sheet(coinSheetPath, 8, 8);

            Console.WriteLine("Creating card base...");
            Drawable card = new Drawable(196, 40); // 150, 200 with 2px padding
            card.Scale = ImageScale.Medium;
            card.Padding = new Padding(2);
            card.Colors = GammaColorMap.Default;

            Console.WriteLine($"Drawing avatar...");
            #region Avatar
            // AVATAR

            BitmapLayer avatar = new BitmapLayer
            {
                Source = BitmapUtils.GetHttpImage(details.AvatarUrl),
                Offset = new Point(4, 4),
                Padding = new Padding(right: 2)
            };
            #endregion

            Console.WriteLine($"Drawing name...");
            #region Name
            // USERNAME
            BitmapLayer username = new BitmapLayer
            {
                Source = DrawString(allCaps ? details.Name.ToUpper() : details.Name, GammaColorMap.Default, textConfig, useFoxtrot ? foxtrot : null),
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
                Source = DrawString(details.Activity ?? details.Status.ToString(), GammaColorMap.Default, textConfig, minic),
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
                Source = BitmapUtils.AutoCrop(iconSheet.GetBitmap(1, 1), true),
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
                Source = DrawString(details.Level.ToString(), GammaColorMap.Default, textConfig, delton),
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
                Source = _graphics.DrawFillable(GammaColorMap.Default[Gamma.Standard],
                GammaColorMap.Default[Gamma.Max], levelCounter.Source.Width, 2,
                RangeF.Convert(details.CurrentLevelExp, details.NextLevelExp, 0.0f, 1.0f, details.Exp)),

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
                Source = BitmapUtils.AutoCrop(coinSheet.GetBitmap(1, 1), true),

                Offset = new Point(
                    levelCounter.Offset.X + levelCounter.Source.Width + levelCounter.Padding.Width,
                    activity.Offset.Y + activity.Source.Height + activity.Padding.Height),

                Padding = new Padding(right: 2)
            };
            #endregion

            Console.WriteLine($"Drawing money counter...");
            #region Balance Display
            string bal = OriFormat.GetShortValue(details.Balance, out PlaceValue value);
            // BALANCE COUNTER
            BitmapLayer moneyCounter = new BitmapLayer
            {
                Source = DrawString(bal, GammaColorMap.Default, textConfig, delton),

                Offset = new Point(
                    coinIcon.Offset.X + coinIcon.Source.Width + coinIcon.Padding.Width,
                    coinIcon.Offset.Y),

                Padding = new Padding(right: 1)
            };
            #endregion

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
                Color = GammaColorMap.Default[Gamma.Max],
                Offset = new Point(0, 0),
                Width = 2,
                Height = 40
            };

            SolidLayer borderRight = new SolidLayer
            {
                Color = GammaColorMap.Default[Gamma.Max],
                Offset = new Point(194, 0),
                Width = 2,
                Height = 40
            };

            SolidLayer borderTop = new SolidLayer
            {
                Color = GammaColorMap.Default[Gamma.Max],
                Offset = new Point(2, 0),
                Width = 192,
                Height = 2
            };

            SolidLayer borderBottom = new SolidLayer
            {
                Color = GammaColorMap.Default[Gamma.Max],
                Offset = new Point(2, 38),
                Width = 192,
                Height = 2
            };

            if (autoResize)
            {
                Length widest = card.Layers
                    .Where(x => x is BitmapLayer)
                    .Select(x => new Length { OffsetX = x.Offset.X, OffsetY = x.Offset.Y, Padding = x.Padding, SourceWidth = (x as BitmapLayer).Source.Width, SourceHeight = (x as BitmapLayer).Source.Height })
                    .OrderByDescending(x => x.Width).FirstOrDefault();

                Length tallest = card.Layers
                    .Where(x => x is BitmapLayer)
                    .Select(x => new Length { OffsetX = x.Offset.X, OffsetY = x.Offset.Y, Padding = x.Padding, SourceWidth = (x as BitmapLayer).Source.Width, SourceHeight = (x as BitmapLayer).Source.Height })
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
                    Source = BitmapUtils.AutoCrop(iconSheet.GetBitmap(1, (int)value), true),
                    Offset = new Point(
                        moneyCounter.Offset.X + moneyCounter.Source.Width + moneyCounter.Padding.Width,
                        moneyCounter.Offset.Y)
                };

                card.AddLayer(valueSuffix);
            }

            #endregion

            return card.DisposeBuild();
        }
    }

    public class Length
    {
        public int OffsetX;
        public int OffsetY;
        public int SourceWidth;
        public int SourceHeight;
        public Padding Padding;

        public int Width => OffsetX + SourceWidth + Padding.Width;
        public int Height => OffsetY + SourceHeight + Padding.Height;
    }
}
