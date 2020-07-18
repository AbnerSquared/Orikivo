using Newtonsoft.Json;
using System;

namespace Arcadia
{
    public class ItemAction
    {
        internal ItemAction() { }

        // The amount of times that this item can be used.
        public int? Durability { get; set; }

        public TimeSpan? Cooldown { get; set; }

        // What happens when this is used?
        public Action<ArcadeUser> OnUse { get; set; }

        // What happens when this item is broken?
        // This can be used to apply debuffs.
        public Action<ArcadeUser> OnBreak { get; set; }

        // Is this item deleted when broken?
        public bool DeleteOnBreak { get; set; }
    }
}
