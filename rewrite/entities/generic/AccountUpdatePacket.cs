using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orikivo
{
    /// <summary>
    /// Represents an update packet for an OriUser.
    /// </summary>
    public class AccountUpdatePacket
    {
        /// <summary>
        /// Returns an update packet for an account derived from a game result.
        /// </summary>
        /// <param name="result"></param>
        public static AccountUpdatePacket FromGameResult(GameResult result)
        {
            AccountUpdatePacket packet = new AccountUpdatePacket();
            /* Calculate what values from GameResult are utilized here. */
            return packet;
        }

        private AccountUpdatePacket() { }

        // TODO: Learn how to properly assign a customized action onto here.
        /// <summary>
        /// An optional customized action that can be done on the user.
        /// </summary>
        public Func<OriUser, Task> Custom { get;  internal/*private*/ set; }

        public List<UpgradeUpdatePacket> Upgrades { get; private set; } = new List<UpgradeUpdatePacket>();
        public List<ItemUpdatePacket> Items { get; private set; } = new List<ItemUpdatePacket>();
        //IReadOnly
        public List<StatUpdatePacket> Stats { get; private set; } = new List<StatUpdatePacket>();

        public List<ExpUpdatePacket> Exp { get; private set; } = new List<ExpUpdatePacket>();

        public List<WalletUpdatePacket> Balance { get; private set; } = new List<WalletUpdatePacket>();

    }
}
