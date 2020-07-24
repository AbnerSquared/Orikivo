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
            Criteria = new List<StatCriterion>();
        }

        // It determines what replies to choose based on their priority.
        public int Priority { get; private set; } = 0;
        public string Content { get; private set; }
        public List<StatCriterion> Criteria { get; private set; }

        public static implicit operator CasinoReply(string value)
            => new CasinoReply { Content = value };

        public static implicit operator string(CasinoReply reply)
            => reply.Content;
    }
}
