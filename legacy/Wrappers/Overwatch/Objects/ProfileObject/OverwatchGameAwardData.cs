using Newtonsoft.Json;

namespace Orikivo.Systems.Wrappers.Overwatch.Objects.ProfileObject
{
    public class OverwatchGameAwardData
    {
        [JsonProperty("cards")] public int Cards { get; set; }
        [JsonProperty("medals")] public int Medals { get; set; }
        [JsonProperty("medalsBronze")] public int MedalsBronze { get; set; }
        [JsonProperty("medalsSilver")] public int MedalsSilver { get; set; }
        [JsonProperty("medalsGold")] public int MedalsGold { get; set; }
    }
}