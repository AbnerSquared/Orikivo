using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Orikivo;

namespace Arcadia
{
    public class ArcadeData
    {
        // Once a day
        public static readonly TimeSpan CatalogRefreshRate = TimeSpan.FromHours(24);

        public ArcadeData()
        {
            Catalogs = new Dictionary<string, ItemCatalog>();
        }

        [JsonConstructor]
        internal ArcadeData(Dictionary<string, ItemCatalog> catalogs)
        {
            Catalogs = catalogs ?? new Dictionary<string, ItemCatalog>();
        }

        [JsonProperty("catalogs")]
        public Dictionary<string, ItemCatalog> Catalogs { get; }

        public ItemCatalog GetOrGenerateCatalog(Shop shop, ArcadeUser user = null)
        {
            if (Catalogs.ContainsKey(shop.Id))
            {
                // If the two days are exactly the same, keep the catalog
                if (DateTime.UtcNow.DayOfYear - Catalogs[shop.Id].GeneratedAt.DayOfYear == 0)
                {
                    var catalog = new ItemCatalog(Catalogs[shop.Id]);

                    if (user == null || !user.CatalogHistory.ContainsKey(shop.Id))
                        return catalog;

                    var toRemoveIds = new List<string>();

                    foreach ((string itemId, int amount) in user.CatalogHistory[shop.Id].PurchasedIds)
                    {
                        if (amount <= 0)
                        {
                            toRemoveIds.Add(itemId);
                            continue;
                        }

                        if (catalog.ItemIds.ContainsKey(itemId))
                        {
                            catalog.ItemIds[itemId] -= amount;

                            if (catalog.ItemIds[itemId] <= 0)
                                catalog.ItemIds.Remove(itemId);
                        }
                    }

                    foreach (string removeId in toRemoveIds)
                        user.CatalogHistory[shop.Id].PurchasedIds.Remove(removeId);

                    return catalog;
                }
            }

            Catalogs[shop.Id] = shop.Catalog.Generate();

            if (user != null && user.CatalogHistory.ContainsKey(shop.Id))
                user.CatalogHistory[shop.Id].Clear();

            return new ItemCatalog(Catalogs[shop.Id]);
        }
    }

    public class CatalogHistory
    {
        public CatalogHistory()
        {
            PurchasedIds = new Dictionary<string, int>();
            SoldIds = new Dictionary<string, int>();
        }

        [JsonConstructor]
        internal CatalogHistory(Dictionary<string, int> purchasedIds, Dictionary<string, int> soldIds)
        {
            PurchasedIds = purchasedIds ?? new Dictionary<string, int>();
            SoldIds = soldIds ?? new Dictionary<string, int>();
        }

        [JsonProperty("purchased_ids")]
        public Dictionary<string, int> PurchasedIds { get; }

        [JsonProperty("sold_ids")]
        public Dictionary<string, int> SoldIds { get; }

        public void Clear()
        {
            PurchasedIds.Clear();
            SoldIds.Clear();
        }
    }

    public class ArcadeContainer
    {
        public ArcadeContainer()
        {
            Users = new JsonContainer<ArcadeUser>(@"..\data\users\");
            Guilds = new JsonContainer<BaseGuild>(@"..\data\guilds\");
            Data = JsonHandler.Load<ArcadeData>(@"..\data\global.json") ?? new ArcadeData();
        }

        public JsonContainer<ArcadeUser> Users { get; }
        public JsonContainer<BaseGuild> Guilds { get; }
        public ArcadeData Data { get; }

        public void SaveGlobalData()
        {
            JsonHandler.Save(Data, @"global.json");
        }
    }
}
