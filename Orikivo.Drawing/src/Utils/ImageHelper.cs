using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static System.MathF;

namespace Orikivo.Drawing
{
    public static class ImageHelper
    {
        public static float GetImageRatio(ImageRatio ratio)
        {
            return ratio switch
            {
                ImageRatio.Square => 1.0f,
                ImageRatio.Tall => 0.5f,
                ImageRatio.Wide => 2.0f,
                ImageRatio.Rectangle => 4.0f / 3.0f,
                ImageRatio.Widescreen => 16.0f / 9.0f,
                _ => 1.0f
            };
        }

        internal static Bitmap Fill(Bitmap bmp, Color color)
        {
            using (Graphics graphics = Graphics.FromImage(bmp))
                graphics.Clear(color);

            return bmp;
        }

        internal static Bitmap ReplaceColor(Bitmap bmp, Color color, Color? alphaColor)
        {
            Color alpha = alphaColor ?? Color.Empty;

            Grid<Color> pixels = ImageHelper.GetPixelData(bmp);
            pixels.SetEachValue((pixel, x, y) => pixel.Equals(alpha) ? color : pixel);

            return ImageHelper.CreateArgbBitmap(pixels.Values);
        }

        public static ColorMap[] CreateColorTable(Color[] fromColors, Color[] toColors)
        {
            if (fromColors == null || toColors == null)
                throw new ArgumentException("One of the specified arrays is empty or null.");

            if (fromColors.Length != toColors.Length)
                throw new ArgumentException("The specified arrays are not equal in length.");

            var colors = fromColors.Select((x, i) => (x, toColors[i]));

            return CreateColorTable(colors.ToArray());
        }

        public static ColorMap[] CreateColorTable(params (Color From, Color To)[] colors)
        {
            if (!(colors?.Length > 0))
                throw new Exception("At least one color map value must be specified.");

            return colors.Select(x => new ColorMap
            {
                OldColor = x.From,
                NewColor = x.To                
            }).ToArray();
        }

        public static Bitmap SetColorMap(Bitmap bmp, ColorMap[] table)
        {
            var result = new Bitmap(bmp.Width, bmp.Height);

            using (Graphics graphics = Graphics.FromImage(result))
            {
                var attributes = new ImageAttributes();
                var destination = new Rectangle(0, 0, bmp.Width, bmp.Height);

                attributes.SetRemapTable(table, ColorAdjustType.Bitmap);
                graphics.DrawImage(bmp, destination, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
            }

            return result;
        }

        public static Bitmap SetColorMap(Bitmap bmp, GammaPalette from, GammaPalette to)
            => SetColorMap(bmp, CreateColorTable(from.Values.Zip(to.Values, (x, y) => ((Color)x, (Color)y)).ToArray()));

        public static bool IsEmptyOrNull(Bitmap bmp)
        {
            if (bmp == null)
                return true;

            return GetPixelData(bmp).All(x => x.A == 0);
        }

        internal static int GetNonEmptyWidth(Bitmap bmp)
        {
            if (bmp == null)
                return 0;

            Grid<Color> pixels = GetPixelData(bmp);
            int nonEmptyLen = 0;

            for (int y = 0; y < pixels.Height; y++)
            {
                int xLen = 0;

                for (int x = 0; x < pixels.Width; x++)
                {
                    if (pixels.GetValue(x, y).A == 0)
                        continue;

                    xLen = x + 1;
                }

                if (xLen > nonEmptyLen)
                    nonEmptyLen = xLen;
            }

            return nonEmptyLen;
        }

        internal static int GetNonEmptyHeight(Bitmap bmp)
        {
            if (bmp == null)
                return 0;

            Grid<Color> pixels = GetPixelData(bmp);
            int nonEmptyLen = 0;

            for (int x = 0; x < pixels.Width; x++)
            {
                int yLen = 0;

                for (int y = 0; y < pixels.Height; y++)
                {
                    if (pixels.GetValue(x, y).A == 0)
                        continue;

                    yLen = y + 1;
                }

                if (yLen > nonEmptyLen)
                    nonEmptyLen = yLen;
            }

            return nonEmptyLen;
        }

        public static Size GetNonEmptySize(Bitmap bmp)
            => new Size(GetNonEmptyWidth(bmp), GetNonEmptyHeight(bmp));

        public static Bitmap DrawOutline(Bitmap bmp, int width, Color color, Color? alphaColor = null, bool drawOnNew = false)
        {
            Grid<Color> pixels = GetPixelData(bmp);
            var validPoints = new List<(int px, int py)>();
            Color alpha = alphaColor ?? Color.FromArgb(0, 0, 0, 0);

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    if (pixels[x, y] == alpha)
                        continue;

                    int minX = x - width;
                    int maxX = x + width;
                    int minY = y - width;
                    int maxY = y + width;

                    for (int m = minX; m <= maxX; m++)
                    {
                        if (m < 0)
                            continue;

                        if (m > bmp.Width - 1)
                            break;

                        for (int n = minY; n <= maxY; n++)
                        {
                            if (n > bmp.Height - 1)
                                break;

                            if (n < 0)
                                continue;

                            //Console.WriteLine($"{m} ({minX}/{maxX}) || {n} ({minY}/{maxY})");

                            if (!validPoints.Contains((m, n)))
                            {
                                if (pixels[m, n].Equals(alpha))
                                {
                                    //Console.WriteLine(pixels[m, n].ToString());
                                    validPoints.Add((m, n));
                                }
                            }
                        }
                    }
                }
            }

