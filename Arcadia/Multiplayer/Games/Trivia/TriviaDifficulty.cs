using System;

namespace Arcadia.Games
{
    [Flags]
    public enum TriviaDifficulty
    {
        Easy = 1,
        Medium = 2,
        Hard = 3,
        Any = Easy | Medium | Hard
    }
}
