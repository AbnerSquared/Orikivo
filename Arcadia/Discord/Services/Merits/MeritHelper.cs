using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;
using Orikivo.Text.Pagination;

namespace Arcadia
{
    public static class MeritHelper
    {
        public static readonly List<Merit> Merits =
            new List<Merit>
            {
                new Merit
                {
                    Id = "common:prisma_infusion",
                    Icon = "🌈",
                    Name = "Prisma Infusion",
                    Group = MeritGroup.Common,
                    Rank = MeritRank.Diamond,
                    Score = 500,
                    Quote = "You have collected every single color available.",
                    Hidden = true
                },
                new Merit
                {
                    Id = "common:color_theory",
                    Icon = "🍸",
                    Name = "Color Theory",
                    Group = MeritGroup.Common,
                    Rank = MeritRank.Silver,
                    Score = 25,
                    Quote = "You have created a new color from other colors.",
                    Hidden = true
                },
                new Merit
                {
                    Id = "common:tinkerer",
                    Icon = "🔨",
                    Name = "Tinkerer",
                    Group = MeritGroup.Common,
                    Rank = MeritRank.Bronze,
                    Score = 5,
                    Quote = "You have crafted an item for the first time.",
                    Criteria = user => user.GetVar(Stats.TimesCrafted) > 0
                },
                new Merit
                {
                    Id = "common:trade_beginner",
                    Icon = "🔂",
                    Name = "Trading Beginner",
                    Group = MeritGroup.Common,
                    Rank = MeritRank.Bronze,
                    Score = 5,
                    Quote = "You have traded with another user for the first time.",
                    Criteria = user => user.GetVar(Stats.TimesTraded) > 0
                },
                new Merit
                {
                    Id = "common:bronze_heart",
                    Icon = "🤎",
                    Name = "Bronze Heart",
                    Group = MeritGroup.Common,
                    Rank = MeritRank.Bronze,
                    Score = 5,
                    Quote = "You were a kind soul and gave someone else an item of your own.",
                    Criteria = user => user.GetVar(Stats.ItemsGifted) > 0
                },
                new Merit
                {
                    Id = "common:silver_heart",
                    Icon = "🤍",
                    Name = "Silver Heart",
                    Group = MeritGroup.Common,
                    Rank = MeritRank.Silver,
                    Score = 25,
                    Quote = "You have been a good person and gifted over 50 items to plenty of people.",
                    Criteria = user => user.GetVar(Stats.ItemsGifted) > 50
                },
                new Merit
                {
                    Id = "common:golden_heart",
                    Icon = "💛",
                    Name = "Golden Heart",
                    Group = MeritGroup.Common,
                    Rank = MeritRank.Gold,
                    Score = 50,
                    Quote = "You have given over 100 items to plenty of people.",
                    Criteria = user => user.GetVar(Stats.ItemsGifted) > 100,
                    Hidden = true
                },
                new Merit
                {
                    Id = "common:ignition",
                    Icon = "🕯️",
                    Name = "Ignition",
                    Group = MeritGroup.Common,
                    Rank = MeritRank.Bronze,
                    Score = 5,
                    Quote = "You have equipped your first booster.",
                    Criteria = user => user.Boosters.Count > 0
                },
                new Merit
                {
                    Id = "common:progress_pioneer",
                    Icon = "🚝",
                    Name = "Progression Pioneer",
                    Group = MeritGroup.Common,
                    Rank = MeritRank.Diamond,
                    Score = 100,
                    Quote = "You were there at the start, carving the path to the future.",
                    Hidden = true
                },
                new Merit
                {
                    Id = "casino:liquidation",
                    Name = "Liquidation",
                    Group = MeritGroup.Casino,
                    Rank = MeritRank.Bronze,
                    Score = 5,
                    Quote = "Your requests have been met with gold.",
                    Criteria = user => user.GetVar(GimiStats.TimesGold) > 0
                },
                new Merit
                {
                    Id = "casino:deprivation",
                    Name = "Deprivation",
                    Group = MeritGroup.Casino,
                    Rank = MeritRank.Bronze,
                    Score = 5,
                    Quote = "Your greed has led you to perish under the moonlight.",
                    Criteria = user => user.GetVar(GimiStats.TimesCursed) > 0
                },
                new Merit
                {
                    Id = "casino:golden_touch",
                    Name = "Golden Touch",
                    Group = MeritGroup.Casino,
                    Rank = MeritRank.Gold,
                    Score = 50,
                    Quote = "Midas must have gifted you with his abilities.",
                    Criteria = user => user.GetVar(GimiStats.LongestGold) >= 2,
                    Hidden = true
                },
                new Merit
                {
                    Id = "casino:pandoras_box",
                    Name = "Pandora's Box",
                    Group = MeritGroup.Casino,
                    Rank = MeritRank.Gold,
                    Score = 50,
                    Quote = "Your ruthless requests released the worst of this world.",
                    Criteria = user => user.GetVar(GimiStats.LongestCurse) >= 2,
                    Hidden = true
                },
                new Merit
                {
                    Id = "casino:lucky_guesses",
                    Name = "Lucky Guesses",
                    Group = MeritGroup.Casino,
                    Rank = MeritRank.Silver,
                    Score = 25,
                    Quote = "Guessing the exact tick 3 times in a row is quite the feat.",
                    Criteria = user => user.GetVar(TickStats.LongestWinExact) >= 3,
                    Hidden = true
                },
                new Merit
                {
                    Id = "casino:gimi_beginner",
                    Name = "Gimi Beginner",
                    Group = MeritGroup.Casino,
                    Rank = MeritRank.Bronze,
                    Score = 10,
                    Quote = "You have requested funds 100 times.",
                    Criteria = user => user.GetVar(GimiStats.TimesPlayed) >= 100
                },
                new Merit
                {
                    Id = "casino:gimi_clover",
                    Icon = "☘️",
                    Name = "Clover of Gimi",
                    Group = MeritGroup.Casino,
                    Rank = MeritRank.Bronze,
                    Score = 10,
                    Quote = "You have won over 20 times in a row in Gimi.",
                    Criteria = user => user.GetVar(GimiStats.LongestWin) >= 20,
                    Hidden = true
                },
                new Merit
                {
                    Id = "casino:gimi_curse",
                    Icon = "🧿",
                    Name = "Curse of Gimi",
                    Group = MeritGroup.Casino,
                    Rank = MeritRank.Bronze,
                    Score = 10,
                    Quote = "You have lost over 20 times in a row in Gimi.",
                    Criteria = user => user.GetVar(GimiStats.LongestLoss) >= 20,
                    Hidden = true
                },
                new Merit
                {
                    Id = "casino:tick_clover",
                    Icon = "☘️",
                    Name = "Clover of Doubler",
                    Group = MeritGroup.Casino,
                    Rank = MeritRank.Bronze,
                    Score = 10,
                    Quote = "You have won over 20 times in a row in Doubler.",
                    Criteria = user => user.GetVar(TickStats.LongestWin) >= 20,
                    Hidden = true
                },
                new Merit
                {
                    Id = "casino:tick_exact_clover",
                    Icon = "🏵️",
                    Name = "Golden Clover of Doubler",
                    Group = MeritGroup.Casino,
                    Rank = MeritRank.Gold,
                    Score = 50,
                    Quote = "You have won over 20 times in a row in Doubler while guessing the exact tick.",
                    Criteria = user => user.GetVar(TickStats.LongestWinExact) >= 20,
                    Hidden = true
                },
                new Merit
                {
                    Id = "casino:tick_beginner",
                    Name = "Doubler Beginner",
                    Group = MeritGroup.Casino,
                    Rank = MeritRank.Bronze,
                    Score = 10,
                    Quote = "You have attempted to double your chips 100 times.",
                    Criteria = user => user.GetVar(TickStats.TimesPlayed) >= 100
                },
                new Merit
                {
                    Id = "casino:gimi_advocate",
                    Name = "Gimi Advocate",
                    Group = MeritGroup.Casino,
                    Rank = MeritRank.Silver,
                    Score = 25,
                    Quote = "Despite all of the losses, you've kept requesting 1,000 times at this point.",
                    Criteria = user => user.GetVar(GimiStats.TimesPlayed) >= 1000
                },
                new Merit
                {
                    Id = "casino:gimi_expert",
                    Name = "Gimi Expert",
                    Group = MeritGroup.Casino,
                    Rank = MeritRank.Gold,
                    Score = 50,
                    Quote = "The addiction of your quest for wealth is starting to scare me after 5,000 times.",
                    Criteria = user => user.GetVar(GimiStats.TimesPlayed) >= 5000,
                    Hidden = true,
                    Reward = new Reward
                    {
                        Money = 100
                    }
                },
                new Merit
                {
                    Id = "casino:gimi_maniac",
                    Icon = "⚗️",
                    Name = "Gimi Maniac",
                    Group = MeritGroup.Casino,
                    Rank = MeritRank.Diamond,
                    Score = 250,
                    Quote = "No matter what anyone said, you kept going 10,000 times over.",
                    Criteria = user => user.GetVar(GimiStats.TimesPlayed) >= 10000,
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
                    Id = "common:weekly_worker",
                    Icon = "✨",
                    Name = "Weekly Worker",
                    Group = MeritGroup.Common,
                    Rank = MeritRank.Bronze,
                    Score = 7,
                    Quote = "You've stopped by for 7 days, making your name known.",
                    Criteria = user => user.GetVar(Stats.LongestDailyStreak) >= 7
                },
                new Merit
                {
                    Id = "common:monthly_advocate",
                    Icon = "⭐",
                    Name = "Monthly Advocate",
                    Group = MeritGroup.Common,
                    Rank = MeritRank.Gold,
                    Score = 30,
                    Quote = "30 days have passed, and you have yet to miss a single one.",
                    Criteria = user => user.GetVar(Stats.LongestDailyStreak) >= 30
                },
                new Merit
                {
                    Id = "common:daily_automaton",
                    Icon = "💫",
                    Name = "Daily Automaton",
                    Group = MeritGroup.Common,
                    Rank = MeritRank.Platinum,
                    Score = 100,
                    Quote = "You're still here. Even after 100 days.",
                    Criteria = user => user.GetVar(Stats.LongestDailyStreak) >= 100,
                    Hidden = true
                },
                new Merit
                {
                    Id = "common:perfect_attendance",
                    Icon = "🌟",
                    Name = "Perfect Attendance",
                    Group = MeritGroup.Common,
                    Rank = MeritRank.Diamond,
                    Score = 365,
                    Quote = "For an entire year, day by day, you checked in and made yourself noticed.",
                    Criteria = user => user.GetVar(Stats.LongestDailyStreak) >= 365,
                    Hidden = true
                },
                new Merit
                {
                    Id = "common:escaping_trouble",
                    Icon = "☎️",
                    Name = "Escaping Trouble",
                    Group = MeritGroup.Common,
                    Rank = MeritRank.Bronze,
                    Score = 10,
                    Quote = "With a quick call from the mini debt guardian, your troubles fade into the void.",
                    Criteria = user => user.GetVar($"{Items.PocketLawyer}:times_used") >= 1
                } // TODO: Create automatic item stat tracking
            };

