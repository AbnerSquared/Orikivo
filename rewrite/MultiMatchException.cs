using System;
using System.Collections.Generic;

namespace Orikivo
{
    // An exception resulting from multiple search results stemming from a slim.
    /// <summary>
    /// Represents errors that occur when multiple matches from a search event were returned.
    /// </summary>
    public class MultiMatchException : Exception
    {
        public IEnumerable<object> Matches { get; }
        public MultiMatchException(string message = "") : base(message)
        {
        }

        public MultiMatchException(string message, IEnumerable<object> matches) : base(message)
        {
            Matches = matches;
        }
    }
}
