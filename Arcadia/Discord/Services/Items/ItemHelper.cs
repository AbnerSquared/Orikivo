using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arcadia.Graphics;
using Arcadia.Services;
using Orikivo;

namespace Arcadia
{
    // TODO: Implement item attribute reading and data population
    public static class ItemHelper
    {
        public static readonly DateTime UniqueIdOffset = new DateTime(2020, 8, 20, 0, 0, 0, 0, DateTimeKind.Utc);

        // Assets.Items.Any
        // x => x.Id == itemId
        public static bool Exists(string itemId)
            => Assets.Items.Any(x => x.Id == itemId);

        // Check.NotNull(id)
        // Assets.Groups.Any
        // x => x.Id == id
        // Assets.Groups.Any
        // x => x.Icon?.Equals(id) ?? false
        public static bool GroupExists(string id)
            => Check.NotNull(id) && (Assets.Groups.Any(x => x.Id == id) || Assets.Groups.Any(x => x.Icon?.Equals(id) ?? false));

        // Check.NotNull(id)
        // Assets.Recipes.Any
        // x => CompareId(GetRecipeId(x), id)
        public static bool RecipeExists(string id)
            => Check.NotNull(id) && Assets.Recipes.Any(x => CompareRecipeId(GetRecipeId(x), id));

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

        public static string GetCatalogId(string itemId)
            => $"catalog:{itemId}";

        public static IEnumerable<Item> Search(string input)
            => Assets.Items.Where(x => MatchesAny(x, input));

        private static bool MatchesAny(Item item, string input)
        {
            if (item.Id == input)
                return true;

            if (item.Name.Contains(input, StringComparison.OrdinalIgnoreCase))
                return true;

            if (Check.NotNull(item.GroupId))
            {
                if (item.GroupId == input)
                    return true;

                var group = GetGroup(item.GroupId);

                if (group.Name.Equals(input, StringComparison.OrdinalIgnoreCase))
                    return true;

                if (Check.NotNull(group.Prefix) && group.Prefix.Contains(input, StringComparison.OrdinalIgnoreCase))
                    return true;

                if (group.Icon?.Equals(input) ?? false)
                    return true;
            }

            if (item.Rarity.ToString().Equals(input, StringComparison.OrdinalIgnoreCase))
                return true;

            if (item.Tag.GetFlags().Any(x => x.ToString().Equals(input, StringComparison.OrdinalIgnoreCase)))
                return true;

            if (input.Equals("ingredient", StringComparison.OrdinalIgnoreCase))
                return IsIngredient(item);

            if (input.Equals("craftable", StringComparison.OrdinalIgnoreCase))
                return CanCraft(item);

            if (input.Equals("sellable", StringComparison.OrdinalIgnoreCase))
                return item.CanSell;

            if (input.Equals("buyable", StringComparison.OrdinalIgnoreCase))
                return item.CanBuy;

            if (input.Equals("usable", StringComparison.OrdinalIgnoreCase))
                return item.Usage != null;

            if (input.Equals("tradable", StringComparison.OrdinalIgnoreCase))
                return item.TradeLimit != 0;

            if (input.Equals("unique", StringComparison.OrdinalIgnoreCase))
                return IsUnique(item);

            return false;
        }


        public static IEnumerable<Item> GetSeenItems(ArcadeUser user)
        {
            return user.Stats
                .Where(x => x.Key.StartsWith("catalog:") && x.Value == (long) CatalogStatus.Seen)
                .Select(x => GetItem(Var.GetKey(x.Key)));
        }

        public static IEnumerable<Item> GetSeenItems(ArcadeUser user, string itemGroupId)
        {
            return user.Stats
                .Where(x => x.Key.StartsWith("catalog:") && GroupOf(Var.GetKey(x.Key)) == itemGroupId &&
                            x.Value == (long)CatalogStatus.Seen)
                .Select(x => GetItem(Var.GetKey(x.Key)));
        }

        public static IEnumerable<Item> GetKnownItems(ArcadeUser user)
        {
            return user.Stats
                .Where(x => x.Key.StartsWith("catalog:") && x.Value == (long) CatalogStatus.Known)
                .Select(x => GetItem(Var.GetKey(x.Key)));
        }