        public static string GetTooltip(string tooltip)
            => $"> 🛠️ {tooltip}";

        public static string GetTooltips(in IEnumerable<string> tooltips)
        {
            if (!Check.NotNullOrEmpty(tooltips))
                return "";

            if (tooltips.Count() == 1)
                return GetTooltip(tooltips.First());

            return $"> 🛠️ **Tips**\n{string.Join("\n", tooltips.Select(x => $"• {x}"))}";
        }

        public static Merit GetMerit(string id)
        {
            IEnumerable<Merit> merits = Merits.Where(x => x.Id == id);

            if (merits.Count() > 1)
                throw new ArgumentException("There were more than one Merits of the specified ID.");

            return Merits.FirstOrDefault(x => x.Id == id);
        }

        public static bool HasMerit(ArcadeUser user, Merit merit)
            => user.Merits.ContainsKey(merit.Id);

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

                score += merit.Score;
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

            if (user != null && CanClaim(user, merit))
            {
                info.AppendLine($"> 🛠️ Type `claim {merit.Id}` to claim this merit.");
            }
            else if (merit.Hidden && (user?.Config?.Tooltips ?? false))
            {
                info.AppendLine($"> 🛠️ This merit does not count for total completion.");
            }

            if (merit.Criteria == null || (merit.Hidden && (user?.Config?.Tooltips ?? false)))
                info.AppendLine();

