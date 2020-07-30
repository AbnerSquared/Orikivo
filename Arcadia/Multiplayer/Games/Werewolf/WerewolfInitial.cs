namespace Arcadia.Multiplayer.Games
{
    public enum WerewolfInitial
    {
        None = 0, // this role does not do anything at the start
        
        Copy = 1, // This role copies the role of the player they select

        Awake = 2 // This ability allows the player to look at their card AT THE END of the night
    }
}