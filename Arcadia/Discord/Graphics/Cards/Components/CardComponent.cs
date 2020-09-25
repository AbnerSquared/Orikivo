using System;
using System.Drawing;

namespace Arcadia.Graphics
{
    [Flags]
    public enum CardComponent
    {
        Username = 1,
        Activity = 2,
        Avatar = 4,
        Level = 8,
        Money = 16,
        Exp = 32,
        Bar = 64
    }

    // Likewise, this is also handled by Merit slots

    // Get the fill space of text by simply requesting the opacity mask
    // Set up the fill design by creating a bitmap of the same width and height
}
