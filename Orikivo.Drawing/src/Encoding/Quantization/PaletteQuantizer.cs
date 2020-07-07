using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace Orikivo.Drawing
{
    // NOTE: Referenced from the following GitHub projects:
    // https://github.com/mrousavy/AnimatedGif
    public class PaletteQuantizer : Quantizer
    {
        private readonly Hashtable _colorMap;
        protected Color[] Colors;
        public PaletteQuantizer(ArrayList palette) : base(true)
        {
            _colorMap = new Hashtable();
            Colors = new Color[palette.Count];
            palette.CopyTo(Colors);

        }

        protected override byte QuantizePixel(Color32 pixel)
        {
            byte colorIndex = 0;
            int colorHash = pixel.Argb;

            if (_colorMap.ContainsKey(colorHash))
            {
                colorIndex = (byte)_colorMap[colorHash];
            }
            else
            {
                if (pixel.Alpha == 0)
                {
                    for (int index = 0; index < Colors.Length; index++)
                    {
                        if (Colors[index].A == 0)
                        {
                            colorIndex = (byte)index;
                            break;
                        }
                    }
                }
                else
                {
                    int leastDistance = int.MaxValue;
                    int red = pixel.Red;
                    int green = pixel.Green;
                    int blue = pixel.Blue;

                    for (int index = 0; index < Colors.Length; index++)
                    {
                        Color paletteColor = Colors[index];

                        int redDistance = paletteColor.R - red;
                        int greenDistance = paletteColor.G - green;
                        int blueDistance = paletteColor.B - blue;

                        int distance = redDistance * redDistance +
                            greenDistance * greenDistance +
                            blueDistance * blueDistance;

                        if (distance < leastDistance)
                        {
                            colorIndex = (byte)index;
                            leastDistance = distance;


                            if (0 == distance)
                                break;
                        }
                    }
                }

                _colorMap.Add(colorHash, colorIndex);
            }

            return colorIndex;
        }

        protected override ColorPalette GetPalette(ColorPalette palette)
        {
            for (int index = 0; index < Colors.Length; index++)
            {
                palette.Entries[index] = Colors[index];
            }

            return palette;
        }
    }
}
