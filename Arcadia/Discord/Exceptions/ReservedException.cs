using System;

namespace Arcadia.Multiplayer
{
    public class ReservedException : Exception
    {
        public ReservedException(ulong entityId, EntityType type)
            : base($"Cannot initialize a new server as the specified {type.ToString().ToLower()} {entityId} is already reserved to an existing server")
        {
            EntityId = entityId;
            Type = type;
        }

        public ulong EntityId { get; }
        public EntityType Type { get; }
    }
}