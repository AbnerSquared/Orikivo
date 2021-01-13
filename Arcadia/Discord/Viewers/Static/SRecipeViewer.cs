using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;
using Orikivo.Text.Pagination;

namespace Arcadia.Services
{
    public class SRecipeViewer
    {

        private static readonly int VariationPageLimit = 3;
        private static readonly int GroupPageLimit = 5;

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

            int pageCount = Paginate.GetPageCount(knownGroups.Count(), GroupPageLimit);
            page = Paginate.ClampIndex(page, pageCount);

            string pageCounter = Format.PageCount(page + 1, pageCount, null, false);

            recipes.AppendLine(Locale.GetHeader(Headers.Recipe, pageCounter)).AppendLine();

            foreach (IGrouping<string, Recipe> group in Paginate.GroupAt(knownGroups, page, GroupPageLimit))
                recipes.AppendLine(PreviewRecipeGroup(group.Key, group));

            return recipes.ToString();
        }

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
                if (knownVariations.Any(x => CraftHelper.CanCraft(user, x)))
                {
                    result.AppendLine(Format.Tooltip("Recipes marked with ✨ meet the requirements to be crafted."))
                    .AppendLine();
                }
            }

            result.Append(ViewGroupVariations(user, item.Id, knownVariations, page));

            return result.ToString();
        }

        public static string ViewRecipe(ArcadeUser user, Recipe recipe)
        {
            var info = new StringBuilder();

            bool craft = CraftHelper.CanCraft(user, recipe);

            if (craft)
            {
                CraftHelper.SetRecipeStatus(user, recipe, RecipeStatus.Known);

                if (user.Config.Tooltips)
                {
                    info.AppendLine(Format.Tooltip($"Type `craft {CraftHelper.GetInternalRecipeId(recipe)}` to craft this recipe."))
                        .AppendLine();
                }
            }
            else if (CraftHelper.GetRecipeStatus(user, recipe) == RecipeStatus.Unknown)
                return Format.Warning("An unknown recipe was specified.");

            return ViewVariation(user, recipe);
        }

        public static string PreviewRecipeComponent(RecipeComponent component, int ownedAmount = 0)
        {
            return PreviewRecipeComponent(component.ItemId, component.Amount, ownedAmount);
        }

        public static string PreviewRecipeComponent(string itemId, int amount, int ownedAmount = 0)
        {
            string bullet = ownedAmount >= amount ? "✓ " : "";
            string icon = ItemHelper.GetIconOrDefault(itemId) ?? "•";
            string counter = amount > 1 ? $" (x{amount:##,0})" : "";
            return $"> {bullet}`{itemId}` {icon} **{ItemHelper.GetBaseName(itemId)}**{counter}";
        }

        private static IEnumerable<IGrouping<string, Recipe>> GroupItemRecipes(IEnumerable<Recipe> recipes)
        {
            return recipes.GroupBy(x => x.Result.ItemId);
        }

        private static string PreviewRecipeGroup(string itemId, IEnumerable<Recipe> recipes)
        {
            var result = new StringBuilder();

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

        private static string ViewVariation(ArcadeUser user, Recipe variation)
        {
            int componentSum = variation.Components.Sum(x => x.Amount);

            var result = new StringBuilder();

            bool canCraft = CraftHelper.CanCraft(user, variation);

            string craftIcon = canCraft ? "✨ " : "";

            result.AppendLine($"> `{CraftHelper.GetInternalRecipeId(variation)}`");
            result.AppendLine($"> {craftIcon}**{ItemHelper.GetBaseName(variation.Result.ItemId)}** (**{componentSum}** {Format.TryPluralize("component", componentSum)})");

            foreach (RecipeComponent component in variation.Components)
            {
                result.AppendLine(PreviewRecipeComponent(component, ItemHelper.GetOwnedAmount(user, component.ItemId)));
            }

            return result.ToString();
        }

        private static string ViewGroupVariations(ArcadeUser user, string itemId, IEnumerable<Recipe> variations, int page = 0)
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
                result.AppendLine(ViewGroupVariation(user, variation, i++));
            }

            return result.ToString();
        }

        private static string ViewGroupVariation(ArcadeUser user, Recipe variation, int index)
        {
            int componentSum = variation.Components.Sum(x => x.Amount);

            var result = new StringBuilder();

            result.AppendLine($"> `{variation.Result.ItemId}#{index}` (or `{CraftHelper.GetInternalRecipeId(variation)}`)");
            result.AppendLine($"> **Variation {index + 1}** (**{componentSum}** {Format.TryPluralize("component", componentSum)})");

            foreach (RecipeComponent component in variation.Components)
            {
                result.AppendLine(PreviewRecipeComponent(component, ItemHelper.GetOwnedAmount(user, component.ItemId)));
            }

            return result.ToString();
        }
    }
}
