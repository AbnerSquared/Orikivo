using System;

namespace Arcadia.Multiplayer.Games.Werewolf
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
