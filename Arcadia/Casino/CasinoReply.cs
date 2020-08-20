using System;
using System.Collections.Generic;

namespace Arcadia.Casino
{
    public enum CasinoMode
    {
        Gimi = 1,
        Tick = 2
    }

    public class CasinoReply
    {
        internal CasinoReply() {}

        public CasinoReply(int priority, string content)
        {
            Priority = priority;
            Content = content;
        }

        // It determines what replies to choose based on their priority.
        public int Priority { get; }
        public string Content { get; internal set; }

        public Func<ArcadeUser, ICasinoResult, bool> Criteria { get; set; }

        public Func<ArcadeUser, ICasinoResult, string> Writer { get; set; }

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
}
