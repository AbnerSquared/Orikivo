using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class Vendor
    {
        string Id;
        string Name;
        Dictionary<string, VendorDialogue> Dialogue { get; } = new Dictionary<string, VendorDialogue>();
        float SellRate;
        float BuyRate;
        List<WorldItemTag> LikedItemTags;
        List<WorldItemTag> DislikedItemTags;
        bool OnlyBuyLikedItems;
        List<TimeBlock> Schedule;
    }
}
