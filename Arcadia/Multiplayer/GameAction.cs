using System;

namespace Arcadia
{
    // this should be a system in which a timer is included
    public class GameAction
    {
        // what is the name of this action
        public string Id { get; internal set; }

        // what to do when this is called
        public Action<GameServer, GameSession> OnExecute { get; set; }
    }
}
