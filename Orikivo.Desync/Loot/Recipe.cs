namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a crafting recipe for an <see cref="Item"/>.
    /// </summary>
    public class Recipe
    {
        // a collection of items (as IDs) that are required to craft
        public RecipeComponent[] Components { get; set; }

        // a collection of tools and workbenches (as IDs) that are needed to craft
        public string[] RequiredToolIds { get; set; }

        // the result of the craft
        public string ResultId { get; set; }

        // the amount of the result
        public int ResultAmount { get; set; } = 1;
    }
}
