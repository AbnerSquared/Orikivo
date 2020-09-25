using Discord;
using System;
using System.Threading.Tasks;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents a player connection to a <see cref="GameServer"/>.
    /// </summary>
    public class Player
    {
        internal Player(GameServer server, IUser user)
        {
            Server = server;
            User = user;
            JoinedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Represents the <see cref="DateTime"/> at which this <see cref="Player"/> joined a <see cref="GameServer"/>.
        /// </summary>
        public DateTime JoinedAt { get; }

        /// <summary>
        /// Represents the <see cref="IUser"/> that this <see cref="Player"/> originates from.
        /// </summary>
        public IUser User { get; }

        /// <summary>
        /// If true, this <see cref="Player"/> is the host of this <see cref="GameServer"/>.
        /// </summary>
        public bool Host => Server.HostId == User.Id;

        /// <summary>
        /// If true, this <see cref="Player"/> is currently playing a game.
        /// </summary>
        public bool Playing { get; internal set; }

        /// <summary>
        /// Represents the <see cref="DateTime"/> at which this <see cref="Player"/> has last spoken.
        /// </summary>
        public DateTime LastSpoke { get; internal set; }

        public DateTime LastInviteSent { get; internal set; }

        /// <summary>
        /// Represents the <see cref="GameServer"/> that this <see cref="Player"/> is currently in.
        /// </summary>
        public GameServer Server { get; }

        /// <summary>
        /// Gets the direct message channel for this <see cref="Player"/>, if any.
        /// </summary>
        public PlayerChannel Channel { get; private set; }

        /// <summary>
        /// Gets or creates the direct message channel for this <see cref="Player"/>.
        /// </summary>
        public async Task<PlayerChannel> GetOrCreateChannelAsync()
        {
            return Channel ??= await PlayerChannel.CreateAsync(User);
        }
    }
}
