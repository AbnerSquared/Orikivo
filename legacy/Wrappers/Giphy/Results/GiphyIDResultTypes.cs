using Newtonsoft.Json;
using Orikivo.Systems.Wrappers.Giphy.Objects.GifObject;

namespace Orikivo.Systems.Wrappers.Giphy.Results
{
    public class GiphyIdResult
    {
        [JsonProperty("data")] public GiphyData Data { get; set; }
        [JsonProperty("meta")] public GiphyMetaData Meta { get; set; }
    }

    public class GiphyIdResults
    {
        [JsonProperty("data")] public GiphyData[] Data { get; set; }
        [JsonProperty("meta")] public GiphyMetaData Meta { get; set; }
    }
}