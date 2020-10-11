using Discord.WebSocket;
using System.Threading.Tasks;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Orikivo
{
    /// <summary>
    /// Represents a generic message session handler.
    /// </summary>
    public abstract class MessageSession
    {
        /// <summary>
        /// Represents the method that is executed whenever this <see cref="MessageSession"/> is initialized.
        /// </summary>
        public virtual Task OnStartAsync()
            => Task.CompletedTask;

        /// <summary>
        /// Represents the method that is executed whenever this <see cref="MessageSession"/> receives a message.
        /// </summary>
        /// <returns>An <see cref="SessionTaskResult"/> that determines what the <see cref="MessageCollector"/> should proceed with.</returns>
        public abstract Task<SessionTaskResult> OnMessageReceivedAsync(SocketMessage message);

        /// <summary>
        /// Represents the method that is invoked whenever this <see cref="MessageSession"/> times out.
        /// </summary>
        public abstract Task OnTimeoutAsync(SocketMessage message);

        /// <summary>
        /// Represents the method that is executed whenever this <see cref="MessageSession"/> is cancelled.
        /// </summary>
        public virtual Task OnCancelAsync()
            => Task.CompletedTask;
    }
}
