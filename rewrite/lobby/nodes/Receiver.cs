using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Orikivo
{
    public class Receiver
    {
        public Receiver(SocketGuild guild, ReceiverConfig config)
        {
            Id = guild.Id;
            Name = config.Name;
            ChannelProperties = config.Properties;
            ModifySource = config.ModifySource;

            if (!guild.CurrentUser.GuildPermissions.Has(GuildPermission.ManageChannels))
                throw new MissingGuildPermissionsException(GuildPermission.ManageChannels, guild.Id, ActionType.ChannelCreated);

            Console.WriteLine($"[Debug] -- Client has access. Now linking a receiver to {Id}. --");
            Channel = guild.CreateTextChannelAsync(Name, x => { x.Topic = ChannelProperties.Topic; }).Result;
            // figure out why slow mode cant be set on the creation of a new text channel.
            Channel.ModifyAsync(x => { x.SlowModeInterval = ChannelProperties.SlowModeInterval; }).ConfigureAwait(false);
            DeleteMessages = guild.CurrentUser.GuildPermissions.Has(GuildPermission.ManageMessages);
        }
        public bool DeleteMessages { get; } // if the client can delete sent messages 
        public bool ModifySource { get; } // if the message is modified, or if a new message is sent upon each update.
        
        public ulong Id { get; } // the guild in which the channel is built in.
        public string Name { get; } // the name used to build the receiver.
        public string Mention { get { return Channel?.Mention; } }
        public ulong? ChannelId { get { return Channel?.Id; } }
        public ulong? MessageId { get { return Message?.Id; } }
        
        private TextChannelProperties ChannelProperties { get; } // the properties used on build, in case of
        // the message being deleted, or the node is manually updated.
        private RestTextChannel Channel { get; set; } // the location of the message source.
        private RestUserMessage Message { get; set; } // the message source used to display nodes.

        // instead of key comparing, why not use a SyncPromise, which is a taskcompletionsource
        public string SyncKey { get; private set; } // the key used to synchronize receivers.

        // maybe limit updates to stay under the ratelimit?
        // 5 / 5sec

        // allow 
        // private RestVoiceChannel Voice {get; set; } // the voice chat used, in the case of the game that is to be played requires it.

        public async Task UpdateAsync(BaseSocketClient rootClient, Display display)
        {
            // a quick test to see if the channel exists
            await Channel.UpdateAsync();
            if (Channel == null)
            {
                Console.WriteLine("[Debug] -- Channel source is missing. --");
                SocketGuild guild = rootClient.GetGuild(Id);
                if (guild == null)
                    throw new Exception("The guild that was originally used for this receiver is now missing.");

                if (guild.CurrentUser.GuildPermissions.Has(GuildPermission.ManageChannels))
                    throw new MissingGuildPermissionsException(GuildPermission.ManageChannels, Id, ActionType.ChannelCreated);

                Channel = guild.CreateTextChannelAsync(Name, x => { x = ChannelProperties; }).Result;
                Console.WriteLine("[Debug] -- Channel restored. --");
            }

            if (ModifySource)
            {
                if (Message == null)
                {
                    Console.WriteLine("[Debug] -- Message source is missing. --");
                    Message = Channel.SendMessageAsync(display.ForReceiver(Id)).Result;
                    Console.WriteLine("[Debug] -- Message restored. --");
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(SyncKey))
                    {
                        if (SyncKey == display.SyncKey)
                        {
                            Console.WriteLine("[Debug] -- Receiver is already synchronized. --");
                            return;
                        }
                    }

                    await Message.ModifyAsync(x => x.Content = display.ForReceiver(Id));
                }
            }
            else
            {
                Console.WriteLine("[Debug] -- Sending new message update. --");
                Message = Channel.SendMessageAsync(display.ForReceiver(Id)).Result;
            }

            SyncKey = display.SyncKey;
        }

        // this closes the receiver.
        public async Task CloseAsync(string message = null, TimeSpan? delay = null)
        {
            if (!string.IsNullOrWhiteSpace(message))
                await Channel.SendMessageAsync(message);

            if (delay.HasValue)
                await Task.Delay(delay.Value);
            await Channel.DeleteAsync();
        }
    }
}
