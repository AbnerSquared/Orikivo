namespace Orikivo
{
    public class Card
    {
        public Card(CardRank rank, CardSuit suit)
        {
            Rank = rank;
            Suit = suit;
        }

        public CardRank Rank { get; }
        public CardSuit Suit { get; }

        public override string ToString()
            => $"{Rank} of {Suit}";

        public int Id => (int)Rank * (int)Suit;
    }
}
