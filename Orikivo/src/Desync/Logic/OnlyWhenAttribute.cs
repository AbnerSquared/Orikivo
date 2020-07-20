using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents an <see cref="Attribute"/> that allows a command to execute only when the specified values match.
    /// </summary>
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

        public OnlyWhenAttribute(LogicGate gate, string flag, params string[] rest)
        {
            Gate = gate;

            Flags = rest.Prepend(flag);
            Type = JudgeType.Flag;
        }

        public OnlyWhenAttribute(string valueA, LogicMatch match, long valueB)
        {
            Matches = new List<LogicArgument> { new LogicArgument(valueA, match, valueB) };
            Type = JudgeType.Stat;
        }

        internal JudgeType Type { get; }

        private LogicGate Gate { get; }

        private IEnumerable<LogicArgument> Matches { get; }

        private IEnumerable<string> Flags { get; }

        public bool Judge(User user)
        {
            switch(Type)
            {
                case JudgeType.Flag:
                    return JudgeFlags(user.Brain.Flags.ToArray(), Gate, Flags);

                case JudgeType.Stat:

                    foreach ((string id, LogicMatch logic, long expected) in Matches)
                    {
                        long actual = user.GetStat(id);

                        if (!Compare(actual, logic, expected))
                            return false;
                    }

                    return true;

                default:
                    throw new Exception("Invalid JudgeType specified.");
            }
        }

        private static bool JudgeFlags(IEnumerable<string> actualFlags, LogicGate gate, IEnumerable<string> expectedFlags)
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
                LogicMatch.GTR => actual > expected,
                LogicMatch.GEQ => actual >= expected,
                LogicMatch.EQU => actual == expected,
                LogicMatch.LEQ => actual <= expected,
                LogicMatch.LSS => actual < expected,
                LogicMatch.NEQ => actual != expected,
                _ => true
            };
        }
    }
}