
using Discord.WebSocket;
using Orikivo;
using System;
using System.Text;
using System.Threading.Tasks;
using Arcadia.Formatters;
using Arcadia.Services;
using Arcadia.Graphics;
using Orikivo.Text;
using Discord;
using Format = Orikivo.Format;
using Discord.Interactions;
using SummaryAttribute = Discord.Interactions.SummaryAttribute;
using Discord.Commands;
using Arcadia.Interactions;

namespace Arcadia.Modules
{
    // TODO: Transfer everything to CommonService
    [Icon("🧮")]
    [Name("Common")]
    // [Summary("Generic commands that are commonly used.")]
    public class Common : ArcadeInteractionModule
    {
        private readonly LocaleProvider _locale;

        public Common(LocaleProvider locale)
        {
            _locale = locale;
        }

        #region Slash Commands
        [RequireUser]
        [SlashCommand("equipment", "View all of your equipped items.")]
        public async Task ViewEquipmentAsync()
        {
            await Context.Interaction.RespondAsync(EquipHelper.View(Context.Account)).ConfigureAwait(continueOnCapturedContext: false);
        }

        [RequireUser]
        [SlashCommand("researchinfo", "View details about the research progress of an Item.")]
        public async Task ReadResearchAsync([Summary("item", "The ID of the Item to view")]Item item)
        {
            string result = ResearchHelper.ViewResearch(Context.Account, item);
            await Context.Interaction.RespondAsync(Context.Account, result);
        }

        [RequireUser]
        [SlashCommand("research", "View the current progress of your research.")]
        public async Task ViewResearchAsync()
        {
            string result = ResearchHelper.ViewProgress(Context.Account);
            await Context.Interaction.RespondAsync(Context.Account, result);
        }

        [RequireUser]
        [SlashCommand("researchstart", "Begin research on the specified Item."), Priority(1)]
        public async Task ResearchAsync([Summary("item", "The ID of the Item to view")]Item item)
        {
            string result = ResearchHelper.ResearchItem(Context.Account, item);
            await Context.Interaction.RespondAsync(Context.Account, result);
        }

        [SlashCommand("guide", "Read and view the contents of the specified guide.")]
        public async Task ReadGuideAsync([Summary("guide_id", "The ID of the Guide to read")]string id, [Summary("page", "The page to view of the specified Guide")]int page = 1)
        {
            await Context.Interaction.RespondAsync(GuideViewer.ViewGuide(id, --page)).ConfigureAwait(continueOnCapturedContext: false);
        }

        [SlashCommand("guides", "Learn how to use **Orikivo Arcade** with this collection of guides.")]
        public async Task ViewGuidesAsync([Summary("page", "The page that shows all guides")]int page = 1)
        {
            await Context.Interaction.RespondAsync(GuideViewer.View(--page)).ConfigureAwait(continueOnCapturedContext: false);
        }

        [RequireUser]
        [SlashCommand("craft", "Attempt to craft an Item using the specified Recipe.")]
        public async Task CraftAsync([Summary("recipe", "The ID of the Recipe to view")]Recipe recipe)
        {
            if (!CraftHelper.CanCraft(Context.Account, recipe))
            {
                var notice = new StringBuilder();

                notice.AppendLine(Format.Warning(_locale.GetValue("craft_fail_missing_header", Context.Account.Config.Language)));
                notice.AppendLine($"\n> **{_locale.GetValue("craft_fail_missing_subtitle", Context.Account.Config.Language)}**");

                foreach ((string itemId, int amount) in CraftHelper.GetMissingFromRecipe(Context.Account, recipe))
                {
                    notice.AppendLine(RecipeViewer.PreviewRecipeComponent(itemId, amount));
                }

                await Context.Interaction.RespondAsync(notice.ToString(), ephemeral: true).ConfigureAwait(continueOnCapturedContext: false);
                return;
            }

            if (CraftHelper.Craft(Context.Account, recipe))
            {
                var result = new StringBuilder();

                result.AppendLine($"> 📑 **{ItemHelper.NameOf(recipe.Result.ItemId)}**{Format.ElementCount(recipe.Result.Amount, false, true)}");
                result.AppendLine($"> {_locale.GetValue("craft_success_header", Context.Account.Config.Language)}");

                result.AppendLine($"\n> **{_locale.GetValue("craft_success_losses", Context.Account.Config.Language)}**");

                foreach ((string itemId, int amount) in recipe.Components)
                {
                    string icon = ItemHelper.GetIconOrDefault(itemId, "•");

                    result.AppendLine($"{icon} **{ItemHelper.NameOf(itemId)}**{Format.ElementCount(amount, false)}");
                }

                await Context.Interaction.RespondAsync(result.ToString()).ConfigureAwait(continueOnCapturedContext: false);
                return;
            }

            await Context.Interaction.RespondAsync(Format.Warning(_locale.GetValue("warning_craft_unknown_error", Context.Account.Config.Language)), ephemeral: true).ConfigureAwait(continueOnCapturedContext: false);

        }

