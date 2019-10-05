namespace Orikivo
{
    // instead shoot for GuildEvent
    // which would specify the type of event you wish to log.

    public class GuildEvent
    {
        public GuildEvent(GuildEventType eventType)
        { }
        // This message is what shows when the event occurs.
        public GuildEventType Type { get; set; }
        public string Message { get; set; }
        public string ImageUrl { get; set; }
        public bool HasImage => !string.IsNullOrWhiteSpace(ImageUrl);
    }

    // custom commands could used
    // a custom command framework
}