            if (drawOnNew)
            {
                var result = new Grid<Color>(bmp.Width, bmp.Height, alpha);

                var handle = new ImageHandle(bmp.Width, bmp.Height);

                foreach ((int pX, int pY) in validPoints)
                    handle.SetPixel(pX, pY, color);

                return handle.Bitmap;
                // validPoints.ForEach(x => result.SetValue(color, x.px, x.py));
                // return ImageEditor.CreateArgbBitmap(result.Values);
            }

            foreach ((int pX, int pY) in validPoints)
                pixels.SetValue(color, pX, pY);

            return CreateArgbBitmap(pixels.Values);
            // return bmp;
        }

        public static Bitmap Pad(Bitmap image, Padding padding, bool dispose = false)
        {
            var result = new Bitmap(image.Width + padding.Width, image.Height + padding.Height);

            using (Graphics g = Graphics.FromImage(result))
                ClipAndDrawImage(g, image, padding.Left, padding.Top);

            if (dispose)
                image.Dispose();

            return result;
        }

        public static Bitmap Crop(string localPath, int x, int y, int width, int height)
        {
            using (Bitmap bmp = new Bitmap(localPath))
                return Crop(bmp, x, y, width, height);
        }

        public static Bitmap Crop(Bitmap bmp, int x, int y, int width, int height, bool dispose = false)
            => Crop(bmp, new Rectangle(x, y, width, height), dispose);

        public static Bitmap Crop(Bitmap bitmap, Rectangle crop, bool disposeOnCrop = false)
        {
            Bitmap tmp = bitmap.Clone(crop, bitmap.PixelFormat);

            if (disposeOnCrop)
                bitmap.Dispose();

            return tmp;
        }

        public static Bitmap Trim(Bitmap bmp, bool dispose = false)
            => Crop(bmp, new Rectangle(0, 0, GetNonEmptyWidth(bmp), GetNonEmptyHeight(bmp)), dispose);

