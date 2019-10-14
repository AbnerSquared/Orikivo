using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// An active user within a game.
    /// </summary>
    public class Player
    {
        public ulong Id { get; }
        public string Name { get; }
        public List<GameAttribute> Attributes { get; }
    }
}
