using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Orikivo;
using Orikivo.Framework;

namespace Arcadia.Casino
{
    public class BlackJackSession : BaseSession
    {
        public BlackJackSession(ArcadeUser invoker, ISocketMessageChannel channel, long wager) : base(invoker, channel)
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
        }

        public CardDeck Deck { get; }

        public List<Card> DealerHand { get; }

        public List<Card> Hand { get; }

        public long Wager { get; private set; }

        public BlackJackState State { get; private set; }

        private bool IsHidden { get; set; } = true;


        private string DrawCardSuit(CardSuit suit)
        {
            return suit switch
            {
                CardSuit.Clubs => "♣️",
                CardSuit.Diamonds => "♦️",
                CardSuit.Hearts => "♥️",
                CardSuit.Spades => "♠️",
                _ => throw new Exception("Unknown rsuit")
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

            result.AppendLine($"> **Dealer's Hand**{(State == BlackJackState.Active ? "" : $" (**{GetBestSum(DealerHand)}**)")}");
            result.AppendJoin("\n", DealerHand.Select((c, i) => $"> {(isHidden && i > 0 ? DrawHidden() : DrawCard(c))}"));

            return result.ToString();
        }

        private string DrawHandWorth()
        {
            var sums = GetPossibleSums(Hand);

            if (sums.Count == 0)
                return $"(**Bust!**)";

            if (State != BlackJackState.Active)
                return $"(**{GetBestSum(Hand)}**)";

            return $"({string.Join(" or ", sums.Select(x => $"**{x}**"))})";
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
            result.AppendLine($"> **Your Hand** {DrawHandWorth()}");
            result.AppendJoin("\n", Hand.Select(c => $"> {DrawCard(c)}"));
            return result.ToString();
        }

        private string DrawGameDisplay()
        {
            var display = new StringBuilder();

            display
                .AppendLine($"> **Blackjack**")
                .AppendLine($"> Playing with {CurrencyHelper.WriteCost(Wager, CurrencyType.Chips)}\n")
                .AppendLine($"{DrawDealerHand(IsHidden)}\n")
                .AppendLine($"{DrawPlayerHand()}\n")
                .AppendLine($"{(State == BlackJackState.Active ? DrawActions() : DrawStateResult())}");

            return display.ToString();
        }

        private string DrawStateResult()
        {
            if (State == BlackJackState.Active)
                return "";

            var result = new StringBuilder($"> **{State}**\n");
            result.AppendLine($"> {GetStateSummary(State)}");

            return result.ToString();
        }

        private string GetStateSummary(BlackJackState state)
        {
            return state switch
            {
                BlackJackState.Bust => "You have gone over **21**. The dealer has won.",
                BlackJackState.Fold => $"You have backed out of the game. You have only lost 50% of your wager ({CurrencyHelper.WriteCost(Wager, CurrencyType.Chips)}).",
                BlackJackState.Lose => $"The dealer has a stronger hand. You have lost {CurrencyHelper.WriteCost(Wager, CurrencyType.Chips)}.",
                BlackJackState.Win => $"You have a stronger hand than the dealer. You have won {CurrencyHelper.WriteCost(Wager, CurrencyType.Chips)}.",
                BlackJackState.Draw => $"You and the dealer have equal hands. Nobody has won.",
                BlackJackState.Timeout => "You have timed out. Your wager has been returned to you.",
                _ => "INVALID_STATE"
            };
        }

        private string DrawActions()
        {
            var actionBar = new StringBuilder();

            actionBar.AppendLine($"> **Actions**")
                .Append("`hit` `stand`");

            if (Invoker.ChipBalance >= Wager * 2)
                actionBar.Append(" `double`");

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

            // Logger.Debug($"{card.Rank} = {worth}");
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

        public override async Task OnStartAsync()
        {
            Invoker.AddToVar(BlackJackStats.TimesPlayed);
            DealerHand.Add(Deck.Take());
            DealerHand.Add(Deck.Take());
            Hand.Add(Deck.Take());
            Hand.Add(Deck.Take());

            Reference = await Channel.SendMessageAsync(DrawGameDisplay());
        }

        /// <inheritdoc />
        public override async Task<SessionTaskResult> OnMessageReceivedAsync(SocketMessage message)
        {
            string input = message.Content;

            if (input == "hit")
            {
                if (Hit() == BlackJackState.Bust)
                {
                    State = BlackJackState.Bust;
                    Invoker.Take(Wager, CurrencyType.Chips);
                    await UpdateAsync();
                    return SessionTaskResult.Success;
                }
                else
                {
                    await UpdateAsync();
                    return SessionTaskResult.Continue;
                }
            }

            if (input == "stand")
            {
                State = GetResult();

                if (State == BlackJackState.Win)
                {
                    if (GetBestSum(Hand) == 21)
                    {
                        Invoker.AddToVar(BlackJackStats.TimesWonExact);
                    }

                    Invoker.AddToVar(BlackJackStats.TimesWon);
                    Invoker.Give(Wager, CurrencyType.Chips);
                }
                else if (State != BlackJackState.Draw && State != BlackJackState.Timeout)
                    Invoker.Take(Wager, CurrencyType.Chips);

                await UpdateAsync();
                return SessionTaskResult.Success;
            }

            if (input == "double" && Invoker.ChipBalance >= Wager * 2)
            {
                Wager *= 2;

                if (Hit() == BlackJackState.Bust)
                {
                    State = BlackJackState.Bust;
                    Invoker.Take(Wager, CurrencyType.Chips);
                    await UpdateAsync();
                    return SessionTaskResult.Success;
                }

                State = GetResult();

                if (State == BlackJackState.Win)
                {
                    if (GetBestSum(Hand) == 21)
                    {
                        Invoker.AddToVar(BlackJackStats.TimesWonExact);
                    }

                    Invoker.AddToVar(BlackJackStats.TimesWon);
                    Invoker.Give(Wager, CurrencyType.Chips);
                }
                else if (State != BlackJackState.Draw && State != BlackJackState.Timeout)
                    Invoker.Take(Wager, CurrencyType.Chips);

                await UpdateAsync();
                return SessionTaskResult.Success;
            }

            if (input == "fold")
            {
                long partial = (long)Math.Floor(Wager / (double)2);
                Wager = partial == 0 ? 1 : partial;
                State = BlackJackState.Fold;
                Invoker.Take(Wager, CurrencyType.Chips);
                await UpdateAsync();
                return SessionTaskResult.Success;
            }

            return SessionTaskResult.Continue;
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