using Discord.Commands;
using Discord.WebSocket;
using Orikivo;
using Orikivo.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcadia
{
    // TODO: Instead of being an enum value, simply make the flag NULL
    public enum LeaderboardFlag
    {
        Default = 0,
        Money = 1,
        Debt = 2,
        Level = 3,
        Custom = 4 // This allows for leaderboards by stats
    }

    public enum LeaderboardSort
    {
        Most = 1,
        Least = 2
    }

    public class Leaderboard
    {
        public Leaderboard(LeaderboardFlag flag, LeaderboardSort sort, bool allowEmptyValues = false, int pageSize = 10)
        {
            Flag = flag;
            Sort = sort;
            AllowEmptyValues = allowEmptyValues;
            PageSize = pageSize;
        }

        public Leaderboard(string statId, LeaderboardSort sort, bool allowEmptyValues = false, int pageSize = 10)
        {
            Flag = LeaderboardFlag.Custom;
            StatId = statId;
            Sort = sort;
            AllowEmptyValues = allowEmptyValues;
            PageSize = pageSize;
        }

        public LeaderboardFlag Flag { get; } = LeaderboardFlag.Default; // If none is specified, just show the leaders of each flag
        public LeaderboardSort Sort { get; } = LeaderboardSort.Most;

        // The STAT to compare.
        public string StatId { get; } = null;

        // Example: User.Balance = 0
        public bool AllowEmptyValues { get; set; } = false;

        public int PageSize { get; set; } = 10;

        private static string GetHeader(LeaderboardFlag flag)
        {
            return flag switch
            {
                LeaderboardFlag.Money => "> **Leaderboard - Wealth**",
                LeaderboardFlag.Debt => "> **Leaderboard - Debt**",
                LeaderboardFlag.Level => "> **Leaderboard - Experience**",
                _ => "> **Leaderboard**"
            };
        }

        private static string GetHeaderQuote(LeaderboardFlag flag)
        {
            return flag switch
            {
                LeaderboardFlag.Default => "> *View the current pioneers of a specific category.*",
                LeaderboardFlag.Money => "> *These are the users that managed to beat all odds.*",
                LeaderboardFlag.Debt => "> *These are the users with enough debt to make a pool.*",
                LeaderboardFlag.Level => "> *These are the users dedicated to Orikivo.*",
                _ => ""
            };
        }

        private static string GetUserTitle(LeaderboardFlag flag)
        {
            return flag switch
            {
                LeaderboardFlag.Money => "The Wealthy",
                LeaderboardFlag.Debt => "The Cursed",
                LeaderboardFlag.Level => "The Experienced",
                _ => "INVALID_FLAG"
            };
        }

        private static string GetFlagSegment(LeaderboardFlag flag)
        {
            return flag switch
            {
                LeaderboardFlag.Money => "with 💸",
                LeaderboardFlag.Debt => "with 📃",
                LeaderboardFlag.Level => "at level",
                _ => "INVALID_FLAG"
            };
        }

        private static readonly string _leaderFormat = "> **{0}**: **{1}** {2} **{3}**";
        private static readonly string _userFormat = "**{0}** ... {1} **{2}**";
        private static readonly string _customFormat = "**{0}** ... {1}";

        // This is only on LeaderboardFlag.Default
        private static string WriteLeader(LeaderboardFlag flag, ArcadeUser user, bool allowEmptyValues = false)
        {
            var title = GetUserTitle(flag);
            var segment = GetFlagSegment(flag);


            if (!allowEmptyValues)
            {
                if (GetValue(user, flag) == 0)
                {
                    return $"> **{title}**: **Nobody!**";
                }
            }

            return flag switch
            {
                LeaderboardFlag.Money => string.Format(_leaderFormat, title, user.Username, segment, user.Balance.ToString("##,0")),
                LeaderboardFlag.Debt => string.Format(_leaderFormat, title, user.Username, segment, user.Debt.ToString("##,0")),
                LeaderboardFlag.Level => string.Format(_leaderFormat, title, user.Username, segment, WriteLevel(user)),
                _ => "INVALID_FLAG"
            };
        }

        private static string WriteUser(LeaderboardFlag flag, ArcadeUser user, string statId = null)
        {
            if (string.IsNullOrWhiteSpace(statId) && flag == LeaderboardFlag.Custom)
                throw new ArgumentException("Cannot use a custom flag if the stat is unspecified.");

            return flag switch
            {
                LeaderboardFlag.Money => string.Format(_userFormat, user.Username, "💸", user.Balance.ToString("##,0")),
                LeaderboardFlag.Debt => string.Format(_userFormat, user.Username, "📃", user.Debt.ToString("##,0")),
                LeaderboardFlag.Level => string.Format(_userFormat, user.Username, "Level", WriteLevel(user)),
                LeaderboardFlag.Custom => string.Format(_customFormat, user.Username, user.GetStat(statId)),
                _ => "INVALID_FLAG"
            };
        }

        private static string WriteLevel(ArcadeUser user)
        {
            var level = $"{user.Level}";

            if (user.Ascent > 0)
                level = $"{user.Ascent}." + level;

            return level;
        }

        public string Write(IEnumerable<ArcadeUser> users, int page = 0)
        {
            var leaderboard = new StringBuilder();

            leaderboard.AppendLine(GetHeader(Flag));

            if (Flag != LeaderboardFlag.Custom)
            {
                leaderboard.AppendLine(GetHeaderQuote(Flag));
            }

            if (Flag == LeaderboardFlag.Default)
            {
                leaderboard.AppendLine();
                leaderboard.AppendLine("**Leaders**");
                leaderboard.AppendLine(WriteLeader(LeaderboardFlag.Money, GetLeader(users, LeaderboardFlag.Money, Sort)));
                leaderboard.AppendLine(WriteLeader(LeaderboardFlag.Debt, GetLeader(users, LeaderboardFlag.Debt, Sort)));
                leaderboard.Append(WriteLeader(LeaderboardFlag.Level, GetLeader(users, LeaderboardFlag.Level, Sort))); // Levels aren't implemented yet.
            }
            else
            {
                leaderboard.AppendLine();
                leaderboard.Append(WriteUsers(users, PageSize * page, PageSize, Flag, Sort, AllowEmptyValues, StatId));
            }

            return leaderboard.ToString();
        }

        private static ArcadeUser GetLeader(IEnumerable<ArcadeUser> users, LeaderboardFlag flag, LeaderboardSort sort)
        {
            return sort switch
            {
                LeaderboardSort.Least => users.OrderBy(x => GetValue(x, flag)).First(),
                _ => users.OrderByDescending(x => GetValue(x, flag)).First()
            };
        }

        private static long GetValue(ArcadeUser user, LeaderboardFlag flag, string statId = null)
        {
            if (string.IsNullOrWhiteSpace(statId) && flag == LeaderboardFlag.Custom)
                throw new ArgumentException("Cannot use a custom flag if the stat is unspecified.");

            return flag switch
            {
                LeaderboardFlag.Money => (long)user.Balance,
                LeaderboardFlag.Debt => (long)user.Debt,
                LeaderboardFlag.Level => (user.Ascent * 100) + user.Level,
                LeaderboardFlag.Custom => user.GetStat(statId),
                _ => 0
            };
        }

        private static string WriteUsers(IEnumerable<ArcadeUser> users, int offset, int capacity, LeaderboardFlag flag, LeaderboardSort sort, bool allowEmptyValues = false, string statId = null)
        {
            if (users.Count() <= offset)
                throw new ArgumentException("The specified offset is larger than the amount of users specified.");

            users = users.Skip(offset);

            var result = new StringBuilder();

            // The indexing is done this way, so that it doesn't have to be at that exact amount.
            int i = 0;
            foreach (ArcadeUser user in users)
            {
                var value = GetValue(user, flag, statId);

                if (!allowEmptyValues && value == 0)
                    continue;

                result.AppendLine(WriteUser(flag, user, statId));
                i++;
            }

            if (i == 0)
            {
                return "No users were provided for this leaderboard.";
            }

            return result.ToString();
        }
    }

    // TODO: Implement dailies, shopping, merits, stats, double or nothing
    // - Missions
    // - Daily
    // - Shopping
    // - Card Customization
    // - Merits
    // - Stats
    // - Double Or Nothing
    // - Leaderboard
    // TODO: Implement an alternate funds that can be traded with others.
    [Name("Common")]
    [Summary("Generic commands that are commonly used.")]
    public class Common : OriModuleBase<ArcadeContext>
    {
        [Command("stats")]
        [RequireUser(AccountHandling.ReadOnly)]
        public async Task GetStatsAsync(SocketUser user = null)
        {
            user ??= Context.User;
            Context.Data.Users.TryGetValue(user.Id, out ArcadeUser account);

            string stats = string.Join("\n", account.Stats.Where(x => !x.Key.StartsWith("cooldown") && x.Value != 0).Select(s => $"`{s.Key}`: {s.Value}"));

            if (string.IsNullOrWhiteSpace(stats))
            {
                stats = "*No stats have been specified!*";
            }

            if (Context.User.Id != user.Id)
                stats = $"> **Stats**: **{user.Username}**\n\n" + stats;

            await Context.Channel.SendMessageAsync(stats);
        }

        // TODO: Implement enum value listings
        [Command("leaderboard"), Alias("top")]
        [Summary("View the current pioneers of a specific category (`money`, `debt`, or `level`).")]
        public async Task GetLeaderboardAsync(LeaderboardFlag flag = LeaderboardFlag.Default, LeaderboardSort sort = LeaderboardSort.Most, int page = 0)
        {
            if (flag == LeaderboardFlag.Custom)
                flag = LeaderboardFlag.Default;

            var board = new Leaderboard(flag, sort);

            var result = board.Write(Context.Data.Users.Values.Values, page);

            await Context.Channel.SendMessageAsync(result);
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("balance"), Alias("money", "bal")]
        public async Task GetMoneyAsync()
        {
            var values = new StringBuilder();

            values.AppendLine($"**Balance**: 💸 **{Context.Account.Balance.ToString("##,0.###")}**");
            values.AppendLine($"**Tokens**: 🏷️ **{Context.Account.TokenBalance.ToString("##,0.###")}**");
            values.AppendLine($"**Debt**: 📃 **{Context.Account.Debt.ToString("##,0.###")}**");

            await Context.Channel.SendMessageAsync(values.ToString());
        }

        // TODO: Place in a CommonProvider, which can take care of these common methods
        // https://github.com/AbnerSquared/Orikivo.Classic/blob/master/legacy/Modules/AlphaModule.cs#L1088
        [RequireUser]
        [Command("daily")]
        public async Task GetDailyAsync()
        {
            long lastTicks = Context.Account.GetStat(Cooldowns.Daily);
            long streak = Context.Account.GetStat(Stats.DailyStreak);
            var message = new MessageBuilder();
            var embedder = Embedder.Default;

            ulong reward = 15;

            TimeSpan sinceLast = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - lastTicks);

            if (lastTicks > 0)
            {
                if (sinceLast.TotalHours < 24)
                {
                    TimeSpan rem = TimeSpan.FromHours(24) - sinceLast;
                    var time = DateTime.UtcNow.Add(rem);
                    embedder.Color = ImmutableColor.NeonRed;
                    embedder.Header = $"** {CasinoReplies.GetHourEmote(time.Hour)} {OriFormat.ShowTime(time)}**";
                    message.Content = $"*\"{CasinoReplies.GetCooldownText()}\"*";
                    message.Embedder = embedder;
                    await Context.Channel.SendMessageAsync(message.Build());
                    return;
                }
            }

            if (sinceLast.TotalHours > 48)
            {
                if (streak > 1)
                {
                    embedder.Color = ImmutableColor.GammaGreen;
                    embedder.Header = $"**+ 💸 {reward.ToString("##,0")}**";
                    message.Content = $"*\"{CasinoReplies.GetResetText()}\"*";
                    Context.Account.SetStat(Stats.DailyStreak, 0);
                    message.Embedder = embedder;
                    await Context.Channel.SendMessageAsync(message.Build());
                    return;
                }
            }

            Context.Account.UpdateStat(Stats.DailyStreak, 1);

            if (streak >= 5)
            {
                ulong bonus = 50;
                embedder.Color = GammaPalette.Glass[Gamma.Max];
                embedder.Header = $"**+ 💸 {reward.ToString("##,0")} + {bonus.ToString("##,0")}**";
                message.Content = $"*\"{CasinoReplies.GetBonusText()}\"*";
                reward += bonus;
                Context.Account.SetStat(Stats.DailyStreak, 0);
            }
            else
            {
                embedder.Color = ImmutableColor.GammaGreen;
                embedder.Header = $"**+ 💸 {reward.ToString("##,0")}**";
                message.Content = $"*\"{CasinoReplies.GetDailyText()}\"*";
            }

            Context.Account.SetStat(Cooldowns.Daily, DateTime.UtcNow.Ticks);
            Context.Account.Give((long)reward);

            message.Embedder = embedder;
            await Context.Channel.SendMessageAsync(message.Build());
        }

        [RequireUser]
        [Command("card")]
        public async Task GetCardAsync(SocketUser user = null)
        {
            user ??= Context.User;
            if (!Context.TryGetUser(user.Id, out ArcadeUser account))
            {
                await Context.Channel.ThrowAsync("The specified user does not have an existing account.");
                return;
            }

            try
            {
                using (var graphics = new GraphicsService())
                {
                    CardDetails d = new CardDetails(account, user);

                    CardProperties p = CardProperties.Default;
                    p.Palette = PaletteType.Glass;
                    p.Trim = false;
                    p.Casing = Casing.Upper;

                    var card = graphics.DrawCard(d, p);

                    await Context.Channel.SendImageAsync(card, $"../tmp/{Context.User.Id}_card.png");
                }
            }
            catch (Exception ex)
            {
                await Context.Channel.CatchAsync(ex);
            }
        }
    }
}