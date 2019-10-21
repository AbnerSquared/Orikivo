namespace Orikivo
{
    // refer to Discord.Net.Utils

    internal static class Int32Extensions
    {
        public static bool IsInRange(this int i, int max)
            => i <= max - 1 && i >= 0;
        public static bool IsInRange(this int i, int min, int max)
            => i <= max - 1 && i >= min;
    }
}
