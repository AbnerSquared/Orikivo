using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Addons.Collectors;
using Discord.WebSocket;
using Orikivo;
using Orikivo.Text;

namespace Arcadia.Casino
{
    public class BlackJackSession : BaseSession
    {
        public BlackJackSession(ArcadeUser invoker, ISocketMessageChannel channel, long wager, LocaleProvider locale) : base(invoker, channel)
        {
            Deck = CardDeck.Create(new DeckProperties
            {
                Size = 52,
                AllowJokers = false,
                DeckCount = 4
            });

            DealerHand = new List<Card>();
            Hand = new List<Card>();

            State = BlackJackState.Active;
            Wager = wager;
            Locale = locale.GetBank(Invoker.Config.Language);
        }

        public LocaleBank Locale { get; }

        public CardDeck Deck { get; }

        public List<Card> DealerHand { get; }

        public List<Card> Hand { get; private set; }

        public BlackJackHand Offhand { get; } = new BlackJackHand();

        public long Wager { get; private set; }

        public BlackJackState State { get; private set; }

        private bool IsHidden { get; set; } = true;

        public bool HasSplit { get; private set; }

        public bool HasExecutedAction { get; private set; }

        private string DrawCardSuit(CardSuit suit)
        {
            return suit switch
            {
                CardSuit.Clubs => "♣️",
                CardSuit.Diamonds => "♦️",
                CardSuit.Hearts => "♥️",
                CardSuit.Spades => "♠️",
                _ => throw new Exception("Unknown card suit")
            };
        }

        private string DrawCardRank(CardRank rank)
        {
            if ((int)rank >= 2 && (int)rank <= 10)
                return $"{(int)rank}";

            return rank switch
            {
                CardRank.Joker => "\\*",
                _ when rank >= CardRank.Two && rank <= CardRank.Ten => $"{(int)rank}",
                _ when rank > 0 && rank <= CardRank.King => $"{rank.ToString()[0]}",
                _ => throw new Exception("UNKNOWN_RANK")
            };
        }

        private string DrawCard(Card card)
        {
            return $"{DrawCardSuit(card.Suit)} **{DrawCardRank(card.Rank)}**";
        }

        private string DrawHidden()
            => "||[Hidden]||";

        private string DrawDealerHand(bool isHidden = false)
        {
            var result = new StringBuilder();

            result.AppendLine($"> **{Locale.GetString("blackjack_dealer_hand")}**{(State == BlackJackState.Active ? "" : $" (**{GetBestSum(DealerHand)}**)")}");
            result.AppendJoin("\n", DealerHand.Select((c, i) => $"> {(isHidden && i > 0 ? DrawHidden() : DrawCard(c))}"));

            return result.ToString();
        }

        private string DrawHandWorth(List<Card> hand)
        {
            var sums = GetPossibleSums(hand);

            if (sums.Count == 0)
                return $"(**{Locale.GetString("blackjack_bust")}**)"; // Bust!

            if (State != BlackJackState.Active)
                return $"(**{GetBestSum(hand)}**)";

            return $"({string.Join($" {Locale.GetString("blackjack_or")} ", sums.Select(x => $"**{x}**"))})";
        }

        private List<int> GetPossibleSums(List<Card> hand)
        {
            var possible = new List<int>();
            int aceCount = hand.Count(c => c.Rank == CardRank.Ace);
            int baseSum = GetHandSum(hand);

            if (aceCount > 0)
            {
                possible.Add(baseSum + aceCount);
                possible.Add(baseSum + 11 + (aceCount - 1));
            }
            else
            {
                possible.Add(baseSum);
            }

            possible.RemoveAll(sum => !IsValid(sum));
            return possible;
        }

        private string DrawPlayerHand()
        {
            var result = new StringBuilder();
            result.AppendLine($"> **{Locale.GetString("blackjack_your_hand")}** {DrawHandWorth(Hand)}");
            result.AppendJoin("\n", Hand.Select(c => $"> {DrawCard(c)}"));
            return result.ToString();
        }

        private string DrawSecondaryHand(string title)
        {
            var result = new StringBuilder();

            result
                .Append($"> {title}\n> ")
                .AppendJoin(" • ", Offhand.Cards.Select(DrawCard))
                .AppendLine();

            return result.ToString();
        }

        private string DrawGameDisplay()
        {
            var display = new StringBuilder();

            display
                .AppendLine($"> **{Locale.GetString("blackjack_header")}**")
                // Playing with {0}
                .AppendLine($"> {Locale.GetString("blackjack_hand_title")} {CurrencyHelper.WriteCost(Wager, CurrencyType.Chips)}\n")
                .AppendLine($"{DrawDealerHand(IsHidden)}\n")
                .AppendLine($"{DrawPlayerHand()}\n");

            // off-hand draw

            if (Offhand.Cards.Count > 0)
            {
                string title = $"**{Locale.GetString("blackjack_offhand_title")}** {DrawHandWorth(Offhand.Cards)}";

                if (Offhand.State != BlackJackState.Active)
                {
                    bool isLoss = Offhand.State.EqualsAny(BlackJackState.Bust, BlackJackState.Lose, BlackJackState.Fold);
                    string wager = Offhand.State == BlackJackState.Draw ? "" : $" [{(isLoss ? "-" : "+")} {CurrencyHelper.WriteCost(Offhand.Wager, CurrencyType.Chips)}]";
                    title = $"**{Locale.GetString("blackjack_previous_title")}** (**{Locale.GetString($"blackjack_state_{Offhand.State.ToString().ToLower()}")}!**){wager}";
                }

                display.AppendLine(DrawSecondaryHand(title));
            }

            display.AppendLine($"{(State == BlackJackState.Active ? DrawActions() : GetStateResult())}");

            return display.ToString();
        }

