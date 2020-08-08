using System;

namespace Arcadia.Multiplayer.Games
{
    [Flags]
    public enum WerewolfGroup
    {
        Unknown = 0,

        Villager = 1,

        Tanner = 2,

        Werewolf = 4
    }
}