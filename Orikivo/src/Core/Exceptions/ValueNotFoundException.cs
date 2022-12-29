using System;

namespace Orikivo
{
    /// <summary>
    /// The exception that is thrown when a search result fails to find a matching value.
    /// </summary>
    public class ValueNotFoundException : Exception
    {
        public ValueNotFoundException(string message, string input) : base($"{message}{(Check.NotNull(input) ? $"\nInput: {input}" : "")}")
        {
            Input = input;
        }

        public ValueNotFoundException(string message = "") : base(message) { }

        public string Input { get; }
    }
}
