using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Orikivo.Text
{
    public class LocaleProvider
    {
        private static readonly string DefaultPath = @"..\assets\locale\";

        public static LocaleProvider GetDefault()
        {
            IDictionary<string, List<LocaleNode>> entries = RestoreDirectory<List<LocaleNode>>(DefaultPath);
            var banks = new List<LocaleBank>();

            foreach ((string lang, List<LocaleNode> nodes) in entries)
            {
                if (!(Enum.TryParse(lang, true, out Language result) && banks.All(x => x.Language != result)))
                    throw new ArgumentException("One of the specified locale files has already been specified or is invalid");

                banks.Add(new LocaleBank(result, nodes));
            }

            return new LocaleProvider
            {
                Banks = banks
            };
        }

        public IEnumerable<LocaleBank> Banks { get; set; }

        public LocaleBank GetBank(Language language)
            => Banks.FirstOrDefault(x => x.Language == language || x.Language == Language.English);

        public string GetValue(string id, Language language = Language.English)
        {
            return GetBank(language)?.GetNode(id)?.ToString() ?? "UNKNOWN_NODE";
        }

        private static JsonSerializerSettings DefaultSerializerSettings
            => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                Formatting = Formatting.Indented
            };

        private static T Load<T>(string path, JsonSerializer serializer = null, bool throwOnEmpty = false)
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

        private static string GetOrCreateDirectory(string directory)
        {
            return Directory.CreateDirectory(directory).FullName;
        }

        private static IDictionary<string, TValue> RestoreDirectory<TValue>(string directory)
        {
            var tmp = new ConcurrentDictionary<string, TValue>();
            List<string> files = ReadJsonDirectory(GetOrCreateDirectory(directory));

            foreach (string path in files)
            {
                var value = Load<TValue>(path);
                tmp.AddOrUpdate(Path.GetFileNameWithoutExtension(path), value, (k, v) => value);
            }

            return tmp;
        }

        private static List<string> ReadJsonDirectory(string directoryPath)
            => Directory.Exists(directoryPath) ? Directory.GetFiles(directoryPath, "*.json").ToList() : new List<string>();
    }
}