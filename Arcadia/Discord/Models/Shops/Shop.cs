using System;
using System.Collections.Generic;

namespace Arcadia
{
    /// <summary>
    /// Represents a market from which items can be purchased from.
    /// </summary>
    public class Shop
    {
        public string Id { get; set; }

        // What is the ID of the shop that this points to?
        // Once ToUnlock has been met, the previous shop will mention it to you the next time you leave
        public string ParentId { get; set; }

        public string Name { get; set; }

        public string Quote { get; set; }

        public CatalogGenerator Catalog { get; set; }

        // What is needed for the user to reach the specified tiers?
        public Dictionary<long, List<VarCriterion>> CriteriaTiers { get; set; }

        public ShopAllow Allow { get; set; }

        public bool Buy => Allow.HasFlag(ShopAllow.Buy);

        public bool Sell => Allow.HasFlag(ShopAllow.Sell);

        public CurrencyType AllowedCurrency { get; set; }

        public List<string> AllowedSellGroups { get; set; }

        public int? MaxAllowedPurchases { get; set; }

        public int SellDeduction { get; set; } = 50;

        public Func<ArcadeUser, bool> ToVisit { get; set; }
    }
}
