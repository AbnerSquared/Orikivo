using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;
using Orikivo.Text.Pagination;
using Orikivo.Text;

namespace Arcadia
{
    public static class MeritHelper
    {
        private static class Errors
        {
            public static readonly string UnknownMerit = "An unknown merit was specified";
            public static readonly string MultipleMerits = "There were multiple merits given for the specified ID";
        }

        private static readonly int BaseScoreTier = 10;
        private static readonly int RankPower = 2;

        // B   S   G   P    D
        // 10, 25, 50, 100, 250
        public static int GetDefaultScore(int rank)
        {
            return BaseScoreTier * (RankPower * Math.Max(rank - 1, 0));
        }

        public static Merit GetMerit(string id)
        {
            IEnumerable<Merit> merits = Assets.Merits.Where(x => x.Id == id);

            if (merits.Count() > 1)
                throw new MultiMatchException(Errors.MultipleMerits);

            return Assets.Merits.FirstOrDefault(x => x.Id == id);
        }

        public static MeritData GetEmptyData()
            => new MeritData(DateTime.UtcNow);

        public static bool HasUnlocked(ArcadeUser user, Merit merit)
            => user.Merits.ContainsKey(merit.Id);

        public static bool HasUnlocked(ArcadeUser user, string meritId)
            => user.Merits.ContainsKey(meritId);

        public static bool Exists(string meritId)
            => Assets.Merits.Any(x => x.Id == meritId);

        public static string NameOf(string meritId)
            => GetMerit(meritId)?.Name;

        public static long GetScore(ArcadeUser user)
        {
            long score = 0;

            foreach (string id in user.Merits.Keys)
            {
                Merit merit = GetMerit(id);
                score += GetWorth(merit);
            }

            return score;
        }

        public static long GetWorth(Merit merit)
            => merit.Score <= 0 ? GetDefaultScore((int)merit.Rank) : merit.Score;

        private static string DrawCounter(long score)
            => $"(**{score:##,0}**m)";

        private static string GetPreview(Merit merit)
            => $"> {GetIconOrDefault(merit, "•")} **{merit.Name}** {DrawCounter(merit.Score)}";

        private static bool CanShowQuote(Merit merit, bool isUnlocked)
            => isUnlocked || IsSecret(merit) || Check.NotNull(merit.LockQuote);

        private static string GetQuote(Merit merit, bool isUnlocked = false)
        {
            string quote = (isUnlocked || !Check.NotNull(merit.LockQuote))
                ? merit.Quote
                : merit.LockQuote;

            if (!CanShowQuote(merit, isUnlocked))
                quote = "Unlock this merit to learn more.";

            if (string.IsNullOrWhiteSpace(quote))
                return "";

            return DrawQuote(quote, isUnlocked && IsSecret(merit));
        }

        public static bool IsSecret(Merit merit)
            => merit.Tags.HasFlag(MeritTag.Secret);

        public static bool IsExclusive(Merit merit)
            => merit.Tags.HasFlag(MeritTag.Exclusive);

        public static bool IsMilestone(Merit merit)
            => merit.Tags.HasFlag(MeritTag.Milestone);

        private static string DrawQuote(string quote, bool hidden)
        {
            string result = $"\"{quote}\"";

            if (hidden)
                result = Format.Spoiler(result);

            return $"> {result}";
        }

        public static string ViewMerit(Merit merit, ArcadeUser user = null)
        {
            var info = new StringBuilder();
            bool allowTooltips = user?.Config?.Tooltips ?? true;
            bool hasUnlocked = user != null && HasUnlocked(user, merit);

            if (merit.Tags.HasFlag(MeritTag.Exclusive))
                info.AppendLine(Format.Warning("This is an exclusive merit.")).AppendLine();

            if (allowTooltips)
            {
                var tooltips = new List<string>();

                if (user != null && CanClaim(user, merit))
                    tooltips.Add($"Type `claim {merit.Id}` to claim this merit.");

                if (merit.Hidden)
                    tooltips.Add("Hidden merits do not count towards completion.");

                if (tooltips.Any())
                {
                    info.AppendLine(Format.Tooltip(tooltips)).AppendLine();
                }
            }

            info.AppendLine(GetPreview(merit));

            string quote = GetQuote(merit, hasUnlocked);

            if (Check.NotNull(quote))
                info.AppendLine(quote);

            if (hasUnlocked)
                    info.AppendLine($"> Unlocked: {Format.FullTime(user.Merits[merit.Id].UnlockedAt, '.')}");

            info.AppendLine($"> Rank: **{merit.Rank}**");

            if (merit.Reward != null || merit.Tags.HasFlag(MeritTag.Secret))
                info.AppendLine()
                    .Append(ViewReward(merit.Reward, hasUnlocked && user.Merits[merit.Id]?.IsClaimed == true, merit.Tags.HasFlag(MeritTag.Secret)));

            return info.ToString();
        }

