using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Orikivo.Drawing
{
    // TODO: Complete this class
    public sealed class BitmapSource : IDisposable
    {
        private readonly Bitmap _image;

        public BitmapSource(Bitmap image)
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