using System;

namespace Orikivo
{
    /// <summary>
    /// Marks a context to be ignored when read.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class IgnoreAttribute : Attribute
    {
        public IgnoreAttribute()
        {
            Usage = IgnoreUsage.All;
        }

        public IgnoreAttribute(IgnoreUsage usage)
        {
            Usage = usage;
        }

        public IgnoreUsage Usage { get; }
    }
}
