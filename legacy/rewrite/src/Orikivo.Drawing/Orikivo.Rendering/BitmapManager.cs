using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using DiscordImageFormat = Discord.ImageFormat;
namespace Orikivo
{
    public class BitmapManager
    {
        public static Bitmap ColorBitmap(Bitmap bmp, Color[] colors)
        {
            using(Graphics g = Graphics.FromImage(bmp))
            {
                Rectangle rct = bmp.ToRectangle();
                ImageAttributes attr = new ImageAttributes();
                attr.SetRemapTable(BuildColorMap(colors), ColorAdjustType.Bitmap);
                g.DrawImage(bmp, rct, 0, 0, rct.Width, rct.Height, GraphicsUnit.Pixel, attr);
            }
            return bmp;
        }

        public static Bitmap ColorIndexedBitmap(Bitmap bmp, Color[] colors)
        {
            Bitmap tmp = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(tmp))
            {
                Rectangle rct = bmp.ToRectangle();
                ImageAttributes attr = new ImageAttributes();
                attr.SetRemapTable(BuildColorMap(colors), ColorAdjustType.Bitmap);
                g.DrawImage(bmp, rct, 0, 0, rct.Width, rct.Height, GraphicsUnit.Pixel, attr);
            }
            return bmp;
        }

        public static ColorMap[] BuildColorMap(Color[] palette)
        {
            Color[] pal = ColorScheme.Default;
            List<ColorMap> cmap = new List<ColorMap>();
            for (int i = 0; i < pal.Length; i++)
            {
                ColorMap cm = new ColorMap();
                cm.OldColor = pal[i];
                cm.NewColor = palette[i];
                cmap.Add(cm);
            }

            return cmap.ToArray();
        }

        public static int GetClosestMatchingColor(Color input, Color[] palette)
        {
            int m = 0;
            int prev = int.MaxValue;
            for (int i = 0; i < palette.Length; i++)
            {
                int cur = GetColorAverage(input, palette[i]);
                if (cur >= prev)
                    continue;
                m = i;
                prev = cur;
                if (cur == 0)
                    return i;
            }
            return m;
        }

        public static int GetColorAverage(Color input, Color col)
        {
            int r = col.R - input.R;
            int g = col.G - input.G;
            int b = col.B - input.B;
            return (r * r) + (g * g) + (b * b);
        }

        public static int GetColorAverage(Color input, Color col, out int r, out int g, out int b)
        {
            r = col.R - input.R;
            g = col.G - input.G;
            b = col.B - input.B;
            return (r * r) + (g * g) + (b * b);
        }

        public static byte[] GetBytes(Bitmap bmp, out int stride)
        {
            Point p = new Point(0, 0);
            Rectangle rct = new Rectangle(p, bmp.Size);
            BitmapData source = bmp.LockBits(rct, ImageLockMode.ReadOnly, bmp.PixelFormat);
            stride = source.Stride;
            byte[] bytes = new byte[stride * bmp.Height];
            Marshal.Copy(source.Scan0, bytes, 0, bytes.Length);
            bmp.UnlockBits(source);
            return bytes;
        }

        public static byte[] DrawBpp8(byte[] bytes, Size size, Color[] palette, ref int stride)
            => DrawBpp8(bytes, size.Width, size.Height, palette, ref stride);

        public static byte[] DrawBpp8(byte[] bytes, int width, int height, Color[] palette, ref int stride)
        {
            if (stride < width * 4)
                throw new ArgumentException("stride is ded");
            
            byte[] bpp8 = new byte[width * height];
            for(int y = 0; y < height; y++)
            {
                int input = y * stride;
                int output = y * width;
                for (int x = 0; x < width; x++)
                {
                    byte r = bytes[input + 2];
                    byte g = bytes[input + 1];
                    byte b = bytes[input];
                    Color col = Color.FromArgb(r, g, b);
                    bpp8[output] = (byte) GetClosestMatchingColor(col, palette);
                    input += 4;
                    output++;
                }
            }
            stride = width;
            return bpp8;
        }

