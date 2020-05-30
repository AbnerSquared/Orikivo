using System;

namespace Arcadia

{
    [Flags]
    public enum ReactionFlag
    {
        Add = 1,
        Remove = 2,
        Any = Add | Remove
    }
}
