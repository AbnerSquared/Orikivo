using System;
using System.Collections.Generic;

namespace Orikivo
{
    public class Market
    {
        // these help define the market.
        public List<Vendor> Vendors {get; private set;} // the collection of people that work here each day.
        public MarketScheduleBlock Schedule {get; private set;} // the schedule that this market is set to follow.
        public MarketCharacteristicBlock Character {get; private set;} // the loadout of the market.
        public MarketDesignBlock Design {get; private set;} // the design of the market.
        public MarketStockType Type {get; private set;} // the type of market that this is, defining what items are sold.

        public MarketStock Stock {get; private set;} // the collection of items current in the market, alongside count.
        public DateTime TimeLeft {get;} // the time left in the store.
    }

    public class MarketScheduleBlock { }
    public class MarketCharacteristicBlock { }
    public class MarketDesignBlock { }
    
}