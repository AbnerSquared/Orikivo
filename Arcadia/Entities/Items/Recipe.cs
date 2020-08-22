using System.Collections.Generic;

namespace Arcadia
{
    /// <summary>
    /// Represents a crafting recipe for an <see cref="Item"/>.
    /// </summary>
    public class Recipe
    {
        public string Id { get; set; }

        // TODO: Create recipes with variations of the same result with different amounts
        public List<RecipeComponent> Components { get; set; }

        public RecipeComponent Result { get; set; }
    }
}
