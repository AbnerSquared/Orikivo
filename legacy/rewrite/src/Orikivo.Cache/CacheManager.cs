using Orikivo.Static;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Orikivo.Cache
{
    public static class CacheManager
    {
        public static FontCache LoadFontCache()
        {
            List<FontFace> fonts = new List<FontFace>();
            char[][][][] arrayMap = Manager.LoadDynamicArray<char[][][][]>(Locator.ArrayMap);
            string directory = Directory.CreateDirectory(Locator.Resources + "\\fonts\\").FullName;
            List<string> files = LoadCache(directory);
            foreach (string file in files)
                fonts.Add(Manager.Load<FontFace>(file));

            return new FontCache(fonts, arrayMap);
        }

        public static List<string> LoadCache(string path)
            => Directory.Exists(path) ? Directory.GetFiles(path, "*.json").ToList() : new List<string>();

        public static List<string> LoadFiles(string path)
            => Directory.Exists(path) ? Directory.GetFiles(path).ToList() : new List<string>();

        // find a way to polish this up.
        public static string GetWriteDirectory<T>()
        {
            if (typeof(T) == typeof(OriUser))
                return Directory.CreateDirectory(Locator.Data + Locator.Accounts).FullName;
            if (typeof(T) == typeof(OriGuild))
                return Directory.CreateDirectory(Locator.Data + Locator.Guilds).FullName;
            return Directory.CreateDirectory(Locator.Data).FullName;
        }

        public static string GetWritePath<T>(T obj)
        {
            if (typeof(T) == typeof(IStorable))
                return GetWriteDirectory<T>() + "\\" + (obj as IStorable).Id + ".json";
            if (typeof(T) == typeof(Global))
                return GetWriteDirectory<T>() + "\\" + "global.json";
            return GetWriteDirectory<T>() + "\\" + obj.ToString() + ".json";
        }

        public static ConcurrentDictionary<ulong, T> GetContainer<T>()
        {
            ConcurrentDictionary<ulong, T> tmp = new ConcurrentDictionary<ulong, T>();
            AddToContainer(tmp, LoadFiles(GetWriteDirectory<T>()));
            return tmp;
        }

        public static ConcurrentDictionary<ulong, T> AddToContainer<T>(ConcurrentDictionary<ulong, T> container, List<string> paths)
        {
            foreach(string path in paths)
            {
                T tmp = Manager.Load<T>(path);
                container.AddOrUpdate(ParsePath(path), tmp, (k, v) => tmp);
            }
            return container;
        }

        private static ulong ParsePath(string path)
            => ulong.Parse(Path.GetFileNameWithoutExtension(path));
    }
}