            string icon = (Check.NotNull(merit.Icon) ? $"{merit.Icon}" : "•");
            info.AppendLine($"> {icon} **{merit.Name}** (**{merit.Score:##,0}**m)");

            if (Check.NotNull(merit.Quote))
            {
                info.Append("> ");

                if (merit.Hidden)
                    info.Append("||");

                info.Append($"\"{merit.Quote}\"");

                if (merit.Hidden)
                    info.Append("||");

                info.AppendLine();
            }

            if (user != null)
            {
                if (HasMerit(user, merit.Id))
                {
                    info.AppendLine($"> Unlocked: {Format.FullTime(user.Merits[merit.Id].AchievedAt, '.')}");
                }
            }

            info.AppendLine($"> Rank: **{merit.Rank.ToString()}**");

            if (merit.Reward != null)
            {
                info.AppendLine();
                info.Append($"> 🎁 **{Format.TryPluralize("Reward", merit.Reward.ItemIds.Count + (merit.Reward.Money > 0 ? 1 : 0) + (merit.Reward.Exp > 0 ? 1 : 0))}**");

                if (user != null)
                {
                    if (HasMerit(user, merit.Id))
                    {
                        if (user?.Merits[merit.Id]?.IsClaimed == true)
                        {
                            info.Append(" (Claimed)");
                        }
                    }
                }

                info.AppendLine();

                if (merit.Reward.Money > 0)
                {
                    info.AppendLine($"> 💸 **{merit.Reward.Money:##,0}**");
                }

                if (merit.Reward.ItemIds != null)
                {
                    foreach ((string itemId, int amount) in merit.Reward.ItemIds)
                        info.AppendLine($"> {WriteItem(itemId, amount)}");
                }
            }

