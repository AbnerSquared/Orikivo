using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Orikivo.Drawing
{
    public class OctreeQuantizer : Quantizer
    {
        private readonly int _maxColors;
        private readonly Octree _octree;

        public OctreeQuantizer(int maxColors, int maxColorBits) : base(false)
        {
            if (maxColors > 255)
                throw new ArgumentOutOfRangeException();

            if ((maxColorBits < 1) | (maxColorBits > 8))
                throw new ArgumentOutOfRangeException();

            _octree = new Octree(maxColorBits);
            _maxColors = maxColors;
        }

        protected override void InitialQuantizePixel(Color32 pixel)
        {
            _octree.AddColor(pixel);
        }

        protected override byte QuantizePixel(Color32 pixel)
        {
            byte paletteIndex = (byte)_maxColors;

            if (pixel.Alpha > 0)
                paletteIndex = (byte)_octree.GetPaletteIndex(pixel);

            return paletteIndex;
        }

        protected override ColorPalette GetPalette(ColorPalette original)
        {
            var palette = _octree.Palletize(_maxColors - 1);

            for (int index = 0; index < palette.Count; index++)
                original.Entries[index] = (Color)palette[index];

            original.Entries[_maxColors] = Color.FromArgb(0, 0, 0, 0);

            return original;
        }
    }
}
