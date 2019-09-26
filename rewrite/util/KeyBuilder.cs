using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    // used to randomly generate keys and codes
    public static class KeyBuilder
    {
        public const int DefaultKeyLength = 8;
        private static readonly string alphanumeric = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";

        public static string Generate()
            => Generate(DefaultKeyLength);

        public static string Generate(int len)
            => OriRandom.GetChars(alphanumeric, (len > 256) ? 256 : (len < 1 ? 1 : len));
    }
}
