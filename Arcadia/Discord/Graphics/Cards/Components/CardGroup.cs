using System;

namespace Arcadia.Graphics
{
    [Flags]
    public enum CardGroup
    {
        Username = 1,
        Activity = 2,
        Avatar = 4,
        Level = 8,
        Money = 16,
        Exp = 32,
        Bar = 64
    }
}
