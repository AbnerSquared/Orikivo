using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;

namespace Arcadia
{
    public static class CraftHelper
    {
        public static bool RecipeExists(string id)
            => Check.NotNull(id)
               && Assets.Recipes.Any(x => CompareRecipeId(GetRecipeId(x), id));

        // Scrap this.
        public static string GetBaseRecipeId(Recipe recipe)
        {
            if (recipe.Components.Count == 0)
                throw new Exception("Expected at least one recipe component");

            var id = new StringBuilder();

            id.Append($"{recipe.Result.ItemId}/");
            id.AppendJoin('.', recipe.Components.Select(GetComponentId));

            return id.ToString();
        }

        public static string GetRecipeId(Recipe recipe)
            => $"recipe:{GetBaseRecipeId(recipe)}";

        public static Dictionary<string, int> GetMissingFromRecipe(ArcadeUser user, Recipe recipe)
        {
            if (recipe == null)
                throw new Exception("Could not find a recipe with the specified ID");

            var missing = new Dictionary<string, int>();

            foreach ((string itemId, int amount) in recipe.Components)
            {
                int owned = ItemHelper.GetOwnedAmount(user, itemId);

                if (owned < amount)
                    missing[itemId] = amount - owned;
            }

            return missing;
        }

        public static IEnumerable<Recipe> GetKnownRecipes(ArcadeUser user)
        {
            return Assets.Recipes.Where(x => GetRecipeStatus(user, x) != RecipeStatus.Unknown);
        }

        public static IEnumerable<Recipe> RecipesFor(string itemId)
            => RecipesFor(ItemHelper.GetItem(itemId));

        public static IEnumerable<Recipe> RecipesFor(Item item)
        {
            return Assets.Recipes.Where(x => x.Result.ItemId == item.Id);
        }

        public static Recipe GetRecipe(string id)
        {
            if (Assets.Recipes.Count(x => CompareRecipeId(GetRecipeId(x), id)) > 1)
                throw new ArgumentException("There are more than one recipes with the specified ID.");

            return Assets.Recipes.FirstOrDefault(x => CompareRecipeId(GetRecipeId(x), id));
        }

        public static RecipeStatus GetRecipeStatus(ArcadeUser user, Recipe recipe)
        {
            return (RecipeStatus)user.GetVar(GetRecipeId(recipe));
        }

        public static void UpdateKnownRecipes(ArcadeUser user)
        {
            foreach (Recipe recipe in Assets.Recipes.Where(x => CanCraft(user, x)))
                SetRecipeStatus(user, recipe, RecipeStatus.Known);
        }

        public static void UpdateRecipeStatus(ArcadeUser user, Recipe recipe)
        {
            if (CanCraft(user, recipe))
                SetRecipeStatus(user, recipe, RecipeStatus.Known);
        }

        public static void SetRecipeStatus(ArcadeUser user, Recipe recipe, RecipeStatus status)
        {
            if (GetRecipeStatus(user, recipe) >= status)
                return;

            user.SetVar(GetRecipeId(recipe), (long)status);
        }

        public static bool KnowsRecipe(ArcadeUser user, string recipeId)
            => KnowsRecipe(user, GetRecipe(recipeId));

        public static bool KnowsRecipe(ArcadeUser user, Recipe recipe)
        {
            return user.GetVar(GetRecipeId(recipe)) == (long)RecipeStatus.Known;
        }

        public static bool TryGetRecipe(string id, out Recipe recipe)
        {
            recipe = null;

            if (!RecipeExists(id))
                return false;

            recipe = GetRecipe(id);
            return true;
        }

        public static bool Craft(ArcadeUser user, string recipeId)
            => Craft(user, GetRecipe(recipeId));

        public static bool Craft(ArcadeUser user, Recipe recipe)
        {
            if (GetRecipeStatus(user, recipe) == RecipeStatus.Unknown)
                return false;

            if (!CanCraft(user, recipe))
                return false;

            if (recipe.Result == null)
                throw new Exception("Expected recipe result but returned null");

            foreach ((string itemId, int amount) in recipe.Components)
                ItemHelper.TakeItem(user, itemId, amount);

            ItemHelper.GiveItem(user, recipe.Result.ItemId, recipe.Result.Amount);

            user.AddToVar(Stats.Common.ItemsCrafted);
            return true;
        }
        public static bool CanCraft(Item item)
            => Assets.Recipes.Any(x => x.Result.ItemId == item.Id);

        public static bool CanCraft(ArcadeUser user, string recipeId)
            => CanCraft(user, GetRecipe(recipeId));

        public static bool CanCraft(ArcadeUser user, Recipe recipe)
        {
            if (recipe == null)
                throw new Exception("Could not find a recipe with the specified ID");

            foreach ((string itemId, int amount) in recipe.Components)
            {
                if (!ItemHelper.HasItem(user, itemId) || ItemHelper.GetOwnedAmount(user, itemId) != amount)
                    return false;
            }

            return true;
        }

        private static bool CompareRecipeId(string id, string input)
            => id == input || id[7..] == input;

        private static string GetComponentId(RecipeComponent component)
            => $"{component.ItemId}{(component.Amount > 1 ? $"#{component.Amount}" : "")}";
    }
}