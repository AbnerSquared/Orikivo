using Discord.WebSocket;
using Newtonsoft.Json;
using Orikivo.Desync;

namespace Arcadia
{
    public class ArcadeGuild : BaseGuild
    {
        public ArcadeGuild(SocketGuild guild)
            : base(guild)
        {
            Balance = 0;
            Exp = 0;
        }

        [JsonProperty("balance")]
        public ulong Balance { get; internal set; }

        [JsonProperty("exp")]
        public ulong Exp { get; internal set; }
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
