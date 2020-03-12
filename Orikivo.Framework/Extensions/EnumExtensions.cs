using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    public static class EnumExtensions
    {
        public static List<TEnum> GetValues<TEnum>(this TEnum @enum)
            where TEnum : Enum
            => typeof(TEnum).GetEnumValues().Cast<TEnum>().ToList();

        public static List<TEnum> GetFlags<TEnum>(this TEnum @enum)
            where TEnum : Enum
        {
            List<TEnum> flags = new List<TEnum>();
            foreach (TEnum flag in GetValues(@enum))
            {
                if (@enum.HasFlag(flag))
                    flags.Add(flag);
            }

            return flags;
        }
    }
}
