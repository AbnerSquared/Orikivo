using System.Collections.Generic;
using System.Collections.Immutable;

namespace Orikivo
{
    /* Referenced from Discord.Net.Utils (Unable to use their version of the class, as it is hidden from public view) */

    internal class AsyncEvent<T> where T : class
    {
        private readonly object _subLock = new object();
        internal ImmutableArray<T> _subscriptions;
        internal bool HasSubscribers => _subscriptions.Length != 0;
        internal IReadOnlyList<T> Subscriptions => _subscriptions;

        internal AsyncEvent()
        {
            _subscriptions = ImmutableArray.Create<T>();
        }

        internal void Add(T subscriber)
        {
            Catch.NotNull(subscriber, nameof(subscriber));
            lock (_subLock)
                _subscriptions = _subscriptions.Add(subscriber);
        }

        internal void Remove(T subscriber)
        {
            Catch.NotNull(subscriber, nameof(subscriber));
            lock (_subLock)
                _subscriptions = _subscriptions.Remove(subscriber);
        }
    }
}
