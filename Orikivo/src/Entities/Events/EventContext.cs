using Discord.WebSocket;
using System;

namespace Orikivo
{
    // Moderation class
    /// <summary>
    /// Context used to assist when parsing any matching keys from an <see cref="EventType"/>.
    /// </summary>
    public class EventContext
    {
        internal EventContext(BaseGuild server, SocketGuild guild, SocketGuildUser user)
        {
            Server = server;
            Guild = guild;
            User = user;
            ReceivedAt = DateTime.UtcNow;
        }

        public BaseGuild Server { get; }

        public SocketGuild Guild { get; }

        public SocketGuildUser User { get; }

        public DateTime ReceivedAt { get; }
    }
}
