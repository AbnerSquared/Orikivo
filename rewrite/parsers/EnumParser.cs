using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    public static class EnumParser
    {
        /// <summary>
        /// Attempts to loosely parse a matching enumeration from a specified string.
        /// </summary>
        public static bool TryParse<T>(string value, out T enumValue) where T : Enum
        {
            enumValue = default;
            foreach (string name in typeof(T).GetEnumNames())
            {
                if (value.ToLower() == name.ToLower())
                {
                    enumValue = (T)typeof(T).GetField(name).GetRawConstantValue();
                    return true;
                }
            }
            return false;
        }
    }
}
