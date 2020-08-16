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

    public sealed class BitmapHandle : IDisposable
    {
        private readonly Bitmap _image;

        public BitmapHandle(Bitmap image)
        {
            _image = image;
        }

        public void LockBits(Rectangle area, ImageLockMode lockMode = ImageLockMode.ReadWrite)
        {
            if (IsLocked)
                throw new ArgumentException("This source already has locked bits");

            Source = _image.LockBits(area, lockMode, _image.PixelFormat);
            IsLocked = true;
        }

        public void UnlockBits()
        {
            if (!IsLocked)
                throw new ArgumentException("Could not find any locked bits to release");

            _image.UnlockBits(Source);
            IsLocked = false;
        }

        public int Bpp => Image.GetPixelFormatSize(_image.PixelFormat) / 8;

        public int? BitWidth => Source.Width * Bpp;

        public int? BitHeight => Source.Height;

        public bool Disposed { get; private set; }

        public bool IsLocked { get; private set; }

        private BitmapData Source { get; set; }

        public Color32 GetPixel(int x, int y)
        {
            throw new NotImplementedException();
        }

        public void SetPixel(Color color, int x, int y)
        {

        }

        private int GetOffsetY(int y)
        {
            if (!IsLocked)
                throw new Exception("Could not find any locked bits to reference");

            if (y > BitHeight)
                throw new Exception("The specified index is out of range");

            return y * Source.Stride;
        }

        public void Dispose()
        {
            if (Disposed)
                return;

            _image.Dispose();
            Disposed = true;
        }
    }
}