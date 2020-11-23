using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Orikivo;

namespace Arcadia.Multiplayer
{
    public class ReceiverProperties
    {
        // refresh rate
        // refresh counter limit
        // can delete messages
    }

    /// <summary>
    /// Represents a generic connection to a <see cref="GameBroadcast"/>.
    /// </summary>
    public class Receiver
    {
        public Receiver(IMessageChannel channel)
        {
            Channel = channel;
        }

        private readonly Queue<MessageQueue> _pendingMessages = new Queue<MessageQueue>();

        public int Frequency { get; set; }

        protected IMessageChannel Channel { get; }

        private IUserMessage Message { get; set; }

        public bool CanReceiveMessages { get; set; } = true;

        public bool IsDMChannel => Channel is IDMChannel;

        // The refresh mechanism should be handled here.

        public async Task<bool> SendMessageAsync(string content, bool sendAsNew = false)
        {
            if (!CanReceiveMessages)
                return false;

            try
            {
                if (Message != null && !sendAsNew)
                {
                    await Message.ModifyAsync(content);
                }
                else
                {
                    Message = await Channel.SendMessageAsync(content);
                }

                return true;
            }
            catch (HttpException error) when (error.DiscordCode == 50007)
            {
                //CanReceiveMessages = false;
                return false;
            }
        }

        internal async Task<bool> RetryAsync()
        {
            if (!CanReceiveMessages)
                return false;

            try
            {
                while (_pendingMessages.Count > 0)
                {
                    MessageQueue message = _pendingMessages.Peek();
                    await SendMessageAsync(message.Content, message.SendAsNew);
                }

                return true;
            }
            catch (HttpException error) when (error.DiscordCode == 50007)
            {
                return false;
            }
        }
    }
}
