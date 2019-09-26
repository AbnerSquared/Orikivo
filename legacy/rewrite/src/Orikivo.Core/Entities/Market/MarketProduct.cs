namespace Orikivo
{
    public class MarketProduct
    {
        public IItem Item {get; private set;} // the item for sale.
        public uint Amount {get; private set;} // the amount of items in stock.
    }
}