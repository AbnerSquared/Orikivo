using System;

namespace Orikivo.Net
{
    /// <summary>
    /// Defines the key and value of a query argument.
    /// </summary>
    public struct QueryArg
    {
        /// <summary>
        /// Creates a new QueryArg with a specified key and value.
        /// </summary>
        internal QueryArg(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("A key or value that was specified is null.");
            
            Key = key;
            Value = value;
        }

        /// <summary>
        /// The key used to specify the argument.
        /// </summary>
        public string Key { get; internal set; }

        /// <summary>
        /// The value used to define the argument.
        /// </summary>
        public string Value { get; internal set; }

        /// <summary>
        /// Returns a string formatted query argument.
        /// </summary>
        public override string ToString()
            => $"{Key}={Value}";
    }
}
