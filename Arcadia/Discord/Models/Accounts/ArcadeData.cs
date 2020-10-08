using System;
using System.Collections.Generic;
using Arcadia.Multiplayer;
using Newtonsoft.Json;
using Orikivo;

namespace Arcadia
{
    public class ArcadeData
    {
        public static readonly string Version = "0.8.1b";

        public ArcadeData()
        {
            Catalogs = new Dictionary<string, ItemCatalog>();
        }

        [JsonConstructor]
        internal ArcadeData(ulong logChannelId, Dictionary<string, ItemCatalog> catalogs, string bonusGameId, DateTime bonusAssignedAt)
        {
            LogChannelId = logChannelId;
            Catalogs = catalogs ?? new Dictionary<string, ItemCatalog>();
            BonusGameId = bonusGameId;
            BonusAssignedAt = bonusAssignedAt;
        }

        [JsonProperty("log_channel_id")]
        public ulong LogChannelId { get; internal set; }

        [JsonProperty("catalogs")]
        public Dictionary<string, ItemCatalog> Catalogs { get; }

        [JsonProperty("bonus_game_id")]
        public string BonusGameId { get; internal set; }

        [JsonProperty("bonus_assigned_at")]
        internal DateTime BonusAssignedAt { get; set; }

        public string GetOrAssignBonusGame(GameManager games)
        {
            if (BonusAssignedAt == DateTime.MinValue || CooldownHelper.DaysSince(BonusAssignedAt) >= 7)
            {
                BonusGameId = Randomizer.Choose(games.Games.Keys);
                BonusAssignedAt = DateTime.UtcNow;
            }

            return BonusGameId;
        }

        public ItemCatalog GetOrGenerateCatalog(Shop shop, ArcadeUser user = null)
        {
            if (Catalogs.ContainsKey(shop.Id)
                && DateTime.UtcNow.DayOfYear - Catalogs[shop.Id].GeneratedAt.DayOfYear == 0)
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

            Catalogs[shop.Id] = shop.Catalog.Generate(Var.GetOrSet(user, ShopHelper.GetTierId(shop.Id), 1));

            if (user != null && user.CatalogHistory.ContainsKey(shop.Id))
                user.CatalogHistory[shop.Id].Clear();

            return new ItemCatalog(Catalogs[shop.Id]);
        }
    }
}