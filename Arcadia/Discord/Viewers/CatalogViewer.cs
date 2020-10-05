using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;
using Orikivo.Text.Pagination;

namespace Arcadia.Services
{
    public class CatalogViewer
    {
        public static string View(ArcadeUser user)
        {
            var info = new StringBuilder();

            info.AppendLine("> 🗃️ **Catalog**");
            info.AppendLine("> Learn about all of the items you have discovered so far.");

            if (!CatalogHelper.CanViewCatalog(user))
            {
                info.AppendLine("You haven't seen any items yet. Get out there and explore!");
                return info.ToString();
            }

            foreach (ItemGroup group in Assets.Groups)
            {
                int known = CatalogHelper.GetKnownCount(user, group.Id);
                int seen = CatalogHelper.GetSeenCount(user, group.Id);

                if (known == 0 && seen == 0)
                    continue;

                string icon = Check.NotNull(group.Icon) ? $" {group.Icon} " : "  ";
                info.AppendLine($"\n> `{group.Id}`{icon}**{group.Name}**");

                int count = known == 0 ? seen : known;
                string term = known == 0 ? "seen" : "known";
                info.AppendLine($"> **{count}** {term} {Format.TryPluralize("entry", count)}");
            }

            return info.ToString();
        }

        private static string GetStatusIcon(CatalogStatus status)
        {
            return status switch
            {
                CatalogStatus.Known => "",
                CatalogStatus.Seen => "🔭 ",
                _ => "UNKNOWN_ICON"
            };
        }

        private static readonly int _pageSize = 8;

        private static string GetTooltip()
            => "Use `item <item_id>` to view more details about an item.";

        private static TextList GetGroupSection(string groupId, ArcadeUser user)
        {
            var result = new TextList();

            if (!ItemHelper.GroupExists(groupId))
                throw new Exception("An invalid item group was specified");

            ItemGroup group = ItemHelper.GetGroup(groupId);

            List<Item> entries = Assets.Items
                .Where(x => x.GroupId == groupId && CatalogHelper.GetCatalogStatus(user, x) > CatalogStatus.Unknown)
                .ToList();

            result.Icon = group.Icon?.ToString() ?? "•";
            result.Title = $"**{Format.Plural(group.Name)}** (**{entries.Count}** discovered)";
            result.Values = entries
                .OrderBy(x => x.Name)
                .Select(x => GetItemElement(x, CatalogHelper.GetCatalogStatus(user, x)))
                .ToList();

            return result;
        }

        private static string GetItemElement(Item item, CatalogStatus status)
            => $"{GetStatusIcon(status)} `{item.Id}` **{item.Name}**";

        private static string ViewAll(ArcadeUser user, int page = 0)
        {
            var info = new StringBuilder();

            var result = new TextBody();
            result.Header = Locale.GetOrCreateHeader(Headers.Catalog);
            result.Header.Group = "All";
            result.Tooltips.Add(GetTooltip());

            var entries = Assets.Items
                .Where(x => CatalogHelper.GetCatalogStatus(user, x) != CatalogStatus.Unknown)
                .OrderBy(x => x.GetName()).ToList();

            page = Paginate.ClampIndex(page, _pageSize);
            int pageCount = Paginate.GetPageCount(entries.Count, _pageSize);

            if (pageCount > 1)
                result.Header.Extra = $"({Format.PageCount(Paginate.ClampIndex(page, pageCount) + 1, pageCount)})";

            foreach (Item item in Paginate.GroupAt(entries, page, _pageSize))
            {
                CatalogStatus status = CatalogHelper.GetCatalogStatus(user, item);

                if (status == CatalogStatus.Unknown)
                    continue;

                string icon = item.GetIcon() ?? "•";

                info.AppendLine($"> {GetStatusIcon(status)} `{item.Id}` {icon} **{item.GetName()}**");
            }

            result.Sections.Add(new TextSection { Content = info.ToString() });

            return result.Build(user.Config.Tooltips);
        }

