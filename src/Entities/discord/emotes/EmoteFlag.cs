using System;

namespace Orikivo
{
    [Flags]
    public enum EmoteFlag
    {
        Animated = 1,
        Boosted = 2,
        Managed = 4// this is an emote that exists over the default guild cap,
        // which has the chance of being unusable within 3 days if the boost level drops
    }
}