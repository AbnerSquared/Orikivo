namespace Arcadia.Games
{
    [System.Flags]
    public enum CatanAward
    {
        None = 0,
        Army = 1,
        Road = 2,
        All = Army | Road
    }
}