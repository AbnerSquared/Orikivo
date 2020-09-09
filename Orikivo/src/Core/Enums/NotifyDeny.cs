using System;

namespace Orikivo.Desync
{
    /// <summary>
    /// Defines what a <see cref="Notifier"/> is allowed to store.
    /// </summary>
    [Flags]
    public enum NotifyAllow
    {
        Merit = 1,
        Cooldown = 2,
        OfferAccepted = 4,
        OfferInbound = 8,
        GiftInbound = 16,
        Invite = 32,
        Level = 64
    }
}
