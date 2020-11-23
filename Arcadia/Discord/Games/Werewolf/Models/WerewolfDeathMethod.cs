namespace Arcadia.Multiplayer.Games.Werewolf
{
    public enum WolfDeathMethod
    {
        Unknown = 0, // We aren't sure how the player died
        Hang = 1, // The player was hung by vote
        Wolf = 2,  // The player was killed by werewolves
        Hunted = 3, // The player was killed by the hunter
        Injury = 4 // The player has died from their injuries
    }
}
