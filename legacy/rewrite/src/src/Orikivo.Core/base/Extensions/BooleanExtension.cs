namespace Orikivo
{
    public static class BooleanExtension
    {
        public static string ToToggleString(this bool b)
            => b ? "Enabled" : "Disabled";

        public static string AsCoinFlip(this bool b)
            => b ? "H" : "T";
    }
}