            return info.ToString();
        }

        public static string WriteRow(Merit merit, ArcadeUser user = null)
        {
            string icon = (Check.NotNull(merit.Icon) ? $"{merit.Icon} " : "•");
            var info = new StringBuilder();

            info.AppendLine($"> `{merit.Id}`");
            info.AppendLine($"> {icon} **{merit.Name}**{(user != null && HasMerit(user, merit) ? "\\*": "")} (**{merit.Score:##,0}**m)");

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

        public static string View(ArcadeUser user, MeritQuery query = MeritQuery.Default, int page = 0, int pageSize = 5)
        {
            bool allowTooltips = (user?.Config?.Tooltips ?? true);
            var info = new StringBuilder();

            if (query != MeritQuery.Default)
                info.Append(GetNotice(query));

            if (query == MeritQuery.Default)
            {
                info.AppendLine($"{Locale.GetHeader(Headers.Merits, $"(**{GetScore(user)}**m)", GetSummary(query))}\n");

                foreach (MeritGroup g in MeritGroup.Common.GetValues())
                    info.AppendLine($"> `{g.ToString().ToLower()}` • **{g.ToString()}**\n> {GetProgress(user, g)}\n");

                if (GetCountOf(user, MeritQuery.Hidden) != 0)
                    info.AppendLine($"> `{MeritQuery.Hidden.ToString().ToLower()}` • **{MeritQuery.Hidden.ToString()}**\n> {GetProgress(user, MeritQuery.Hidden)}\n");
            }
            else
            {
                List<Merit> merits = Merits
                       .Where(GetInvokerFor(query, user))
                       .Where(x => HasMerit(user, x) || !x.Hidden)
                       .OrderBy(x => x.Name).ToList();

                int pageCount = Paginate.GetPageCount(merits.Count, pageSize);
                string counter = pageCount > 1 ? $"({Format.PageCount(page + 1, pageCount)})" : null;
                string header = Locale.GetHeader(Headers.Merits, counter, group: query.ToString());

                IEnumerable<Merit> group = Paginate.GroupAt(merits, page, pageSize);

                if (allowTooltips)
                {
                    var tooltips = new List<string>();

                    tooltips.Add("Type `merit <id>` to view more details about a specific merit.");

                    if (merits.Any(x => HasMerit(user, x)))
                        tooltips.Add("Unlocked merits are marked with a `*`.");

                    info.AppendLine(GetTooltips(tooltips));
                    info.AppendLine();
                }

                info.AppendLine(header);
                info.AppendLine();

                foreach (Merit merit in Paginate.GroupAt(merits, page, pageSize))
                    info.AppendLine($"{WriteRow(merit, user)}");

                if (merits.Count == 0)
                    info.AppendLine("> *This category does not contain any merits.*");
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
                MeritQuery.Common => m => m.Group == MeritGroup.Common && (!m.Hidden || HasMerit(user, m.Id)),
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
                MeritQuery.Common => "*These are common accomplishments for beginners to tackle.*",
                MeritQuery.Casino => "*These are accomplishments given to the lucky.*",
                _ => "INVALID_FLAG"
            };
        }

        private static string GetNotice(MeritQuery flag)
        {
            return flag switch
            {
                MeritQuery.Hidden => "> 🛠️ These merits do not account for total completion.\n\n",
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
                return false;

            return merit.Criteria(user) && !HasMerit(user, merit.Id);
        }

        internal static void Unlock(ArcadeUser user, string meritId)
            => Unlock(user, GetMerit(meritId));

        internal static void Unlock(ArcadeUser user, Merit merit)
        {
            if (HasMerit(user, merit))
                return;

            if (merit.Criteria != null)
            {
                if (!merit.Criteria(user))
                    return;
            }

            user.Merits.Add(merit.Id, merit.GetData());
            user.Notifier.Append($"Merit unlocked: **{merit.Name}** (**{merit.Score}**m)");
        }

        public static void TryUnlock(ArcadeUser user, string meritId)
            => TryUnlock(user, GetMerit(meritId));

        public static bool CanClaim(ArcadeUser user, string meritId)
        {
            if (!HasMerit(user, meritId))
                return false;

            return GetMerit(meritId).Reward != null && user.Merits[meritId].IsClaimed != true;
        }

        public static bool CanClaim(ArcadeUser user, Merit merit)
        {
            if (!HasMerit(user, merit))
                return false;

            return merit.Reward != null && user.Merits[merit.Id].IsClaimed != true;
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

        public static IEnumerable<Merit> GetClaimable(ArcadeUser user)
            => Merits.Where(x => CanClaim(user, x.Id));

        // attempts to claim all available merits
        public static string ClaimAll(ArcadeUser user)
        {
            if (!CanClaimAny(user))
                return "> ⚠️ You don't have any merits that can be claimed.";

            long money = 0;
            var items = new Dictionary<string, int>();
            IEnumerable<Merit> toClaim = Merits.Where(x => CanClaim(user, x.Id)).ToList();

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
                result.AppendLine($"> {WriteItem(itemId, amount)}");
                ItemHelper.GiveItem(user, itemId, amount);
            }

            return result.ToString();
        }

        public static string Claim(ArcadeUser user, string meritId)
        {
            if (!Check.NotNull(meritId))
            {
                var claimable = GetClaimable(user);

                if (!Check.NotNullOrEmpty(claimable))
                    return Format.Warning("You don't have any merits you can claim.");

                var result = new StringBuilder();

                if (user.Config.Tooltips)
                {
                    result.AppendLine("> 🛠️ Type `claim all` to claim all available merits.");
                    result.AppendLine();
                }

                result.AppendLine($"> **Claimable Merits**\n");
                result.AppendJoin("\n", GetClaimable(user).Select(x => WriteRow(x, user)));

                return result.ToString();
            }

            if (meritId.Equals("all", StringComparison.OrdinalIgnoreCase))
                return ClaimAll(user);

            if (!Exists(meritId) || (!HasMerit(user, meritId) && GetMerit(meritId).Hidden))
                return Format.Warning("An unknown merit was specified.");

            return Claim(user, GetMerit(meritId));
        }

        // attempts to claim the specified merit.
        public static string Claim(ArcadeUser user, Merit merit)
        {
            if (HasMerit(user, merit) && user.Merits[merit.Id]?.IsClaimed == true)
                return $"> ⚠️ You have already claimed **{merit.Name}**.";

            if (merit.Reward == null)
                return $"> ⚠️ There are no rewards assigned to **{merit.Name}**.";

            if (!CanClaim(user, merit))
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
                result.AppendLine($"> {WriteItem(itemId, amount)}");
                ItemHelper.GiveItem(user, itemId, amount);
            }

            user.Merits[merit.Id].IsClaimed = true;
            return result.ToString();
        }

        private static string WriteItem(string itemId, int amount)
        {
            string icon = ItemHelper.IconOf(itemId) ?? "•";
            string name = Check.NotNull(icon) ? ItemHelper.GetBaseName(itemId) : ItemHelper.NameOf(icon);
            string counter = amount > 1 ? $" (x**{amount:##,0}**)" : "";
            return $"`{itemId}` {icon} **{name}**{counter}";
        }
    }
}