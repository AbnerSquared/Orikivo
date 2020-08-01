namespace Arcadia.Multiplayer.Games
{
    internal static class WolfChannel
    {
        internal static readonly int Initial = 19;
        internal static readonly int Main = 20;
        internal static readonly int Results = 21;
        internal static readonly string ConsoleNode = "console";
        internal static readonly string HeaderNode = "header";
        internal static readonly string SummaryNode = "summary";
        internal static readonly string PlayersNode = "players";
        internal static readonly string FactsNode = "facts";

        internal static readonly int Death = 22;
        internal static readonly int Peek = 23;
        internal static readonly int Feast = 24;
        internal static readonly int Hunt = 25;
        internal static readonly int Hunted = 26;
        internal static readonly int Protect = 27;
    }
}