using System;

namespace Arcadia
{
    [Flags]
    public enum ExpiryTrigger
    {
        // Expiration starts when first given
        Own = 1,

        // Expiration starts when first used
        Use = 2,

        // Expiration starts when traded
        Trade = 4,

        // Expiration starts when gifted
        Gift = 8,

        // Expiration starts when equipped
        Equip = 16
    }
}