using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using Orikivo.Static;
using Discord.WebSocket;

namespace Orikivo.Systems.Services
{
    public class ImageConfiguration
    {
        
        public static int FindClosestMatch(Color c, Color[] colors)
        {
            int match = 0;
            int lDst = int.MaxValue;
            for (int i = 0; i < colors.Length; i++)
            {
                Color color = colors[i];
                int r = color.R - c.R;
                int g = color.G - c.G;
                int b = color.B - c.B;
                int dst = r.Pow2() + g.Pow2() + b.Pow2();
                if (dst >= lDst)
                    continue;
                match = i;
                lDst = dst;
                if (dst == 0)
                    return i;
            }

            return match;
        }

        public static Bitmap To32BitsPerPixel(Bitmap b, Color? c)
        {
            Bitmap tmp = new Bitmap(b.Width, b.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(tmp))
            {
                Point p = new Point(0, 0);
                Rectangle r = new Rectangle(new Point(0, 0), b.Size);
                if (c.HasValue)
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, c.Value)))
                        g.FillRectangle(brush, r);
                
                r.Size = tmp.Size;
                g.DrawImage(b, r);
            }

            return tmp;
        }

        public static byte[] GetBytes(Bitmap b, out int stride)
        {
            Point p = new Point(0, 0);
            Rectangle r = new Rectangle(p, b.Size);
            BitmapData source = b.LockBits(r, ImageLockMode.ReadOnly, b.PixelFormat);
            stride = source.Stride;
            byte[] bytes = new byte[stride * b.Height];
            Marshal.Copy(source.Scan0, bytes, 0, bytes.Length);
            b.UnlockBits(source);
            return bytes;
        }

        public static byte[] To8BitsPerPixel(byte[] bytes, int width, int height, Color[] colors, ref int stride)
        {
            if (stride < width * 4)
                throw new ArgumentException("The stride must be greater than the image width times four.", nameof(stride));
            
            byte[] bpp8 = new byte[width * height];
            for (int y = 0; y < height; y++)
            {
                int input = y * stride;
                int output = y * width;
                for (int x = 0; x < width; x++)
                {
                    Color c = Color.FromArgb(bytes[input + 2], bytes[input + 1], bytes[input]);
                    bpp8[output] = (byte) c.FindClosestMatch(colors);
                    input += 4;
                    output++;
                }
            }
            stride = width;
            return bpp8;
        }

        public static Bitmap SetBitmap(byte[] bytes, int width, int height, int stride, PixelFormat format, Color[] colors, Color? c)
        {
            Bitmap b = new Bitmap(width, height, format);
            BitmapData data = b.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, b.PixelFormat);
            int length = ((Image.GetPixelFormatSize(format) * width) + 7) / 8;
            bool flipped = stride < 0;
            stride = Math.Abs(stride);
            int targetStride = data.Stride;
            long scan0 = data.Scan0.ToInt64();
            for (int y = 0; y < height; y++)
                Marshal.Copy(bytes, y * stride, new IntPtr(scan0 + y * targetStride), length);
            
            b.UnlockBits(data);
            if (flipped)
                b.RotateFlip(RotateFlipType.Rotate180FlipX);

            if ((format & PixelFormat.Indexed) != 0 && colors != null)
            {
                ColorPalette p = b.Palette;
                for (int i = 0; i < p.Entries.Length; i++)
                {
                    if (i < colors.Length)
                        p.Entries[i] = colors[i];
                    
                    else if (c.HasValue)
                        p.Entries[i] = c.Value;
                    
                    else
                        break;
                }

                b.Palette = p;
            }

            return b;
        }

        public static Bitmap SetPalette(Bitmap b, Color[] colors)
        {
            int stride;
            byte[] bpp32;
            using (Bitmap x = b.ToBpp32(colors[0]))
                bpp32 = x.GetBytes(out stride);
            
            byte[] bpp8 = To8BitsPerPixel(bpp32, b.Width, b.Height, colors, ref stride);
            return bpp8.ToBitmap(b.Width, b.Height, stride, PixelFormat.Format8bppIndexed, colors, Color.Black);
        }

        public ImageCodecInfo GetCodec(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }

        public static Bitmap Resize(Bitmap b, Size s)
            => Resize(b, s.Width, s.Height);

        public static Bitmap Resize(Bitmap b, int width, int height)
        {
            Rectangle r = new Rectangle(0, 0, width, height);
            Bitmap tmp = new Bitmap(width, height);
            tmp.SetResolution(b.HorizontalResolution, b.VerticalResolution);
            using (Graphics g = Graphics.FromImage(tmp))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;

                using (ImageAttributes attributes = new ImageAttributes())
                {
                    attributes.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(b, r, 0, 0, b.Width, b.Height, GraphicsUnit.Pixel, attributes);
                }
            }

            return tmp;
        }

        public void SaveAs(Bitmap b, string path, Discord.ImageFormat f)
        {
            ImageFormat tmp;
            switch(f)
            {
                case Discord.ImageFormat.Jpeg:
                    tmp = ImageFormat.Jpeg;
                    break;
                case Discord.ImageFormat.Gif:
                    tmp = ImageFormat.Gif;
                    break;
                default:
                    tmp = ImageFormat.Png;
                    break;
            }

            SaveAs(b, path, tmp);
        }

        public void SaveAs(Bitmap b, string path, ImageFormat f)
        {
                Encoder e = Encoder.Quality;
                EncoderParameter[] args = { new EncoderParameter(e, 100L) };
                EncoderParameters p = new EncoderParameters(args.Length);
                for (int i = 0; i < args.Length; i++)
                    p.Param[i] = args[i];
                
                b.Save(path, GetCodec(f), p);
        }

        public void TrySaveAvatar(SocketUser u, string path)
            => TrySaveAvatar(u.GetAvatarUrl(Discord.ImageFormat.Auto, 32), path);

        public void TrySaveAvatar(string url, string path)
        {
            if (!url.Exists() || File.Exists(path)) return;
            try
            {
                using (WebClient web = new WebClient())
                    web.DownloadFile(new Uri(url), path);
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine($"orikivo.cs @ {DateTime.UtcNow}\v=> download.error\n( 'The URI provided is null.' )\n____/\n\n");
            }
            Console.WriteLine("Avatar saved.");
        }

        public Bitmap TryGetAvatar(string url, string path)
        {
            if (!url.Exists())
                return new Bitmap(Locator.DefaultAvatar);
            TrySaveAvatar(url, path);
            return new Bitmap(path);
        }
        
    }
}