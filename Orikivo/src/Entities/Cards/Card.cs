using System.Collections.Generic;

namespace Orikivo
{
    public enum CardRank
    {
        Ace = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Joker = 14
    }

    public enum CardSuit
    {
        Clubs = 1,
        Diamonds = 2,
        Hearts = 3,
        Spades = 4
    }

    public class Card
    {
        public CardRank Rank;
        public CardSuit Suit;

        public override string ToString()
            => "";

        // Create parser system for card IDs
        public int Id => 0;
    }

    public class DeckProperties
    {
        public int Size { get; }
        // How many 52-card decks should be included?
        public int DeckCount { get; }
        public CardSuit AllowedSuits { get; }
        public CardRank AllowedRanks { get; }
    }

    public class CardDeck
    {
        public static CardDeck GetDefault()
        {
            return null;
            // Return a standard 52-card deck (no Jokers)
        }

        public static CardDeck Create(DeckProperties properties)
        {
            return null;
        }

        public List<Card> Cards { get; }
        public List<Card> Graveyard { get; }
        private List<Card> Taken { get; }

        public Card Peek(int index)
        {
            return null;
        }

        public Card Take()
        {
            return null;
        }

        public List<Card> Take(int amount)
        {
            return null;
        }

        public void Skip(int amount)
        {
            // Place all cards here into the graveyard
        }

        public void Shuffle()
        {
            // Shuffles all accessible cards
        }

        public void Reset()
        {
            // Shuffles and restores all cards taken
        }
    }
}