        private static string ViewGroups(ArcadeUser user, int page = 0)
        {
            var result = new TextBody();
            result.Header = Locale.GetOrCreateHeader(Headers.Catalog);
            result.Header.Group = "All";
            result.Tooltips.Add(GetTooltip());

            List<TextSection> groups = GetVisibleGroups(user).ToList();
            int pageCount = Paginate.GetPageCount(groups.Count, 3);

            if (pageCount > 1)
                result.Header.Extra = $"({Format.PageCount(Paginate.ClampIndex(page, pageCount) + 1, pageCount)})";

            result.Sections = Paginate.GroupAt(groups.OrderBy(x => x.Title[2..]), page, 3).ToList();
            return result.Build(user.Config.Tooltips);
        }

        private static IEnumerable<TextSection> GetVisibleGroups(ArcadeUser user)
        {
            return Assets.Groups
                .Where(x => CatalogHelper.GetVisibleCount(user, x.Id) > 0)
                .Select(x => GetGroupSection(x.Id, user) as TextSection);
        }

        public static string Search(ArcadeUser user, string input, int page = 0)
        {
            List<Item> results = CatalogHelper
                .Search(input)
                .Where(x => CatalogHelper.GetCatalogStatus(user, x) > CatalogStatus.Unknown)
                .ToList();

            if (!results.Any())
                return Format.Warning("Unable to find any matching results.");

            var info = new StringBuilder();
            info.AppendLine($"{GetTooltip()}\n");
            info.AppendLine($"> 🗃️ **Catalog Search** (**{results.Count:##,0}** {Format.TryPluralize("result", results.Count)} found)");

            foreach (Item item in Paginate.GroupAt(results, page, _pageSize))
            {
                CatalogStatus status = CatalogHelper.GetCatalogStatus(user, item);
                string icon = item.GetIcon() ?? "•";

                info.Append($"\n> {GetStatusIcon(status)} `{item.Id}` {icon} **{item.GetName()}**");
            }

            return info.ToString();
        }

        public static string View(ArcadeUser user, string query, int page = 0)
        {
            if (!Check.NotNull(query))
                return View(user);

            if (query == "all")
                return ViewAll(user, page);

            if (query == "groups")
                return ViewGroups(user, page);

            if (!ItemHelper.GroupExists(query))
                return Format.Warning("Unable to find the specified group query.");

            int owned = CatalogHelper.GetKnownCount(user, query);
            int seen = CatalogHelper.GetSeenCount(user, query);

            // You are not authorized to view this group query.
            if (owned == 0 && seen == 0)
                return Format.Warning("Unknown group query specified.");

            var info = new StringBuilder();

            ItemGroup group = ItemHelper.GetGroup(query);

            List<Item> entries = Assets.Items
                .Where(x => x.GroupId == query && CatalogHelper.GetCatalogStatus(user, x) != CatalogStatus.Unknown)
                .OrderBy(x => x.GetName()).ToList();

            int pageCount = (int)Math.Ceiling(entries.Count / (double)_pageSize) - 1;
            page = page < 0 ? 0 : page > pageCount ? pageCount : page;

            int offset = page * _pageSize;
            int i = 0;

            string baseIcon = group.Icon.Fallback ?? "🗃️";
            string extra = pageCount > 1 ? $" ({Format.PageCount(page, pageCount)})" : "";
            info.AppendLine($"> {baseIcon} **Catalog: {group.Name}**{extra}");

            info.Append("> Discovery: ");

            if (owned > 0 && seen > 0)
            {
                info.Append($"**{owned:##,0}** owned (**{seen:##,0}** seen)");
            }
            else
            {
                int counter = owned > 0 ? owned : seen;
                string counterName = owned > 0 ? "owned" : "seen";

                info.Append($"**{counter:##,0}** {counterName}");
            }

            info.AppendLine();

            foreach (Item item in entries.Skip(offset))
            {
                if (i >= _pageSize)
                    break;

                CatalogStatus status = CatalogHelper.GetCatalogStatus(user, item);

                if (status == CatalogStatus.Unknown)
                    continue;

                string icon = item.GetIcon() ?? "•";

                info.Append($"\n> {GetStatusIcon(status)} `{item.Id}` {icon} **{item.GetName()}**");
                i++;
            }

            return info.ToString();
        }

