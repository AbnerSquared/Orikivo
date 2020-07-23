using System;
using System.Collections.Generic;
using System.Linq;
using Arcadia.Casino;
using Orikivo;

namespace Arcadia
{
    public static class MoneyConvert
    {
        public const double MoneyToChip = 1 / ChipToMoney;
        public const double ChipToMoney = 0.67;
        public const double TokenToMoney = 5;

        public static long GetChips(long money)
            => (long)Math.Ceiling(money * MoneyToChip);

        public static long GetChipMoney(long chips)
            => (long)Math.Ceiling(chips * ChipToMoney);

            public static long GetTokenMoney(long tokens)
            => (long) Math.Ceiling(tokens * TokenToMoney);

        // Tokens are received from voting
        // Due to their rarity, 1 Token is worth 5 Money

    }

    public enum DailyResultFlag
    {
        Success,
        Cooldown,
        Reset,
        Bonus
    }

    // TODO: Create criteria-based replies.
    public static class CasinoReplies
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


        private const string CLOCK_1 = "🕐";
        private const string CLOCK_2 = "🕑";
        private const string CLOCK_3 = "🕒";
        private const string CLOCK_4 = "🕓";
        private const string CLOCK_5 = "🕔";
        private const string CLOCK_6 = "🕕";
        private const string CLOCK_7 = "🕖";
        private const string CLOCK_8 = "🕗";
        private const string CLOCK_9 = "🕘";
        private const string CLOCK_10 = "🕙";
        private const string CLOCK_11 = "🕚";
        private const string CLOCK_12 = "🕛";

        public static string GetHourEmote(int hour)
        {
            switch(hour)
            {
                case 11: case 23:
                    return CLOCK_11;

                case 10: case 22:
                    return CLOCK_10;

                case 9: case 21:
                    return CLOCK_9;

                case 8: case 20:
                    return CLOCK_8;

                case 7: case 19:
                    return CLOCK_7;

                case 6: case 18:
                    return CLOCK_6;

                case 5: case 17:
                    return CLOCK_5;

                case 4: case 16:
                    return CLOCK_4;

                case 3: case 15:
                    return CLOCK_3;

                case 2: case 14:
                    return CLOCK_2;

                case 1: case 13:
                    return CLOCK_1;

                default:
                    return CLOCK_12;
            }
        }

        public static string GetReply(DailyResultFlag flag)
        {
            var replies = GetReplies(flag);
            return Check.NotNullOrEmpty(replies) ? Randomizer.Choose(replies) : GetGeneric(flag);
        }

        public static string GetCooldownText()
        {
            return DailyCooldown.Length > 0 ? Randomizer.Choose(DailyCooldown) : CooldownGeneric;
        }

        public static string GetResetText()
        {
            return DailyReset.Length > 0 ? Randomizer.Choose(DailyReset) : ResetGeneric;
        }


        public static string GetBonusText()
        {
            return DailyBonus.Length > 0 ? Randomizer.Choose(DailyBonus) : BonusGeneric;
        }

        public static string GetDailyText()
        {
            return Daily.Length > 0 ? Randomizer.Choose(Daily) : DailyGeneric;
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


            // USE .Any() instead of .Count() > 0
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
