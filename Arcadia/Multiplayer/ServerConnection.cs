using Discord;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orikivo;
using Format = Orikivo.Format;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents the origin of an <see cref="IConnection"/>
    /// </summary>
    public enum OriginType
    {
        /// <summary>
        /// Represents an unknown origin.
        /// </summary>
        Unknown = 0,

        Server = 1,

        // Specifies that the server connection originated from the GameSession
        Session = 2
    }

    // Create a SessionConnection, Which inherits IConnection, and can be easily modified for a GameSession
    // Have SessionConnection store their own properties
    // 

    public class ServerConnection
    {
        // private DisplayChannel Display { get; private set; }
        // public void SetDisplay(DisplayChannel display)
        // {
        //     Display = display;
        // }
        public static async Task<ServerConnection> CreateAsync(Player player, ConnectionProperties properties = null)
        {
            properties ??= ConnectionProperties.Default;
            IDMChannel channel = await player.User.GetOrCreateDMChannelAsync();
            Console.WriteLine($"[{Format.Time(DateTime.UtcNow)}] Creating connection with ID of {channel.Id} for {player.User.Id}");

            return new ServerConnection
            {
                RefreshRate = TimeSpan.FromSeconds(1),
                Type = ConnectionType.Direct,
                RefreshCounter = 4,
                BlockInput = false,
                UserId = player.User.Id,
                CouldDeleteMessages = properties.CanDeleteMessages,
                Channel = channel,
                InternalMessage = await channel.SendMessageAsync(Check.NotNull(properties.ContentOverride) ? properties.ContentOverride?.ToString() : "Unspecified content"),
                ChannelId = channel.Id,
                Frequency = properties.Frequency,
                LastRefreshed = DateTime.UtcNow,
                State = properties.State,
                ContentOverride = properties.ContentOverride,
                Inputs = properties.Inputs
            };
        }

        public static async Task<ServerConnection> CreateAsync(IMessageChannel channel, DisplayBroadcast display, ConnectionProperties properties = null)
        {
            properties ??= ConnectionProperties.Default;
            IUserMessage message = await channel.SendMessageAsync(Check.NotNull(properties.ContentOverride) ? properties.ContentOverride?.ToString() : display.ToString());

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
                Inputs = properties.Inputs,
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
            Inputs = new List<IInput>();
        }

        // STATIC, KEEP ONCE SET
        public DateTime CreatedAt { get; set; }

        // SPECIFIED ON INHERITED INITIALIZATION
        public ConnectionType Type { get; set; }

        // Specifies the origin of this ServerConnection
        internal OriginType Origin { get; set; }

        // CONFIG
        public string Group { get; set; }

        // INTERNAL REF
        public ulong? GuildId { get; set; }

        public ulong UserId { get; set; }
        // REMOVE, refer to Channel
        public ulong ChannelId { get; set; }

        // REMOVE, refer to InternalMessage
        public ulong MessageId { get; set; }

        // CONTAINER OF DISPLAY
        public IMessageChannel Channel { get; set; }

        // DISPLAY OF CONNECTION
        public IUserMessage InternalMessage { get; set; }

        // PUBLIC, DISPLAY POINTER
        public int Frequency { get; set; }

        // INTERNAL REF
        internal DateTime LastRefreshed { get; set; }

        // CONFIG
        public TimeSpan? RefreshRate { get; set; }

        // CONFIG, KEEP HIDDEN
        internal int RefreshCounter { get; set; }

        // KEEP HIDDEN
        internal int CurrentMessageCounter { get; set; }

        // MERGE WITH CanDeleteMessages
        public bool CouldDeleteMessages { get; set; }

        // CONFIG
        public bool CanDeleteMessages { get; set; } = false;

        // CONFIG
        public bool BlockInput { get; set; }

        // REMOVE THIS
        public DisplayContent ContentOverride { get; set; }

        // If this server connection has any specific inputs
        public List<IInput> Inputs { get; set; } = new List<IInput>();

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
            DisplayContent content = State == GameState.Playing ? server.GetBroadcast(Frequency)?.Content : server.GetBroadcast(State)?.Content;

            if (content == null)
                throw new Exception("Expected display channel but returned null");

            // If the time to refresh is less than the specified refresh rate, ignore
            if (DateTime.UtcNow - LastRefreshed < RefreshRate)
            {
                Console.WriteLine("Unable to refresh, refresh called too quickly");
                return;
            }

            await InternalMessage.DeleteAsync();
            InternalMessage = await Channel.SendMessageAsync(Check.NotNull(ContentOverride?.ToString()) ? ContentOverride?.ToString() : content.ToString());
            MessageId = InternalMessage.Id;
            LastRefreshed = DateTime.UtcNow;
            CurrentMessageCounter = 0;

            Console.WriteLine($"[{Orikivo.Format.Time(DateTime.UtcNow)}] Connection refresh was called");
        }
    }
}
