﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Orikivo.Text
{
    public class LocaleProvider
    {
        private static readonly string DefaultPath = @"bin/Release/assets/locale/";

        public LocaleProvider()
        {
            IDictionary<string, Dictionary<string, string>> entries = RestoreDirectory<Dictionary<string, string>>(DefaultPath);
            var banks = new List<LocaleBank>();

            foreach ((string lang, Dictionary<string, string> nodes) in entries)
            {
                if (!(Enum.TryParse(lang, true, out Language result) && banks.All(x => x.Language != result)))
                    throw new ArgumentException("One of the specified locale files has already been specified or is invalid");

                banks.Add(new LocaleBank(result, nodes.Select(x => new LocaleNode(x.Key, x.Value)).ToList()));
            }

            Banks = banks;
        }

        public IEnumerable<LocaleBank> Banks { get; set; }

        public Language DefaultLanguage { get; set; } = Language.English;

        public LocaleBank GetBank(Language language)
            => Banks.FirstOrDefault(x => x.Language == language);

        public string GetValue(string id, Language language = Language.English, params object[] args)
        {
            return GetBank(language)?.GetNode(id)?.ToString(args) ?? "INVALID_LOCALE";
        }

        public string GetValueOrDefault(string id, Language language = Language.English, string defaultValue = "INVALID_LOCALE")
            => GetBank(language)?.GetNode(id)?.ToString() ?? defaultValue;

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
            string fullPath = GetOrCreateDirectory(directory);
            Console.WriteLine(fullPath);
            var tmp = new ConcurrentDictionary<string, TValue>();
            List<string> files = ReadJsonDirectory(fullPath);

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