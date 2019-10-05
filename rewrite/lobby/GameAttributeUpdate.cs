namespace Orikivo
{
    // game attributes are only ints
    public class GameAttributeUpdate
    {
        internal GameAttributeUpdate(string id, int amount)
        {
            Id = id; // the id of the attribute to update.
            Amount = amount; // the amount to update a game attribute by.
        }

        public string Id { get; } // the name of the attribute to be updated.
        public int Amount { get; } // the value of the attribute to be updated.
    }
}
