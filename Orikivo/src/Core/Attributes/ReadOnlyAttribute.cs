using System;

namespace Orikivo
{
    // This attribute is primarily used on configuration properties, where the value cannot be changed by users.
    /// <summary>
    /// Marks a property to be read-only.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ReadOnlyAttribute : Attribute { }
}