using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;
using Orikivo.Text;
using Orikivo.Text.Pagination;

namespace Arcadia.Services
{
    public class CatalogViewer
    {
        public static string View(ArcadeUser user)
        {
            var result = new TextBody();

            result.Header = Locale.GetOrCreateHeader(Headers.Catalog);

            if (!CatalogHelper.CanViewCatalog(user))
            {
                result.Header.Subtitle = "You haven't seen any items yet. Get out there and explore!";
                return result.Build(user.Config.Tooltips);
            }

            result.Tooltips = new List<string>
            {
                "Type `catalog <category | group>` to view your known items in a specific category or group.",
                "Type `catalog search:<input>` to look for a specific item."
            };

            int count = CatalogHelper.GetVisibleCount(user);

            result.Header.Subtitle = count > 0
                ? $"**{count}** {Format.TryPluralize("item", count)} discovered"
                : "View your log of discovered items.";


            result.Sections.Add(GetCategorySection(user));
            var info = new StringBuilder();

            info.AppendLine();
            info.AppendLine("> **Categories**");

            var categories = new List<string>
            {
                "all", "groups"
            };

            categories.AddRange(EnumUtils.GetValues<ItemTag>().Select(x => x.ToString().ToLower()));

            info.AppendJoin(" ", categories.OrderBy(x => x).Select(x => $"`{x}`"));
            info.AppendLine();

            if (user.Stats.Any(x => x.Key.StartsWith("catalog:", StringComparison.OrdinalIgnoreCase)))
                result.Sections.Add(GetGroupPreviewSection(user));

            return result.Build(user.Config.Tooltips);
        }

        private static TextSection GetCategorySection(ArcadeUser user)
        {
            var categories = new List<string>
            {
                "all", "groups"
            };

            categories.AddRange(EnumUtils.GetValues<ItemFilter>().Select(x => x.ToString().ToLower()));

            var section = new TextSection
            {
                Title = "**Categories**",
                Content = string.Join(' ', categories.OrderBy(x => x).Select(x => $"`{x}`"))
            };

            return section;
        }


        private static TextSection GetGroupPreviewSection(ArcadeUser user)
        {
            var groups = Assets.Groups.Where(x => CatalogHelper.GetVisibleCount(user, x.Id) > 0);

            var section = new TextSection
            {
                Title = "**Groups**",
                Content = string.Join("\n", groups.OrderBy(x => x.Name).Select(x => PreviewGroup(user, x)))
            };

            return section;
        }

        private static string PreviewGroup(ArcadeUser user, ItemGroup group)
        {
            var result = new StringBuilder();
            int count = CatalogHelper.GetVisibleCount(user, group.Id);

            string icon = Check.NotNull(group.Icon) ? $"{group.Icon}" : "•";
            string id = group.Id.Equals(group.Name, StringComparison.OrdinalIgnoreCase) ? "" : $"`{group.Id}` ";
            result.Append($"{id}{icon} **{group.Name}**");
            result.Append($" (**{count}** {Format.TryPluralize("entry", count)})");

            return result.ToString();
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
            => "Type `item <item_id>` to learn more about an item.";

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
            var entries = Assets.Items
                .Where(x => CatalogHelper.GetCatalogStatus(user, x) != CatalogStatus.Unknown)
                .OrderBy(x => x.GetName()).ToList();

            return BuildView(user, entries, "All", null, page);
        }

