using System;
using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// Represents an update packet for an OriUser.
    /// </summary>
    public class AccountUpdatePacket
    {
        // TODO: Incorporate GameResult into this update packet. This way, all of the things that can be updated can go here.
        public AccountUpdatePacket() { throw new NotImplementedException(); }

        public ItemCustomAction? CustomAction { get; internal set; }

        public List<UpgradeUpdatePacket> Upgrades { get; internal set; } = new List<UpgradeUpdatePacket>();
        public List<ItemUpdatePacket> Items { get; internal set; } = new List<ItemUpdatePacket>();

        public List<StatUpdatePacket> Stats { get; internal set; } = new List<StatUpdatePacket>();

        public List<ExpUpdatePacket> Exp { get; internal set; } = new List<ExpUpdatePacket>();

        public List<BalanceUpdatePacket> Balance { get; internal set; } = new List<BalanceUpdatePacket>();

    }
}
