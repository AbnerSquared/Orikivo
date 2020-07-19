using Discord;
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
    public enum TradeState
    {
        Invite, // This is the initial state for trading, at which a user is waiting for the other user to accept
        Timeout, // This is the state at which a trade or invite was timed out.
        Menu, // The menu showing the items that will be traded between two users.
        Inventory, // The inventory display screen, waiting for a user to select their item.
        Item, // The item select screen, waiting for a user to select an item with amount
        Accept, // The accept screen, waiting for both users to accept
        Success, // The trade has gone through
        Cancel // The trade has been cancelled
    }
    // This will use Discord.Addons.Collectors
    public class TradeHandler : MatchAction
    {
        public TradeHandler(ArcadeContext context, ArcadeUser participant) : base()
        {
            Context = context;
            LastState = null;
            State = TradeState.Invite;
            SpeakerId = null;
            Host = Context.Account;
            Participant = participant;
            HostAccepted = ParticipantAccepted = false;
            HostItems = new Dictionary<string, int>();
            ParticipantItems = new Dictionary<string, int>();
        }

        public ArcadeContext Context; // This is the context received from the initial trade.
        public TradeState? LastState; // The last state on the trade
        public TradeState State; // The current state of the trade

        public IUserMessage MessageReference; // The reference of the message initialized, used to update

        public ArcadeUser Host; // This is the user that initialized the trade
        public ArcadeUser Participant; // The is the user that was invited to trade

        public ulong? SpeakerId; // If an inventory was opened, mark the speakerId to that user.

        public bool HostAccepted; // True if the host has accepted.
        public bool ParticipantAccepted; // True if the participant has accepted.

        public Dictionary<string, int> HostItems; // All of the items marked in the host's inventory.
        public Dictionary<string, int> ParticipantItems; // All of the items marked in the participant's inventory.

        public override async Task OnStartAsync()
        {
            // This is where the message is built.

            // TODO: detect if a user can participate in a trade.

            MessageReference = await Context.Channel.SendMessageAsync($"> Invited **{Participant.Id}** to trade!\n> *Waiting for a response...*");
        }

        private void MarkAsAccepted(ulong userId, bool value = true)
        {
            if (userId == Host.Id)
                HostAccepted = value;
            else if (userId == Participant.Id)
                ParticipantAccepted = value;
            
            throw new ArgumentException("The specified ID did not match either the host or participant.");
        }

        private bool IsParticipant(ulong userId)
            => userId == Participant.Id;

        private string GetTradeMenu()
        {
            var menu = new StringBuilder();

            // Write the host's items first
            menu.AppendLine(WriteTradeSlot(Host, HostItems));

            if (HostAccepted)
                menu.AppendLine(WriteReady(Host));

            // A line break in the menu.
            menu.AppendLine();

            // Afterwards, write the participant's items
            menu.AppendLine(WriteTradeSlot(Participant, ParticipantItems));

            if (ParticipantAccepted)
                menu.AppendLine(WriteReady(Participant));

            return menu.ToString();
        }

        private string WriteReady(ArcadeUser user)
            => $"{user.Username} is ready to trade!";

        private string WriteInventory(ArcadeUser user)
            => Inventory.Write(user);

        private string WriteTradeSlot(ArcadeUser user, Dictionary<string, int> items)
        {
            var slot = new StringBuilder();

            slot.AppendLine($"> **{user.Username}** offers:\n");

            int i = 0;
            foreach ((string itemId, int amount) in items)
            {
                if (i == 0)
                    slot.AppendLine("\n");

                slot.Append("> ");
                slot.Append(ItemHelper.NameOf(itemId));

                if (amount > 1)
                {
                    slot.Append($" (x**{amount.ToString("##,0")}**)");
                }
            }

            return slot.ToString();
        }

        private ArcadeUser GetUser(ulong userId)
            => userId == Host.Id ? Host
            : userId == Participant.Id
            ? Participant
            : throw new ArgumentException("The specified ID did not match either the host or participant.");

        // This is only invoked if the ID is either the Host or the Participant.
        public override async Task<ActionResult> InvokeAsync(SocketMessage message)
        {
            // gets the account that executed this.
            var account = GetUser(message.Author.Id);

            string input = message.Content;
            // Up next, we want to determine who is allowed to start.


            // Next, we want to handle invitation accept or deny
            if (State == TradeState.Invite) // TradeState.Pending
            {
                if (input == "accept" && IsParticipant(account.Id))
                {
                    // If the user accepted the trade invitation, go to the base trade menu.
                    await SetStateAsync(TradeState.Menu);
                    return ActionResult.Continue;
                }

                if (input == "deny" && IsParticipant(account.Id))
                {
                    await SetStateAsync(TradeState.Cancel); // Update and handle the message input here.
                    await UpdateMessageAsync($"**Sorry!**\n> **{Participant}** has denied the invitation to trade.");
                    return ActionResult.Success;
                }


            }

            // However, if the input specified is 'cancel', cancel the trade.
            if (input == "cancel")
            {
                await SetStateAsync(TradeState.Cancel);
                await UpdateMessageAsync($"**Oops!**\n**{account.Username}** has cancelled the trade.");
                return ActionResult.Success;
            }


            if (SpeakerId.HasValue)
            {
                // If the current invoker is not the current speaker, ignore their inputs
                

                

                if (SpeakerId.Value != account.Id)
                    return ActionResult.Continue;
            }

            // Here are all of the current commands in a Trade:

            // CANCEL: This cancels the current trade, regardless of who executed it.
            // ACCEPT & IF STATE == Menu: This marks the user who executed it that they accepted their end of the trade.
            //     If both users accept, then the trade goes through. Take all specified items from both, swap, and give both the other's items.
            if (input == "accept")
            {
                if (State != TradeState.Menu) // If they aren't currently in the menu, ignore input.
                {
                    return ActionResult.Continue;
                }

                MarkAsAccepted(account.Id);
                
                if (HostAccepted && ParticipantAccepted)
                {
                    Trade();
                    // Use the SetState method to update messages as well.
                    await SetStateAsync(TradeState.Success);
                    return ActionResult.Success;
                }
            }

            // BACKPACK: Opens the backpack for the user that executed it.
            if (input == "inventory")
            {
                // If a speaker is already in their personal backpack, ignore input.
                if (SpeakerId.HasValue)
                {
                    return ActionResult.Continue;
                }

                // If the backpack is now opened, mark the speaker, and let them inspect their backpack.
                SpeakerId = account.Id;
                
                // This gets the speaker's inventory
                await SetStateAsync(TradeState.Inventory); // Mark the state as in an inventory.
            
            }

            if (State == TradeState.Inventory)
            {
                // If the user wishes to return to the main trade menu
                if (input == "back")
                {
                    SpeakerId = null;
                    await SetStateAsync(TradeState.Menu);
                    return ActionResult.Continue;
                }

                // First, you want to check if the specified item exists in this user's inventory
                if (!account.Items.ContainsKey(input))
                {
                    // Prepend a notice saying that an invalid item was specified.
                    await UpdateMessageAsync($"> An invalid item ID was specified.\n" + MessageReference.Content);
                    return ActionResult.Continue;
                }

                var selectedItem = account.Items[input];

                if (!ItemHelper.CanTrade(input, selectedItem.Data))
                {
                    await UpdateMessageAsync($"> This item cannot be traded.\n" + MessageReference.Content);
                    return ActionResult.Continue;
                }

                AddItem(account.Id, input);

                // <ITEM_ID> & STATE == INVENTORY: This adds the specified item into the trade.
                // If the item cannot be traded, notify them and don't do anything.
            }

            throw new NotImplementedException("Incomplete input method handling.");
        }

        private async Task SetStateAsync(TradeState state)
        {
            LastState = State;
            State = state;

            if (state == TradeState.Inventory)
            {
                if (!SpeakerId.HasValue)
                    throw new Exception("Cannot read the inventory of an empty primary ID");

                await UpdateMessageAsync(WriteInventory(GetUser(SpeakerId.Value)));
            }

            // Default Menu
            if (state == TradeState.Menu)
                await UpdateMessageAsync(GetTradeMenu());
        }

        private void AddItem(ulong speakerId, string itemId, int amount = 1)
        {
            if (speakerId == Host.Id)
            {
                if (HostItems.ContainsKey(itemId))
                {
                    HostItems[itemId] += amount;
                }
                else
                {
                    HostItems[itemId] = amount;
                }
            }
        }

        private void RemoveItem(ulong speakerId, string itemId, int amount = 1)
        {
            if (speakerId == Host.Id)
            {
                if (HostItems.ContainsKey(itemId))
                {
                    if (HostItems[itemId] - amount <= 0)
                        HostItems.Remove(itemId);
                    else
                    {
                        HostItems[itemId] -= amount;
                    }
                }
            }

            else if (speakerId == Participant.Id)
            {
                if (ParticipantItems.ContainsKey(itemId))
                {
                    if (ParticipantItems[itemId] - amount <= 0)
                        ParticipantItems.Remove(itemId);
                    else
                    {
                        ParticipantItems[itemId] -= amount;
                    }
                }
            }
        }

        // Invokes the trade, and lets everything go through
        private void Trade()
        {
            if (HostAccepted && ParticipantAccepted)
            {
                // This needs to handle unique item data.
                foreach ((string itemId, int amount) in HostItems)
                {
                    ItemHelper.TakeItem(Host, itemId, amount);
                    ItemHelper.GiveItem(Participant, itemId, amount);
                }

                foreach ((string itemId, int amount) in ParticipantItems)
                {
                    ItemHelper.TakeItem(Participant, itemId, amount);
                    ItemHelper.GiveItem(Host, itemId, amount);
                }
            }
        }

        public override async Task OnCancelAsync()
        {
            if (LastState == TradeState.Invite)
            {
                await UpdateMessageAsync($"**Sorry!**\n> **{Participant}** has denied the invitation to trade.");
                return;
            }
            // If the trade was cancelled, simply notify it here.
            // In this case, this is executed the moment someone cancels

            await UpdateMessageAsync("");
        }

        public override async Task OnTimeoutAsync(SocketMessage message)
        {
            if (State == TradeState.Invite)
            {
                await UpdateMessageAsync($"**Oops!**\n> **{Participant.Username}** did not respond to the invitation in time.");
                return;
            }

            // If the action has timed out, and the current state was Invite
            // Say that the user failed to reply to the trade.

            // Otherwise, say that the trade has timed out.
            await UpdateMessageAsync($"**Oops!**\n> The trade has timed out.");
        }

        public async Task UpdateMessageAsync(string content)
        {
            await MessageReference?.ModifyAsync(content);
        }
    }
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

    public class Inventory
    {
        private static string GetHeader(long capacity)
        {
            return $"> **Inventory**\n> `{GetCapacity(capacity)}` **{GetSuffix(capacity)}** available.";
        }

        private static string GetCapacity(long capacity)
        {
            var suffix = GetSuffix(capacity);

            return suffix switch
            {
                "B" => $"{capacity}",
                "KB" => $"{(double)(capacity / 1000)}",
                "MB" => $"{(double)(capacity / 1000000)}",
                "GB" => $"{(double)(capacity / 1000000000)}",
                "TB" => $"{(double)(capacity / 1000000000000)}",
                _ => "∞"
            };
        }

        private static string GetSuffix(long capacity)
        {
            int len = capacity.ToString().Length;

            if (len < 4)
                return "B";

            if (len < 7)
                return "KB";

            if (len < 10)
                return "MB";

            if (len < 13) 
                return "GB";

            if (len < 16)
                return "TB";

            return "PB";
        }

        private static string WriteItem(int index, string id, ItemData data)
        {
            var item = ItemHelper.GetItem(id);
            var summary = new StringBuilder();

            summary.AppendLine($"**#**{index}");
            summary.Append($"> `{id}` **{item.Name}**");
            
            if (data.Count > 1)
            {
                summary.Append($" (x**{data.Count}**)");
            }

            summary.AppendLine();
            summary.Append($"> `{GetCapacity(item.Size)}` **{GetSuffix(item.Size)}**");


            return summary.ToString();
        }

        public static string Write(ArcadeUser user)
        {
            var inventory = new StringBuilder();

            inventory.AppendLine(GetHeader(user.GetStat(Stats.Capacity)));

            int i = 0;
            foreach ((string id, ItemData data) in user.Items)
            {
                if (i > 0)
                {
                    inventory.AppendLine("\n");
                }

                inventory.AppendLine(WriteItem(i, id, data));
                i++;
            }

            if (i == 0)
                inventory.AppendLine("\n> *\"I could not locate any files.\"*");

            return inventory.ToString();
        }
    }

    public class Catalog
    {
        private static string _line = "> **{0}**: {1}";
        private static string GetId(Item item)
            => string.Format(_line, "ID", $"`{item.Id}`");

        private static string GetName(Item item)
            => string.Format(_line, "Name", $"**`{item.Name}`**");

        private static string GetSummary(Item item)
            => string.Format(_line, "Summary", $"`{item.Summary}`");
        
        private static string GetQuotes(Item item)
            => string.Format(_line, OriFormat.TryPluralize("Quote", item.Quotes.Count), string.Join(", ", item.Quotes.Select(x => $"*`\"{x}\"`*")));

        private static string GetRarity(Item item)
            => string.Format(_line, "Rarity", $"`{item.Rarity.ToString()}`");

        private static string GetTags(Item item)
            => string.Format(_line, OriFormat.TryPluralize("Tag", item.Tag.GetActiveFlags().Count()), string.Join(", ", item.Tag.GetActiveFlags().Select(x => $"`{x.ToString()}`")));

        private static string GetValue(Item item)
            => string.Format(_line, "Value", $"**`{item.Value.ToString("##,0")}`**");

        private static string GetBuyState(Item item)
            => string.Format(_line, "Can Buy?", item.CanBuy ? "`Yes`": "`No`");

        private static string GetSellState(Item item)
            => string.Format(_line, "Can Sell?", item.CanSell ? "`Yes`" : "`No`");

        private static string GetTradeState(Item item)
            => string.Format(_line, "Can Trade?", item.TradeLimit.HasValue ? item.TradeLimit.Value == 0 ? "`No`" : $"`Yes (x{item.TradeLimit.Value.ToString("##,0")})`" : "`Yes`");

        private static string GetGiftState(Item item)
            => string.Format(_line, "Can Gift?", item.GiftLimit.HasValue ? item.GiftLimit.Value == 0 ? "`No`" : $"`Yes (x{item.GiftLimit.Value.ToString("##,0")})`" : "`Yes`");

        private static string GetUseState(Item item)
            => string.Format(_line, "Can Use?", item.OnUse != null ? "`Yes`": "`No`");

        private static string GetBypassState(Item item)
            => string.Format(_line, "Bypass Requirements On Gift?", item.BypassCriteriaOnGift ? "`Yes`" : "`No`");

        private static string GetOwnLimit(Item item)
            => string.Format(_line, "Own Limit", item.OwnLimit.HasValue ? $"`{item.OwnLimit.Value.ToString("##,0")}`" : "`None`");

        // this is only the definer
        public static string WriteItem(Item item)
        {
            var details = new StringBuilder();

            details.AppendLine(GetId(item));
            details.AppendLine(GetName(item));

            if (!string.IsNullOrWhiteSpace(item.Summary))
                details.AppendLine(GetSummary(item));

            if (item.Quotes.Count > 0)
                details.AppendLine(GetQuotes(item));

            details.AppendLine(GetRarity(item));
            details.AppendLine(GetTags(item));

            if (item.Value > 0)
            {
                details.AppendLine(GetValue(item));
                details.AppendLine(GetBuyState(item));
                details.AppendLine(GetSellState(item));
                details.AppendLine(GetTradeState(item));
                details.AppendLine(GetGiftState(item));
                details.AppendLine(GetBypassState(item));
                details.AppendLine(GetUseState(item));
                
            }

            details.AppendLine(GetOwnLimit(item));

            return details.ToString();
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
        //[Command("catalog")]
        [Command("catalog")]
        [Summary("View your current **Item** catalog.")]
        public async Task GetCatalogAsync()
        {

        }

        [Command("trade")]
        [Summary("Attempts to start a trade with the specified user.")]
        public async Task TradeAsync(SocketUser user)
        {
            Context.Data.Users.TryGetValue(user.Id, out ArcadeUser participant);

            if (participant == null)
            {
                await Context.Channel.SendMessageAsync("> **Oops!**\n> I ran into an issue.\n```The specified user does not seem to have an account.```");
                return;
            }

            var handler = new TradeHandler(Context, participant);

            await HandleTradeAsync(handler);

        }

        private async Task HandleTradeAsync(TradeHandler handler)
        {
            try
            {
                var collector = new MessageCollector(Context.Client);
                var options = new MatchOptions
                {
                    ResetTimeoutOnAttempt = true,
                    Timeout = TimeSpan.FromSeconds(20),
                    Action = handler
                };

                Func<SocketMessage, int, bool> filter = delegate (SocketMessage message, int index)
                {
                    return (handler.Host.Id == message.Author.Id || handler.Participant.Id == message.Author.Id)
                        && (message.Channel.Id == Context.Channel.Id);
                };

                await collector.MatchAsync(filter, options);
            }
            catch (Exception e)
            {
                await Context.Channel.CatchAsync(e);
            }
        }

        [Command("gift")]
        [Summary("Attempts to gift an **Item** to the specified user.")]
        public async Task GiftAsync(SocketUser user, string itemId)
        {
            
        }

        [Command("use")]
        [Summary("Uses the specified **Item** by its internal ID.")]
        public async Task UseItemAsync(string id)
        {

        }

        [Command("destroy"), Alias("delete", "del")]
        [Summary("Attempts to destroy the specified **Item** by its internal ID.")]
        public async Task DestroyItemAsync(string id)
        {

        }

        [Command("inspect")]
        [Summary("Provides details about the specified **Item**, if it has been previously discovered.")]
        public async Task InspectAsync(string id)
        {
            var result = ItemHelper.GetItem(id);
            if (result == null)
            {
                await Context.Channel.SendMessageAsync("> **Oops!**\n> I ran into an issue.\n```I couldn't find any items with the specified ID.```");
                return;
            }

            //Console.WriteLine(ItemHelper.GetUniqueId());

            await Context.Channel.SendMessageAsync(Catalog.WriteItem(result));
        }

        // This gets a person's backpack.
        [Command("inventory"), Alias("backpack", "items", "bp")]
        [RequireUser(AccountHandling.ReadOnly)]
        public async Task GetBackpackAsync(int page = 0, SocketUser user = null)
        {
            user ??= Context.User;
            Context.Data.Users.TryGetValue(user.Id, out ArcadeUser account);

            if (account == null)
            {
                await Context.Channel.SendMessageAsync("> **Oops!**\n> I ran into an issue.\n```The specified user does not seem to have an account.```");
                return;
            }

            await Context.Channel.SendMessageAsync(Inventory.Write(account));
        }

        [Command("stats")]
        [RequireUser(AccountHandling.ReadOnly)]
        public async Task GetStatsAsync(SocketUser user = null)
        {
            user ??= Context.User;
            Context.Data.Users.TryGetValue(user.Id, out ArcadeUser account);

            if (account == null)
            {
                await Context.Channel.SendMessageAsync("> **Oops!**\n> I ran into an issue.\n```The specified user does not seem to have an account.```");
                return;
            }

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
        [Summary("Returns a current wallet state.")]
        public async Task GetMoneyAsync(SocketUser user = null)
        {
            user ??= Context.User;
            Context.Data.Users.TryGetValue(user.Id, out ArcadeUser account);

            if (account == null)
            {
                await Context.Channel.SendMessageAsync("> **Oops!**\n> I ran into an issue.\n```The specified user does not seem to have an account.```");
                return;
            }

            var values = new StringBuilder();
            if (user != null)
            {
                if (user != Context.User)
                    values.AppendLine($"**Wallet - {account.Username}**");
            }

            values.AppendLine($"**Balance**: 💸 **{account.Balance.ToString("##,0.###")}**");
            values.AppendLine($"**Tokens**: 🏷️ **{account.TokenBalance.ToString("##,0.###")}**");
            values.AppendLine($"**Debt**: 📃 **{account.Debt.ToString("##,0.###")}**");

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
            Context.Data.Users.TryGetValue(user.Id, out ArcadeUser account);

            if (account == null)
            {
                await Context.Channel.SendMessageAsync("> **Oops!**\n> I ran into an issue.\n```The specified user does not seem to have an account.```");
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