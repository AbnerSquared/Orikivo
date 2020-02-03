using System;

namespace Orikivo
{
    /// <summary>
    /// Marks a property to be skipped whenever a class is read.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ReadIgnoreAttribute : Attribute {}
}
