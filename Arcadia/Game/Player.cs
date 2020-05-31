using Discord;
using System.Collections.Generic;

namespace Arcadia
{

    public class Player
    {
        // who is my base user?
        public IUser User { get; }

        // is this player the host?
        public bool Host { get; set; }

        // is this player currently in-game?
        public bool Playing { get; set; }

        // maybe change where this is stored?
        // if i have a private display connection bound, what is it?
        public PlayerChannel Channel { get; set; }
    }
}
