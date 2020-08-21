namespace Arcadia
{
    public class VarCriterion
    {
        public VarCriterion(string id, long expectedValue)
        {
            Id = id;
            ExpectedValue = expectedValue;
        }

        public string Id { get; }

        public long ExpectedValue { get; }
    }
}