        public static IEnumerable<Item> GetKnownItems(ArcadeUser user, string itemGroupId)
        {
            return user.Stats
                .Where(x => x.Key.StartsWith("catalog:") && GroupOf(Var.GetKey(x.Key)) == itemGroupId &&
                            x.Value == (long) CatalogStatus.Known)
                .Select(x => GetItem(Var.GetKey(x.Key)));
        }

        public static bool CanViewCatalog(ArcadeUser user)
        {
            return user.Stats.Any(x =>
                x.Key.StartsWith("catalog")
                && (x.Value == (long) CatalogStatus.Seen
                    || x.Value == (long) CatalogStatus.Known));
        }

        public static int GetKnownCount(ArcadeUser user)
        {
            return user.Stats
                .Count(x => x.Key.StartsWith("catalog:") && x.Value == (long) CatalogStatus.Known);
        }

        public static int GetKnownCount(ArcadeUser user, string itemGroup)
        {
            return user.Stats
                .Count(x => x.Key.StartsWith("catalog:") && GroupOf(Var.GetKey(x.Key)) == itemGroup &&
                            x.Value == (long) CatalogStatus.Known);
        }

        public static int GetVisibleCount(ArcadeUser user)
        {
            return user.Stats
                .Count(x => x.Key.StartsWith("catalog:") &&
                            x.Value >= (long)CatalogStatus.Seen);
        }

        public static int GetVisibleCount(ArcadeUser user, string itemGroup)
        {
            return user.Stats
                .Count(x =>
                x.Key.StartsWith("catalog:")
                && GroupOf(Var.GetKey(x.Key)) == itemGroup
                && x.Value >= (long)CatalogStatus.Seen);
        }

        public static int GetSeenCount(ArcadeUser user)
        {
            return user.Stats
                .Count(x => x.Key.StartsWith("catalog:") && x.Value == (long) CatalogStatus.Seen);
        }

        public static int GetSeenCount(ArcadeUser user, string itemGroup)
        {
            return user.Stats
                .Count(x => x.Key.StartsWith("catalog:") && GroupOf(Var.GetKey(x.Key)) == itemGroup &&
                            x.Value == (long) CatalogStatus.Seen);
        }

        public static Dictionary<string, int> GetMissingFromRecipe(ArcadeUser user, Recipe recipe)
        {
            if (recipe == null)
                throw new Exception("Could not find a recipe with the specified ID");

            var missing = new Dictionary<string, int>();

            foreach ((string itemId, int amount) in recipe.Components)
            {
                int owned = GetOwnedAmount(user, itemId);

                if (owned < amount)
                    missing[itemId] = amount - owned;
            }

            return missing;
        }

        public static CatalogStatus GetCatalogStatus(ArcadeUser user, string itemId)
        {
            if (GroupOf(itemId) == ItemGroups.Internal)
                return CatalogStatus.Unknown;

            long raw = user.GetVar(GetCatalogId(itemId));

            if (raw > (int)CatalogStatus.Known)
                return CatalogStatus.Known;

            return (CatalogStatus)raw;
        }

        public static CatalogStatus GetCatalogStatus(ArcadeUser user, Item item)
            => GetCatalogStatus(user, item.Id);

        public static long GetResearchTier(ArcadeUser user, Item item)
            => GetResearchTier(user, item.Id);

        public static long GetResearchTier(ArcadeUser user, string itemId)
        {
            return Math.Max(0, user.GetVar(GetCatalogId(itemId)) - 2);
        }

        public static void SetCatalogStatus(ArcadeUser user, string itemId, CatalogStatus status)
        {
            if (GroupOf(itemId) == ItemGroups.Internal)
                return;

            // If the user has already seen or known about this item, return;
            if (GetCatalogStatus(user, itemId) >= status)
                return;


            user.SetVar(GetCatalogId(itemId), (long) status);
        }

        public static void SetCatalogStatus(ArcadeUser user, Item item, CatalogStatus status)
            => SetCatalogStatus(user, item.Id, status);

        public static bool HasAttributes(Item item)
        {
            return item.CanBuy
                   || item.CanSell
                   || item.BypassCriteriaOnTrade
                   || item.OwnLimit.HasValue
                   || (item.TradeLimit > 0 || !item.TradeLimit.HasValue)
                   || IsUnique(item)
                   || HasUsageAttributes(item);
        }

