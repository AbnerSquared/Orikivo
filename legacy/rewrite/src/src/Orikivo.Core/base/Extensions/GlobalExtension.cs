using System.Collections.Generic;
using System.Linq;
using Orikivo.Logging;
using Orikivo.Storage;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.IO;
using Orikivo.Static;
using Orikivo.Utility;
using System;
using Orikivo.Providers;

namespace Orikivo
{
    // Methods that base off of T.
    public static class GlobalExtension
    {
        public static void Debug(this object obj, string s = null) =>
            Logger.Log(obj.ToString() + (s == null ? "" : $" | {s}"));

        public static T GetAny<T>(this IEnumerable<T> enumerable)
            => enumerable.ToList().GetAny();

        public static int InstanceCount<T>(this List<T> list, T item)
        {
            return list.Where(x => x.Equals(item)).Count();
        }

        public static T GetAny<T>(this List<T> list)
        {
            return list[RandomProvider.Instance.Next(1, list.Count()) - 1];
        }

        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
                action(item);
        }

        public static bool Exists<T>(this T obj) =>
            !(obj == null);

        public static bool EqualsAny<T>(this T obj, params T[] args) => 
            args.Contains(obj);

        public static bool EqualsAny<T>(this T obj, List<T> args) =>
            args.Contains(obj);

        public static void SaveObject<T>(this T obj, string s) =>
            Manager.Save(obj, s);
        
        public static void SaveObject<T>(this T obj) =>
            Manager.Save(obj, FileManager.TryGetPath(obj));

        public static void SaveObject<T>(this T obj, out string path)
        {
            path = FileManager.TryGetPath(obj);
            Manager.Save(obj, path);
        }

        public static ConcurrentDictionary<ulong, T> UpdateContainer<T>(this ConcurrentDictionary<ulong, T> d, T obj, List<string> index)
            => FileManager.UpdateContainer<T>(d, index);

        public static ConcurrentDictionary<ulong, T> GetContainer<T>(this ConcurrentDictionary<ulong, T> d)
            => FileManager.GetContainer<T>();

        public static T GetObject<T>(this T obj, string s)
            => Manager.Load<T>(s);

        public static T GetObject<T>(this T obj)
            => Manager.Load<T>(FileManager.TryGetPath(obj));

        public static bool TryGetObject<T>(this T obj, out T value)
        {
            value = obj.GetObject();
            return value.Exists();
        }

        public static void RestoreObject<T>(this T obj)
            => obj = obj.GetObject();

        public static void RestoreObject<T>(this T obj, string s)
            => obj = obj.GetObject(s);

        public static void RestoreContainer<T>(this ConcurrentDictionary<ulong, T> d)
            => d = d.GetContainer<T>();

        public static void SetObject<T>(this T obj, T data)
            => obj = data;
    }
}