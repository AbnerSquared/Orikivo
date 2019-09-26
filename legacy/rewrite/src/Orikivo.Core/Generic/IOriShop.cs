using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    // random rarity formula

    // shop that sells items..


    public class OriShop
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("slots")]
        public ushort Slots { get; set; }
        [JsonProperty("time_block")]
        public DateTimeRange TimeBlock { get; set; } // enforce open/close times; otherwise open 24/7.

        [JsonProperty("group")]
        public OriItemGroupType LootGroup { get; set; }
        [JsonProperty("vendor")]
        public OriShopVendor Vendor { get; set; } // the vendor at the store; otherwise empty
    }

    public class OriShopVendor
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("reply")]
        public OriShopVendorReplyBlock Responses { get; set; }
    }

    public class OriShopVendorReplyBlock
    {

        [JsonProperty("on_enter")]
        public List<string> OnShopEntry { get; set; }
        [JsonProperty("on_buy")]
        public List<string> OnItemBuy { get; set; }
        [JsonProperty("on_exit")]
        public List<string> OnShopExit { get; set; }
    }

    // all available items that can be sold at this store.
}
