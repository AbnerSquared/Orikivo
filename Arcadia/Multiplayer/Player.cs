using Discord;
using System;
using System.Threading.Tasks;

namespace Arcadia.Multiplayer
{
    // likewise, the player should have a system that makes it easier to know if they are already in a server,
    // or if they were custom built

    /// <summary>
    /// Represents a player connection to a <see cref="GameServer"/>.
    /// </summary>
    public class Player
    {
        public Player()
        {
            //Channel = new PlayerChannel(User);
        }

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

        public PlayerChannel Channel { get; }
    }

    public class PlayerChannel
    {
        public IUser User { get; }
        public IUserMessage LastMessage;

        public async Task ReplaceAsync()
        {

        }

        public async Task DeleteLastAsync()
        {

        }

        public async Task SendAsync()
        {

        }
    }
}
