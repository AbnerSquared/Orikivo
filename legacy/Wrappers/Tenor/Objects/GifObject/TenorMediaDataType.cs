using Newtonsoft.Json;

namespace Orikivo.Systems.Wrappers.Tenor.Objects.GifObject
{
    public class TenorMediaDataType
    {
        [JsonProperty("preview")] public string Preview { get; set; }
        [JsonProperty("url")] public string Url { get; set; }
        [JsonProperty("dims")] public int[] Dims { get; set; }
        [JsonProperty("size")] public int Size { get; set; }
    }
}