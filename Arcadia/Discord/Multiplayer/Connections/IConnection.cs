using System;
using System.Threading.Tasks;
using Discord;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents a generic connection to a <see cref="GameServer"/>.
    /// </summary>
    public interface IConnection
    {
        // The time at which this connection was established
        DateTime CreatedAt { get; }

        // The type of connection that was established
        ConnectionType Type { get; }
        
        /// <summary>
        /// Represents the message that this <see cref="IConnection"/> references.
        /// </summary>
        IUserMessage Message { get; }
        
        /// <summary>
        /// Represents the channel that this <see cref="IConnection"/> is bound to.
        /// </summary>
        IMessageChannel Channel { get; }

        // Deletes the message for this connection and clears all of them
        Task DestroyAsync();

        // Refreshes this connection for the specified server
        Task RefreshAsync(GameServer server);
    }
}