using System.Drawing;
using System.Drawing.Imaging;

namespace Orikivo.Drawing.Encoding
{
    /// <summary>
    /// Defines how the <see cref="ColorPalette"/> quality is stored when encoding a GIF.
    /// </summary>
    public enum Quality
    {
        /// <summary>
        /// Inherits the <see cref="ColorPalette"/> provided from a <see cref="Image"/>.
        /// </summary>
        Inherit = 0,

        /// <summary>
        /// Sets the <see cref="ColorPalette"/> for an <see cref="Image"/> to 1 bit per pixel, providing up to 2 different color entries.
        /// </summary>
        Bpp1 = 1,

        /// <summary>
        /// Sets the <see cref="ColorPalette"/> for an <see cref="Image"/> to 2 bits per pixel, providing up to 4 different color entries.
        /// </summary>
        Bpp2 = 2,

        /// <summary>
        /// Sets the <see cref="ColorPalette"/> for an <see cref="Image"/> to 4 bits per pixel, providing up to 16 different color entries.
        /// </summary>
        Bpp4 = 4,

        /// <summary>
        /// Sets the <see cref="ColorPalette"/> for an <see cref="Image"/> to 8 bits per pixel, providing up to 256 different color entries.
        /// </summary>
        Bpp8 = 8,

        /// <summary>
        /// Sets the <see cref="ColorPalette"/> for an <see cref="Image"/> to 8 bits per pixel in grayscale, providing up to 256 different color entries from #000000 to #FFFFFF.
        /// </summary>
        Grayscale = 9
    }
}
