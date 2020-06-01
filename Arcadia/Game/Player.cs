using Discord;
using System;
using System.Collections.Generic;

namespace Arcadia
{
    // likewise, the player should have a system that makes it easier to know if they are already in a server,
    // or if they were custom built
    public class Player
    {
        // who is my base user?
        public IUser User { get; set; }

        // is this player the host?
        public bool Host { get; set; }

        // is this player currently in-game?
        public bool Playing { get; set; }

        // maybe change where this is stored?
        // if i have a private display connection bound, what is it?
        public PlayerChannel Channel { get; set; }

        // when did this player join the server?
        // if the host is removed, the new host is the oldest person in the server
        public DateTime JoinedAt { get; set; }
    }
}
