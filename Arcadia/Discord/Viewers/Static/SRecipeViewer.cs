using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;

namespace Arcadia.Services
{
    public class SRecipeViewer
    {
        public static string View(ArcadeUser user)
        {
            CraftHelper.UpdateKnownRecipes(user);
            var recipes = new StringBuilder();
            recipes.AppendLine(Locale.GetHeader(Headers.Recipe));

            IEnumerable<Recipe> known = CraftHelper.GetKnownRecipes(user).ToList();

            if (!Check.NotNullOrEmpty(known))
            {
                recipes.AppendLine("> You don't know any recipes.");
                return recipes.ToString();
            }

            // Group recipes by: Var.GetKey(id).StartsWith(itemId)
            foreach (Recipe recipe in known)
                recipes.AppendLine(WriteRecipeText(user, recipe));

            return recipes.ToString();
        }

        private static string WriteRecipeText(ArcadeUser user, Recipe recipe)
        {
            var text = new StringBuilder();

            string recipeName = ItemHelper.GetItem(recipe.Result.ItemId)?.Name;

            if (!Check.NotNull(recipeName))
                recipeName = "Unknown Item";

            text.AppendLine($"\n> `{CraftHelper.GetInternalRecipeId(recipe)}`")
                .Append($"> {(CraftHelper.CanCraft(user, recipe) ? "📑" : "📄")} **Recipe: {recipeName}**");

            return text.ToString();
        }

        // Group all recipes with the same result ID
        // Paginate all variations
        public static string ViewRecipeInfo(ArcadeUser user, Recipe recipe)
        {
            var info = new StringBuilder();

            string resultName = ItemHelper.GetItem(recipe.Result.ItemId)?.Name ?? "Unknown Item";
            bool craft = CraftHelper.CanCraft(user, recipe);

            if (craft)
            {
                CraftHelper.SetRecipeStatus(user, recipe, RecipeStatus.Known);
            }
            else if (CraftHelper.GetRecipeStatus(user, recipe) == RecipeStatus.Unknown)
                return Format.Warning("Unknown recipe specified.");


            info.AppendLine($"> {(craft ? "📑" : "📄")} **Recipe: {resultName}**");

            if (craft)
                info.AppendLine("> You can craft this recipe.");

            info.AppendLine($"\n> **Components**");

            foreach ((string itemId, int amount) in recipe.Components)
                info.AppendLine(WriteRecipeComponent(itemId, amount));

            info.AppendLine($"\n> **Result**");
            info.AppendLine(WriteRecipeComponent(recipe.Result.ItemId, recipe.Result.Amount));

            return info.ToString();
        }

        public static string WriteRecipeComponent(string itemId, int amount)
        {
            string icon = ItemHelper.IconOf(itemId);

            if (!Check.NotNull(icon))
                icon = "•";

            string name = ItemHelper.NameOf(itemId);

            string counter = Format.ObjectCount(amount, false);

            return $"`{itemId}` {icon} **{name}**{counter}";
        }
    }
}