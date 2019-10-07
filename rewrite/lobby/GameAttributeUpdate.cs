namespace Orikivo
{
    // game attributes are only ints
    public class GameAttributeUpdate
    {
        internal GameAttributeUpdate(string id, int amount, AttributeUpdateType updateType = AttributeUpdateType.Append)
        {
            Id = id; // the id of the attribute to update.
            Amount = amount; // the amount to update a game attribute by.
            UpdateType = updateType;
        }

        public string Id { get; } // the name of the attribute to be updated.
        public int Amount { get; } // the value of the attribute to be updated.
        public AttributeUpdateType UpdateType { get; }
    }
}
