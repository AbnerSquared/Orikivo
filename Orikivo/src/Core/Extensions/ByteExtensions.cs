using System;

namespace Orikivo
{
    internal static class ByteExtensions
    {
        /// <summary>
        /// Returns the bit at the specified index of this <see cref="byte"/>.
        /// </summary>
        public static bool GetBit(this byte b, int bitIndex)
            => (b & (1 << bitIndex)) != 0;

        /// <summary>
        /// Sets the value of a bit at the specified index of this <see cref="byte"/>.
        /// </summary>
        public static byte SetBit(this byte b, int bitIndex, bool bit)
        {
            bool[] bits = GetBits(b);

            bits[bitIndex] = bit;

            return ToByte(bits);
        }

        /// <summary>
        /// Returns an array containing all of the bits in this <see cref="byte"/>.
        /// </summary>
        public static bool[] GetBits(this byte b)
        {
            bool[] bits = new bool[8];

            for (int i = 0; i < 8; i++)
                bits[i] = GetBit(b, i);

            return bits;
        }

        /// <summary>
        /// Returns a new <see cref="byte"/> from a specified collection of bits.
        /// </summary>
        public static byte ToByte(this bool[] bits)
        {
            if (bits.Length != 8)
                throw new ArgumentException("You must specify exactly 8 bits.");
            byte b = 0x00;

            for (int i = 0; i < 8; i++)
                b |= (byte)(bits[i] ? (0x01 << i) : 0x00);

            return b;
        }
    }
}
