namespace Orikivo
{
    /*
     
         upgrades manage booster data.
         Value Booster: Increases the returning value of an item.
         
         */
    public class Item
    {
        public Rarity Rarity { get; set; } // the item rarity.
        public bool Limited { get; set; } // if the item was limited.
        public bool Consumable { get; set; } // if the item is a one-time use.
        public bool Tradable { get; set; } // if the item is tradable
        public string Name { get; set; } // the item's name.
        public ulong Cost { get; set; } // the item's cost.
        //public ulong Value { get { return Cost / ValueBooster } }
    }
}
