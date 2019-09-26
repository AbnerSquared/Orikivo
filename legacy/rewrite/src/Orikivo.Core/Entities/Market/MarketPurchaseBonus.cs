using System;

namespace Orikivo
{
    public class MarketPurchaseBonus
    {
        public ushort ItemId {get; private set;} // the item id that this affects
        public double Bonus {get; private set;} // the amount of bonus appended.
        public DateTime Launch {get; private set;} // the time that the bonus started.
        public TimeSpan Duration {get; private set;} // the duration of the bonus.
        public TimeSpan TimeLeft {get;} // the time left for the bonus.
    }
}