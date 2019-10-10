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
}
