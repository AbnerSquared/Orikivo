using System;

namespace Orikivo
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    internal class TitleAttribute : Attribute
    {
        public TitleAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
