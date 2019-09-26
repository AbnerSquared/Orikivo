using Newtonsoft.Json;

namespace Orikivo.Systems.Wrappers.Overwatch.Objects.ProfileObject
{
    public class OverwatchGameData
    {
        [JsonProperty("eliminationsAvg")] public decimal EliminationsAvg { get; set; }
        [JsonProperty("damageDoneAvg")] public int DamageDoneAvg { get; set; }
        [JsonProperty("deathsAvg")] public decimal DeathsAvg { get; set; }
        [JsonProperty("finalBlowAvg")] public decimal FinalBlowsAvg { get; set; }
        [JsonProperty("healingDoneAvg")] public int HealingDoneAvg { get; set; }
        [JsonProperty("objectiveKillsAvg")] public decimal ObjectiveKillsAvg { get; set; }
        [JsonProperty("objectiveTimeAvg")] public string ObjectiveTimeAvg { get; set; }
        [JsonProperty("soloKillsAvg")] public decimal SoloKillsAvg { get; set; }
        [JsonProperty("games")] public OverwatchGameWinningData Games { get; set; }
        [JsonProperty("awards")] public OverwatchGameAwardData Awards { get; set; }
    }
}