namespace Arcadia.Multiplayer.Games
{
    public enum WerewolfGroup
    {
        Unknown = 0,

        Villager = 1,
        
        // If their group is tanner, the only win condition is that they are hung
        Tanner = 2,
        
        Werewolf = 3
    }
}