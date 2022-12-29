using Discord;
using System.Threading.Tasks;
using Discord.Net;
using Orikivo;

namespace Arcadia.Multiplayer
{
    public class ReceiverOLD //: IReceiver
    {
        public IMessageChannel Channel { get; protected set; }

        public IUserMessage Message { get; private set; }

        public bool Enabled { get; internal set; } = true;

        public bool CanSendMessages { get; protected set; } = true;

        public async Task<IUserMessage> UpdateAsync(string content, bool replacePrevious = false)
        {
            if (!CanSendMessages)
                return null;

            try
            {
                if (Message != null && !replacePrevious)
                {
                    await Message.ModifyAsync(content);
                }
                else
                {
                    Message = await Channel.SendMessageAsync(content);
                }

                return Message;
            }
            catch (HttpException error) when (error.DiscordCode == DiscordErrorCode.CannotSendMessageToUser)
            {
                CanSendMessages = false;
                return null;
            }
        }
    }
}
