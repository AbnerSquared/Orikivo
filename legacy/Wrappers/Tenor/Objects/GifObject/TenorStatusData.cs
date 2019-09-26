using Newtonsoft.Json;

namespace Orikivo.Systems.Wrappers.Tenor.Objects.GifObject
{
    public class TenorStatusData
    {
        [JsonProperty("status")] public string Status { get; set; }
    }
}