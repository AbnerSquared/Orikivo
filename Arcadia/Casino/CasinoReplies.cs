using Orikivo;

namespace Arcadia
{
    // TODO: Create criteria-based replies.
    public static class CasinoReplies
    {
        public static readonly string DebtGeneric = "You have been fined.";
        public static readonly string CurseGeneric = "You have been cursed.";
        public static readonly string GoldGeneric = "You have been blessed.";
        public static readonly string WinGeneric = "You have won.";
        public static readonly string LoseGeneric = "You have lost.";
        public static readonly string EvenGeneric = "You broke even.";
        public static readonly string DailyGeneric = "You have claimed your daily income.";
        public static readonly string ResetGeneric = "Your streak has been reset.";
        public static readonly string CooldownGeneric = "You are currently on cooldown.";
        public static readonly string BonusGeneric = "You have been given a bonus.";


        private static readonly string _clock1 = "🕐";
        private static readonly string _clock2 = "🕑";
        private static readonly string _clock3 = "🕒";
        private static readonly string _clock4 = "🕓";
        private static readonly string _clock5 = "🕔";
        private static readonly string _clock6 = "🕕";
        private static readonly string _clock7 = "🕖";
        private static readonly string _clock8 = "🕗";
        private static readonly string _clock9 = "🕘";
        private static readonly string _clock10 = "🕙";
        private static readonly string _clock11 = "🕚";
        private static readonly string _clock12 = "🕛";

        public static string GetHourEmote(int hour)
        {
            switch(hour)
            {
                case 11:
                case 23:
                    return _clock11;

                case 10:
                case 22:
                    return _clock10;

                case 9:
                case 21:
                    return _clock9;

                case 8:
                case 20:
                    return _clock8;

                case 7:
                case 19:
                    return _clock7;

                case 6:
                case 18:
                    return _clock6;

                case 5:
                case 17:
                    return _clock5;

                case 4:
                case 16:
                    return _clock4;

                case 3:
                case 15:
                    return _clock3;

                case 2:
                case 14:
                    return _clock2;

                case 1:
                case 13:
                    return _clock1;

                default:
                    return _clock12;
            }
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

        public static string NextReply(GimiResultFlag flag)
        {
            string[] replies = GetReplies(flag);
            string generic = GetGeneric(flag);

            return replies.Length > 0 ? Randomizer.Choose(replies) : generic;
        }

        private static string GetGeneric(GimiResultFlag flag)
            => flag switch
            {
                GimiResultFlag.Win => WinGeneric,
                GimiResultFlag.Gold => GoldGeneric,
                GimiResultFlag.Lose => LoseGeneric,
                GimiResultFlag.Curse => CurseGeneric,
                _ => throw new System.Exception("Unknown flag.")
            };

        private static string[] GetReplies(GimiResultFlag flag)
            => flag switch
            {
                GimiResultFlag.Win => Win,
                GimiResultFlag.Gold => Gold,
                GimiResultFlag.Lose => Lose,
                GimiResultFlag.Curse => Curse,
                _ => throw new System.Exception("Unknown flag.")
            };



        public static readonly string[] Debt =
        {
            "I guess you reap what you sow.",
            "Please stop. You're hurting yourself.",
            "ORS isn't pleased. I'm worried for you.",
            "Unable to compute negative funds.",
            "Perish."
        };

        public static readonly string[] Curse =
        {
            "Experience oblivion.",
        };

        public static readonly string[] Gold =
        {
            "All that truly glitters is gold.",
            "The calm before the storm.",
            "Let's hope this got you out of debt.",
            "Huh. Who would've thought I could even give out this much?"
        };

        public static readonly string[] Win =
        {
            "A little bit of **Orite** can go far in life.",
            "Your wish has been granted.",
            "Hope is a powerful emotion.",
            "Ask and you shall receive.",
            "A new wave of profits splash your way!",
            "z):^)"
        };

        public static readonly string[] Lose =
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
