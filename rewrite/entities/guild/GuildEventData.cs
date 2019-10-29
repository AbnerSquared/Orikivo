using Newtonsoft.Json;

namespace Orikivo
{
    /// <summary>
    /// Represents information about what to write in a guild when an event occurs that matches the specified type.
    /// </summary>
    public class GuildEventData
    {
        public GuildEventData(GuildEvent type)
        { }

        [JsonProperty("event_type")]
        public GuildEvent Type { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }

        [JsonIgnore]
        public bool HasImage => Checks.NotNull(ImageUrl);
    }
}
