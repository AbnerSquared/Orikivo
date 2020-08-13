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
        internal static Bitmap Fill(Bitmap bmp, Color color)
        {
            using (Graphics graphics = Graphics.FromImage(bmp))
                graphics.Clear(color);

            return bmp;
        }

        internal static Bitmap ReplaceColor(Bitmap bmp, Color color, Color? alphaColor)
        {
            Color alpha = alphaColor ?? Color.Empty;

            Grid<Color> pixels = ImageEditor.GetPixelData(bmp);
            pixels.SetEachValue((pixel, x, y) => pixel.Equals(alpha) ? color : pixel);

            return ImageEditor.CreateArgbBitmap(pixels.Values);
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

            return ImageEditor.GetPixelData(bmp).All(x => x.A == 0);
        }

        internal static int GetNonEmptyWidth(Bitmap bmp)
        {
            if (bmp == null)
                return 0;

            Grid<Color> pixels = ImageEditor.GetPixelData(bmp);
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

            Grid<Color> pixels = ImageEditor.GetPixelData(bmp);
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
            Grid<Color> pixels = ImageEditor.GetPixelData(bmp);
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

            return ImageEditor.CreateArgbBitmap(pixels.Values);
            // return bmp;
        }

        public static Bitmap Pad(Bitmap image, Padding padding, bool dispose = false)
        {
            var result = new Bitmap(image.Width + padding.Width, image.Height + padding.Height);

            using (Graphics g = Graphics.FromImage(result))
                ImageEditor.ClipAndDrawImage(g, image, padding.Left, padding.Top);

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
            return Rotate(bmp, angle, ImageEditor.GetOrigin(bounds, axis));
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
                    Rectangle clip = ImageEditor.ClampRectangle(Point.Empty, viewport, position, edited.Size);
                    using Bitmap crop = Crop(edited, clip);

                    ImageEditor.ClipAndDrawImage(g, crop, position);
                    return result;
                }
            }

            ImageEditor.ClipAndDrawImage(g, edited, position);
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
    }
}
