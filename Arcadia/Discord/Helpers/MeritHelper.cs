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

        // B   S   G   P    D
        // 10, 25, 50, 100, 250
        public static int GetDefaultScore(int rank)
        {


            return 10 * (2 * Math.Max(rank - 1, 0));
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
                score += merit.Score;
            }

            return score;
        }

        private static string GetPreview(Merit merit)
        {
            string icon = (Check.NotNull(merit.Icon) ? $"{merit.Icon}" : "•");
            return $"> {icon} **{merit.Name}** (**{merit.Score:##,0}**m)";
        }

        private static string GetQuote(Merit merit, bool isUnlocked = false)
        {
            string quote = !Check.NotNull(merit.LockQuote) || isUnlocked ? merit.Quote : merit.LockQuote;

            if (!isUnlocked && merit.Tags.HasFlag(MeritTag.Secret) && !Check.NotNull(merit.LockQuote))
                quote = "Unlock this merit to learn more.";

            if (Check.NotNull(quote))
            {
                string display = $"\"{quote}\"";

                if (merit.Hidden)
                    display = Format.Spoiler(display);

                return $"> {display}";
            }

            return "";
        }

        public static string ViewMerit(Merit merit, ArcadeUser user = null)
        {
            var info = new StringBuilder();
            bool allowTooltips = user?.Config?.Tooltips ?? true;
            bool hasUnlocked = user != null && HasUnlocked(user, merit);

            if (merit.Criteria == null)
                info.AppendLine(Format.Warning("This is an exclusive merit."));

            if (allowTooltips)
            {
                var tooltips = new List<string>();

                if (user != null && CanClaim(user, merit))
                    tooltips.Add($"Type `claim {merit.Id}` to claim this merit.");

                if (merit.Hidden)
                    tooltips.Add("This merit is excluded from completion progress.");

                info.AppendLine(Format.Tooltip(tooltips));
            }

            if (merit.Criteria == null || (allowTooltips && (merit.Hidden || user != null && CanClaim(user, merit))))
                info.AppendLine();

            info.AppendLine(GetPreview(merit));

            string quote = GetQuote(merit, hasUnlocked);

            if (Check.NotNull(quote))
                info.AppendLine(quote);

            if (hasUnlocked)
                    info.AppendLine($"> Unlocked: {Format.FullTime(user.Merits[merit.Id].UnlockedAt, '.')}");

            info.AppendLine($"> Rank: **{merit.Rank}**");

            if (merit.Reward != null || merit.Tags.HasFlag(MeritTag.Secret))
                info.AppendLine().Append(ViewReward(merit.Reward, hasUnlocked && user.Merits[merit.Id]?.IsClaimed == true, merit.Tags.HasFlag(MeritTag.Secret)));

            return info.ToString();
        }

        private static string ViewReward(Reward reward, bool isClaimed = false, bool isSecret = false)
        {
            if (reward == null && !isSecret)
                return "";

            // 🎁
            var result = new StringBuilder($"> **Completion Reward**"); // Use localization for this..?

            if (isClaimed)
                result.Append(" (Claimed)");

            result.AppendLine();

            if (!isClaimed && isSecret)
            {
                result.Append("> Unlock this merit to view the possible rewards");
            }

            if (reward.Money > 0)
                result.AppendLine($"> {CurrencyHelper.WriteCost(reward.Money, CurrencyType.Money)} Orite");

            if (reward.Exp > 0)
                result.AppendLine($"> {Icons.Exp} **{reward.Exp:##,0}**");

            if (Check.NotNullOrEmpty(reward.ItemIds))
            {
                foreach ((string itemId, int amount) in reward.ItemIds)
                    result.AppendLine($"> {GetItemPreview(itemId, amount)}");
            }

            return result.ToString();
        }

        public static string GetQueryPreview(Merit merit, ArcadeUser user = null)
        {
            string icon = (Check.NotNull(merit.Icon) ? $"{merit.Icon}" : "•");
            return $"> `{merit.Id}`\n> {icon} **{merit.Name}**{(user != null && HasUnlocked(user, merit) ? "\\*" : "")} (**{merit.Score:##,0}**m)";
        }

        public static Merit GetNewestUnlocked(ArcadeUser user)
        {
            if (user.Merits.Count == 0)
                return null;

            return GetMerit(user.Merits.OrderByDescending(x => x.Value.UnlockedAt).First().Key);
        }

        private static string WriteLastUnlocked(ArcadeUser user)
        {
            Merit merit = GetNewestUnlocked(user);

            if (merit == null)
                return $"The directory of all known milestones.";

            string icon = Check.NotNull(merit.Icon) ? $"{merit.Icon}" : "•";
            return $"Recently Unlocked: {icon} **{merit.Name}** (**{merit.Score:##,0}**m)";
        }

        private static MeritTag GetAvailableTags()
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

            // add tags as a query
            queries.AddRange(GetAvailableTags().GetFlags().Select(x => x.ToString().ToLower()));

            // add ranks as a query
            queries.AddRange(EnumUtils.GetValues<MeritRank>().Select(x => x.ToString().ToLower()));

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
                _ => "You lack the spirit of greatness. Get out there and start hunting."
            };
        }

        private static string GetQuerySubtitle(ArcadeUser user, string query, ref List<Merit> merits)
        {
            int ownCount = merits.Count(x => HasUnlocked(user, x));

            return merits.Count == 0
                ? "This category does not contain any merits."
                : ownCount == 0 ? GetSummary(query)
                : $"Completion: {Format.Percent(ownCount / (double)merits.Count)}";
        }

        public static string View(ArcadeUser user, string query = null, int page = 0, int pageSize = 5)
        {
            bool allowTooltips = user.Config.Tooltips;
            var info = new StringBuilder();

            bool valid = IsValidQuery(user, query);

            if (string.IsNullOrWhiteSpace(query) || !valid)
            {
                if (!string.IsNullOrWhiteSpace(query))
                    info.AppendLine(Format.Warning("An invalid category was specified."));

                if (allowTooltips)
                {
                    var tooltips = new List<string>
                    {
                        "Type `merits <category>` to view all of the merits in a specific category."
                    };

                    if (user.Merits.Any())
                    {
                        tooltips.Add("Type `merit recent` to learn about your recently unlocked merit.");
                    }

                    info.AppendLine(Format.Tooltip(tooltips));
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
            string subtitle = GetQuerySubtitle(user, query, ref merits);

            string header = Locale.GetHeader(Headers.Merits, counter, subtitle, query.ToString(Casing.Pascal));

            IEnumerable<Merit> group = Paginate.GroupAt(merits, page, pageSize);

            if (allowTooltips)
            {
                var tooltips = new List<string>();

                tooltips.Add("Type `merit <id>` to view more details about a specific merit.");

                if (query.Equals("hidden", StringComparison.OrdinalIgnoreCase))
                    tooltips.Add("All merits in this category are excluded from completion progress.");

                if (!query.EqualsAny(StringComparison.OrdinalIgnoreCase, "claimed", "unlocked") && group.Any(x => HasUnlocked(user, x)))
                    tooltips.Add("Unlocked merits are marked with `*`.");

                info.AppendLine(Format.Tooltip(tooltips));
                info.AppendLine();
            }

            info.AppendLine(header);
            info.AppendLine();

            foreach (Merit merit in Paginate.GroupAt(merits, page, pageSize))
                info.AppendLine($"{GetQueryPreview(merit, user)}\n");

            return info.ToString();
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

        private static string WriteUnlockNotice(Merit merit)
            => $"Merit unlocked: **{merit.Name}** (**{merit.Score}**m)";

        public static IEnumerable<Merit> GetClaimable(ArcadeUser user)
            => Assets.Merits.Where(x => CanClaim(user, x.Id));

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
                    result
                        .AppendLine(Format.Tooltip("Type `claim all` to claim all available merits."))
                        .AppendLine();
                }

                result
                    .AppendLine($"> **Claimable Merits**\n")
                    .AppendJoin("\n", GetClaimable(user).Select(x => GetQueryPreview(x, user)));

                return result.ToString();
            }

            if (input.Equals("all", StringComparison.OrdinalIgnoreCase))
                return ClaimAvailable(user);

            if (!Exists(input) || (!HasUnlocked(user, input) && GetMerit(input).Hidden))
                return Format.Warning("An unknown merit was specified.");

            return ClaimAndDisplay(user, GetMerit(input));
        }

        public static string ClaimAndDisplay(ArcadeUser user, Merit merit)
        {
            if (HasUnlocked(user, merit) && user.Merits[merit.Id]?.IsClaimed == true)
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
                user.Give(merit.Reward.Money);
            }

            foreach ((string itemId, int amount) in merit.Reward.ItemIds)
            {
                result.AppendLine($"> {GetItemPreview(itemId, amount)}");
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
            IEnumerable<Merit> toClaim = Assets.Merits.Where(x => CanClaim(user, x.Id)).ToList();

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
                user.Give(money);
            }

            foreach ((string itemId, int amount) in items)
            {
                result.AppendLine($"> {GetItemPreview(itemId, amount)}");
                ItemHelper.GiveItem(user, itemId, amount);
            }

            return result.ToString();
        }

        // TODO: Move item previews over to ItemViewer instead to be used as a common base
        private static string GetItemPreview(string itemId, int amount)
        {
            string icon = ItemHelper.GetIconOrDefault(itemId) ?? "•";
            string name = Check.NotNull(icon) ? ItemHelper.GetBaseName(itemId) : ItemHelper.NameOf(icon);
            string counter = amount > 1 ? $" (x**{amount:##,0}**)" : "";
            return $"`{itemId}` {icon} **{name}**{counter}";
        }
    }
}
