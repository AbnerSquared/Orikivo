namespace Arcadia.Casino
{
    public class Reply<T> : IReply<T>
    {
        public int Priority { get; set; }

        public ReplyCriteria<T> Criteria { get; set; }

        public ReplyWriter<T> Writer { get; set; }
    }

    public class Reply : IReply
    {
        public int Priority { get; set; }

        public ReplyCriteria Criteria { get; set; }

        public ReplyWriter Writer { get; set; }
    }
}