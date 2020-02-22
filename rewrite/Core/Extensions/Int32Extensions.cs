using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    // refer to Discord.Net.Utils
    internal static class ByteExtensions
    {
        /// <summary>
        /// Returns the bit at the specified index of a <see cref="byte"/>.
        /// </summary>
        /// <param name="bitIndex">The index of the bit to search for (zero-based).</param>
        public static bool GetBit(this byte b, int bitIndex)
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
                bits[i] = GetBit(b, i);

            return bits;
        }

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
    internal static class Int32Extensions
    {
        public static bool IsInRange(this int i, int max)
            => i <= max - 1 && i >= 0;
        public static bool IsInRange(this int i, int min, int max)
            => i <= max - 1 && i >= min;

        // Creates an array starting from 0 up (or down) to int i.
        public static IEnumerable<int> Increment(this int max)
            => Int32Utils.Increment(0, Math.Abs(max), Math.Sign(max));
    }
}
