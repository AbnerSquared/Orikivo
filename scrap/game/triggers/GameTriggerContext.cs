using Discord.WebSocket;
using System;

namespace Orikivo
{
    /// <summary>
    /// A global trigger context for a game derived from a <see cref="Discord.WebSocket.SocketMessage"/>.
    /// </summary>
    public class GameTriggerContext
    {
        public GameTriggerContext(SocketUser user) => throw new NotImplementedException();

        public OriAuthor Author { get; }
        public string GameId { get; }
        public ulong ChannelId { get; }
        public ulong MessageId { get; }
        public string Message { get; }
    }
}
