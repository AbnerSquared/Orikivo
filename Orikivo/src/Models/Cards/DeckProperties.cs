namespace Orikivo
{
    public class DeckProperties
    {
        public static readonly DeckProperties Default = new DeckProperties
        {
            Size = 52,
            DeckCount = 1,
            AllowJokers = false
        };

        public int Size { get; set; }

        // How many 52-card decks should be included?
        public int DeckCount { get; set; }
        public bool AllowJokers { get; set; }
    }
}