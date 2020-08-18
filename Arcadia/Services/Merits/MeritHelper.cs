using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;

namespace Arcadia
{
    public static class MeritHelper
    {
        public static readonly List<Merit> Merits =
            new List<Merit>
            {
                new Merit
                {
                    Id = "generic:ignition",
                    Name = "Ignition",
                    Group = MeritGroup.Generic,
                    Rank = MeritRank.Bronze,
                    Value = 5,
                    Quote = "You have equipped your first booster.",
                    Criteria = user => user.Boosters.Count > 0
                },
                new Merit
                {
                    Id = "generic:progress_pioneer",
                    Name = "Progression Pioneer",
                    Group = MeritGroup.Generic,
                    Rank = MeritRank.Diamond,
                    Value = 100,
                    Quote = "You were there at the start, leading the path to what exists now.",
                    Hidden = true
                },
                new Merit
                {
                    Id = "casino:liquidation",
                    Name = "Liquidation",
                    Group = MeritGroup.Casino,
                    Rank = MeritRank.Bronze,
                    Value = 5,
                    Quote = "Your requests have been met with gold.",
                    Criteria = user => user.GetStat(GimiStats.TimesGold) > 0
                },
                new Merit
                {
                    Id = "casino:deprivation",
                    Name = "Deprivation",
                    Group = MeritGroup.Casino,
                    Rank = MeritRank.Bronze,
                    Value = 5,
                    Quote = "Your greed has led you to perish under the moonlight.",
                    Criteria = user => user.GetStat(GimiStats.TimesCursed) > 0
                },
                new Merit
                {
                    Id = "casino:golden_touch",
                    Name = "Golden Touch",
                    Group = MeritGroup.Casino,
                    Rank = MeritRank.Gold,
                    Value = 50,
                    Quote = "Midas must've gifted you with his abilities.",
                    Criteria = user => user.GetStat(GimiStats.LongestGold) >= 2,
                    Hidden = true
                },
                new Merit
                {
                    Id = "casino:pandoras_box",
                    Name = "Pandora's Box",
                    Group = MeritGroup.Casino,
                    Rank = MeritRank.Gold,
                    Value = 50,
                    Quote = "Your ruthless requests released the worst of this world.",
                    Criteria = user => user.GetStat(GimiStats.LongestCurse) >= 2,
                    Hidden = true
                },
                new Merit
                {
                    Id = "casino:lucky_guesses",
                    Name = "Lucky Guesses",
                    Group = MeritGroup.Casino,
                    Rank = MeritRank.Silver,
                    Value = 25,
                    Quote = "Guessing the exact tick 3 times in a row is quite the feat.",
                    Criteria = user => user.GetStat(TickStats.LongestWinExact) >= 3,
                    Hidden = true
                },
                new Merit
                {
                    Id = "casino:gimi_beginner",
                    Name = "Gimi Beginner",
                    Group = MeritGroup.Casino,
                    Rank = MeritRank.Bronze,
                    Value = 10,
                    Quote = "You have requested funds 100 times.",
                    Criteria = user => user.GetStat(GimiStats.TimesPlayed) >= 100
                },
                new Merit
                {
                    Id = "casino:gimi_advocate",
                    Name = "Gimi Advocate",
                    Group = MeritGroup.Casino,
                    Rank = MeritRank.Silver,
                    Value = 25,
                    Quote = "Despite all of the losses, you've kept requesting 1,000 times at this point.",
                    Criteria = user => user.GetStat(GimiStats.TimesPlayed) >= 1000
                },
                new Merit
                {
                    Id = "casino:gimi_expert",
                    Name = "Gimi Expert",
                    Group = MeritGroup.Casino,
                    Rank = MeritRank.Gold,
                    Value = 50,
                    Quote = "The addiction of your quest for wealth is starting to scare me after 5,000 times.",
                    Criteria = user => user.GetStat(GimiStats.TimesPlayed) >= 5000,
                    Hidden = true,
                    Reward = new Reward
                    {
                        Money = 100
                    }
                },
                new Merit
                {
                    Id = "casino:gimi_maniac",
                    Name = "Gimi Maniac",
                    Group = MeritGroup.Casino,
                    Rank = MeritRank.Diamond,
                    Value = 250,
                    Quote = "No matter what anyone said, you kept going 10,000 times over.",
                    Criteria = user => user.GetStat(GimiStats.TimesPlayed) >= 10000,
                    Hidden = true,
                    Reward = new Reward
                    {
                        ItemIds = new Dictionary<string, int>
                        {
                            [Items.AutomatonGimi] = 1
                        }
                    }
                },
                new Merit
                {
                    Id = "generic:weekly_worker",
                    Name = "Weekly Worker",
                    Group = MeritGroup.Generic,
                    Rank = MeritRank.Bronze,
                    Value = 7,
                    Quote = "You've stopped by for 7 days, making your name known.",
                    Criteria = user => user.GetStat(Stats.LongestDailyStreak) >= 7
                },
                new Merit
                {
                    Id = "generic:monthly_advocate",
                    Name = "Monthly Advocate",
                    Group = MeritGroup.Generic,
                    Rank = MeritRank.Gold,
                    Value = 30,
                    Quote = "30 days have passed, and you have yet to miss a single one.",
                    Criteria = user => user.GetStat(Stats.LongestDailyStreak) >= 30
                },
                new Merit
                {
                    Id = "generic:daily_automaton",
                    Name = "Daily Automaton",
                    Group = MeritGroup.Generic,
                    Rank = MeritRank.Platinum,
                    Value = 100,
                    Quote = "You're still here. Even after 100 days.",
                    Criteria = user => user.GetStat(Stats.LongestDailyStreak) >= 100,
                    Hidden = true
                },
                new Merit
                {
                    Id = "generic:perfect_attendance",
                    Name = "Perfect Attendance",
                    Group = MeritGroup.Generic,
                    Rank = MeritRank.Diamond,
                    Value = 365,
                    Quote = "For an entire year, day by day, you checked in and made yourself noticed.",
                    Criteria = user => user.GetStat(Stats.LongestDailyStreak) >= 365,
                    Hidden = true
                },
                new Merit
                {
                    Id = "generic:escaping_trouble",
                    Name = "Escaping Trouble",
                    Group = MeritGroup.Generic,
                    Rank = MeritRank.Bronze,
                    Value = 10,
                    Quote = "With a quick call from the mini debt guardian, your troubles fade into the void.",
                    Criteria = user => user.GetStat($"{Items.PocketLawyer}:times_used") >= 1
                } // TODO: Create automatic item stat tracking
            };