        private static string ViewGroups(ArcadeUser user, int page = 0)
        {
            var result = new TextBody();
            result.Header = Locale.GetOrCreateHeader(Headers.Catalog);
            result.Header.Group = "Groups";
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

        private static string ViewFilter(ArcadeUser user, ItemFilter filter, int page = 0)
        {
            List<Item> entries = Assets.Items
                .Where(x => CatalogHelper.GetCatalogStatus(user, x) > CatalogStatus.Unknown && CatalogHelper.MeetsFilter(x, filter))
                .OrderBy(x => x.GetName()).ToList();

            return BuildView(user, entries, filter.ToString(), CatalogHelper.GetFilterIcon(filter), page);
        }

        private static string BuildView(ArcadeUser user, List<Item> entries, string title, string icon, int page = 0)
        {
            var result = new TextBody();

            result.Header = Locale.GetOrCreateHeader(Headers.Catalog);
            result.Header.Group = title;

            int owned = CatalogHelper.GetKnownCount(user, entries);
            int seen = CatalogHelper.GetSeenCount(user, entries);

            if (!entries.Any())
            {
                result.Header.Subtitle = "You have not discovered any matching items under this category.";
                return result.Build(user.Config.Tooltips);
            }

            result.Tooltips.Add(GetTooltip());

            if (seen > 0)
                result.Tooltips.Add("Items written in italic are items that have only been seen.");

            int pageCount = Paginate.GetPageCount(entries.Count, _pageSize);
            page = Paginate.ClampIndex(page, pageCount);

            if (pageCount > 1)
                result.Header.Extra = $"({Format.PageCount(page + 1, pageCount)})";

            result.Header.Icon = Check.NotNull(icon) ? icon : "🗃️";
            result.Header.Subtitle = GetDiscoverySubtitle(owned, seen);

            result.Sections.Add(new TextSection
            {
                Content = string.Join("\n", Paginate.GroupAt(entries, page, _pageSize).Select(x => PreviewItem(user, x)))
            });

            return result.Build(user.Config.Tooltips);
        }

        private static string ViewGroup(ArcadeUser user, ItemGroup group, int page = 0)
        {
            List<Item> entries = Assets.Items
                   .Where(x => CatalogHelper.GetCatalogStatus(user, x) > CatalogStatus.Unknown && x.GroupId == group.Id)
                   .OrderBy(x => x.GetName()).ToList();

            return BuildView(user, entries, group.Name, group.Icon?.ToString(), page);
        }

        public static string View(ArcadeUser user, string query, int page = 0)
        {
            if (!Check.NotNull(query))
                return View(user);

            if (query.StartsWith("search:", StringComparison.OrdinalIgnoreCase))
            {
                var reader = new StringReader(query);
                reader.Skip(7);
                reader.SkipWhiteSpace();

                if (!reader.CanRead())
                    return Format.Warning("Failed to specify a search term (`catalog search:<input>`).");

                return Search(user, reader.GetRemaining(), page);
            }

            if (query.Equals("all", StringComparison.OrdinalIgnoreCase))
                return ViewAll(user, page);

            if (query.Equals("groups", StringComparison.OrdinalIgnoreCase))
                return ViewGroups(user, page);

            if (Enum.TryParse(query, true, out ItemFilter filter))
                return ViewFilter(user, filter, page);

            var result = new TextBody();
            result.Header = Locale.GetOrCreateHeader(Headers.Catalog);

            ItemGroup group = ItemHelper.GetGroup(query)
                ?? Assets.Groups.FirstOrDefault(x => Format.Plural(x.Name).Equals(query, StringComparison.OrdinalIgnoreCase));

            if (group == null)
            {
                result.Header.Subtitle = "The group that was specified does not exist.";
                return result.Build(user.Config.Tooltips);
            }

            return ViewGroup(user, group, page);
        }

        private static string PreviewItem(ArcadeUser user, Item item)
        {
            CatalogStatus status = CatalogHelper.GetCatalogStatus(user, item);

            string icon = item.GetIcon() ?? "•";
            string name = status == CatalogStatus.Seen ? Format.Italics(item.GetName()) : Format.Bold(item.GetName());

            return $"`{item.Id}` {icon} {name}";
        }

        private static string GetDiscoverySubtitle(int owned, int seen)
        {
            var result = new StringBuilder("Discovery: ");

            if (owned > 0 && seen > 0)
            {
                result.Append($"**{owned:##,0}** owned (**{seen:##,0}** seen)");
            }
            else
            {
                int counter = owned > 0 ? owned : seen;
                string counterName = owned > 0 ? "owned" : "seen";

                result.Append($"**{counter:##,0}** {counterName}");
            }

            return result.ToString();
        }

        public static string InspectItem(ArcadeContext ctx, ArcadeUser user, ItemData data, int index)
        {
            var details = new StringBuilder();

            Item item = ItemHelper.GetItem(data.Id) ?? ItemHelper.GetItem(Ids.Items.InternalUnknown);


            bool exists = ItemHelper.Exists(data.Id);
            bool isUnique = data.Data != null;

            string refId = !exists ? "" : isUnique || data.Seal != null ? $" (`{(isUnique ? $"{data.Data.Id}" : $"{data.TempId}")}`)" : "";

            details.AppendLine($"> {Icons.Inventory} **Inventory: Slot {index + 1}**{refId}");

            if (data.Seal == null)
            {
                details.Append($"> {GetDisplayName(item)}");

                if (exists)
                    details.Append($" ({item.Rarity})");

                details.AppendLine();

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

            if (!exists)
                return details.ToString();

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
                if (item.Usage.Durability == 1 && item.Usage.DeleteTriggers == DeleteTriggers.Break)
                    details.AppendLine($"🥪 **Consumable**");
                else if (item.Usage.Durability.HasValue && isUnique)
                    details.AppendLine($"❤️ **Durability**: {item.Usage.Durability.Value - data.Data.Durability ?? 0}");
            }

            if (isUnique && data.Data.ExpiresOn.HasValue)
            {
                if (DateTime.UtcNow - data.Data.ExpiresOn.Value >= TimeSpan.Zero)
                    details.AppendLine("💀 **Expired**");
                else
                    details.AppendLine($"💀 Expires in {Format.Counter(data.Data.ExpiresOn.Value - DateTime.UtcNow)}");
            }

            if (isUnique)
            {
                TimeSpan? remainder = ItemHelper.GetCooldownRemainder(user, data.Id, data.Data.Id);
                if (remainder.HasValue)
                    details.AppendLine($"🕘 Usable in {Format.Counter(remainder.Value)}");
            }

            if (isUnique && (data.Data.Properties.Count > 0 || Check.NotNull(data.Data.Name)))
            {
                details.AppendLine("\n> **Properties**");

                if (Check.NotNull(data.Data.Name))
                    details.AppendLine($"🉐 **Name Tag**: **{data.Data.Name}**");

                foreach ((string id, ItemPropertyData property) in data.Data.Properties)
                    details.AppendLine($"• `{id}`: **{property.Value:##,0}**");
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

            if (item.Tags != 0)
            {
                details.Append($"**#** ");
                details.AppendJoin(", ", item.Tags.GetFlags().Select(x => $"`{x.ToString().ToLower()}`"));
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

            if (item.Tags.HasFlag(ItemTag.Orderable))
                details.AppendLine("💳 **Orderable**");

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

                if (item.Usage.Durability == 1 && item.Usage.DeleteTriggers == DeleteTriggers.Break)
                    details.AppendLine($"🥪 **Consumable**");
                else
                {
                    string removeOn = "";


                    if (item.Usage.DeleteTriggers.HasFlag(DeleteTriggers.Break))
                        removeOn = " - Removed on break";

                    if (item.Usage.Durability.HasValue)
                    {
                        details.AppendLine($"❤️ **Durability: {item.Usage.Durability.Value:##,0}**{removeOn}");
                    }
                }

                if (item.Usage.Timer.HasValue)
                {
                    string expiryHandle = item.Usage.ExpireTriggers switch
                    {
                        ExpireTriggers.Use => " - Starts when first used",
                        ExpireTriggers.Own => " - Starts when owned",
                        ExpireTriggers.Trade => " - Starts when traded or gifted",
                        _ => ""
                    };

                    details.AppendLine($"💀 **Expiry:** {Format.Counter(item.Usage.Timer.Value)}{expiryHandle}");
                }

                if (item.Usage.Cooldown.HasValue)
                {
                    string cooldownHandle = item.Usage.CooldownTarget switch
                    {
                        CooldownTarget.Global => " - For all items",
                        CooldownTarget.Group => " - For this item group only",
                        CooldownTarget.Item => " - For this item only",
                        CooldownTarget.Instance => " - For this instance only",
                        _ => ""
                    };

                    details.AppendLine($"🕘 **Cooldown:** {Format.Counter(item.Usage.Cooldown.Value)}{cooldownHandle}");
                }
            }

            return details.ToString();
        }
    }
}
