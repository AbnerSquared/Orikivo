using Newtonsoft.Json;
using System;

namespace Orikivo
{
    // Moderation class
    // TODO: Try removing the set methods from each property.
    /// <summary>
    /// Represents information about what to write in a guild when an event occurs that matches the specified type.
    /// </summary>
    public class GuildEvent
    {
        public GuildEvent(EventType type, string message, string imageUrl = null)
        {
            if (Check.NotNull(imageUrl))
            {
                ExtensionType? url = EnumUtils.GetUrlExtension(imageUrl);

                if (url.HasValue)
                    if (url.Value != ExtensionType.Image)
                        throw new ArgumentException("The specified URL given is not an image file.");
            }

            Type = type;
            Message = message;
            ImageUrl = imageUrl;
        }

        [JsonProperty("event_type")]
        public EventType Type { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }

        [JsonIgnore]
        public bool HasImage => Check.NotNull(ImageUrl);
    }
}
