using System.Collections.Generic;

namespace Orikivo.Unstable
{
    /// <summary>
    /// Represents a rough configuration for a <see cref="Market"/>'s catalog.
    /// </summary>
    public class LootTable
    {
        /// <summary>
        /// Represents the required tags an <see cref="Item"/> must have in order to be selected when generating a <see cref="MarketCatalog"/>.
        /// </summary>
        public ItemTag RequiredTags { get; set; }

        // as long as one of these tags are on the item, it counts
        /// <summary>
        /// Represents the groups an <see cref="Item"/> must have at least one <see cref="ItemTag"/> matching for. If none are specified, the <see cref="Item"/> is only selected based on <see cref="RequiredTags"/>.
        /// </summary>
        public List<ItemTag> Groups { get; set; }

        // the most amount of items a market can hold at a time.
        /// <summary>
        /// Represents the most a <see cref="MarketCatalog"/> can sell at a given time.
        /// </summary>
        public int Capacity { get; set; }

        // the most a market can have of ONE item.
        /// <summary>
        /// Represents the most a <see cref="MarketCatalog"/> can generate of a single <see cref="Item"/>.
        /// </summary>
        public int MaxStack { get; set; }
    }
}
