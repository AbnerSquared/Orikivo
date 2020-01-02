using System;

namespace Orikivo
{
    [Flags]
    public enum ParameterMod
    {
        Optional = 1,
        Trailing = 2,
        Mentionable = 4
    }
}
