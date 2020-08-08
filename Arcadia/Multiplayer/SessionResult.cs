using System.Collections.Generic;

namespace Arcadia.Multiplayer
{
    // add custom criterion for an ArcadeUser needed to start a game.
    // this is the result of a GameSession that is used to update EXP, give rewards, etc.
    public class SessionResult
    {
        // the list of user IDs to update.
        public List<ulong> UserIds { get; set; } = new List<ulong>();
        public Dictionary<ulong, long> Money { get; } = new Dictionary<ulong, long>();
        public Dictionary<ulong, ulong> Exp { get; } = new Dictionary<ulong, ulong>();
        public Dictionary<ulong, List<StatUpdatePacket>> Stats { get; } = new Dictionary<ulong, List<StatUpdatePacket>>();
        public Dictionary<ulong, List<ItemUpdatePacket>> Items { get; } = new Dictionary<ulong, List<ItemUpdatePacket>>();

        public void Apply(ArcadeContainer container)
        {
            foreach (ulong userId in UserIds)
            {
                if (!container.Users.TryGet(userId, out ArcadeUser user))
                    continue;

                if (Money.ContainsKey(userId))
                    user.Give(Money[userId]);

                // NOTE: No exp yet, formulas not implemented

                if (Stats.ContainsKey(userId))
                    foreach (StatUpdatePacket packet in Stats[userId])
                        packet.Apply(user);

                if (Items.ContainsKey(userId))
                    foreach (ItemUpdatePacket packet in Items[userId])
                        ItemHelper.GiveItem(user, packet.Id, packet.Amount);

                container.Users.AddOrUpdate(userId, user);
            }
        }
    }
}
