using Newtonsoft.Json;

namespace Orikivo.Systems.Wrappers.Giphy.Objects.GifObject
{
    public class GiphyPagingData
    {
        [JsonProperty("offset")] public int Offset { get; set; }
        [JsonProperty("total_count")] public int TotalCount { get; set; }
        [JsonProperty("count")] public int Count { get; set; }
    }
}