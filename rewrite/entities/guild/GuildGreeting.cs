namespace Orikivo
{

    public class GuildGreeting
    {
        public string ImageUrl { get; set; }
        public string Frame { get; set; }
        public bool HasImage => !string.IsNullOrWhiteSpace(ImageUrl);
    }

    // custom commands could used
    // a custom command framework
}
