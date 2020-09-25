using System.Collections.Generic;

namespace Arcadia.Multiplayer
{
    public interface IPlayer
    {
        Player Source { get; }

        List<GameProperty> GetProperties();
    }
}