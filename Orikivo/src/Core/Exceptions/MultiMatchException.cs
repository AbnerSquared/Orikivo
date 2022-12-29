using System;
using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// Represents errors that occur when multiple matches from a method were returned.
    /// </summary>
    public class MultiMatchException : Exception
    {
        public IEnumerable<object> Matches { get; }
        public MultiMatchException(string message = "") : base(message) { }

        public MultiMatchException(string message, IEnumerable<object> matches) : base(message)
        {
            Matches = matches;
        }
    }
}
