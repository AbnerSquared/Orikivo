using System.Collections.Generic;
using Arcadia.Casino;
using Orikivo;

namespace Arcadia
{
    // TODO: Create criteria-based replies.
    public static class Replies
    {
        public static readonly string DebtGeneric = "You have been fined.";
        public static readonly string CurseGeneric = "You have been cursed.";
        public static readonly string GoldGeneric = "You have been blessed.";
        public static readonly string WinGeneric = "You have won.";
        public static readonly string RecoverGeneric = "You have paid off some debt.";
        public static readonly string LoseGeneric = "You have lost.";
        public static readonly string EvenGeneric = "You broke even.";
        public static readonly string DailyGeneric = "You have claimed your daily income.";
        public static readonly string ResetGeneric = "Your streak has been reset.";
        public static readonly string CooldownGeneric = "You are currently on cooldown.";
        public static readonly string BonusGeneric = "You have been given a bonus.";

        public static string GetReply(DailyResultFlag flag)
        {
            string[] replies = GetReplies(flag);
            return Check.NotNullOrEmpty(replies) ? Randomizer.Choose(replies) : GetGeneric(flag);
        }

        // TODO: implement criterion and priorities for replies
        public static string GetReply(GimiResultFlag flag, ArcadeUser user = null)
        {
            IEnumerable<CasinoReply> replies = GetReplies(flag);

            /*
            if (user == null)
            {
                replies = replies.Where(x => !x.Criteria.Any());
            }
            */

            return Check.NotNullOrEmpty(replies) ? (string)Randomizer.Choose(replies) : GetGeneric(flag);
        }

        private static string GetGeneric(GimiResultFlag flag)
            => flag switch
            {
                GimiResultFlag.Win => WinGeneric,
                GimiResultFlag.Gold => GoldGeneric,
                GimiResultFlag.Lose => LoseGeneric,
                GimiResultFlag.Curse => CurseGeneric,
                _ => "INVALID_FLAG"
            };

        private static string GetGeneric(DailyResultFlag flag)
            => flag switch
            {
                DailyResultFlag.Success => DailyGeneric,
                DailyResultFlag.Cooldown => CooldownGeneric,
                DailyResultFlag.Reset => ResetGeneric,
                DailyResultFlag.Bonus => BonusGeneric,
                _ => "INVALID_FLAG"
            };

        private static CasinoReply[] GetReplies(GimiResultFlag flag)
            => flag switch
            {
                GimiResultFlag.Win => Win,
                GimiResultFlag.Gold => Gold,
                GimiResultFlag.Lose => Lose,
                GimiResultFlag.Curse => Curse,
                _ => null
            };

        private static string[] GetReplies(DailyResultFlag flag)
            => flag switch
            {
                DailyResultFlag.Success => Daily,
                DailyResultFlag.Cooldown => DailyCooldown,
                DailyResultFlag.Reset => DailyReset,
                DailyResultFlag.Bonus => DailyBonus,
                _ => null
            };



        public static readonly CasinoReply[] Debt =
        {
            "I guess you reap what you sow.",
            "Please stop. You're hurting yourself.",
            "ORS isn't pleased. I'm worried for you.",
            "Unable to compute negative funds.",
            "Perish."
        };

        public static readonly CasinoReply[] Curse =
        {
            "Experience oblivion.",
        };

        public static readonly CasinoReply[] Gold =
        {
            "All that truly glitters is gold.",
            "The calm before the storm.",
            "Let's hope this got you out of debt.",
            "Huh. Who would've thought I could even give out this much?"
        };

        public static readonly CasinoReply[] Win =
        {
            "A little bit of **Orite** can go far in life.",
            "Your wish has been granted.",
            "Hope is a powerful emotion.",
            "Ask and you shall receive.",
            "A new wave of profits splash your way!",
            "z):^)"
        };

        public static readonly CasinoReply[] Recover =
        {
            "Paying off your debt early helps in the long run."
        };

        public static readonly CasinoReply[] Lose =
        {
            "I guess they can't all be winners.",
            "Yikes!",
            "Sorry, lad. I can't host myself for free."
        };

        public static readonly string[] DailyReset =
        {
            "Waiting until the right moment is called patience. Waiting forever is called laziness.",
            "You took too long. Now your candy is gone."
        };

        public static readonly string[] Daily =
        {
            "Enjoy the funds.",
            "Don't mention it. It's the least I could do.",
            "I'll be honest, this is your best bet to earning funds."
        };

        public static readonly string[] DailyCooldown =
        {
            "Hang tight. I'm not an OTM.",
            "Patience is key.",
            "Access denied."
        };

        public static readonly string[] DailyBonus =
        {
            "You have achieved true consistency.",
            "Glad to see you could make it."
        };
    }
}
