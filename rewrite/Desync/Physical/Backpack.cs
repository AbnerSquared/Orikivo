using System.Collections.Generic;

namespace Orikivo.Unstable
{
    /// <summary>
    /// Represents a <see cref="Husk"/>'s physical storage container.
    /// </summary>
    public class Backpack
    {
        public Backpack(int capacity)
        {
            Capacity = capacity;
            ItemIds = new Dictionary<string, int>(capacity);
        }
        // the maximum amount of slots that a backpack can hold.
        public int Capacity { get; }
        public Dictionary<string, int> ItemIds { get; }


        public List<Item> GetItems()
        {
            List<Item> items = new List<Item>();

            foreach (string itemId in ItemIds.Keys)
                items.Add(WorldEngine.GetItem(itemId));

            return items;
        }

    }
}
