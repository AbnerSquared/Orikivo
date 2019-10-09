namespace Orikivo
{
    public class AttributeCriterion : ICriterion<GameAttribute>
    {
        internal AttributeCriterion(string requiredId, int requiredValue)
        {
            RequiredId = requiredId;
            RequiredValue = requiredValue;
        }
        public string RequiredId { get; }
        public int RequiredValue { get; }

        public bool Check(GameAttribute attribute)
            => attribute.Id == RequiredId && attribute.Value == RequiredValue;
    }
}
