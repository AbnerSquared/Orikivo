using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;
using Orikivo.Desync;
using Orikivo.Text.Pagination;

namespace Arcadia
{
    public static class MeritHelper
    {
        public static Merit GetMerit(string id)
        {
            IEnumerable<Merit> merits = Assets.Merits.Where(x => x.Id == id);

            if (merits.Count() > 1)
                throw new ArgumentException("There were more than one Merits of the specified ID.");

            return Assets.Merits.FirstOrDefault(x => x.Id == id);
        }

        public static MeritData GetEmptyData()
            => new MeritData(DateTime.UtcNow);
        public static bool HasMerit(ArcadeUser user, Merit merit)
            => user.Merits.ContainsKey(merit.Id);

        public static bool HasMerit(ArcadeUser user, string id)
            => user.Merits.ContainsKey(id);

        public static bool Exists(string id)
            => Assets.Merits.Any(x => x.Id == id);

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
                    tooltips.Add($"Type `claim {merit.Id}` to claim this merit.");

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

            foreach (Merit merit in Assets.Merits)
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

            return Assets.Merits.Where(x => CanView(user, x) && (comparer?.Invoke(x) ?? false));
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
            bool canNotify = user.Config.Notifier.HasFlag(NotifyAllow.Merit);
            foreach (Merit merit in Assets.Merits.Where(x => CanUnlock(user, x)))
            {
                user.Merits.Add(merit.Id, GetEmptyData());

                if (canNotify)
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

            bool canNotify = user.Config.Notifier.HasFlag(NotifyAllow.Merit);
            user.Merits.Add(merit.Id, GetEmptyData());

            if (canNotify)
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
            => Assets.Merits.Where(x => CanClaim(user, x.Id));

        public static bool CanClaim(ArcadeUser user)
            => Assets.Merits.Any(x => CanClaim(user, x.Id));

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