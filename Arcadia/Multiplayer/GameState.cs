﻿namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Defines the current state of a <see cref="ServerConnection"/>.
    /// </summary>
    public enum GameState
    {
        Waiting = 1, 

        Editing = 2,

        Watching = 4,

        Playing = 8
    }
}
