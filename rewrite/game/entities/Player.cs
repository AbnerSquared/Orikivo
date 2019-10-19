using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// An active user within a game.
    /// </summary>
    public class Player
    {
        public ulong UserId { get; }
        public string Id => $"user.{UserId}";
        public string Name { get; }
        public List<GameAttribute> Attributes { get; }
        public bool CanSpeak { get; internal set; }
    }
}
