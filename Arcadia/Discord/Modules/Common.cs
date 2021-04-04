using Discord.Commands;
using Discord.WebSocket;
using Orikivo;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Arcadia.Formatters;
using Arcadia.Services;
using Arcadia.Graphics;
using DiscordBoats;
using Microsoft.Extensions.Configuration;
using Orikivo.Text;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Arcadia.Modules
{
    // TODO: Transfer everything to CommonService
    [Icon("🧮")]
    [Name("Common")]
    [Summary("Generic commands that are commonly used.")]
    public class Common : ArcadeModule
    {
        private readonly DiscordSocketClient _client;
        private readonly IConfigurationRoot _config;
        private readonly LocaleProvider _locale;

        public Common(DiscordSocketClient client, IConfigurationRoot config, LocaleProvider locale)
        {
            _client = client;
            _config = config;
            _locale = locale;
        }

        [RequireUser]
        [Command("equipment")]
        [Summary("View all of your equipped items.")]
        public async Task ViewEquipmentAsync()
        {
            await Context.Channel.SendMessageAsync(EquipHelper.View(Context.Account));
        }

        [RequireUser]
        //[Command("challenges")]
        [Summary("View your current challenge set.")]
        public async Task ViewChallengesAsync()
        {
            string result = ChallengeHelper.View(Context.Account);
            await Context.Channel.SendMessageAsync(result);
        }

        [RequireUser]
        //[Command("submit")]
        [Summary("Submit your challenge set for completion.")]
        public async Task SubmitChallengesAsync()
        {
            string result = ChallengeHelper.Submit(Context.Account);
            await Context.Channel.SendMessageAsync(result);
        }

        [RequireUser]
        [Command("researchinfo"), Alias("rinfo")]
        [Summary("View details about the research progress of an **Item**.")]
        public async Task ReadResearchAsync(Item item)
        {
            string result = ResearchHelper.ViewResearch(Context.Account, item);
            await Context.Channel.SendMessageAsync(Context.Account, result);
        }

        [RequireUser]
        [Command("research"), Priority(0)]
        [Summary("View the current progress of your research.")]
        public async Task ViewResearchAsync()
        {
            string result = ResearchHelper.ViewProgress(Context.Account);
            await Context.Channel.SendMessageAsync(Context.Account, result);
        }

        [RequireUser]
        [Command("research"), Priority(1)]
        [Summary("Begin research on the specified **Item**.")]
        public async Task ResearchAsync(Item item)
        {
            string result = ResearchHelper.ResearchItem(Context.Account, item);
            await Context.Channel.SendMessageAsync(Context.Account, result);
        }

        [Command("guide")]
        [Summary("Read and view the contents of the specified guide.")]
        public async Task ReadGuideAsync([Name("guide_id")]string id, int page = 1)
        {
            await Context.Channel.SendMessageAsync(GuideViewer.ViewGuide(id, --page));
        }

        [Command("guides")]
        [Summary("Learn how to use **Orikivo Arcade** with this collection of guides.")]
        public async Task ViewGuidesAsync(int page = 1)
        {
            await Context.Channel.SendMessageAsync(GuideViewer.View(--page));
        }

        [RequireUser]
        [Command("craft")]
        [Summary("Attempt to craft an **Item** using the specified **Recipe**.")]
        public async Task CraftAsync([Name("recipe_id")]Recipe recipe)
        {
            if (!CraftHelper.CanCraft(Context.Account, recipe))
            {
                var notice = new StringBuilder();

                notice.AppendLine(Format.Warning(_locale.GetValue("craft_fail_missing_header", Context.Account.Config.Language)));
                notice.AppendLine($"\n> **{_locale.GetValue("craft_fail_missing_subtitle", Context.Account.Config.Language)}**");

                foreach ((string itemId, int amount) in CraftHelper.GetMissingFromRecipe(Context.Account, recipe))
                {
                    notice.AppendLine(SRecipeViewer.PreviewRecipeComponent(itemId, amount));
                }

                await Context.Channel.SendMessageAsync(notice.ToString());
                return;
            }

            if (CraftHelper.Craft(Context.Account, recipe))
            {
                var result = new StringBuilder();

                result.AppendLine($"> 📑 **{ItemHelper.NameOf(recipe.Result.ItemId)}**{(recipe.Result.Amount > 1 ? $" (x**{recipe.Result.Amount:##,0}**)" : "")}");
                result.AppendLine($"> {_locale.GetValue("craft_success_header", Context.Account.Config.Language)}");

                result.AppendLine($"\n> **{_locale.GetValue("craft_success_losses", Context.Account.Config.Language)}**");

                foreach ((string itemId, int amount) in recipe.Components)
                {
                    string icon = ItemHelper.GetIconOrDefault(itemId);

                    if (!Check.NotNull(icon))
                        icon = "•";

                    result.AppendLine($"{icon} **{ItemHelper.NameOf(itemId)}**{Format.ElementCount(amount, false)}");
                }

                await Context.Channel.SendMessageAsync(result.ToString());
                return;
            }

            await Context.Channel.SendMessageAsync(Format.Warning(_locale.GetValue("warning_craft_unknown_error", Context.Account.Config.Language)));

        }

        [RequireUser]
        [Command("quests"), Alias("missions", "tasks"), Priority(0)]
        [Summary("View all of your currently assigned quests.")]
        public async Task ViewQuestsAsync()
        {
            await Context.Channel.SendMessageAsync(QuestHelper.View(Context.Account));
        }

        [RequireUser]
        [Command("quests"), Alias("missions", "tasks"), Priority(1)]
        [Summary("View the currently assigned objective on the specified slot.")]
        public async Task ViewQuestAsync(int slot)
        {
            await Context.Channel.SendMessageAsync(QuestHelper.InspectQuest(Context.Account, --slot));
        }

        [RequireUser]
        [Command("assign")]
        [Summary("Assign a new set of quests.")]
        public async Task AssignQuestsAsync()
        {
            await Context.Channel.SendMessageAsync(QuestHelper.AssignAndDisplay(Context.Account));
        }

        [RequireUser]
        [Command("skip")]
        [Summary("Skip the specified **Quest** you are currently working on.")]
        public async Task TossQuestAsync(int slot)
        {
            await Context.Channel.SendMessageAsync(QuestHelper.SkipAndDisplay(Context.Account, --slot));
        }

        [RequireUser]
        [Command("complete")]
        [Summary("Claim the rewards from all of your completed quests.")]
        public async Task CompleteQuestsAsync()
        {
            await Context.Channel.SendMessageAsync(QuestHelper.CompleteAndDisplay(Context.Account));
        }

        [RequireUser]
        [Command("cooldowns"), Alias("cooldown", "expiry")]
        [Summary("View all currently active cooldowns and expirations.")]
        public async Task ViewCooldownsAsync()
        {
            await Context.Channel.SendMessageAsync(CooldownHelper.ViewAllTimers(Context.Account));
        }

        [RequireUser]
        [Command("claim")]
        [Summary("Attempt to claim the specified **Merit**.")]
        public async Task ClaimAsync(string meritId = null)
        {
            await Context.Channel.SendMessageAsync(MeritHelper.ClaimAndDisplay(Context.Account, meritId));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("level"), Alias("exp", "ascent", "lv", "xp")]
        [Summary("View a user's current experience.")]
        public async Task ViewLevelAsync(ArcadeUser user = null)
        {
            user ??= Context.Account;

            await Context.Channel.SendMessageAsync(LevelViewer.View(user, !user.Equals(Context.Account)));
        }

        /*
        [Group("quests"), Alias("missions", "tasks")]
        public class Quests : BaseModule<ArcadeContext>
        {
            [RequireUser]
            [Command("assign")]
            [Summary("Assign a new set of quests.")]
            public async Task AssignQuestsAsync()
            {
                await Context.Channel.SendMessageAsync(QuestHelper.AssignAndDisplay(Context.Account));
            }

            [RequireUser]
            [Command("toss")]
            [Summary("Toss the specified quest you are currently working on.")]
            public async Task TossQuestAsync(int slot)
            {
                await Context.Channel.SendMessageAsync(QuestHelper.TossSlot(Context.Account, --slot));
            }

            [RequireUser]
            [Command("complete")]
            [Summary("Claim the rewards from all of your completed quests.")]
            public async Task CompleteQuestsAsync()
            {
                await Context.Channel.SendMessageAsync(QuestHelper.CompleteAndDisplay(Context.Account));
            }
        }
        */

        [RequireUser]
        [Command("vote")]
        [Summary("Support **Orikivo Arcade** and receive **Tokens**.")]
        public async Task VoteAsync()
        {
            if (string.IsNullOrWhiteSpace(_config["token_discord_boats"]))
                throw new Exception("Expected valid API token for Discord Boats");

            using var boatClient = new BoatClient(_client.CurrentUser.Id, _config["token_discord_boats"]);
            VoteResultFlag result = VoteService.Next(boatClient, Context.Account);
            await Context.Channel.SendMessageAsync(VoteService.ApplyAndDisplay(Context.Account, result));
        }

        [RequireUser]
        [Command("cashout")]
        [Summary("Cash out **Tokens** for **Orite**.")]
        public async Task CashOutAsync(long amount = 0)
        {
            if (amount < 0)
            {
                await Context.Channel.SendMessageAsync($"> 👁️ {_locale.GetValue("warning_negative_wager", Context.Account.Config.Language)}\n> *\"{_locale.GetValue("warning_negative_wager_subtitle", Context.Account.Config.Language)}\"*");
                return;
            }

            if (amount == 0)
            {
                // $"> ⚠️ You need to specify a positive amount of **Orite** to convert.
                await Context.Channel.SendMessageAsync(CommandDetailsViewer.WriteCashOut());
                return;
            }

            if (amount > Context.Account.TokenBalance)
            {
                await Context.Channel.SendMessageAsync(Format.Warning(_locale.GetValue("warning_missing_convert", Context.Account.Config.Language, Format.Bold("Tokens"))));
                return;
            }

            long money = MoneyConvert.TokensToMoney(amount);

            Context.Account.Take(amount, CurrencyType.Tokens);
            Context.Account.Give(money, CurrencyType.Money);
            await Context.Channel.SendMessageAsync($"> {_locale.GetValue("currency_convert_success", Context.Account.Config.Language, CurrencyHelper.WriteCost(amount, CurrencyType.Tokens), CurrencyHelper.WriteCost(money, CurrencyType.Money))}");
        }

        [RequireUser]
        [Command("daily")]
        [Summary("Check in for the day to receive rewards.")]
        public async Task GetDailyAsync()
        {
            DailyResultFlag result = DailyService.Next(Context.Account);
            await Context.Channel.SendMessageAsync(DailyService.ApplyAndDisplay(Context.Account, result));
        }

        [RequireUser]
        [Command("use")]
        [Summary("Uses the specified **Item** by its internal or unique ID.")]
        public async Task UseItemAsync([Name("data_id")][Summary("The item data instance to inspect.")]string dataId, [Remainder]string input = null)
        {
            ItemData data = ItemHelper.GetItemData(Context.Account, dataId);

            if (data == null)
            {
                await Context.Channel.SendMessageAsync(Format.Warning(_locale.GetValue("warning_invalid_data_reference", Context.Account.Config.Language)));
                return;
            }

            UsageResult result = ItemHelper.UseItem(Context.Account, data, input);

            if (result.Message != null)
            {
                await Context.Channel.SendMessageAsync(Context.Account, result.Message);
                return;
            }

            await Context.Channel.SendMessageAsync(Format.Warning($"You have used **{ItemHelper.NameOf(data.Id)}**."));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("inspectat")]
        [Summary("Inspect a specific **Item** slot in your inventory.")]
        public async Task ViewInventorySlotAsync([Name("slot_index")]int slot)
        {
            if (Context.Account.Items.Count == 0)
            {
                await Context.Channel.SendMessageAsync(Format.Warning(_locale.GetValue("warning_inventory_empty", Context.Account.Config.Language)));
                return;
            }

            slot--;

            Math.Clamp(slot, 0, Context.Account.Items.Count - 1);

            ItemData data = Context.Account.Items[slot];

            await Context.Channel.SendMessageAsync(CatalogViewer.InspectItem(Context, Context.Account, data, slot));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("inspect")]
        [Summary("Inspect a specific **Item** in your inventory using a data instance.")]
        public async Task InspectInInventoryAsync([Name("data_id")][Summary("The item data instance to inspect.")]string dataId)
        {
            ItemData data = ItemHelper.GetItemData(Context.Account, dataId);
            int slot = Context.Account.Items.IndexOf(data);

            if (data == null)
            {
                if (ItemHelper.Exists(dataId))
                {
                    await Context.Channel.SendMessageAsync(Format.Warning(_locale.GetValue("warning_item_not_owned", Context.Account.Config.Language)));
                    return;
                }

                await Context.Channel.SendMessageAsync(Format.Warning(_locale.GetValue("warning_invalid_data_reference", Context.Account.Config.Language)));
                return;
            }

            await Context.Channel.SendMessageAsync(CatalogViewer.InspectItem(Context, Context.Account, data, slot));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("inventory"), Alias("backpack", "inv", "bp")]
        [Summary("View a collection of items that you or another user own.")]
        public async Task GetBackpackAsync(ArcadeUser user = null)
        {
            user ??= Context.Account;
            await Context.Channel.SendMessageAsync(InventoryViewer.View(user, user.Id == Context.Account.Id));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("balance"), Alias("money", "bal", "chips", "tokens", "debt")]
        [Summary("Returns a display showing all of the values in a wallet.")]
        public async Task GetMoneyAsync(ArcadeUser user = null)
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

            await Context.Channel.SendMessageAsync(result.ToString());
        }

        [RequireUser]
        [Command("clearpalette")]
        [Summary("Remove your currently equipped palette, if any.")]
        public async Task RemovePaletteAsync()
        {
            string name = ItemHelper.NameFor(Context.Account.Card.Palette.Primary, Context.Account.Card.Palette.Secondary);

            if (!ItemHelper.RemovePalette(Context.Account))
            {
                await Context.Channel.SendMessageAsync(Format.Warning(_locale.GetValue("warning_palette_not_equipped", Context.Account.Config.Language)));
                return;
            }

            await Context.Channel.SendMessageAsync($"> {_locale.GetValue("card_remove_success", Context.Account.Config.Language, Format.Bold(name), Format.Bold("Card Palette"))}");
        }

        [RequireUser]
        [Command("card")]
        [Summary("View a user's or your own current **Card**.")]
        public async Task GetCardAsync(SocketUser user = null)
        {
            bool canRender = Environment.OSVersion.Platform == PlatformID.Win32NT;

            if (!canRender)
            {
                await Context.Channel.SendMessageAsync($"> {Icons.Warning} Due to current rendering issues, cards are disabled on **Unix** operating systems.");
                return;
            }

            user ??= Context.User;
            Context.TryGetUser(user.Id, out ArcadeUser account);

            if (account == null)
            {
                await Context.Channel.SendMessageAsync($"> {Icons.Warning} **Odd.**\n> {_locale.GetValue("warning_account_not_found", Context.Account.Config.Language)}");
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

                await Context.Channel.SendImageAsync(card, $@"bin/Release/tmp/{user.Id}_card.png");
            }
            catch (Exception ex)
            {
                await Context.Channel.CatchAsync(ex, Context.Account.Config.ErrorHandling);
            }
        }
    }
}
