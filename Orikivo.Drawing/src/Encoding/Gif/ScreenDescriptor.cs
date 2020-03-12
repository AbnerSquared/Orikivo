namespace Orikivo.Drawing.Encoding.Gif
{
    public class ScreenDescriptor
    {
        // Logical Screen Descriptor (get from GIF)
        ushort ScreenWidth; // 2 bytes, minimum width required to display image data
        ushort ScreenHeight; // 2 bytes, minimum height required to display image data

        // 01234567, byte index chart
        // each byte index
        // 765  => Global Color Table Size == bpp - 1
        // 4    => Color Table Sort Flag // 1 to sort by most important colors (89a), otherwise 0
        // 321  => Color Resolution // # of bits in entry of original color palette - 1
        // 0    => Global Color Table Flag // 1 if global color table exists, otherwise 0
        byte Packed;

        byte BackgroundColor; // index value of the color into the global color table (where pixels arent drawn)

        byte AspectRatio; // if 0, no aspect ratio is specified.
    }
}
