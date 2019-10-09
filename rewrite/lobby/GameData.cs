using System.Collections.Generic;

namespace Orikivo
{
    // this is the root game client data that is passed down to each game client.
    public class GameData
    {
        // attributes and triggers can be left empty.
        internal GameData(List<GameAttribute> attributes, List<UserGameData> userData, List<GameTrigger> triggers)
        {
            Attributes = attributes;
            UserData = userData;
            Triggers = triggers;
        }
        public List<GameAttribute> Attributes { get; }
        public List<UserGameData> UserData { get; }
        public List<GameTrigger> Triggers { get; }
    }
}
