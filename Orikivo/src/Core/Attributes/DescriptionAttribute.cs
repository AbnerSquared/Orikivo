using System;

namespace Orikivo
{
    /// <summary>
    /// An <see cref="Attribute"/> that stores a short description for the value it is applied to.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DescriptionAttribute : Attribute
    {
        public DescriptionAttribute(string subtitle)
        {
            Subtitle = subtitle;
        }

        public string Subtitle { get; }
    }
}
