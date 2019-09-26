using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Orikivo
{
    // this deals with storing and loading to objects
    // related to json files
    public static class OriJsonHandler
    {
        internal static readonly string DefaultFileFormat = "{0}.json";
        internal static readonly string DefaultGlobalFileName = "global";
        public static JsonSerializerSettings DefaultSerializerSettings
        {
            get
            {
                JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
                serializerSettings.NullValueHandling = NullValueHandling.Ignore;
                serializerSettings.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
                serializerSettings.Formatting = Formatting.Indented;
                return serializerSettings;
            }
        }

        public static void SaveJsonEntity<T>(T obj, JsonSerializer serializer = null) where T : IJsonEntity
            => Save(obj, string.Format(DefaultFileFormat, obj.Id), serializer);

        public static void Save<T>(T obj, string path, JsonSerializer serializer = null)
        {
            Console.WriteLine($"[Debug] -- Saving object of type '{typeof(T).Name}'. --");
            path = GetDirectoryIndex<T>() + path;
            using (StreamWriter stream = File.CreateText(path))
            {
                using (JsonWriter writer = new JsonTextWriter(stream))
                {
                    serializer = serializer ?? JsonSerializer.Create(DefaultSerializerSettings);
                    serializer.Serialize(stream, obj);
                }
            }
        }
        public static T LoadJsonEntity<T>(ulong id, JsonSerializer serializer = null) where T : IJsonEntity
            => Load<T>(GetDirectoryIndex<T>() + string.Format(DefaultFileFormat, id), serializer);

        public static T Load<T>(string path, JsonSerializer serializer = null)
        {
            Console.WriteLine($"[Debug] -- Loading object of type '{typeof(T).Name}'. --");
            if (typeof(T) == typeof(OriGlobal))
                path = GetDirectoryIndex<OriGlobal>() + path;

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
                    return tmp == null ? default : tmp;
                }
            }
        }

        public static T LoadDynamicArray<T>(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
                return default;
            }

            using (StreamReader stream = File.OpenText(path))
            {
                T tmp = Deserialize<T>(stream.ReadToEndAsync().Result, new DynamicArrayJsonConverter<T>());
                return tmp == null ? default : tmp;
            }
        }

        public static ConcurrentDictionary<ulong, T> RestoreJsonContainer<T>()
        {
            Console.WriteLine($"[Debug] -- Restoring JSON container for type '{typeof(T).Name}'. --");
            ConcurrentDictionary<ulong, T> tmp = new ConcurrentDictionary<ulong, T>();
            List<string> directory = ReadJsonDirectory(GetDirectoryIndex<T>());
            foreach (string path in directory)
            {
                T value = Load<T>(path);
                tmp.AddOrUpdate(ulong.Parse(Path.GetFileNameWithoutExtension(path)), value, (k, v) => value);
            }
            return tmp;
        }

        private static string GetDirectoryIndex<T>()
        {
            if (typeof(T) == typeof(OriUser))
                return Directory.CreateDirectory(@"..\data\users\").FullName;
            if (typeof(T) == typeof(OriGuild))
                return Directory.CreateDirectory(@"..\data\guilds\").FullName;
            if (typeof(T) == typeof(OriGlobal))
                return Directory.CreateDirectory(@"..\data\global\").FullName;
            return Directory.CreateDirectory(@"..\data\").FullName;
            //throw new Exception($"Type '{typeof(T).Name}' is not a valid JSON container.");
        }

        private static List<string> ReadJsonDirectory(string directoryPath)
            => Directory.Exists(directoryPath) ? Directory.GetFiles(directoryPath, "*.json").ToList() : new List<string>();

        private static Dictionary<string, T> LoadJsonDirectory<T>(List<string> directory)
        {
            Dictionary<string, T> tmp = new Dictionary<string, T>();
            foreach(string path in directory)
            {
                tmp[path] = Load<T>(path);
            }
            return tmp;
        }

        public static T Deserialize<T>(string response, JsonConverter converter)
            => JsonConvert.DeserializeObject<T>(response, converter);

        public static T Deserialize<T>(string response)
            => JsonConvert.DeserializeObject<T>(response, DefaultSerializerSettings);
    }
}
