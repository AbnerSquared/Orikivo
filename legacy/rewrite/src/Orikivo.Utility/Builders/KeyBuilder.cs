using Discord;
using Orikivo.Providers;

namespace Orikivo.Utility
{
    public static class KeyBuilder
    {
        public const int DefaultKeyLength = 8;
        private static string alphanumeric = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";

        public static string Generate()
            => Generate(DefaultKeyLength);

        public static string Generate(int length)
        {
            length = length.InRange(1, EmbedBuilder.MaxTitleLength);
            char[] tree = new char[length];
            for (int i = 0; i < tree.Length; i++)
                tree[i] = alphanumeric[RandomProvider.Instance.Next(alphanumeric.Length)];

            return new string(tree);
        }
    }
}