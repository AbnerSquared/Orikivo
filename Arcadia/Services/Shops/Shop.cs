using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Orikivo;
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

    [Flags]
    public enum ShopAllow
    {
        Buy = 1,
        Sell = 2,
        All = Buy | Sell
    }

    /// <summary>
    /// Represents a market from which items can be sold.
    /// </summary>
    public class Shop
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Quote { get; set; }

        public List<Vendor> Vendors { get; set; }

        public CatalogGenerator Catalog { get; set; }

        public ShopAllow Allow { get; set; }

        public bool Buy => Allow.HasFlag(ShopAllow.Buy);
        public bool Sell => Allow.HasFlag(ShopAllow.Sell);

        public CurrencyType AllowedCurrency { get; set; }
        public ItemTag SellTags { get; set; }
        // Deduct 20% by default
        public int SellDeduction { get; set; } = 50;
    }

    public class CatalogGenerator
    {
        public CatalogGenerator() {}

        public CatalogGenerator(int size, int maxDiscountsAllowed = 0, int maxSpecialsAllowed = 0)
        {
            if (size <= 0)
                throw new ArgumentException("Cannot initialize a catalog with an empty or negative size.");

            Size = size;
            MaxDiscountsAllowed = maxDiscountsAllowed;
            MaxSpecialsAllowed = maxSpecialsAllowed;
            Entries = new List<CatalogEntry>();
        }

        public CatalogGenerator(int size, List<CatalogEntry> entries, int maxDiscountsAllowed = 0,
            int maxSpecialsAllowed = 0)
        {
            Size = size;

            if (!Check.NotNullOrEmpty(entries))
                throw new Exception("Expected at least entry for the catalog generator but returned empty");

            Entries = entries;
            MaxDiscountsAllowed = maxDiscountsAllowed;
            MaxSpecialsAllowed = maxSpecialsAllowed;
        }

        public int Size { get; internal set; }

        public int MaxDiscountsAllowed { get; internal set; }

        public int MaxSpecialsAllowed { get; internal set; }

        public List<CatalogEntry> Entries { get; internal set; }

        /// <summary>
        /// Generates a new <see cref="ItemCatalog"/>.
        /// </summary>
        public ItemCatalog Generate()
        {
            int specials = 0;
            int discounts = 0;
            var counters = new Dictionary<string, int>();
            var discountEntries = new Dictionary<string, int>();
            int entries = 0;

            if (Size <= 0)
                throw new ArgumentException("Cannot initialize a catalog with an empty or negative size.");

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
                        // NOTE: Since the upper bound is exclusive, add 1 to the max discount, so that it's possible to land on the specified max
                        int discountToApply = RandomProvider.Instance
                            .Next(entry.MinDiscount ?? CatalogEntry.DefaultMinDiscount, (entry.MaxDiscount ?? CatalogEntry.DefaultMaxDiscount) + 1);

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
            => Entries.Sum(x => x.Weight);

        private CatalogEntry[] GetAvailableEntries(int specials, IReadOnlyDictionary<string, int> counters)
        {
            return Entries.Where(x =>
                (specials < MaxSpecialsAllowed || !x.IsSpecial)
                && (!x.MaxAllowed.HasValue || counters.GetValueOrDefault(x.ItemId, 0) >= x.MaxAllowed)).ToArray();
        }

        private CatalogEntry GetNextEntry(int specials, IReadOnlyDictionary<string, int> counters)
        {
            CatalogEntry[] entries = GetAvailableEntries(specials, counters);
            int totalWeight = entries.Sum(x => x.Weight);
            int marker = RandomProvider.Instance.Next(0, totalWeight);
            int weightSum = 0;

            if (totalWeight == 0)
                return null;

            for (int i = 0; i < entries.Length; i++)
            {
                weightSum += entries[i].Weight;

                if (marker <= weightSum)
                    return Entries[i];
            }

            throw new Exception("Weighed marker was out of bounds.");
        }
    }

    /// <summary>
    /// Represents a collection of <see cref="Item"/> entries generated from a <see cref="CatalogGenerator"/>.
    /// </summary>
    public class ItemCatalog
    {
        public ItemCatalog() {}

        public ItemCatalog(Dictionary<string, int> itemIds, Dictionary<string, int> discounts)
        {
            ItemIds = itemIds?.Count > 0 ? itemIds : throw new Exception("Expected catalog to have at least a single item entry but returned empty");
            Discounts = discounts;
            GeneratedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new <see cref="ItemCatalog"/> that contains properties copied from the specified <see cref="ItemCatalog"/>.
        /// </summary>
        /// <param name="catalog">The <see cref="ItemCatalog"/> to create an instance of.</param>
        public ItemCatalog(ItemCatalog catalog)
        {
            ItemIds = catalog?.ItemIds.Count > 0
                ? new Dictionary<string, int>(catalog.ItemIds)
                : throw new Exception("Expected catalog to have at least a single item entry but returned empty");

            Discounts = catalog.Discounts.Count > 0
                ? new Dictionary<string, int>(catalog.Discounts)
                : new Dictionary<string, int>();

            GeneratedAt = catalog.GeneratedAt;
        }

        [JsonConstructor]
        public ItemCatalog(DateTime generatedAt, Dictionary<string, int> itemIds, Dictionary<string, int> discounts)
        {
            ItemIds = itemIds;
            Discounts = discounts;
            GeneratedAt = generatedAt;
        }

        [JsonProperty("generated_at")]
        public DateTime GeneratedAt { get; }

        [JsonProperty("item_ids")]
        public Dictionary<string, int> ItemIds { get; }

        [JsonProperty("discounts")]
        public Dictionary<string, int> Discounts { get; }

        [JsonIgnore]
        public int Count => ItemIds.Values.Sum();
    }

    public class CatalogEntry
    {
        public static readonly int DefaultMinDiscount = 1;
        public static readonly int DefaultMaxDiscount = 25;

        public string ItemId { get; set; }

        public int? MaxAllowed { get; set; }

        public int? MinDiscount { get; set; }

        public int? MaxDiscount { get; set; }

        public float DiscountChance { get; set; }

        public bool IsSpecial { get; set; }

        public int Weight { get; set; }
    }
}