using Discord.WebSocket;
using System;

namespace Orikivo
{
    /// <summary>
    /// Context used to assist when parsing any matching keys from an <see cref="EventType"/>.
    /// </summary>
    public class EventContext
    {
        internal EventContext(OriGuild server, SocketGuild guild, SocketGuildUser user)
        {
            Server = server;
            Guild = guild;
            User = user;
            ReceivedAt = DateTime.UtcNow;
        }

        public OriGuild Server { get; }
        public SocketGuild Guild { get; }
        public SocketGuildUser User { get; }

        public DateTime ReceivedAt { get; }
    }
}
