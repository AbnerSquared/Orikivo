using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Orikivo
{
    // a subscriber to the game that reads off of the game's display.
    public class GameReceiver
    {
        private TextChannelProperties _properties;

        public GameReceiver(SocketGuild guild, GameReceiverConfig config)
        {
            config = config ?? GameReceiverConfig.Default; // ??=
            Id = guild.Id;
            Name = config.Name;
            _properties = config?.Properties;
            Channel = CreateChannelAsync(guild).Result;//.GetAwaiter().GetResult();
            CanDeleteMessages = guild.CurrentUser.GuildPermissions.Has(GuildPermission.ManageMessages);
            // if you can delete messages, go off of user preference; otherwise, send a new msg each time.
            CanUpdateMessage = CanDeleteMessages ? config.CanUpdateMessage : false;
            State = GameState.Inactive;
        }

        public ulong Id { get; } // this is the guild id
        public string Name { get; } // the name used for the receiver.
        public bool CanDeleteMessages { get; } // if the receiver allows for deleting messages.
        public bool CanUpdateMessage { get; } // if the receiver can update the existing message.
        public GameState State { get; internal set; }
        private RestTextChannel Channel { get; set; }
        private RestUserMessage Message { get; set; }
        public ulong? ChannelId => Channel?.Id;
        public ulong? MessageId => Message?.Id;
        public string Mention => Channel?.Mention;
        public string SyncKey { get; private set; }

        private async Task<RestTextChannel> CreateChannelAsync(BaseSocketClient client)
            => await CreateChannelAsync(client.GetGuild(Id));
        private async Task<RestTextChannel> CreateChannelAsync(SocketGuild guild)
        {
            if (guild == null)
                throw new Exception("The original guild used for this receiver is empty.");
            if (!guild.CurrentUser.GuildPermissions.Has(GuildPermission.ManageChannels))
                // $"Orikivo is missing permissions to perform action '{Type.ToString()}' at {GuildId}: {Permission.ToString()}";
                // GuildPermission.ManageChannels, guild.Id, ActionType.ChannelCreated
                throw new Exception("The bot is missing permissions for MANAGE_CHANNELS.");
            // Channel.ModifyAsync(x => { x.SlowModeInterval = _properties.SlowModeInterval; }).ConfigureAwait(false);
            return await guild.CreateTextChannelAsync(Name, x => { x = _properties; });//.ConfigureAwait(false); // .ConfigureAwait(false);
        }

        internal async Task UpdateStateAsync(GameState state)
        {

        }

        internal async Task UpdateAsync(BaseSocketClient client, GameDisplay display)
        {
            //await Channel?.UpdateAsync();
            if (Channel == null)
                Channel = await CreateChannelAsync(client);//.ConfigureAwait(false);

            if (Message == null || !CanUpdateMessage)
                Message = await Channel.SendMessageAsync(display[State].Content);//.ConfigureAwait(false);
            else
            {
                if (SyncKey == display[State].SyncKey)
                    return;

                await Message.ModifyAsync(x => x.Content = display[State].Content);
            }

            SyncKey = display[State].SyncKey;
        }

        // cleans the receiver
        public async Task CloseAsync(string reason = null, TimeSpan? delay = null)
        {
            await Channel?.UpdateAsync();
            if (Channel == null)
                return;
            if (Check.NotNull(reason))
                await Channel.SendMessageAsync(reason ?? "An unknown reason was given.");
            if (delay.HasValue)
                await Task.Delay(delay.Value);
            await Channel.DeleteAsync();
        }
    }
}
