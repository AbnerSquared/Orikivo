using Discord.WebSocket;
using System.Threading.Tasks;
using System;

namespace Orikivo
{
    public class HuskInitializer : MessageSession
    {
        public override Task OnStartAsync()
        {
            return base.OnStartAsync();
        }

        public override Task<SessionTaskResult> OnMessageReceivedAsync(SocketMessage message)
        {
            throw new NotImplementedException();
        }

        public override Task OnTimeoutAsync(SocketMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
