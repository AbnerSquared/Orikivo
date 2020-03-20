using Discord.WebSocket;
using System;

namespace Orikivo
{
    /// <summary>
    /// Represents a filtered result of a <see cref="SocketMessage"/> from a <see cref="MessageCollector"/>.
    /// </summary>
    public class FilterMatch
    {
        internal FilterMatch(SocketMessage message, int index, bool isSuccess)
        {
            Message = message;
            Index = index;
            IsSuccess = isSuccess;
        }

        /// <summary>
        /// Represents the index at which this message was filtered (zero-based).
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Gets the message that was received from the <see cref="MessageCollector"/>.
        /// </summary>
        public SocketMessage Message { get; }

        /// <summary>
        /// Defines if the <see cref="FilterMatch"/> was a success.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Converts the <see cref="FilterMatch"/> into the specified enclosing <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="TValue">The enclosing <see cref="Type"/> that the <see cref="FilterMatch"/> will convert to.</typeparam>
        /// <param name="converter">The method used to convert the <see cref="FilterMatch"/>.</param>
        public TValue Convert<TValue>(Func<FilterMatch, TValue> converter)
        {
            return converter.Invoke(this);
        }
    }
}
