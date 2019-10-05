using System;
using System.Collections.Generic;

namespace Orikivo
{
    // this defines what the game needs before actually launching.
    public class GameProperties
    {
        public static GameProperties FromMode(GameMode mode)
        {
            throw new NotImplementedException();
        }

        public List<GameAttribute> Attributes { get; set; } // the root list of attributes.
        public List<GameTask> Tasks { get; set; } // the list of tasks.
    }
}
