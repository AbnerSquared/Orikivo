namespace Arcadia.Casino
{
    public interface IReply<in T>
    {
        int Priority { get; }

        ReplyCriteria<T> Criteria { get; }

        ReplyWriter<T> Writer { get; }
    }

    public interface IReply
    {
        int Priority { get; }

        ReplyCriteria Criteria { get; }

        ReplyWriter Writer { get; }
    }
}