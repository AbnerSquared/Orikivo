using System.Drawing;
using System.Drawing.Imaging;

namespace Orikivo
{
    public static class ByteExtension
    {
        public static Bitmap ToBitmap(this byte[] bytes, int width, int height, int stride, PixelFormat f, Color[] palette, Color? alpha)
            => BitmapManager.DrawBitmap(bytes, width, height, stride, f, palette, alpha);
    }
}