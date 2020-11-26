using System;

#pragma warning disable CS1998

namespace Orikivo
{
    /// <summary>
    /// Represents a parser for a <see cref="Type"/>.
    /// </summary>
    public abstract class TypeParser
    {
        public abstract bool TryParse(string input, out object result);
    }
}
