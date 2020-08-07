namespace Arcadia.Multiplayer.Games
{
    public enum WerewolfDeathMethod
    {
        Unknown = 0, // We aren't sure how the player died
        Hang = 1, // The player was hung by vote
        Wolf = 2,  // The player was killed by werewolves
        Hunted = 3, // The player was killed by the hunter
        Injury = 4
    }
}