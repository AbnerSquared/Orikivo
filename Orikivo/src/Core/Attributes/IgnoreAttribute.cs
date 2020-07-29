using System;

namespace Orikivo
{
    /// <summary>
    /// Marks a context to be ignored when read.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Method)]
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
