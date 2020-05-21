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
    }

    public class BufferBlock : DataBlock
    {
        private const byte BLOCK_LENGTH = 3;
        private const byte BLOCK_ID = 0x02;

        // unsigned 4-byte integer in little-endian
        public uint BufferSize;
    }
}
