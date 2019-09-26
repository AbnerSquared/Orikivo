using System.Collections.Generic;

namespace Orikivo
{
    public class GameLobby
    {
        public IGame Game {get; set;}
        public List<IGameUser> Players {get; set;}
        //public IGameSession Start() { return new GameSession(); }
    }
}