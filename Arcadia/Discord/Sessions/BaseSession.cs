using System.Threading.Tasks;
using Discord;
using Orikivo;

namespace Arcadia.Casino
{
    public abstract class BaseSession : MessageSession
    {
        protected BaseSession(ArcadeContext context)
        {
            Context = context;
        }

        public ArcadeContext Context { get; }

        public ArcadeUser User => Context.Account;

        protected IUserMessage Reference { get; set; }
    }
}
