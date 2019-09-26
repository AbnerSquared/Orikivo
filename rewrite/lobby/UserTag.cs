using System;

namespace Orikivo
{
    [Flags]
    public enum UserTag
    {
        None = 0, // the user is not doing anything
        Host = 1, // if the user is the host of the lobby.
        Playing = 2, // if the user is currently in a game.
        Spectating = 3 // if the user is currently spectating.
    }
}
