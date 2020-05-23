using System;

namespace Orikivo
{
    /// <summary>
    /// An <see cref="Attribute"/> that stores a short description for the command it is applied to.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DescriptionAttribute : Attribute
    {
        public DescriptionAttribute(string content)
        {
            Content = content;
        }

        public string Content { get; }
    }
}
