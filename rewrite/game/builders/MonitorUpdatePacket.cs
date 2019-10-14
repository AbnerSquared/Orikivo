using System.Collections.Generic;

namespace Orikivo
{
    // contains what to do upon a monitor.
    public class MonitorUpdatePacket
    {
        // the window to display
        string ToWindowId { get; } // the GameWindow to be displayed. Leave empty to leave the window as is
        string ToTabId { get; } // the GameTab to be displayed. Leave empty to leave the tab as is

        // a list defining a collection of tabs to be updated.
        public List<TabUpdatePacket> Packets { get; }
    }
}
