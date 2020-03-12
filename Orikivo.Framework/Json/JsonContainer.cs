using System;
using System.Collections.Concurrent;

namespace Orikivo
{
    /// <summary>
    /// Represents a default <see cref="IJsonContainer{TKey, TObject}"/>.
    /// </summary>
    public class JsonContainer<TKey, TObject> : IJsonContainer<TKey, TObject>
    {
        public void Restore(string directory)
        {
            throw new NotImplementedException();
        }

        public ConcurrentDictionary<TKey, TObject> Directory { get; set; }

        public TObject GetOrAdd(TObject @object)
        {
            throw new NotImplementedException();
        }

        public bool TryGet(TKey key, out TObject @object)
        {
            throw new NotImplementedException();
        }

        public void AddOrUpdate(TObject @object)
        {
            throw new NotImplementedException();
        }

        public void Save(TObject @object)
        {
            throw new NotImplementedException();
        }

        public void SaveAll()
        {
            throw new NotImplementedException();
        }
    }
}
