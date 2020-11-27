using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;
using Orikivo.Text.Pagination;

namespace Arcadia.Services
{
    public class SRecipeViewer
    {
        private static IEnumerable<IGrouping<string, Recipe>> GroupItemRecipes(IEnumerable<Recipe> recipes)
        {
            return recipes.GroupBy(x => x.Result.ItemId);
        }

        // TODO: Support pagination on recipes, since there could be a LOT
        // If there is only one recipe variation for an item, showcase it as a single page instead
        public static string View(ArcadeUser user, int page = 0)
        {
            CraftHelper.UpdateKnownRecipes(user);

            IEnumerable<Recipe> known = CraftHelper.GetKnownRecipes(user);

            var recipes = new StringBuilder();

            if (!Check.NotNullOrEmpty(known))
            {
                recipes.AppendLine(Locale.GetHeader(Headers.Recipe)).AppendLine();
                recipes.AppendLine("> Your recipe book is empty.");
                return recipes.ToString();
            }

            IEnumerable<IGrouping<string, Recipe>> knownGroups = GroupItemRecipes(known);

            if (user.Config.Tooltips)
            {
                var tooltips = new List<string>();

                if (knownGroups.Any(x => x.Count() > 1))
                    tooltips.Add("Type `recipes <item_id> [page]` to view all of the recipe variations of a single item.");

                if (knownGroups.Any(x => x.Count() == 1))
                    tooltips.Add("Type `recipe <recipe_id>` to view the required components of a specific recipe.");

                if (tooltips.Count > 0)
                {
                    recipes
                    .AppendLine(Format.Tooltip(tooltips))
                    .AppendLine();
                }
            }

            recipes.AppendLine(Locale.GetHeader(Headers.Recipe)).AppendLine();

            foreach (IGrouping<string, Recipe> group in knownGroups)
                recipes.AppendLine(PreviewRecipeGroup(group.Key, group));

            return recipes.ToString();
        }

        private static string PreviewRecipeGroup(string itemId, IEnumerable<Recipe> recipes)
        {
            var result = new StringBuilder();

            // If there is only one variation, simply show that page
            if (recipes.Count() == 1)
            {
                return result
                    .AppendLine($"> `{CraftHelper.GetInternalRecipeId(recipes.First())}`")
                    .Append($"> 📄 **{ItemHelper.GetBaseName(itemId)}**")
                    .ToString();
            }

            return result
                .AppendLine($"> `{itemId}` (**{recipes.Count()}** {Format.TryPluralize("variation", recipes.Count())} discovered)")
                .Append($"> 📒 **{ItemHelper.GetBaseName(itemId)}**")
                .ToString();
        }

        private static string ViewVariation(Recipe variation)
        {
            int componentSum = variation.Components.Sum(x => x.Amount);

            var result = new StringBuilder();

            result.AppendLine($"> `{CraftHelper.GetInternalRecipeId(variation)}`");
            result.AppendLine($"> **{ItemHelper.GetBaseName(variation.Result.ItemId)}** (**{componentSum}** {Format.TryPluralize("component", componentSum)})");

            foreach (RecipeComponent component in variation.Components)
            {
                result.AppendLine(ViewRecipeComponent(component));
            }

            return result.ToString();
        }

        private static string ViewGroupVariation(Recipe variation, int index)
        {
            int componentSum = variation.Components.Sum(x => x.Amount);

            var result = new StringBuilder();

            result.AppendLine($"> `{variation.Result.ItemId}#{index}` (or `{CraftHelper.GetInternalRecipeId(variation)}`)");
            result.AppendLine($"> **Variation {index + 1}** (**{componentSum}** {Format.TryPluralize("component", componentSum)})");

            foreach (RecipeComponent component in variation.Components)
            {
                result.AppendLine(ViewRecipeComponent(component));
            }

            return result.ToString();
        }

        // if (int ownedAmount) >= component.Amount => ✓, else •
        private static string ViewRecipeComponent(RecipeComponent component)
        {
            string icon = ItemHelper.IconOf(component.ItemId) ?? "•";
            string counter = component.Amount > 1 ? $" (x{component.Amount:##,0})" : "";
            return $"> `{component.ItemId}` {icon} **{ItemHelper.GetBaseName(component.ItemId)}**{counter}";
        }

        private static readonly int VariationPageLimit = 3;

        public static string ViewItemRecipes(ArcadeUser user, Item item, int page = 0)
        {
            IEnumerable<Recipe> knownVariations = CraftHelper.GetItemRecipes(user, item);

            if (!knownVariations.Any())
            {
                return Format.Warning("You do not know about any recipe variations for this item.");
            }

            var result = new StringBuilder();

            if (user.Config.Tooltips)
            {
                result.AppendLine(Format.Tooltip("Type `craft <recipe_id>` to craft the specified item with a specific variation."))
                    .AppendLine();
            }

            result.Append(ViewGroupVariations(item.Id, knownVariations, page));

            return result.ToString();
        }

        private static string ViewGroupVariations(string itemId, IEnumerable<Recipe> variations, int page = 0)
        {
            var result = new StringBuilder();

            int pageCount = Paginate.GetPageCount(variations.Count(), VariationPageLimit);
            page = Paginate.ClampIndex(page, pageCount);

            result.AppendLine($"> 📒 **Recipe: {ItemHelper.GetBaseName(itemId)}**{Format.PageCount(page + 1, pageCount, " ({0})", false)}");
            result.AppendLine($"> Known Variations: {variations.Count()}");
            result.AppendLine();

            int i = 0;
            foreach (Recipe variation in Paginate.GroupAt(variations, page, VariationPageLimit))
            {
                result.AppendLine(ViewGroupVariation(variation, i++));
            }

            return result.ToString();
        }

        // todo: add checkmarks next to the components of a recipe that a user owns
        public static string ViewRecipe(ArcadeUser user, Recipe recipe)
        {
            var info = new StringBuilder();

            bool craft = CraftHelper.CanCraft(user, recipe);

            // update recipe status before showing
            if (craft)
            {
                CraftHelper.SetRecipeStatus(user, recipe, RecipeStatus.Known);

                if (user.Config.Tooltips)
                {
                    info.AppendLine(Format.Tooltip("You meet the requirements to craft this recipe."))
                        .AppendLine();
                }
            }
            else if (CraftHelper.GetRecipeStatus(user, recipe) == RecipeStatus.Unknown)
                return Format.Warning("An unknown recipe was specified.");

            return ViewVariation(recipe);
        }

        // This could be scrapped soon
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
