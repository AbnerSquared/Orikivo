namespace Orikivo
{
    public class CasinoCard
    {
        public static CasinoCard Any { get { return new CasinoCard(); } }
        public CasinoCardRank Rank { get; }
        public CasinoCardSuit Suit { get; }
        public override string ToString()
            => $"{Rank} of {Suit}";

        public int Id
        {
            get
{
                return int.Parse($"{(int)Rank}{(int)Suit}");
            }
        }
    }
}