        public static Merit GetMerit(string id)
        {
            IEnumerable<Merit> merits = Merits.Where(x => x.Id == id);

            if (merits.Count() > 1)
                throw new ArgumentException("There were more than one Merits of the specified ID.");

            return Merits.FirstOrDefault(x => x.Id == id);
        }

        public static bool HasMerit(ArcadeUser user, string id)
            => user.Merits.ContainsKey(id);

        public static bool Exists(string id)
            => Merits.Any(x => x.Id == id);

        public static long GetScore(ArcadeUser user)
        {
            long score = 0;
            foreach ((string id, MeritData data) in user.Merits)
            {
                Merit merit = GetMerit(id);

                score += merit.Value;
            }

            return score;
        }

        public static string Write(Merit merit, ArcadeUser user = null)
        {
            var info = new StringBuilder();

            if (merit.Criteria == null)
            {
                info.AppendLine(Format.Warning("This is an exclusive merit."));
            }

            if (user != null)
            {
                if (HasMerit(user, merit.Id))
                {
                    info.AppendLine($"> 🏆 **Achieved: {Format.FullTime(user.Merits[merit.Id].AchievedAt)}**\n");
                }
            }

            info.AppendLine($"> `{merit.Id}`");
            info.AppendLine($"> **{merit.Name}** • *{merit.Rank.ToString()}* (**{merit.Value:##,0}**m)");

            if (Check.NotNull(merit.Quote))
            {
                info.Append("> ");

                if (merit.Hidden)
                    info.Append("||");

                info.Append(merit.Quote);

                if (merit.Hidden)
                    info.Append("||");

                info.AppendLine();
            }

            if (merit.Reward != null)
            {
                info.AppendLine();
                info.Append("> **Rewards**");

                if (user != null)
                {
                    if (HasMerit(user, merit.Id))
                    {
                        if (user?.Merits[merit.Id]?.IsClaimed == true)
                        {
                            info.Append(" (Claimed!)");
                        }
                    }
                }

                info.AppendLine();

                if (merit.Reward.Money > 0)
                {
                    info.AppendLine($"> • 💸 **{merit.Reward.Money:##,0}**");
                }

                if (merit.Reward.ItemIds != null)
                {
                    foreach ((string itemId, int amount) in merit.Reward.ItemIds)
                    {
                        info.AppendLine(
                            $"> • **{ItemHelper.NameOf(itemId)}**{(amount > 1 ? $" (x**{amount:##,0}**)" : "")}");
                    }
                }
            }

            return info.ToString();
        }

        public static string WriteRow(Merit merit, ArcadeUser user = null)
        {
            var info = new StringBuilder();

            if (merit.Criteria == null)
            {
                info.AppendLine(Format.Warning("This is an exclusive merit."));
            }

            info.AppendLine($"> `{merit.Id}`");
            info.Append("> ");

            if (user != null)
            {
                if (HasMerit(user, merit.Id))
                {
                    info.Append("🔓 ");
                }
            }

            info.AppendLine($"**{merit.Name}** • *{merit.Rank.ToString()}* (**{merit.Value:##,0}**m)");

            if (Check.NotNull(merit.Quote))
            {
                info.Append($"> {(merit.Hidden ? "||": "")}*\"{merit.Quote}\"*{(merit.Hidden ? "||" : "")}");
            }

            return info.ToString();
        }

        private static string GetProgress(ArcadeUser user, MeritQuery flag)
        {
            var progress = new StringBuilder();

            int total = GetTotalOf(user, flag);
            int count = GetCountOf(user, flag);

            progress.Append($"**{count:##,0}");

            if (flag != MeritQuery.Hidden)
                progress.Append($"**/**{total:##,0}");

            progress.Append($" {Format.TryPluralize("merit", flag == MeritQuery.Hidden ? count : total)} unlocked**");

            return progress.ToString();
        }

        private static string GetProgress(ArcadeUser user, MeritGroup group)
        {
            var progress = new StringBuilder();

            int total = GetTotalOf(user, group);

            progress.Append($"**{GetCountOf(user, group):##,0}**/**{total:##,0}");

            progress.Append($" {Format.TryPluralize("merit", total)} unlocked**");

            return progress.ToString();
        }

        public static string View(ArcadeUser user, MeritQuery flag = MeritQuery.Default, int maxAllowedValues = 5)
        {
            var info = new StringBuilder();

            if (flag != MeritQuery.Default)
            {
                info.Append(GetNotice(flag));
            }

            info.Append("> 🏆 **Merits");

            if (flag != MeritQuery.Default)
                info.Append($": {flag.ToString()}");

            info.Append("**");

            if (flag == MeritQuery.Default)
                info.Append($" (**{GetScore(user)}**m)");

            info.AppendLine();

            info.AppendLine($"> {GetSummary(flag)}\n");

            if (flag == MeritQuery.Default)
            {
                foreach (MeritGroup g in MeritGroup.Generic.GetValues())
                {
                    info.AppendLine($"`{g.ToString().ToLower()}` • **{g.ToString()}**\n> {GetProgress(user, g)}\n");
                }

                if (GetCountOf(user, MeritQuery.Hidden) != 0)
                    info.AppendLine($"`{MeritQuery.Hidden.ToString().ToLower()}` • **{MeritQuery.Hidden.ToString()}**\n> {GetProgress(user, MeritQuery.Hidden)}\n");
            }
            else
            {
                int i = 0;
                foreach (Merit merit in Merits.Where(GetInvokerFor(flag, user)).OrderBy(x => x.Name))
                {
                    if (i >= maxAllowedValues)
                        break;

                    if (!HasMerit(user, merit.Id) && merit.Hidden)
                        continue;


                    info.AppendLine($"{WriteRow(merit, user)}\n");
                    i++;
                }

                if (i == 0)
                {
                    info.AppendLine("> *Could not find any achievements for this query.*");
                }
            }

            return info.ToString();
        }

        private static int GetCountOf(ArcadeUser user, MeritGroup group)
            => user.Merits.Select(x => GetMerit(x.Key))
                .Count(x => x.Group == group);

        private static int GetCountOf(ArcadeUser user, MeritQuery flag)
            => user.Merits.Select(x => GetMerit(x.Key))
                .Count(GetInvokerFor(flag, user));

        private static int GetTotalOf(ArcadeUser user, MeritQuery flag)
            => Merits.Count(GetInvokerFor(flag, user));

        private static int GetTotalOf(ArcadeUser user, MeritGroup group)
            => Merits.Count(x => x.Group == group && (!x.Hidden || HasMerit(user, x.Id)));

        // TODO: include hidden counters.
        private static Func<Merit, bool> GetInvokerFor(MeritQuery flag, ArcadeUser user)
        {
            return flag switch
            {
                MeritQuery.Generic => m => m.Group == MeritGroup.Generic && (!m.Hidden || HasMerit(user, m.Id)),
                MeritQuery.Casino => m => m.Group == MeritGroup.Casino && (!m.Hidden || HasMerit(user, m.Id)),
                MeritQuery.Hidden => m => m.Hidden,
                _ => throw new NotSupportedException("Unknown merit flag type")
            };
        }

        private static string GetSummary(MeritQuery flag)
        {
            return flag switch
            {
                MeritQuery.Default => "View the directory of major accomplishments.",
                MeritQuery.Hidden => "*These are accomplishments that triumph over everything done before.*",
                MeritQuery.Generic => "*These are common accomplishments for beginners to tackle.*",
                MeritQuery.Casino => "*These are accomplishments given to the lucky.*",
                _ => "INVALID_FLAG"
            };
        }

        private static string GetNotice(MeritQuery flag)
        {
            return flag switch
            {
                MeritQuery.Hidden => "> 🔧 These merits do not account for total completion.\n\n",
                _ => ""
            };
        }

        public static void TryGiveMerits(ArcadeUser user)
        {
            foreach (Merit merit in Merits.Where(x => IsEligible(user, x)))
            {
                user.Merits.Add(merit.Id, merit.GetData());
                user.Notifier.Append($"Merit unlocked: **{merit.Name}**");
            }
        }

        public static string NameOf(string meritId)
            => GetMerit(meritId).Name;

        public static bool IsEligible(ArcadeUser user, Merit merit)
        {
            if (merit.Criteria == null)
                return !HasMerit(user, merit.Id);

            return merit.Criteria(user) && !HasMerit(user, merit.Id);
        }

        public static void TryUnlock(ArcadeUser user, string meritId)
            => TryUnlock(user, GetMerit(meritId));

        public static bool CanClaim(ArcadeUser user, string meritId)
        {
            if (!HasMerit(user, meritId))
                return false;

            return GetMerit(meritId).Reward != null && user.Merits[meritId].IsClaimed != true;
        }

        public static void TryUnlock(ArcadeUser user, Merit merit)
        {
            if (!IsEligible(user, merit))
                return;

            user.Merits.Add(merit.Id, merit.GetData());
            user.Notifier.Append($"Merit unlocked: **{merit.Name}**");
        }

        public static bool CanClaimAny(ArcadeUser user)
            => Merits.Any(x => CanClaim(user, x.Id));

        // attempts to claim all available merits
        public static string ClaimAll(ArcadeUser user)
        {
            if (!CanClaimAny(user))
                return $"> ⚠️ You don't have any merits that can be claimed.";

            long money = 0;
            var items = new Dictionary<string, int>();

            IEnumerable<Merit> toClaim = Merits.Where(x => CanClaim(user, x.Id));
            foreach (Merit merit in toClaim)
            {
                money += merit.Reward.Money;

                foreach ((string itemId, int amount) in merit.Reward.ItemIds)
                {
                    if (!items.TryAdd(itemId, amount))
                        items[itemId] += amount;
                }

                user.Merits[merit.Id].IsClaimed = true;
            }

            var result = new StringBuilder();

            result.AppendLine($"> You have claimed **{toClaim.Count():##,0} {Format.TryPluralize("merit", toClaim.Count())}** and received:");

            if (money > 0)
            {
                result.AppendLine($"> 💸 **{money:##,0}**");
                user.Give(money, false);
            }

            foreach ((string itemId, int amount) in items)
            {
                result.AppendLine($"> • **{ItemHelper.NameOf(itemId)}**{(amount > 1 ? $" (x**{amount:##,0}**)" : "")}");
                ItemHelper.GiveItem(user, itemId, amount);
            }

            return result.ToString();
        }

        // attempts to claim the specified merit.
        public static string Claim(ArcadeUser user, string meritId)
        {
            Merit merit = GetMerit(meritId);

            if (HasMerit(user, meritId) && user.Merits[merit.Id]?.IsClaimed == true)
                return $"> ⚠️ You have already claimed **{merit.Name}**.";

            if (merit.Reward == null)
                return $"> ⚠️ There are no rewards assigned to **{merit.Name}**.";

            if (!CanClaim(user, meritId))
                return $"> ⚠️ You are unable to claim **{merit.Name}**.";

            var result = new StringBuilder();

            result.AppendLine($"> You have claimed **{merit.Name}** and received:");

            if (merit.Reward.Money > 0)
            {
                result.AppendLine($"> 💸 **{merit.Reward.Money:##,0}**");
                user.Give(merit.Reward.Money, false);
            }

            foreach ((string itemId, int amount) in merit.Reward.ItemIds)
            {
                result.AppendLine($"> • **{ItemHelper.NameOf(itemId)}**{(amount > 1 ? $" (x**{amount:##,0}**)" : "")}");
                ItemHelper.GiveItem(user, itemId, amount);
            }

            user.Merits[merit.Id].IsClaimed = true;
            return result.ToString();
        }
    }
}