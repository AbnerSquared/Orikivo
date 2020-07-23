using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    public static class EnumExtensions
    {
        public static IEnumerable<TEnum> GetValues<TEnum>(this TEnum @enum)
            where TEnum : Enum
            => typeof(TEnum).GetEnumValues().Cast<TEnum>().ToList();

        public static IEnumerable<TEnum> GetActiveFlags<TEnum>(this TEnum @enum)
            where TEnum : Enum
        {
            foreach (TEnum flag in GetValues(@enum))
            {
                if (@enum.HasFlag(flag))
                    yield return flag;
            }
        }
    }
}
