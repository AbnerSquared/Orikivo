using System;
using System.Collections.Generic;

namespace Arcadia
{
    public class TradeOffer
    {
        public TradeOffer(ulong userId)
        {
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
        }

        public ulong UserId { get; }
        public DateTime CreatedAt { get; }
        public Dictionary<string, int> ItemIds { get; internal set; } = new Dictionary<string, int>();
        public Dictionary<string, int> RequestedItemIds { get; internal set; } = new Dictionary<string, int>();
    }
}