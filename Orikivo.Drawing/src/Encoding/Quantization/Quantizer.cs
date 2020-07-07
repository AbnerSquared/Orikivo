using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Orikivo.Drawing
{
    // NOTE: Referenced from the following GitHub projects:
    // https://github.com/mrousavy/AnimatedGif
    public abstract class Quantizer
    {
        private readonly int _pixelSize;
        private readonly bool _singlePass;

        protected Quantizer(bool singlePass)
        {
            _singlePass = singlePass;
            _pixelSize = Marshal.SizeOf(typeof(Color32));
        }

        public Bitmap Quantize(Image source)
        {
            int width = source.Width;
            int height = source.Height;

            Rectangle bounds = new Rectangle(0, 0, width, height);
            Bitmap copy = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            Bitmap output = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

            using (Graphics graphics = Graphics.FromImage(copy))
            {
                graphics.PageUnit = GraphicsUnit.Pixel;

                graphics.DrawImage(source, bounds);
            }

            BitmapData sourceData = null;

            try
            {
                sourceData = copy.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                if (!_singlePass)
                    FirstPass(sourceData, width, height);

                output.Palette = GetPalette(output.Palette);

                SecondPass(sourceData, output, width, height, bounds);
            }
            finally
            {
                copy.UnlockBits(sourceData);
            }

            return output;
        }

        protected virtual void FirstPass(BitmapData sourceData, int width, int height)
        {
            IntPtr pSourceRow = sourceData.Scan0;

            for (int row = 0; row < height; row++)
            {
                IntPtr pSourcePixel = pSourceRow;
            
                for (int col = 0; col < width; col++)
                {
                    InitialQuantizePixel(new Color32(pSourcePixel));
                    pSourcePixel = (IntPtr)((long)pSourcePixel + _pixelSize);
                }

                pSourceRow = (IntPtr)((long)pSourceRow + sourceData.Stride);
            }
        }

        protected virtual void SecondPass(BitmapData sourceData, Bitmap output, int width, int height,
            Rectangle bounds)
        {
            BitmapData outputData = null;

            try
            {
                outputData = output.LockBits(bounds, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

                IntPtr pSourceRow = sourceData.Scan0;
                IntPtr pSourcePixel = pSourceRow;
                IntPtr pPreviousPixel = pSourcePixel;

                IntPtr pDestinationRow = outputData.Scan0;
                IntPtr pDestinationPixel = pDestinationRow;

                byte pixelValue = QuantizePixel(new Color32(pSourcePixel));

                Marshal.WriteByte(pDestinationPixel, pixelValue);

                for (int row = 0; row < height; row++)
                {
                    pSourcePixel = pSourceRow;

                    pDestinationPixel = pDestinationRow;

                    for (int col = 0; col < width; col++)
                    {
                        if (Marshal.ReadInt32(pPreviousPixel) != Marshal.ReadInt32(pSourcePixel))
                        {
                            pixelValue = QuantizePixel(new Color32(pSourcePixel));
                            pPreviousPixel = pSourcePixel;
                        }

                        Marshal.WriteByte(pDestinationPixel, pixelValue);

                        pSourcePixel = (IntPtr)((long)pSourcePixel + _pixelSize);
                        pDestinationPixel = (IntPtr)((long)pDestinationPixel + 1);
                    }

                    pSourceRow = (IntPtr)((long)pSourceRow + sourceData.Stride);
                    pDestinationRow = (IntPtr)((long)pDestinationRow + outputData.Stride);
                }
            }
            finally
            {
                output.UnlockBits(outputData);
            }   
        }

        protected virtual void InitialQuantizePixel(Color32 pixel)
        { }

        protected abstract byte QuantizePixel(Color32 pixel);
        protected abstract ColorPalette GetPalette(ColorPalette original);
    }
}
