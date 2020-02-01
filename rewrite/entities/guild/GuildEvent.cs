using Newtonsoft.Json;

namespace Orikivo
{
    /// <summary>
    /// Represents information about what to write in a guild when an event occurs that matches the specified type.
    /// </summary>
    public class GuildEvent
    {
        public GuildEvent(EventType type)
        { }

        [JsonProperty("event_type")]
        public EventType Type { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }

        [JsonIgnore]
        public bool HasImage => Checks.NotNull(ImageUrl);
    }
}
