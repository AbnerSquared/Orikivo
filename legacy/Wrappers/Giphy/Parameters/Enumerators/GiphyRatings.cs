namespace Orikivo.Systems.Wrappers.Giphy.Parameters.Enumerators
{
    public enum GiphyRatings { Empty, Y, G, Pg, Pg13, R }

    public static class GiphyRatingReturn
    {
        public static string ToRatingString(this GiphyRatings rating)
        {
            switch (rating)
            {
                case GiphyRatings.Empty:
                    return "";
                case GiphyRatings.Y:
                    return "y";
                case GiphyRatings.G:
                    return "g";
                case GiphyRatings.Pg:
                    return "pg";
                case GiphyRatings.Pg13:
                    return "pg-13";
                case GiphyRatings.R:
                    return "r";
                default:
                    return "";
            }
        }
    }
}