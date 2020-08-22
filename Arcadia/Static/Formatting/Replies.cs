using System;
using System.Collections.Generic;
using System.Linq;
using Arcadia.Casino;
using Orikivo;

namespace Arcadia
{
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
        public static readonly string TickExactGeneric = "You have guessed the exact tick correctly.";

        public static string GetReply(DailyResultFlag flag)
        {
            string[] replies = GetReplies(flag);
            return Check.NotNullOrEmpty(replies) ? Randomizer.Choose(replies) : GetGeneric(flag);
        }

        // TODO: implement criterion and priorities for replies
        public static string GetReply(GimiResultFlag flag, ArcadeUser user = null, GimiResult result = null)
        {
            IEnumerable<CasinoReply> replies = GetReplies(flag);

            if (user != null && result != null)
            {
                replies = replies.Where(x => MeetsCriteria(x, user, result));
            }

            if (Check.NotNullOrEmpty(replies))
                return Randomizer.Choose(replies).ToString(user, result);

            return GetGeneric(flag);
        }

        public static string GetReply(TickResultFlag flag, ArcadeUser user = null, TickResult result = null)
        {
            IEnumerable<CasinoReply> replies = GetReplies(flag);

            if (user != null && result != null)
            {
                replies = replies.Where(x => MeetsCriteria(x, user, result));
            }

            if (Check.NotNullOrEmpty(replies))
                return Randomizer.Choose(replies).ToString(user, result);

            return GetGeneric(flag);
        }

        private static bool MeetsCriteria(CasinoReply reply, ArcadeUser user, ICasinoResult result)
        {
            if (reply.Criteria == null)
                return true;

            return reply.Criteria(user, result);
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

        private static string GetGeneric(TickResultFlag flag)
            => flag switch
            {
                TickResultFlag.Win => WinGeneric,
                TickResultFlag.Exact => TickExactGeneric,
                TickResultFlag.Lose => LoseGeneric,
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

        private static CasinoReply[] GetReplies(TickResultFlag flag)
            => flag switch
            {
                TickResultFlag.Win => TickWin,
                TickResultFlag.Exact => TickExact,
                TickResultFlag.Lose => TickLoss,
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
            "The moonlight burns through your funds.",
            new CasinoReply
            {
                Content = "Pocket Lawyer can't save you this time.",
                Criteria = (user, result) => ItemHelper.GetCooldownRemainder(user, Items.PocketLawyer)?.Ticks > 0
            }
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
            "z):^)",
            new CasinoReply
            {
                Content = "Maximum profits achieved!",
                Criteria = (user, result) => result.Reward == 10
            }
        };

        public static readonly CasinoReply[] Recover =
        {
            "Paying off your debt early helps in the long run.",
            "Look at you! You're being an adult.",
            "The debt in this reality is no laughing matter."
        };

        public static readonly CasinoReply[] Lose =
        {
            "I guess they can't all be winners.",
            "Yikes!",
            "Sorry, lad. I can't host myself for free.",
            new CasinoReply
            {
                Content = "The **E** in your name stands for empty. Just like your wallet.",
                Criteria = (user, result) => user.Username.ToLower().StartsWith('e')
            },
            new CasinoReply
            {
                Content = "Maximum losses obtained.",
                Criteria = (user, result) => result.Reward == 10
            }
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
            "Access denied.",
            "Please go find something better to do."
        };

        public static readonly string[] DailyBonus =
        {
            "You have achieved true consistency.",
            "Glad to see you could make it."
        };

        public static readonly CasinoReply[] TickWin =
        {
            "Nicely done!",
            new CasinoReply
            {
                Criteria = (user, result) => result is TickResult t
                                             && t.Multiplier > 5,
                Writer = delegate(ArcadeUser user, ICasinoResult result)
                {
                    if (!(result is TickResult t))
                        return "INVALID_RESULT_TYPE";

                    return $"You beat the odds, boosting your bet by an astonishing x**{t.Multiplier:##,0.#}**!";
                }
            },
            new CasinoReply
            {
                Criteria = (user, result) => result is TickResult,
                Writer = delegate(ArcadeUser user, ICasinoResult result)
                {
                    if (!(result is TickResult t))
                        return "INVALID_RESULT_TYPE";

                    return $"The machine stopped at **{t.ActualTick}** {Format.TryPluralize("tick", t.ActualTick)}.";
                }
            },
        };

        public static readonly CasinoReply[] TickExact =
        {
            "Now that was a clean guess.",
            new CasinoReply
            {
                Criteria = (user, result) => result is TickResult,
                Writer = delegate(ArcadeUser user, ICasinoResult result)
                {
                    if (!(result is TickResult t))
                        return "INVALID_RESULT_TYPE";

                    return $"The machine stopped at exactly **{t.ActualTick}** {Format.TryPluralize("tick", t.ActualTick)}. Good guessing!";
                }
            },
        };

        public static readonly CasinoReply[] TickLoss =
        {
            "Ouch. Sorry about that.",
            new CasinoReply
            {
                Criteria = (user, result) => result is TickResult tResult
                                             && tResult.ActualTick > 0,
                Writer = delegate(ArcadeUser user, ICasinoResult result)
                {
                    if (!(result is TickResult t))
                        return "INVALID_RESULT_TYPE";

                    return $"The machine stopped at **{t.ActualTick}** {Format.TryPluralize("tick", t.ActualTick)}.";
                }
            },
            new CasinoReply
            {
                Criteria = (user, result) => result is TickResult t
                                             && t.ActualTick != 0
                                             && !(t.ActualTick - t.ExpectedTick).EqualsAny(t.ActualTick, 0),
                Writer = delegate(ArcadeUser user, ICasinoResult result)
                {
                    if (!(result is TickResult t))
                        return "INVALID_RESULT_TYPE";

                    int sign = Math.Sign(t.ActualTick - t.ExpectedTick);
                    int diff = Math.Abs(t.ActualTick - t.ExpectedTick);
                    return $"You were **{diff}** {Format.TryPluralize("tick", diff)} {(sign == 1 ? "above" : "below")} **{t.ExpectedTick}**.";
                }
            },
            new CasinoReply
            {
                Writer = (user, result) => $"You missed a chance to win {Icons.Chips} **{result.Reward:##,0}**."
            },
            new CasinoReply
            {
                Content = "This machine has flat-lined at the start.",
                Criteria = (user, result) => result is TickResult t
                                             && t.ActualTick == 0
            },
            new CasinoReply
            {
                Content = "You were off by a single tick.",
                Criteria = (user, result) => result is TickResult t
                                             && Math.Abs(t.ActualTick - t.ExpectedTick) == 1
            }
        };
    }
}
