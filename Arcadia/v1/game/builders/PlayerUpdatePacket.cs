using System.Collections.Generic;

namespace Arcadia.Old
{
    public class PlayerUpdatePacket
    {
        // The user to update. If left empty, it will default to the person that executed the trigger.
        ulong Id { get; }
        public List<AttributeUpdatePacket> Packets { get; }
        public bool CanSpeak { get; } // If the user can send messages
    }
}
