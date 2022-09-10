using System;
using System.Threading.Tasks;
using Discord;

namespace Arcadia.Multiplayer
{
    // To make connections platform-independant, the message/channel grouping needs to be removed.
    // Instead, the connection should store the last updated text value that was updated

    /// <summary>
    /// Represents a generic connection to a <see cref="GameServer"/>.
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Represents the time at which this connection was established.
        /// </summary>
        DateTime CreatedAt { get; }

        // Deletes the message for this connection and clears all of them
        Task DestroyAsync();

        // Refreshes this connection for the specified server
        Task RefreshAsync(GameServer server);
    }
}
