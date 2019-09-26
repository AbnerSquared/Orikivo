using System.Collections.Generic;

namespace Orikivo
{
    public class ItemData
    {
        public ItemData()
        {
            Info = new List<UniqueItemInfo>();
        }

        public int Count { get { return StackCount ?? Info.Count; } }
        public int? StackCount { get; set; }
        public List<UniqueItemInfo> Info { get; private set; } // used if each item is different
    }
}
