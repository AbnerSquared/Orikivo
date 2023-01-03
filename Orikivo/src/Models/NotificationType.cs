namespace Orikivo
{
    [System.Flags]
    public enum NotificationType
    {
        BadgeReceived = 1,
        CooldownExpired = 2,
        OfferAccepted = 4,
        OfferReceived = 8,
        GiftReceived = 16,
        InviteReceived = 32,
        LevelUpdated = 64,
        ResearchCompleted = 128,
        DailyAvailable = 256,
        ItemReceived = 512,
        OrderCompleted = 1024
    }
}
