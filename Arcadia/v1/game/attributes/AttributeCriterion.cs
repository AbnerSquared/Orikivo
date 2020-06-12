using System;

namespace Arcadia.Old
{
    /// <summary>
    /// Defines a criterion judgement for an attribute.
    /// </summary>
    public class AttributeCriterion : IGameCriterion<GameAttribute>
    {
        internal AttributeCriterion(string requiredId, int requiredValue, AttributeMatch match = AttributeMatch.Equals)
        {
            RequiredId = requiredId;
            RequiredValue = requiredValue;
            Match = match;
        }

        /// <summary>
        /// The attribute to be checked.
        /// </summary>
        public string RequiredId { get; }

        /// <summary>
        /// The required value that will be used to check the attribute.
        /// </summary>
        public int RequiredValue { get; }

        /// <summary>
        /// The type of attribute match that is required to pass the check.
        /// </summary>
        public AttributeMatch Match { get; }

        public bool Check(GameAttribute attribute)
            => attribute.Id == RequiredId && Compare(attribute.Value);

        // Compares the specified attribute value upon its match type.
        private bool Compare(int value)
        {
            switch(Match)
            {
                case AttributeMatch.Equals:
                    return value == RequiredValue;
                case AttributeMatch.NotEquals:
                    return value != RequiredValue;
                case AttributeMatch.Greater:
                    return value > RequiredValue;
                case AttributeMatch.Lesser:
                    return value < RequiredValue;
                case AttributeMatch.GreaterOrEquals:
                    return value >= RequiredValue;
                case AttributeMatch.LesserOrEquals:
                    return value <= RequiredValue;
                default:
                    throw new Exception("An unknown type of AttributeMatch was specified.");
            }
        }
    }
}
