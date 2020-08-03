namespace Arcadia.Multiplayer.Games
{
    public enum WerewolfPhase
    {
        Unknown = 0,

        // Day phase: (Can point to: Night, Accuse)
        Day = 1,
        // Day sub-phases: Accuse, Trial, Defense, Vote

        Death = 2, // Death phase

        Night = 4 // Night phase
    }
}