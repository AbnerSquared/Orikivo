using System;

namespace Arcadia
{
    [Flags]
    public enum ExpiryTrigger
    {
        /// <summary>
        /// Expiration starts when first placed into an inventory.
        /// </summary>
        Own = 1,

        /// <summary>
        /// Expiration starts when the item is used for the first time.
        /// </summary>
        Use = 2,

        /// <summary>
        /// Expiration starts when the item is traded with for the first time.
        /// </summary>
        Trade = 4
    }
}
