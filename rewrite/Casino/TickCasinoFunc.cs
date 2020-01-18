using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo.Casino
{
    class TickCasinoFunc
    {
    }

    public class CasinoProvider
    {
        public Embed FromResult()
        {
            throw new NotImplementedException();
        }
    }

    public class CasinoReply
    {
        public string Content { get; }
        public List<ReplyCriterion> Criteria { get; }
    }

    public class ReplyCriterion
    {
        public string Id;
        public int ExpectedValue;
    }
}
