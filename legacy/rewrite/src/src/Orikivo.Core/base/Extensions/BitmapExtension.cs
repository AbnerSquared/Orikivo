using System.Drawing;
using SystemImageFormat = System.Drawing.Imaging.ImageFormat;
using DiscordImageFormat = Discord.ImageFormat;

namespace Orikivo
{
    public static class BitmapExtension
    {
        public static Bitmap Color(this Bitmap bmp, Color[] colors)
            => BitmapManager.ColorBitmap(bmp, colors);

        public static Bitmap ColorIndexed(this Bitmap bmp, Color[] colors)
            => BitmapManager.ColorIndexedBitmap(bmp, colors);

        public static Bitmap Resize(this Bitmap bmp, int scale)
            => BitmapManager.Resize(bmp, bmp.Width * scale, bmp.Height * scale);

        public static Bitmap Resize(this Bitmap bmp, Size size)
            => BitmapManager.Resize(bmp, size);

        public static Bitmap Resize(this Bitmap bmp, int width, int height)
            => BitmapManager.Resize(bmp, width, height);

        public static Bitmap ToPalette(this Bitmap bmp, Color[] palette)
            => BitmapManager.DrawToPalette(bmp, palette);

        public static Bitmap ToBpp32(this Bitmap bmp, Color? col)
            => BitmapManager.DrawBpp32(bmp, col);

        public static bool HasSize(this Bitmap bmp, int width, int height)
            => HasSize(bmp, new Size(width, height));

        public static bool HasSize(this Bitmap b, Size size)
            => b.Size == size;

        public static byte[] GetBytes(this Bitmap b, out int stride)
            => BitmapManager.GetBytes(b, out stride);

        public static Point At(this Bitmap bmp, int x, int y)
            => new Point (x, y);

        public static Rectangle ToRectangle(this Bitmap bmp)
            => new Rectangle(bmp.At(0, 0), bmp.Size);

        public static void SaveBitmap(this Bitmap bmp, string path, DiscordImageFormat f)
            => BitmapManager.Save(bmp, path, f);

        public static void SaveBitmap(this Bitmap bmp, string path, SystemImageFormat f)
            => BitmapManager.Save(bmp, path, f);
    }
}