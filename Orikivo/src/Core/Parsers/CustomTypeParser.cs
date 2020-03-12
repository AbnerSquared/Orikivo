using System;

#pragma warning disable CS1998

namespace Orikivo
{
    /// <summary>
    /// Represents an abstract parser for custom <see cref="Type"/>.
    /// </summary>
    public abstract class CustomTypeParser
    {
        public abstract bool TryParse(string input, out object result);
    }
}
