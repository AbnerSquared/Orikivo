namespace Orikivo.Drawing.Encoding.Gif
{
    public enum InterlaceFlag : byte
    {
        NotInterlaced = 0, // 00000000
        Interlaced = 64 // 01000000
    }

    // Defines the palette size of an image
    // GIF only allows a maximum of 256 unique colors, so bpp8 is the max
    public enum BitDepth : byte
    {
        Bpp1 = 0, // 00000000   2

        Bpp2 = 1, // 00000001   4

        Bpp3 = 2, // 00000010   8

        Bpp4 = 3, // 00000011  16

        Bpp5 = 4, // 00000100  32

        Bpp6 = 5, // 00000101  64

        Bpp7 = 6, // 00000110 128

        Bpp8 = 7  // 00000111 256
    }

    public class ImageDescriptor
    {
        private const byte IMAGE_SEPARATOR = 0x2C;

        // Byte 1-2
        public ushort LeftPosition;

        // Byte 3-4
        public ushort TopPosition;

        // Byte 5-6
        public ushort Width; // 2 bytes

        // Byte 7-8
        public ushort Height; // 2 bytes

        // Byte 9
        // <Packed Fields>

        // 7-------
        // Local Color Table Flag
        // This will be set to true if a LocalColorTable is specified.

        // -6------
        // Interlace Flag
        public InterlaceFlag InterlaceMethod;

        // --5-----
        // Sort Flag
        public ColorSortFlag ColorSortMethod;

        // ---43---
        // Reserved
        public byte Reserved => 0; // 00000000

        // -----210
        // Size of Local Color Table
        // This is the Bits Per Pixel, up to a max of 8 bpp (-1)
        // Example: 8 - 1 => 7 => 111; 4 - 1 => 3 => 011
        // This will be automatically retrieved by calculating the length of the local color table
    }
}
