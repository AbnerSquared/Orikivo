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

        public TEntity GetOrAdd(TEntity entity)
        {
            TEntity result = default;

            if (entity == null)
                return result;

            if (!Values.ContainsKey(entity.Id))
                Values.AddOrUpdate(entity.Id, entity, (key, value) => value);

            Values.TryGetValue(entity.Id, out result);

            return result;
        }

        public void AddOrUpdate(ulong id, TEntity entity)
        {
            if (id != entity.Id)
                throw new ArgumentException("The specified ID does not match to the specified value");

            Values.AddOrUpdate(id, entity, (key, value) => value);
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
