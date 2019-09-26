using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Orikivo.Static;
using Orikivo.Utility;
using System;
using System.Linq;
using Discord.WebSocket;

namespace Orikivo.Storage
{
    public class ShopCache
    {
        public ShopCache(List<OriShop> shops)
        {
            Shops = shops;
        }

        public List<OriShop> Shops { get; set; }
    }

    public class ItemCache
    {
        public ItemCache(List<OriItem> items)
        {
            Items = items;
        }

        public List<OriItem> Items { get; set; }
    }

    public class FileManager
    {
        public static ShopCache GetShops()
        {
            List<OriShop> shops = new List<OriShop>();
            string dir = Directory.CreateDirectory($"{Locator.Resources}\\shops\\").FullName;
            List<string> jsons = GetJsonFiles(dir);
            foreach (string json in jsons)
            {
                shops.Add(LoadShop(json));
            }
            // catch if a group that isnt in the item cache doesn't exist.
            return new ShopCache(shops);
        }

        public static ItemCache GetItems()
        {
            List<OriItem> items = new List<OriItem>();
            string dir = Directory.CreateDirectory($"{Locator.Resources}//items//").FullName;
            List<string> jsons = GetJsonFiles(dir);
            foreach (string json in jsons)
            {
                items.Add(LoadItem(json));
            }
            return new ItemCache(items);
        }

        public static FontCache GetFonts()
        {
            List<FontFace> maps = new List<FontFace>();
            char[][][][] arrayMap = Manager.LoadDynamicArray<char[][][][]>(Locator.ArrayMap);
            string dir = Directory.CreateDirectory($"{Locator.Resources}\\fonts\\").FullName;
            List<string> jsons = GetJsonFiles(dir);
            foreach (string json in jsons)
            {
                maps.Add(LoadFont(json));
            }

            return new FontCache(maps, arrayMap);
        }

        public static MeritCache GetMerits()
        {
            List<Merit> merits = new List<Merit>();
            string dir = Directory.CreateDirectory($"{Locator.Resources}\\merits\\").FullName;
            List<string> jsons = GetJsonFiles(dir);
            foreach ( string json in jsons)
            {
                merits.Add(LoadMerit(json));
            }
            return new MeritCache(merits);

        }

        public static List<string> GetJsonFiles(string dir)
            => Directory.Exists(dir) ? Directory.GetFiles(dir, "*.json").ToList() : new List<string>();

        public static OriShop LoadShop(string path)
        {
            Path.GetFileName(path).Debug("Including shop...");
            return Manager.Load<OriShop>(path);
        }

        public static Merit LoadMerit(string path)
        {
            Path.GetFileName(path).Debug("Including merit...");
            return Manager.Load<Merit>(path);
        }

        public static FontFace LoadFont(string path)
        {
            Path.GetFileName(path).Debug("Including font face...");
            return Manager.Load<FontFace>(path);
        }

        public static OriItem LoadItem(string path)
        {
            Path.GetFileName(path).Debug("Including item...");
            return Manager.Load<OriItem>(path);
        }

        public static string TryGetDirectory<T>()
        {
            string dir = $".\\{Locator.Data}\\";

            if (typeof(T).Equals(typeof(OldAccount)))
                dir += $"{Locator.Accounts}";
            else if (typeof(T).Equals(typeof(Server)))
                dir += $"{Locator.Guilds}";

            dir = Directory.CreateDirectory(dir).FullName;
            return dir;
        }

        public static string TryGetPath<T>(T obj)
        {
            string path = TryGetDirectory<T>();
            if (typeof(T).Equals(typeof(OldAccount)))
                path += $"\\{(obj as OldAccount).Id}.{Locator.Output}";
            else if (typeof(T).Equals(typeof(Server)))
                path += $"\\{(obj as Server).Id}.{Locator.Output}";
            else if (typeof(T).Equals(typeof(OldGlobal)))
                path += $"global.{Locator.Output}";
            else
                path += $"{obj}.{Locator.Output}";

            return path;
        }

        public static List<string> TryGetFiles(string directory) =>
            Directory.Exists(directory)? Directory.GetFiles(directory).ToList() : new List<string>();
        
        public static ConcurrentDictionary<ulong, T> GetContainer<T>()
        {
            ConcurrentDictionary<ulong, T> tmp = new ConcurrentDictionary<ulong, T>();
            UpdateContainer(tmp, TryGetFiles(TryGetDirectory<T>()));
            return tmp;
        }

        public static ConcurrentDictionary<ulong, T> UpdateContainer<T>(
            ConcurrentDictionary<ulong, T> container, List<string> index)
        {
            foreach (string path in index)
            {
                T tmp = Manager.Load<T>(path);
                tmp.Debug();
                container.AddOrUpdate(PathToId(path), tmp, (k, v) => tmp);
            }
            return container;
        }

        public static ulong PathToId(string path) =>
            ulong.Parse(Path.GetFileNameWithoutExtension(path));
    }
}