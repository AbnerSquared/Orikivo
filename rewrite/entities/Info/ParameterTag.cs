using System;

namespace Orikivo
{
    [Flags]
    public enum ParameterTag
    {
        Optional = 1,
        Trailing = 2,
        Mentionable = 4
    }
}
