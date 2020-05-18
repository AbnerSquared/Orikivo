using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    // TODO: Make the market system a more generic structure, so Vendors can be portable Markets.

    /// <summary>
    /// Represents a worker for a specific <see cref="Market"/>.
    /// </summary>
    public class Vendor : Npc
    {
        public List<ItemType> LikedTags { get; set; }
        public List<ItemType> DislikedTags { get; set; }
        public bool OnlyBuyLiked { get; set; }
        public Schedule Schedule { get; set; }
    }
}
