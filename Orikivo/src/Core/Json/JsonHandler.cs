using Newtonsoft.Json;
using Orikivo.Framework;
using Orikivo.Framework.Json;
using Orikivo.Models.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Orikivo
{
    // TODO: Transfer data over to MongoDB instead
    // ALT: Create a method that only locks if the file is currently open.
    /// <summary>
    /// Represents a handler for JSON serialization and file management.
    /// </summary>
    public static class JsonHandler
    {
        internal static readonly string JsonFrame = "{0}.json";
        internal static readonly string GlobalFileName = "global";
        internal static string BaseDirectory = @"..\data\";
        // private static readonly object _lock = new object();

        public static JsonSerializerSettings DefaultSerializerSettings
            => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                Formatting = Formatting.Indented
            };

        public static void SaveJsonEntity<T>(T obj, JsonSerializer serializer = null)
            where T : IJsonModel
            => Save(obj, string.Format(JsonFrame, obj.Id), serializer);

        public static void SaveEntity<T>(T obj, string directory, JsonSerializer serializer = null)
            where T : IJsonModel
        {
            //lock (_lock)
            Save(obj, directory, string.Format(JsonFrame, obj.Id), serializer);
        }

        public static void DeleteEntity<T>(T obj, string directory)
            where T : IJsonModel
        {
            Delete($"{directory}{string.Format(JsonFrame, obj.Id)}");
        }

        public static void Delete(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        private static string GetDirectory(string directory)
        {
            return Directory.CreateDirectory(directory).FullName;
        }

        /// <summary>
        /// Saves an object to a specified local path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object to serialize as JSON.</param>
        /// <param name="path">The local path to save to.</param>
        /// <param name="serializer"></param>
        public static void Save<T>(T obj, string path, JsonSerializer serializer = null)
        {
            Logger.Debug($"[Debug] -- Saving object of type '{typeof(T).Name}'. --");
            path = JsonUtils.GetDirectoryIndex<T>() + path;
            using var stream = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write));
            serializer ??= JsonSerializer.Create(DefaultSerializerSettings);
            serializer.Serialize(stream, obj);
        }

        public static void Save<T>(T obj, string folder, string name, JsonSerializer serializer = null)
        {
            string path = GetDirectory(folder) + name;
            using var stream = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write));
            serializer ??= JsonSerializer.Create(DefaultSerializerSettings);
            serializer.Serialize(stream, obj);
        }

        public static T LoadJsonEntity<T>(ulong id, JsonSerializer serializer = null) where T : IJsonModel
            => Load<T>(JsonUtils.GetDirectoryIndex<T>() + string.Format(JsonFrame, id), serializer);

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

            using StreamReader stream = File.OpenText(path);
            using var reader = new JsonTextReader(stream);

            serializer ??= JsonSerializer.Create(DefaultSerializerSettings);
            var tmp = serializer.Deserialize<T>(reader);

            if (throwOnEmpty && tmp == null)
                throw new Exception("The file deserialized returned as empty.");

            return tmp == null ? default : tmp;
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

            using StreamReader stream = File.OpenText(path);
            var tmp = Deserialize<T>(stream.ReadToEndAsync().Result, converter);
            return tmp ?? default;
        }

        public static JsonContainer<TKey, TObject> RestoreContainer<TKey, TObject>(string directory)
        {
            throw new NotImplementedException();
        }

        public static ConcurrentDictionary<ulong, T> RestoreContainer<T>(string directory)
        {
            Logger.Debug($"-- Restoring JSON container for type '{typeof(T).Name}'. --");
            var tmp = new ConcurrentDictionary<ulong, T>();
            List<string> files = ReadJsonDirectory(GetDirectory(directory));

            foreach (string path in files)
            {
                var value = Load<T>(path);
                tmp.AddOrUpdate(ulong.Parse(Path.GetFileNameWithoutExtension(path)), value, (k, v) => value);
            }

            return tmp;
        }

        public static ConcurrentDictionary<ulong, T> RestoreContainer<T>()
            => RestoreContainer<T>(JsonUtils.GetDirectoryIndex<T>());

        public static string GetJsonPath<T>(ulong id)
            where T : IJsonModel
            => JsonUtils.GetDirectoryIndex<T>() + string.Format(JsonFrame, id);

        public static bool JsonExists<T>(ulong id) where T : IJsonModel
            => File.Exists(GetJsonPath<T>(id));


        private static List<string> ReadJsonDirectory(string directoryPath)
            => Directory.Exists(directoryPath) ? Directory.GetFiles(directoryPath, "*.json").ToList() : new List<string>();

        private static Dictionary<string, T> LoadJsonDirectory<T>(List<string> directory)
        {
            var tmp = new Dictionary<string, T>();

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