        public static string InspectItem(ArcadeContext ctx, ArcadeUser user, ItemData data, int index)
        {
            var details = new StringBuilder();

            Item item = ItemHelper.GetItem(data.Id);

            bool isUnique = data.Data != null;

            string refId = isUnique || data.Seal != null ? $" (`{(isUnique ? $"{data.Data.Id}" : $"{data.TempId}")}`)" : "";

            details.AppendLine($"> {Icons.Inventory} **Inventory: Slot {index + 1}**{refId}");

            if (data.Seal == null)
            {
                details.AppendLine($"> {GetDisplayName(item)} ({item.Rarity})");

                if (Check.NotNullOrEmpty(item.Quotes))
                    details.AppendLine($"> *\"{item.GetQuote()}*\"");
            }
            else
            {
                string sealIcon = ItemHelper.IconOf(data.Seal.ReferenceId);
                details.AppendLine($"> {(Check.NotNull(sealIcon) ? $"{sealIcon} " : "")}**{ItemHelper.NameOf(data.Seal.ReferenceId)}**");
            }

            if (data.Seal?.SenderId != null)
            {
                ctx.TryGetUser(data.Seal.SenderId.Value, out ArcadeUser sender);

                string senderName = sender?.Username ?? "Unknown User";
                details.AppendLine($"> 🧾 **From**: **{senderName}**");
            }

            details.AppendLine("\n> **Details**");

            if (!isUnique)
                details.AppendLine($"• **Stack Count**: **{data.Count:##,0}**");

            details.AppendLine($"📏 **Size**: {InventoryViewer.WriteCapacity(item.Size * data.Count)}");

            if (!ItemHelper.CanTrade(data))
                details.AppendLine($"📫 **Untradable**");
            else if (item.TradeLimit.HasValue && isUnique)
            {
                int remainder = item.TradeLimit.Value - data.Data.TradeCount ?? 0;
                details.AppendLine($"📫 **{remainder}** {Format.TryPluralize("trade", remainder)} remaining");
            }

            if (data.Seal != null)
                return details.ToString();

            if (item.Usage != null)
            {
                if (item.Usage.Durability == 1 && item.Usage.DeleteMode == DeleteMode.Break)
                    details.AppendLine($"🥪 **Consumable**");
                else if (item.Usage.Durability.HasValue && isUnique)
                    details.AppendLine($"❤️ **Durability**: {item.Usage.Durability.Value - data.Data.Durability ?? 0}");
            }

            if (isUnique && data.Data.ExpiresOn.HasValue)
            {
                if (DateTime.UtcNow - data.Data.ExpiresOn.Value >= TimeSpan.Zero)
                    details.AppendLine("💀 **Expired**");
                else
                    details.AppendLine($"💀 Expires in {Format.LongCounter(data.Data.ExpiresOn.Value - DateTime.UtcNow)}");
            }

            if (isUnique)
            {
                TimeSpan? remainder = ItemHelper.GetCooldownRemainder(user, data.Id, data.Data.Id);
                if (remainder.HasValue)
                    details.AppendLine($"🕘 Usable in {Format.LongCounter(remainder.Value)}");
            }

            if (isUnique && (data.Data.Properties.Count > 0 || Check.NotNull(data.Data.Name)))
            {
                details.AppendLine("\n> **Properties**");

                if (Check.NotNull(data.Data.Name))
                    details.AppendLine($"🉐 **Name Tag**: **{data.Data.Name}**");

                foreach ((string id, long value) in data.Data.Properties)
                    details.AppendLine($"• `{id}`: **{value:##,0}**");
            }

            return details.ToString();
        }

        public static string GetDisplayName(Item item)
        {
            string icon = item.GetIcon();
            string name = Check.NotNull(icon) ? item.Name : item.GetName();

            return $"{(Check.NotNull(icon) ? icon : "•")} **{name}**";
        }

