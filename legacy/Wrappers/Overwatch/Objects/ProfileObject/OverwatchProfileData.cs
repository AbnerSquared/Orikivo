using Newtonsoft.Json;

namespace Orikivo.Systems.Wrappers.Overwatch.Objects.ProfileObject
{
    public class OverwatchProfileData
    {
        [JsonProperty("icon")] public string Icon { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("level")] public int Level { get; set; }
        [JsonProperty("levelIcon")] public string LevelIcon { get; set; }
        [JsonProperty("prestige")] public int Prestige { get; set; }
        [JsonProperty("prestigeIcon")] public string PrestigeIcon { get; set; }
        [JsonProperty("rating")] public string Rating { get; set; }
        [JsonProperty("ratingIcon")] public string RatingIcon { get; set; }
        [JsonProperty("ratingName")] public string RatingName { get; set; }
        [JsonProperty("gamesWon")] public int GamesWon { get; set; }
        [JsonProperty("quickPlayStats")] public OverwatchGameData QuickPlayStats { get; set; }
        [JsonProperty("competitiveStats")] public OverwatchGameData CompetitiveStats { get; set; }
    }
}