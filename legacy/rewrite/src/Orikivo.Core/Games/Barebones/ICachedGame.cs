using System.Collections.Generic;

namespace Orikivo
{
    public interface ICachedGame
    {
        string GameId {get;}
        List<IGameUser> Players {get; set;}
    }
}