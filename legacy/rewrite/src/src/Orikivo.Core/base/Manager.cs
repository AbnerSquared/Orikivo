using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.IO;
using Orikivo.Static;
using Orikivo.Utility;

namespace Orikivo
{
    // The root of all managers. This stores the basic functions of saving, loading, and executing processes.
    public static class Manager
    {

        public static async void WriteTextAsync(string text, string path)
        {
            //Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (StreamWriter data = File.CreateText(path))
            {
                await data.WriteAsync(text);
            }
        }

        public static void Save<T>(T obj, string path)
        {
            using (StreamWriter data = File.CreateText(path))
            {
                using (JsonWriter writer = new JsonTextWriter(data))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.NullValueHandling = NullValueHandling.Ignore;
                    serializer.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
                    serializer.Formatting = Newtonsoft.Json.Formatting.Indented;
                    serializer.Serialize(data, obj);
                }
            }
        }

        public static T Load<T>(T obj, string path) =>
            Load<T>(path);
        
        public static T Load<T>(string path)
        {
            //T obj = typeof(T).Build<T>();

            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
                return default;
            }

            using (StreamReader data = File.OpenText(path))
            {
                using (JsonReader reader = new JsonTextReader(data))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.NullValueHandling = NullValueHandling.Ignore;
                    serializer.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
                    serializer.Formatting = Newtonsoft.Json.Formatting.Indented;

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

            using (StreamReader data = File.OpenText(path))
            {
                T tmp = JsonConvert.DeserializeObject<T>(data.ReadToEndAsync().Result, new DynamicArrayJsonConverter<T>());
                return tmp == null ? default : tmp;
            }
        }

        public static T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public static ProcessStartInfo BuildProcessStartInfo(string program, [Runner]string args)
            => new ProcessStartInfo
            {
                FileName = program,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };

        public static Process Execute(string program, [Runner]string args)
            => Process.Start(BuildProcessStartInfo(program, args));

        public static Process Execute(ProcessStartInfo structure)
            => Process.Start(structure);

        public static string ExecuteOutput(Process p)
            => p.StandardOutput.ReadToEnd();

        public static string ExecuteOutput(ProcessStartInfo p)
            => Execute(p).StandardOutput.ReadToEnd();

        public static string ExecuteError(Process p)
            => p.StandardError.ReadToEnd();

        public static string ExecuteError(ProcessStartInfo p)
            => Execute(p).StandardError.ReadToEnd();
    }
}