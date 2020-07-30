using System;

namespace Arcadia.Multiplayer.Games
{
    [Flags]
    public enum WerewolfPassive
    {
        None = 0,
        Tough = 1, // if attacked by wolves, they can live through the day, dying at the end of the night
        
        // if a hunter is about to be hanged, they can choose someone to kill
        // if a hunter was marked for death, they can choose someone to kill before they die
        Hunt = 2,
        
        // This always forces the player to vote to live
        Pacifist = 4,

        // This always forces the player to vote for death
        Militarist = 8,

        // they can see everything that is happening, being able to write 1 letter at the start of each day
        Ghost = 16 
    }
}