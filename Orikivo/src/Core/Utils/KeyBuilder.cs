namespace Orikivo
{
    /// <summary>
    /// Represents a class that simplifies random string generation.
    /// </summary>
    public static class KeyBuilder
    {
        private static readonly string _alphanumeric = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";

        /// <summary>
        /// Generates a new key with a specified length.
        /// </summary>
        /// <param name="len">The length of the key to generate.</param>
        public static string Generate(int len)
            => Randomizer.GetChars(_alphanumeric, len);

        /// <summary>
        /// Generates a specified amount of keys at a specified length.
        /// </summary>
        /// <param name="len">The length of the keys to generate.</param>
        /// <param name="amount">The total amount of keys that will be generated.</param>
        public static string[] GenerateMany(int len, int amount)
        {
            var keys = new string[amount];

            for (int i = 0; i < amount; i++)
                keys[i] = Generate(len);

            return keys;
        }
    }
}
