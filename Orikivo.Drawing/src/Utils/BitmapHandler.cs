using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using Encoder = System.Drawing.Imaging.Encoder;

namespace Orikivo.Drawing
{

    public static class BitmapHandler
    {
        internal static Bitmap Fill(Bitmap bmp, Color color) // FillAlpha: Fills only empty colors.
        {
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                graphics.Clear(color);
            }

            return bmp;
        }

        public static bool IsEmpty(Bitmap bmp)
            => GraphicsUtils.GetPixels(bmp).All(x => x.A == 0);
        public static ColorMap[] CreateColorMaps(Color[] fromColors, Color[] toColors)
        {
            if (fromColors == null || toColors == null)
                throw new Exception("A color array is null.");

            if (fromColors.Length != toColors.Length)
                throw new Exception("The 'from' and 'to' color arrays must be the same length.");

            List<(Color, Color)> colors = new List<(Color, Color)>();

            for (int i = 0; i < fromColors.Length; i++)
                colors.Add((fromColors[i], toColors[i]));

            return CreateColorMaps(colors.ToArray());
        }

        internal static ColorMap[] CreateColorMaps(params (Color From, Color To)[] colors)
        {
            if (!(colors?.Length > 0))
                throw new Exception("At least one color map value must be specified.");

            return colors.Select(x => new ColorMap
            {
                NewColor = x.To,
                OldColor = x.From
            }).ToArray();
        }

        // remember that a discord image container has a border radius of 3px
        // to prevent pixels from looking wonky, add a minimum padding of 4px
        public static Bitmap ReplacePalette(Bitmap bmp, GammaPalette from, GammaPalette to)
            => ReplacePalette(bmp, CreateColorMaps(from.Values.Select(x => (Color)x).ToArray(), to.Values.Select(x => (Color)x).ToArray()));

        public static Bitmap ReplacePalette(Bitmap bmp, ColorMap[] mapTable)
        {
            Bitmap canvas = new Bitmap(bmp.Width, bmp.Height);

            using (Graphics graphics = Graphics.FromImage(canvas))
            {
                ImageAttributes attributes = new ImageAttributes();

                attributes.SetRemapTable(mapTable, ColorAdjustType.Bitmap);

                graphics.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height),
                    0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
            }

            return canvas;
        }

        internal static int GetNonEmptyWidth(Bitmap bmp)
        {
            if (bmp == null)
                return 0;

            Grid<Color> pixels = GraphicsUtils.GetPixels(bmp);

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

        // creates a new bitmap from a direct stream.
        public static Bitmap GetHttpImage(string url)
        {
            // TODO: Determine file type before making it a Bitmap.
            using (WebClient webClient = new WebClient())
                using (Stream stream = webClient.OpenRead(url))
                    return new Bitmap(stream);
        }

        // creates an outline around the bitmap given, with an option to include the bitmap it was drawn on or not
        public static Bitmap DrawOutline(Bitmap bmp, int width, Color color, Color? alphaColor = null, bool drawOnEmpty = false)
        {
            Color alpha = alphaColor ?? Color.Empty;
            List<(int px, int py)> validPoints = new List<(int px, int py)>();
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    if (bmp.GetPixel(x, y) == alphaColor)
                        continue;

                    int minX = x - width;
                    int minY = y - width;
                    
                    int maxX = x + width;
                    int maxY = y + width;

                    Console.WriteLine($"Outline.Bounds: (X: ({minX}, {maxX}), Y: ({minY}, {maxY}))");

                    for (int m = minX; m <= maxX; m++)
                    {
                        // ignore all values out of bounds
                        if (m < 0)
                            continue;

                        if (m > bmp.Width - 1) // we know the rest of the values outside of the width will be bad
                        {
                            Console.WriteLine($"Outline.Cursor.X > Sprite.Width");
                            break;
                        }

                        for (int n = minY; n <= maxY; n++)
                        {
                            // Console.WriteLine($"Outline.Cursor: ({m}, {n})");

                            if (n > bmp.Height - 1)
                            {
                                Console.WriteLine($"Outline.Cursor.Y > Sprite.Height");
                                break;
                            }

                            if (n < 0)
                                continue;
                            
                            if (bmp.GetPixel(m, n) == alphaColor)
                                if (!validPoints.Contains((m, n)))
                                {
                                    Console.WriteLine($"Outline.DrawPoint: ({m}, {n})");
                                    validPoints.Add((m, n));
                                }
                        }
                    }
                }
            }

            if (drawOnEmpty)
            {
                // you can't return a using(), as it's disposed before it can save
                Bitmap tmp = new Bitmap(bmp.Width, bmp.Height);
                validPoints.ForEach(x => tmp.SetPixel(x.px, x.py, color));
                return tmp;
            }

            validPoints.ForEach(x => bmp.SetPixel(x.px, x.py, color));

            return bmp;
        }

        private static ImageCodecInfo GetImageCodec(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
                if (codec.FormatID == format.Guid)
                    return codec;
            return null; // no matching codec found.
        }

        public static Bitmap Crop(string localPath, int x, int y, int width, int height)
        {
            using (Bitmap bmp = new Bitmap(localPath))
                return Crop(bmp, x, y, width, height);
        }

        public static Bitmap AutoCrop(Bitmap bmp, bool disposeOnCrop = false)
            => Crop(bmp, new Rectangle(0, 0, GetNonEmptyWidth(bmp), bmp.Height), disposeOnCrop);

        public static Bitmap Crop(Bitmap bmp, int x, int y, int width, int height, bool disposeOnCrop = false)
            => Crop(bmp, new Rectangle(x, y, width, height), disposeOnCrop);

        public static Bitmap Crop(Bitmap bitmap, Rectangle crop, bool disposeOnCrop = false)
        {
            Bitmap tmp = bitmap.Clone(crop, bitmap.PixelFormat);

            if (disposeOnCrop)
                bitmap.Dispose();

            return tmp;
        }

        public static void Save(Image bmp, string path, ImageFormat format)
        {
            using (bmp)
            {
                Encoder encoder = Encoder.Quality;
                EncoderParameter[] args = { new EncoderParameter(encoder, 100) };
                EncoderParameters parameters = new EncoderParameters(args.Length);
                for (int i = 0; i < args.Length; i++)
                    parameters.Param[i] = args[i];
                bmp.Save(path, GetImageCodec(format), parameters); // bmp can be disposed, as it's simply being stored
            }
        }
    }
}
