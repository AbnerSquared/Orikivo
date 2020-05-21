namespace Orikivo.Drawing.Encoding.Gif
{
    // this is a Data Sub-Block, used by a couple of extensions
    // The extension block is a modified Data Sub-Block
    public class DataBlock
    {
        private const byte MAX_DATA_SIZE = 255;
        private const byte MIN_DATA_SIZE = 1;

        // At a minimum of 1 byte, to a maximum of 255 bytes
        protected byte BlockSize;

        // A collection of bytes, up to 255 at most
        public byte[] Data;
    }
}
