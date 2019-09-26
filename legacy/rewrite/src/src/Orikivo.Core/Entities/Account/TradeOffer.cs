using System.Collections.Generic;

namespace Orikivo
{
    public class TradeOffer
    {
        public ulong Sender { get; set; } // sender id
        public ulong Recipient { get; set; } // Recipient id
        public List<Item> Inbound { get; set; } // sender's items
        public ulong? InboundWallet { get; set; }
        public List<Item> Outbound { get; set; } // Recipient's items
        public ulong? OutboundWallet { get; set; }
    }
}
