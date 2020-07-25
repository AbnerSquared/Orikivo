using Orikivo.Drawing;
using Orikivo.Desync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;

namespace Arcadia
{
    public enum MeritFlag
    {
        Default = 0,
        Hidden = 1,
        Generic = 2,
        Casino = 4
    }

    public static class MeritHelper
    {
        public static readonly List<Merit> Merits =
            new List<Merit>
            {
                new Merit
                {
                    Id = "generic:progress_pioneer",
                    Name = "Progression Pioneer",
                    Group = MeritGroup.Generic,
                    Rank = MeritRank.Diamond,
                    Value = 100,
                    Quote = "You were there at the start, leading the path to what exists now.",
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
                    Id = "casino:precognitive_chance",
                    Name = "Precognitive Chance",
                    Group = MeritGroup.Casino,
                    Rank = MeritRank.Silver,
                    Value = 25,
                    Quote = "Guessing the exact tick 3 times in a row is quite the feat.",
                    Criteria = user => user.GetStat(TickStats.LongestWinExact) >= 3,
                    Hidden = true
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

        public static string Write(Merit merit)
        {
            var info = new StringBuilder();

            if (merit.Criteria == null)
            {
                info.AppendLine(Format.Warning("This is an exclusive merit."));
            }

            info.AppendLine($"> **{merit.Name}** • *{merit.Rank.ToString()}* (**{merit.Value:##,0}**m)");

            if (Check.NotNull(merit.Quote))
            {
                info.Append($"> {(merit.Hidden ? "||": "")}*\"{merit.Quote}\"*{(merit.Hidden ? "||" : "")}");
            }

            return info.ToString();
        }

        private static string GetProgress(ArcadeUser user, MeritFlag flag)
        {
            var progress = new StringBuilder();

            int total = GetTotalOf(flag);

            progress.Append($"**{GetCountOf(user, flag):##,0}");

            if (flag != MeritFlag.Hidden)
                progress.Append($"**/**{total:##,0}");

            progress.Append($"{Format.TryPluralize("merit", total)} unlocked**");

            return progress.ToString();
        }

        private static string GetProgress(ArcadeUser user, MeritGroup group)
        {
            var progress = new StringBuilder();

            int total = GetTotalOf(group);

            progress.Append($"**{GetCountOf(user, group):##,0}**/**{total:##,0}");

            progress.Append($" {Format.TryPluralize("merit", total)} unlocked**");

            return progress.ToString();
        }

        public static string View(ArcadeUser user, MeritFlag flag = MeritFlag.Default, int maxAllowedValues = 8)
        {
            var info = new StringBuilder();

            if (flag != MeritFlag.Default)
            {
                info.Append(GetNotice(flag));
            }

            info.Append("> **Merits");

            if (flag != MeritFlag.Default)
                info.Append($": {flag.ToString()}");

            info.AppendLine("**");

            info.AppendLine($"> {GetSummary(flag)}\n");

            if (flag == MeritFlag.Default)
            {
                foreach (MeritGroup g in MeritGroup.Generic.GetValues())
                {
                    info.AppendLine($"`{g.ToString().ToLower()}` • **{g.ToString()}**\n> {GetProgress(user, g)}\n");
                }

                if (GetCountOf(user, MeritFlag.Hidden) != 0)
                    info.AppendLine($"`{MeritFlag.Hidden.ToString().ToLower()}` • **{MeritFlag.Hidden.ToString()}**\n> {GetProgress(user, MeritFlag.Hidden)}\n");
            }
            else
            {
                int i = 0;
                foreach (Merit merit in Merits.Where(GetInvokerFor(flag)))
                {
                    if (!HasMerit(user, merit.Id) && merit.Hidden)
                        continue;

                    info.AppendLine($"{Write(merit)}\n");
                    i++;
                }

                if (i == 0)
                {
                    info.AppendLine("> *\"I could not find any achievements for you.\"*");
                }
            }

            return info.ToString();
        }

        private static int GetCountOf(ArcadeUser user, MeritGroup group)
            => user.Merits.Select(x => GetMerit(x.Key))
                .Count(x => x.Group == group);

        private static int GetCountOf(ArcadeUser user, MeritFlag flag)
            => user.Merits.Select(x => GetMerit(x.Key))
                .Count(GetInvokerFor(flag));

        private static int GetTotalOf(MeritFlag flag)
            => Merits.Count(GetInvokerFor(flag));

        private static int GetTotalOf(MeritGroup group)
            => Merits.Count(x => x.Group == group);

        private static Func<Merit, bool> GetInvokerFor(MeritFlag flag)
        {
            return flag switch
            {
                MeritFlag.Generic => m => m.Group == MeritGroup.Generic && !m.Hidden,
                MeritFlag.Casino => m => m.Group == MeritGroup.Casino && !m.Hidden,
                MeritFlag.Hidden => m => m.Hidden,
                _ => throw new NotSupportedException("Unknown merit flag type")
            };
        }

        private static string GetSummary(MeritFlag flag)
        {
            return flag switch
            {
                MeritFlag.Default => "View the directory of major accomplishments.",
                MeritFlag.Hidden => "*These are accomplishments that triumph over everything done before.*",
                MeritFlag.Generic => "*These are common accomplishments for beginners to tackle.*",
                MeritFlag.Casino => "*These are accomplishments given to the lucky.*",
                _ => "INVALID_FLAG"
            };
        }

        private static string GetNotice(MeritFlag flag)
        {
            return flag switch
            {
                MeritFlag.Hidden => "> 🔧 These merits do not account for total completion.\n\n",
                _ => ""
            };
        }
    }

    /// <summary>
    /// Handles all methods relating to a <see cref="Merit"/>.
    /// </summary>
    internal static class MeritHandler
    {
        internal static string ViewDefault(ArcadeUser user)
        {
            bool showTooltips = user.Config?.Tooltips ?? false;

            var panel = new StringBuilder();
            panel.AppendLine("> **Merits**");

            if (showTooltips)
                panel.AppendLine("> Use `merits <group>` to view a specific merit category.");

            panel.AppendLine();


            foreach (MeritGroup type in EnumUtils.GetValues<MeritGroup>())
            {
                IEnumerable<string> merits = MeritHelper.Merits.Where(x => x.Group == type).Select(x => x.Id);
                int collected = user.Merits.Keys.Count(k => merits.Contains(k));

                panel.Append($"> **{type.ToString()}**");
                panel.AppendLine(GetProgress(type, collected, merits.Count()));

                string summary = Summarize(type);

                if (Check.NotNull(summary))
                    panel.AppendLine($"> {summary}");
            }

            return panel.ToString();
        }

        private static string GetProgress(MeritGroup group, int collected, int total)
            => CanShowProgress(group) ? $" (`{RangeF.Convert(0, total, 0, 100, collected)}%`)" : "";

        // determines if the % of completion when viewing categories is displayed.
        private static bool CanShowProgress(MeritGroup group)
            => group switch
            {
                _ => true
            };

        // summarizes the merit group
        private static string Summarize(MeritGroup group)
            => group switch
            {
                _ => null
            };

        // view a specific merit category.
        internal static string ViewCategory(ArcadeUser user, MeritGroup group)
        {
            bool showTooltips = user.Config?.Tooltips ?? false;

            var panel = new StringBuilder();

            panel.AppendLine("> **Merits**");
            panel.AppendLine($"> {group.ToString()}");

            panel.AppendLine();

            if (MeritHelper.Merits.Count(x => x.Group.HasFlag(group)) == 0)
                return Format.Warning($"There are no visible **Merits** found under **{group.ToString()}**.");

            if (showTooltips)
            {
                if (user.Merits.Any(x => !x.Value.IsClaimed.GetValueOrDefault(true)))
                    panel.Insert(0, "> Use `claim <id>` to claim the reward from a **Merit**.\n\n");
            }

            foreach (Merit merit in MeritHelper.Merits.Where(x => x.Group.HasFlag(group)))
            {
                panel.AppendLine(GetMeritSummary(user, merit));
            }

            return panel.ToString();
        }

        internal static string GetMeritSummary(ArcadeUser user, Merit merit)
        {
            var summary = new StringBuilder();
            bool unlocked = MeritHelper.HasMerit(user, merit.Id);

            // LINE 1
            summary.Append($"`{merit.Id}`");
            summary.Append(" • ");
            summary.Append($"**{merit.Name}**");
            summary.AppendLine();

            // LINE 2 (?)
            if (Check.NotNull(merit.Summary))
                summary.Append($"⇛ {merit.Summary}");

            summary.AppendLine();

            if (unlocked)
            {
                summary.AppendLine($"> Achieved **{Format.FullTime(user.Merits[merit.Id].AchievedAt)}**");

                if (merit.Reward != null)
                {
                    summary.Append("> **Reward**: ");
                    summary.AppendJoin(", ", merit.Reward.GetNames());

                    if (user.Merits[merit.Id].IsClaimed.HasValue)
                        summary.Append($" ({(user.Merits[merit.Id].IsClaimed.Value ? "Claimed" : "Unclaimed")})");
                }
            }

            return summary.ToString();
        }



        // claim a merit; true if successful.
        internal static bool Claim(ArcadeUser user, string meritId)
        {
            if (Exists(user, meritId))
            {
                Merit merit = MeritHelper.GetMerit(meritId);

                //if (CanReward(user, merit))
                    //ApplyReward(user, merit.Reward);

                user.Merits[meritId].IsClaimed = true;
                return true;
            }

            return false;
        }

        internal static string ClaimAndDisplay(ArcadeUser user, Merit merit)
            => ClaimAndDisplay(user, merit.Id);

        internal static string ClaimAndDisplay(ArcadeUser user, string meritId)
        {
            if (!MeritHelper.Exists(meritId))
                return Format.Warning("The **Merit** you specified doesn't exist.");

            Merit merit = MeritHelper.GetMerit(meritId);

            if (!user.Merits.ContainsKey(meritId))
                return Format.Warning("You haven't met the criteria in order to be able to claim this **Merit**.");

            if (merit.Reward == null)
                return Format.Warning("The **Merit** you specified doesn't have a reward.");

            if (user.Merits[meritId].IsClaimed.GetValueOrDefault(true))
                return Format.Warning("You already claimed this **Merit**.");

            if (Claim(user, meritId))
                return GetRewardSummary(merit.Reward);

            return Format.Warning("An unknown error has occurred.");
        }

        private static string GetRewardSummary(Reward reward)
            => string.Join("\n", reward.GetNames().Select(x => $"• +{x}"));

        // checks if the user has the merit AND the merit exists
        private static bool Exists(ArcadeUser user, string meritId)
            => MeritHelper.Exists(meritId) && MeritHelper.HasMerit(user, meritId);

        // checks if the merit has a reward AND the user hasn't claimed it.
        private static bool CanReward(ArcadeUser user, Merit merit)
            => merit.Reward != null && (!user.Merits[merit.Id].IsClaimed ?? false);

        
    }
}