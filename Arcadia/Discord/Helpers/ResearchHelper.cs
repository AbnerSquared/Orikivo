using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;
using Orikivo.Desync;

namespace Arcadia.Services
{
    public static class ResearchHelper
    {
        public static long GetResearchTier(ArcadeUser user, Item item)
            => GetResearchTier(user, item.Id);

        public static long GetResearchTier(ArcadeUser user, string itemId)
        {
            return Math.Max(0, user.GetVar(CatalogHelper.GetCatalogId(itemId)) - 2);
        }

        public static string ViewMemos(ArcadeUser user, int page = 0)
        {
            var details = new StringBuilder();

            details.AppendLine($"> 📠 **Memos**");
            details.AppendLine("This is an upcoming feature. Stay tuned.");

            return details.ToString();
        }

        public static string ViewMemo(ArcadeUser user, Item item)
        {
            CatalogStatus status = CatalogHelper.GetCatalogStatus(user, item);

            if (status == CatalogStatus.Unknown)
                return Format.Warning("Unknown item.");

            var details = new StringBuilder();

            details.AppendLine($"> 📋 **Memo: {item.Name}** (No. **{item.MemoId:000}**)");

            if (GetResearchTier(user, item) >= 1 && !string.IsNullOrWhiteSpace(item.Memo))
            {
                details.Append($"```{item.Memo}```");
            }
            else
            {
                if (status == CatalogStatus.Seen)
                {
                    details.Append($"```This item is completely foreign to me. You might need to grab a physical instance of it.```");
                }
                else
                    details.Append($"```This item is confusing to say the least. More research might need to be done.```");
            }

            return details.ToString();
        }

        private static string GetEmptyTips()
            => "\n> **Keep In Mind**\n• Researching consumes the item that you wish to research.\n• Research kits can be crafted to reduce the total amount of time needed.\n• Researching can expand the overall flexibility of an item, allowing for new usage methods.";
        public static string GetResearchId(string itemId)
            => $"research:{itemId}";

        public static string WriteNotice(string itemName, long tier)
            => $"Research Complete: **{itemName}** (Tier **{tier:##,0}**)";

        private static string WriteTooltip()
            => $"Type `research <item_id>` to begin research on an item.";

        private static string WriteInfoTooltip()
            => $"Type `researchinfo <item_id>` to view research tiers for an item.";


        private static string WriteTooltip(string itemId)
            => $"Type `research {itemId}` to begin research.";

        private static string WriteMemoTooltip(string itemId)
            => $"Type `memo {itemId}` to view the research memo.";

        public static string View(ArcadeUser user, int page = 0)
        {
            var details = new StringBuilder();

            return details.ToString();
        }

        private static readonly TimeSpan BaseResearchDuration = TimeSpan.FromHours(12);
        private static readonly int BaseResearchScale = 100; // Scale every 100
        private static readonly float BaseResearchTierScale = 0.5f;

        // Time to research at the specified tier
        public static TimeSpan GetResearchTime(Item item, long tier)
        {
            int rarity = (int) item.Rarity;
            long value = item.Value;
            // Determine research formula.

            // 1 = floor(value / (100 / rarity)) * (1 + (0.5 * (tier - 1)))
            // => floor(750 / (100 / 4)) * (1 + (0.5 * (1 - 1)))
            // => floor(750 / 25) * (1 + (0.5 * 0))
            // => floor(30) * (1 + 0)
            // => 30 * 1
            // => 30
            // 30;

            // t = floor(C / (S / r)) * (1 + (Z * (L - 1)));
            // 1 hr every (100 / MaxRarity - rarity) Rare = 3 100 / 10 - 3 => 100 / 7
            double baseTime = Math.Floor(value / (BaseResearchScale / (double)rarity));
            double scale = 1 + (BaseResearchTierScale * (tier - 1));
            return TimeSpan.FromHours(baseTime * scale);
        }

        private static long GetResearchTicks(ArcadeUser user)
            => user.Stats.Any(x => x.Key.StartsWith("research", StringComparison.OrdinalIgnoreCase))
                ? user.Stats.First(x => x.Key.StartsWith("research", StringComparison.OrdinalIgnoreCase)).Value
                : 0;

        private static string GetResearchId(ArcadeUser user)
            => user.Stats.Any(x => x.Key.StartsWith("research", StringComparison.OrdinalIgnoreCase))
                ? user.Stats.First(x => x.Key.StartsWith("research", StringComparison.OrdinalIgnoreCase)).Key
                : "";

        private static bool HasActiveResearch(ArcadeUser user)
            => user.Stats.Any(x => x.Key.StartsWith("research", StringComparison.OrdinalIgnoreCase));

        public static void CompleteResearch(ArcadeUser user, Item item)
        {
            if (CatalogHelper.GetCatalogStatus(user, item) < CatalogStatus.Known)
                throw new Exception("Expected researched item to be at least known");

            user.AddToVar(CatalogHelper.GetCatalogId(item.Id));
            user.SetVar(GetResearchId(item.Id), 0);

            if (user.Config.Notifier.HasFlag(NotifyAllow.Research))
                user.Notifier.Add(WriteNotice(item.Name, GetResearchTier(user, item)));

        }

