using System;

namespace Orikivo
{
    /// <summary>
    ///     Marks the public identifier of a command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class IdentifierAttribute : Attribute
    {
        public uint Id { get; }

        /// <summary>
        ///     Marks the identifier of a command with the specified identifier.
        /// </summmary>
        /// <param name="id">The public identity of the object.</param>
        public IdentifierAttribute(uint id)
        {
            Id = id;
        }
    }
}