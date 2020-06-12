using System.Collections.Generic;

namespace Arcadia.Old
{
    // Defines an update packet for the currently running game.
    public class GameUpdatePacket
    {
        public GameUpdatePacket(List<AttributeUpdatePacket> attributePackets = null, List<WindowUpdatePacket> windowPackets = null)
        {
            AttributePackets = attributePackets ?? new List<AttributeUpdatePacket>();
            WindowPackets = windowPackets ?? new List<WindowUpdatePacket>();
        }

        public List<AttributeUpdatePacket> AttributePackets { get; set; }
        public List<WindowUpdatePacket> WindowPackets { get; set; }

    }
}
