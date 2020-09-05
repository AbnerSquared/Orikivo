namespace Arcadia.Multiplayer.Games.Werewolf
{

    // If shared peeking is used instead, Ignore listing out peek
    // Otherwise, ignore their own UserId

    public enum WerewolfPeekMode
    {
        Hidden = 1, // The default: All of the information a seer is given stays with them
        Player = 2, // When a seer identifies someone, the person they chose is publicly announced at the start of a day (ONLY THE PLAYER)
        Role = 3 // When a seer identifies someone, everyone will be told if they selected a werewolf or not
    }
}
