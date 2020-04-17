using System;
using System.Linq;

namespace Orikivo.Desync
{

    // this checks flags and stats; flags are simply a 0 OR 1 state, and stats have a long value equipped. The Engine class handles the object type that stat actually stores
    // stats might be converted to bytes.
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class OnlyWhenAttribute : Attribute
    {
        internal enum JudgeType
        {
            Flag,
            Stat
        }

        public OnlyWhenAttribute(string flag)
        {
            Gate = LogicGate.AND;
            Flags = new string[1] { flag };
            Type = JudgeType.Flag;
        }

        // for husks
        public OnlyWhenAttribute(LogicGate gate, string flag, params string[] rest)
        {
            Gate = gate;

            var flags = new string[rest.Length + 1];

            flags[0] = flag;

            for (int i = 0; i < rest.Length; i++)
                flags[i] = rest[i + 1];


            Flags = flags;
            Type = JudgeType.Flag;
        }

        // for users
        public OnlyWhenAttribute(string valueA, LogicMatch match, long valueB)
        {
            var matches = new (string, LogicMatch, long)[1];

            matches[0] = (valueA, match, valueB);

            Matches = matches;
            Type = JudgeType.Stat;
        }

        internal JudgeType Type { get; }

        private LogicGate Gate { get; }

        private (string, LogicMatch, long)[] Matches { get; }

        private string[] Flags { get; }

        public bool Judge(Husk husk, HuskBrain brain)
        {
            if (Type != JudgeType.Flag)
                throw new ArgumentException("This Attribute was set up for a User.");

            return JudgeFlags(brain.Flags.ToArray(), Gate, Flags);
        }

        public bool Judge(User user)
        {
            if (Type != JudgeType.Stat)
                throw new ArgumentException("This Attribute was set up for a Husk.");


            foreach ((string id, LogicMatch logic, long expected) in Matches)
            {
                long actual = user.GetStat(id);

                if (!Compare(actual, logic, expected))
                    return false;
            }

            return true;
        }

        private static bool JudgeFlags(string[] actualFlags, LogicGate gate, string[] expectedFlags)
        {
            switch(gate)
            {
                case LogicGate.AND:
                    foreach(string flag in actualFlags)
                    {
                        if (!expectedFlags.Contains(flag))
                            return false;
                    }

                    return true;

                case LogicGate.OR:
                    foreach (string flag in actualFlags)
                    {
                        if (expectedFlags.Contains(flag))
                            return true;
                    }

                    return false;

                case LogicGate.NAND:
                    foreach(string flag in actualFlags)
                    {
                        if (!expectedFlags.Contains(flag))
                            return true;
                    }

                    return false;

                case LogicGate.NOR:
                    foreach (string flag in actualFlags)
                    {
                        if (expectedFlags.Contains(flag))
                            return false;
                    }

                    return true;

                default:
                    return true;
            }
        }

        private static bool Compare(long actual, LogicMatch match, long expected)
        {
            return match switch
            {
                LogicMatch.GREATER => actual > expected,
                LogicMatch.GREATER_EQUALS => actual >= expected,
                LogicMatch.EQUALS => actual == expected,
                LogicMatch.LESSER_EQUALS => actual <= expected,
                LogicMatch.LESSER => actual < expected,
                LogicMatch.NOT_EQUALS => actual != expected,
                _ => true
            };
        }
    }
}