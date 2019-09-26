using Orikivo.Systems.Wrappers.Giphy.Parameters.Enumerators;

namespace Orikivo.Systems.Wrappers.Giphy.Parameters
{
    public class GiphyTranslation
    {
        public string Phrase { get; set; }
        public GiphyRatings Rating { get; set; }
        public string Format { get; set; }
    }
}