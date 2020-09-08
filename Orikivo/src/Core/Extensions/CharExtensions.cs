namespace Orikivo
{
    public static class CharExtensions
    {
        /// <summary>
        /// Returns a new string in which this character is escaped.
        /// </summary>
        public static string Escape(this char c)
            => $"\\{c}";
    }
}
