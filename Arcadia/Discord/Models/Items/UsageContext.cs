using Orikivo;

namespace Arcadia
{
    /// <summary>
    /// Represents information for an <see cref="Arcadia.Item"/> usage.
    /// </summary>
    public class UsageContext
    {
        /// <summary>
        /// Initializes a default <see cref="UsageContext"/>.
        /// </summary>
        /// <param name="user">The user that invoked this usage.</param>
        /// <param name="input">The raw input that was specified for this usage.</param>
        /// <param name="data">The item data instance that was referenced.</param>
        public UsageContext(ArcadeUser user, string input, ItemData data)
        {
            User = user;
            Input = input;
            Data = data;
            Item = ItemHelper.GetItem(data.Id);
        }

        /// <summary>
        /// Represents the user that invoked this usage.
        /// </summary>
        public ArcadeUser User { get; }

        public ArcadeContainer Container { get; }

        /// <summary>
        /// Represents the raw input that was specified for this usage.
        /// </summary>
        public string Input { get; }

        /// <summary>
        /// Represents the data instance for the <see cref="Item"/> that was used.
        /// </summary>
        public ItemData Data { get; }

        /// <summary>
        /// Represents the <see cref="Item"/> that was used.
        /// </summary>
        public Item Item { get; }
    }
}
