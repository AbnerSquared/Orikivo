using System.Collections.Generic;

namespace Arcadia
{
    /// <summary>
    /// Represents a crafting recipe for an <see cref="Item"/>.
    /// </summary>
    public class Recipe
    {
        public List<RecipeComponent> Components { get; set; }

        // This is the primary result of a recipe
        public RecipeComponent Result { get; set; }

        // This is a collection of possible extra returning components on craft
        public List<RecipeResult> Extra { get; set; }
    }
}