        public static Bitmap Rotate(Bitmap bmp, AngleF angle, Point? axis = null)
        {
            Size bounds = GetRotationBounds(bmp.Width, bmp.Height, angle);
            var rotated = new Bitmap(bounds.Width, bounds.Height);
            using Graphics g = Graphics.FromImage(rotated);

            float mX = bounds.Width / (float) 2;
            float mY = bounds.Height / (float) 2;

            axis ??= new Point((int)Floor(mX), (int)Floor(mY));

            g.TranslateTransform(axis.Value.X, axis.Value.Y);
            g.RotateTransform(angle);
            g.TranslateTransform(-axis.Value.X, -axis.Value.Y);
            g.DrawImage(bmp, (bounds.Width - bmp.Width) / 2, (bounds.Height - bmp.Height) / 2);

            return rotated;
        }

        public static Bitmap Rotate(Bitmap bmp, AngleF angle, OriginAnchor axis)
        {
            Size bounds = GetRotationBounds(bmp.Width, bmp.Height, angle);
            return Rotate(bmp, angle, GetOrigin(bounds, axis));
        }

        private static Size GetRotationBounds(int oldWidth, int oldHeight, AngleF angle)
        {
            AngleF gamma = 90f;
            AngleF beta = 180f - angle - gamma;

            float c1 = oldHeight;
            float c2 = oldWidth;

            float a1 = Abs(c1 * Sin(angle.Radians) / Sin(gamma.Radians));
            float b1 = Abs(c1 * Sin(beta.Radians) / Sin(gamma.Radians));
            float a2 = Abs(c2 * Sin(angle.Radians) / Sin(gamma.Radians));
            float b2 = Abs(c2 * Sin(beta.Radians) / Sin(gamma.Radians));

            int width = (int) Floor(b2 + a1);
            int height = (int) Floor(b1 + a2);

            return new Size(width, height);
        }

        public static Bitmap SetSize(Bitmap image, int width, int height)
        {
            var result = new Bitmap(width, height);

            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using Graphics g = Graphics.FromImage(result);
            using var wrap = new ImageAttributes();

            g.CompositingMode = CompositingMode.SourceCopy;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.SmoothingMode = SmoothingMode.None;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            wrap.SetWrapMode(WrapMode.TileFlipXY);

            var destination = new Rectangle(0, 0, width, height);
            g.DrawImage(image, destination, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrap);

            return result;
        }

        public static Bitmap Scale(Bitmap image, float widthScale, float heightScale)
            => SetSize(image, (int)Floor(image.Width * widthScale), (int)Floor(image.Height * heightScale));

