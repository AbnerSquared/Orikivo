namespace Orikivo
{
    // instead shoot for GuildEvent
    // which would specify the type of event you wish to log.

    public class GuildGreeting
    {
        public string ImageUrl { get; set; }
        public string Frame { get; set; }
        public bool HasImage => !string.IsNullOrWhiteSpace(ImageUrl);
    }

    // custom commands could used
    // a custom command framework
}