        public static bool HasUsageAttributes(Item item)
        {
            if (item.Usage == null)
                return false;

            return item.Usage.Durability.HasValue
                   || item.Usage.Cooldown.HasValue
                   || item.Usage.Expiry.HasValue;
        }

        public static IEnumerable<Recipe> GetKnownRecipes(ArcadeUser user)
        {
            return Assets.Recipes.Where(x => GetRecipeStatus(user, x) != RecipeStatus.Unknown);
        }

        // This just reads the boost multiplier
        public static float GetBoostMultiplier(ArcadeUser user, BoostType type)
        {
            float rate = 1f;
            var toRemove = new List<BoostData>();
            foreach (BoostData booster in user.Boosters)
            {
                if (!CanApplyBooster(booster))
                    toRemove.Add(booster);

                if (booster.Type != type)
                    continue;

                rate += booster.Rate;

                if (booster.UsesLeft.HasValue)
                {
                    if (booster.UsesLeft <= 0)
                        toRemove.Add(booster);
                }
            }

            user.Boosters.RemoveAll(x => toRemove.Contains(x));
            return rate < 0 ? 0 : rate;
        }

        public static DateTime? GetLastUsed(ArcadeUser user, string itemId, string uniqueId = null)
        {
            Item item = GetItem(itemId);

            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (item.Usage == null)
                throw new Exception("Expected item usage to be specified but returned null");

            string statGroup = item.Usage?.CooldownMode switch
            {
                CooldownMode.Instance => uniqueId ?? throw new Exception("Expected a unique item reference but was unspecified"),
                CooldownMode.Item => itemId,
                CooldownMode.Group => item.GroupId ?? throw new Exception("Expected an item group but was empty"),
                CooldownMode.Global => "global",
                _ => null
            };

            if (statGroup == null)
                return null;

            if (statGroup == uniqueId)
            {
                ItemData data = GetFromInventory(user, itemId, uniqueId);
                return data.Data.LastUsed;
            }

            var ticks = user.GetVar(GetCooldownId(statGroup));

            if (ticks == 0)
                return null;

            return new DateTime(ticks);
        }

        public static TimeSpan? GetCooldownRemainder(ArcadeUser user, string itemId, string uniqueId = null)
        {
            Item item = GetItem(itemId);

            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (item.Usage != null)
            {
                DateTime? lastUsed = GetLastUsed(user, itemId, uniqueId);

                if (item.Usage.Cooldown.HasValue && lastUsed.HasValue)
                {
                    return DateTime.UtcNow - lastUsed.Value.Add(item.Usage.Cooldown.Value);
                }
            }

            return null;
        }

        public static string GetBaseName(string itemId)
            => GetItem(itemId)?.Name ?? itemId;

        public static ItemTag GetTag(string itemId)
            => GetItem(itemId)?.Tag ?? 0;

        public static long BoostValue(ArcadeUser user, long value, BoostType type, bool isNegative = false)
        {
            float rate = 1;

            if (user.Boosters.Count == 0)
                return value;

            var toRemove = new List<BoostData>();
            foreach (BoostData booster in user.Boosters)
            {
                if (!CanApplyBooster(booster))
                    toRemove.Add(booster);

                if (booster.Type != type)
                    continue;

                rate += booster.Rate;

                if (booster.UsesLeft.HasValue)
                {
                    if (--booster.UsesLeft <= 0)
                        toRemove.Add(booster);
                }
            }

            foreach (BoostData booster in toRemove)
            {
                user.Boosters.Remove(booster);

                if (Check.NotNull(booster.ParentId))
                {
                    Item parent = GetItem(booster.ParentId);
                    parent?.Usage?.OnBreak?.Invoke(user);
                }
            }

            return BoostConvert.GetValue(value, rate, isNegative);
        }

        

        

        public static IEnumerable<Recipe> RecipesFor(string itemId)
            => RecipesFor(GetItem(itemId));

        public static IEnumerable<Recipe> RecipesFor(Item item)
        {
            return Assets.Recipes.Where(x => x.Result.ItemId == item.Id);
        }

        

        public static long CreateUniqueId()
            => (DateTime.UtcNow - UniqueIdOffset).Ticks;

        public static IEnumerable<Item> GetItemsInGroup(string groupId)
        {
            return Assets.Items.Where(x => x.GroupId == groupId);
        }

