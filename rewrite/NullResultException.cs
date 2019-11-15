using System;

namespace Orikivo
{
    /// <summary>
    /// Represents errors that occur when a search result yields no values.
    /// </summary>
    public class NullResultException : Exception
    {
        public NullResultException(string message = "") : base(message)
        { }
    }
}
