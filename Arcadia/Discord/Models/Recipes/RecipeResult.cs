namespace Arcadia
{
    /// <summary>
    /// Represents a returning <see cref="Item"/> from the result of a <see cref="Recipe"/>.
    /// </summary>
    public class RecipeResult
    {
        public RecipeResult() { }

        public RecipeResult(string itemId, int minStack = 1, int? maxStack = null, float chance = 1f) : this(itemId, new StackRange(minStack, maxStack), chance) { }
        
        public RecipeResult(string itemId, StackRange stack, float chance)
        {
            ItemId = itemId;
            Stack = stack;
            Chance = chance;
        }

        /// <summary>
        /// The ID of the <see cref="Item"/> to return.
        /// </summary>
        public string ItemId { get; set; }

        public StackRange Stack { get; set; }

        /// <summary>
        /// The chance that this component is returned.
        /// </summary>
        public float Chance { get; set; } = 1f;
    }
}
