using System.Collections.Generic;

namespace Arcadia
{
    /// <summary>
    /// Represents a crafting recipe for an <see cref="Item"/>.
    /// </summary>
    public class Recipe
    {
        public List<RecipeComponent> Components { get; set; }

        public RecipeComponent Result { get; set; }
    }
}
