using System;
using System.Collections;
using System.Drawing;

namespace Orikivo.Drawing
{
    // Doesn't currently work.
    public class GrayscaleQuantizer : PaletteQuantizer
    {
        public GrayscaleQuantizer() : base (new ArrayList())
        {
            Colors = new Color[256];
            const int nColors = 256;

            for (uint i = 0; i < nColors; i++)
            {
                const uint alpha = 0xFF;
                uint intensity = Convert.ToUInt32(i * 0xFF / (nColors - 1));

                Colors[i] = Color.FromArgb((int)alpha,
                    (int)intensity,
                    (int)intensity,
                    (int)intensity);
            }
        }

        protected override byte QuantizePixel(Color32 pixel)
        {
            double luminance = pixel.Red * 0.299 + pixel.Green * 0.587 + pixel.Blue * 0.114;

            byte colorIndex = (byte)(luminance + 0.5);

            return colorIndex;
        }

    }
}
