using System;

namespace Orikivo.Text
{
    [Flags]
    public enum Markdown
    {
        Bold = 1,
        Italics = 2,
        Strike = 4,
        Underline = 8,
        Code = 16,
        Quote = 32,
        Spoiler = 64
    }
}
