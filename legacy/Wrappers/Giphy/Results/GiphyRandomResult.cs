using Newtonsoft.Json;
using Orikivo.Systems.Wrappers.Giphy.Objects.GifObject;

namespace Orikivo.Systems.Wrappers.Giphy.Results
{
    public class GiphyRandomResult
    {
        [JsonProperty("data")] public GiphyRandomData Data { get; set; }
        [JsonProperty("meta")] public GiphyMetaData Meta { get; set; }
    }
}