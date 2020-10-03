using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Orikivo;

namespace Arcadia.Casino
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
