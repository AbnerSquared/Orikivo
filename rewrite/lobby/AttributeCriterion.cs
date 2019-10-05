namespace Orikivo
{
    public class AttributeCriterion
    {
        public AttributeCriterion(GameAttribute attribute, int requiredValue)
        {
            AttributeId = attribute.Name;
            RequiredValue = requiredValue;
        }
        public string AttributeId { get; }
        public int RequiredValue { get; }

        public bool Check(GameAttribute attribute)
        {
            if (attribute.Name == AttributeId)
                if (attribute.Value == RequiredValue)
                    return true;
            return false;
        }
    }
}
