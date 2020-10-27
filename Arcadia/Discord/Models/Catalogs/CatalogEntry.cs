namespace Arcadia
{
    /// <summary>
    /// Represents an item entry for a <see cref="CatalogGenerator"/>.
    /// </summary>
    public class CatalogEntry
    {
        public static readonly int DefaultMinDiscount = 1;
        public static readonly int DefaultMaxDiscount = 25;

        public CatalogEntry() { }

        public CatalogEntry(string itemId, int weight, int? maxAllowed = null)
        {
            ItemId = itemId;
            Weight = weight;
            MaxAllowed = maxAllowed;
        }

        /// <summary>
        /// Specifies the item ID for this <see cref="CatalogEntry"/>.
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// Specifies the <see cref="CatalogGroup"/> for this <see cref="CatalogEntry"/> (optional).
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// Determines the maximum allowed amount of times this <see cref="CatalogEntry"/> can be selected.
        /// </summary>
        public int? MaxAllowed { get; set; }

        /// <summary>
        /// Represents the minimum discount that can be applied to this <see cref="CatalogEntry"/> (0% minimum).
        /// </summary>
        public int? MinDiscount { get; set; }

        /// <summary>
        /// Represents the maximum discount that can be applied to this item (100% maximum).
        /// </summary>
        public int? MaxDiscount { get; set; }

        /// <summary>
        /// Represents the base chance that this <see cref="CatalogEntry"/> can receive a discount.
        /// </summary>
        public float DiscountChance { get; set; }

        /// <summary>
        /// Determines the minimum tier that is required for this <see cref="CatalogEntry"/> be included.
        /// </summary>
        public int RequiredTier { get; set; } = 1;

        /// <summary>
        /// If true, excludes this <see cref="CatalogEntry"/> from being selected if the specified tier does not match <see cref="RequiredTier"/>.
        /// </summary>
        public bool ForceAtTier { get; set; }

        // TODO: Store if an item is special based on its relative chance to appear when compared to other entries.
        /// <summary>
        /// Determines if this <see cref="CatalogEntry"/> is special.
        /// </summary>
        public bool IsSpecial { get; set; }

        /// <summary>
        /// Represents the base chance for this <see cref="CatalogEntry"/> to be selected.
        /// </summary>
        public int Weight { get; set; }
    }
}
