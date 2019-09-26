using Newtonsoft.Json;

namespace Orikivo.Systems.Wrappers.Overwatch.Objects.ProfileObject
{
    public class OverwatchGameWinningData
    {
        [JsonProperty("played")] public int Played { get; set; }
        [JsonProperty("won")] public int Won { get; set; }
    }
}