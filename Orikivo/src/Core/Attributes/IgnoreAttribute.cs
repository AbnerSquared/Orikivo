using System;

namespace Orikivo
{
    /// <summary>
    /// Marks a value to be ignored when read.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class IgnoreAttribute : Attribute
    {

    }
}
