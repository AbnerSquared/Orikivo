using System;

namespace Arcadia.Multiplayer.Games
{
    [Flags]
    public enum ChessMoveAction
    {
        Check = 1,
        Checkmate = 2,
        Capture = 4,
        EnPassant = 8
    }
}