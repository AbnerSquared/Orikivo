using System.Collections.Concurrent;

namespace Orikivo
{
    /// <summary>
    /// Represents a generic JSON container.
    /// </summary>
    /// <typeparam name="TKey">The key that points to a corresponding object.</typeparam>
    /// <typeparam name="TValue">The object that the JSON container will revolve around.</typeparam>
    public interface IJsonContainer<TKey, TValue>
    {
        string DirectoryPath { get; }
        ConcurrentDictionary<TKey, TValue> Source { get; }
        void Reload();
        TValue GetOrAdd(TKey key);
        bool TryGet(TKey key, out TValue value);
        void AddOrUpdate(TKey key, TValue value);
        void Save(TKey key);
        void SaveAll();
    }
}
