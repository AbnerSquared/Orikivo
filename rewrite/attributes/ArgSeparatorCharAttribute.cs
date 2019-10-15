using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    // This is used to alter the separator char on a command that is to be executed.
    /// <summary>
    /// An attribute that marks the command to use an alternate separator character as opposed to its default.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ArgSeparatorCharAttribute : Attribute
    {
        public char Separator { get; }
        public ArgSeparatorCharAttribute(char separator)
        {
            Separator = separator;
        }
    }
}
