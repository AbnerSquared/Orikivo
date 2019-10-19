using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// The data of a player that is used upon the construction of all players.
    /// </summary>
    public class BasePlayerData
    {
        /// <summary>
        /// The attributes to be passed onto each player.
        /// </summary>
        List<GameAttribute> Attributes { get; set; } = new List<GameAttribute>();
    }
}
