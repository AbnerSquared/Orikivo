using Discord.WebSocket;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    public static class AccountExtensions
    {
        public static async Task UpdateAsync(this OriUser user, AccountUpdatePacket packet)
        {
            foreach (StatUpdatePacket stat in packet.Stats)
            { }
            foreach (UpgradeUpdatePacket upgrade in packet.Upgrades)
            { }
            foreach (ItemUpdatePacket item in packet.Items)
            { }
            foreach (ExpUpdatePacket exp in packet.Exp)
            { }
            foreach (WalletUpdatePacket bal in packet.Balance)
            {
                
            }

            if (packet.Custom != null)
            {
                await packet.Custom.Invoke(user);
            }
        }
    }
}
