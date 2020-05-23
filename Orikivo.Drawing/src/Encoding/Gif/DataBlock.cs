namespace Orikivo.Drawing.Encoding.Gif
{
    // this is a Data Sub-Block, used by a couple of extensions
    // The extension block is a modified Data Sub-Block
    public class DataBlock
    {
        public DataBlock()
        {

        }

        public DataBlock(byte[] data)
        {
            if (data == null)
            {
                throw new System.Exception("The specified byte array is null");
            }
            else if (data.Length < MIN_DATA_SIZE || data.Length > MAX_DATA_SIZE)
            {
                throw new System.Exception("The specified byte array must be within the size of 1 and 255 bytes");
            }

            Data = data;
            BlockSize = (byte) data.Length;
        }

        private const byte MAX_DATA_SIZE = 255;
        private const byte MIN_DATA_SIZE = 1;

        // At a minimum of 1 byte, to a maximum of 255 bytes
        protected virtual byte BlockSize { get; }

        // A collection of bytes, up to 255 at most
        public virtual byte[] Data { get; }
    }
}
