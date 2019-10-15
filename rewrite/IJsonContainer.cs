using System.Collections.Concurrent;

namespace Orikivo
{
    /// <summary>
    /// Represents a generic JSON container.
    /// </summary>
    /// <typeparam name="TKey">The object that the JSON container will revolve around.</typeparam>
    /// <typeparam name="TObject">The object that the JSON container will revolve around.</typeparam>
    public interface IJsonContainer<TKey, TObject>
    {
        void Restore(string directory);
        ConcurrentDictionary<TKey, TObject> Directory { get; }
        TObject GetOrAdd(TObject @object);
        bool TryGet(TKey key, out TObject @object);
        void AddOrUpdate(TObject @object);
        void Save(TObject @object);
        void SaveAll();
    }
}
