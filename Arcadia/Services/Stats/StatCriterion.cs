namespace Arcadia
{
    public class StatCriterion
    {
        public StatCriterion(string id, long expectedValue)
        {
            Id = id;
            ExpectedValue = expectedValue;
        }

        public string Id { get; }
        public long ExpectedValue { get; }
    }
}
