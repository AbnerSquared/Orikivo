using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    public class GameWindow
    {
        public string Id { get; } // screen:{id}
        // the channel id that the current display is set to.
        public string CurrentTabId { get; set; }
        // what this window's output is for.
        public GameOutput Output { get; }
        public GameTab DefaultTab { get; }
        public List<GameTab> Tabs { get; } = new List<GameTab>();
        public GameTab CurrentTab => this[CurrentTabId];

        // public void AddTab(){}
        // public void RemoveTab(){}
        public void SetCurrentTab(string tabId)
        {
            if (!Tabs.Any(x => x.Id == tabId))
                throw new Exception("There are no tabs that correspond to that id.");
            CurrentTabId = tabId;
        }
        public GameTab this[string id]
            => Tabs.First(x => x.Id == id);

        // Make it the parsing content.
        public string Content { get; }
        public string SyncKey { get; private set; }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
