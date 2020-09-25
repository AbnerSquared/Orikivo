namespace Arcadia.Services
{
    // This could possibly be used instead so that it's easier to filter out stuff

    public enum LeaderboardQuery
    {
        Default = 0,
        Money = 1,
        Debt = 2,
        Level = 3,
        Chips = 4,
        Merits = 5,
        Custom = 6 // This allows for leaderboards by stats
    }
}