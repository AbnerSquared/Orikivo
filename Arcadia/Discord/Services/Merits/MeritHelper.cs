using System;
using System.Collections.Generic;
using System.Globalization;
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
                    Tag = MeritTag.Common,
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
                    Tag = MeritTag.Common,
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
                    Tag = MeritTag.Common,
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
                    Tag = MeritTag.Common,
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
                    Tag = MeritTag.Common,
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
                    Tag = MeritTag.Common,
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
                    Tag = MeritTag.Common,
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
                    Tag = MeritTag.Common,
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
                    Tag = MeritTag.Common,
                    Rank = MeritRank.Diamond,
                    Score = 100,
                    Quote = "You were there at the start, carving the path to the future.",
                    Hidden = true
                },
                new Merit
                {
                    Id = "casino:liquidation",
                    Name = "Liquidation",
                    Tag = MeritTag.Casino,
                    Rank = MeritRank.Bronze,
                    Score = 5,
                    Quote = "Your requests have been met with gold.",
                    Criteria = user => user.GetVar(GimiStats.TimesGold) > 0
                },
                new Merit
                {
                    Id = "casino:deprivation",
                    Name = "Deprivation",
                    Tag = MeritTag.Casino,
                    Rank = MeritRank.Bronze,
                    Score = 5,
                    Quote = "Your greed has led you to perish under the moonlight.",
                    Criteria = user => user.GetVar(GimiStats.TimesCursed) > 0
                },
                new Merit
                {
                    Id = "casino:golden_touch",
                    Name = "Golden Touch",
                    Tag = MeritTag.Casino,
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
                    Tag = MeritTag.Casino,
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
                    Tag = MeritTag.Casino,
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
                    Tag = MeritTag.Casino,
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
                    Tag = MeritTag.Casino,
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
                    Tag = MeritTag.Casino,
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
                    Tag = MeritTag.Casino,
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
                    Tag = MeritTag.Casino,
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
                    Tag = MeritTag.Casino,
                    Rank = MeritRank.Bronze,
                    Score = 10,
                    Quote = "You have attempted to double your chips 100 times.",
                    Criteria = user => user.GetVar(TickStats.TimesPlayed) >= 100
                },
                new Merit
                {
                    Id = "casino:gimi_advocate",
                    Name = "Gimi Advocate",
                    Tag = MeritTag.Casino,
                    Rank = MeritRank.Silver,
                    Score = 25,
                    Quote = "Despite all of the losses, you've kept requesting 1,000 times at this point.",
                    Criteria = user => user.GetVar(GimiStats.TimesPlayed) >= 1000
                },
                new Merit
                {
                    Id = "casino:gimi_expert",
                    Name = "Gimi Expert",
                    Tag = MeritTag.Casino,
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
                    Tag = MeritTag.Casino,
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
                    Tag = MeritTag.Common,
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
                    Tag = MeritTag.Common,
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
                    Tag = MeritTag.Common,
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
                    Tag = MeritTag.Common,
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
                    Tag = MeritTag.Common,
                    Rank = MeritRank.Bronze,
                    Score = 10,
                    Quote = "With a quick call from the mini debt guardian, your troubles fade into the void.",
                    Criteria = user => user.GetVar($"{Items.PocketLawyer}:times_used") >= 1
                } // TODO: Create automatic item stat tracking
            };

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

        public static string NameOf(string meritId)
            => GetMerit(meritId)?.Name;

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
            bool allowTooltips = user?.Config?.Tooltips ?? true;

            if (merit.Criteria == null)
                info.AppendLine(Format.Warning("This is an exclusive merit."));

            if (allowTooltips)
            {
                var tooltips = new List<string>();

                if (user != null && CanClaim(user, merit))
                    tooltips.Add("Type `claim {merit.Id}` to claim this merit.");

                if (merit.Hidden)
                    tooltips.Add("This merit is excluded from completion progress.");

                info.AppendLine(Format.Tooltip(tooltips));
            }

            if (merit.Criteria == null || (allowTooltips && (merit.Hidden || user != null && CanClaim(user, merit))))
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
                if (HasMerit(user, merit.Id))
                    info.AppendLine($"> Unlocked: {Format.FullTime(user.Merits[merit.Id].AchievedAt, '.')}");

            info.AppendLine($"> Rank: **{merit.Rank.ToString()}**");

            if (merit.Reward != null)
            {
                info.AppendLine();
                info.Append($"> 🎁 **{Format.TryPluralize("Reward", merit.Reward.Count)}**");

                if (user != null)
                {
                    if (HasMerit(user, merit.Id) && user?.Merits[merit.Id]?.IsClaimed == true)
                        info.Append(" (Claimed)");
                }

                info.AppendLine();

                if (merit.Reward.Money > 0)
                    info.AppendLine($"> {Icons.Balance} **{merit.Reward.Money:##,0}**");

                if (merit.Reward.Exp > 0)
                    info.AppendLine($"> {Icons.Exp} **{merit.Reward.Exp:##,0}**");

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
            string icon = (Check.NotNull(merit.Icon) ? $"{merit.Icon}" : "•");
            return $"> `{merit.Id}`\n> {icon} **{merit.Name}**{(user != null && HasMerit(user, merit) ? "\\*" : "")} (**{merit.Score:##,0}**m)";
        }

        public static Merit GetOldest(ArcadeUser user)
        {
            if (user.Merits.Count == 0)
                return null;

            return GetMerit(user.Merits.OrderByDescending(x => x.Value.AchievedAt).First().Key);
        }

        private static string WriteLastUnlocked(ArcadeUser user)
        {
            Merit merit = GetOldest(user);

            if (merit == null)
                return $"The directory of all known milestones.";

            string icon = Check.NotNull(merit.Icon) ? $"{merit.Icon}" : "•";
            return $"Last Unlocked: {icon} **{merit.Name}** (**{merit.Score:##,0}**m)";
        }

        private static MeritTag GetUsedTags()
        {
            MeritTag tag = 0;

            foreach (Merit merit in Merits)
                tag |= merit.Tag;

            return tag;
        }

        private static List<string> GetQueryValues(ArcadeUser user)
        {
            var queries = new List<string>
            {
                "all"
            };

            // add tags as a query
            queries.AddRange(GetUsedTags().GetFlags().Select(x => x.ToString().ToLower()));

            // add ranks as a query
            queries.AddRange(EnumUtils.GetValues<MeritRank>().Select(x => x.ToString().ToLower()));

            if (user.Merits.Any(x => GetMerit(x.Key).Hidden))
                queries.Add("hidden");

            return queries;
        }

        private static string GetQueries(ArcadeUser user)
        {
            return string.Join(" ", GetQueryValues(user).OrderBy(x => x).Select(x => $"`{x}`"));
        }

        private static bool IsValidQuery(ArcadeUser user, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return false;

            return GetQueryValues(user).Contains(query);
        }

        private static IEnumerable<Merit> GetVisible(ArcadeUser user, string query)
        {

            bool isNumber = int.TryParse(query, out int number);

            Func<Merit, bool> comparer;

            if (!isNumber && Enum.TryParse(query, true, out MeritTag tag))
                comparer = x => x.Tag.HasFlag(tag);
            else if (!isNumber && Enum.TryParse(query, true, out MeritRank rank))
                comparer = x => x.Rank == rank;
            else
            {
                comparer = query switch
                {
                    "hidden" => m => m.Hidden,
                    "all" => m => true,
                    _ => null
                };
            }

            return Merits.Where(x => CanView(user, x) && (comparer?.Invoke(x) ?? false));
        }

        private static string GetSummary(string query)
        {
            return query switch
            {
                "hidden" => "Milestones that hide in the depths.",
                "common" => "Common accomplishments for beginners to tackle.",
                "casino" => "Milestones given only to the lucky.",
                _ => "You lack the spirit of greatness. Get out there and start hunting."
            };
        }

        public static string View(ArcadeUser user, string query = null, int page = 0, int pageSize = 5)
        {
            bool allowTooltips = (user?.Config?.Tooltips ?? true);
            var info = new StringBuilder();

            bool valid = IsValidQuery(user, query);

            if (string.IsNullOrWhiteSpace(query) || !valid)
            {
                if (!string.IsNullOrWhiteSpace(query))
                    info.AppendLine(Format.Warning("An invalid category was specified."));

                if (allowTooltips)
                {
                    info.AppendLine(Format.Tooltip("Type `merits <category>` to view all of the merits in a specific category."));
                    info.AppendLine();
                }

                info.AppendLine($"{Locale.GetHeader(Headers.Merits, $"(**{GetScore(user)}**m)", WriteLastUnlocked(user))}\n");
                info.AppendLine($"> **Categories**");
                info.AppendLine($"> {GetQueries(user)}");
                return info.ToString();
            }

            List<Merit> merits = GetVisible(user, query)
                   .OrderBy(x => x.Name)
                   .ToList();

            int pageCount = Paginate.GetPageCount(merits.Count, pageSize);
            string counter = pageCount > 1 ? $"({Format.PageCount(page + 1, pageCount)})" : null;
            int ownCount = merits.Count(x => HasMerit(user, x));
            string subtitle = merits.Count == 0 ? "This category does not contain any merits." : ownCount == 0 ? GetSummary(query) : $"Completion: {Format.Percent(ownCount / (double)merits.Count)}";
            string header = Locale.GetHeader(Headers.Merits, counter, subtitle, query.ToString(Casing.Pascal));

            IEnumerable<Merit> group = Paginate.GroupAt(merits, page, pageSize);

            if (allowTooltips)
            {
                var tooltips = new List<string>();

                tooltips.Add("Type `merit <id>` to view more details about a specific merit.");

                if (query.Equals("hidden", StringComparison.OrdinalIgnoreCase))
                    tooltips.Add("All merits in this category are excluded from completion progress.");

                if (merits.Any(x => HasMerit(user, x)))
                    tooltips.Add("Unlocked merits are marked with `*`.");

                info.AppendLine(Format.Tooltip(tooltips));
                info.AppendLine();
            }

            info.AppendLine(header);
            info.AppendLine();

            foreach (Merit merit in Paginate.GroupAt(merits, page, pageSize))
                info.AppendLine($"{WriteRow(merit, user)}\n");

            return info.ToString();
        }

        public static void UnlockAvailable(ArcadeUser user)
        {
            foreach (Merit merit in Merits.Where(x => CanUnlock(user, x)))
            {
                user.Merits.Add(merit.Id, merit.GetData());
                user.Notifier.Append(WriteUnlockNotice(merit));
            }
        }

        public static bool CanView(ArcadeUser user, Merit merit)
            => !merit.Hidden || HasMerit(user, merit);

        public static bool CanUnlock(ArcadeUser user, Merit merit)
        {
            return merit.Criteria != null && merit.Criteria(user) && !HasMerit(user, merit.Id);
        }

        internal static void Unlock(ArcadeUser user, string meritId)
            => Unlock(user, GetMerit(meritId));

        internal static void Unlock(ArcadeUser user, Merit merit)
        {
            if (!CanUnlock(user, merit))
                return;

            user.Merits.Add(merit.Id, merit.GetData());
            user.Notifier.Append(WriteUnlockNotice(merit));
        }

        public static bool CanClaim(ArcadeUser user, string meritId)
            => CanClaim(user, GetMerit(meritId));

        public static bool CanClaim(ArcadeUser user, Merit merit)
        {
            return HasMerit(user, merit) && user.Merits[merit.Id].IsClaimed != true && merit.Reward != null;
        }

        private static string WriteUnlockNotice(Merit merit)
            => $"Merit unlocked: **{merit.Name}** (**{merit.Score}**m)";

        public static IEnumerable<Merit> GetClaimable(ArcadeUser user)
            => Merits.Where(x => CanClaim(user, x.Id));

        public static bool CanClaim(ArcadeUser user)
            => Merits.Any(x => CanClaim(user, x.Id));

        public static string Claim(ArcadeUser user, string input)
        {
            if (!Check.NotNull(input))
            {
                IEnumerable<Merit> claimable = GetClaimable(user);

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

            if (input.Equals("all", StringComparison.OrdinalIgnoreCase))
                return ClaimAvailable(user);

            if (!Exists(input) || (!HasMerit(user, input) && GetMerit(input).Hidden))
                return Format.Warning("An unknown merit was specified.");

            return Claim(user, GetMerit(input));
        }

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

        public static string ClaimAvailable(ArcadeUser user)
        {
            if (!CanClaim(user))
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

        private static string WriteItem(string itemId, int amount)
        {
            string icon = ItemHelper.IconOf(itemId) ?? "•";
            string name = Check.NotNull(icon) ? ItemHelper.GetBaseName(itemId) : ItemHelper.NameOf(icon);
            string counter = amount > 1 ? $" (x**{amount:##,0}**)" : "";
            return $"`{itemId}` {icon} **{name}**{counter}";
        }
    }
}