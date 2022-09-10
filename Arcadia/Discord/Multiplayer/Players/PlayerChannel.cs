using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Orikivo;
using Orikivo.Framework;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents a direct channel connection to a <see cref="Player"/>.
    /// </summary>
    public class PlayerChannel
    {
        public static async Task<PlayerChannel> CreateAsync(IUser user)
        {
            // TODO: Use correct method to get a DM channel
            IMessageChannel channel = await user.CreateDMChannelAsync();
            return new PlayerChannel(channel, user);
        }

        private PlayerChannel(IMessageChannel channel, IUser user)
        {
            Source = channel;
            Recipient = user;
        }

        public ulong Id => Source.Id;

        public IMessageChannel Source { get; }

        /// <summary>
        /// Represents the <see cref="IUser"/> that this <see cref="PlayerChannel"/> is bound to.
        /// </summary>
        public IUser Recipient { get; }

        /// <summary>
        /// Represents the previous message sent in this <see cref="PlayerChannel"/>.
        /// </summary>
        public IUserMessage PreviousMessage { get; private set; }

        /// <summary>
        /// If true, this <see cref="PlayerChannel"/> is disabled and cannot receive messages.
        /// </summary>
        public bool Disabled { get; private set; }

        /// <summary>
        /// Attempts to replace the previous message in this <see cref="PlayerChannel"/> with the specified text. Otherwise, a new message is sent.
        /// </summary>
        public async Task ReplaceAsync(string text)
        {
            if (Disabled)
                return;

            if (PreviousMessage != null)
                await PreviousMessage.ModifyAsync(text);
            else
                await SendAsync(text);
        }

        /// <summary>
        /// Attempts to remove the previous message in this <see cref="PlayerChannel"/>.
        /// </summary>
        public async Task RemovePreviousAsync()
        {
            if (Disabled)
                return;

            if (PreviousMessage != null)
            {
                await PreviousMessage.DeleteAsync();
                PreviousMessage = null;
            }
        }

        /// <summary>
        /// Attempts to send a message to this <see cref="PlayerChannel"/>.
        /// </summary>
        public async Task<IUserMessage> SendAsync(string text)
        {
            if (Disabled)
                return null;

            try
            {
                PreviousMessage = await Source.SendMessageAsync(text);
                return PreviousMessage;
            }
            catch (HttpException error) when (error.DiscordCode == DiscordErrorCode.CannotSendMessageToUser) // 50007
            {
                Logger.Debug($"Unable to send message to user {Recipient.Id} as their direct message channel is disabled");
                Disabled = true;
                return null;
            }
        }
    }
}
