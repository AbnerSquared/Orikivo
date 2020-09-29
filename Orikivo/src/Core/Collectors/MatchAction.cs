using Discord.WebSocket;
using System.Threading.Tasks;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Orikivo
{
    /// <summary>
    /// Represents a generic message session handler for a <see cref="FilterMatch"/>.
    /// </summary>
    public abstract class MatchSession
    {
        /// <summary>
        /// Represents the method that is executed whenever a <see cref="MessageCollector"/> starts filtering messages.
        /// </summary>
        public virtual async Task OnStartAsync() { }

        /// <summary>
        /// Represents the method that is executed whenever a <see cref="MessageCollector"/> receives a successful <see cref="FilterMatch"/>.
        /// </summary>
        /// <returns>An <see cref="MatchResult"/> that determines what the <see cref="MessageCollector"/> should proceed with.</returns>
        public abstract Task<MatchResult> InvokeAsync(SocketMessage message);

        /// <summary>
        /// Represents the method that is invoked whenever a <see cref="MessageCollector"/> runs out of time.
        /// </summary>
        public abstract Task OnTimeoutAsync(SocketMessage message);

        /// <summary>
        /// Represents the method that is executed whenever a <see cref="MessageCollector"/> is cancelled.
        /// </summary>
        public virtual async Task OnCancelAsync() { }
    }
}
