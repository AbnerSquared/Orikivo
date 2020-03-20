using Discord.WebSocket;

namespace Orikivo
{
    /// <summary>
    /// Represents a filter for a <see cref="MessageCollector"/> to determine the messages that it should read.
    /// </summary>
    public abstract class MessageFilter
    {
        /// <summary>
        /// Judges if the message received should be read.
        /// </summary>
        public abstract bool Judge(SocketMessage message, int index);

        /// <summary>
        /// Judges if the message received should be read based on previous matches.
        /// </summary>
        public virtual bool JudgeMany(SocketMessage message, FilterCollection matches, int index)
            => Judge(message, index);
    }
}
