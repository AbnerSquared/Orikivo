using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a generator for an <see cref="Item"/> list.
    /// </summary>
    public class CatalogGenerator : IGenerationTable
    {
        /// <summary>
        /// Represents the required tags an <see cref="Item"/> must have in order to be selected when generating a <see cref="Catalog"/>.
        /// </summary>
        public ItemType RequiredTags { get; set; }

        /// <summary>
        /// Represents the dimension that this generator is meant for.
        /// </summary>
        public ItemDimension Dimension { get; set; } = ItemDimension.Physical;

        // TODO: Use a single version instead of List<T>
        // as long as one of these tags are on the item, it counts
        /// <summary>
        /// Represents the groups an <see cref="Item"/> must have at least one <see cref="ItemTag"/> matching for. If none are specified, the <see cref="Item"/> is only selected based on <see cref="RequiredTags"/>.
        /// </summary>
        public List<ItemType> Groups { get; set; }

        // the most amount of items a market can hold at a time.
        /// <summary>
        /// Represents the most a <see cref="Catalog"/> can hold at a given time.
        /// </summary>
        public int Size { get; set; }

        public int MaxDiscountsAllowed { get; set; }

        public int MaxSpecialsAllowed { get; set; }

        public List<CatalogEntry> Entries { get; set; }

        IEnumerable<ITableEntry> IGenerationTable.Entries => Entries;

        // the most a market can have of ONE item.
        /// <summary>
        /// Represents the most a <see cref="Catalog"/> can generate of a single <see cref="Item"/>.
        /// </summary>
        public int MaxStack { get; set; }

        // This generates based on criteria for the entire item index.
        /// <summary>
        /// Generates a new <see cref="Catalog"/>.
        /// </summary>
        public Catalog Generate()
        {
            IEnumerable<Item> loot = Engine.Items
                .Where(delegate (KeyValuePair<string, Item> x)
                {
                    if (x.Value.Dimension != Dimension)
                        return false;

                    if (RequiredTags != 0)
                    {
                        if (!x.Value.Tag.HasFlag(RequiredTags))
                            return false;
                    }

                    if (!(Groups?.Any(t => x.Value.Tag.HasFlag(t)) ?? true))
                        return false;

                    return true;

                }).Select(x => x.Value);

            List<Item> catalog = MaxStack <= 0 ?
                Randomizer.ChooseMany(loot, Size, true).ToList() :
                Randomizer.ChooseMany(loot, Size, MaxStack).ToList();

            var items = new Dictionary<Item, int>();

            foreach (Item item in catalog)
            {
                if (items.ContainsKey(item))
                    continue;

                items.Add(item, catalog.Where(x => x == item).Count());
            }

            return new Catalog(items);
        }


        // This generates a catalog based on the specified entries given.
        public Catalog GenerateNew()
        {
            int specials = 0;
            int discounts = 0;

            var counters = new Dictionary<string, int>();


            var entries = new List<Item>();

            // the float is the % that is taken off.
            // 1.0 - discount = % value.
            var discountEntries = new Dictionary<string, float>();

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
                }

                if (entry.CanDiscount)
                {
                    if (!discountEntries.ContainsKey(entry.ItemId) && discounts < MaxDiscountsAllowed)
                    {
                        int discountToApply = RandomProvider.Instance.Next(0, entry.MaxPossibleDiscount);

                        float rawDiscount = discountToApply / (float) 100;

                        discountEntries.Add(entry.ItemId, rawDiscount);
                        discounts++;
                    }
                }

                if (!Engine.Items.ContainsKey(entry.ItemId))
                    throw new System.Exception("The specified item ID could not be found.");

                if (counters.ContainsKey(entry.ItemId))
                {
                    counters[entry.ItemId]++;
                }
                else
                {
                    counters.Add(entry.ItemId, 1);
                }

                entries.Add(Engine.GetItem(entry.ItemId));
            }

            Catalog catalog = new Catalog(entries.ToDictionary(
                key => key,
                value => entries.Where(x => x == value).Count()), discountEntries);

            return catalog;
        }

        private CatalogEntry GetNextEntry()
        {
            int totalWeight = Entries.Select(x => x.Weight).Sum();

            int marker = RandomProvider.Instance.Next(0, totalWeight);

            int weightSum = 0;

            for (int i = 0; i < Entries.Count; i++)
            {
                weightSum += Entries[i].Weight;

                if (marker <= weightSum)
                    return Entries[i];
            }

            throw new System.Exception("Weighed marker was out of bounds.");

        }
    }

    public class CatalogEntry : ITableEntry
    {
        public string ItemId { get; set; }

        public ItemType Groups { get; set; }

        // The most amount of times this entry can be chosen.
        public int? MaxAllowed { get; set; }

        // If this entry can be discounted
        public bool CanDiscount { get; set; }

        // If this entry can be discounted, determines the most this item can be discounted for.
        public int MaxPossibleDiscount { get; set; }

        // If this entry is a special item
        public bool IsSpecial { get; set; }

        // The weight it has when being chosen.
        public int Weight { get; set; }
    }
}
