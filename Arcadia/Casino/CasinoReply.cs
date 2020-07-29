using System;
using System.Collections.Generic;

namespace Arcadia.Casino
{
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

        public Func<ArcadeUser, GimiResult, bool> Criteria { get; set; }

        public Func<ArcadeUser, GimiResult, string> Writer { get; set; }

        public static implicit operator CasinoReply(string value)
            => new CasinoReply { Content = value };

        public static implicit operator string(CasinoReply reply)
            => reply.Content;

        public override string ToString()
            => Content;

        public string ToString(ArcadeUser user, GimiResult result)
        {
            if (Writer == null)
                return Content;

            return Writer(user, result);
        }
    }
}
