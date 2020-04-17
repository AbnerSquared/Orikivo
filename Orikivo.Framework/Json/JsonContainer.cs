using System;
using System.Collections.Concurrent;

namespace Orikivo.Framework.Json
{
    /// <summary>
    /// Represents a default <see cref="IJsonContainer{TKey, TObject}"/>.
    /// </summary>
    public class JsonContainer<TKey, TValue> : IJsonContainer<TKey, TValue>
    {
        public JsonContainer(string directoryPath,
            StringConverter<TKey> keyConverter,
            Func<TKey, TValue> defaultValueBuilder,
            Func<TValue, TKey> keySelector)
        {
            DirectoryPath = directoryPath;
            KeyConverter = keyConverter;
            DefaultValueBuilder = defaultValueBuilder;
            KeySelector = keySelector;
            Source = JsonHandler.DeserializeDirectory<TKey, TValue>(DirectoryPath, KeyConverter);
        }

        public string DirectoryPath { get; }

        public ConcurrentDictionary<TKey, TValue> Source { get; private set; }

        public StringConverter<TKey> KeyConverter { get; }
        
        public Func<TValue, TKey> KeySelector { get; }

        public Func<TKey, TValue> DefaultValueBuilder { get; }

        public void Reload()
        {
            Source = JsonHandler.DeserializeDirectory<TKey, TValue>(DirectoryPath, KeyConverter);
        }

        public TValue GetOrAdd(TKey key)
        {
            TValue value;

            if (!Source.ContainsKey(key))
            {
                value = DefaultValueBuilder.Invoke(key);
                Source.AddOrUpdate(key, value, (k, v) => value);
            }

            Source.TryGetValue(key, out value);
            return value;
        }

        public bool TryGet(TKey key, out TValue value)
            => Source.TryGetValue(key, out value);

        public void AddOrUpdate(TKey key, TValue value)
            => Source.AddOrUpdate(key, value, (k, v) => value);

        public void Save(TKey key)
        {
            if (!Source.ContainsKey(key))
                throw new ArgumentException("The specified key does not exist within the source dictionary.");

            JsonHandler.Save(Source[key], KeyConverter.Serializer.Invoke(key));
        }

        public void SaveAll()
        {
            foreach ((TKey key, TValue value) in Source)
            {
                JsonHandler.Save(value, KeyConverter.Serializer.Invoke(key));
            }
        }
    }
}
