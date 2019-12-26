using System;

namespace Orikivo
{
    /// <summary>
    /// An attribute that marks a command to use an alternate parameter separator.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ArgSeparatorAttribute : Attribute
    {
        public string Separator { get; }

        public ArgSeparatorAttribute(char separator)
        {
            Separator = separator.ToString();
        }

        public ArgSeparatorAttribute(string separator)
        {
            Separator = separator.ToString();
        }
    }
}
