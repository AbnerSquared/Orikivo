using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo
{
    ///<summary>
    /// A container bound to a game that handles what to display across each receiver.
    ///</summary>
    public class GameDisplay
    {
        // Goal: I want the windows to be updated separately. Since each window is a separate display type,
        // you want the windows to be explicitly called in order to update.
        private GameEventHandler _events;
        internal GameDisplay(string gameId, GameEventHandler events, GameWindowProperties windowProperties)
        {
            GameId = gameId;
            _events = events;
            windowProperties.Output = GameOutput.Game;
            var lobby = GameWindowProperties.Lobby;
            lobby.Output = GameOutput.Lobby;
            Windows.Add(new GameWindow(lobby));
            Windows.Add(new GameWindow(windowProperties));
        }

        public string GameId { get; }

        public string Name { get; }
        public string Id => $"{(Checks.NotNull(GameId) ? $"game.{GameId}:" : "")}tab.{Name}";


        /// <summary>
        /// A value defining if any of the updates committed are visible.
        /// </summary>
        private bool CanSeeUpdates = false;
        
        /// <summary>
        /// A list of game windows that are utilized.
        /// </summary>
        public List<GameWindow> Windows { get; } = new List<GameWindow>();

        /// <summary>
        /// Refreshes all connected receivers to the current display state.
        /// </summary>
        internal async Task RefreshAsync()
        {
           // if (CanSeeUpdates)
            //{
                await _events.InvokeDisplayUpdatedAsync(this);
                //CanSeeUpdates = false;
           // }
        }

        /// <summary>
        /// Updates a window with an update packet and refreshes the screen, if applicable.
        /// </summary>
        internal async Task<bool> UpdateWindowAsync(GameState state, WindowUpdatePacket packet)
        {
            bool result = UpdateWindow(state, packet);
            await RefreshAsync();
            return result;
        }

        /// <summary>
        /// Updates a window's tab with an update packet and refreshes the screen, if applicable.
        /// </summary>
        internal async Task<bool> UpdateWindowAsync(GameState state, TabUpdatePacket packet)

        {
            bool result = UpdateWindow(state, packet);
            await RefreshAsync();
            return result;
        }

        /// <summary>
        /// Updates a window's current tab with an element update packet and refreshes the screen, if applicable.
        /// </summary>
        internal async Task<bool> UpdateWindowAsync(GameState state, ElementUpdatePacket packet)
        {
            bool result = UpdateWindow(state, packet);
            await RefreshAsync();
            return result;
        }

        /// <summary>
        /// Sets a window's current tab to the one specified and refreshes the screen, if applicable.
        /// </summary>
        internal async Task<bool> UpdateWindowAsync(GameState state, string tabId)
        {
            SetTab(state, tabId);
            await RefreshAsync();
            return true;
        }

        /// <summary>
        /// Updates a window with an update packet.
        /// </summary>
        internal bool UpdateWindow(GameState state, WindowUpdatePacket packet)
            => GetWindow(state).Update(packet);

        ///<summary>
        /// Updates a game window with an update packet.
        ///</summary>
        internal bool UpdateWindow(WindowUpdatePacket packet)
            => GetWindow(packet.WindowId).Update(packet);

        /// <summary>
        /// Updates a window's tab with an update packet.
        /// </summary>
        internal bool UpdateWindow(GameState state, TabUpdatePacket packet)
        {
            GameWindow window = GetWindow(state);
            if (Checks.NotNull(packet.TabId))
                return window.GetTab(packet.TabId).Update(packet).IsSuccess;
            return window.CurrentTab.Update(packet).IsSuccess;
        }

        /// <summary>
        /// Updates a window's current tab with an element update packet.
        /// </summary>
        internal bool UpdateWindow(GameState state, ElementUpdatePacket packet)
            => GetWindow(state).CurrentTab.Update(packet).IsSuccess;

        /// <summary>
        /// Sets a window's current tab to the one specified.
        /// </summary>
        internal void SetTab(GameState state, string tabId)
        {
            this[state].SetCurrentTab(tabId);
        }

        /// <summary>
        /// Gets the game window matching the specified ID.
        /// </summary>
        internal GameWindow GetWindow(string id)
            => Windows.First(x => x.Id == id);

        /// <summary>
        /// Gets the game window matching the current game state specified.
        /// </summary>
        internal GameWindow GetWindow(GameState state)
            => this[state];

        internal GameWindow this[GameState state]
            => Windows.First(x => x.Output == (GameOutput)(int)state);

        internal GameWindow this[GameOutput output]
            => Windows.First(x => x.Output == output);
    }
}
