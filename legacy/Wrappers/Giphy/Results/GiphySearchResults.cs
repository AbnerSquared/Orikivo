using Newtonsoft.Json;
using Orikivo.Systems.Wrappers.Giphy.Objects.GifObject;

namespace Orikivo.Systems.Wrappers.Giphy.Results
{
    public class GiphySearchResults
    {
        [JsonProperty("data")] public GiphyData[] Data { get; set; }
        [JsonProperty("meta")] public GiphyMetaData Meta { get; set; }
        [JsonProperty("pagination")] public GiphyPagingData Pagination { get; set; }
    }
}