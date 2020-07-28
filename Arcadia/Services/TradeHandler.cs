using Discord;
using Discord.WebSocket;
using Orikivo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Format = Orikivo.Format;

namespace Arcadia
{
    // TODO: Implement StringReader to parse input to be much more precise than direct string comparison
    public class TradeHandler : MatchAction
    {
        public TradeHandler(ArcadeContext context, ArcadeUser participant) : base()
        {
            Context = context;
            LastState = null;
            State = TradeState.Invite;
            CurrentId = null;
            Host = Context.Account;
            Participant = participant;
            HostReady = ParticipantReady = false;
            HostOffer = new Dictionary<string, int>();
            ParticipantOffer = new Dictionary<string, int>();
        }

        // This is the context received from the initial trade.
        public ArcadeContext Context { get; }

        // The last state on the trade
        public TradeState? LastState { get; private set; }

        // The current state of the trade
        public TradeState State { get; private set; } 

        // The reference of the message initialized, used to update
        public IUserMessage MessageReference { get; private set; }

        // The user that initialized the trade
        public ArcadeUser Host { get; }

        // The user that was invited to trade
        public ArcadeUser Participant { get; } 

        // The ID of the person in their inventory
        internal ulong? CurrentId { get; set; }

        // True if the host has accepted
        public bool HostReady { get; private set; }

        // True if the participant has accepted
        public bool ParticipantReady { get; private set; }

        // All of the items marked in the host's inventory
        public Dictionary<string, int> HostOffer { get; }

        // All of the items marked in the participant's inventory
        public Dictionary<string, int> ParticipantOffer { get; }

        public override async Task OnStartAsync()
        {
            MessageReference = await Context.Channel.SendMessageAsync($"> 📨 Invited **{Participant.Username}** to trade!\n> *Waiting for a response...*");
        }

        private string WriteTradeMenu()
        {
            var menu = new StringBuilder();

            // Write the host's items first
            menu.AppendLine(WriteOffer(Host, HostOffer));

            if (HostReady)
                menu.AppendLine(WriteOnReady(Host));

            // A line break in the menu.
            menu.AppendLine();

            // Afterwards, write the participant's items
            menu.AppendLine(WriteOffer(Participant, ParticipantOffer));

            if (ParticipantReady)
                menu.AppendLine(WriteOnReady(Participant));

            return menu.ToString();
        }

        private static string WriteOnReady(ArcadeUser invoker)
            => $"> ☑️ **{invoker.Username}** is ready to trade.";

        private string WriteOnCancel(ArcadeUser invoker)
        {
            if (LastState == TradeState.Invite && invoker.Equals(Participant))
                return $"> 💢 **Sorry.**\n> **{Participant.Username}** has denied the invitation to trade.";

            return $"> 💢 **Oops!**\n> **{invoker.Username}** has cancelled the trade.";
        }

        private string WriteOnTimeout()
        {
            // If the action has timed out during the invite, say that the user failed to reply to the trade.
            if (State == TradeState.Invite)
                return $"> ⌛ **Oops!**\n> **{Participant.Username}** did not respond to the invitation in time.";

            // Otherwise, say that the trade has timed out.
            return "> ⌛ **Oops!**\n> The trade has timed out.";
        }

        private string WriteInventory(ArcadeUser user)
            => Inventory.Write(user, false);

        private string WriteOffer(ArcadeUser user, Dictionary<string, int> items)
        {
            var slot = new StringBuilder();

            if (items.Count == 0)
                slot.Append($"> **{user.Username}** has not offered anything.");
            else
            {
                slot.AppendLine($"> **{user.Username}** offers:\n");

                int i = 0;
                foreach ((string itemId, int amount) in items)
                {
                    if (i >= 0)
                        slot.AppendLine();

                    slot.Append($"> **{ItemHelper.NameOf(itemId)}**");

                    if (amount > 1)
                    {
                        slot.Append($" (x**{amount:##,0}**)");
                    }

                    i++;
                }
            }

            return slot.ToString();
        }

