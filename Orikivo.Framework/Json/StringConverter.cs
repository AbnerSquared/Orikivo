using System;

namespace Orikivo.Framework.Json
{
    public class StringConverter<T>
    {
        public Func<string, T> Deserializer { get; set; }
        public Func<T, string> Serializer { get; set; }
    }
}
