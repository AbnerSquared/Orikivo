using System.Collections.Generic;

namespace Arcadia.Casino
{
    public interface IReplyCollection
    {
        string Default { get; }

        IEnumerable<IReply> Replies { get; }

        string GetReply(ArcadeUser user, object reference);
    }
}