using System;

namespace Orikivo.Desync
{
    public class LogicArgument
    {
        public LogicArgument(string id, LogicMatch match, long value)
        {
            Id = id;
            Match = match;
            Value = value;
        }

        public string Id { get; set; }
        public LogicMatch Match { get; set; }
        public long Value { get; set; }

        public bool Judge(User user)
        {
            long actual = user.GetStat(Id);

            return Match switch
            {
                LogicMatch.GTR => actual > Value,
                LogicMatch.GEQ => actual >= Value,
                LogicMatch.EQU => actual == Value,
                LogicMatch.LEQ => actual <= Value,
                LogicMatch.LSS => actual < Value,
                LogicMatch.NEQ => actual != Value,
                _ => false
            };
        }

        internal void Deconstruct(out string id, out LogicMatch match, out long value)
        {
            id = Id;
            match = Match;
            value = Value;
        }
    }
}