        private ArcadeUser GetAccount(ulong userId)
            => userId == Host.Id ? Host : userId == Participant.Id ? Participant
            : throw new ArgumentException("The specified ID did not match either the host or participant.");

        private void MarkAsReady(ulong userId, bool value = true)
        {
            if (userId == Host.Id)
                HostReady = value;
            else if (userId == Participant.Id)
                ParticipantReady = value;

            throw new ArgumentException("The specified ID did not match either the host or participant.");
        }

        private bool IsCurrentHost()
            => CurrentId == Host.Id;

        private bool IsCurrentParticipant()
            => CurrentId == Participant.Id;

        private bool CanStartTrade()
            => HostReady && ParticipantReady;

        private Dictionary<string, int> GetCurrentItems()
        {
            if (!CurrentId.HasValue)
                return null;

            if (IsCurrentHost())
                return HostOffer;

            if (IsCurrentParticipant())
                return ParticipantOffer;

            return null;
        }

        private void AddItemToCurrent(string itemId, int amount = 1)
        {
            var items = GetCurrentItems();

            if (!items.TryAdd(itemId, amount))
                items[itemId] += amount;

            HostReady = false;
            ParticipantReady = false;
        }

        private void RemoveItemFromCurrent(string itemId, int amount = 1)
        {
            var items = GetCurrentItems();

            if (items.ContainsKey(itemId))
            {
                if (items[itemId] - amount <= 0)
                    items.Remove(itemId);
                else
                    items[itemId] -= amount;

                HostReady = false;
                ParticipantReady = false;
            }
        }

        // Invokes the trade, and lets everything go through
        private void Trade()
        {
            if (HostReady && ParticipantReady)
            {
                // This needs to handle unique item data.
                foreach ((string itemId, int amount) in HostOffer)
                {
                    ItemHelper.TakeItem(Host, itemId, amount);
                    ItemHelper.GiveItem(Participant, itemId, amount);
                }

                foreach ((string itemId, int amount) in ParticipantOffer)
                {
                    ItemHelper.TakeItem(Participant, itemId, amount);
                    ItemHelper.GiveItem(Host, itemId, amount);
                }
            }
        }

        // This is only invoked if the ID is either the Host or the Participant.
        public override async Task<ActionResult> InvokeAsync(SocketMessage message)
        {
            // gets the account that executed this.
            var account = GetAccount(message.Author.Id);

            // Get the input from the message
            string input = message.Content;

            // CANCEL: This cancels the current trade, regardless of who executed it.
            if (input == "cancel")
            {
                await SetStateAsync(TradeState.Cancel);
                await UpdateMessageAsync(WriteOnCancel(account));
                return ActionResult.Success;
            }

            // If the current invoker is not the current speaker, ignore their inputs
            if (CurrentId.HasValue)
            {
                if (CurrentId.Value != account.Id)
                    return ActionResult.Continue;
            }

            switch (State)
            {
                case TradeState.Invite:
                    // ACCEPT
                    if (input == "accept" && account.Id == Participant.Id)
                    {
                        // If the user accepted the trade invitation, go to the base trade menu.
                        await SetStateAsync(TradeState.Menu, $"> ☑️ **{Participant.Username}** has agreed to trade.");
                        return ActionResult.Continue;
                    }

                    // DECLINE
                    if (input == "decline" && account.Id == Participant.Id)
                    {
                        await SetStateAsync(TradeState.Cancel);
                        await UpdateMessageAsync(WriteOnCancel(account));
                        return ActionResult.Success;
                    }
                    break;

                case TradeState.Menu:
                    // ACCEPT: This marks the user who executed it that they accepted their end of the trade.
                    // If both users accept, then the trade goes through. Take all specified items from both, swap, and give both the other's items.
                    if (input == "ready")
                    {
                        if (IsOfferEmpty())
                        {
                            await SetStateAsync(TradeState.Menu, Format.Warning("There aren't any items specified!"));
                            return ActionResult.Continue;
                        }

                        if (CanStartTrade())
                        {
                            Trade();
                            await SetStateAsync(TradeState.Success);
                            return ActionResult.Success;
                        }

                        MarkAsReady(account.Id);
                        await SetStateAsync(TradeState.Menu);
                        return ActionResult.Continue;
                    }

                    if (input == "inventory")
                    {
                        // If someone is already inspecting their backpack, ignore input.
                        if (CurrentId.HasValue)
                            return ActionResult.Continue;

                        CurrentId = account.Id;
                        await SetStateAsync(TradeState.Inventory);
                        
                        return ActionResult.Continue;
                    }
                    break;

                case TradeState.Inventory:
                    // BACK: Used by the current invoker to return to the menu.
                    if (input == "back")
                    {
                        CurrentId = null;
                        await SetStateAsync(TradeState.Menu);
                        return ActionResult.Continue;
                    }

                    // TODO: Handle unique item ID when trading.

                    // Check if the specified item exists in the invoker's inventory
                    if (!ItemHelper.HasItem(account, input))
                    {
                        await SetStateAsync(TradeState.Inventory, Format.Warning("An invalid item ID was specified."));
                        return ActionResult.Continue;
                    }

                    // If the specified item was already selected, remove it from their trade.
                    if (GetCurrentItems().ContainsKey(input))
                    {
                        RemoveItemFromCurrent(input);
                        await SetStateAsync(TradeState.Inventory, "> 📤 Removed the specified item from your offer.");
                        HostReady = false;
                        ParticipantReady = false;
                        return ActionResult.Continue;
                    }

                    var selectedItem = ItemHelper.DataOf(account, input);

                    if (!ItemHelper.CanTrade(input, selectedItem.Data))
                    {
                        await SetStateAsync(TradeState.Inventory, Format.Warning("This item is unavailable for trading."));
                        return ActionResult.Continue;
                    }

                    AddItemToCurrent(input);
                    await SetStateAsync(TradeState.Inventory, "> 📥 Added the specified item to your offer.");
                    HostReady = false;
                    ParticipantReady = false;
                    return ActionResult.Continue;
            }

            return ActionResult.Continue;
        }

