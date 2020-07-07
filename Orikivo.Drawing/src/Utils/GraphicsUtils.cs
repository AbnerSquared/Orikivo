using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo.Drawing
{
    public static class GraphicsUtils
    {
        private static readonly Size Bounds16_9 = new Size(400, 225);
        private static readonly Size Bounds4_3 = new Size(400, 300);
        private static readonly Size Bounds1_1 = new Size(300, 300);
        private static readonly Size Bounds1_2 = new Size(400, 200);
        private static readonly Size Bounds2_1 = new Size(150, 300); 

        private static readonly Size Thumbs16_9 = new Size(80, 45);
        private static readonly Size Thumbs4_3 = new Size(80, 60);
        private static readonly Size Thumbs1_1 = new Size(80, 80);
        private static readonly Size Thumbs1_2 = new Size(80, 40);
        private static readonly Size Thumbs2_1 = new Size(40, 80);

        public static Bitmap CreateRgbBitmap(Color color, int width, int height)
            => CreateRgbBitmap(new Grid<Color>(width, height, color).Values);

        public static Bitmap CreateRgbBitmap(Grid<GammaColor> colors)
            => CreateRgbBitmap(colors.ConvertAll<Color>().Values);

        public static Bitmap CreateRgbBitmap(Color[,] colors)
            => CreateBitmap(colors, false);

        public static Bitmap CreateArgbBitmap(Color[,] colors)
            => CreateBitmap(colors);

        private static Bitmap CreateBitmap(Color[,] colors, bool isArgb = true)
        {
            int width = colors.GetLength(1);
            int height = colors.GetLength(0);

            var bmp = new Bitmap(width, height, isArgb ? PixelFormat.Format32bppArgb : PixelFormat.Format32bppRgb);

            unsafe
            {
                var area = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData source = bmp.LockBits(area, ImageLockMode.WriteOnly, bmp.PixelFormat);

                int bitsPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int sourceWidth = source.Width * bitsPerPixel;
                int sourceHeight = source.Height;
                byte* ptr = (byte*)source.Scan0;

                Parallel.For(0, sourceHeight, y =>
                {
                    byte* row = ptr + (y * source.Stride);

                    for (int x = 0; x < sourceWidth; x += bitsPerPixel)
                    {
                        Color color = colors[y, x / bitsPerPixel];

                        if (isArgb)
                            row[x + 3] = color.A;

                        row[x + 2] = color.R;
                        row[x + 1] = color.G;
                        row[x] = color.B;
                    }
                });

                bmp.UnlockBits(source);
            }

            return bmp;
        }

        public static Grid<Color> GetPixels(Bitmap bmp)
        {
            var pixels = new Grid<Color>(bmp.Width, bmp.Height, Color.Empty);

            unsafe
            {
                var area = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData source = bmp.LockBits(area, ImageLockMode.ReadOnly, bmp.PixelFormat);

                int bitsPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int sourceWidth = source.Width * bitsPerPixel;
                int sourceHeight = source.Height;
                byte* ptr = (byte*)source.Scan0;

                Parallel.For(0, sourceHeight, y =>
                {
                    byte* row = ptr + (y * source.Stride);

                    for (int x = 0; x < sourceWidth; x += bitsPerPixel)
                    {
                        Color pixel = new GammaColor(row[x + 2], row[x + 1], row[x], row[x + 3]);
                        pixels.SetValue(pixel, x / bitsPerPixel, y);
                    }
                });

                bmp.UnlockBits(source);
            }

            return pixels;
        }

        public static Grid<Color> GetSolidMask(Bitmap bmp)
        {
            var pixels = new Grid<Color>(bmp.Width, bmp.Height, Color.Transparent);

            unsafe
            {
                var area = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData source = bmp.LockBits(area, ImageLockMode.ReadOnly, bmp.PixelFormat);
                int bitsPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int sourceWidth = source.Width * bitsPerPixel;
                int sourceHeight = source.Height;
                
                byte* ptr = (byte*)source.Scan0;

                Parallel.For(0, sourceHeight, y =>
                {
                    byte* row = ptr + (y * source.Stride);

                    for (int x = 0; x < sourceWidth; x += bitsPerPixel)
                        pixels.SetValue(row[x + 3] > 0 ? Color.Black : Color.White, x / bitsPerPixel, y);
                });

                bmp.UnlockBits(source);
            }

            return pixels;
        }

        public static Bitmap SetPalette(Bitmap bmp, GammaPalette colors)
        {
            unsafe
            {
                var area = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData source = bmp.LockBits(area, ImageLockMode.ReadWrite, bmp.PixelFormat);

                int bitsPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int sourceWidth = source.Width * bitsPerPixel;
                int sourceHeight = source.Height;
                byte* ptr = (byte*)source.Scan0;

                Parallel.For(0, sourceHeight, y =>
                {
                    byte* row = ptr + (y * source.Stride);

                    for (int x = 0; x < sourceWidth; x += bitsPerPixel)
                    {
                        GammaColor color = new GammaColor(row[x + 2], row[x + 1], row[x], row[x + 3]);
                        Color forcedColor = GammaColor.ClosestMatch(color, colors);

                        //row[x + 3] = forcedColor.A; // This is because you don't want to change opacity
                        row[x + 2] = forcedColor.R;
                        row[x + 1] = forcedColor.G;
                        row[x] = forcedColor.B;
                    }
                });

                bmp.UnlockBits(source);
            }

            return bmp;
        }

        /// <summary>
        /// Returns the size of the specified <see cref="ImageRatio"/> and <see cref="DiscordMedia"/>.
        /// </summary>
        public static Size GetRatioDims(ImageRatio ratio, DiscordMedia media)
        {
            bool isThumb = media == DiscordMedia.Thumbnail;
            return ratio switch
            {
                ImageRatio.Widescreen => isThumb ? Thumbs16_9 : Bounds16_9,
                ImageRatio.Wide => isThumb ? Thumbs2_1 : Bounds2_1,
                ImageRatio.Rectangle => isThumb ? Thumbs4_3 : Bounds4_3,
                ImageRatio.Square => isThumb ? Thumbs1_1 : Bounds1_1,
                ImageRatio.Tall => isThumb ? Thumbs1_2 : Bounds1_2,
                _ => throw new ArgumentException("The ratio type specified is not a valid ratio.")
            };
        }

        public static void ClipAndDrawImage(Graphics graphics, Bitmap image, Point point)
            => ClipAndDrawImage(graphics, image, new Rectangle(point, image.Size));

        public static void ClipAndDrawImage(Graphics graphics, Bitmap image, Rectangle clip)
        {
            graphics.SetClip(clip);
            graphics.DrawImage(image, clip);
            graphics.ResetClip();
        }

        internal static Rectangle ClampRectangle(Point origin, Size size, Point offset, Size innerSize)
        {
            int x = ClampPoint(origin.X, offset.X);
            int y = ClampPoint(origin.Y, offset.Y);

            int width = ClampLength(origin.X, offset.X, innerSize.Width, size.Width);
            int height = ClampLength(origin.Y, offset.Y, innerSize.Height, size.Height);

            return new Rectangle(x, y, width, height);
        }

        internal static CharMapIndex GetCharIndex(char c, char[][][][] map)
        {
            (int? I, int? X, int? Y) = new ValueTuple<int?, int?, int?>();
            
            foreach(char[][][] page in map)
            {
                if (page.Any(x => x.Any(y => y.Contains(c))))
                {
                    I = map.ToList().IndexOf(page);

                    foreach (char[][] row in page)
                    {
                        if (row.Any(x => x.Contains(c)))
                        {
                            Y = page.ToList().IndexOf(row);

                            foreach(char[] item in row)
                            {
                                if (item.Contains(c))
                                {
                                    X = row.ToList().IndexOf(item);
                                    return CharMapIndex.FromResult(c, I, X, Y);
                                }
                            }
                        }
                    }
                }
            }

            return CharMapIndex.FromResult(c, I, X, Y);
        }

        private static int ClampLength(int origin, int offset, int innerLength, int length)
            => innerLength - (Math.Abs(Math.Min(origin + offset, 0)) + Math.Max(offset + innerLength - length, 0));

        private static int ClampPoint(int origin, int offset)
            => offset < 0 ? Math.Abs(origin + offset) : 0;

        
    }
}
