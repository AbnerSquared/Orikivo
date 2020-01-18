using Discord.WebSocket;
using System.Threading.Tasks;

namespace Orikivo
{
    public abstract class MatchAction
    {
        public virtual async Task OnStartAsync() { }
        public abstract Task<ActionResult> InvokeAsync(SocketMessage message);
        public abstract Task OnTimeoutAsync(SocketMessage message);

        public virtual async Task OnCancelAsync() { }
    }
}
