using Discord.WebSocket;

namespace Orikivo
{
    /// <summary>
    /// Context used to assist when parsing any matching keys from a <see cref="GuildEvent"/>.
    /// </summary>
    public class GuildEventContext
    {
        internal GuildEventContext(OriGuild server, SocketGuild guild, SocketGuildUser user)
        {
            Server = server;
            Guild = guild;
            User = user;
        }

        public OriGuild Server { get; }
        public SocketGuild Guild { get; }
        public SocketGuildUser User { get; }
    }
}
