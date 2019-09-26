namespace Orikivo.Systems.Wrappers.Tenor.Parameters.Enumerators
{
    public enum TenorRating
    {
        Disabled, Moderate, Mild, Strict
    }

    public static class SafeReturn
    {
        public static string ToSafeString(this TenorRating rating)
        {
            switch (rating)
            {
                case TenorRating.Disabled:
                    return "";
                case TenorRating.Moderate:
                    return "moderate";
                case TenorRating.Mild:
                    return "mild";
                case TenorRating.Strict:
                    return "strict";
                default:
                    return "";

            }
        }
    }
}