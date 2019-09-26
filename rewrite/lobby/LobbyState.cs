namespace Orikivo
{
    public enum LobbyState
    {
        Open = 1, // the lobby can be joined
        InProgress = 2, // the lobby is currently in the middle of playing a game
        Closed = 3 // the lobby has been shut down, and can no longer be joined.
    }
}
