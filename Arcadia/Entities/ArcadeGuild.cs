using System.Collections.Generic;
using Discord;
using Newtonsoft.Json;
using Orikivo;

namespace Arcadia
{
    public class ArcadeGuild : BaseGuild
    {
        public ArcadeGuild(IGuild guild)
            : base(guild)
        {
            Balance = 0;
            Exp = 0;
        }

        [JsonProperty("balance")]
        public ulong Balance { get; internal set; }

        [JsonProperty("exp")]
        public ulong Exp { get; internal set; }

        [JsonProperty("stats")]
        public Dictionary<string, long> Stats { get; internal set; }

        [JsonProperty("quests")]
        public List<QuestData> Quests { get; internal set; }
    }

    // Arcadia property
    // public ulong Balance { get; internal set; }

    // Arcadia property
    // public ulong Exp { get; internal set; }

    // Moderation property
    // public List<GuildEvent> Events { get; internal set; }

    // Arcadia property
    // public List<GuildCommand> Commands { get; internal set; }
}
