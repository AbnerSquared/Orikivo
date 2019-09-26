using System.Collections.Generic;

namespace Orikivo
{
    public class MarketStock
    {
        public List<MarketProductDiscount> Discounts {get; private set;}
        public List<MarketProduct> Products {get; private set;}
    }
}
