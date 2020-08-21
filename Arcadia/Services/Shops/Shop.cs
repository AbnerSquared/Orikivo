using System.Collections.Generic;

namespace Arcadia
{
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
}