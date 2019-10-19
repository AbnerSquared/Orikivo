using System.Collections.Generic;

namespace Orikivo
{
    // this is the root game client data that is passed down to each game client.
    public class GameData
    {
        // attributes and triggers can be left empty.
        internal GameData(List<Player> players, List<GameAttribute> attributes = null, List<GameTrigger> triggers = null)
        {
            Attributes = attributes ?? new List<GameAttribute>();
            Players = players;
            Triggers = triggers ?? new List<GameTrigger>();
        }
        
        public string TaskId { get; internal set; }
        public List<GameAttribute> Attributes { get; }
        public List<Player> Players { get; }
        public List<GameTrigger> Triggers { get; }
    }
}
