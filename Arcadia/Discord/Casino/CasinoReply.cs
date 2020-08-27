using System;
using System.Collections.Generic;

namespace Arcadia.Casino
{
    public interface IReplyCollection
    {
        string Default { get; }

        IEnumerable<IReply> Replies { get; }

        string GetReply(ArcadeUser user, object reference);
    }

    public enum CasinoMode
    {
        Gimi = 1,
        Tick = 2
    }

    public delegate bool ReplyCriteria(ArcadeUser user, object reference);

    public delegate string ReplyWriter(ArcadeUser user, object reference);

    public delegate bool ReplyCriteria<in T>(ArcadeUser user, T reference);

    public delegate string ReplyWriter<in T>(ArcadeUser user, T reference);

    public interface IReply
    {
        int Priority { get; }

        ReplyCriteria Criteria { get; }

        ReplyWriter Writer { get; }
    }

    public interface IReply<in T>
    {
        int Priority { get; }

        ReplyCriteria<T> Criteria { get; }

        ReplyWriter<T> Writer { get; }
    }

    public class Reply : IReply
    {
        public int Priority { get; set; }

        public ReplyCriteria Criteria { get; set; }

        public ReplyWriter Writer { get; set; }
    }

    public class Reply<T> : IReply<T>
    {
        public int Priority { get; set; }

        public ReplyCriteria<T> Criteria { get; set; }

        public ReplyWriter<T> Writer { get; set; }
    }

    public class CasinoReply : CasinoReply<ICasinoResult>
    {
        internal CasinoReply() {}

        public CasinoReply(int priority, string content)
        {
            Priority = priority;
            Content = content;
        }

        public string Content { get; internal set; }

        public static implicit operator CasinoReply(string value)
            => new CasinoReply { Content = value };

        public static implicit operator string(CasinoReply reply)
            => reply.Content;

        public override string ToString()
            => Content;

        public string ToString(ArcadeUser user, ICasinoResult result)
        {
            return Writer == null ? Content : Writer(user, result);
        }
    }

    public class CasinoReply<TResult> : Reply<TResult>
        where TResult : ICasinoResult
    {
        internal CasinoReply() { }

        public CasinoReply(int priority, string content)
        {
            Priority = priority;
            Content = content;
        }

        public string Content { get; internal set; }


        public static implicit operator CasinoReply<TResult>(string value)
            => new CasinoReply<TResult> { Content = value };

        public static implicit operator string(CasinoReply<TResult> reply)
            => reply.Content;

        public override string ToString()
            => Content;

        public string ToString(ArcadeUser user, TResult result)
        {
            return Writer == null ? Content : Writer(user, result);
        }
    }
}
