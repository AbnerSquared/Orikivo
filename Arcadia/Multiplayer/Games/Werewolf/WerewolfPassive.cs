using System;

namespace Arcadia.Multiplayer.Games
{
    [Flags]
    public enum WerewolfPassive
    {
        None = 0,
        Tough = 1, // if attacked by wolves, they can live through the day, dying at the end of the night
        
        
        
        // This always forces the player to vote to live
        Pacifist = 2,

        // This always forces the player to vote for death
        Militarist = 4,

        // they can see everything that is happening, being able to write 1 letter at the start of each day
        Ghost = 8 
    }
}