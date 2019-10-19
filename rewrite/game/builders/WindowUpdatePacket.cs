using System.Collections.Generic;

namespace Orikivo
{
    ///<summary>
    /// An update packet that contains information on what to do within a game window.
    ///</summary>
    public class WindowUpdatePacket
    {
        public WindowUpdatePacket(GameOutput output, string toTabId = null, List<TabUpdatePacket> packets = null)
        {
            Output = output;
            ToTabId = toTabId;
            Packets = packets ?? new List<TabUpdatePacket>();
        }

        public WindowUpdatePacket(string windowId, string toTabId = null, List<TabUpdatePacket> packets = null)
        {
            WindowId = windowId;
            ToTabId = toTabId;
            Packets = packets ?? new List<TabUpdatePacket>();
        }

        ///<summary>
        /// Defines the output type that correlates to the window. This can be used in place of an ID.
        ///</summary>
        public GameOutput Output { get; set; }

        ///<summary>
        /// The game window to be updated.
        ///</summary>
        public string WindowId { get; set; }

        ///<summary>
        /// The new tab to display within the window. If left empty, it will be left at the current tab.
        ///</summary>
        public string ToTabId { get; set; }

        ///<summary>
        /// A collection of tab update packets to utilize when called.
        ///</summary>
        public List<TabUpdatePacket> Packets { get; } = new List<TabUpdatePacket>();
    }
}