        private static string ViewReward(Reward reward, bool isClaimed = false, bool isSecret = false)
        {
            bool isPlural = !isSecret && reward != null && reward.Count > 1;
            var result = new StringBuilder($"> **Completion {Format.TryPluralize("Reward", "Rewards", isPlural)}**");

            if (isSecret && !isClaimed)
                return result.Append("\n> Unlock this merit to view possible rewards!").ToString();

            if (reward == null)
                return "";

            if (isClaimed)
                result.Append(" (Claimed)");

            result
                .AppendLine()
                .Append(reward.ToString());

            return result.ToString();
        }

        private static string GetUnlockMarker(bool hasUnlocked)
            => hasUnlocked ? "\\*" : "";

        public static string PreviewMerit(Merit merit, ArcadeUser user = null)
        {
            string icon = GetIconOrDefault(merit, "•");
            bool isUnlocked = user != null && HasUnlocked(user, merit);

            return DrawMeritPreview(merit.Id, icon, merit.Name, merit.Score, isUnlocked);
        }

        private static string DrawMeritPreview(string id, string icon, string name, long score, bool isUnlocked)
            => $"> `{id}`\n> {icon} **{name}**{GetUnlockMarker(isUnlocked)} {DrawCounter(score)}";

        public static Merit GetNewestUnlocked(ArcadeUser user)
        {
            if (user.Merits.Count == 0)
                return null;

            return GetMerit(user.Merits.OrderByDescending(x => x.Value.UnlockedAt).First().Key);
        }

        public static string GetIconOrDefault(Merit merit, string fallback = "")
            => merit.Icon ?? fallback;

        private static string GetRecentMeritSubtitle(ArcadeUser user)
        {
            Merit merit = GetNewestUnlocked(user);

            if (merit == null)
                return $"A directory of accomplishments.";

            return $"Recently Unlocked: {GetIconOrDefault(merit, "•")} **{merit.Name}** {DrawCounter(GetWorth(merit))}";
        }

        private static MeritTag GetActiveTags()
        {
            MeritTag tag = 0;

            foreach (Merit merit in Assets.Merits)
                tag |= merit.Tags;

            return tag;
        }

        private static List<string> GetQueryValues(ArcadeUser user)
        {
            var queries = new List<string>
            {
                "all"
            };

            queries.AddRange(GetActiveTags().GetFlagNames().Select(x => x.ToLower()));
            queries.AddRange(EnumUtils.GetValueNames<MeritRank>().Select(x => x.ToLower()));

            if (user.Merits.Any(x => GetMerit(x.Key).Hidden))
                queries.Add("hidden");

            if (user.Merits.Any())
                queries.Add("unlocked");

            if (user.Merits.Any(x => x.Value.IsClaimed == true))
                queries.Add("claimed");

            return queries;
        }

        private static string GetQueries(ArcadeUser user)
        {
            return string.Join(" ", GetQueryValues(user).OrderBy(x => x).Select(x => $"`{x}`"));
        }

        private static bool ValidateQuery(ArcadeUser user, string query)
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
                comparer = x => x.Tags.HasFlag(tag);
            else if (!isNumber && Enum.TryParse(query, true, out MeritRank rank))
                comparer = x => x.Rank == rank;
            else
            {
                comparer = query switch
                {
                    "hidden" => m => m.Hidden,
                    "all" => m => true,
                    "unlocked" => m => HasUnlocked(user, m),
                    "claimed" => m => user.Merits.ContainsKey(m.Id) && user.Merits[m.Id].IsClaimed == true,
                    _ => null
                };
            }

            return Assets.Merits.Where(x => CanView(user, x) && (comparer?.Invoke(x) ?? false));
        }

