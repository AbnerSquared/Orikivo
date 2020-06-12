using System;

namespace Arcadia
{
    // this is a ruleset that can be used to quickly determine if certain criteria was met
    public class GameRule
    {
        public string Id { get; internal set; }

        // what is the criterion?
        public Func<GameSession, bool> Criterion { get; set; }

        // what is the action i execute whenever this rule is utilized
        public string ActionId { get; set; }
    }
}
