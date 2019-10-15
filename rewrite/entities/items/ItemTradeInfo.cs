namespace Orikivo
{
    /// <summary>
    /// Defines how an item is handled when trading.
    /// </summary>
    public class ItemTradeInfo
    {
        /// <summary>
        /// The amount of times an item can be traded until it trade-locks. If left empty, it will default to no limit.
        /// </summary>
        public int? MaxTrades { get; } = null;
    }
}
