using System.Drawing;

namespace Orikivo.Drawing
{
    /// <summary>
    /// Represents an image from which their color channels are separated into three grids.
    /// </summary>
    public class ImageChannels
    {
        private ImageChannels() { }
        public static ImageChannels FromImage(Bitmap image)
        {
           Grid<Color> pixels = GraphicsUtils.GetPixels(image);

            ImageChannels channels = new ImageChannels();

            channels.R = channels.G = channels.B = new Grid<float>(image.Size, 0.0f);

            channels.R.SetEachValue((int x, int y) => RangeF.Convert(0, 255, 0.0f, 1.0f, pixels.GetValue(x, y).R));
            channels.G.SetEachValue((int x, int y) => RangeF.Convert(0, 255, 0.0f, 1.0f, pixels.GetValue(x, y).G));
            channels.B.SetEachValue((int x, int y) => RangeF.Convert(0, 255, 0.0f, 1.0f, pixels.GetValue(x, y).B));

            return channels;
        }

        public Grid<float> R { get; private set; }
        public Grid<float> G { get; private set; }
        public Grid<float> B { get; private set; }
    }
}
