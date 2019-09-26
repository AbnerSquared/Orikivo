using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    public class EnumParser
    {
        public static bool TryParseEnum<T>(string value, out T enumValue) where T : Enum
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
