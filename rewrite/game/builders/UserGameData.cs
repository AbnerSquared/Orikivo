using System.Collections.Generic;

namespace Orikivo
{
    public class UserGameData
    {
        public ulong Id { get; } // user id
        public List<GameAttribute> Attributes { get; }
    }

    public class UserUpdatePacket
    {
        // The user to update. If left empty, it will default to the person that executed the trigger.
        ulong UserId { get; }
        public List<AttributeUpdatePacket> Packets { get; }
        public bool CanSpeak { get; } // If the user can send messages
    }
}
