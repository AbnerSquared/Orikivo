using System;

namespace Arcadia.Multiplayer.Games
{
    /// <summary>
    /// Defines a collection of possible events that occurred during a move in Chess.
    /// </summary>
    [Flags]
    public enum ChessEvents
    {
        Check = 1,
        Checkmate = 2,
        Capture = 4,
        EnPassant = 8,
        Castle = 16
    }
}
