using System.Collections.Generic;
using Orikivo;

namespace Arcadia.Casino
{
    public enum BlackjackResultFlag
    {
        Win = 1,
        Lose = 2
    }

    public class Blackjack
    {
        // Between 1-8 unique card decks are shuffled
        public CardDeck Deck { get; }
        public List<Card> Dealer { get; }
        public List<Card> Player { get; }

        public static int GetHandSum(List<Card> cards)
        {
            // Sum all of the cards in a hand together
            // If it is over 21, the deck is invalid
             return 0;
        }

        // Handle

        // Actions:
        // Hit: Take a card
        // Stand: End their turn
        // Double Down: Double the wager, take a card, end turn
        // Split: If two cards have the same value, separate them into 2 hands
        // Fold: Retire and lose 50% of the wager
    }

    public class BlackjackHandle
    {
        // Handle a unique session of blackjack here
    }

    public class BlackjackResult
    {

    }
}
