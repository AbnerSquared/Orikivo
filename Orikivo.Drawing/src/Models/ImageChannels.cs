using System.Drawing;

namespace Orikivo.Drawing
{
    /// <summary>
    /// Represents an image from which their color channels are separated into three layers.
    /// </summary>
    public class ImageChannels
    {
        private ImageChannels(int width, int height)
        {
            Width = width;
            Height = height;
            Red = Green = Blue = new Grid<float>(width, height);
        }

        public static ImageChannels FromImage(Bitmap image)
        {
            Grid<Color> pixels = ImageHelper.GetPixelData(image);
            var channels = new ImageChannels(image.Width, image.Height);

            channels.Red.SetEachValue((x, y) => RangeF.Convert(0, 255, 0, 1, pixels[x, y].R));
            channels.Green.SetEachValue((x, y) => RangeF.Convert(0, 255, 0, 1, pixels[x, y].G));
            channels.Blue.SetEachValue((x, y) => RangeF.Convert(0, 255, 0, 1, pixels[x, y].B));

            return channels;
        }

        public int Width { get; }

        public int Height { get; }

        public Grid<float> Red { get; }

        public Grid<float> Green { get; }

        public Grid<float> Blue { get; }

        public Bitmap Build()
        {
            // Merge all RGB values into a color
            var image = new Grid<Color>(Width, Height);

            image.SetEachValue((x, y) => ImmutableColor.FromRange(Red[x, y], Green[x, y], Blue[x, y]));

            return ImageHelper.CreateArgbBitmap(image.Values);
        }
    }
}
