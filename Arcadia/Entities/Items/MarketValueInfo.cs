namespace Arcadia
{
    public class MarketValueInfo
    {
        public CurrencyType Currency { get; set; }
        public long Value { get; set; }
        public bool CanBuy { get; set; }
        public bool CanSell { get; set; }
    }
}