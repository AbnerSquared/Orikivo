namespace Arcadia
{
    [System.Flags]
    public enum CardDeny
    {
        None = 0,
        Avatar = 1,
        Username = 2,
        Activity = 4,
        Level = 8,
        Money = 16,
        Merit = 32,
        Exp = 64
    }
}
