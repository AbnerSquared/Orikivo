using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// An active user within a game.
    /// </summary>
    public class Player
    {
        public Player(ulong userId, string name, List<GameAttribute> attributes = null)
        {
            UserId = userId;
            Name = name;
            Attributes = attributes ?? new List<GameAttribute>();
        }

        public int Index { get; internal set; }
        
        public ulong UserId { get; }
        
        public string Id => $"user.{UserId}";
        
        public string Name { get; }
        
        public List<GameAttribute> Attributes { get; }
        
        public bool CanSpeak { get; internal set; } = true;
    }
}
