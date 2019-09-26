using Orikivo.Systems.Wrappers.Giphy.Parameters.Enumerators;

namespace Orikivo.Systems.Wrappers.Giphy.Parameters
{
    public class GiphySearch
    {
        public string Query { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public GiphyRatings Rating { get; set; }
        public string Format { get; set; }
    }
}