        private string GetStateResult()
        {
            if (State == BlackJackState.Active)
                return "";

            var result = new StringBuilder($"> **{Locale.GetString($"blackjack_state_{State.ToString().ToLower()}")}!**\n");
            result.AppendLine($"> {GetStateSummary(State)}");

            return result.ToString();
        }

        private string GetStateSummary(BlackJackState state)
        {
            string chips = CurrencyHelper.WriteCost(Wager, CurrencyType.Chips);
            return state switch
            {
                BlackJackState.Bust => Locale.GetString("blackjack_on_bust"),
                BlackJackState.Fold => Locale.GetString("blackjack_on_fold", chips),
                BlackJackState.Lose => Locale.GetString("blackjack_on_lose", chips),
                BlackJackState.Win => Locale.GetString("blackjack_on_win", chips),
                BlackJackState.Draw => Locale.GetString("blackjack_on_draw"),
                BlackJackState.Timeout => Locale.GetString("blackjack_on_timeout"),
                _ => "INVALID_STATE"
            };
        }

        private string DrawActions()
        {
            var actionBar = new StringBuilder();

            actionBar.AppendLine($"> **{Locale.GetString("blackjack_action_title")}**")
                .Append("`hit` `stand`");

            if (Invoker.ChipBalance >= Wager * 2)
                actionBar.Append(" `double`");

            if (Invoker.ChipBalance >= Wager * 2 && !HasExecutedAction && !HasSplit && Hand.Count == 2 && Hand.All(x => x.Rank == Hand.First().Rank))
                actionBar.Append(" `split`");

            actionBar.Append(" `fold`");

            return actionBar.ToString();
        }

        public int GetWorth(Card card)
        {
            int worth = 0;


            if (card.Rank == CardRank.Joker)
                throw new Exception("Jokers are not supported in Blackjack");

            if (card.Rank.EqualsAny(CardRank.King, CardRank.Queen, CardRank.Jack))
                worth = 10;

            else if (card.Rank == CardRank.Ace)
                worth = 0;
            else
                worth = (int)card.Rank;

            return worth;
        }

        public static bool IsValid(int sum)
            => sum <= 21;

        public static bool CanSplit(List<Card> cards)
        {
            throw new NotImplementedException();
        }

        public int GetHandSum(List<Card> cards)
        {
            // Sum all of the cards in a hand together (aces are handled separately
            // If it is over 21, the deck is invalid
            return cards.Sum(GetWorth);
        }

        private int GetBestSum(List<Card> hand)
        {
            var sums = GetPossibleSums(hand);

            if (sums.Count == 0)
                return 22;

            return sums
                .OrderByDescending(x => x)
                .FirstOrDefault();
        }

        public BlackJackState Hit()
        {
            Hand.Add(Deck.Take());

            if (GetBestSum(Hand) > 21)
                return BlackJackState.Bust;

            return BlackJackState.Active;
        }

        public BlackJackState DoubleDown()
        {
            Wager *= 2;
            Hit();
            return GetResult();
        }

        private void FinalizeDealer()
        {
            if (GetBestSum(DealerHand) >= 17)
                return;

            while (GetHandSum(DealerHand) < 17)
            {
                Card toAdd = Deck.Take();
                DealerHand.Add(toAdd);

                if (toAdd.Rank == CardRank.Ace && GetHandSum(DealerHand) + 11 >= 17 && GetHandSum(DealerHand) + 11 <= 21)
                    break;
            }
        }

        // equal to executing a stand action
        public BlackJackState GetResult()
        {
            IsHidden = false;
            int playerSum = GetBestSum(Hand);

            if (playerSum > 21)
                return BlackJackState.Bust;

            FinalizeDealer();
            int dealerSum = GetBestSum(DealerHand);

            if (dealerSum > 21 || playerSum > dealerSum)
                return BlackJackState.Win;

            if (playerSum == dealerSum)
                return BlackJackState.Draw;

            return BlackJackState.Lose;
        }

        private bool HasSplitHand()
        {
            return Offhand.Cards.Count > 0 && Offhand.State == BlackJackState.Active;
        }

        public override async Task OnStartAsync()
        {
            Invoker.AddToVar(Stats.BlackJack.TimesPlayed);
            DealerHand.AddRange(Deck.Take(), Deck.Take());
            Hand.AddRange(Deck.Take(), Deck.Take());
            Reference = await Channel.SendMessageAsync(DrawGameDisplay());
        }

