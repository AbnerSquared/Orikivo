using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo
{
    // remove 2 when rewritten.
    ///<summary>
    /// A container bound to a game that handles what to display across each receiver.
    ///</summary>
    public class GameDisplay
    {
        // Goal: I want the windows to be updated separately. Since each window is a separate display type,
        // you want the windows to be explicitly called in order to update.
        private GameEventHandler _events;
        internal GameDisplay(string gameId, GameEventHandler events)
        {
            Id = gameId;
            _events = events;
        }
        public string Id { get; }
        private bool CanSeeUpdates = false;
        // a list of windows that a Game can set to.
        public List<GameWindow> Windows { get; } = new List<GameWindow>();

        // Calls to update all displays
        internal async Task RefreshAsync()
        {
           // if (CanSeeUpdates)
            //{
                await _events.InvokeDisplayUpdatedAsync(this);
                CanSeeUpdates = false;
           // }
        }

        internal async Task<bool> UpdateWindowAsync(GameState state, WindowUpdatePacket packet)
        {
            bool result = UpdateWindow(state, packet);
            await RefreshAsync();
            return result;
        }
        internal async Task<bool> UpdateWindowAsync(GameState state, TabUpdatePacket packet)

        {
            bool result = UpdateWindow(state, packet);
            await RefreshAsync();
            return result;
        }
        internal async Task<bool> UpdateWindowAsync(GameState state, ElementUpdatePacket packet)
        {
            bool result = UpdateWindow(state, packet);
            await RefreshAsync();
            return result;
        }

        internal async Task<bool> UpdateWindowAsync(GameState state, string tabId)
        {
            SetTab(state, tabId);
            await RefreshAsync();
            return true;
        }

        ///<summary>
        /// Updates the current game state bound to a window with an update packet.
        ///</summary>
        internal bool UpdateWindow(GameState state, WindowUpdatePacket packet)
            => GetWindow(state).Update(packet);

        ///<summary>
        /// Updates a game window with an update packet.
        ///</summary>
        internal bool UpdateWindow(WindowUpdatePacket packet)
            => GetWindow(packet.WindowId).Update(packet);

        internal bool UpdateWindow(GameState state, TabUpdatePacket packet)
        {
            GameWindow window = GetWindow(state);
            if (Checks.NotNull(packet.TabId))
                return window.GetTab(packet.TabId).Update(packet);
            return window.CurrentTab.Update(packet);
        }

        internal bool UpdateWindow(GameState state, ElementUpdatePacket packet)
            => GetWindow(state).CurrentTab.Update(packet).IsSuccess;

        // internal async Task UpdateWindowAsync(GameOutput output, ElementUpdatePacket packet) {}
        internal void SetTab(GameState state, string tabId)
        {
            this[state].SetCurrentTab(tabId);
        }

        internal GameWindow GetWindow(string id)
            => Windows.First(x => x.Id == id);

        internal GameWindow GetWindow(GameState state)
            => this[state];

        internal GameWindow this[GameState state]
            => Windows.First(x => x.Output == (GameOutput)(int)state);

        internal GameWindow this[GameOutput output]
            => Windows.First(x => x.Output == output);
    }
}