        private bool IsOfferEmpty()
        {
            return ParticipantOffer.Count == 0 && HostOffer.Count == 0;
        }

        private async Task SetStateAsync(TradeState state, string prepend = "")
        {
            if (State != state)
            {
                LastState = State;
                State = state;
            }

            var content = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(prepend))
                content.AppendLine(prepend);

            switch (state)
            {
                case TradeState.Menu:
                    content.Append(WriteTradeMenu());
                    break;

                case TradeState.Inventory:
                    if (!CurrentId.HasValue)
                        throw new Exception("Cannot read the inventory of an empty primary ID");

                    content.Append(WriteInventory(GetAccount(CurrentId.Value)));
                    break;

                case TradeState.Success:
                    content.AppendLine("> ✅ **Success!**\n> The trade has successfully gone through.");
                    content.Append(WriteTradeResult());
                    break;
            }

            await UpdateMessageAsync(content.ToString());
        }

        private string WriteTradeResult()
        {
            var result = new StringBuilder();

            result.AppendLine();

            if (ParticipantOffer.Count > 0)
            {
                result.AppendLine($"> **{Host.Username}** has received:");
                foreach ((string itemId, int amount) in ParticipantOffer)
                {
                    result.Append($"**{ItemHelper.NameOf(itemId)}**");

                    if (amount > 1)
                    {
                        result.Append($" (x**{amount:##,0}**)");
                    }

                    result.AppendLine();
                }
            }

            result.AppendLine();

            if (HostOffer.Count > 0)
            {
                result.AppendLine($"> **{Participant.Username}** has received:");
                foreach ((string itemId, int amount) in HostOffer)
                {
                    result.Append($"**{ItemHelper.NameOf(itemId)}**");

                    if (amount > 1)
                    {
                        result.Append($" (x**{amount:##,0}**)");
                    }

                    result.AppendLine();
                }
            }

            return result.ToString();
        }

        public override async Task OnTimeoutAsync(SocketMessage message)
            => await UpdateMessageAsync(WriteOnTimeout());

        public async Task UpdateMessageAsync(string content)
            => await MessageReference?.ModifyAsync(content);
    }
}