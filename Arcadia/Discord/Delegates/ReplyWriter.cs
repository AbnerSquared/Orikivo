namespace Arcadia.Casino
{
    public delegate string ReplyWriter(ArcadeUser user, object reference);

    public delegate string ReplyWriter<in T>(ArcadeUser user, T reference);
}