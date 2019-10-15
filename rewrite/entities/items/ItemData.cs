using System.Collections.Generic;

namespace Orikivo
{
    public class ItemData
    {
        public ItemData()
        {
            Info = new List<UniqueItemData>();
        }

        public int Count => StackCount ?? Info.Count;
        public int? StackCount { get; set; }

        /// <summary>
        /// A collection of each unique version of the specified item.
        /// </summary>
        public List<UniqueItemData> Info { get; private set; }
    }
}
