using System;
using System.Collections.Generic;
using System.Linq;
using Orikivo.Framework;

namespace Orikivo
{
    public class CardDeck
    {
        public static CardDeck GetDefault()
        {
            return Create(DeckProperties.Default);
        }

        private static void AddCardsInSuit(ref List<Card> cards, CardSuit suit, bool allowJokers = false)
        {
            int upper = (int)(allowJokers ? CardRank.Joker : CardRank.King);

            for (int rank = (int)CardRank.Ace; rank <= upper; rank++)
                cards.Add(new Card((CardRank)rank, suit));
        }

        private static List<Card> GetCardPack(bool allowJokers = false)
        {
            var cards = new List<Card>();

            for (int suit = (int)CardSuit.Clubs; suit < (int)CardSuit.Spades; suit++)
                AddCardsInSuit(ref cards, (CardSuit)suit, allowJokers);

            return cards;
        }

        public static CardDeck Create(DeckProperties properties)
        {
            properties ??= DeckProperties.Default;

            List<Card> available = Randomizer.Shuffle(GetCardPack(properties.AllowJokers)).ToList();
            Logger.Debug($"{available.Count}");
            int limit = properties.Size > available.Count ? available.Count : properties.Size;

            if (limit == properties.Size)
                available.RemoveRange(properties.Size - 1, available.Count - properties.Size);
            Logger.Debug($"{available.Count}");
            return new CardDeck(available);
        }

        private CardDeck(List<Card> cards)
        {
            Cards = cards;
            Graveyard = new List<Card>();
            Taken = new List<Card>();
        }

        public List<Card> Cards { get; }
        public List<Card> Graveyard { get; }
        private List<Card> Taken { get; }

        public Card Peek(int depth)
        {
            return Cards[Cards.Count - 1 - depth];
        }

        public Card Take()
        {
            Card card = Cards[^1];
            Taken.Add(card);
            Cards.RemoveAt(Cards.Count - 1);
            return card;
        }

        public List<Card> Take(int amount)
        {
            throw new NotImplementedException();
            // var cards = new List<Card>();
            // return null;
        }

        public void Skip(int amount)
        {
            // Place all cards here into the graveyard
            throw new NotImplementedException();
            // var cards = new List<Card>();
            // return null;
        }

        public void Shuffle()
        {
            // Shuffles all accessible cards
            throw new NotImplementedException();
        }

        public void Reset()
        {
            // Shuffles and restores all cards taken
            throw new NotImplementedException();
        }
    }
}
