namespace Arcadia.Casino
{
    public delegate bool ReplyCriteria(ArcadeUser user, object reference);

    public delegate bool ReplyCriteria<in T>(ArcadeUser user, T reference);
}