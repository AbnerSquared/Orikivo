﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo
{
    // container for game displays.
    public class GameMonitor
    {
        private GameEventHandler _events;
        internal GameMonitor(GameEventHandler events, GameMode mode)
        {
            List<GameDisplay> displays = new List<GameDisplay>();
            _events = events;
            GameDisplay lobbyDisplay = new GameDisplay(GameState.Inactive);
            lobbyDisplay.Append("**Lobby**");
            GameDisplay gameDisplay = new GameDisplay(GameState.Active);
            gameDisplay.Append("**Game**");
            displays.Add(lobbyDisplay);
            displays.Add(gameDisplay);
            Displays = displays;
        }
        List<GameDisplay> Displays { get; }

        internal async Task UpdateDisplayAsync(GameState state, string content)
        {
            this[state].Append(content);
            await _events.InvokeDisplayUpdatedAsync(this[state]);
        }

        internal GameDisplay this[GameState state]
        {
            get
            {
                return Displays.First(x => x.Type == state);
            }
        }
    }

    // remove 2 when rewritten.
    public class GameMonitor2
    {
        private GameEventHandler _events;
        internal GameMonitor2(string gameId, GameEventHandler events)
        {
            Id = gameId;
            _events = events;
        }
        public string Id { get; }
        // a list of windows that a Game can set to.
        public List<GameWindow> Windows { get; } = new List<GameWindow>();
        internal async Task UpdateTabAsync(GameState state, ElementUpdatePacket packet)
        {
            ElementUpdateResult result = this[state].CurrentTab.Update(packet);
            if (!result.IsSuccess)
                throw new Exception("An error has occured while updating an element.");
            //await _events.InvokeWindowUpdatedAsync(this[state]);
        }

        // internal async Task UpdateWindowAsync(GameOutput output, ElementUpdatePacket packet) {}
        internal async Task SetTabAsync(GameState state, string tabId)
        {
            this[state].SetCurrentTab(tabId);
            //await _events.InvokeWindowUpdatedAsync(this[state]);
        }

        internal GameWindow this[GameState state]
            => Windows.First(x => x.Output == (GameOutput)(int)state);

        internal GameWindow this[GameOutput output]
            => Windows.First(x => x.Output == output);
    }
}