        public static ItemGroup GetGroup(string id)
        {
            if (Assets.Groups.Count(x => x.Id == id) > 1)
                throw new ArgumentException("There are more than one groups with the specified ID.");

            return Assets.Groups.FirstOrDefault(x => x.Id == id);
        }

        public static Recipe GetRecipe(string id)
        {
            if (Assets.Recipes.Count(x => CompareRecipeId(GetRecipeId(x), id)) > 1)
                throw new ArgumentException("There are more than one recipes with the specified ID.");

            return Assets.Recipes.FirstOrDefault(x => CompareRecipeId(GetRecipeId(x), id));
        }

        public static RecipeStatus GetRecipeStatus(ArcadeUser user, Recipe recipe)
        {
            return (RecipeStatus) user.GetVar(GetRecipeId(recipe));
        }

        public static ItemData GetBestStack(ArcadeUser user, Item item)
            => user.Items.FirstOrDefault(x => CanStack(x) && x.Id == item.Id);

        public static ItemData AddOrSetToStack(ArcadeUser user, Item item, int amount)
        {
            ItemData stack = GetBestStack(user, item);

            if (stack != null)
            {
                stack.StackCount += amount;
                return stack;
            }

            stack = new ItemData(item.Id, amount);
            user.Items.Add(stack);

            return stack;
        }

        public static IEnumerable<ItemData> GetFromInventory(ArcadeUser user, string itemId)
            => user.Items.Where(x => x.Id == itemId);

        public static ItemData GetFromInventory(ArcadeUser user, string itemId, string uniqueId)
            => user.Items.First(x => x.Id == itemId && x.Data != null && x.Data.Id == uniqueId);

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

            user.SetVar(GetRecipeId(recipe), (long) status);
        }

        public static bool KnowsRecipe(ArcadeUser user, string recipeId)
            => KnowsRecipe(user, GetRecipe(recipeId));

        public static bool KnowsRecipe(ArcadeUser user, Recipe recipe)
        {
            return user.GetVar(GetRecipeId(recipe)) == (long) RecipeStatus.Known;
        }

        public static bool TryGetRecipe(string id, out Recipe recipe)
        {
            recipe = null;

            if (!RecipeExists(id))
                return false;

            recipe = GetRecipe(id);
            return true;
        }

        public static bool TryGetGroup(string id, out ItemGroup group)
        {
            group = null;

            if (!GroupExists(id))
                return false;

            group = GetGroup(id);
            return true;
        }

        public static bool TryGetItem(string id, out Item item)
        {
            item = null;

            if (!Exists(id))
                return false;

            item = GetItem(id);
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
                TakeItem(user, itemId, amount);

            GiveItem(user, recipe.Result.ItemId, recipe.Result.Amount);

            user.AddToVar(Stats.TimesCrafted);
            return true;
        }

        

        

        public static ItemData SetInStack(ArcadeUser user, Item item, int amount = 1)
        {
            if (amount <= 0)
                throw new Exception("Expected item amount to be larger than 0");

            ItemData best = user.Items.FirstOrDefault(CanStack);

            if (best != null)
            {
                best.StackCount += amount;
                return best;
            }

            // If no available stacks are found, a new stack is made instead.
            best = new ItemData(item.Id, amount);
            user.Items.Add(best);

            return best;
        }

        

        

        public static ItemData GetItemData(ArcadeUser user, string dataId)
        {
            if (!Check.NotNull(dataId))
                return null;

            bool isUniqueId = user.Items.Any(x => x.Seal == null && x.Data?.Id == dataId);

            if (!isUniqueId)
            {
                if (user.Items.Any(x => x.TempId == dataId))
                    return user.Items.First(x => x.TempId == dataId);

                if (Exists(dataId))
                    return GetBestStack(user, GetItem(dataId));
            }


            if (!Exists(dataId) && !isUniqueId)
            {
                return null;
            }

            string itemId = isUniqueId ? ItemOf(user, dataId).Id : dataId;
            string uniqueId = isUniqueId ? dataId : null;

            bool isUnique = IsUnique(itemId);

            if (isUnique && !isUniqueId)
            {
                string uId = GetBestUniqueId(user, itemId);

                if (!Check.NotNull(uId))
                    return null;

                return DataOf(user, itemId, uId);
            }

            return DataOf(user, itemId, uniqueId);
        }

