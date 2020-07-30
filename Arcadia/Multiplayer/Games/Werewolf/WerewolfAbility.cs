using System;

namespace Arcadia.Multiplayer.Games
{
    [Flags]
    public enum WerewolfAbility
    {
        None = 0,
        Feast = 1, // on each night, every role with this ability chooses a player to kill
        Peek = 2, // on each night, this role can see if a player is a werewolf
        Protect = 4 // on each night, this role can choose something to protect
    }
}