using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    // make this a [Flags], which can use Enum.HasFlag(Enum.Value);
    // defines all the possible mods a parameter might have
    public enum ParameterMod
    {
        Optional = 1,
        Trailing = 2,
        Mentionable = 3
    }
}