        public static string ViewItem(Item item, CatalogStatus status = CatalogStatus.Known)
        {
            // You are not authorized to view this item.
            if (status == CatalogStatus.Unknown)
                return Format.Warning("Unknown item specified.");

            var details = new StringBuilder();

            string icon = item.GetIcon();

            if (!Check.NotNull(icon))
                icon = "📔";

            details.AppendLine($"> {icon} **{item.Name}**");

            if (Check.NotNullOrEmpty(item.Quotes))
                details.AppendLine($"> *\"{item.GetQuote()}\"*");

            if (status == CatalogStatus.Seen)
            {
                details.AppendLine("\n> You do not know enough about this item.");
                return details.ToString();
            }

            if (item.Tag != 0)
            {
                details.Append($"**#** ");
                details.AppendJoin(", ", item.Tag.GetFlags().Select(x => $"`{x.ToString().ToLower()}`"));
                details.AppendLine();
            }

            string summary = item.GetSummary();

            if (Check.NotNull(summary))
                details.AppendLine($"\n```{summary}```");
            else
                details.AppendLine();

            details.AppendLine("> **Details**");
            details.AppendLine($"ID: `{item.Id}`");

            if (Check.NotNull(item.GroupId))
                details.AppendLine($"Group: **{ItemHelper.GetGroup(item.GroupId).Name}** ({ItemHelper.GetGroup(item.GroupId).Rarity.ToString()})");

            details.AppendLine("\n> **Marketing**");
            details.AppendLine($"Rarity: **{item.Rarity.ToString()}** (Relative)");

            if (item.Value > 0)
                details.AppendLine($"Value: {Icons.IconOf(item.Currency)} **{item.Value:##,0}**");

            if (item.Size > 0)
                details.AppendLine($"Storage Size: {InventoryViewer.WriteCapacity(item.Size)}");

            if (!CatalogHelper.HasAttributes(item))
                return details.ToString();

            details.AppendLine("\n> **Attributes**");

            if (ItemHelper.IsUnique(item))
                details.AppendLine("🔸 **Unique**");

            // Instead of reading flags, check to see if any shops that the USER can see has a chance of being able to buy this
            if (item.CanBuy)
                details.AppendLine("🛍️ **Buyable**");

            if (item.CanSell)
                details.AppendLine("📦 **Sellable**");

            string bypass = "";

            if (item.BypassCriteriaOnTrade)
                bypass = " - Bypasses criteria when gifted";

            if (item.TradeLimit.HasValue)
            {
                if (item.TradeLimit.Value > 0)
                    details.Append("📪 **Tradable**");

                if (item.TradeLimit.Value == 1)
                    details.AppendLine($" (**Once**)");
                else if (item.TradeLimit.Value > 1)
                    details.AppendLine($" (x**{item.TradeLimit.Value:##,0}**)");
            }
            else
                details.AppendLine($"📪 **Tradable**{bypass}");

            if (ItemHelper.IsIngredient(item))
                details.AppendLine("🧂 **Crafting Ingredient**");

            if (CraftHelper.CanCraft(item))
                details.AppendLine("📒 **Craftable**");

            if (item.OwnLimit.HasValue)
                details.AppendLine($"📂 **Max Allowed: {item.OwnLimit.Value:##,0}**");

            if (!CatalogHelper.HasUsageAttributes(item))
                return details.ToString();

            if (item.Usage != null)
            {
                if (item.Usage.Action != null)
                    details.AppendLine("🔹 **Usable**");

                if (item.Usage.Durability == 1 && item.Usage.DeleteMode == DeleteMode.Break)
                    details.AppendLine($"🥪 **Consumable**");
                else
                {
                    string removeOn = "";


                    if (item.Usage.DeleteMode.HasFlag(DeleteMode.Break))
                        removeOn = " - Removed on break";

                    if (item.Usage.Durability.HasValue)
                    {
                        details.AppendLine($"❤️ **Durability: {item.Usage.Durability.Value:##,0}**{removeOn}");
                    }
                }

                if (item.Usage.Expiry.HasValue)
                {
                    string expiryHandle = item.Usage.ExpiryTrigger switch
                    {
                        ExpiryTrigger.Use => " - Starts when first used",
                        ExpiryTrigger.Own => " - Starts when owned",
                        ExpiryTrigger.Trade => " - Starts when traded or gifted",
                        _ => ""
                    };

                    details.AppendLine($"💀 **Expiry:** {Format.LongCounter(item.Usage.Expiry.Value)}{expiryHandle}");
                }

                if (item.Usage.Cooldown.HasValue)
                {
                    string cooldownHandle = item.Usage.CooldownMode switch
                    {
                        CooldownMode.Global => " - For all items",
                        CooldownMode.Group => " - For this item group only",
                        CooldownMode.Item => " - For this item only",
                        CooldownMode.Instance => " - For this instance only",
                        _ => ""
                    };

                    details.AppendLine($"🕘 **Cooldown:** {Format.LongCounter(item.Usage.Cooldown.Value)}{cooldownHandle}");
                }
            }

            return details.ToString();
        }
    }
}
