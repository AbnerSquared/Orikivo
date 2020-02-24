using Orikivo.Unstable;

namespace Orikivo
{
    /// <summary>
    /// Represents a crafting recipe for an <see cref="Item"/>.
    /// </summary>
    public class Recipe
    {
        // a collection of items (as IDs) that are required to craft
        public string[] RequiredItemIds { get; set; }

        // a collection of tools and workbenches (as IDs) that are needed to craft
        public string[] RequiredToolIds { get; set; }

        // the result of the craft
        public string ResultId { get; set; }

        // the amount of the result
        public int ResultCount { get; set; } = 1;
    }
}
