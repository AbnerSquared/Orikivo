using System;

namespace Orikivo
{
    // refer to Discord.Net.Utils
    internal static class ByteExtensions
    {
        /// <summary>
        /// Returns the bit at the specified index of a <see cref="byte"/>.
        /// </summary>
        /// <param name="bitIndex">The index of the bit to search for (zero-based).</param>
        public static bool BitAt(this byte b, int bitIndex)
            => (b & (1 << bitIndex)) != 0;

        public static byte SetBit(this byte b, int bitIndex, bool bit)
        {
            bool[] bits = GetBits(b);
            bits[bitIndex] = bit;

            return ToByte(bits);
        }

        public static bool[] GetBits(this byte b)
        {
            bool[] bits = new bool[8];

            for (int i = 0; i < 8; i++)
                bits[i] = BitAt(b, i);

            return bits;
        }

        public static byte ToByte(this bool[] bits)
        {
            if (bits.Length != 8)
                throw new ArgumentException("You must specify exactly 8 bits.");

            byte b = 0x00;

            for (int i = 0; i < 8; i++)
                b |= (byte) (bits[i] ? (0x01 << i) : 0x00);

            return b;
        }
    }
}
