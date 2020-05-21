namespace Orikivo.Drawing.Encoding.Gif
{
    // Tip:
    // If characters less than 0x20 or greater than 0xf7 are encountered,
    // it is recommended that the decoder display a Space character (0x20).

    public class PlainTextExtension : ExtensionBlock
    {
        protected override byte Label => 0x01;
        protected override byte? BlockSize => 12;

        // Byte 1-2
        // Text Grid Left Position
        public ushort GridLeftPosition;

        // Byte 3-4
        // Text Grid Top Position
        public ushort GridTopPosition;

        // Byte 5-6
        // Image Grid Width
        public ushort GridWidth;

        // Byte 7-8
        // Image Grid Height
        public ushort GridHeight;

        // Byte 9
        // Character Cell Width
        public byte CellWidth;

        // Byte 10
        // Character Cell Height
        public byte CellHeight;

        // Byte 11
        // Text Foreground Color Index
        public byte ForegroundColorIndex;

        // Byte 12
        // Text Background Color Index
        public byte BackgroundColorIndex;

        // N
        // The data value representing the plain text.
        public DataBlock[] PlainTextData;

        protected override byte[] GetInternalData()
        {
            throw new System.NotImplementedException();
        }
    }
}