        public static bool RemovePalette(ArcadeUser user)
        {
            if (user.Card.Palette == PaletteType.Default)
                return false;

            GiveItem(user, IdFor(user.Card.Palette.Primary, user.Card.Palette.Secondary));
            user.Card.Palette = PaletteType.Default;
            return true;
        }

        

        

        public static bool IsUnique(string itemId)
            => IsUnique(GetItem(itemId));

        public static bool IsUnique(Item item)
        {
            if (item.TradeLimit.HasValue)
                if (item.TradeLimit > 0)
                    return true;

            // is there any form of usage specified?
            if (item.Usage == null)
                return false;

            // can it be used more than once?
            if (item.Usage.Durability > 1)
                return true;

            // is this item left behind when the durability is broken?
            if (item.Usage.Durability.HasValue && !item.Usage.DeleteMode.HasFlag(DeleteMode.Break))
                return true;

            // does this item have a cooldown that is applied to the instance of an item?
            if (item.Usage.Cooldown.HasValue && item.Usage.CooldownMode == CooldownMode.Instance)
                return true;

            return false;
        }

        public static bool IsIngredient(Item item)
            => Assets.Recipes.Any(x => x.Components.Any(c => c.ItemId == item.Id));

        public static Item GetItem(string id)
        {
            if (Assets.Items.Count(x => x.Id == id) > 1)
                throw new ArgumentException("There are more than one items with the specified ID.");

            return Assets.Items.FirstOrDefault(x => x.Id == id);
        }

        

        public static string NameFor(PaletteType palette, PaletteType? secondary = null)
            => GetItem(IdFor(palette, secondary)).Name;

        public static string IdFor(PaletteType palette, PaletteType? secondary = null)
        {
            return palette switch
            {
                PaletteType.GammaGreen => Items.PaletteGammaGreen,

                PaletteType.Crimson => Items.PaletteCrimson,

                PaletteType.Glass when secondary == PaletteType.Wumpite => Items.PaletteGlossyWumpite,
                PaletteType.Glass => Items.PaletteGlass,

                PaletteType.Wumpite when secondary == PaletteType.Glass => Items.PaletteGlossyWumpite,
                PaletteType.Wumpite => Items.PaletteWumpite,
                _ => null
            };
        }

        public static string NameOf(string id)
        {
            if (!TryGetItem(id, out Item item))
                return id;

            if (TryGetGroup(item.GroupId, out ItemGroup group))
                return $"{group.Prefix}{item.Name}";

            return item.Name;
        }

        public static string GroupOf(string id)
        {
            if (!TryGetItem(id, out Item item))
                return null;

            return item.GroupId;
        }

        public static string DetailsOf(Item item)
            => Catalog.ViewItem(item);

        public static string IconOf(string itemId)
            => GetItem(itemId)?.GetIcon() ?? "";

        public static long SizeOf(string itemId)
            => GetItem(itemId)?.Size ?? 0;

        public static Item ItemOf(ArcadeUser user, string uniqueId)
        {
            if (user.Items.Any(x => x.Data?.Id == uniqueId))
                return null;

            return GetItem(user.Items.FirstOrDefault(x => x.Data?.Id == uniqueId)?.Id);
        }

        public static ItemData DataOf(ArcadeUser user, string itemId, string uniqueId = null)
        {
            return user.Items.FirstOrDefault(x => (!string.IsNullOrWhiteSpace(uniqueId)
                ? x.Data?.Id == uniqueId
                : x.Id == itemId) && x.Seal == null);
        }

        public static ItemData DataOf(ArcadeUser user, Item item, string uniqueId = null)
            => DataOf(user, item.Id, uniqueId);

        

        public static void TakeItem(ArcadeUser user, string itemId, int amount = 1)
            => TakeItem(user, GetItem(itemId), amount);

        public static void TakeItem(ArcadeUser user, Item item, int amount = 1)
        {
            if (!HasItem(user, item.Id))
                return;

            if (IsUnique(item))
            {
                int available = user.Items.Count(x => x.Id == item.Id);

                if (available < amount)
                    amount = available;

                for (int i = 0; i < amount; i++)
                {
                    string uniqueId = GetBestUniqueId(user, item.Id);

                    if (uniqueId == null)
                        throw new Exception("Expected to find an item ID but returned null");

                    TakeItem(user, item, uniqueId);
                }
                return;
            }

            if (GetOwnedAmount(user, item) - amount <= 0)
            {
                ItemData slot = user.Items.First(x => x.Id == item.Id);
                user.Items.Remove(slot);
            }
            else
            {
                user.Items.First(x => x.Id == item.Id).StackCount -= amount;
            }
        }

        

        

