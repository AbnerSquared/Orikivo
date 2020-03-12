namespace Orikivo.Desync
{
    public class LogicArgument
    {
        public LogicArgument(string id, LogicMatch matchType, long matchValue)
        {
            Id = id;
            MatchType = matchType;
            MatchValue = matchValue;
        }

        public string Id { get; set; }
        public LogicMatch MatchType { get; set; }
        public long MatchValue { get; set; }
    }
}