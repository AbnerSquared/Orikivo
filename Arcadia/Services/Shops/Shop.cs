using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Orikivo;
using Orikivo.Drawing;
using Orikivo.Framework;

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

    [Flags]
    public enum CurrencyType
    {
        Money = 1,
        Chips = 2,
        Tokens = 4,
        Debt = 8
    }

    public class Shop
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Quote { get; set; }

        public List<Vendor> Vendors { get; set; }

        public CatalogGenerator Catalog { get; set; }

        public bool Buy { get; set; }
        public bool Sell { get; set; }

        public CurrencyType AllowedCurrency { get; set; }
        public ItemTag SellTags { get; set; }
        // Deduct 20% by default
        public int SellDeduction { get; set; } = 50;
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
            var discountEntries = new Dictionary<string, int>();
            int entries = 0;

            if (Size <= 0)
            {
                throw new ArgumentException("Cannot initialize a catalog with an empty size.");
            }

            while (entries < Size)
            {
                CatalogEntry entry = GetNextEntry(specials, counters);

                if (entry == null)
                {
                    break;
                }

                Logger.Debug($"Next entry loaded ({entry.ItemId})");

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

                    Logger.Debug($"Special entry applied ({entry.ItemId})");

                    specials++;
                }

                if (entry.DiscountChance > 0 && entry.MaxDiscount.HasValue)
                {
                    if (!discountEntries.ContainsKey(entry.ItemId)
                        && discounts < MaxDiscountsAllowed
                        && RandomProvider.Instance.NextDouble() <= entry.DiscountChance)
                    {
                        // Since upperBound is exclusive, add 1 to the max discount
                        int discountToApply = RandomProvider.Instance.Next(entry.MinDiscount ?? 1, entry.MaxDiscount.Value + 1);
                        discountEntries.Add(entry.ItemId, discountToApply);

                        Logger.Debug($"{discountToApply}% discount applied ({entry.ItemId})");
                        discounts++;
                    }
                }

                if (!ItemHelper.Exists(entry.ItemId))
                    throw new Exception("The specified item ID could not be found.");

                if (!counters.TryAdd(entry.ItemId, 1))
                    counters[entry.ItemId]++;

                entries++;
            }

            Logger.Debug($"Compiling catalog with {entries} {Format.TryPluralize("entry", entries)}");

            return new ItemCatalog(counters, discountEntries);
        }

        private int GetTotalWeight()
            => Entries.Select(x => x.Weight).Sum();

        private int GetAvailableWeight(int specials, Dictionary<string, int> counters)
        {
            return GetAvailableEntries(specials, counters).Sum(x => x.Weight);
        }

        private IEnumerable<CatalogEntry> GetAvailableEntries(int specials, Dictionary<string, int> counters)
        {
            return Entries.Where(x =>
                (specials < MaxSpecialsAllowed || !x.IsSpecial)
                && (!x.MaxAllowed.HasValue || counters.GetValueOrDefault(x.ItemId, 0) >= x.MaxAllowed));
        }

        private CatalogEntry GetNextEntry(int specials, Dictionary<string, int> counters)
        {
            var entries = GetAvailableEntries(specials, counters);
            int totalWeight = entries.Sum(x => x.Weight);
            int marker = RandomProvider.Instance.Next(0, totalWeight);
            int weightSum = 0;

            if (totalWeight == 0)
                return null;

            for (int i = 0; i < entries.Count(); i++)
            {
                weightSum += entries.ElementAt(i).Weight;

                if (marker <= weightSum)
                    return Entries[i];
            }

            throw new Exception("Weighed marker was out of bounds.");
        }
    }

    public class ItemCatalog
    {
        public ItemCatalog(){}

        public ItemCatalog(Dictionary<string, int> itemIds, Dictionary<string, int> discounts)
        {
            GeneratedAt = DateTime.UtcNow;
            var items = new Dictionary<Item, int>();

            foreach ((string item, int amount) in itemIds)
            {
                items.Add(ItemHelper.GetItem(item), amount);
            }

            Items = items;

            Discounts = discounts;
        }

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

        public int? MinDiscount { get; set; }

        public int? MaxDiscount { get; set; }

        public float DiscountChance { get; set; }

        public bool IsSpecial { get; set; }

        public int Weight { get; set; }
    }
}