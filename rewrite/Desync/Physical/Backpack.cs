using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class Backpack
    {
        public Backpack(int capacity)
        {
            Capacity = capacity;
            Items = new List<Item>(capacity);
        }
        // the maximum amount of slots that a backpack can hold.
        public int Capacity { get; }
        public List<Item> Items { get; }

    }
}
