using System;

namespace Orikivo
{
    /// <summary>
    /// Represents errors that occur when a search result yields no values.
    /// </summary>
    public class ResultNotFoundException : Exception
    {
        public ResultNotFoundException(string message = "") : base(message)
        { }
    }
}
