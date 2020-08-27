using System.Collections.Generic;

namespace Arcadia.Multiplayer
{
    public class SessionResult
    {
        public Dictionary<ulong, PlayerResult> UserIds { get; set; } = new Dictionary<ulong, PlayerResult>();

        public void Apply(ArcadeContainer container)
        {
            foreach ((ulong userId, PlayerResult result) in UserIds)
            {
                if (!container.Users.TryGet(userId, out ArcadeUser user))
                    continue;

                if (result.Money > 0)
                    user.Give(result.Money);

                // NOTE: No exp yet, formulas not implemented
                foreach (StatUpdatePacket packet in result.Stats)
                    packet.Apply(user);

                foreach (ItemUpdatePacket packet in result.Items)
                    ItemHelper.GiveItem(user, packet.Id, packet.Amount);

                container.Users.AddOrUpdate(userId, user);
            }
        }
    }
}