        public static void TakeItem(ArcadeUser user, Item item, string uniqueId)
        {
            if (!HasItem(user, item.Id))
                return;

            // This method only works for unique items.
            if (!IsUnique(item))
                return;

            ItemData match = user.Items.FirstOrDefault(x => x.Id == item.Id && x.Data.Id == uniqueId);

            if (match == null)
                throw new Exception("Could not find a unique item with the specified ID.");

            user.Items.Remove(match);
        }

        

        public static int GetOwnedAmount(ArcadeUser user, string itemId)
            => GetOwnedAmount(user, GetItem(itemId));

        public static int GetOwnedAmount(ArcadeUser user, Item item)
        {
            if (!HasItem(user, item.Id))
                return 0;

            if (IsUnique(item))
                return user.Items.Count(x => x.Id == item.Id);

            return user.Items.First(x => x.Id == item.Id).Count;
        }

        

        // this stores an item to the specified user.
        

        

        public static bool CanApplyBooster(BoostData booster)
        {
            if (booster.ExpiresOn.HasValue)
            {
                if ((DateTime.UtcNow - booster.ExpiresOn.Value) > TimeSpan.Zero)
                    return false;
            }

            if (booster.UsesLeft.HasValue)
            {
                if (booster.UsesLeft <= 0)
                    return false;
            }

            return true;
        }

