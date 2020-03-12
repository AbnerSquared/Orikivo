using Discord.WebSocket;
using System.Threading.Tasks;

namespace Orikivo
{
    /// <summary>
    /// Represents the barebones structure of a handler for a <see cref="FilterMatch"/>.
    /// </summary>
    public abstract class MatchAction
    {
        /// <summary>
        /// Represents the method that is invoked whenever a <see cref="MessageCollector"/> starts filtering messages.
        /// </summary>
        public virtual async Task OnStartAsync() { }
        /// <summary>
        /// Represents the method that is invoked whenever a <see cref="MessageCollector"/> receives a successful <see cref="FilterMatch"/>.
        /// </summary>
        public abstract Task<ActionResult> InvokeAsync(SocketMessage message);

        /// <summary>
        /// Represents the method that is invoked whenever a <see cref="MessageCollector"/> runs out of time.
        /// </summary>
        public abstract Task OnTimeoutAsync(SocketMessage message);

        /// <summary>
        /// Represents the method that is invoked whenever a <see cref="MessageCollector"/> is cancelled.
        /// </summary>
        /// <returns></returns>
        public virtual async Task OnCancelAsync() { }
    }
}
