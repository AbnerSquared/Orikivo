using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    public static class EnumUtils
    {
        public static List<TEnum> GetValues<TEnum>() where TEnum : Enum
            => typeof(TEnum).GetEnumValues().Cast<TEnum>().ToList();
    }
}