        public static string GetCurrentProgress(ArcadeUser user)
        {
            string id = GetResearchId(user);
            long ticks = GetResearchTicks(user);

            if (ticks == 0)
                return "> No target selected.";

            string itemId = Var.GetKey(id);
            Item item = ItemHelper.GetItem(itemId);
            long tier = GetResearchTier(user, itemId);
            string name = item.Name;
            TimeSpan remainder = TimeSpan.FromTicks(ticks - DateTime.UtcNow.Ticks);

            // if remainder <= 0, research is complete
            if (remainder <= TimeSpan.Zero)
            {
                CompleteResearch(user, item);
                return "> No target selected.";
            }

            return $"> Researching: **{name}** (Tier **{tier + 1}**)\n> Time Remaining: **{Format.Countdown(remainder)}**";

        }

        public static void TryCompleteResearch(ArcadeUser user)
        {
            if (!HasActiveResearch(user))
                return;

            string id = GetResearchId(user);
            long ticks = user.GetVar(id);

            string itemId = Var.GetKey(id);

            if (ticks == 0)
            {
                user.SetVar(id, 0);
                return;
            }

            TimeSpan remainder = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - ticks);

            if (remainder >= TimeSpan.Zero)
                CompleteResearch(user, ItemHelper.GetItem(itemId));
        }

        public static bool CanResearch(ArcadeUser user, Item item)
        {
            long tier = GetResearchTier(user, item);
            long maxTier = GetMaxResearchTier(item);

            return tier < maxTier;
        }

        private static long GetMaxResearchTier(Item item)
        {
            if (string.IsNullOrWhiteSpace(item.Memo))
                return 0;

            if (item.ResearchTiers?.Any() ?? false)
                return item.ResearchTiers.Keys.OrderByDescending(x => x).First();

            return 1;
        }

        public static string ViewResearch(ArcadeUser user, Item item)
        {
            bool allowTooltips = user.Config.Tooltips;
            CatalogStatus status = CatalogHelper.GetCatalogStatus(user, item);

            if (status == CatalogStatus.Unknown)
                return Format.Warning("Unknown item.");

            long tier = GetResearchTier(user, item);

            var details = new StringBuilder();

            if (allowTooltips)
            {
                var tooltips = new List<string>();

                if (tier >= 1 && !string.IsNullOrWhiteSpace(item.Memo))
                    tooltips.Add(WriteMemoTooltip(item.Id));

                if (CanResearch(user, item))
                    tooltips.Add(WriteTooltip(item.Id));

                details.Append($"{Format.Tooltip(tooltips)}\n\n");
            }

            details.AppendLine($"> **Research: {item.Name}**");

            if (GetMaxResearchTier(item) == 0)
            {
                details.AppendLine("> Research cannot progress. Unknown entity.");
                return details.ToString();
            }

            details.AppendLine($"> Current Tier: **{tier}**");

            if (status == CatalogStatus.Seen)
            {
                details.AppendLine("• You need to learn more about this item up close before you can begin research.");
                return details.ToString();
            }

            details.AppendLine($"\n> **Tiers**");

            if (!string.IsNullOrWhiteSpace(item.Memo))
                details.AppendLine($"• **Tier 1**: Research memo");

            foreach ((int level, string summary) in item.ResearchTiers.OrderBy(x => x.Key))
                details.AppendLine($"• **Tier {level}**: {summary}");

            return details.ToString();
        }

        public static string ViewProgress(ArcadeUser user)
        {
            bool allowTooltips = user.Config.Tooltips;
            var details = new StringBuilder();

            details.AppendLine(Format.Header("Research", "🔬"));
            details.AppendLine(GetCurrentProgress(user));

            if (!HasActiveResearch(user))
            {
                if (allowTooltips)
                {
                    var tooltips = new List<string>
                    {
                        WriteTooltip(),
                        WriteInfoTooltip()
                    };

                    details.Insert(0, $"{Format.Tooltip(tooltips)}\n\n");
                }
                details.Append(GetEmptyTips());
            }

            // research:su_pl = expires_on

            // view details of the current research underway

            // if none is currently active, show the most recent
            // or a random one

            return details.ToString();
        }

        public static string ResearchItem(ArcadeUser user, Item item)
        {
            var details = new StringBuilder();

            // Include item later on.
            if (HasActiveResearch(user))
                return Format.Warning("You are currently in active research.");

            if (!CanResearch(user, item))
            {
                return Format.Warning("You are unable to research this item at the moment.");
            }

            long nextTier = GetResearchTier(user, item) + 1;
            long itemCount = GetTierItemCount(nextTier);

            if (ItemHelper.GetOwnedAmount(user, item) < itemCount)
                return Format.Warning($"You need **{itemCount}** more {Format.TryPluralize("instance", (int)itemCount)} to further progress research for **{item.Name}**.");

            user.SetVar(GetResearchId(item.Id), DateTime.UtcNow.Add(GetResearchTime(item, nextTier)).Ticks);
            details.AppendLine($"> 🔬 You have started research on **{item.Name}** (Tier **{nextTier}**).");

            return details.ToString();
        }

        private static long GetTierItemCount(long nextTier)
        {
            return nextTier * 2;
        }
    }
}
