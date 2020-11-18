using System;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents errors that occur when an entity is already reserved in a game.
    /// </summary>
    public class ReservedException : Exception
    {
        public ReservedException(ulong entityId, EntityType type)
            : base($"Cannot initialize a new server as the specified {type.ToString().ToLower()} {entityId} is already reserved to an existing server")
        {
            EntityId = entityId;
            EntityType = type;
        }

        public ulong EntityId { get; }

        public EntityType EntityType { get; }
    }
}
