using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
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
            Slots = new List<ItemData>();
        }

        // the maximum amount of slots that a backpack can hold.
        public int Capacity { get; }

        public List<ItemData> Slots { get; }
        public Dictionary<string, int> ItemIds { get; }


        public List<Item> GetItems()
        {
            List<Item> items = new List<Item>();

            foreach (string itemId in ItemIds.Keys)
                items.Add(Engine.GetItem(itemId));

            return items;
        }

        public bool Contains(Item item)
            => Slots.Any(x => x.Id == item.Id);

        /// <summary>
        /// Returns a bool that determines if this <see cref="Backpack"/> can store another <see cref="Item"/>.
        /// </summary>
        /// <param name="item">The item that is going to be stored.</param>
        public bool CanFit(Item item)
        {
            if (Slots.Count + 1 <= Capacity)
                return true;

            foreach (ItemData data in Slots)
            {
                if (data.Id == item.Id)
                {
                    int maxStack = Engine.GetItem(item.Id).StackSize;

                    if (data.Count + 1 <= maxStack)
                        return true;

                    break;
                }
            }

            return false;
        }

        public void Store(Item item, int amount = 1)
        {
            int maxStack = Engine.GetItem(item.Id).StackSize;
            int remainder = amount;

            foreach (ItemData data in Slots)
            {
                if (data.Id == item.Id)
                {
                    if (data.Count + amount <= maxStack)
                    {
                        data.StackCount += amount;
                        return;
                    }
                    else
                    {
                        int remaining = maxStack - data.Count;
                        remainder = amount - remaining;
                        data.StackCount += remaining;
                    }

                    break;
                }
            }

            int slotSize = (int)Math.Ceiling((double)maxStack / remainder);

            if (Slots.Count + slotSize > Capacity)
                throw new Exception("This backpack does not have enough room to store the specified item.");
            else
            {
                for (int i = 0; i < slotSize; i++)
                {
                    // todo: figure out how to initialize unique item data
                    Slots.Add(new ItemData(item.Id, maxStack));
                }
            }
        }

        public void Remove(Item item, int amount = 1)
        {

        }
    }
}
