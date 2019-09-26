using System.Collections.Generic;

namespace Orikivo
{
    public static class CasinoDeckBuilder
    {
        public static Range SuitRange = new Range(1, 4);
        public static Range DefaultRankRange = new Range(1, 13);
        public static Range JokerRankRange = new Range(1, 14);
        public static Range DefaultDeckRange = new Range(1, 52);
        public static Range JokerDeckRange = new Range(1, 56);
        
        /*public CasinoDeck FromSeed(ulong rawSeed)
        {
            Seed seed = new Seed(rawSeed);
        }*/

        public static CasinoDeck Default(bool repeatCards = false, bool includeJoker = false)
        {
            Range rankRange = includeJoker ? JokerRankRange : DefaultRankRange;
            Range deckRange = includeJoker ? JokerDeckRange : DefaultDeckRange;
            List<CasinoCard> deck = new List<CasinoCard>();

            for (int i = 0; i < deckRange.Max; i++)
            {
                CasinoCard card = CasinoCard.Any;
            }
            return new CasinoDeck();
        }
    }

    public class CasinoDeck
    {
        public CasinoDeck(bool repeatCards = false, bool includeJoker = false)
        {

        }
        
        public bool IncludeJoker { get; } = false;
        public bool RepeatValues { get; } = false;
        public CasinoCardDeck Deck { get; private set; } = new CasinoCardDeck();

        public static CasinoDeck Default = CasinoDeckBuilder.Default();
        // this gets cards w/o removing them.
        public CasinoCard PeekTop { get { return Deck.Deck[0]; } }
        public CasinoCard PeekBottom { get { return Deck.Deck[Deck.Deck.Count - 1]; } }

        public CasinoCard GetTop()
        {
            CasinoCard card = PeekTop;
            Deck.Shift(card);
            return card;
        }

        public void Reshuffle()
        {

        }
    }

    public class CasinoCardDeck
    {
        public List<CasinoCard> Deck { get; private set; } = new List<CasinoCard>();
        public List<CasinoCard> Offhand { get; private set; } = new List<CasinoCard>();

        public void Shift(CasinoCard card)
        {
            if (!HasDeckedCard(card))
                return;

            Deck.Remove(card);
            Offhand.Add(card);
        }

        public void Return(CasinoCard card)
        {
            if (!HasOffhandCard(card))
                return;
            
            Offhand.Remove(card);
            Deck.Add(card);
        }

        public bool HasDeckedCard(CasinoCard card)
            => Deck.Contains(card);
        public bool HasOffhandCard(CasinoCard card)
            => Offhand.Contains(card);
        public bool HasCard(CasinoCard card)
            => HasDeckedCard(card) || HasOffhandCard(card);
    }

}