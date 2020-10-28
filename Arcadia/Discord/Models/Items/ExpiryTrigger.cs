using System;

namespace Arcadia
{
    /// <summary>
    /// Represents a collection of triggers for an <see cref="Item"/> expiry.
    /// </summary>
    [Flags]
    public enum ExpiryTrigger
    {
        /// <summary>
        /// Expiration starts when an <see cref="Item"/> is first placed into an inventory.
        /// </summary>
        Own = 1,

        /// <summary>
        /// Expiration starts when an <see cref="Item"/> is used for the first time.
        /// </summary>
        Use = 2,

        /// <summary>
        /// Expiration starts when an <see cref="Item"/> is traded with for the first time.
        /// </summary>
        Trade = 4
    }
}
