using System;

namespace Orikivo
{
    // TODO: figure out how to utilize this argument separator
    // in its actual purpose
    // a custom command parser might very well be required.
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
            Separator = separator;
        }
    }
}
