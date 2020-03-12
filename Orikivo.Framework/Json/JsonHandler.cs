﻿using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Orikivo
{
    /// <summary>
    /// Represents a handler for JSON serialization and file management.
    /// </summary>
    public static class JsonHandler
    {
        private const string DEFAULT_DIRECTORY = @"..\data\";
        internal static readonly string JsonFrame = "{0}.json";
        internal static readonly string GlobalFileName = "global";
        internal static string BaseDirectory = DEFAULT_DIRECTORY;

        public static JsonSerializerSettings DefaultSerializerSettings
            => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                Formatting = Formatting.Indented
            };

        public static void SetDirectory(string directory)
            => BaseDirectory = directory;

        public static void ResetDirectory()
            => BaseDirectory = DEFAULT_DIRECTORY;

        public static void SaveJsonEntity<T>(T obj, JsonSerializer serializer = null)
            where T : IJsonEntity
            => Save(obj, string.Format(JsonFrame, obj.Id), serializer);

        /// <summary>
        /// Saves an object to a specified local path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object to serialize as JSON.</param>
        /// <param name="path">The local path to save to.</param>
        /// <param name="serializer"></param>
        public static void Save<T>(T obj, string path, JsonSerializer serializer = null)
        {
            // path = GetDirectory() + path;
            using (StreamWriter stream = File.CreateText(path))
            {
                using (JsonWriter writer = new JsonTextWriter(stream))
                {
                    serializer ??= JsonSerializer.Create(DefaultSerializerSettings);
                    serializer.Serialize(stream, obj);
                }
            }
        }

        public static T LoadJsonEntity<T>(ulong id, JsonSerializer serializer = null) where T : IJsonEntity
            => Load<T>(GetDirectory() + string.Format(JsonFrame, id), serializer);

        /// <summary>
        /// Loads an object by the specified local path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">The local path of the object.</param>
        /// <param name="serializer">The serializer to use when loading this object.</param>
        /// <param name="throwOnEmpty">Defines whether or not an empty object should throw an Exception.</param>
        public static T Load<T>(string path, JsonSerializer serializer = null, bool throwOnEmpty = false)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
                return default;
            } 

            using (StreamReader stream = File.OpenText(path))
            {
                using (JsonReader reader = new JsonTextReader(stream))
                {
                    serializer = serializer ?? JsonSerializer.Create(DefaultSerializerSettings);
                    T tmp = serializer.Deserialize<T>(reader);
                    if (throwOnEmpty)
                        if (tmp == null)
                            throw new Exception("The file deserialized returned as empty.");
                    return tmp == null ? default : tmp;
                }
            }
        }

        /// <summary>
        /// Loads an object by the specified local path.
        /// </summary>
        /// <param name="path">The local path of the object.</param>
        /// <param name="converter">The JSON converter to use when deserializing.</param>
        public static T Load<T>(string path, JsonConverter converter)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
                return default;
            }

            using (StreamReader stream = File.OpenText(path))
            {
                T tmp = Deserialize<T>(stream.ReadToEndAsync().Result, converter);
                return tmp ?? default;
            }
        }

        public static JsonContainer<TKey, TObject> RestoreContainer<TKey, TObject>(string directory)
        {
            throw new NotImplementedException();
        }

        public static ConcurrentDictionary<ulong, T> RestoreContainer<T>()
        {
            ConcurrentDictionary<ulong, T> tmp = new ConcurrentDictionary<ulong, T>();
            List<string> directory = ReadJsonDirectory(GetDirectory());

            foreach (string path in directory)
            {
                T value = Load<T>(path);
                tmp.AddOrUpdate(ulong.Parse(Path.GetFileNameWithoutExtension(path)), value, (k, v) => value);
            }
            return tmp;
        }

        public static string GetJsonPath<T>(ulong id) where T : IJsonEntity
            => GetDirectory() + string.Format(JsonFrame, id);

        public static bool JsonExists<T>(ulong id) where T : IJsonEntity
            => File.Exists(GetJsonPath<T>(id));

        private static string GetDirectory()
            => Directory.CreateDirectory(BaseDirectory).FullName;

        private static List<string> ReadJsonDirectory(string directoryPath)
            => Directory.Exists(directoryPath) ? Directory.GetFiles(directoryPath, "*.json").ToList() : new List<string>();

        private static Dictionary<string, T> LoadJsonDirectory<T>(List<string> directory)
        {
            Dictionary<string, T> tmp = new Dictionary<string, T>();
            foreach(string path in directory)
                tmp[path] = Load<T>(path);

            return tmp;
        }

        public static T Deserialize<T>(string response, JsonConverter converter)
            => JsonConvert.DeserializeObject<T>(response, converter);

        public static T Deserialize<T>(string response)
            => JsonConvert.DeserializeObject<T>(response, DefaultSerializerSettings);
    }
}
