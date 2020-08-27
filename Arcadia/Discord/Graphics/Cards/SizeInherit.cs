using System;

namespace Arcadia.Graphics
{
    [Flags]
    public enum SizeInherit
    {
        None = 0,
        Width = 1,
        PaddingWidth = 2,
        Height = 4,
        PaddingHeight = 8,
        X = Width | PaddingWidth,
        Y = Height | PaddingHeight,
        XY = Width | PaddingWidth | Height | PaddingHeight
    }
}