        private static string GetSummary(string query)
        {
            return query switch
            {
                "hidden" => "Objectives that hide in the depths.",
                "common" => "Common accomplishments for beginners to tackle.",
                "casino" => "Achievements given only to the lucky.",
                "exclusive" => "Rare and limited accolades.",
                "secret" => "Accomplishments that shall soon be revealed.",
                "milestone" => "Merits that will arrive in due time.",
                "unlocked" => "\"That's quite the collection you have.\"",
                _ => "You lack the spirit of greatness. Get out there and start hunting."
            };
        }

        private static string GetQuerySubtitle(ArcadeUser user, string query, ref List<Merit> merits)
        {
            int ownCount = merits.Count(x => HasUnlocked(user, x));

            return merits.Count == 0
                ? "This category does not contain any merits."
                : ownCount == 0 || query == "unlocked" ? GetSummary(query)
                : $"Completion: {Format.Percent(ownCount / (double)merits.Count)}";
        }

        public static string View(ArcadeUser user, string query = null, int page = 0)
        {
            if (ValidateQuery(user, query))
                return ViewQuery(user, query, page);

            var result = new TextBody();

            var header = Locale.GetOrCreateHeader(Headers.Merits);
            header.Extra = DrawCounter(GetScore(user));
            header.Subtitle = GetRecentMeritSubtitle(user);

            result.WithHeader(header);

            if (!string.IsNullOrWhiteSpace(query))
                result.Warning = "An invalid category was specified.";

            result.AppendTip("Type `merits <category>` to view all of the merits in a specific category.");

            if (user.Merits.Any())
                result.AppendTip("Type `merit recent` to learn about your recently unlocked merit.");

            result.WithSection("**Categories**", $"> {GetQueries(user)}");

            return result.Build(user.Config.Tooltips);
        }

        private static readonly int PageLength = 5;

        private static string ViewQuery(ArcadeUser user, string query, int page = 0)
        {
            List<Merit> merits = GetVisible(user, query)
                   .OrderBy(x => x.Name)
                   .ToList();

            var result = new TextBody();

            int pageCount = Paginate.GetPageCount(merits.Count, PageLength);
            page = Paginate.ClampIndex(page, pageCount);

            var header = Locale.GetOrCreateHeader(Headers.Merits);
            header.Extra = Format.PageCount(page + 1, pageCount, "({0})", false);
            header.Subtitle = GetQuerySubtitle(user, query, ref merits);
            header.Group = query.ToString(Casing.Pascal);

            result.WithHeader(header);

            bool allowTooltips = user.Config.Tooltips;

            IEnumerable<Merit> elements = Paginate.GroupAt(merits, page, PageLength);

            result.AppendTip("Type `merit <id>` to view more details about a specific merit.");

            if (query.Equals("hidden", StringComparison.OrdinalIgnoreCase))
                result.AppendTip("Hidden merits do not count towards group completion.");

            if (!query.EqualsAny(StringComparison.OrdinalIgnoreCase, "claimed", "unlocked") && elements.Any(x => HasUnlocked(user, x)))
                result.AppendTip("Unlocked merits are marked with `*`.");

            result.WithSection(null, string.Join("\n\n", elements.Select(x => PreviewMerit(x, user))));

            return result.Build(user.Config.Tooltips);
        }

        public static void UnlockAvailable(ArcadeUser user)
        {
            bool canNotify = user.Config.Notifier.HasFlag(NotifyAllow.Merit);

            foreach (Merit merit in Assets.Merits.Where(x => CanUnlock(user, x)))
            {
                user.Merits.Add(merit.Id, new MeritData(DateTime.UtcNow));

                if (canNotify)
                    user.Notifier.Add(WriteUnlockNotice(merit));
            }
        }

        public static bool CanView(ArcadeUser user, string meritId)
            => CanView(user, GetMerit(meritId) ?? throw new ArgumentException(Errors.UnknownMerit));

        public static bool CanView(ArcadeUser user, Merit merit)
        {
            return !merit.Hidden
                || HasUnlocked(user, merit);
        }

        public static bool CanUnlock(ArcadeUser user, string meritId)
            => CanUnlock(user, GetMerit(meritId) ?? throw new ArgumentException(Errors.UnknownMerit));

        public static bool CanUnlock(ArcadeUser user, Merit merit)
        {
            return merit.Criteria != null
                && merit.Criteria(user)
                && !HasUnlocked(user, merit.Id);
        }

