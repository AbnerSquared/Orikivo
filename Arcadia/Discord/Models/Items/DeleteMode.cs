using System;

namespace Arcadia
{
    /// <summary>
    /// Specifies a collection of delete triggers for an <see cref="Item"/>.
    /// </summary>
    [Flags]
    public enum DeleteMode
    {
        /// <summary>
        /// The <see cref="Item"/> is deleted when it is broken (durability is 0).
        /// </summary>
        Break = 1,

        /// <summary>
        /// The <see cref="Item"/> is deleted when it expires.
        /// </summary>
        Expire = 2,

        /// <summary>
        /// The <see cref="Item"/> is deleted when broken or expired.
        /// </summary>
        Any = Break | Expire
    }
}
