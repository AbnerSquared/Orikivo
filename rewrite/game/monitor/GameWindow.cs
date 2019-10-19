using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{

    public class GameWindow
    {
        public GameWindow(GameWindowProperties properties)
        {
            if (properties == null)
                return;

            Name = properties.Name;
            CurrentTabId = properties.CurrentTabId;
            InternalTabs = properties.Tabs;
            Output = properties.Output;
        }
        public string Id => $"window.{Name}"; // screen:{id}
                                  // the channel id that the current display is set t
        public string Name { get; }
        public string CurrentTabId { get; set; }
        // what this window's output is for.
        public GameOutput Output { get; }
        public GameTab DefaultTab { get; }
        public IReadOnlyList<GameTab> Tabs => InternalTabs;
        private List<GameTab> InternalTabs { get; } = new List<GameTab>();
        public GameTab CurrentTab => this[CurrentTabId];


        // Figure out if you want the game window to be able to insert new tab screens.
        /*
        public bool AddTab(GameTab tab)
        {
            tab.WindowId = Id;
            Tabs.Add(tab);
            return true;
        }*/

        //public void RemoveTab(string tabId) {}

        //public void ClearTab(string tabId) {}

        public void SetCurrentTab(string tabId)
        {
            if (!InternalTabs.Any(x => x.Id == tabId))
                throw new Exception("There are no tabs that correspond to that ID.");
            CurrentTabId = tabId;
        }

        public bool Update(WindowUpdatePacket packet)
        {
            if (packet.ToTabId != null)
                SetCurrentTab(packet.ToTabId);

            foreach (TabUpdatePacket tabPacket in packet.Packets)
                if (!CurrentTab.Update(tabPacket).IsSuccess)
                    return false;

            return true;
        }

        public GameTab GetTab(string id)
            => this[id];

        public GameTab this[string id]
            => InternalTabs.First(x => x.Id == id);

        // Make it the parsing content.
        public string Content => CurrentTab.Content;
        public string SyncKey { get; private set; }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
