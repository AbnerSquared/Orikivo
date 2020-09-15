using Discord.WebSocket;
using System.Threading.Tasks;
using System;

namespace Orikivo
{
    public class HuskInitializer : MatchSession
    {
        public override Task OnStartAsync()
        {
            return base.OnStartAsync();
        }

        public override Task<ActionResult> InvokeAsync(SocketMessage message)
        {
            throw new NotImplementedException();
        }

        public override Task OnTimeoutAsync(SocketMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
