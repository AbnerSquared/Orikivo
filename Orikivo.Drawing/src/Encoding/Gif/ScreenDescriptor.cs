namespace Orikivo.Drawing.Encoding.Gif
{
    public enum ColorSortFlag : byte
    {
        // bit 4 is the only bit that utilizes this flag
        Unordered = 0, // 00000000
        DecreasingFrequency = 16 // 00010000
    }

    

    public class ScreenDescriptor
    {
        // Logical Screen Descriptor (get from GIF)
        
        // Byte 1-2
        public ushort ScreenWidth;
        
        // Byte 3-4
        public ushort ScreenHeight;

        // Byte 5
        // <Packed Fields>

        // 7-------
        // Global Color Table Flag
        // This is set to true if GlobalColorTable is specified.

        // -654----
        // Color Resolution
        public BitDepth ColorResolution;

        // ----3---
        // Sort Flag
        public ColorSortFlag ColorSortMethod;

        // -----210
        // Size of Global Color Table
        // This uses the same method as the Color Resolution data.
        // This will be automatically retrieved by calculating the length of the global color table


        // Byte 6
        // Background Color Index
        // The index of the palette from which the background color uses
        // If there isn't a global color table set, this should be zero and ignored
        public byte BackgroundColorIndex;

        // Byte 7
        // Pixel Aspect Ratio
        // If unspecified, set to 0
        public byte? PixelAspectRatio;
    }
}
