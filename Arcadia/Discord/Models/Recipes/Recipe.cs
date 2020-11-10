using System.Collections.Generic;
using Arcadia.Models;

namespace Arcadia
{
    /// <summary>
    /// Represents a crafting recipe for an <see cref="Item"/>.
    /// </summary>
    public class Recipe : IModel<string>
    {
        public string Id => CraftHelper.GetRecipeId(this);

        public string Name => ItemHelper.NameOf(Result?.ItemId);

        public List<RecipeComponent> Components { get; set; }

        // This is the primary result of a recipe
        public RecipeComponent Result { get; set; }

        // This is a collection of possible extra returning components on craft
        // An example is: crafting a color core from a glass jar, returning the jar
        public List<RecipeResult> Extra { get; set; }
    }
}
