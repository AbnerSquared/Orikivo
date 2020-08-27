using System;

namespace Arcadia.Graphics
{
    [Flags]
    public enum CursorOffset
    {
        None = 0,
        X = 1,
        Y = 2,
        XY = X | Y
    }
}