        public static Bitmap DrawBpp32(Bitmap bmp, Color? col)
        {
            PixelFormat format = PixelFormat.Format32bppArgb;
            Bitmap tmp = new Bitmap(bmp.Width, bmp.Height, format);
            using (Graphics g = Graphics.FromImage(tmp))
            {
                Point p = new Point(0, 0);
                Rectangle rct = new Rectangle(p, bmp.Size);
                if (col.HasValue)
                    using (SolidBrush b = new SolidBrush(Color.FromArgb(255, col.Value)))
                        g.FillRectangle(b, rct);

                rct.Size = tmp.Size;
                g.DrawImage(bmp, rct);
            }

            return tmp;
        }

        public static Bitmap DrawBitmap(byte[] bytes, Size size, int stride, PixelFormat f, Color[] palette, Color? alpha)
            => DrawBitmap(bytes, size.Width, size.Height, stride, f, palette, alpha);

        public static Bitmap DrawBitmap(byte[] bytes, int width, int height, int stride, PixelFormat f, Color[] palette, Color? alpha)
        {
            Bitmap bmp = new Bitmap(width, height, f);
            Point p = new Point(0, 0);
            Rectangle rct = new Rectangle(p, bmp.Size);
            BitmapData source = bmp.LockBits(rct, ImageLockMode.WriteOnly, bmp.PixelFormat);
            int size = Image.GetPixelFormatSize(f);
            int length = ((size * width) + 7) / 8;
            bool flipped = stride < 0;
            stride = Math.Abs(stride);
            int target = source.Stride;
            long scan = source.Scan0.ToInt64();
            for (int y = 0; y < height; y++)
            {
                int index = y * stride;
                IntPtr pointer = new IntPtr(scan + y * target);
                Marshal.Copy(bytes, index, pointer, length);
            }

            bmp.UnlockBits(source);
            if (flipped)
                bmp.RotateFlip(RotateFlipType.Rotate180FlipX);

            if ((f & PixelFormat.Indexed) != 0 && palette != null)
            {
                ColorPalette pal = bmp.Palette;
                for(int i = 0; i < pal.Entries.Length; i++)
                {
                    if (i < palette.Length)
                        pal.Entries[i] = palette[i];
                    else if (alpha.HasValue)
                        pal.Entries[i] = alpha.Value;
                    else
                        break;
                }

                bmp.Palette = pal;
            }

            return bmp;
        }

        public static Bitmap DrawToPalette(Bitmap bmp, Color[] palette)
        {
            PixelFormat format = PixelFormat.Format8bppIndexed;
            Color alpha = palette[7];
            Bitmap tmp = bmp.ToBpp32(alpha);
            byte[] bpp32 = tmp.GetBytes(out int stride);
            tmp.Dispose();
            byte[] bpp8 = DrawBpp8(bpp32, bmp.Size, palette, ref stride);
            return DrawBitmap(bpp8, bmp.Size, stride, format, palette, alpha);
        }
        
        public static Bitmap Resize(Bitmap bmp, Size size)
            => Resize(bmp, size.Width, size.Height);

        public static Bitmap Resize(Bitmap bmp, int width, int height)
        {
            InterpolationMode mode = InterpolationMode.NearestNeighbor;
            Rectangle rct = new Rectangle(0, 0, width, height);
            Bitmap tmp = new Bitmap(width, height);
            tmp.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
            using (Graphics g = Graphics.FromImage(tmp))
            {
                g.InterpolationMode = mode;
                using (ImageAttributes attributes = new ImageAttributes())
                {
                    attributes.SetWrapMode(WrapMode.TileFlipXY);
                    GraphicsUnit unit = GraphicsUnit.Pixel;
                    g.DrawImage(bmp, rct, 0, 0, bmp.Width, bmp.Height, unit, attributes);
                }
            }

            return tmp;
        }

        public static ImageCodecInfo GetCodec(ImageFormat f)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
                if (codec.FormatID == f.Guid)
                    return codec;

            return null;
        }

        public static void Save(Bitmap bmp, string path, DiscordImageFormat f)
            => Save(bmp, path, f.ToSystemFormat());

        public static void Save(Bitmap bmp, string path, ImageFormat f)
        {
            using (Bitmap b = bmp)
            {
                Encoder encoder = Encoder.Quality;
                long quality = 100;
                EncoderParameter[] args = { new EncoderParameter(encoder, quality) };
                EncoderParameters parameters = new EncoderParameters(args.Length);
                for (int i = 0; i < args.Length; i++)
                    parameters.Param[i] = args[i];

                b.Save(path, GetCodec(f), parameters);
            }
        }
    }
}
