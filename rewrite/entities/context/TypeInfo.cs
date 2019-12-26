using System;

namespace Orikivo
{
    public class TypeInfo
    {
        public TypeInfo(Type type, string summary)
        {
            Type = type;
            Summary = summary;
        }

        public Type Type { get; }
        public string Summary { get; }
    }
}
