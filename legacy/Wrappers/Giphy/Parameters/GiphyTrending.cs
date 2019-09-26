using Orikivo.Systems.Wrappers.Giphy.Parameters.Enumerators;

namespace Orikivo.Systems.Wrappers.Giphy.Parameters
{
    public class GiphyTrending
    {
        public int Limit { get; set; }
        public GiphyRatings Rating { get; set; }
        public string Format { get; set; }
    }
}