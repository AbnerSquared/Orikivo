using System.Collections.Generic;
using System.Collections.Immutable;

namespace Orikivo
{
    // THIS CLASS WAS WRITTEN SINCE DISCORD'S HANDLER CLASS WAS INTERNAL.
    // https://github.com/discord-net/Discord.Net/blob/dev/src/Discord.Net.Core/Utils/AsyncEvent.cs

    /// <summary>
    /// Represents an asynchronous event handler.
    /// </summary>
    public class AsyncEvent<T> where T : class
    {
        private readonly object _subLock = new object();
        public ImmutableArray<T> _subscriptions;

        /// <summary>
        /// Gets a <see cref="bool"/> that determines if the <see cref="AsyncEvent{T}"/> contains any subscribers.
        /// </summary>
        public bool HasSubscribers => _subscriptions.Length != 0;

        /// <summary>
        /// Gets an <see cref="IReadOnlyList{T}"/> that contains all of the subscribers for this <see cref="AsyncEvent{T}"/>.
        /// </summary>
        public IReadOnlyList<T> Subscriptions => _subscriptions;

        /// <summary>
        /// Initializes a new <see cref="AsyncEvent{T}"/>.
        /// </summary>
        public AsyncEvent()
        {
            _subscriptions = ImmutableArray.Create<T>();
        }

        /// <summary>
        /// Subscribes the specified value to the <see cref="AsyncEvent{T}"/>.
        /// </summary>
        /// <param name="subscriber">The value that will be subscribed to the <see cref="AsyncEvent{T}"/>.</param>
        public void Add(T subscriber)
        {
            Catch.NotNull(subscriber, nameof(subscriber));
            lock (_subLock)
                _subscriptions = _subscriptions.Add(subscriber);
        }

        /// <summary>
        /// Unsubscribes the specified value from the <see cref="AsyncEvent{T}"/>.
        /// </summary>
        /// <param name="subscriber">The value that will be unsubscribed from the <see cref="AsyncEvent{T}"/>.</param>
        public void Remove(T subscriber)
        {
            Catch.NotNull(subscriber, nameof(subscriber));
            lock (_subLock)
                _subscriptions = _subscriptions.Remove(subscriber);
        }

        /// <summary>
        /// Removes all existing subscriptions from the <see cref="AsyncEvent{T}"/>.
        /// </summary>
        public void Clear()
        {
            lock (_subLock)
                _subscriptions = _subscriptions.Clear();
        }
    }
}
