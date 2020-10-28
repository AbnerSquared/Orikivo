using System.Collections.Generic;

namespace Arcadia
{
    /// <summary>
    /// Represents a base group for an <see cref="Item"/>.
    /// </summary>
    public class ItemGroup
    {
        // NOTE: This is the ID that is prefixed on items {short}_{id} su_pl  Summon: Pocket Lawyer
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

        /// <summary>
        /// Represents the name prefix for an <see cref="Item"/> in this group.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Represents the default summary for an <see cref="Item"/>, if left unspecified.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Represents a base collection of custom properties for an <see cref="Item"/> in this group.
        /// </summary>
        public List<ItemProperty> Properties { get; set; }

        /// <summary>
        /// Represents a collection of research tiers for this <see cref="ItemGroup"/>.
        /// </summary>
        public Dictionary<int, string> ResearchTiers { get; set; }

        // NOTE: This can be handled by 'catalog_group:item_group = research_tier'
        /// <summary>
        /// When true, allows group research for this <see cref="ItemGroup"/> (unimplemented).
        /// </summary>
        public bool CanResearch { get; set; }
    }
}
