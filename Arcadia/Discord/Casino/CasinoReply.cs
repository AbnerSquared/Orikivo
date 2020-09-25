using System;

namespace Arcadia.Casino
{
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
