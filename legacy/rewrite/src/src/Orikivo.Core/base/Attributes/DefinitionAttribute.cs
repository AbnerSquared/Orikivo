using System;

namespace Orikivo
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DefinitionAttribute : Attribute
    {
        public string Definition { get; }
        public DefinitionAttribute(string definition)
        {
            Definition = definition;
        }
    }
}
