namespace Orikivo
{
    /// <summary>
    /// An object used to keep track of a specific value across a game.
    /// </summary>
    public class GameAttribute
    {
        public GameAttribute(string id, int defaultValue = 0)
        {
            Id = id;
            Value = DefaultValue = defaultValue;
        }

        /// <summary>
        /// The unique identifier of this attribute.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The attribute's current value.
        /// </summary>
        public int Value { get; internal set; }

        /// <summary>
        /// The default value used for this attribute.
        /// </summary>
        public int DefaultValue { get; }
    }
}