        // puts primary hand to offhand with its result.
        private void SwapHands()
        {
            List<Card> offhand = new List<Card>(Offhand.Cards);
            Offhand.Cards = new List<Card>(Hand);
            Offhand.State = State;
            Offhand.Wager = Wager;
            Hand = offhand;
            State = BlackJackState.Active;
        }

        /// <inheritdoc />
        public override async Task<SessionResult> OnMessageReceivedAsync(SocketMessage message)
        {
            string input = message.Content;

            if (input == "hit")
            {
                if (!HasExecutedAction)
                    HasExecutedAction = true;

                if (Hit() == BlackJackState.Bust)
                {
                    State = BlackJackState.Bust;
                    Invoker.Take(Wager, CurrencyType.Chips);

                    if (HasSplitHand())
                    {
                        SwapHands();
                        await UpdateAsync();
                        return SessionResult.Continue;
                    }

                    await UpdateAsync();
                    return SessionResult.Success;
                }
                else
                {
                    await UpdateAsync();
                    return SessionResult.Continue;
                }
            }

            if (input == "stand")
            {
                if (!HasExecutedAction)
                    HasExecutedAction = true;

                State = GetResult();

                if (State == BlackJackState.Win)
                {
                    if (GetBestSum(Hand) == 21)
                    {
                        Invoker.AddToVar(Stats.BlackJack.TimesWonExact);
                    }

                    Invoker.AddToVar(Stats.BlackJack.TimesWon);
                    Invoker.Give(Wager, CurrencyType.Chips);
                }
                else if (State != BlackJackState.Draw && State != BlackJackState.Timeout)
                    Invoker.Take(Wager, CurrencyType.Chips);

                if (HasSplitHand())
                {
                    SwapHands();
                    await UpdateAsync();
                    return SessionResult.Continue;
                }

                await UpdateAsync();
                return SessionResult.Success;
            }

            if (input == "double" && Invoker.ChipBalance >= Wager * 2)
            {
                if (!HasExecutedAction)
                    HasExecutedAction = true;

                Wager *= 2;

                if (Hit() == BlackJackState.Bust)
                {
                    State = BlackJackState.Bust;
                    Invoker.Take(Wager, CurrencyType.Chips);

                    if (HasSplitHand())
                    {
                        SwapHands();
                        await UpdateAsync();
                        return SessionResult.Continue;
                    }

                    await UpdateAsync();
                    return SessionResult.Success;
                }

                State = GetResult();

                if (State == BlackJackState.Win)
                {
                    if (GetBestSum(Hand) == 21)
                    {
                        Invoker.AddToVar(Stats.BlackJack.TimesWonExact);
                    }

                    Invoker.AddToVar(Stats.BlackJack.TimesWon);
                    Invoker.Give(Wager, CurrencyType.Chips);
                }
                else if (State != BlackJackState.Draw && State != BlackJackState.Timeout)
                    Invoker.Take(Wager, CurrencyType.Chips);

                if (HasSplitHand())
                {
                    SwapHands();
                    await UpdateAsync();
                    return SessionResult.Continue;
                }

                await UpdateAsync();
                return SessionResult.Success;
            }

            if (input == "split" && Invoker.ChipBalance >= Wager * 2 && !HasSplit && Hand.Count == 2 && Hand.All(x => x.Rank == Hand.First().Rank))
            {
                if (!HasExecutedAction)
                    HasExecutedAction = true;

                HasSplit = true;
                Card copy = Hand.ElementAt(Hand.Count - 1);
                Hand.RemoveAt(Hand.Count - 1);
                Hand.Add(Deck.Take());

                Offhand.Cards.Add(copy);
                Offhand.Cards.Add(Deck.Take());

                await UpdateAsync();
                return SessionResult.Continue;
            }

            if (input == "fold")
            {
                if (!HasExecutedAction)
                    HasExecutedAction = true;

                long old = Wager;
                long partial = (long)Math.Floor(Wager / (double)2);
                Wager = partial == 0 ? 1 : partial;
                State = BlackJackState.Fold;
                Invoker.Take(Wager, CurrencyType.Chips);

                if (HasSplitHand())
                {
                    SwapHands();
                    Wager = old;
                    await UpdateAsync();
                    return SessionResult.Continue;
                }

                await UpdateAsync();
                return SessionResult.Success;
            }

            return SessionResult.Continue;
        }

        private async Task UpdateAsync()
        {
            if (State != BlackJackState.Active)
                IsHidden = false;

            await Reference.ModifyAsync(DrawGameDisplay());
        }

        /// <inheritdoc />
        public override async Task OnTimeoutAsync(SocketMessage message)
        {
            State = BlackJackState.Timeout;
            IsHidden = false;
            await Reference.ModifyAsync(DrawGameDisplay());
        }

        // Handle

        // Actions:
        // Hit: Take a card
        // Stand: End their turn
        // Double Down: Double the wager, take a card, end turn
        // Split: If two cards have the same value, separate them into 2 hands
        // Fold: Retire and lose 50% of the wager
    }
}
