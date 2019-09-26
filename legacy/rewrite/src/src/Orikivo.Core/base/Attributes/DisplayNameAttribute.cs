using System;

namespace Orikivo
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DisplayNameAttribute : Attribute
    {
        public string Name { get; }
        public DisplayNameAttribute(string name)
        {
            Name = name;
        }
    }
}
