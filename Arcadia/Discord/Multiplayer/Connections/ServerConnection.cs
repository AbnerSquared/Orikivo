using Discord;
using Orikivo;
using Orikivo.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arcadia.Multiplayer
{
    public class ServerConnection
    {
        public static async Task<ServerConnection> CreateAsync(Player player, ConnectionProperties properties = null)
        {
            properties ??= ConnectionProperties.Default;
            PlayerChannel channel = await player.GetOrCreateChannelAsync();
            Logger.Debug($"Creating connection with ID of {channel.Id} for {player.User.Id}");
            IUserMessage message = await channel.SendAsync(Check.NotNull(properties.ContentOverride) ? properties.ContentOverride?.ToString() : $"> ⚠️ Could not find a channel at the specified frequency ({properties.Frequency}).");

            if (message == null)
                throw new Exception("This user is unable to receive messages");

            return new ServerConnection
            {
                Server = player.Server,
                RefreshRate = TimeSpan.FromSeconds(1),
                Type = ConnectionType.Direct,
                RefreshCounter = 4,
                BlockInput = false,
                UserId = player.User.Id,
                DeleteMessages = properties.CanDeleteMessages,
                Channel = channel.Source,
                Message = message,
                ChannelId = channel.Id,
                Frequency = properties.Frequency,
                LastRefreshed = DateTime.UtcNow,
                State = properties.State,
                ContentOverride = properties.ContentOverride,
                Inputs = properties.Inputs,
                Origin = properties.Origin
            };
        }

        public static async Task<ServerConnection> CreateAsync(IMessageChannel channel, GameServer server, ConnectionProperties properties = null)
        {
            properties ??= ConnectionProperties.Default;
            IUserMessage message = await channel.SendMessageAsync(server.GetBroadcast(properties.Frequency)?.ToString() ?? $"> ⚠️ Could not find a channel at the specified frequency ({properties.Frequency}).");

            return new ServerConnection
            {
                Server = server,
                Frequency = properties.Frequency,
                State = properties.State,
                RefreshRate = properties.RefreshRate,
                RefreshCounter = properties.AutoRefreshCounter,
                BlockInput = properties.BlockInput,
                ContentOverride = properties.ContentOverride,
                Inputs = properties.Inputs,
                Channel = channel,
                ChannelId = channel.Id,
                MessageId = message.Id,
                Message = message,
                DeleteMessages = properties.CanDeleteMessages,
                LastRefreshed = DateTime.UtcNow
            };
        }

        private ServerConnection()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public GameServer Server { get; internal set; }

        public DateTime CreatedAt { get; }

        public ConnectionType Type { get; set; }

        internal OriginType Origin { get; set; }

        public string Group { get; set; }

        public ulong? GuildId { get; set; }

        public ulong UserId { get; set; }
        public ulong ChannelId { get; set; }

        public ulong MessageId { get; set; }

        public IMessageChannel Channel { get; private set; }

        internal IUserMessage Message { get; set; }

        public int Frequency { get; set; }

        internal DateTime LastRefreshed { get; private set; }

        public TimeSpan? RefreshRate { get; set; }

        internal int RefreshCounter { get; set; }

        internal int CurrentMessageCounter { get; set; }

        public bool DeleteOnRefresh { get; set; } = true;

        public bool DeleteMessages { get; internal set; }

        public bool BlockInput { get; set; }

        public DisplayContent ContentOverride { get; set; }

        public List<IInput> Inputs { get; set; } = new List<IInput>();

        public GameState State { get; set; }

        public bool Destroyed { get; internal set; }

        // If true, this cannot update
        public bool Paused { get; set; } = false;

        internal bool IsDeleted { get; set; }

        private string Content { get; set; }

        public List<IInput> GetAvailableInputs()
        {
            DisplayBroadcast broadcast = State == GameState.Playing
                ? Server.GetBroadcast(Frequency)
                : Server.GetBroadcast(State);

            var inputs = new List<IInput>(Inputs);

            if (broadcast != null)
                inputs.AddRange(broadcast.Inputs);

            return inputs;
        }

        public async Task DestroyAsync()
        {
            if (Destroyed)
                throw new Exception("This connection was destroyed");

            if (Server.HasConnection(ChannelId))
                await Server.RemoveConnectionAsync(ChannelId);

            if (DeleteMessages)
                await Message.TryDeleteAsync();
        }

        public DisplayBroadcast GetBroadcast()
        {
            return State switch
            {
                GameState.Watching => Server?.GetSpectatorBroadcast(),
                GameState.Playing => Server?.GetBroadcast(Frequency),
                _ => Server?.GetBroadcast(State)
            };
        }

        public async Task RefreshAsync(bool replacePrevious = false)
        {
            if (Destroyed)
                throw new Exception("This connection was destroyed");

            if (Paused)
            {
                Logger.Debug("Connection paused");
                return;
            }

            DisplayContent content =
                State switch
                {
                    GameState.Watching => Server?.GetSpectatorBroadcast()?.Content,
                    GameState.Playing => Server?.GetBroadcast(Frequency)?.Content,
                    _ => Server?.GetBroadcast(State)?.Content
                };

            string text = Check.NotNull(ContentOverride?.ToString())
                ? ContentOverride?.ToString()
                : content?.ToString() ?? $"> ⚠️ Could not find a channel at the specified frequency ({Frequency}).";

            if (Message == null || IsDeleted || replacePrevious)
            {
                Message = await Channel.SendMessageAsync(text);
                MessageId = Message.Id;
                LastRefreshed = DateTime.UtcNow;
                IsDeleted = false;

                Logger.Debug("Created replacement message");
                return;
            }

            // If the time to refresh is less than the specified refresh rate, ignore
            if (DateTime.UtcNow - LastRefreshed < RefreshRate)
            {
                Logger.Debug("Refresh called too quickly");
                Content = text; // Store/preserve the existing content;
                return;
            }

            if (RefreshCounter > 0 && CurrentMessageCounter >= RefreshCounter)
            {
                if (State == GameState.Watching)
                {
                    string panel = $"> **{Server?.Name ?? "Unknown Server"}**\n> You are currently spectating the active session.\n{(Check.NotNull(text) ? text : "Unable to load the spectator panel.")}";
                    text = panel;
                }

                Message = await Message.ReplaceAsync(text, deleteLastMessage: DeleteOnRefresh);
                MessageId = Message.Id;
                LastRefreshed = DateTime.UtcNow;
                CurrentMessageCounter = 0;
                Logger.Debug("Replace message success");
                return;
            }

            // Don't update if the content is already exactly the same
            if (Message.Content == text)
            {
                Logger.Debug("Duplicate text specified");
                return;
            }

            await Message.ModifyAsync(text);
            LastRefreshed = DateTime.UtcNow;

            Logger.Debug("Connection refresh was called");
        }
    }
}
