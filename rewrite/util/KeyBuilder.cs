using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    /// <summary>
    /// A static helper used to easily create random strings.
    /// </summary>
    public static class KeyBuilder
    {
        public const int DefaultKeyLength = 8;
        private static readonly string alphanumeric = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";

        /// <summary>
        /// Randomly generates a key with a length of 8.
        /// </summary>
        public static string Generate()
            => Generate(DefaultKeyLength);

        /// <summary>
        /// Randomly generates a key with a specified length.
        /// </summary>
        /// <param name="len">The length of the key.</param>
        public static string Generate(int len)
            => OriRandom.GetChars(alphanumeric, (len > 256) ? 256 : (len < 1 ? 1 : len));
    }
}
