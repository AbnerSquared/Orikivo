using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Arcadia
{
    public class ArcadeData
    {
        public static readonly string Version = "0.8.0b";
        // Once a day
        public static readonly TimeSpan CatalogRefreshRate = TimeSpan.FromHours(24);

        public ArcadeData()
        {
            Catalogs = new Dictionary<string, ItemCatalog>();
        }

        [JsonConstructor]
        internal ArcadeData(ulong logChannelId, Dictionary<string, ItemCatalog> catalogs)
        {
            LogChannelId = logChannelId;
            Catalogs = catalogs ?? new Dictionary<string, ItemCatalog>();
        }

        [JsonProperty("log_channel_id")]
        public ulong LogChannelId { get; internal set; }

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
}