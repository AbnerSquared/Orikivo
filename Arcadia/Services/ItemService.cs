using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcadia
{
    // TODO: This might be better than using static classes, compare later on
    public class ItemService
    {
        public List<Item> Items { get; set; }

        public IEnumerable<Item> Search(ItemTag tag)
        {
            return Items.Where(x => x.Tag.HasFlag(tag));
        }

        public Item GetItem(string id)
        {
            var items = Items.Where(x => x.Id == id);

            if (items.Count() > 1)
                throw new ArgumentException("There are more than one items with the specified ID.");

            return items.FirstOrDefault();
        }
    }
}
