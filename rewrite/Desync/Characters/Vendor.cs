using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class Vendor
    {
        public string Id { get; }
        public string Name { get; }

        public float SellRate { get; }
        public float BuyRate { get; }
        public List<ItemTag> LikedItemTags { get; }
        public List<ItemTag> DislikedItemTags { get; }
        public bool OnlyBuyLikedItems { get; }
        public List<TimeBlock> Schedule { get; }
    }
}
