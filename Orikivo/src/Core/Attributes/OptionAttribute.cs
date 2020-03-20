using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    /// <summary>
    /// Represents an <see cref="Attribute"/> that marks a command with an option that can be specified to adjust how the command functions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class OptionAttribute : Attribute
    {
        public Type Type { get; }
        public string Name { get; }
        public List<string> Aliases { get; }

        // utilize the typereader classes
        public OptionAttribute(Type type, string name, params string[] aliases)
        {
            Type = type;
            Name = name;
            Aliases = aliases.ToList();
        }
    }
}
