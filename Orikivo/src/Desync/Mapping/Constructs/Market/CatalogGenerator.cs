using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a rough configuration for a <see cref="Market"/>'s catalog.
    /// </summary>
    public class CatalogGenerator
    {
        /// <summary>
        /// Represents the required tags an <see cref="Item"/> must have in order to be selected when generating a <see cref="Catalog"/>.
        /// </summary>
        public ItemTag RequiredTags { get; set; }

        public ItemType RequiredType { get; set; } = ItemType.Physical;

        // as long as one of these tags are on the item, it counts
        /// <summary>
        /// Represents the groups an <see cref="Item"/> must have at least one <see cref="ItemTag"/> matching for. If none are specified, the <see cref="Item"/> is only selected based on <see cref="RequiredTags"/>.
        /// </summary>
        public List<ItemTag> Groups { get; set; }

        // the most amount of items a market can hold at a time.
        /// <summary>
        /// Represents the most a <see cref="Catalog"/> can sell at a given time.
        /// </summary>
        public int Capacity { get; set; }

        // the most a market can have of ONE item.
        /// <summary>
        /// Represents the most a <see cref="Catalog"/> can generate of a single <see cref="Item"/>.
        /// </summary>
        public int MaxStack { get; set; }

        /// <summary>
        /// Generates a new <see cref="Catalog"/>.
        /// </summary>
        public Catalog Generate()
        {
            IEnumerable<Item> loot = Engine.Items
                .Where(delegate (KeyValuePair<string, Item> x)
                {
                    if (x.Value.Type != RequiredType)
                        return false;

                    if (RequiredTags != 0)
                        if (!x.Value.Tag.HasFlag(RequiredTags))
                            return false;

                    if (!(Groups?.Any(t => x.Value.Tag.HasFlag(t)) ?? true))
                        return false;

                    return true;
                }).Select(x => x.Value);

            List<Item> catalog = MaxStack <= 0 ? Randomizer.ChooseMany(loot, Capacity, true).ToList() :
                Randomizer.ChooseMany(loot, Capacity, MaxStack).ToList();

            Dictionary<Item, int> items = new Dictionary<Item, int>();

            foreach (Item item in catalog)
            {
                if (items.ContainsKey(item))
                    continue;

                items.Add(item, catalog.Where(x => x == item).Count());
            }

            return new Catalog(items);
        }
    }
}