        public static Bitmap SetOpacity(Bitmap image, float opacity)
        {
            var result = new Bitmap(image.Width, image.Height);
            using Graphics g = Graphics.FromImage(result);

            var attributes = new ImageAttributes();
            var m = new ColorMatrix
            {
                Matrix33 = opacity
            };

            attributes.SetColorMatrix(m, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            var destination = new Rectangle(0, 0, result.Width, result.Height);
            g.DrawImage(image, destination, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);

            return result;
        }

        public static Bitmap Transform(Size viewport, Bitmap bmp, ImageTransform transform, float opacity = 1.0f)
        {
            var result = new Bitmap(viewport.Width, viewport.Height, PixelFormat.Format32bppArgb);

            using Graphics g = Graphics.FromImage(result);
            using Bitmap edited = Transform(bmp, transform, opacity);

            Size bounds = GetRotationBounds(bmp.Width, bmp.Height, transform.Rotation);

            float mX = transform.Position.X - (bounds.Width - bmp.Width) / (float) 2;
            float mY = transform.Position.Y - (bounds.Height - bmp.Height) / (float) 2;

            var position = new Point((int) Floor(mX), (int) Floor(mY));

            if (position.X < 0 || position.X > viewport.Width ||
                position.Y < 0 || position.Y > viewport.Height)
            {
                if (position.X < 0 || position.X + edited.Width > viewport.Width ||
                    position.Y < 0 || position.Y + edited.Height > viewport.Height)
                {
                    Rectangle clip = ClampRectangle(Point.Empty, viewport, position, edited.Size);
                    using Bitmap crop = Crop(edited, clip);

                    ClipAndDrawImage(g, crop, position);
                    return result;
                }
            }

            ClipAndDrawImage(g, edited, position);
            return result;
        }

        public static Bitmap Transform(Bitmap bmp, ImageTransform transform, float opacity = 1.0f)
        {
            using Bitmap scaled = Scale(bmp, transform.Scale.X, transform.Scale.Y);
            using Bitmap rotated = Rotate(scaled, transform.Rotation);
            return SetOpacity(rotated, opacity);
        }

        // TODO: Determine file type before making it a Bitmap.
        public static Bitmap GetHttpImage(string url)
        {
            using (var webClient = new WebClient())
                using (Stream stream = webClient.OpenRead(url))
                    return new Bitmap(stream);
        }

        public static void Save(Image image, string path, ImageFormat format, bool dispose = true)
        {
            using (image)
            {
                EncoderParameter[] args =
                {
                    new EncoderParameter(Encoder.Quality, 100)
                };

                image.Save(path, GetImageCodec(format), BuildEncoderParameters(args));

                if (dispose)
                    image.Dispose();
            }
        }

        private static EncoderParameters BuildEncoderParameters(params EncoderParameter[] parameters)
        {
            var result = new EncoderParameters(parameters.Length);

            for (int i = 0; i < parameters.Length; i++)
                result.Param[i] = parameters[i];

            return result;
        }

        private static ImageCodecInfo GetImageCodec(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            for (int i = 0; i < codecs.Length; i++)
            {
                if (codecs[i].FormatID == format.Guid)
                    return codecs[i];
            }

            return null;
        }

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

        private static Rectangle GetForeClip(int width, int height, int fillLength, Direction direction)
        {
            return direction switch
            {
                Direction.Left => new Rectangle(width - fillLength, 0, fillLength, height),
                Direction.Up => new Rectangle(0, height - fillLength, width, fillLength),
                Direction.Right => new Rectangle(0, 0, fillLength, height),
                Direction.Down => new Rectangle(0, 0, width, fillLength),
                _ => throw new ArgumentException("Direction specified is out of range")
            };
        }

        private static Rectangle GetBackClip(int width, int height, int fillLength, Direction direction)
        {
            return direction switch
            {
                Direction.Left => new Rectangle(0, 0, width - fillLength, height),
                Direction.Up => new Rectangle(0, 0, width, height - fillLength),
                Direction.Right => new Rectangle(fillLength, 0, width - fillLength, height),
                Direction.Down => new Rectangle(0, fillLength, width, height - fillLength),
                _ => throw new ArgumentException("Direction specified is out of range")
            };
        }

        // TODO: Implement AngleF instead of Direction
        public static Bitmap CreateProgressBar(Color foreground, Color background, int width, int height, float progress, Direction direction = Direction.Right)
        {
            int length = direction.HasFlag(Direction.Left | Direction.Right) ? width : height;
            int fillLength = (int)Math.Floor(RangeF.Convert(0, 1, 0, length, progress));

            var result = new Bitmap(width, height);

            using Graphics g = Graphics.FromImage(result);

            using (var backBrush = new SolidBrush(background))
            {
                Rectangle backClip = GetBackClip(width, height, fillLength, direction);
                g.SetClip(backClip);
                g.FillRectangle(backBrush, backClip);
                g.ResetClip();
            }

            using (var foreBrush = new SolidBrush(foreground))
            {
                Rectangle foreClip = GetForeClip(width, height, fillLength, direction);
                g.SetClip(foreClip);
                g.FillRectangle(foreBrush, foreClip);
                g.ResetClip();
            }

            return result;
        }

        private static int GetGradientStrength(int width, int height, Direction direction, int index)
        {
            float strength = direction switch
            {
                Direction.Left => RangeF.Convert(0, width, 0, 1, width - index),
                Direction.Up => RangeF.Convert(0, height, 0, 1, height - index),
                Direction.Right => RangeF.Convert(0, width, 0, 1, index),
                Direction.Down => RangeF.Convert(0, height, 0, 1, index),
                _ => throw new ArgumentException("The specified direction is out of bounds")
            };

            return (int)Floor(strength);
        }

        private static Color BlendGradientValue(Color from, Color to, Direction direction, float strength)
        {
            if (direction.HasFlag(Direction.Left | Direction.Up))
                return ImmutableColor.Blend(to, from, strength);

            return ImmutableColor.Blend(from, to, strength);
        }

        private static int GetLowerBound(int width, int height, Direction direction)
        {
            return direction switch
            {
                Direction.Left => width,
                Direction.Up => height,
                Direction.Right => 0,
                Direction.Down => 0,
                _ => throw new ArgumentException("The specified direction is out of bounds")
            };
        }

        private static int GetUpperBound(int width, int height, Direction direction)
        {
            return direction switch
            {
                Direction.Left => 0,
                Direction.Up => 0,
                Direction.Right => width,
                Direction.Down => height,
                _ => throw new ArgumentException("The specified direction is out of bounds")
            };
        }

        // TODO: Implement angled gradient generation (AngleF angle)
        public static Bitmap CreateGradient(Color from, Color to, int width, int height, Direction direction = Direction.Right)
        {
            var pixels = new Grid<Color>(width, height, from);
            bool flip = (Direction.Up | Direction.Left).HasFlag(direction);

            for (int i = GetLowerBound(width, height, direction); flip ? i > 0 : i < GetUpperBound(width, height, direction); i += flip ? -1 : 1)
            {
                int length = (Direction.Left | Direction.Right).HasFlag(direction) ? width : height;
                float strength = RangeF.Convert(0, length, 0, 1, flip ? length - i : i);
                Color blend = BlendGradientValue(from, to, direction, strength);
                pixels.SetColumn(i, blend);
            }

            return CreateRgbBitmap(pixels.Values);
        }

        // TODO: Implement angled gradient generation (AngleF angle)
        public static Bitmap CreateGradient(Dictionary<float, Color> markers, int width, int height, Direction direction = Direction.Right, GradientColorHandling colorHandling = GradientColorHandling.Blend)
        {
            var pixels = new Grid<Color>(width, height, GetInitialColor(markers));
            bool flip = (Direction.Up | Direction.Left).HasFlag(direction);

            for (int i = GetLowerBound(width, height, direction); flip ? i > 0 : i < GetUpperBound(width, height, direction); i += flip ? -1 : 1)
            {
                int length = (Direction.Left | Direction.Right).HasFlag(direction) ? width : height;
                float strength = RangeF.Convert(0, length, 0, 1, flip ? length - i : i);

                if ((Direction.Left | Direction.Right).HasFlag(direction))
                    pixels.SetColumn(i, GetColorAtProgress(markers, strength, colorHandling));
                else
                    pixels.SetRow(i, GetColorAtProgress(markers, strength, colorHandling));
            }

            return CreateRgbBitmap(pixels.Values);
        }

        // TODO: Implement angled gradient generation (AngleF angle)
        public static Bitmap CreateGradient(GammaPalette palette, int width, int height, Direction direction = Direction.Right, GradientColorHandling colorHandling = GradientColorHandling.Snap)
        {
            const float colorDist = 1.0f / GammaPalette.RequiredLength;
            var markers = new Dictionary<float, Color>();

            for (int i = 0; i < GammaPalette.RequiredLength; i++)
                markers.Add(colorDist * i, palette[i]);

            return CreateGradient(markers, width, height, direction, colorHandling);
        }

        public static Bitmap CreateRgbBitmap(Color color, int width, int height)
            => CreateRgbBitmap(new Grid<Color>(width, height, color).Values);

        public static Bitmap CreateRgbBitmap(Color[,] colors)
            => CreateBitmap(colors, false);

        public static Bitmap CreateArgbBitmap(Color[,] colors)
            => CreateBitmap(colors);

        private static Bitmap CreateBitmapFromHandle(Color[,] colors)
        {
            var timer = Stopwatch.StartNew();
            int width = colors.GetLength(1);
            int height = colors.GetLength(0);

            using var handle = new ImageHandle(width, height);

            Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    handle.SetPixel(x, y, colors[y, x]);
                }
            });

            Bitmap result = handle.Bitmap.Clone(new Rectangle(0, 0, width, height), handle.Bitmap.PixelFormat);

            timer.Stop();
            Console.WriteLine($"Bitmap created with handle in {timer.ElapsedTicks} ticks");

            return result;
        }

        private static Bitmap CreateBitmap(Color[,] colors, bool isArgb = true)
        {
            var timer = Stopwatch.StartNew();
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

            timer.Stop();
            Console.WriteLine($"Bitmap created in {timer.ElapsedTicks} ticks");

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
            var mask = new Grid<float>(image.Size);
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
                var ptr = (byte*)source.Scan0;

                Parallel.For(0, sourceHeight, y =>
                {
                    byte* row = ptr + y * source.Stride;
                    int pX = 0;
                    for (var x = 0; x < sourceWidth; x += bitsPerPixel)
                    {
                        var alpha = (byte)Math.Floor(RangeF.Convert(0, 1, 0, 255, mask[pX, y]));
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

        public static int GetUniqueColorCount(Bitmap image)
            => GetUniqueColors(image).Count;

        public static List<Color> GetUniqueColors(Bitmap image)
        {
            var pixels = new List<Color>(image.Width * image.Height);
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
                        pixels.Add(pixel);
                    }
                });

                image.UnlockBits(source);
            }

            var unique = new List<Color>();

            foreach (Color pixel in pixels)
            {
                if (!unique.Contains(pixel))
                    unique.Add(pixel);
            }

            return unique;
        }

        public static int GetLumenColorCount(Bitmap image, GammaPalette colors)
        {
            List<Color> unique = GetUniqueColors(image);

            var lumenColors = new List<int>();

            foreach (Color color in unique)
            {
                int match = ImmutableColor.ClosestBrightnessAt(color, colors.Values);

                if (!lumenColors.Contains(match))
                    lumenColors.Add(match);
            }

            Console.WriteLine(string.Join(" ", lumenColors));

            return lumenColors.Count;
        }

        public static Bitmap ForceLumenPalette(Bitmap bmp, GammaPalette colors)
        {
            Console.WriteLine(GetUniqueColorCount(bmp));
            Console.WriteLine(GetLumenColorCount(bmp, colors));
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
                        Color match = ImmutableColor.ClosestLumen(color, colors.Values);

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
                using Bitmap overlap = Crop(inner, visible);
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

        public static void ClipAndDrawImage(Graphics graphics, Bitmap image, Point point, Size size)
            => ClipAndDrawImage(graphics, image, new Rectangle(point, size));

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

            foreach (char[][][] page in map)
            {
                i++;

                if (!page.Any(r => r.Any(g => g.Contains(c))))
                    continue;

                foreach (char[][] row in page)
                {
                    y++;

                    if (!row.Any(g => g.Contains(c)))
                        continue;

                    foreach (char[] group in row)
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
            => (int)Floor(length / (float)2);

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
                BorderEdge.Center => (int)Floor(width / (float)2),
                BorderEdge.Inside => width,
                _ => 0
            };

        private static int GetOuterLength(int width, BorderEdge edge)
            => edge switch
            {
                BorderEdge.Outside => width,
                BorderEdge.Center => width - (int)Floor(width / (float)2),
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
                    .OrderBy(x => Abs(progress - x))
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
                    .OrderBy(x => Abs(progress - x))
                    .First();
            }

            return null;
        }
    }
}
