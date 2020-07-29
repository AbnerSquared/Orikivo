using Discord;
using System;

namespace Arcadia
{
    public enum ConnectionType
    {
        // If guild-wise, utilize the State property
        Guild = 1,

        // If user-wise, ignore the State property
        Direct = 2
    }

    public class ConnectionProperties
    {
        public int Frequency { get; set; } = 0;
        public GameState State { get; set; } = GameState.Waiting;

        public bool CanDeleteMessages { get; set; } = false;

        // After 4 messages is sent that CANNOT be deleted, this screen is refreshed, which resends the content into
        // a new message body
        public int AutoRefreshCounter { get; set; } = 4;
    }

    public class ServerConnection
    {
        // every 4 messages, the InternalMessage will be updated.
        private static readonly int AfterMessageLimit = 4;
        public ServerConnection() {}

        public ServerConnection(ulong guildId, IMessageChannel channel,
            IUserMessage messageBind,
            int frequency = 0,
            GameState state = GameState.Waiting,
            bool canDeleteMessages = false)
        {
            GuildId = guildId;
            InternalChannel = channel;
            InternalMessage = messageBind;
            ChannelId = channel.Id;
            MessageId = messageBind.Id;
            Frequency = frequency;
            State = state;
            CouldDeleteMessages = canDeleteMessages;
        }

        public ServerConnection(IMessageChannel channel,
            IUserMessage messageBind,
            int frequency = 0,
            GameState state = GameState.Waiting,
            bool canDeleteMessages = false)
        {
            InternalChannel = channel;
            InternalMessage = messageBind;
            ChannelId = channel.Id;
            MessageId = messageBind.Id;
            Frequency = frequency;
            State = state;
            CouldDeleteMessages = canDeleteMessages;
        }

        public ulong? GuildId { get; set; }

        public ulong ChannelId { get; set; }

        public ulong MessageId { get; set; }

        // If this is specified, attempt to re-initialize the channel in this manner
        public string DirectId { get; set; }

        // only 1 channel can be bound to a server at a time
        // check to see if the specified channel is connected to anything else.
        // likewise, you can simply create a cache of channels with their ID and server ID
        // where should i listen to input?
        public IMessageChannel InternalChannel { get; set; }

        // what message should i update?
        public IUserMessage InternalMessage { get; set; }

        // what is the frequency of the display am I currently pointing to?
        public int Frequency { get; set; }

        // when was the last time this connection was refreshed
        // if the refreshrate is specified, if the time since the last refresh is shorter
        // don't refresh the console. likewise, you can create an async refresh that refreshes automatically
        // once the time to refresh has been met.
        internal DateTime LastRefreshed { get; set; }

        // this keeps track of how many messages were sent after the server message was sent.
        // if enough messages are sent after the lobby message and
        // can delete messages is false
        // a new message is sent in replacement
        internal int AfterMessageCount { get; set; }

        // determines if the bot can attempt to delete messages
        public bool CouldDeleteMessages { get; set; } = false;

        // determines if the bot can 100% delete messages in this channel
        public bool CanDeleteMessages { get; set; } = false;

        // this determines what is currently being executed in the server connection
        public GameState State { get; set; }
    }
}
