namespace Orikivo
{
    /// <summary>
    /// A class defining the update parameters for a specified attribute.
    /// </summary>
    public class AttributeUpdatePacket
    {
        /// <summary>
        /// Creates a new update packet for an attribute.
        /// </summary>
        /// <param name="id">The identity of the attribute to update.</param>
        /// <param name="amount">The value that is to be used on the attribute.</param>
        /// <param name="method">The method that is used when updating the attribute.</param>
        internal AttributeUpdatePacket(string id, int amount, AttributeUpdateMethod method = AttributeUpdateMethod.Add)
        {
            Id = id; // the id of the attribute to update.
            Amount = amount; // the amount to update a game attribute by.
            Method = method;
        }

        /// <summary>
        /// The identity of the attribute to update.
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// The amount used to update the attribute.
        /// </summary>
        public int Amount { get; internal set; }

        /// <summary>
        /// The update method used on the attribute.
        /// </summary>
        public AttributeUpdateMethod Method { get; }
    }
}

