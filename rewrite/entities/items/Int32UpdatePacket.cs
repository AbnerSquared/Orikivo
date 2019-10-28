namespace Orikivo
{
    /// <summary>
    /// Represents a generic update packet for an attribute of type Int32.
    /// </summary>
    public class Int32UpdatePacket
    {
        public Int32UpdatePacket(string id, int amount, UpdateMethod method = UpdateMethod.Add)
        {
            Id = id;
            Amount = amount;
            Method = method;
        }

        /// <summary>
        /// The unique identifier for an attribute.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The amount to utilize with the attribute.
        /// </summary>
        public int Amount { get; }

        /// <summary>
        /// The method used when updating the attribute.
        /// </summary>
        public UpdateMethod Method { get; }
    }
}
