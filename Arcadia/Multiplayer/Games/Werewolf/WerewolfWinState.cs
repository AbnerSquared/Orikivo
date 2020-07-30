namespace Arcadia.Multiplayer.Games
{
    public enum WerewolfWinState
    {
        // The game is still going
        Pending = 0,

        // The villagers win
        Villager = 1,

        // The werewolves win
        Wolf = 2
    }
}