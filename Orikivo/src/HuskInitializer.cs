using Discord.WebSocket;
using System.Threading.Tasks;
using System;
using Discord.Addons.Collectors;

namespace Orikivo
{
    public class HuskInitializer : MessageSession
    {
        public override Task OnStartAsync()
        {
            return base.OnStartAsync();
        }

        public override Task<SessionResult> OnMessageReceivedAsync(SocketMessage message)
        {
            throw new NotImplementedException();
        }

        public override Task OnTimeoutAsync(SocketMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
