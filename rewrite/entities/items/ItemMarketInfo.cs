namespace Orikivo
{
    /// <summary>
    /// Defines how the item is handled on the market.
    /// </summary>
    public class ItemMarketInfo
    {
        /// <summary>
        /// Defines the value of an item. If left empty, it is assumed to be unmarketable.
        /// </summary>
        public ulong? Value { get; internal set; } // how much it's worth

        public bool? CanBuy { get; internal set; }
        public bool? CanSell { get; internal set; }

        // TODO: Have the buy rate decrease with new criteria.
        /// <summary>
        /// The rate that is used when purchasing the item. If left empty, it defaults to 1.0.
        /// </summary>
        public double? BuyRate { get; internal set; }

        /// <summary>
        /// The rate that is used when selling the item. If left empty, it defaults to 1.0.
        /// </summary>
        public double? SellRate { get; internal set; } // the percent of its worth upon being sold
    }
}
