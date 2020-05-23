using System;

namespace Orikivo.Drawing.Encoding.Gif
{

    public class GraphicControlExtension : ExtensionBlock
    {
        protected override byte Label => 0xF9;
        protected override byte? BlockSize => 4;


        // Byte 1
        // <Packed Fields>

        // 000-----
        // Reserved - 3 Bits
        public byte Reserved => 0;

        // ---000--
        // Disposal Method - 3 Bits
        public DisposalMethod OnDisposal; // 432

        // ------0-
        // User Input Flag - 1 Bit (Boolean)
        // It is recommended that the encoder not set the
        // User Input Flag without a Delay Time specified.
        // HasUserInput ? 00000010 : 00000000
        public bool HasUserInput; // 1, UserInputFlag

        // -------0
        // Transparent Color Flag - 1 Bit (Boolean)
        // This flag is automatically set if TransparencyIndex is specified
        // TransparencyIndex.HasValue ? 00000001 : 00000000

        // Byte 2-3
        public ushort? DelayTime; // If unspecified, 0

        // Byte 4
        public byte? TransparencyIndex;

        // This encodes all of the specified values into the proper formatting.
        // This does not provide the completely built extension block.
        protected override byte[] GetByteArray()
        {
            var bytes = new byte[BlockSize.Value]; // 4

            // Byte 1
            byte packed = (byte) (Reserved
                | (byte) OnDisposal
                | (byte) (HasUserInput ? 2 : 0)
                | (byte) (TransparencyIndex.HasValue ? 1 : 0));

            byte delayHighByte = 0;
            byte delayLowByte = 0;

            // Byte 2-3
            if (DelayTime.HasValue)
            {
                // 00000000--------
                delayHighByte = (byte) (DelayTime.Value & 0xFF);

                // --------00000000
                delayLowByte = (byte) ((DelayTime.Value >> 8) & 0xFF); 
            }

            byte transparencyIndex = TransparencyIndex ?? 0;

            bytes[0] = packed;
            bytes[1] = delayHighByte;
            bytes[2] = delayLowByte;
            bytes[3] = transparencyIndex;

            return bytes;
        }
    }
}
