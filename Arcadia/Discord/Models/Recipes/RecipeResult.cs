namespace Arcadia
{
    public class RecipeResult
    {
        public RecipeResult()
        {

        }

        public RecipeResult(string itemId, int minStack = 1, int? maxStack = null, float chance = 1f)
        {
            ItemId = itemId;
            MinStack = minStack;
            MaxStack = maxStack;
            Chance = chance;
        }
        public string ItemId { get; set; }
        public int MinStack { get; set; } = 1;
        public int? MaxStack { get; set; }
        public float Chance { get; set; } = 1f;
    }
}
