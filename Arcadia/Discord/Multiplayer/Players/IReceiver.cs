using Discord;
using Discord.Net;
using Orikivo;
using System.Threading.Tasks;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents a generic receiver to a broadcast.
    /// </summary>
    public interface IReceiver
    {

        // Until I figure out an efficient way to reference broadcasts
        int Frequency { get; }
      
        Task UpdateAsync();

        Task DisposeAsync();
    }

    public class Receiver : IReceiver
    {
        private GameServer _server;
        private bool _canSendMessages = true;
        private bool _replacePrevious = false;

        public int Frequency { get; internal set; }

        protected IMessageChannel Channel { get; set; }

        protected IUserMessage Message { get; set; }

        public async Task UpdateAsync()
        {
            if (!_canSendMessages)
                return;

            DisplayBroadcast broadcast = _server.GetBroadcast(Frequency);
            string content = broadcast.Content.ToString();

            try
            {
                if (Message != null && !_replacePrevious)
                {
                    await Message.ModifyAsync(content);
                }
                else
                {
                    Message = await Channel.SendMessageAsync(content);
                }
            }
            catch (HttpException error) when (error.DiscordCode == DiscordErrorCode.CannotSendMessageToUser)
            {
                _canSendMessages = false;
            }
        }

        public async Task DisposeAsync() { }
    }

    public class Broadcast
    {

    }
}