        public static bool CanClaim(ArcadeUser user)
            => Assets.Merits.Any(x => CanClaim(user, x.Id));

        public static bool CanClaim(ArcadeUser user, string meritId)
            => CanClaim(user, GetMerit(meritId) ?? throw new ArgumentException(Errors.UnknownMerit));

        public static bool CanClaim(ArcadeUser user, Merit merit)
        {
            return HasUnlocked(user, merit)
                && user.Merits[merit.Id].IsClaimed != true
                && merit.Reward != null;
        }

        public static void Unlock(ArcadeUser user, string meritId)
            => Unlock(user, GetMerit(meritId) ?? throw new ArgumentException(Errors.UnknownMerit));

        public static void Unlock(ArcadeUser user, Merit merit)
        {
            if (!CanUnlock(user, merit))
                return;

            bool canNotify = user.Config.Notifier.HasFlag(NotifyAllow.Merit);
            user.Merits.Add(merit.Id, GetEmptyData());

            if (canNotify)
                user.Notifier.Add(WriteUnlockNotice(merit));
        }

        private static string DrawBasePreview(Merit merit)
            => DrawBasePreview(GetIconOrDefault(merit, "•"), merit.Name, GetWorth(merit));

        private static string DrawBasePreview(string icon, string name, long score)
            => $"{icon} **{name}** {DrawCounter(score)}";

        private static string WriteUnlockNotice(Merit merit)
            => $"Merit unlocked: {DrawBasePreview(merit)}";

        public static IEnumerable<Merit> GetClaimable(ArcadeUser user)
            => Assets.Merits.Where(x => CanClaim(user, x.Id));

        public static string ClaimAndDisplay(ArcadeUser user, string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                IEnumerable<Merit> claimable = GetClaimable(user);

                if (!Check.NotNullOrEmpty(claimable))
                    return Format.Warning("You don't have any merits you can claim.");

                return new TextBody()
                    .AppendTip("Type `claim all` to claim all available merits.")
                    .WithSection(null, DrawClaimSection(ref claimable, user))
                    .Build(user.Config.Tooltips);
            }

            if (input.Equals("all", StringComparison.OrdinalIgnoreCase))
                return ClaimAvailable(user);

            if (!Exists(input) || (!HasUnlocked(user, input) && GetMerit(input).Hidden))
                return Format.Warning("An unknown merit was specified.");

            return ClaimAndDisplay(user, GetMerit(input));
        }

        private static string DrawClaimSection(ref IEnumerable<Merit> claimable, ArcadeUser user)
        {
            var result = new StringBuilder();

            return result.AppendLine($"> {Format.Bold("Claimable Merits")}")
                .AppendLine()
                .AppendJoin("\n", claimable.Select(x => PreviewMerit(x, user)))
                .ToString();
        }

        public static string ClaimAndDisplay(ArcadeUser user, Merit merit)
        {
            string name = Format.Bold(merit.Name);

            if (HasUnlocked(user, merit) && user.Merits[merit.Id]?.IsClaimed == true)
                return Format.Warning($"You have already claimed {name}.");

            if (merit.Reward == null)
                return Format.Warning($"There are no rewards assigned to {name}.");

            if (!CanClaim(user, merit))
                return Format.Warning($"You are unable to claim {name}.");

            var result = new StringBuilder();

            result.AppendLine($"> You have claimed {name} and received:")
                .Append(merit.Reward.ToString());

            merit.Reward.Apply(user);
            user.Merits[merit.Id].IsClaimed = true;

            return result.ToString();
        }

        public static string ClaimAvailable(ArcadeUser user)
        {
            if (!CanClaim(user))
                return Format.Warning("You don't have any merits that can be claimed.");

            List<Merit> toClaim = GetClaimable(user).ToList();
            var reward = new Reward();

            foreach (Merit merit in toClaim)
            {
                reward.Add(merit.Reward);
                user.Merits[merit.Id].IsClaimed = true;
            }

            var result = new StringBuilder();

            string claimCount = Format.WordCounter("merit", toClaim.Count);
            long scoreWorth = toClaim.Sum(x => GetWorth(x));

            result.AppendLine($"> You have claimed {claimCount} and received:")
                //.AppendLine($"> **{scoreWorth:##,0}**m")
                .Append(reward.ToString());

            reward.Apply(user);

            return result.ToString();
        }
    }
}