        public static bool CanStack(ItemData data)
            => (data.Seal == null) & (data.Data == null);

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
                if (!HasItem(user, itemId) || GetOwnedAmount(user, itemId) != amount)
                    return false;
            }

            return true;
        }

        public static bool CanGift(string itemId, ItemData data = null)
            => CanGift(GetItem(itemId), data);

        public static bool CanGift(Item item, ItemData data = null)
        {
            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (data?.Seal != null)
            {
                if (data.Seal.SenderId.HasValue || Check.NotNullOrEmpty(data.Seal.ToUnlock))
                    return false;

                return true;
            }

            return !item.TradeLimit.HasValue || (data?.Data?.TradeCount ?? 0) < item.TradeLimit.Value;
        }

        public static bool CanTrade(ItemData data)
            => CanTrade(GetItem(data.Id), data);

        public static bool CanTrade(string itemId, ItemData data = null)
            => CanTrade(GetItem(itemId), data);

        public static bool CanTrade(Item item, ItemData data = null)
        {
            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (data?.Seal != null)
            {
                return false;
            }

            return !item.TradeLimit.HasValue || (data?.Data?.TradeCount ?? 0) < item.TradeLimit.Value;
        }

        public static bool CanUse(ArcadeUser user, string itemId, ItemData data = null)
            => CanUse(user, GetItem(itemId), data);

        public static bool CanUse(ArcadeUser user, Item item, ItemData data = null)
        {
            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (item.Usage == null)
                return false;

            DateTime? lastUsed = GetLastUsed(user, item.Id);

            if (lastUsed.HasValue && item.Usage.Cooldown.HasValue)
            {
                // if the cooldown has expired, allow use.
                if (DateTime.UtcNow - lastUsed.Value.Add(item.Usage.Cooldown.Value) >= TimeSpan.Zero)
                    return true;

                return false;
            }

            if (data?.Data != null)
            {
                if (data.Data.ExpiresOn.HasValue)
                    if (DateTime.UtcNow >= data.Data.ExpiresOn.Value)
                        return false;

                if (data.Data.Durability <= 0)
                    return false;
            }

            return true;
        }

        public static bool HasItem(ArcadeUser user, string itemId)
            => user.Items.Any(x => x.Id == itemId);

        public static bool HasItem(ArcadeUser user, Item item)
            => HasItem(user, item.Id);

        public static bool HasItem(ArcadeUser user, Item item, string uniqueId)
            => HasItem(user, item.Id, uniqueId);

        public static bool HasItem(ArcadeUser user, string itemId, string uniqueId)
            => user.Items.Any(x => x.Id == itemId && x.Data != null && x.Data.Id == uniqueId);

        public static bool HasItemWhen(ArcadeUser user, string itemId, Func<ItemData, bool> criterion)
            => HasItem(user, itemId) && user.Items.Any(criterion);

        public static bool HasItemWhen(ArcadeUser user, Item item, Func<ItemData, bool> criterion)
            => HasItem(user, item) && user.Items.Any(criterion);

        

        public static UsageResult UseItem(ArcadeUser user, ItemData data, string input = null)
        {
            var isBroken = false;
            Item item = GetItem(data.Id);

            // If a usage isn't specified
            if (item.Usage == null)
                return UsageResult.FromError(Format.Warning("This item does not have functionality."));

            // If the item is sealed
            if (data.Seal != null)
            {
                // If the item has requirements to unlock
                if (Check.NotNullOrEmpty(data.Seal.ToUnlock))
                {
                    foreach ((string id, VarProgress progress) in data.Seal.ToUnlock)
                        if (progress.Current < progress.Required)
                            return UsageResult.FromError(Format.Warning("You have not met all of the criteria needed to open this seal."));
                }

                ItemData stack = GetBestStack(user, item);

                if (stack != null && !IsUnique(item))
                {
                    stack.StackCount += data.StackCount;
                    user.Items.Remove(data);
                }
                else
                {
                    data.Seal = null;
                }

                return UsageResult.FromSuccess($"> You have released the seal to discover:\n{IconOf(data.Id) ?? "•"} **{GetBaseName(data.Id)}**");
            }

            // If the item has unique properties
            if (data.Data != null)
            {
                // If the item has a cooldown
                TimeSpan? remainder = GetCooldownRemainder(user, item.Id, data.Data.Id);

                if (remainder.HasValue)
                    return UsageResult.FromError(
                        $"This item is on cooldown and can be used in {Format.Counter(remainder.Value.TotalSeconds)}.");

                // If the item has expired
                if (data.Data.ExpiresOn.HasValue)
                    if (DateTime.UtcNow >= data.Data.ExpiresOn.Value)
                        return UsageResult.FromError(Format.Warning("This item has expired and cannot be used."));

                // If the item is broken
                if (data.Data.Durability <= 0)
                {
                    if (item.Usage.DeleteMode.HasFlag(DeleteMode.Break))
                    {
                        user.Items.Remove(data);
                        return UsageResult.FromError(Format.Warning("This item is broken and will be removed."));
                    }

                    return UsageResult.FromError(Format.Warning("This item is broken and cannot be used."));
                }
            }

            // Attempt to use the item
            UsageResult result = item.Usage.Action(new UsageContext(user, input, data));

            // If the item failed on use, return its result
            if (!result.IsSuccess)
                return result;

            // Apply durability effect
            if (item.Usage.Durability.HasValue)
            {
                if (data.Data != null)
                {
                    if (item.Usage.DeleteMode.HasFlag(DeleteMode.Break))
                    {
                        // if it's going to be deleted on use, remove it
                        if (data.Data.Durability - 1 <= 0)
                            TakeItem(user, item, data.Data.Id);
                    }

                    if (data.Data.Durability > 0)
                        data.Data.Durability--; // remove 1 from the durability.

                    if (data.Data.Durability <= 0)
                        isBroken = true;
                }
                else
                {
                    if (item.Usage.Durability == 1)
                    {
                        if (item.Usage.DeleteMode.HasFlag(DeleteMode.Break))
                        {
                            data.StackCount -= 1;

                            if (data.StackCount <= 0)
                                user.Items.Remove(data);
                        }

                        isBroken = true;
                    }
                }
            }

            // Apply item cooldowns
            if (item.Usage.Cooldown.HasValue)
            {
                if (item.Usage.CooldownMode == CooldownMode.Instance)
                {
                    if (data.Data == null)
                        throw new Exception("Expected an item to be unique");

                    data.Data.LastUsed = DateTime.UtcNow;
                }
                else if (item.Usage.CooldownMode == CooldownMode.Group)
                {
                    user.SetVar($"{item.GroupId}:last_used", DateTime.UtcNow.Ticks);
                }
                else if (item.Usage.CooldownMode == CooldownMode.Item)
                {
                    // finally, if all of the checks pass, use up the item.
                    user.SetVar(GetCooldownId(item.Id), DateTime.UtcNow.Ticks);
                }
                else
                {
                    user.SetVar("cooldown:item", DateTime.UtcNow.Ticks);
                }
            }

            // Apply break mechanics
            if (isBroken && item.GroupId != ItemGroups.Booster)
            {
                // Only invoke breaking if the group is not a booster
                // This is because boosters reference this method when they are used up
                item.Usage.OnBreak?.Invoke(user);
                user.AddToVar(Stats.ItemsUsed);
            }

            return result;
        }

        public static UsageResult UseItem(ArcadeUser user, Item item, string uniqueId, string input = null)
            => UseItem(user, DataOf(user, item.Id, uniqueId), input);

        public static UsageResult UseItem(ArcadeUser user, string itemId, string uniqueId, string input)
            => UseItem(user, DataOf(user, itemId, uniqueId), input);

        public static UsageResult UseItem(ArcadeUser user, string dataId, string input = null)
            => UseItem(user, GetItemData(user, dataId), input);

        public static void GiveItem(ArcadeUser user, string itemId, int amount = 1)
            => GiveItem(user, GetItem(itemId), amount);

        public static void GiveItem(ArcadeUser user, Item item, int amount = 1)
        {
            int ownedAmount = GetOwnedAmount(user, item);

            if (item.OwnLimit.HasValue)
            {
                if (ownedAmount >= item.OwnLimit)
                {
                    return; // don't give them the item.
                }
            }

            AddItem(user, item.Id, amount);
            SetCatalogStatus(user, item, CatalogStatus.Known);
            // If they somehow went over, this will fix that
            /*
            if (currentAmount > item.OwnLimit)
            {
                user.Items[item.Id].StackCount = item.OwnLimit;
                currentAmount = item.OwnLimit.Value;
            }
            */
        }

        internal static void AddItem(ArcadeUser user, string itemId, int amount = 1)
            => AddItem(user, GetItem(itemId), amount);

        internal static void AddItem(ArcadeUser user, Item item, int amount = 1)
        {
            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (IsUnique(item))
            {
                for (int i = 0; i < amount; i++)
                {
                    user.Items.Add(new ItemData(item.Id, CreateUniqueData(item)));
                }
            }
            else
                AddOrSetToStack(user, item, amount);
        }

        private static UniqueItemData CreateUniqueData(Item item)
        {
            if (!IsUnique(item))
                throw new Exception("The specified item is not unique.");

            var data = new UniqueItemData
            {
                Durability = item.Usage?.Durability,
                // ExpiresOn = (item.Usage?.ExpiryTrigger ?? 0)
                ExpiresOn = (item.Usage?.ExpiryTrigger ?? 0) == ExpiryTrigger.Own && (item.Usage?.Expiry.HasValue ?? false)
                ? DateTime.UtcNow.Add(item.Usage.Expiry.Value)
                : (DateTime?)null
            };

            // No need to initialize if the item cannot be traded
            if (item.TradeLimit.HasValue && item.TradeLimit > 0)
                data.TradeCount = 0;

            return data;
        }

        private static DateTime? GetExpiry(TimeSpan duration, ExpiryTrigger trigger)
        {
            if (trigger == ExpiryTrigger.Own)
                return DateTime.UtcNow.Add(duration);

            return null;
        }

        private static string GetBestUniqueId(ArcadeUser user, string itemId)
            => GetBestUniqueId(user, GetItem(itemId));

        private static string GetBestUniqueId(ArcadeUser user, Item item)
        {
            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (!IsUnique(item))
                throw new ArgumentException("The specified item is not unique.");

            // Slim down to all matching item entries of the same item
            IEnumerable<ItemData> matching = user.Items.Where(x => x.Id == item.Id && !x.Locked);

            if (user.Items.All(x => x.Id != item.Id))
                return null;

            if (item.Usage?.Durability != null)
                matching = matching.OrderBy(x => x.Data.Durability);

            return matching.First().Id;
        }

        private static bool CompareRecipeId(string id, string input)
            => id == input || id[7..] == input;

        private static string GetComponentId(RecipeComponent component)
            => $"{component.ItemId}{(component.Amount > 1 ? $"#{component.Amount}" : "")}";

        private static string GetCooldownId(string itemId)
            => $"{itemId}:last_used";
    }
}
