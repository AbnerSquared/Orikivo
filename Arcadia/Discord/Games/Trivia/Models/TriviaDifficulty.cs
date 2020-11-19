using System;

namespace Arcadia.Multiplayer.Games
{
    [Flags]
    public enum TriviaDifficulty
    {
        Easy = 1,
        Medium = 2,
        Hard = 3,
        Any = Easy | Medium | Hard
    }

    // The reason this is being made is to combine support with OpenTDB's ID/Name system
}
