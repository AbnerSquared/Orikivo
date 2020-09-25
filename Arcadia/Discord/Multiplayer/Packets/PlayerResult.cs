using System.Collections.Generic;

namespace Arcadia.Multiplayer
{
    public class PlayerResult
    {
        public long Money { get; set; }
        public ulong Exp { get; set; }
        public List<StatUpdatePacket> Stats { get; set; } = new List<StatUpdatePacket>();
        public List<ItemUpdatePacket> Items { get; set; } = new List<ItemUpdatePacket>();
    }
}
