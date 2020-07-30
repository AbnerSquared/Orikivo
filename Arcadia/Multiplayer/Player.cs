using Discord;
using System;
using System.Threading.Tasks;

namespace Arcadia.Multiplayer
{
    // likewise, the player should have a system that makes it easier to know if they are already in a server,
    // or if they were custom built

    /// <summary>
    /// Represents a generic player connection to a <see cref="GameServer"/>.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Represents the <see cref="IUser"/> that this <see cref="Player"/> originates from.
        /// </summary>
        public IUser User { get; set; }

        /// <summary>
        /// If true, this <see cref="Player"/> is the host of this <see cref="GameServer"/>.
        /// </summary>
        public bool Host { get; set; }

        /// <summary>
        /// If true, this <see cref="Player"/> is currently playing a game.
        /// </summary>
        public bool Playing { get; set; }

        /// <summary>
        /// Represents the <see cref="DateTime"/> at which this <see cref="Player"/> joined a <see cref="GameServer"/>.
        /// </summary>
        public DateTime JoinedAt { get; set; }

        /// <summary>
        /// Establishes and returns a connection to the direct channel of this <see cref="Player"/>.
        /// </summary>
        /// <param name="display">Represents the <see cref="DisplayChannel"/> to establish the <see cref="ServerConnection"/> with.</param>
        /// <param name="properties">Represents the properties that the <see cref="ServerConnection"/> will inherit.</param>
        /// <returns>A new <see cref="ServerConnection"/> to the specified <see cref="Player"/>.</returns>
        public async Task<ServerConnection> GetConnectionAsync(DisplayChannel display, ConnectionProperties properties = null)
        {
            return await ServerConnection.CreateAsync(this, display);
        }
    }
}
