namespace Orikivo
{
    /// <summary>
    /// A static helper used to easily create random strings.
    /// </summary>
    public static class KeyBuilder
    {
        private static readonly string _alphanumeric = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";

        /// <summary>
        /// Randomly generates a key of a specified size.
        /// </summary>
        /// <param name="size">The size of the key.</param>
        public static string Generate(int size)
            => OriRandom.GetChars(_alphanumeric, (size > 256) ? 256 : (size < 1 ? 1 : size));
    }
}
