using System;

namespace Arcadia.Games
{
    [Flags]
    public enum TriviaDifficulty
    {
        Easy,
        Medium,
        Hard,
        Any = Easy | Medium | Hard
    }
}
