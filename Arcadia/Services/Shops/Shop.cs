using System;
using System.Collections.Generic;
using System.Linq;
using Orikivo;
using Orikivo.Drawing;

namespace Arcadia
{
    public class Shop
    {
        public string Id { get; set; }

        public string Name { get; set; }
        
        public Sprite Image { get; set; }
        
        public Sprite Interior { get; set; }
        
        // the time frame at which they are open on each day.
        public Orikivo.Desync.TimeBlock Schedule { get; set; }

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

                        float rawDiscount = discountToApply / (float) 100;

                        discountEntries.Add(entry.ItemId, rawDiscount);
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
        public DateTime GeneratedAt { get; set; }

        public Dictionary<Item, int> Items { get; set; }
        
        public Dictionary<string, float> Discounts { get; set; }
        
        public int Count => Items.Values.Sum();

        // Maybe introduce compression?
    }

    public class CatalogEntry
    {
        public string ItemId { get; set; }
        public ItemTag Tag { get; set; }
        public int? MaxAllowed { get; set; }

        // if unspecified, discounts are not allowed.
        public int? MaxDiscount { get; set; }

        public bool IsSpecial { get; set; }

        public int Weight { get; set; }
    }
}