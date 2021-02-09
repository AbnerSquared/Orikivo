using System;
using System.Collections.Generic;
using System.Linq;
using Arcadia.Casino;
using Arcadia.Services;
using Orikivo;

namespace Arcadia
{
    public class BaseReply<TEnum> : Reply<TEnum>
        where TEnum : struct
    {
        public string Content { get; internal set; }


        public static implicit operator BaseReply<TEnum>(string value)
            => new BaseReply<TEnum> { Content = value };

        public static implicit operator string(BaseReply<TEnum> reply)
            => reply.Content;

        public override string ToString()
            => Content;

        public string ToString(ArcadeUser user, TEnum result)
        {
            return Writer == null ? Content : Writer(user, result);
        }
    }

    // TODO: Merge all base casino replies together, as they can be separated by criteria
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

        public static string GetReply(DailyResultFlag flag, ArcadeUser user)
        {
            BaseReply<DailyResultFlag>[] replies = GetReplies(flag);
            return (Check.NotNullOrEmpty(replies) ? Randomizer.Choose(replies).ToString(user, flag) : GetGeneric(flag));
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

        public static string GetReply(DoublerResultFlag flag, ArcadeUser user = null, DoublerResult result = null)
        {
            IEnumerable<CasinoReply> replies = Doubly;

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

        private static string GetGeneric(DoublerResultFlag flag)
            => flag switch
            {
                DoublerResultFlag.Win => WinGeneric,
                DoublerResultFlag.Exact => TickExactGeneric,
                DoublerResultFlag.Lose => LoseGeneric,
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

        private static BaseReply<DailyResultFlag>[] GetReplies(DailyResultFlag flag)
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
            "Your **Orite** exists no longer. Cease interaction."
        };

        public static readonly CasinoReply[] Curse =
        {
            new CasinoReply
            {
                Criteria = (user, result) => result is GimiResult g && g.Flag == GimiResultFlag.Curse,
                Content = "Experience oblivion."
            },
            new CasinoReply
            {
                Criteria = (user, result) => result is GimiResult g && g.Flag == GimiResultFlag.Curse,
                Content = "The moonlight burns through your funds."
            },
            new CasinoReply
            {
                Content = "Pocket Lawyer can't save you this time.",
                Criteria = (user, result) => result is GimiResult g && g.Flag == GimiResultFlag.Curse && ItemHelper.GetCooldownRemainder(user, Ids.Items.PocketLawyer)?.Ticks > 0,
                Priority = 1
            },
            "The moon has risen.",
            "Then perish."
        };

        public static readonly CasinoReply[] Gold =
        {
            "All that truly glitters is gold.",
            "The calm before the storm.",
            "Let's hope this got you out of debt.",
            "Huh. Who would've known I could even give out this much?",
            "You have been blessed.",
            "Use that lawyer wisely. They have a strong benefit.",
            "Looks like the lawyer took note.",
            "It's shiny. It's golden. What's not to like?",
            new CasinoReply
            {
                Content = "I see that you are truly golden now.",
                Criteria = (user, result) => result is GimiResult g && g.WonGold,
                Priority = 1
            },
            new CasinoReply
            {
                Content = "You managed to land a back-to-back golden income...",
                Criteria = (user, result) => user.GetVar(Stats.Gimi.CurrentGoldStreak) > 1,
                Priority = 1
            }
        };

        public static readonly CasinoReply[] Win =
        {
            "I hope you're being wise with your income.",
            "*psst* I got you something.",
            "Spreading joy is a fun pastime.",
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
            },
            new CasinoReply
            {
                Content = "Lucky you.",
                Criteria = (user, result) => user.GetVar(Stats.Gimi.CurrentWinStreak) > 3
            },
            new CasinoReply
            {
                Content = "Hey, it's always good to stop during a win streak.",
                Criteria = (user, result) => user.GetVar(Stats.Gimi.CurrentWinStreak) > 3
            },
            new CasinoReply
            {
                Content = "I have reason to believe that you're modifying something here.",
                Criteria = (user, result) => user.GetVar(Stats.Gimi.CurrentWinStreak) > 10
            },
            new CasinoReply
            {
                Content = "I see you've discovered the clover of **Gimi**. That makes sense now.",
                Criteria = (user, result) => user.GetVar(Stats.Gimi.CurrentWinStreak) > 20,
                Priority = 1
            },
            new CasinoReply
            {
                Content = "I have reason to believe that you're modifying something here.",
                Criteria = (user, result) => user.GetVar(Stats.Gimi.CurrentWinStreak) > 5
            },
            "Profitable success."
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
            "You win some, you lose some.",
            new CasinoReply
            {
                Content = "Maximum losses obtained.",
                Criteria = (user, result) => result.Reward == 10,
                Priority = 2
            },
            new CasinoReply
            {
                Content = "You win some, you *definitely* lose some.",
                Criteria = (user, result) => result.Reward == 10,
                Priority = 2
            },
            new CasinoReply
            {
                Content = "This isn't looking good.",
                Criteria = (user, result) => user.GetVar(Stats.Gimi.CurrentLossStreak) > 3,
                Priority = 1
            },
            new CasinoReply
            {
                Content = "I sense a grim path ahead.",
                Criteria = (user, result) => user.GetVar(Stats.Gimi.CurrentLossStreak) > 3,
                Priority = 1
            },
            new CasinoReply
            {
                Content = "Please stop now. You are headed towards a downfall of debt.",
                Criteria = (user, result) => user.GetVar(Stats.Gimi.CurrentLossStreak) > 3,
                Priority = 1
            }
        };

        public static readonly BaseReply<DailyResultFlag>[] DailyReset =
        {
            "Waiting until the right moment is called patience. Waiting forever is called laziness.",
            "Acting in the moment is important. Don't wait for life to pass by.",
            "You took too long. Now your candy is gone.",
            "The timeframe of your attendance has fallen short.",
            "Your streak has fallen apart.",
            "Welcome to square one.",
            "Even though you're late, I'm still glad you're here.",
            new BaseReply<DailyResultFlag>
            {
                Content = "It's okay. Everyone has tough times.",
                Criteria = (user, result) => user.GetVar(Stats.Common.LongestDailyStreak) >= 14,
                Priority = 1
            },
        };

        public static readonly BaseReply<DailyResultFlag>[] Daily =
        {
            "Enjoy the funds.",
            "Don't mention it. It's the least I could do.",
            new BaseReply<DailyResultFlag>
            {
                Content = "I'll be honest, this is your safest bet to earning **Orite**.",
                Criteria = (user, result) => user.Debt > 0,
                Priority = 1
            },
            new BaseReply<DailyResultFlag>
            {
                Content = "I'm glad you checked in today. Welcome.",
                Criteria = (user, result) => user.GetVar(Stats.Common.DailyStreak) == 1,
                Priority = 1
            },
            new BaseReply<DailyResultFlag>
            {
                Content = "If you ever need helpful tips from time to time, try sticking around.",
                Criteria = (user, result) => Var.Judge(user, Stats.Common.DailyStreak, v => v >= 1 && v < 5)
            },
            new BaseReply<DailyResultFlag>
            {
                Content = "If you check up on me, I'll make it worth while.",
                Criteria = (user, result) => Var.Judge(user, Stats.Common.DailyStreak, v => v >= 1 && v < 5)
            },
            "It's an honor to see you again.",
            new BaseReply<DailyResultFlag>
            {
                Content = "Anything new lately?",
                Criteria = (user, result) => user.GetVar(Stats.Common.DailyStreak) >= 5,
            },
            new BaseReply<DailyResultFlag>
            {
                Content = "Glad to see you.",
                Criteria = (user, result) => user.GetVar(Stats.Common.DailyStreak) >= 5,
            },
            new BaseReply<DailyResultFlag>
            {
                Content = "Be sure to check in on the shopkeepers. They appreciate the company.",
                Criteria = (user, result) => user.GetVar(Stats.Common.DailyStreak) >= 5,
            },
            new BaseReply<DailyResultFlag>
            {
                Content = "Shops do keep note of how often you visit. It might be wise to visit often.",
                Criteria = (user, result) => user.GetVar(Stats.Common.DailyStreak) >= 5,
            },
            new BaseReply<DailyResultFlag>
            {
                Content = "Glad to see you.",
                Criteria = (user, result) => user.GetVar(Stats.Common.DailyStreak) >= 5,
            },
            new BaseReply<DailyResultFlag>
            {
                Content = "I Hope you're having a good day! I understand that times get tough.",
                Criteria = (user, result) => user.GetVar(Stats.Common.DailyStreak) >= 10,
            },
            new BaseReply<DailyResultFlag>
            {
                Content = "While I may not say it often, I appreciate your company.",
                Criteria = (user, result) => user.GetVar(Stats.Common.DailyStreak) >= 30,
            },
            new BaseReply<DailyResultFlag>
            {
                Content = "Your drive to maintain a perfect record is motivating.",
                Criteria = (user, result) => user.GetVar(Stats.Common.DailyStreak) >= 50,
            },
        };

        public static readonly BaseReply<DailyResultFlag>[] DailyCooldown =
        {
            new BaseReply<DailyResultFlag>
            {
                Content = "Don't worry! Your bonus is safe.",
                Criteria = (user, result) => DailyService.IsBonusUpNext(user.GetVar(Stats.Common.DailyStreak))
            },
            "Hang tight. I'm not an OTM.",
            "Patience is key.",
            "Access denied.",
            "Please go find something better to do.",
            "Don't worry. I have already made sure you checked in today.",
            "I already took care of you.",
            "Greed is a road nobody should walk on.",
            "You need to find some hobbies."
        };

        public static readonly BaseReply<DailyResultFlag>[] DailyBonus =
        {
            "I applaud your consistency.",
            "Glad to see you could make it.",
            "Here's something you might like.",
            "You keep the wheel turning and I'll deliver.",
            new BaseReply<DailyResultFlag>
            {
                Content = "You've been quite the attendee.",
                Criteria = (user, result) => user.GetVar(Stats.Common.DailyStreak) == 14,
                Priority = 1
            },
            new BaseReply<DailyResultFlag>
            {
                Content = "Checking in everyday for a month at this point is insane.",
                Criteria = (user, result) => user.GetVar(Stats.Common.DailyStreak) == 30,
                Priority = 1
            },
            new BaseReply<DailyResultFlag>
            {
                Content = "You stay true to yourself, and I appreciate that.",
                Criteria = (user, result) => user.GetVar(Stats.Common.DailyStreak) == 10,
                Priority = 1
            },
            new BaseReply<DailyResultFlag>
            {
                Content = "I might just make you the atomic clock.",
                Criteria = (user, result) => user.GetVar(Stats.Common.DailyStreak) == 100,
                Priority = 1
            },
        };

        public static readonly CasinoReply[] Doubly =
        {
            new CasinoReply
            {
                Criteria = (user, result) => result is DoublerResult t && t.Flag == DoublerResultFlag.Win,
                Content = "Nicely done!"
            },
            new CasinoReply
            {
                Criteria = (user, result) => result is DoublerResult t
                                             && t.Flag == DoublerResultFlag.Win
                                             && t.Multiplier > 5,
                Writer = delegate(ArcadeUser user, ICasinoResult result)
                {
                    if (!(result is DoublerResult t))
                        return "INVALID_RESULT_TYPE";

                    return $"You beat the odds, boosting your bet by an astonishing x**{t.Multiplier:##,0.#}**!";
                }
            },
            new CasinoReply
            {
                Criteria = (user, result) => result is DoublerResult t
                                             && t.Flag == DoublerResultFlag.Win,
                Writer = delegate(ArcadeUser user, ICasinoResult result)
                {
                    if (!(result is DoublerResult t))
                        return "INVALID_RESULT_TYPE";

                    return $"The machine stopped at **{t.ActualTick}** {Format.TryPluralize("tick", t.ActualTick)}.";
                }
            },
            new CasinoReply
            {
                Criteria = (user, result) => result is DoublerResult t && t.Flag == DoublerResultFlag.Exact,
                Content = "Now that was a clean guess."
            },
            new CasinoReply
            {
                Criteria = (user, result) => result is DoublerResult t && t.Flag == DoublerResultFlag.Exact,
                Writer = delegate(ArcadeUser user, ICasinoResult result)
                {
                    if (!(result is DoublerResult t))
                        return "INVALID_RESULT_TYPE";

                    return $"The machine stopped at exactly **{t.ActualTick}** {Format.TryPluralize("tick", t.ActualTick)}. Good guessing!";
                }
            },
            new CasinoReply
            {
                Criteria = (user, result) => result is DoublerResult t && t.Flag == DoublerResultFlag.Lose,
                Content = "Ouch. Sorry about that."
            },
            new CasinoReply
            {
                Criteria = (user, result) => result is DoublerResult t
                                             && t.Flag == DoublerResultFlag.Lose
                                             && t.ActualTick > 0,
                Writer = delegate(ArcadeUser user, ICasinoResult result)
                {
                    if (!(result is DoublerResult t))
                        return "INVALID_RESULT_TYPE";

                    return $"The machine stopped at **{t.ActualTick}** {Format.TryPluralize("tick", t.ActualTick)}.";
                }
            },
            new CasinoReply
            {
                Criteria = (user, result) => result is DoublerResult t
                                             && t.ActualTick != 0
                                             && !(t.ActualTick - t.ExpectedTick).EqualsAny(t.ActualTick, 0),
                Writer = delegate(ArcadeUser user, ICasinoResult result)
                {
                    if (!(result is DoublerResult t))
                        return "INVALID_RESULT_TYPE";

                    int sign = Math.Sign(t.ActualTick - t.ExpectedTick);
                    int diff = Math.Abs(t.ActualTick - t.ExpectedTick);
                    return $"You were **{diff}** {Format.TryPluralize("tick", diff)} {(sign == 1 ? "above" : "below")} **{t.ExpectedTick}**.";
                }
            },
            new CasinoReply
            {
                Criteria = (user, result) => result is DoublerResult t
                                             && t.Flag == DoublerResultFlag.Lose,
                Writer = (user, result) => $"You missed a chance to win {Icons.Chips} **{result.Reward:##,0}**."
            },
            new CasinoReply
            {
                Content = "This machine was already dead.",
                Criteria = (user, result) => result is DoublerResult t
                                             && t.Flag == DoublerResultFlag.Lose
                                             && t.ActualTick == 0
            },
            new CasinoReply
            {
                Content = "You were off by a single tick.",
                Criteria = (user, result) => result is DoublerResult t
                                             && t.Flag == DoublerResultFlag.Lose
                                             && Math.Abs(t.ActualTick - t.ExpectedTick) == 1
            }
        };
    }
}
