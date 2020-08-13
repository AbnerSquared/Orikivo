using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo.Drawing
{
    // TODO: Send methods over to ImageHelper
    // TODO: Rename to ImageBuilder
    public static class ImageEditor
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

        // TODO: Implement OuterColor and InnerColor
        public static Bitmap SetBorder(Bitmap image, Color color, int thickness, BorderEdge edge = BorderEdge.Outside, BorderAllow allow = BorderAllow.All)
        {
            if (!(thickness > 0))
                return image;

            int innerLen = GetInnerLength(thickness, edge);
            int maxInnerLen = (int)Math.Floor((double)image.Width / 2);

            bool hasLeft = allow.HasFlag(BorderAllow.Left);
            bool hasRight = allow.HasFlag(BorderAllow.Right);
            bool hasTop = allow.HasFlag(BorderAllow.Top);
            bool hasBottom = allow.HasFlag(BorderAllow.Bottom);

            if (innerLen > maxInnerLen)
                innerLen = maxInnerLen;

            /*
            bool fillInner = false;
            if (innerLen >= maxInnerLen)
            {
                fillInner = true;
                innerLen = 0;
            }*/

            int outerLen = GetOuterLength(thickness, edge);
            int imageWidth = image.Width;
            int imageHeight = image.Height;

            if (outerLen > 0)
            {
                if (hasLeft)
                    imageWidth += outerLen;

                if (hasRight)
                    imageWidth += outerLen;

                if (hasTop)
                    imageHeight += outerLen;

                if (hasBottom)
                    imageHeight += outerLen;
            }


            var result = new Bitmap(imageWidth, imageHeight);

            using Graphics g = Graphics.FromImage(result);
            var brush = new SolidBrush(color);

            // Do this first to overlap brush marks with center borders
            ClipAndDrawImage(g, image, hasLeft ? outerLen : 0, hasTop ? outerLen : 0);

            // Left Border
            if (hasLeft)
            {
                int width = outerLen + innerLen;
                int height = imageHeight;

                g.FillRectangle(brush, 0, 0, width, height);
            }

            // Right Border
            if (hasRight)
            {
                int x = image.Width - innerLen;

                if (hasLeft)
                    x += outerLen;

                int width = outerLen + innerLen;
                int height = imageHeight;

                g.FillRectangle(brush, x, 0, width, height);
            }

            // Top Border
            if (hasTop)
            {
                int width = imageWidth;
                int height = outerLen + innerLen;

                g.FillRectangle(brush, 0, 0, width, height);
            }

            // Bottom Border
            if (hasBottom)
            {
                int y = image.Height - innerLen;

                if (hasTop)
                    y += outerLen;

                int width = imageWidth;
                int height = outerLen + innerLen;

                g.FillRectangle(brush, 0, y, width, height);
            }

            brush.Dispose();

            return result;
        }

        public static Bitmap CreateSolid(Color color, int width, int height)
            => CreateArgbBitmap(new Grid<Color>(width, height, color).Values);

        // TODO: Implement the usage of angles for fillables (AngleF angle)
        public static Bitmap CreateFillable(Color background, Color foreground, int width, int height, float progress)
        {
            int foreWidth = (int)Math.Floor(RangeF.Convert(0, 1, 0, width, progress));
            var result = new Bitmap(width, height);

            using Graphics g = Graphics.FromImage(result);

            using (var backBrush = new SolidBrush(background))
            {
                var backClip = new Rectangle(foreWidth, 0, width - foreWidth, height);
                g.SetClip(backClip);
                g.FillRectangle(backBrush, backClip);
                g.ResetClip();
            }

            using (var foreBrush = new SolidBrush(foreground))
            {
                var foreClip = new Rectangle(0, 0, foreWidth, height);
                g.SetClip(foreClip);
                g.FillRectangle(foreBrush, foreClip);
                g.ResetClip();
            }

            return result;
        }

        // TODO: Implement angled gradient generation (AngleF angle)
        public static Bitmap CreateGradient(Color from, Color to, int width, int height)
        {
            var pixels = new Grid<Color>(width, height, from);

            for (int i = 0; i < width; i++)
            {
                float strength = RangeF.Convert(0, width, 0, 1, i);
                Color blend = ImmutableColor.Blend(from, to, strength);
                pixels.SetColumn(i, blend);
            }

            return CreateRgbBitmap(pixels.Values);
        }

        // TODO: Implement angled gradient generation (AngleF angle)
        public static Bitmap CreateGradient(Dictionary<float, Color> markers, int width, int height, GradientColorHandling colorHandling = GradientColorHandling.Blend)
        {
            var pixels = new Grid<Color>(width, height, GetInitialColor(markers));

            for (int i = 0; i < width; i++)
            {
                float progress = RangeF.Convert(0, width, 0, 1, i);
                pixels.SetColumn(i, GetColorAtProgress(markers, progress, colorHandling));
            }

            return CreateRgbBitmap(pixels.Values);
        }

        // TODO: Implement angled gradient generation (AngleF angle)
        public static Bitmap CreateGradient(GammaPalette palette, int width, int height, AngleF angle, GradientColorHandling colorHandling = GradientColorHandling.Snap)
        {
            const float colorDist = 1.0f / GammaPalette.RequiredLength;
            var markers = new Dictionary<float, Color>();

            for (int i = 0; i < GammaPalette.RequiredLength; i++)
                markers.Add(colorDist * i, palette[i]);

            return CreateGradient(markers, width, height, colorHandling);
        }

        public static Bitmap CreateRgbBitmap(Color color, int width, int height)
            => CreateRgbBitmap(new Grid<Color>(width, height, color).Values);

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
                var area = new Rectangle(0, 0, width, height);
                BitmapData source = bmp.LockBits(area, ImageLockMode.WriteOnly, bmp.PixelFormat);

                int bitsPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int sourceWidth = source.Width * bitsPerPixel;
                int sourceHeight = source.Height;
                var ptr = (byte*)source.Scan0;

                Parallel.For(0, sourceHeight, y =>
                {
                    byte* row = ptr + y * source.Stride;

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

        public static Grid<Color> GetPixelData(Bitmap image)
        {
            var pixels = new Grid<Color>(image.Width, image.Height, Color.Empty);

            unsafe
            {
                var area = new Rectangle(0, 0, image.Width, image.Height);
                BitmapData source = image.LockBits(area, ImageLockMode.ReadOnly, image.PixelFormat);
                int bitsPerPixel = Image.GetPixelFormatSize(image.PixelFormat) / 8;
                int sourceWidth = source.Width * bitsPerPixel;
                int sourceHeight = source.Height;
                var ptr = (byte*)source.Scan0;

                Parallel.For(0, sourceHeight, y =>
                {
                    byte* row = ptr + y * source.Stride;

                    for (int x = 0; x < sourceWidth; x += bitsPerPixel)
                    {
                        Color pixel = new ImmutableColor(row[x + 2], row[x + 1], row[x], row[x + 3]);
                        pixels.SetValue(pixel, x / bitsPerPixel, y);
                    }
                });

                image.UnlockBits(source);
            }

            return pixels;
        }

        public static Grid<Color> GetBinaryMask(Bitmap image)
        {
            var pixels = new Grid<Color>(image.Width, image.Height, Color.Transparent);

            unsafe
            {
                var area = new Rectangle(0, 0, image.Width, image.Height);
                BitmapData source = image.LockBits(area, ImageLockMode.ReadOnly, image.PixelFormat);
                int bitsPerPixel = Image.GetPixelFormatSize(image.PixelFormat) / 8;
                int sourceWidth = source.Width * bitsPerPixel;
                int sourceHeight = source.Height;
                var ptr = (byte*)source.Scan0;

                Parallel.For(0, sourceHeight, y =>
                {
                    byte* row = ptr + (y * source.Stride);

                    for (int x = 0; x < sourceWidth; x += bitsPerPixel)
                        pixels.SetValue(row[x + 3] > 0 ? Color.Black : Color.White, x / bitsPerPixel, y);
                });

                image.UnlockBits(source);
            }

            return pixels;
        }

        public static Grid<float> GetOpacityMask(Bitmap image)
        {
            var mask = new Grid<float>(image.Size, 0);
            Grid<Color> pixels = GetPixelData(image);

            mask.SetEachValue((x, y) => RangeF.Convert(0, 255, 0, 1, pixels[x, y].A));
            return mask;
        }

        public static Bitmap SetOpacityMask(Bitmap image, Grid<float> mask)
        {
            if (mask.Size != image.Size)
                throw new ArgumentException("The opacity mask specified must be the same size as the binding image");

            unsafe
            {
                var area = new Rectangle(0, 0, image.Width, image.Height);
                BitmapData source = image.LockBits(area, ImageLockMode.WriteOnly, image.PixelFormat);
                int bitsPerPixel = Image.GetPixelFormatSize(image.PixelFormat) / 8;
                int sourceWidth = source.Width * bitsPerPixel;
                int sourceHeight = source.Height;
                var ptr = (byte*) source.Scan0;

                Parallel.For(0, sourceHeight, y =>
                {
                    byte* row = ptr + y * source.Stride;
                    int pX = 0;
                    for (var x = 0; x < sourceWidth; x += bitsPerPixel)
                    {
                        var alpha = (byte) Math.Floor(RangeF.Convert(0, 1, 0, 255, mask[pX, y]));
                        row[x + 3] = alpha;
                        pX++;
                    }
                });

                image.UnlockBits(source);
            }

            return image;
        }

        public static Bitmap ForcePalette(Bitmap bmp, GammaPalette colors)
        {
            unsafe
            {
                var area = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData source = bmp.LockBits(area, ImageLockMode.ReadWrite, bmp.PixelFormat);
                int bitsPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int sourceWidth = source.Width * bitsPerPixel;
                int sourceHeight = source.Height;
                var ptr = (byte*)source.Scan0;

                Parallel.For(0, sourceHeight, y =>
                {
                    byte* row = ptr + y * source.Stride;

                    for (int x = 0; x < sourceWidth; x += bitsPerPixel)
                    {
                        var color = new ImmutableColor(row[x + 2], row[x + 1], row[x], row[x + 3]);
                        Color match = ImmutableColor.ClosestMatch(color, colors);

                        row[x + 2] = match.R;
                        row[x + 1] = match.G;
                        row[x] = match.B;
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

        // TODO: Determine if this method works the same as Drawable.Build()
        public static void DrawImage(Bitmap image, Bitmap inner, int viewportWidth, int viewportHeight, int innerX, int innerY, int innerOffsetX, int innerOffsetY)
        {
            using Graphics g = Graphics.FromImage(image);
            if (innerOffsetX < 0 || innerOffsetX + image.Width > viewportWidth ||
                innerOffsetY < 0 || innerOffsetY + image.Height > viewportHeight)
            {
                // NOTE: The default origin in this case would just be the origin of the viewport
                Rectangle visible = ClampRectangle(Point.Empty, new Size(viewportWidth, viewportHeight), new Point(innerOffsetX, innerOffsetY), inner.Size);
                using Bitmap overlap = ImageHelper.Crop(inner, visible);
                ClipAndDrawImage(g, overlap, innerX, innerY);
            }
            else
            {
                ClipAndDrawImage(g, inner, innerX, innerY);
            }
        }

        public static void ClipAndDrawImage(Graphics graphics, Bitmap image, int x, int y)
            => ClipAndDrawImage(graphics, image, new Rectangle(x, y, image.Width, image.Height));


        public static void ClipAndDrawImage(Graphics graphics, Bitmap image, Point point)
            => ClipAndDrawImage(graphics, image, new Rectangle(point, image.Size));

        public static void ClipAndDrawImage(Graphics graphics, Bitmap image, Rectangle clip)
        {
            graphics.SetClip(clip);
            graphics.DrawImage(image, clip);
            graphics.ResetClip();
        }

        // NOTE: Clipping
        internal static Rectangle ClampRectangle(Point origin, Size maxSize, Point offset, Size innerSize)
        {
            int x = ClampPoint(origin.X, offset.X);
            int y = ClampPoint(origin.Y, offset.Y);

            int width = ClampLength(origin.X, offset.X, innerSize.Width, maxSize.Width);
            int height = ClampLength(origin.Y, offset.Y, innerSize.Height, maxSize.Height);

            return new Rectangle(x, y, width, height);
        }

        internal static CharIndex GetCharIndex(char c, char[][][][] map)
        {
            (int? i, int? x, int? y) = (-1, -1, -1);

            foreach(char[][][] page in map)
            {
                i++;

                if (!page.Any(r => r.Any(g => g.Contains(c))))
                    continue;

                foreach (char[][] row in page)
                {
                    y++;

                    if (!row.Any(g => g.Contains(c)))
                        continue;

                    foreach(char[] group in row)
                    {
                        x++;

                        if (!group.Contains(c))
                            continue;

                        return CharIndex.FromResult(c, i, x, y);
                    }
                }
            }

            return CharIndex.FromResult(c, 0, 0, 0);
        }

        internal static int GetMidpoint(int length)
            => (int)MathF.Floor(length / (float) 2);

        internal static Point GetOrigin(Size size, OriginAnchor anchor)
        {
            return anchor switch
            {
                OriginAnchor.TopLeft => new Point(0, 0),
                OriginAnchor.Top => new Point(GetMidpoint(size.Width), 0),
                OriginAnchor.TopRight => new Point(size.Width, 0),

                OriginAnchor.Left => new Point(0, GetMidpoint(size.Height)),
                OriginAnchor.Center => new Point(GetMidpoint(size.Width), GetMidpoint(size.Height)),
                OriginAnchor.Right => new Point(size.Width, GetMidpoint(size.Height)),

                OriginAnchor.BottomLeft => new Point(0, size.Height),
                OriginAnchor.Bottom => new Point(GetMidpoint(size.Width), size.Height),
                OriginAnchor.BottomRight => new Point(size.Width, size.Height),

                _ => new Point(0, 0)
            };
        }

        private static int ClampLength(int origin, int offset, int innerLength, int length)
            => innerLength - (Math.Abs(Math.Min(origin + offset, 0)) + Math.Max(offset + innerLength - length, 0));

        private static int ClampPoint(int origin, int offset)
            => offset < 0 ? Math.Abs(origin + offset) : 0;

        // NOTE: Borders
        private static int GetInnerLength(int width, BorderEdge edge)
            => edge switch
            {
                BorderEdge.Outside => 0,
                BorderEdge.Center => (int)MathF.Floor(width / (float)2),
                BorderEdge.Inside => width,
                _ => 0
            };

        private static int GetOuterLength(int width, BorderEdge edge)
            => edge switch
            {
                BorderEdge.Outside => width,
                BorderEdge.Center => width - (int)MathF.Floor(width / (float)2),
                BorderEdge.Inside => 0,
                _ => width
            };

        // NOTE: Gradients
        private static Color GetInitialColor(Dictionary<float, Color> markers)
        {
            return markers.OrderBy(x => x.Key).First().Value;
        }

        private static Color GetColorAtProgress(Dictionary<float, Color> markers, float progress, GradientColorHandling colorHandling = GradientColorHandling.Blend)
        {
            float? lastClosest = GetLastClosest(markers, progress);
            float? nextClosest = GetNextClosest(markers, progress);

            if (!lastClosest.HasValue && !nextClosest.HasValue)
                throw new Exception("Could not find a marker at the specified progress.");

            if (!lastClosest.HasValue)
                return markers[nextClosest.Value];

            if (!nextClosest.HasValue)
                return markers[lastClosest.Value];

            switch (colorHandling)
            {
                case GradientColorHandling.Snap:
                    return lastClosest.Value < nextClosest.Value ? markers[lastClosest.Value] : markers[nextClosest.Value];

                default:
                    float strength = RangeF.Convert(lastClosest.Value, nextClosest.Value, 0, 1, progress);
                    return ImmutableColor.Blend(markers[lastClosest.Value], markers[nextClosest.Value], strength);
            }
        }

        private static float? GetLastClosest(Dictionary<float, Color> markers, float progress)
        {
            if (markers.Keys.Any(x => x < progress))
            {
                return markers.Keys
                    .Where(x => x < progress)
                    .OrderBy(x => MathF.Abs(progress - x))
                    .First();
            }

            return null;
        }

        private static float? GetNextClosest(Dictionary<float, Color> markers, float progress)
        {
            if (markers.Keys.Any(x => x >= progress))
            {
                return markers.Keys
                    .Where(x => x >= progress)
                    .OrderBy(x => MathF.Abs(progress - x))
                    .First();
            }

            return null;
        }
    }
}
