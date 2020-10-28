using System.Collections.Generic;

namespace Arcadia
{
    /// <summary>
    /// Represents a base group for an <see cref="Item"/>.
    /// </summary>
    public class ItemGroup
    {
        // This is the ID that is prefixed on items
        public string ShortId { get; set; }

        /// <summary>
        /// Represents the unique identifier for this <see cref="ItemGroup"/>.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Represents the default icon for an <see cref="Item"/> in this group.
        /// </summary>
        public Icon Icon { get; set; }

        /// <summary>
        /// Represents the default rarity for an <see cref="Item"/> in this group.
        /// </summary>
        public ItemRarity Rarity { get; set; }

        /// <summary>
        /// Represents the display name of this <see cref="ItemGroup"/>.
        /// </summary>
        public string Name { get; set; }

        // This is applied to the name before the name of the item
        /// <summary>
        /// Represents the name prefix for an <see cref="Item"/> in this group.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Represents the default summary for an <see cref="Item"/>, if left unspecified.
        /// </summary>
        public string Summary { get; set; }

        public Dictionary<string, long> Attributes { get; set; }

        public Dictionary<int, string> ResearchTiers { get; set; }

        // This can be handled by catalog_group:item_group: research_tier
        /// <summary>
        /// When true, allows group research for this <see cref="ItemGroup"/> (unimplemented).
        /// </summary>
        public bool CanResearch { get; set; }
    }
}
