using Newtonsoft.Json;

namespace Orikivo.Systems.Wrappers.Tenor.Objects.GifObject
{
    public class TenorTagDataType
    {
        [JsonProperty("searchterm")] public string SearchTerm { get; set; }
        [JsonProperty("path")] public string Path { get; set; }
        [JsonProperty("image")] public string Image { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
    }
}