        [RequireUser]
        [SlashCommand("quests", "View all of your currently assigned quests.")]
        public async Task ViewQuestsAsync()
        {
            await Context.Interaction.RespondAsync(QuestHelper.View(Context.Account)).ConfigureAwait(continueOnCapturedContext: false);
        }

        [RequireUser]
        [SlashCommand("questat", "View the currently assigned objective on the specified slot.")]
        public async Task ViewQuestAsync([Summary("slot", "The slot position of the specified Quest")]int slot)
        {
            await Context.Interaction.RespondAsync(QuestHelper.InspectQuest(Context.Account, --slot)).ConfigureAwait(continueOnCapturedContext: false);
        }

        [RequireUser]
        [SlashCommand("questassign", "Assign a new set of quests.")]
        public async Task AssignQuestsAsync()
        {
            await Context.Interaction.RespondAsync(QuestHelper.AssignAndDisplay(Context.Account)).ConfigureAwait(continueOnCapturedContext: false);
        }

        [RequireUser]
        [SlashCommand("questskip", "Skip the specified Quest you are currently working on.")]
        public async Task TossQuestAsync([Summary("slot", "The slot position of the active Quest")]int slot)
        {
            await Context.Interaction.RespondAsync(QuestHelper.SkipAndDisplay(Context.Account, --slot)).ConfigureAwait(continueOnCapturedContext: false);
        }

        [RequireUser]
        [SlashCommand("complete", "Claim the rewards from all of your completed quests.")]
        public async Task CompleteQuestsAsync()
        {
            await Context.Interaction.RespondAsync(QuestHelper.CompleteAndDisplay(Context.Account)).ConfigureAwait(continueOnCapturedContext: false);
        }

        [RequireUser]
        [SlashCommand("cooldowns", "View all currently active cooldowns and expirations.")]
        public async Task ViewCooldownsAsync()
        {
            await Context.Interaction.RespondAsync(CooldownHelper.ViewAllTimers(Context.Account)).ConfigureAwait(continueOnCapturedContext: false);
        }

