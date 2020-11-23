using Discord;
using System;

namespace Arcadia.Multiplayer
{
    public class Player : Receiver
    {
        internal Player(GameServer server, IUser user) : base(user.GetOrCreateDMChannelAsync().ConfigureAwait(false).GetAwaiter().GetResult())
        {
            Server = server;
            User = user;
            JoinedAt = DateTime.UtcNow;
        }

        public IUser User { get; }

        public DateTime JoinedAt { get; }

        public bool IsHost => Server.HostId == User.Id;

        public bool Playing { get; internal set; }

        public DateTime LastSpoke { get; internal set; }

        public DateTime LastInviteSent { get; internal set; }

        public GameServer Server { get; }
    }
}
