using Discord;
using Discord.Addons.Collectors;
using Discord.WebSocket;

namespace Arcadia
{
    public abstract class BaseSession : MessageSession
    {
        protected BaseSession(ArcadeUser invoker, ISocketMessageChannel channel)
        {
            Invoker = invoker;
            Channel = channel;
        }

        public ArcadeUser Invoker { get; }

        public ISocketMessageChannel Channel { get; }

        protected IUserMessage Reference { get; set; }
    }
}
