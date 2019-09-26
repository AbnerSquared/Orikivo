using System;

namespace Orikivo
{
    public class MarketProductDiscount
    {
        public ushort ItemId {get; private set;} // the id of the item that is on discount
        public double DiscountValue {get; private set;} // the % off of the item.
        public DateTime Launch {get; private set;} // the duration of this discount.
        public TimeSpan Duration {get; private set;}
        //public TimeSpan TimeLeft {get { return (Launch.Add(Duration) - Launch) - DateTime.Now; }} // the amount of time left.
    }
}