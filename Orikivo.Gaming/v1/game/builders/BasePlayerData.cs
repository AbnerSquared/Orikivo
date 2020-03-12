using System.Collections.Generic;

namespace Orikivo
{
    // TODO: Implement base mechanics on GameProperties constructor.
    /// <summary>
    /// The data of a player that is used for the construction of all players.
    /// </summary>
    public class BasePlayerData
    {
        /// <summary>
        /// The attributes to be passed onto each player.
        /// </summary>
        List<GameAttribute> Attributes { get; set; } = new List<GameAttribute>();
    }
}
