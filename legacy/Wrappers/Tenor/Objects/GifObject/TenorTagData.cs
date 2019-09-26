using Newtonsoft.Json;

namespace Orikivo.Systems.Wrappers.Tenor.Objects.GifObject
{
    public class TenorTagData
    {
        [JsonProperty("tags")] public TenorTagDataType[] Tags { get; set; }
    }
}