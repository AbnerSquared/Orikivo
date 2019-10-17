using System.Collections.Generic;

namespace Orikivo
{
    ///<summary>
    /// A packet that contains information on what to update within a game window tab.
    ///</summary>
    public class TabUpdatePacket
    {
        public TabUpdatePacket(string tabId = null, List<ElementUpdatePacket> packets = null)
        {
            TabId = tabId;
            Packets = packets ?? new List<ElementUpdatePacket>();
        }

        ///<summary>
        /// The tab to be updated. If left empty, it will default to the current tab.
        ///</summary>
        string TabId { get; set; }

        ///<summary>
        /// A collection of elements to be updated within a tab.
        ///</summary>
        List<ElementUpdatePacket> Packets { get; } = new List<ElementUpdatePacket>();
    }
}
