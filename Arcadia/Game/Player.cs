using Discord;

namespace Arcadia

{
    public class Player
    {
        // who is my base user?
        public IUser User { get; }

        // is this player the host?
        public bool Host { get; set; }

        // if i have a private display connection bound, what is it?
        public PlayerConnection Connection { get; set; }
    }
}
