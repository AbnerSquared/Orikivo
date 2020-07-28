using Orikivo.Framework;
using System;
using System.Collections.Concurrent;

namespace Orikivo
{
    public class JsonContainer<TEntity>
        where TEntity : IJsonEntity
    {
        private readonly string _directory;

        public ConcurrentDictionary<ulong, TEntity> Values { get; }

        public int Count => Values.Count;

        public JsonContainer(string directory)
        {
            _directory = directory;
            Values = JsonHandler.RestoreContainer<TEntity>(_directory);
            Logger.Debug($"-- Restored {Values.Count} {Format.TryPluralize("entity", Values.Count)}. --");
        }

        public TEntity GetOrAdd(TEntity value)
        {
            TEntity result = default(TEntity);

            if (value == null)
                return result;

            if (!Values.ContainsKey(value.Id))
                Values.AddOrUpdate(value.Id, value, (key, value) => value);

            Values.TryGetValue(value.Id, out result);

            return result;
        }

        public void AddOrUpdate(ulong id, TEntity value)
        {
            if (id != value.Id)
                throw new ArgumentException("The specified ID does not match to the specified value");

            Values.AddOrUpdate(id, value, (key, value) => value);
        }

        public bool TryGet(ulong id, out TEntity entity)
            => Values.TryGetValue(id, out entity);

        public void Save(TEntity value)
        {
            if (value != null)
                JsonHandler.SaveEntity(value, _directory);
        }

        public void TrySave(TEntity value)
        {
            if (value != null)
                Save(value);
        }

        public void SaveAll()
        {
            foreach (TEntity value in Values.Values)
                Save(value);
        }
    }
}
