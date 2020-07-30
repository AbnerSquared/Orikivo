using System;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents an error that occurs when attempting to get the value of <see cref="GameProperty"/> that does not match the specified type.
    /// </summary>
    public class PropertyReferenceException : Exception
    {
        public PropertyReferenceException(string message) : base(message)
        {

        }
    }
}