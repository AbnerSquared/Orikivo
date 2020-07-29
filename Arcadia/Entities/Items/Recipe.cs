using System.Collections.Generic;

namespace Arcadia
{
    /// <summary>
    /// Represents a crafting recipe for an <see cref="Item"/>.
    /// </summary>
    public class Recipe
    {
        public string Id { get; set; }
        // a collection of items (as IDs) that are required to craft
        public List<RecipeComponent> Components { get; set; }

        // the result of the craft
        public RecipeComponent Result { get; set; }
    }
}
