using Newtonsoft.Json;
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
        internal static JsonSerializerSettings SerializerSettings = DefaultSerializerSettings;

        public static JsonSerializerSettings DefaultSerializerSettings
            => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                Formatting = Formatting.Indented
            };

        public static void SetBaseDirectory(string directory)
            => BaseDirectory = directory;

        public static void ResetBaseDirectory()
            => BaseDirectory = DEFAULT_DIRECTORY;

        public static void SetSerializerSettings(JsonSerializerSettings settings)
            => SerializerSettings = settings;

        public static void ResetSerializerSettings()
            => SerializerSettings = DefaultSerializerSettings;

        public static string Serialize(object obj)
            => JsonConvert.SerializeObject(obj, SerializerSettings);

        public static string Serialize(object obj, params JsonConverter[] converters)
            => JsonConvert.SerializeObject(obj, converters);

        public static T Deserialize<T>(string value)
            => JsonConvert.DeserializeObject<T>(value, SerializerSettings);

        public static T Deserialize<T>(string value, params JsonConverter[] converters)
            => JsonConvert.DeserializeObject<T>(value, converters);

        /// <summary>
        /// Saves an object to a specified local path.
        /// </summary>
        /// <param name="obj">The object to serialize as JSON.</param>
        /// <param name="path">The local path to save to.</param>
        public static void Save(object obj, string path, JsonSerializer serializer = null)
        {
            // path = GetDirectory() + path;
            using (StreamWriter stream = File.CreateText(path))
            {
                using (JsonWriter writer = new JsonTextWriter(stream))
                {
                    serializer ??= JsonSerializer.Create(SerializerSettings);
                    serializer.Serialize(stream, obj);
                }
            }
        }

        /// <summary>
        /// Loads an object by the specified local path.
        /// </summary>
        /// <param name="path">The local path of the object.</param>
        /// <param name="serializer">The serializer to use when loading this object.</param>
        /// <param name="throwOnEmpty">Determines if an empty object should throw an <see cref="Exception"/></param>
        public static T Load<T>(string path, JsonSerializer serializer, bool throwOnEmpty = false)
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
                    serializer ??= JsonSerializer.Create(SerializerSettings);
                    T tmp = serializer.Deserialize<T>(reader);

                    if (throwOnEmpty)
                        if (tmp == null)
                            throw new NullReferenceException("The file deserialized returned as empty.");

                    return tmp == null ? default : tmp;
                }
            }
        }

        /// <summary>
        /// Loads an object by the specified local path.
        /// </summary>
        /// <param name="path">The local path of the object.</param>
        /// <param name="converters">The JSON converter to use when deserializing.</param>
        public static T Load<T>(string path, params JsonConverter[] converters)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
                return default;
            }

            using (StreamReader stream = File.OpenText(path))
            {
                T tmp = Deserialize<T>(stream.ReadToEndAsync().GetAwaiter().GetResult(), converters);
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
            List<string> directory = GetFilePathsFromDirectory(GetCurrentDirectory());

            foreach (string path in directory)
            {
                T value = Load<T>(path);
                tmp.AddOrUpdate(ulong.Parse(Path.GetFileNameWithoutExtension(path)), value,
                    (k, v) => value);
            }
            return tmp;
        }

        public static string GetJsonPath<T>(ulong id)
            where T : IJsonEntity
            => GetCurrentDirectory() + string.Format(JsonFrame, id);

        public static bool JsonExists<T>(ulong id)
            where T : IJsonEntity
            => File.Exists(GetJsonPath<T>(id));

        private static bool IsValidFileName(string fileName)
        {
            return !fileName.ContainsAny(Path.GetInvalidFileNameChars());
        }

        private static string GetCurrentDirectory()
            => Directory.CreateDirectory(BaseDirectory).FullName;

        private static List<string> GetFilePathsFromDirectory(string directoryPath)
            => Directory.Exists(directoryPath)
            ? Directory.GetFiles(directoryPath, "*.json").ToList()
            : new List<string>();

        public static Dictionary<string, T> GetFilesFromDirectory<T>(string directoryPath)
        {
            var tmp = new Dictionary<string, T>();

            foreach(string path in GetFilePathsFromDirectory(directoryPath))
                tmp[path] = Load<T>(path);

            return tmp;
        }
    }
}
