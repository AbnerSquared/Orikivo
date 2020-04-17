using Discord;

namespace Orikivo.Framework
{
    public class ActivityConfig
    {
        public string Name { get; set; }

        public string StreamUrl { get; set; }

        public ActivityType Type { get; set; } = ActivityType.Playing;
    }
}
