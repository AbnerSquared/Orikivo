using Discord;
using System;
using System.Threading.Tasks;
using Orikivo;

namespace Arcadia.Multiplayer
{
    public enum OriginType
    {
        Unknown = 0,

        // Specifies that the server connection originated from the GameServer
        Server = 1,

        // Specifies that the server connection originated from the GameSession
        Session = 2
    }

    public class ServerConnection
    {
        // private DisplayChannel Display { get; private set; }
        // public void SetDisplay(DisplayChannel display)
        // {
        //     Display = display;
        // }
        public static async Task<ServerConnection> CreateAsync(Player player, DisplayChannel display, ConnectionProperties properties = null)
        {
            properties ??= ConnectionProperties.Default;
            IDMChannel channel = await player.User.GetOrCreateDMChannelAsync();
            return new ServerConnection
            {
                RefreshRate = TimeSpan.FromSeconds(1),
                Type = ConnectionType.Direct,
                RefreshCounter = 4,
                BlockInput = false,
                CouldDeleteMessages = properties.CanDeleteMessages,
                Channel = channel,
                InternalMessage = await channel.SendMessageAsync(Check.NotNull(properties.ContentOverride) ? properties.ContentOverride : display.ToString()),
                ChannelId = channel.Id,
                Frequency = display.Frequency,
                LastRefreshed = DateTime.UtcNow,
                State = properties.State,
                ContentOverride = properties.ContentOverride
            };
        }

        public static async Task<ServerConnection> CreateAsync(IMessageChannel channel, DisplayChannel display, ConnectionProperties properties = null)
        {
            properties ??= ConnectionProperties.Default;
            IUserMessage message = await channel.SendMessageAsync(Check.NotNull(properties.ContentOverride) ? properties.ContentOverride : display.ToString());

            return new ServerConnection
            {
                RefreshRate = TimeSpan.FromSeconds(1),
                CreatedAt = DateTime.UtcNow,
                Type = ConnectionType.Unknown, // Handle ConnectionType.Guild
                Group = null,
                GuildId = null,
                ChannelId = channel.Id,
                MessageId = message.Id,
                Channel = channel,
                InternalMessage = message,
                Frequency = properties.Frequency,
                LastRefreshed = DateTime.UtcNow,
                RefreshCounter = properties.AutoRefreshCounter,
                CurrentMessageCounter = 0,
                CouldDeleteMessages = properties.CanDeleteMessages,
                CanDeleteMessages = false,
                BlockInput = properties.BlockInput,
                ContentOverride = properties.ContentOverride,
                State = properties.State
            };
        }

        

        public ServerConnection() {}

        public ServerConnection(IMessageChannel channel,
            IUserMessage messageBind,
            int frequency = 0,
            GameState state = GameState.Waiting,
            bool canDeleteMessages = false, ulong? guildId = null)
        {
            BlockInput = false;
            RefreshCounter = 4;
            Type = guildId.HasValue ? ConnectionType.Guild : ConnectionType.Unknown;
            Channel = channel;
            InternalMessage = messageBind;
            ChannelId = channel.Id;
            MessageId = messageBind.Id;
            GuildId = guildId;
            Frequency = frequency;
            State = state;
            CouldDeleteMessages = canDeleteMessages;
            LastRefreshed = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
        }

        public DateTime CreatedAt { get; set; }

        public ConnectionType Type { get; set; }

        // Specifies the origin of this ServerConnection
        internal OriginType Origin { get; set; }

        // this allows you to group server connections together
        public string Group { get; set; }

        public ulong? GuildId { get; set; }

        public ulong ChannelId { get; set; }

        public ulong MessageId { get; set; }

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
        // if the refresh rate is specified, if the time since the last refresh is shorter
        // don't refresh the console. likewise, you can create an async refresh that refreshes automatically
        // once the time to refresh has been met.
        internal DateTime LastRefreshed { get; set; }

        // The refresh rate of this connection
        // If update calls are going faster than that, wait until it can before updating again
        public TimeSpan? RefreshRate { get; set; }

        // this keeps track of how many messages were sent after the server message was sent.
        // if enough messages are sent after the lobby message and
        // can delete messages is false
        // a new message is sent in replacement
        internal int RefreshCounter { get; set; }

        // Keeps track of how many messages appear in front of it.
        internal int CurrentMessageCounter { get; set; }

        // determines if the bot can attempt to delete messages
        public bool CouldDeleteMessages { get; set; }

        // determines if the bot can 100% delete messages in this channel
        public bool CanDeleteMessages { get; set; } = false;

        // If true, inputs cannot be read in this connection
        public bool BlockInput { get; set; }

        // If specified, overrides the content received from the frequency
        public string ContentOverride { get; set; }

        // this determines what is currently being executed in the server connection
        public GameState State { get; set; }

        public async Task DestroyAsync()
        {
            if (Type == ConnectionType.Direct)
                await InternalMessage.DeleteAsync();
            else
            {
                if (CanDeleteMessages)
                    await InternalMessage.DeleteAsync();
            }
        }

        public async Task RefreshAsync(GameServer server)
        {
            DisplayContent content = State == GameState.Playing ? server.GetDisplayChannel(Frequency)?.Content : server.GetDisplayChannel(State)?.Content;

            if (content == null)
                throw new Exception("Expected display channel but returned null");

            // If the time to refresh is less than the specified refresh rate, ignore
            if (DateTime.UtcNow - LastRefreshed < RefreshRate)
            {
                Console.WriteLine("Unable to refresh, refresh called too quickly");
                return;
            }

            await InternalMessage.DeleteAsync();
            InternalMessage = await Channel.SendMessageAsync(Check.NotNull(ContentOverride) ? ContentOverride : content.ToString());
            MessageId = InternalMessage.Id;
            LastRefreshed = DateTime.UtcNow;
            CurrentMessageCounter = 0;

            Console.WriteLine($"[{Orikivo.Format.Time(DateTime.UtcNow)}] Connection refresh was called");
        }
    }
}
