using System;

namespace Arcadia
{
    [Flags]
    public enum DeleteMode
    {
        Break = 1, // this item is deleted when broken (durability = 0)
        Expire = 2, // this item is deleted when expired (ExpiresOn < DateTime.UtcNow)
        Any = Break | Expire // This item is deleted when broken or expired
    }
}