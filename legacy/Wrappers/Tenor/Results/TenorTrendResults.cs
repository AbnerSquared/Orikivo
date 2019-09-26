using Newtonsoft.Json;
using Orikivo.Systems.Wrappers.Tenor.Objects.GifObject;

namespace Orikivo.Systems.Wrappers.Tenor.Results
{
    public class TenorTrendResults
    {
        [JsonProperty("results")] public TenorResultData[] Results { get; set; }
        [JsonProperty("next")] public string Next { get; set; }
    }
}