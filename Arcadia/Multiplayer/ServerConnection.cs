using Discord;
using System;
using System.Threading.Tasks;

namespace Arcadia.Multiplayer
{
    public class ServerConnection
    {
        public static async Task<ServerConnection> CreateAsync(Player player, DisplayChannel display, ConnectionProperties properties = null)
        {
            properties ??= ConnectionProperties.Default;
            IDMChannel channel = await player.User.GetOrCreateDMChannelAsync();
            return new ServerConnection
            {
                Type = ConnectionType.Direct,
                RefreshCounter = 4,
                BlockInput = false,
                CouldDeleteMessages = properties.CanDeleteMessages,
                Channel = channel,
                InternalMessage = await channel.SendMessageAsync(channel.ToString()),
                ChannelId = channel.Id,
                Frequency = display.Frequency
            };
        }

        

        public ServerConnection() {}

        public ServerConnection(ulong guildId, IMessageChannel channel,
            IUserMessage messageBind,
            int frequency = 0,
            GameState state = GameState.Waiting,
            bool canDeleteMessages = false)
        {
            GuildId = guildId;
            Channel = channel;
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
            Channel = channel;
            InternalMessage = messageBind;
            ChannelId = channel.Id;
            MessageId = messageBind.Id;
            Frequency = frequency;
            State = state;
            CouldDeleteMessages = canDeleteMessages;
        }

        public ConnectionType Type { get; set; }

        // this allows you to group server connections together
        public string Group { get; set; }

        public ulong? GuildId { get; set; }

        public ulong ChannelId { get; set; }

        public ulong MessageId { get; set; }

        // If this is specified, attempt to re-initialize the channel in this manner
        public string DirectId { get; set; }

        // only 1 channel can be bound to a server at a time
        // check to see if the specified channel is connected to anything else.
        // likewise, you can simply create a cache of channels with their ID and server ID
        // where should i listen to input?
        public IMessageChannel Channel { get; set; }

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
        internal int RefreshCounter { get; set; }

        // Keeps track of how many messages appear in front of it.
        internal int CurrentMessageCounter { get; set; }

        // determines if the bot can attempt to delete messages
        public bool CouldDeleteMessages { get; set; } = false;

        // determines if the bot can 100% delete messages in this channel
        public bool CanDeleteMessages { get; set; } = false;

        // If true, inputs cannot be read in this connection
        public bool BlockInput { get; set; }

        // If specified, overrides the content received from the frequency
        public string ContentOverride { get; set; }

        // this determines what is currently being executed in the server connection
        public GameState State { get; set; }
    }
}
