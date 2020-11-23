using System;

namespace Arcadia.Multiplayer.Games.Werewolf
{
    // Abilities are something that requires their own private channel
    [Flags]
    public enum WolfAbility
    {
        Feast = 1, // on each night, every role with this ability chooses a player to kill
        Peek = 2, // on each night, this role can see if a player is a werewolf
        Protect = 4, // on each night, this role can choose something to protect
        // if a hunter is about to be hanged, they can choose someone to kill
        // if a hunter was marked for death, they can choose someone to kill before they die
        Hunt = 8
    }
}
