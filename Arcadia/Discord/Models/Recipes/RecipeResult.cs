namespace Arcadia
{
    /// <summary>
    /// Represents a returning <see cref="Item"/> from the result of a <see cref="Recipe"/>.
    /// </summary>
    public class RecipeResult
    {
        public RecipeResult() { }

        public RecipeResult(string itemId, int minStack = 1, int? maxStack = null, float chance = 1f)
        {
            ItemId = itemId;
            MinStack = minStack;
            MaxStack = maxStack;
            Chance = chance;
        }

        /// <summary>
        /// The ID of the <see cref="Item"/> to return.
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// The minimum amount of this component to return.
        /// </summary>
        public int MinStack { get; set; } = 1;

        /// <summary>
        /// The maximum amount of this component to return.
        /// </summary>
        public int? MaxStack { get; set; }

        /// <summary>
        /// The chance that this component is returned.
        /// </summary>
        public float Chance { get; set; } = 1f;
    }
}
