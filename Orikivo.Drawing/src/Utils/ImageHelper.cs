using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
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
            Color alpha = alphaColor ?? Color.Empty;

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    if (pixels.GetValue(x, y) == alpha)
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

                            if (!validPoints.Contains((m, n)))
                                if (pixels.GetValue(m, n) == alpha)
                                    validPoints.Add((m, n));
                        }
                    }
                }
            }

            if (drawOnNew)
            {
                var result = new Grid<Color>(bmp.Width, bmp.Height, alpha);
                validPoints.ForEach(x => result.SetValue(color, x.px, x.py));
                return ImageEditor.CreateArgbBitmap(result.Values);
            }

            validPoints.ForEach(x => pixels.SetValue(color, x.px, x.py));
            return bmp;
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
            Bitmap rotated = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(rotated))
            {
                axis ??= new Point(bounds.Width / 2, bounds.Height / 2);

                g.TranslateTransform(axis.Value.X, axis.Value.Y); // the initial translate transform is top left of the image.
                g.RotateTransform(angle);
                g.TranslateTransform(-axis.Value.X, -axis.Value.Y);
                g.DrawImage(bmp, (bounds.Width - bmp.Width) / 2, (bounds.Height - bmp.Height) / 2);
            }

            return rotated;
        }

        public static Bitmap Rotate(Bitmap bmp, AngleF angle, OriginAnchor axis)
        {
            Size bounds = GetRotationBounds(bmp.Width, bmp.Height, angle);
            return Rotate(bmp, angle, ImageEditor.GetOrigin(bounds, axis));
        }

        private static Size GetRotationBounds(int oldWidth, int oldHeight, AngleF angle)
        {
            AngleF gamma = 90.0f;
            AngleF beta = 180.0f - angle - gamma;

            float c1 = oldHeight;
            float c2 = oldWidth;

            float a1 = Abs(c1 * Sin(angle.Radians) / Sin(gamma.Radians));
            float b1 = Abs(c1 * Sin(beta.Radians) / Sin(gamma.Radians));
            float a2 = Abs(c2 * Sin(angle.Radians) / Sin(gamma.Radians));
            float b2 = Abs(c2 * Sin(beta.Radians) / Sin(gamma.Radians));

            int width = (int)Round(b2 + a1);
            int height = (int)Round(b1 + a2);

            return new Size(width, height);
        }

        public static Bitmap SetSize(Bitmap bmp, int width, int height)
        {
            var result = new Bitmap(width, height);

            result.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);

            using (Graphics g = Graphics.FromImage(result)) // this method of resizing might be a tad too much
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.SmoothingMode = SmoothingMode.None;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrap = new ImageAttributes())
                {
                    wrap.SetWrapMode(WrapMode.TileFlipXY);

                    var destination = new Rectangle(0, 0, width, height);
                    g.DrawImage(bmp, destination, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, wrap);
                }
            }

            return result;
        }

        public static Bitmap Scale(Bitmap bmp, float widthScale, float heightScale)
            => SetSize(bmp, (int)Floor(bmp.Width * widthScale), (int)Floor(bmp.Height * heightScale));

        public static Bitmap SetOpacity(Bitmap bmp, float opacity)
        {
            var result = new Bitmap(bmp.Width, bmp.Height);

            using (Graphics g = Graphics.FromImage(result))
            {
                var attributes = new ImageAttributes();
                var m = new ColorMatrix
                {
                    Matrix33 = opacity
                };

                attributes.SetColorMatrix(m, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                var destination = new Rectangle(0, 0, result.Width, result.Height);
                g.DrawImage(bmp, destination, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
            }

            return result;
        }

        public static Bitmap Transform(Size viewport, Bitmap bmp, ImageTransform transform, float opacity = 1.0f)
        {
            Bitmap result = new Bitmap(viewport.Width, viewport.Height, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(result))
            {
                using (Bitmap edited = Transform(bmp, transform, opacity))
                {
                    Size bounds = GetRotationBounds(bmp.Width, bmp.Height, transform.Rotation);

                    // POSITION
                    PointF rawPosition = new PointF(transform.Position.X - ((bounds.Width - bmp.Width) / 2),
                        transform.Position.Y - ((bounds.Height - bmp.Height) / 2));
                    Point position = Point.Truncate(rawPosition);

                    if (position.X > viewport.Width && position.Y > viewport.Height)
                    {
                        if (position.X < 0 || position.X + (edited.Width) > viewport.Width ||
                            position.Y < 0 || position.Y + (edited.Height) > viewport.Height)
                        {
                            Rectangle cropRect = ImageEditor.ClampRectangle(Point.Empty, viewport, position, edited.Size);

                            using (Bitmap crop = Crop(edited, cropRect))
                                ImageEditor.ClipAndDrawImage(g, crop, position);
                        }
                        else
                            ImageEditor.ClipAndDrawImage(g, edited, position);
                    }
                    else
                        ImageEditor.ClipAndDrawImage(g, edited, position);
                }
            }

            return result;
        }

        public static Bitmap Transform(Bitmap bmp, ImageTransform transform, float opacity = 1.0f)
        {
            // SCALE
            using (Bitmap scaled = Scale(bmp, transform.Scale.X, transform.Scale.Y))
            {
                // ROTATE
                using (Bitmap rotated = Rotate(scaled, transform.Rotation))
                {
                    // OPACITY
                    return SetOpacity(rotated, opacity);
                }
            }
        }

        // TODO: Determine file type before making it a Bitmap.
        public static Bitmap GetHttpImage(string url)
        {
            using (WebClient webClient = new WebClient())
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
