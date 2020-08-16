namespace Orikivo.Drawing.Encoding.Gif.Netscape
{
    // https://web.archive.org/web/20140830100320/http://odur.let.rug.nl/~kleiweg/gif/netscape.html
    public class LoopBlock : DataBlock
    {
        private const byte BLOCK_LENGTH = 3; // 3 bytes of data

        // Byte 1
        // sub-block ID
        private const byte BLOCK_ID = 0x01;

        // Byte 2-3
        // Loop Count
        // If 0, loop forever
        public ushort? LoopCount;

        protected override byte BlockSize => BLOCK_LENGTH;

        public override byte[] Data => GetByteArray();

        private byte[] GetByteArray()
        {
            var bytes = new byte[BLOCK_LENGTH]; // 3

            bytes[0] = BLOCK_ID;

            byte loopHighByte = 0;
            byte loopLowByte = 0;

            if (LoopCount.HasValue)
            {
                loopHighByte = (byte) (LoopCount & 0xFF);
                loopLowByte = (byte) ((LoopCount >> 8) & 0xFF);
            }

            bytes[1] = loopHighByte;
            bytes[2] = loopLowByte;

            return bytes;
        }
    }
}
