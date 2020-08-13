using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Orikivo;
using Orikivo.Drawing;

namespace Arcadia
{
    public class Vendor
    {
        public static readonly string NameGeneric = "Vendor";
        public static readonly string EnterGeneric = "Welcome.";
        public static readonly string MenuGeneric = "What can I do for you?";
        public static readonly string TimeoutGeneric = "I'm busy right now. Come back later.";
        public static readonly string BuyGeneric = "Thank you for your business.";
        public static readonly string SellGeneric = "Thank you for the sale.";
        public static readonly string ViewBuyGeneric = "This is everything I have for sale.";
        public static readonly string ViewSellGeneric = "What can I buy from you?";
        public static readonly string ExitGeneric = "Goodbye.";

        public string Name { get; set; }

        public List<string> OnEnter { get; set; }

        public List<string> OnMenu { get; set; }

        public List<string> OnTimeout { get; set; }

        public List<string> OnBuy { get; set; }

        public List<string> OnSell { get; set; }

        public List<string> OnViewBuy { get; set; }

        public List<string> OnViewSell { get; set; }

        public List<string> OnExit { get; set; }
    }

    public class Shop
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Quote { get; set; }

        public List<Vendor> Vendors { get; set; }

        public CatalogGenerator Catalog { get; set; }
    }

    public class CatalogGenerator
    {
        public int Size { get; set; }
        public int MaxDiscountsAllowed { get; set; }
        public int MaxSpecialsAllowed { get; set; }

        public List<CatalogEntry> Entries { get; set; }

        public ItemCatalog Generate()
        {
            int specials = 0;
            int discounts = 0;
            var counters = new Dictionary<string, int>();
            var discountEntries = new Dictionary<string, float>();
            var entries = new List<Item>();

            if (Size <= 0)
            {
                throw new ArgumentException("Cannot initialize a catalog with an empty size.");
            }

            while (entries.Count < Size)
            {
                CatalogEntry entry = GetNextEntry();

                if (entry.MaxAllowed.HasValue)
                {
                    if (counters.ContainsKey(entry.ItemId))
                    {
                        if (counters[entry.ItemId] >= entry.MaxAllowed.Value)
                            continue;
                    }
                }

                if (entry.IsSpecial)
                {
                    if (specials >= MaxSpecialsAllowed)
                        continue;

                    specials++;
                }

                if (entry.MaxDiscount.HasValue)
                {
                    if (!discountEntries.ContainsKey(entry.ItemId) && discounts < MaxDiscountsAllowed)
                    {
                        int discountToApply = RandomProvider.Instance.Next(0, entry.MaxDiscount.Value);

                        discountEntries.Add(entry.ItemId, discountToApply);
                        discounts++;
                    }
                }

                if (!ItemHelper.TryGetItem(entry.ItemId, out Item item))
                    throw new Exception("The specified item ID could not be found.");

                if (!counters.TryAdd(entry.ItemId, 1))
                    counters[entry.ItemId]++;

                entries.Add(ItemHelper.GetItem(entry.ItemId));
            }

            return new ItemCatalog();
        }

        private int GetTotalWeight()
            => Entries.Select(x => x.Weight).Sum();

        private CatalogEntry GetNextEntry()
        {
            int totalWeight = GetTotalWeight();
            int marker = RandomProvider.Instance.Next(0, totalWeight);
            int weightSum = 0;

            for (int i = 0; i < Entries.Count; i++)
            {
                weightSum += Entries[i].Weight;

                if (marker <= weightSum)
                    return Entries[i];
            }

            throw new Exception("Weighed marker was out of bounds.");
        }
    }

    public class ItemCatalog
    {
        public ItemCatalog(){}

        public ItemCatalog(ItemCatalogData data)
        {
            GeneratedAt = data.GeneratedAt;

            var items = new Dictionary<Item, int>();

            foreach((string item, int amount) in data.ItemIds)
                items.Add(ItemHelper.GetItem(item), amount);

            Items = items;

            Discounts = data.Discounts;
        }

        public DateTime GeneratedAt { get; set; }

        public Dictionary<Item, int> Items { get; set; }

        public Dictionary<string, int> Discounts { get; set; }

        public int Count => Items.Values.Sum();

        // Maybe introduce compression?
    }

    public class ItemCatalogData
    {
        [JsonConstructor]
        internal ItemCatalogData(DateTime generatedAt, Dictionary<string, int> itemIds,
            Dictionary<string, int> discounts)
        {
            GeneratedAt = generatedAt;
            ItemIds = itemIds ?? new Dictionary<string, int>();
            Discounts = discounts ?? new Dictionary<string, int>();
        }

        [JsonProperty("generated_at")]
        public DateTime GeneratedAt { get; }

        [JsonProperty("item_ids")]
        public Dictionary<string, int> ItemIds { get; }

        [JsonProperty("discounts")]
        public Dictionary<string, int> Discounts { get; }
    }

    public class CatalogEntry
    {
        public string ItemId { get; set; }

        public int? MaxAllowed { get; set; }

        public int? MaxDiscount { get; set; }

        public bool IsSpecial { get; set; }

        public int Weight { get; set; }
    }
}