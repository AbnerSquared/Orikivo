using System;

namespace Orikivo.Text.Nodes
{
    [Flags]
    public enum Markdown
    {
        Bold,
        Italics,
        Strike,
        Underline,
        Code,
        Quote,
        Spoiler
    }
}
