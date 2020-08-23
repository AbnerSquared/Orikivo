using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;

namespace Arcadia.Services
{
    public class Catalog
    {
        public static string View(ArcadeUser user)
        {
            var info = new StringBuilder();

            info.AppendLine("> 🗃️ **Catalog**");
            info.AppendLine("> Learn about all of the items you have discovered so far.");

            if (!ItemHelper.CanViewCatalog(user))
            {
                info.AppendLine("You haven't seen any items yet. Get out there and explore!");
                return info.ToString();
            }

            foreach (ItemGroup group in ItemHelper.Groups)
            {
                int known = ItemHelper.GetKnownCount(user, group.Id);
                int seen = ItemHelper.GetSeenCount(user, group.Id);

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

        private static string ViewAll(ArcadeUser user, int page = 0)
        {
            var info = new StringBuilder();

            info.AppendLine("> 🗃️ **Catalog: All**");
            info.AppendLine("> Use `inspect <item_id>` to learn more about an item entry.");

            var entries = ItemHelper.LItems
                .Where(x => ItemHelper.GetCatalogStatus(user, x) != CatalogStatus.Unknown)
                .OrderBy(x => x.GetName()).ToList();

            int pageCount = (int) Math.Ceiling(entries.Count / (double) _pageSize) - 1;
            page = page < 0 ? 0 : page > pageCount ? pageCount : page;

            int offset = page * _pageSize;
            int i = 0;
            foreach (Item item in entries.Skip(offset))
            {
                if (i >= _pageSize)
                    break;

                CatalogStatus status = ItemHelper.GetCatalogStatus(user, item);

                if (status == CatalogStatus.Unknown)
                    continue;

                string icon = item.GetIcon() ?? "•";

                info.Append($"\n> {GetStatusIcon(status)} `{item.Id}` {icon} **{item.GetName()}**");
                i++;
            }

            return info.ToString();
        }

        public static string View(ArcadeUser user, string query, int page = 0)
        {
            if (!Check.NotNull(query))
                return View(user);

            if (query == "all")
                return ViewAll(user, page);

            if (!ItemHelper.GroupExists(query))
                return Format.Warning("Unable to find the specified group query.");

            if (ItemHelper.GetKnownCount(user, query) == 0
                && ItemHelper.GetSeenCount(user, query) == 0)
                return Format.Warning("You are not authorized to view this group query.");

            var info = new StringBuilder();

            ItemGroup group = ItemHelper.GetGroup(query);

            var entries = ItemHelper.LItems
                .Where(x => x.GroupId == query && ItemHelper.GetCatalogStatus(user, x) != CatalogStatus.Unknown)
                .OrderBy(x => x.GetName()).ToList();

            int pageCount = (int)Math.Ceiling(entries.Count / (double)_pageSize) - 1;
            page = page < 0 ? 0 : page > pageCount ? pageCount : page;

            int offset = page * _pageSize;
            int i = 0;

            string baseIcon = group.Icon ?? "🗃️";
            string extra = pageCount > 1 ? $" (Page **{page:##,0}**/{pageCount:##,0})" : "";
            info.AppendLine($"> {baseIcon} **Catalog: {group.Name}**{extra}");
            info.AppendLine("> Use `inspect <item_id>` to learn more about an item entry.");

            foreach (Item item in entries.Skip(offset))
            {
                if (i >= _pageSize)
                    break;

                CatalogStatus status = ItemHelper.GetCatalogStatus(user, item);

                if (status == CatalogStatus.Unknown)
                    continue;

                string icon = item.GetIcon() ?? "•";

                info.Append($"\n> {GetStatusIcon(status)} `{item.Id}` {icon} **{item.GetName()}**");
                i++;
            }

            return info.ToString();
        }

        public static string ViewRecipes(ArcadeUser user)
        {
            var recipes = new StringBuilder();
            recipes.AppendLine(Locale.GetHeader(Headers.Recipe));

            IEnumerable<Recipe> known = ItemHelper.GetKnownRecipes(user).ToList();

            if (!Check.NotNullOrEmpty(known))
            {
                recipes.AppendLine("> You don't know any recipes.");
                return recipes.ToString();
            }

            foreach (Recipe recipe in known)
                recipes.AppendLine(WriteRecipeText(user, recipe));

            return recipes.ToString();
        }

        private static string WriteRecipeText(ArcadeUser user, Recipe recipe)
        {
            var text = new StringBuilder();

            string recipeName = ItemHelper.GetItem(recipe.Result.ItemId)?.Name;

            if (!Check.NotNull(recipeName))
                recipeName = "Unknown";

            text.AppendLine($"\n> `{recipe.Id}`")
                .Append($"> {(ItemHelper.CanCraft(user, recipe) ? "📑" : "📄")} **Recipe: {recipeName}**");

            return text.ToString();
        }

        public static string ViewRecipeInfo(ArcadeUser user, Recipe recipe)
        {
            var info = new StringBuilder();

            string resultName = ItemHelper.NameOf(recipe.Result.ItemId);
            bool craft = ItemHelper.CanCraft(user, recipe);

            info.AppendLine($"> {(craft ? "📑" : "📄")} **Recipe: {resultName}**");

            if (craft)
                info.AppendLine("> You can craft this recipe.");

            info.AppendLine($"\n> **Components**");

            foreach ((string itemId, int amount) in recipe.Components)
                info.AppendLine(WriteRecipeComponent(itemId, amount));

            info.AppendLine($"\n> **Result**");
            info.AppendLine(WriteRecipeComponent(recipe.Result.ItemId, recipe.Result.Amount));

            return info.ToString();
        }

        public static string WriteRecipeComponent(string itemId, int amount)
        {
            string icon = ItemHelper.IconOf(itemId);

            if (!Check.NotNull(icon))
                icon = "•";

            string name = ItemHelper.NameOf(itemId);

            string counter = "";

            if (amount > 1)
            {
                counter = $" (x**{amount:##,0}**)";
            }

            return $"`{itemId}` {icon} **{name}**{counter}";
        }

        public static string ViewItem(Item item, CatalogStatus status = CatalogStatus.Known)
        {
            if (status == CatalogStatus.Unknown)
                return Format.Warning("You are not authorized to view this item.");

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
                details.AppendJoin(", ", item.Tag.GetActiveFlags().Select(x => $"`{x.ToString().ToLower()}`"));
                details.AppendLine();
            }

            string summary = item.GetSummary();

            if (Check.NotNull(summary))
                details.AppendLine($"\n```{summary}```");
            else
                details.AppendLine();

            details.AppendLine("> **Details**");
            details.AppendLine($"**ID:** `{item.Id}`");

            if (Check.NotNull(item.GroupId))
                details.AppendLine($"**Group:** `{ItemHelper.GetGroup(item.GroupId).Name}`");

            details.AppendLine("\n> **Marketing**");
            details.AppendLine($"**Rarity:** {item.Rarity.ToString()}");

            if (item.Value > 0)
                details.AppendLine($"**Value:** {Icons.IconOf(item.Currency)} **{item.Value:##,0}**");

            if (item.Size > 0)
                details.AppendLine($"**Storage Size:** {Inventory.WriteCapacity(item.Size)}");

            if (!ItemHelper.HasAttributes(item))
                return details.ToString();

            details.AppendLine("\n> **Attributes**");

            if (ItemHelper.IsUnique(item))
                details.AppendLine("🔸 **Unique**");

            if (item.CanBuy)
                details.AppendLine("🛍️ **Buyable**");

            if (item.CanSell)
                details.AppendLine("📦 **Sellable**");

            string bypass = "";

            if (item.BypassCriteriaOnGift)
                bypass = " - Bypasses criteria when gifted";

            if (item.GiftLimit.HasValue)
            {
                if (item.GiftLimit.Value > 0)
                    details.Append("🎁 **Giftable**");

                if (item.GiftLimit.Value == 1)
                    details.AppendLine($" (**Once**){bypass}");
                else if (item.GiftLimit.Value > 1)
                    details.AppendLine($" (x**{item.GiftLimit.Value:##,0}**){bypass}");
            }
            else
                details.AppendLine($"🎁 **Giftable**{bypass}");

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
                details.AppendLine("📪 **Tradable**");

            if (ItemHelper.IsIngredient(item))
                details.AppendLine("🧂 **Crafting Ingredient**");

            if (ItemHelper.CanCraft(item))
                details.AppendLine("📒 **Craftable**");

            if (item.OwnLimit.HasValue)
                details.AppendLine($"📂 **Max Allowed: {item.OwnLimit.Value:##,0}**");

            if (!ItemHelper.HasUsageAttributes(item))
                return details.ToString();

            if (item.Usage != null)
            {
                if (item.Usage.Action != null)
                    details.AppendLine("🔹 **Usable**");


                string removeOn = "";

                if (item.Usage.DeleteOnBreak)
                    removeOn = " - Removed on break";

                if (item.Usage.Durability.HasValue)
                {
                    details.AppendLine($"❤️ **Durability: {item.Usage.Durability.Value:##,0}**{removeOn}");
                }

                if (item.Usage.Expiry.HasValue)
                {
                    string expiryHandle = item.Usage.ExpiryTrigger switch
                    {
                        ExpiryTrigger.Use => " - Starts when first used",
                        ExpiryTrigger.Equip => " - Starts when equipped",
                        ExpiryTrigger.Own => " - Starts when owned",
                        ExpiryTrigger.Gift => " - Starts when gifted",
                        ExpiryTrigger.Trade => " - Starts when traded",
                        _ => ""
                    };

                    details.AppendLine($"💀 **Expiry:** {Format.LongCounter(item.Usage.Expiry.Value)}{expiryHandle}");
                }

                if (item.Usage.Cooldown.HasValue)
                {
                    string cooldownHandle = item.Usage.CooldownCategory switch
                    {
                        CooldownCategory.Global => " - For all items",
                        CooldownCategory.Group => " - For this item group only",
                        CooldownCategory.Item => " - For this item only",
                        CooldownCategory.Instance => " - For this instance only",
                        _ => ""
                    };

                    details.AppendLine($"🕘 **Cooldown:** {Format.LongCounter(item.Usage.Cooldown.Value)}{cooldownHandle}");
                }
            }

            return details.ToString();
        }
    }
}
