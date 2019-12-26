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

    // TODO: Create criteria-based replies.
    public static class CasinoReplies
    {
        public static string DebtGeneric = "You have been fined.";
        public static string CurseGeneric = "You have been cursed.";
        public static string GoldenGeneric = "You have been blessed.";
        public static string WinGeneric = "You have won.";
        public static string LossGeneric = "You have lost.";

        public static string[] Debt =
        {

        };

        public static string[] Curse =
        {

        };

        public static string[] Golden =
        {
            "All that truly glitters is gold.",
            "The calm before the storm."
        };

        public static string[] Win =
        {
            "A little bit of **Orite** can go far in life."
        };

        public static string[] Loss =
        {
            "They can't all be winners.",
            "Experience oblivion."
        };
    }
}
