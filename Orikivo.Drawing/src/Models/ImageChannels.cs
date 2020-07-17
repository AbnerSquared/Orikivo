using System.Drawing;

namespace Orikivo.Drawing
{
    /// <summary>
    /// Represents an image from which their color channels are separated into three layers.
    /// </summary>
    public class ImageChannels
    {
        private ImageChannels() { }

        public static ImageChannels FromImage(Bitmap image)
        {
            Grid<Color> pixels = ImageEditor.GetPixelData(image);
            var channels = new ImageChannels();

            channels.Width = image.Width;
            channels.Height = image.Height;

            channels.Red = channels.Green = channels.Blue = new Grid<float>(image.Size, 0);

            channels.Red.SetEachValue((x, y) => RangeF.Convert(0, 255, 0, 1, pixels[x, y].R));
            channels.Green.SetEachValue((x, y) => RangeF.Convert(0, 255, 0, 1, pixels[x, y].G));
            channels.Blue.SetEachValue((x, y) => RangeF.Convert(0, 255, 0, 1, pixels[x, y].B));

            return channels;
        }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public Grid<float> Red { get; private set; }

        public Grid<float> Green { get; private set; }

        public Grid<float> Blue { get; private set; }

        public Bitmap Build()
        {
            // Merge all RGB values into a color
            var image = new Grid<Color>(Width, Height);

            image.SetEachValue((x, y) => ImmutableColor.FromRange(Red[x, y], Green[x, y], Blue[x, y]));

            return ImageEditor.CreateArgbBitmap(image.Values);
        }
    }
}
