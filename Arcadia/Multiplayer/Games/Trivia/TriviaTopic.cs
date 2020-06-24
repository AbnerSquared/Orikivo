using System;

namespace Arcadia.Games
{
    [Flags]
    public enum TriviaTopic
    {
        Math = 1,
        Gaming = 2,
        Any = Math | Gaming
    }
}
