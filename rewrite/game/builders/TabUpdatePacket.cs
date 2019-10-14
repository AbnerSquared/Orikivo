using System.Collections.Generic;

namespace Orikivo
{
    // contains what to do on a tab.
    public class TabUpdatePacket
    {
        // apply the tabId in monitor update packet.
        string TabId { get; set; } // the tab to update. leave null for currentTabId

        // a list defining a collection of elements to be updated on a tab.
        List<ElementUpdatePacket> Packets { get; } = new List<ElementUpdatePacket>();
    }
}
