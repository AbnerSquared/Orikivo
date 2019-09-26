using Newtonsoft.Json;

namespace Orikivo.Systems.Wrappers.Giphy.Objects.GifObject
{
    public class GiphyMetaData
    {
        [JsonProperty("msg")] public string Message { get; set; }
        [JsonProperty("status")] public int Status { get; set; }
        [JsonProperty("response_id")] public string ResponseId { get; set; }
    }
}