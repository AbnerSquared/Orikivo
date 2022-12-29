using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Orikivo.Drawing
{
    public sealed class ImageHandle : IDisposable
    {
        public ImageHandle(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new byte[width * height * 4];
            BitHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppArgb, BitHandle.AddrOfPinnedObject());
        }

        public int Width { get; }

        public int Height { get; }

        public Bitmap Bitmap { get; }

        public byte[] Bits { get; }

        public bool Disposed { get; private set; }

        private GCHandle BitHandle { get; }

        public void SetPixel(int x, int y, Color color)
        {
            int index = x + y * Width * 4;
            //int argb = color.ToArgb();
            Bits[index + 3] = color.A;
            Bits[index + 2] = color.R;
            Bits[index + 1] = color.G;
            Bits[index] = color.B;
        }

        public Color GetPixel(int x, int y)
        {
            int index = x + y * Width * 4;
            // int argb = Bits[index];
            return Color.FromArgb(Bits[index + 3], Bits[index + 2], Bits[index + 1], Bits[index]);
        }

        public void Dispose()
        {
            if (Disposed)
                return;

            Disposed = true;
            Bitmap.Dispose();
            BitHandle.Free();
        }
    }
}