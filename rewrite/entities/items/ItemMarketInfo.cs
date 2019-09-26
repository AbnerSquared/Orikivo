namespace Orikivo
{
    public class ItemMarketInfo
    {
        public ulong? Value { get; internal set; } // how much it's worth
        public double SellRate { get; internal set; } // the percent of its worth upon being sold
    }
}