        [RequireUser]
        [SlashCommand("claim", "Attempt to claim the specified Badge.")]
        public async Task ClaimAsync([Summary("badge", "The specific Badge to claim, if any")]Badge badge = null)
        {
            await Context.Interaction.RespondAsync(MeritHelper.ClaimAndDisplay(Context.Account, badge)).ConfigureAwait(continueOnCapturedContext: false);
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [SlashCommand("level", "View a user's current experience.")]
        public async Task ViewLevelAsync([Summary("user", "The user to view")]ArcadeUser user = null)
        {
            user ??= Context.Account;

            await Context.Interaction.RespondAsync(LevelViewer.View(user, !user.Equals(Context.Account))).ConfigureAwait(continueOnCapturedContext: false);
        }

        [RequireUser]
        [SlashCommand("cashout", "Cash out Favors for Cash.")]
        public async Task CashOutAsync([Summary("amount", "The amount of Favors to convert into Cash")]long amount = 0)
        {
            if (amount < 0)
            {
                await Context.Interaction.RespondAsync($"> 👁️ {_locale.GetValue("warning_negative_wager", Context.Account.Config.Language)}\n> *\"{_locale.GetValue("warning_negative_wager_subtitle", Context.Account.Config.Language)}\"*", ephemeral: true).ConfigureAwait(continueOnCapturedContext: false);
                return;
            }

            if (amount == 0)
            {
                // $"> ⚠️ You need to specify a positive amount of **Orite** to convert.
                await Context.Interaction.RespondAsync(CommandDetailsViewer.WriteCashOut(), ephemeral: true);
                return;
            }

            if (amount > Context.Account.TokenBalance)
            {
                await Context.Interaction.RespondAsync(Format.Warning(_locale.GetValue("warning_missing_convert", Context.Account.Config.Language, Format.Bold("Tokens"))), ephemeral: true).ConfigureAwait(continueOnCapturedContext: false);
                return;
            }

            long money = MoneyConvert.TokensToMoney(amount);

            Context.Account.Take(amount, CurrencyType.Tokens);
            Context.Account.Give(money, CurrencyType.Money);
            await Context.Interaction.RespondAsync($"> {_locale.GetValue("currency_convert_success", Context.Account.Config.Language, CurrencyHelper.WriteCost(amount, CurrencyType.Tokens), CurrencyHelper.WriteCost(money, CurrencyType.Money))}").ConfigureAwait(continueOnCapturedContext: false);
        }

        [RequireUser]
        [SlashCommand("daily", "Check in for the day to receive rewards.")]
        public async Task GetDailyAsync()
        {
            DailyResultFlag result = DailyService.Next(Context.Account);
            await Context.Interaction.RespondAsync(DailyService.ApplyAndDisplay(Context.Account, result));
        }

        public Message UseItem(ArcadeUser invoker, ItemData data, string input = null)
        {
            if (data == null)
                return new Message(Format.Warning(_locale.GetValue("warning_invalid_data_reference", invoker.Config.Language)));

            UsageResult result = ItemHelper.UseItem(Context.Account, data, input);

            return result.Message ?? new Message(Format.Warning($"You have used **{ItemHelper.NameOf(data.Id)}**"));
        }

        [RequireUser]
        [SlashCommand("use", "Uses the specified Item by its internal or unique ID.")]
        public async Task UseItemAsync([Summary("data_id", "The item data instance to inspect.")]string dataId, [Summary("input", "Item usage arguments")]string input = null)
        {
            await Context.Interaction.RespondAsync(UseItem(Context.Account, ItemHelper.GetItemData(Context.Account, dataId), input)).ConfigureAwait(continueOnCapturedContext: false);
        }

        public Message ViewSlot(ArcadeUser invoker, int slot)
        {
            if (invoker.Items.Count == 0)
            {
                return new Message(Format.Warning(_locale.GetValue("warning_inventory_empty", Context.Account.Config.Language)));
            }

            if (--slot >= invoker.Items.Count)
            {
                return new Message(Format.Warning("This index is out of bounds."));
            }

            return new Message(CatalogViewer.InspectItem(Context, invoker, invoker.Items[slot], slot));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [SlashCommand("inspectat", "Inspect a specific Item slot in your inventory.")]
        public async Task ViewInventorySlotAsync([Summary("slot_index")]int slot)
        {
            await Context.Interaction.RespondAsync(ViewSlot(Context.Account, slot)).ConfigureAwait(continueOnCapturedContext: false);
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [SlashCommand("inspect", "Inspect a specific Item in your inventory using a data instance.")]
        public async Task InspectInInventoryAsync([Summary("data_id", "The item data instance to inspect.")]string dataId)
        {
            ItemData data = ItemHelper.GetItemData(Context.Account, dataId);
            int slot = Context.Account.Items.IndexOf(data);

            if (data == null)
            {
                if (ItemHelper.Exists(dataId))
                {
                    await Context.Interaction.RespondAsync(Format.Warning(_locale.GetValue("warning_item_not_owned", Context.Account.Config.Language)), ephemeral: true).ConfigureAwait(continueOnCapturedContext: false);
                    return;
                }

                await Context.Interaction.RespondAsync(Format.Warning(_locale.GetValue("warning_invalid_data_reference", Context.Account.Config.Language)), ephemeral: true).ConfigureAwait(continueOnCapturedContext: false);
                return;
            }

            await Context.Interaction.RespondAsync(CatalogViewer.InspectItem(Context, Context.Account, data, slot)).ConfigureAwait(continueOnCapturedContext: false);
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [SlashCommand("inventory", "View a collection of items that you or another user own.")]
        public async Task GetBackpackAsync(ArcadeUser user = null)
        {
            user ??= Context.Account;
            await Context.Interaction.RespondAsync(InventoryViewer.View(user, user.Id == Context.Account.Id)).ConfigureAwait(continueOnCapturedContext: false);
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [SlashCommand("wallet", "View the contents of your wallet.")]
        public async Task GetWalletAsync(ArcadeUser user = null)
        {
            user ??= Context.Account;
            var provider = new CurrencyFormatter();
            var result = new StringBuilder();

            if (user.Id != Context.Account.Id)
                result.AppendLine($"> **Wallet: {user.Username}**");

            if (user.Debt > 0)
                result.AppendLine(string.Format(provider, "{0:D}", user.Debt));
            else if (user.Balance > 0)
                result.AppendLine(string.Format(provider, "{0:O}", user.Balance));

            result.AppendLine(string.Format(provider, "{0:C}", user.ChipBalance));
            result.AppendLine(string.Format(provider, "{0:T}", user.TokenBalance));

            await Context.Interaction.RespondAsync(result.ToString()).ConfigureAwait(continueOnCapturedContext: false);
        }

        [RequireUser]
        [SlashCommand("clearpalette", "Remove your currently equipped palette, if any.")]
        public async Task RemovePaletteAsync()
        {
            string name = ItemHelper.NameFor(Context.Account.Card.Palette.Primary, Context.Account.Card.Palette.Secondary);

            if (!ItemHelper.RemovePalette(Context.Account))
            {
                await Context.Interaction.RespondAsync(Format.Warning(_locale.GetValue("warning_palette_not_equipped", Context.Account.Config.Language)), ephemeral: true).ConfigureAwait(continueOnCapturedContext: false);
                return;
            }

            await Context.Interaction.RespondAsync($"> {_locale.GetValue("card_remove_success", Context.Account.Config.Language, Format.Bold(name), Format.Bold("Card Palette"))}").ConfigureAwait(continueOnCapturedContext: false);
        }

        [RequireUser]
        [SlashCommand("card", "View a Card.")]
        public async Task GetCardAsync(IUser user = null)
        {
            // bool canRender = Environment.OSVersion.Platform == PlatformID.Win32NT;

            /*
            if (!canRender)
            {
                await Context.Channel.SendMessageAsync($"> {Icons.Warning} Due to current rendering issues, cards are disabled on **Unix** operating systems.");
                return;
            }*/

            user ??= Context.User;
            Context.TryGetUser(user.Id, out ArcadeUser account);

            if (account == null)
            {
                await Context.Interaction.RespondAsync($"> {Icons.Warning} **Odd.**\n> {_locale.GetValue("warning_account_not_found", Context.Account.Config.Language)}", ephemeral: true).ConfigureAwait(continueOnCapturedContext: false);
                return;
            }

            try
            {
                CardLayout layout = CardLayout.Default;

                using var graphics = new GraphicsService();
                var details = new CardDetails(account, user, layout.AvatarScale);
                var properties = CardProperties.Default;
                properties.Font = account.Card.Font;
                properties.Palette = account.Card.Palette.Build();
                properties.Trim = false;
                properties.Casing = Casing.Upper;

                CardInfo info = CardBuilder.BuildCardInfo(layout, details, properties);
                System.Drawing.Bitmap card = graphics.DrawCard(info, properties.Deny);

                await Context.Interaction.RespondWithImageAsync(card, $@"bin/Release/tmp/{user.Id}_card.png");
            }
            catch (Exception ex)
            {
                await Context.Interaction.RespondWithErrorAsync(ex, Context.Account.Config.ErrorHandling);
            }
        }
        #endregion

        #region User Commands
        [UserCommand("View Wallet")]
        public async Task GetWalletAsync(IUser user)
        {
            bool canFindAccount = Context.TryGetUser(user.Id, out ArcadeUser account);

            if (!canFindAccount)
            {
                await Context.Interaction.RespondAsync("> **Oops!\n> This user does not have an account.", ephemeral: true);
                return;
            }

            var result = new StringBuilder();
            var provider = new CurrencyFormatter();


            if (Context.User.Id != account.Id)
                result.AppendLine($"> **Wallet: {user.Username}**");

            if (account.Debt > 0)
                result.AppendLine(string.Format(provider, "{0:D}", account.Debt));
            else if (account.Balance > 0)
                result.AppendLine(string.Format(provider, "{0:O}", account.Balance));

            result.AppendLine(string.Format(provider, "{0:C}", account.ChipBalance));
            result.AppendLine(string.Format(provider, "{0:T}", account.TokenBalance));

            await Context.Interaction.RespondAsync(result.ToString());
        }
        #endregion
    }
}
