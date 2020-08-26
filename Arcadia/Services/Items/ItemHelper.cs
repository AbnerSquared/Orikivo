using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arcadia.Graphics;
using Arcadia.Services;
using Orikivo;
using PaletteType = Arcadia.Graphics.PaletteType;

namespace Arcadia
{
    public enum RecipeStatus
    {
        Unknown = 0,
        Known = 1
    }

    public enum CatalogStatus
    {
        Unknown = 0,
        Seen = 1,
        Known = 2
    }

    // TODO: Implement item attribute reading and data population
    public static class ItemHelper
    {
        public static long SizeOf(string itemId)
            => GetItem(itemId)?.Size ?? 0;
        public static string GetRecipeId(Recipe recipe)
        {
            if (recipe.Components.Count == 0)
                throw new Exception("Expected at least one recipe component");

            var id = new StringBuilder();

            id.Append($"recipe:{recipe.Result.ItemId}/");
            id.AppendJoin('.', recipe.Components.Select(GetComponentId));

            return id.ToString();
        }

        private static string GetComponentId(RecipeComponent component)
            => $"{component.ItemId}{(component.Amount > 1 ? $"#{component.Amount}" : "")}";

        public static IEnumerable<Item> GetSeenItems(ArcadeUser user)
        {
            return user.Stats
                .Where(x => x.Key.StartsWith("catalog:") && x.Value == (long) CatalogStatus.Seen)
                .Select(x => GetItem(Var.GetKey(x.Key)));
        }

        public static IEnumerable<Item> GetKnownItems(ArcadeUser user)
        {
            return user.Stats
                .Where(x => x.Key.StartsWith("catalog:") && x.Value == (long) CatalogStatus.Known)
                .Select(x => GetItem(Var.GetKey(x.Key)));
        }

        public static IEnumerable<Item> GetSeenItems(ArcadeUser user, string itemGroupId)
        {
            return user.Stats
                .Where(x => x.Key.StartsWith("catalog:") && GroupOf(Var.GetKey(x.Key)) == itemGroupId &&
                            x.Value == (long) CatalogStatus.Seen)
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

        private static string GetCatalogId(string itemId)
            => $"catalog:{itemId}";

        public static CatalogStatus GetCatalogStatus(ArcadeUser user, string itemId)
        {
            if (GroupOf(itemId) == ItemGroups.Internal)
                return CatalogStatus.Unknown;

            return (CatalogStatus) user.GetVar(GetCatalogId(itemId));
        }

        public static CatalogStatus GetCatalogStatus(ArcadeUser user, Item item)
            => GetCatalogStatus(user, item.Id);

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

        public static bool HasAttributes(Item item)
        {
            return item.CanBuy
                   | item.CanSell
                   | item.BypassCriteriaOnTrade
                   | item.OwnLimit.HasValue
                   | (item.TradeLimit > 0 || !item.TradeLimit.HasValue)
                   | IsUnique(item)
                   | HasUsageAttributes(item);
        }

        public static bool HasUsageAttributes(Item item)
        {
            if (item.Usage == null)
                return false;

            return item.Usage.Durability.HasValue
                   | item.Usage.Cooldown.HasValue
                   | item.Usage.Expiry.HasValue;
        }

        public static readonly List<Recipe> LRecipes = new List<Recipe>
        {
            new Recipe
            {
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Items.PaletteGlass, 1),
                    new RecipeComponent(Items.PaletteGammaGreen, 1)
                },
                Result = new RecipeComponent(Items.PaletteGlossyGreen, 1)
            },
            new Recipe
            {
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Items.PaletteGlass, 1),
                    new RecipeComponent(Items.PaletteWumpite, 1)
                },
                Result = new RecipeComponent(Items.PaletteGlossyWumpite, 1)
            }
        };

        public static IEnumerable<Recipe> GetKnownRecipes(ArcadeUser user)
        {
            return LRecipes.Where(x => GetRecipeStatus(user, x) != RecipeStatus.Unknown);
        }

        public static string IconOf(string itemId)
        {
            Item item = GetItem(itemId);

            if (item == null)
                return "";

            return item.GetIcon();
        }

        public static bool CanApplyBooster(BoosterData booster)
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

        // This just reads the boost multiplier
        public static float GetBoostMultiplier(ArcadeUser user, BoosterType type)
        {
            float rate = 1f;
            var toRemove = new List<BoosterData>();
            foreach (BoosterData booster in user.Boosters)
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

        public static long BoostValue(ArcadeUser user, long value, BoosterType type, bool isNegative = false)
        {
            float rate = 1;

            if (user.Boosters.Count == 0)
                return value;

            var toRemove = new List<BoosterData>();
            foreach (BoosterData booster in user.Boosters)
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

            foreach (BoosterData booster in toRemove)
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

        public static bool IsIngredient(Item item)
            => LRecipes.Any(x => x.Components.Any(c => c.ItemId == item.Id));

        public static bool CanCraft(Item item)
            => LRecipes.Any(x => x.Result.ItemId == item.Id);

        public static IEnumerable<Recipe> RecipesFor(string itemId)
            => RecipesFor(GetItem(itemId));

        public static IEnumerable<Recipe> RecipesFor(Item item)
        {
            return LRecipes.Where(x => x.Result.ItemId == item.Id);
        }

        public static readonly List<ItemGroup> Groups = new List<ItemGroup>
        {
            new ItemGroup
            {
                Id = "$",
                Name = "Internal",
                Summary = "Items that do not exist as an actual obtainable item."
            },
            new ItemGroup
            {
                Id = "booster",
                Icon = Icons.Booster,
                Name = "Booster",
                Prefix = "Booster: ",
                Summary = "Modifies the multiplier for a specified form of income."
            },
            new ItemGroup
            {
                Id = "palette",
                Icon = Icons.Palette,
                Name = "Palette",
                Prefix = "Card Palette: ",
                Summary = "Modifies the color scheme that is displayed on a card."
            },
            new ItemGroup
            {
                Id = "summon",
                Icon = Icons.Summon,
                Name = "Summon",
                Prefix = "Summon: "
            },
            new ItemGroup
            {
                Id = "automaton",
                Name = "Automaton",
                Prefix = "Automaton: ",
                Summary = "Grants the ability to automate specific actions."
            }
        };

        public static readonly DateTime UniqueIdOffset = new DateTime(2020, 8, 20, 0, 0, 0, 0, DateTimeKind.Utc);

        public static long CreateUniqueId()
            => (DateTime.UtcNow - UniqueIdOffset).Ticks;

        public static IEnumerable<Item> GetItemsInGroup(string groupId)
        {
            return LItems.Where(x => x.GroupId == groupId);
        }

        public static ItemGroup GetGroup(string id)
        {
            if (Groups.Count(x => x.Id == id) > 1)
                throw new ArgumentException("There are more than one groups with the specified ID.");

            return Groups.FirstOrDefault(x => x.Id == id);
        }

        public static Recipe GetRecipe(string id)
        {
            if (LRecipes.Count(x => GetRecipeId(x) == id) > 1)
                throw new ArgumentException("There are more than one recipes with the specified ID.");

            return LRecipes.FirstOrDefault(x => GetRecipeId(x) == id);
        }

        public static RecipeStatus GetRecipeStatus(ArcadeUser user, Recipe recipe)
        {
            return (RecipeStatus) user.GetVar(GetRecipeId(recipe));
        }

        public static void UpdateKnownRecipes(ArcadeUser user)
        {
            foreach (Recipe recipe in LRecipes.Where(x => CanCraft(user, x)))
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

        public static bool RecipeExists(string id)
            => Check.NotNull(id) && LRecipes.Any(x => GetRecipeId(x) == id);

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

        public static string DetailsOf(Item item)
            => Catalog.ViewItem(item);

        private static bool CanStack(ItemData data)
            => (data.Seal == null) & (data.Data == null);

        public static ItemData GetBestStack(ArcadeUser user, Item item)
            => user.Items.FirstOrDefault(x => CanStack(x) && x.Id == item.Id);

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

        public static string GetBaseName(string itemId)
            => GetItem(itemId)?.Name ?? itemId;

        public static Item ItemOf(ArcadeUser user, string uniqueId)
        {
            if (user.Items.Any(x => x.Data?.Id == uniqueId))
                return null;

            return GetItem(user.Items.FirstOrDefault(x => x.Data?.Id == uniqueId)?.Id);
        }

        public static ItemData GetItemData(ArcadeUser user, string dataId)
        {
            if (!Check.NotNull(dataId))
                return null;

            bool isUniqueId = user.Items.Any(x => x.Data?.Id == dataId);

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

        public static List<Item> LItems =>
            new List<Item>
            {
                new Item
                {
                    Id = "$_seal",
                    GroupId = "$",
                    Name = "Sealed Item",
                    Quotes = new List<string>
                    {
                        "A mysterious container with unknown contents."
                    }
                },
                new Item
                {
                    Id = "au_gimi",
                    Name = "Gimi",
                    Summary = "Gives you the ability to execute the Gimi command a specified number of times. Please note that you are required to wait a second on each execution.",
                    Quotes = new List<string>
                    {
                        "It echoes a tick loud enough to break the sound barrier."
                    },
                    GroupId = ItemGroups.Automaton,
                    Rarity = ItemRarity.Myth,
                    Tag = ItemTag.Tool | ItemTag.Automaton,
                    Value = 100000,
                    DeniedHandles = ItemDeny.Buy | ItemDeny.Sell,
                    TradeLimit = 0,
                    Size = 1000,
                    OwnLimit = 1,
                    MarketCriteria = null,
                    ToOwn = null
                },
                new Item
                {
                    Id = "t_gw",
                    Name = "Gift Wrap",
                    Quotes = new List<string>
                    {
                        "Add a little mystery to your gifting!"
                    },
                    Summary = "Wraps an item to seal its contents.",
                    Rarity = ItemRarity.Common,
                    Tag = ItemTag.Tool,
                    Value = 25,
                    BypassCriteriaOnTrade = true,
                    Size = 10,
                    Usage = new ItemUsage
                    {
                        Durability = 1,
                        DeleteMode = DeleteMode.Break,
                        Action = delegate(UsageContext ctx)
                        {
                            if (!Check.NotNull(ctx.Input))
                                return UsageResult.FromError("An item data reference must be specified.");

                            var reader = new StringReader(ctx.Input);

                            if (!reader.CanRead())
                                return UsageResult.FromError("An item data reference must be specified.");

                            string id = reader.ReadUnquotedString();

                            // The amount to wrap. If unspecified, the default is 1.
                            int amount = 1;

                            if (reader.CanRead())
                            {
                                reader.SkipWhiteSpace();
                                int.TryParse(reader.ReadUnquotedString(), out amount);
                            }

                            bool isUniqueId = ctx.User.Items.Any(x => x.Data?.Id == id);

                            if (!Exists(id) && !isUniqueId)
                            {
                                return UsageResult.FromError("Unknown data reference specified.");
                            }

                            string itemId = isUniqueId ? ItemOf(ctx.User, id).Id : id;
                            string uniqueId = isUniqueId ? id : null;

                            bool isUnique = IsUnique(itemId);

                            if (isUnique && !isUniqueId)
                                return UsageResult.FromError("This item is marked as unique and must be specified by its unique ID.");

                            ItemData data = DataOf(ctx.User, itemId, uniqueId);

                            if (amount < 0)
                                amount = 1;
                            else if (amount > data.Count)
                                amount = data.Count;

                            if (isUnique)
                            {
                                data.Seal = new ItemSealData("$_seal");
                            }
                            else
                            {
                                if (amount == data.Count)
                                {
                                    data.Seal = new ItemSealData("$_seal");
                                }
                                else
                                {
                                    var seal = new ItemData(data.Id, amount);
                                    seal.Seal = new ItemSealData("$_seal");
                                    data.StackCount -= amount;
                                    ctx.User.Items.Add(seal);
                                }
                            }

                            return UsageResult.FromSuccess($"> Successfully wrapped {amount:##,0} **{NameOf(itemId)}**.");
                        }
                    }
                },
                new Item
                {
                    Id = "b_db",
                    Name = "Debt Blocker",
                    Quotes = new List<string>
                    {
                        "It creates a small shield when near debt."
                    },
                    GroupId = ItemGroups.Booster,
                    Rarity = ItemRarity.Uncommon,
                    Tag = ItemTag.Booster,
                    Value = 500,
                    BypassCriteriaOnTrade = true,
                    Size = 75,
                    Usage = new ItemUsage
                    {
                        Durability = 1,
                        DeleteMode = DeleteMode.Break,
                        Action = delegate(UsageContext ctx)
                        {
                            var booster = new BoosterData("b_db", BoosterType.Debt, -0.2f, TimeSpan.FromHours(12), 20);

                            if (!TryApplyBooster(ctx.User, booster))
                                return UsageResult.FromError(">");

                            return UsageResult.FromSuccess("> The **Debt Blocker** opens up, revealing a crystal clear shield, ready to deny what it can.");
                        },
                        OnBreak = user => user.Boosters.Add(new BoosterData(BoosterType.Debt, 0.2f, 20))
                    },
                    OwnLimit = 2
                },
                new Item
                {
                    Id = "su_pl",
                    Name = "Pocket Lawyer",
                    Summary = "A summon that completely wipes all debt from a user.",
                    Quotes = new List<string>
                    {
                        "With my assistance, ORS doesn't stand a chance.",
                        "You'll get the chance to dispute in court, don't worry."
                    },
                    GroupId = ItemGroups.Summon,
                    Rarity = ItemRarity.Myth,
                    Tag = ItemTag.Summon,
                    Value = 750,
                    DeniedHandles = ItemDeny.Buy,
                    TradeLimit = 0,
                    BypassCriteriaOnTrade = true,
                    Size = 100,
                    Usage = new ItemUsage
                    {
                        Durability = 1,
                        Cooldown = TimeSpan.FromDays(3),
                        DeleteMode = DeleteMode.Break,
                        Action = delegate (UsageContext ctx)
                        {
                            if (ctx.User.Debt < 500)
                                return UsageResult.FromError("> You called for help, but the request remains unanswered.");

                            ctx.User.Debt = 0;
                            return UsageResult.FromSuccess("> You have been freed from the shackles of debt.");
                        }
                    },
                    OwnLimit = 3
                },
                new Item
                {
                    Id = "p_gg",
                    Name = "Gamma Green",
                    Summary = "A palette that can be equipped on a card.",
                    Quotes = new List<string>
                    {
                        "It glows with a shade of green similar to uranium."
                    },
                    GroupId = ItemGroups.Palette,
                    Tag = ItemTag.Palette | ItemTag.Decorator,
                    Value = 1000,
                    Size = 50,
                    BypassCriteriaOnTrade = true,
                    Rarity =  ItemRarity.Common,
                    Usage = new ItemUsage
                    {
                        Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, PaletteType.GammaGreen)),
                        DeleteMode = DeleteMode.Break
                    },
                    OwnLimit = 10
                },
                new Item
                {
                    Id = "p_cr",
                    Name = "Crimson",
                    Summary = "A palette that can be equipped on a card.",
                    Quotes = new List<string>
                    {
                        "It thrives in a neon glow of a reddish-purple hue."
                    },
                    GroupId = ItemGroups.Palette,
                    Tag = ItemTag.Palette | ItemTag.Decorator,
                    Value = 1000,
                    Size = 50,
                    BypassCriteriaOnTrade = true,
                    Rarity =  ItemRarity.Common,
                    Usage = new ItemUsage
                    {
                        Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, PaletteType.Crimson)),
                        DeleteMode = DeleteMode.Break
                    },
                    OwnLimit = 10
                },
                new Item
                {
                    Id = "p_wu",
                    Name = "Wumpite",
                    Summary = "A palette that can be equipped on a card.",
                    Quotes = new List<string>
                    {
                        "Crafted with the shades of a Wumpus."
                    },
                    GroupId = ItemGroups.Palette,
                    Tag = ItemTag.Palette | ItemTag.Decorator,
                    Value = 1500,
                    Size = 75,
                    BypassCriteriaOnTrade = true,
                    Rarity =  ItemRarity.Uncommon,
                    Usage = new ItemUsage
                    {
                        Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, PaletteType.Wumpite)),
                        DeleteMode = DeleteMode.Break
                    },
                    OwnLimit = 10
                },
                new Item
                {
                    Id = "p_gl",
                    Name = "Glass",
                    Summary = "A palette that can be equipped on a card.",
                    Quotes = new List<string>
                    {
                        "It refracts a mixture of light blue to white light."
                    },
                    GroupId = ItemGroups.Palette,
                    Tag = ItemTag.Palette | ItemTag.Decorator,
                    Value = 1500,
                    Size = 75,
                    BypassCriteriaOnTrade = true,
                    Rarity =  ItemRarity.Uncommon,
                    Usage = new ItemUsage
                    {
                        Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, PaletteType.Glass)),
                        DeleteMode = DeleteMode.Break
                    },
                    OwnLimit = 10
                },
                new Item
                {
                    Id = Items.PaletteGlossyGreen,
                    Name = "Glossy Green",
                    Summary = "A palette that can be equipped on a card.",
                    Quotes = new List<string>
                    {
                        "It refracts a mixture of light that glows a bright green."
                    },
                    GroupId = ItemGroups.Palette,
                    Tag = ItemTag.Palette | ItemTag.Decorator,
                    DeniedHandles = ItemDeny.Buy | ItemDeny.Sell | ItemDeny.Clone,
                    Value = 3000,
                    Size = 150,
                    TradeLimit = 0,
                    Rarity =  ItemRarity.Rare,
                    Usage = new ItemUsage
                    {
                        Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, new ComponentPalette(PaletteType.GammaGreen, PaletteType.Glass))),
                        DeleteMode = DeleteMode.Break
                    },
                    OwnLimit = 5
                },
                new Item
                {
                    Id = Items.PaletteGlossyWumpite,
                    Name = "Glossy Wumpite",
                    Summary = "A palette that can be equipped on a card.",
                    Quotes = new List<string>
                    {
                        "It refracts a mixture of light that absorbs the color of a Wumpus."
                    },
                    GroupId = ItemGroups.Palette,
                    Tag = ItemTag.Palette | ItemTag.Decorator,
                    DeniedHandles = ItemDeny.Buy | ItemDeny.Sell | ItemDeny.Clone,
                    Value = 3500,
                    Size = 175,
                    TradeLimit = 0,
                    Rarity =  ItemRarity.Rare,
                    Usage = new ItemUsage
                    {
                        Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, new ComponentPalette(PaletteType.Wumpite, PaletteType.Glass))),
                        DeleteMode = DeleteMode.Break
                    },
                    OwnLimit = 5
                }
            };

        private static bool TryApplyBooster(ArcadeUser user, BoosterData booster)
        {
            if (Var.GetOrSet(user, Vars.BoosterLimit, 1) - user.Boosters.Count <= 0)
                return false;

            if (booster == null)
                throw new Exception("Expected a booster data to be specified but returned null");

            user.Boosters.Add(booster);
            return true;
        }

        private static string SetOrSwapPalette(ArcadeUser user, ComponentPalette palette)
        {
            if (user.Card.Palette == palette)
                return Format.Warning($"You already have **{NameFor(palette.Primary, palette.Secondary)}** equipped on your **Card Palette**.");

            string result = $"> 📟 Equipped **{NameFor(palette.Primary, palette.Secondary)}** to your **Card Palette**.";
            if (user.Card.Palette.Primary != PaletteType.Default)
            {
                GiveItem(user, IdFor(user.Card.Palette.Primary, user.Card.Palette.Secondary));
                result = $"📟 Swapped out **{NameFor(user.Card.Palette.Primary)}** with **{NameFor(palette.Primary, palette.Secondary)}** for your **Card Palette**.";
            }

            TakeItem(user, IdFor(palette.Primary, palette.Secondary));
            user.Card.Palette = palette;
            return result;
        }

        public static bool RemovePalette(ArcadeUser user)
        {
            if (user.Card.Palette == PaletteType.Default)
                return false;

            GiveItem(user, IdFor(user.Card.Palette.Primary, user.Card.Palette.Secondary));
            user.Card.Palette = PaletteType.Default;
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

            return item.TradeLimit.HasValue ? (data?.Data?.TradeCount ?? 0) < item.TradeLimit.Value : true;
        }

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

            return item.TradeLimit.HasValue ? (data?.Data?.TradeCount ?? 0) < item.TradeLimit.Value : true;
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

        internal static string GetCooldownId(string itemId)
            => $"{itemId}:last_used";

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

        public static Item GetItem(string id)
        {
            if (LItems.Count(x => x.Id == id) > 1)
                throw new ArgumentException("There are more than one items with the specified ID.");

            return LItems.FirstOrDefault(x => x.Id == id);
        }

        public static bool Exists(string itemId)
            => LItems.Any(x => x.Id == itemId);

        public static string GroupOf(string id)
        {
            if (!TryGetItem(id, out Item item))
                return null;

            return item.GroupId;
        }

        public static string NameOf(string id)
        {
            if (!TryGetItem(id, out Item item))
                return id;

            if (TryGetGroup(item.GroupId, out ItemGroup group))
                return $"{group.Prefix}{item.Name}";

            return item.Name;
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

        public static bool GroupExists(string id)
            => Check.NotNull(id) && Groups.Any(x => x.Id == id);

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

        internal static string NameFor(PaletteType palette, PaletteType? secondary = null)
            => GetItem(IdFor(palette, secondary)).Name;

        internal static string IdFor(PaletteType palette, PaletteType? secondary = null)
        {
            return palette switch
            {
                PaletteType.GammaGreen when secondary == PaletteType.Glass => Items.PaletteGlossyGreen,
                PaletteType.GammaGreen => Items.PaletteGammaGreen,

                PaletteType.Crimson => Items.PaletteCrimson,

                PaletteType.Glass when secondary == PaletteType.GammaGreen => Items.PaletteGlossyGreen,
                PaletteType.Glass when secondary == PaletteType.Wumpite => Items.PaletteGlossyWumpite,
                PaletteType.Glass => Items.PaletteGlass,

                PaletteType.Wumpite when secondary == PaletteType.Glass => Items.PaletteGlossyWumpite,
                PaletteType.Wumpite => Items.PaletteWumpite,
                _ => null
            };
        }

        private static PaletteType PaletteOf(string paletteId)
        {
            Item item = GetItem(paletteId);

            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (!item.Tag.HasFlag(ItemTag.Palette))
                throw new ArgumentException("Could not resolve item as type of Palette.");

            if (paletteId == Items.PaletteCrimson)
                return PaletteType.Crimson;

            if (paletteId == Items.PaletteGammaGreen)
                return PaletteType.GammaGreen;

            if (paletteId == Items.PaletteGlass)
                return PaletteType.Glass;

            if (paletteId == Items.PaletteWumpite)
                return PaletteType.Wumpite;

            // unknown palette
            return PaletteType.Default;
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

        public static void GiveItem(ArcadeUser user, string itemId, int amount = 1)
            => GiveItem(user, GetItem(itemId), amount);

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

        // this stores an item to the specified user.
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
            {
                ItemData selected = GetBestStack(user, item);

                if (selected == null)
                {
                    user.Items.Add(new ItemData(item.Id, amount));
                }
                else
                {
                    selected.StackCount += amount;
                }
            }
        }

        public static IEnumerable<ItemData> GetFromInventory(ArcadeUser user, string itemId)
            => user.Items.Where(x => x.Id == itemId);

        public static ItemData GetFromInventory(ArcadeUser user, string itemId, string uniqueId)
            => user.Items.First(x => x.Id == itemId && x.Data != null && x.Data.Id == uniqueId);

        public static UniqueItemData CreateUniqueData(Item item)
        {
            if (!IsUnique(item))
                throw new Exception("The specified item is not unique.");

            var data = new UniqueItemData
            {
                Durability = item.Usage?.Durability,
                ExpiresOn = ((item.Usage?.ExpiryTrigger ?? 0) == ExpiryTrigger.Own && (item.Usage?.Expiry.HasValue ?? false)) ? DateTime.UtcNow.Add(item.Usage.Expiry.Value) : (DateTime?) null
            };

            // No need to initialize if the item cannot be traded
            if (item.TradeLimit.HasValue && item.TradeLimit > 0)
                data.TradeCount = 0;

            return data;
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

        public static ItemData Peek(ArcadeUser user, string uniqueId)
            => user.Items.FirstOrDefault(x => x.Data?.Id == uniqueId);

        public static ItemData DataOf(ArcadeUser user, string itemId, string uniqueId = null)
        {
            return user.Items.FirstOrDefault(x => (!string.IsNullOrWhiteSpace(uniqueId)
                ? x.Data?.Id == uniqueId
                : x.Id == itemId) && x.Seal == null);
        }

        public static ItemData DataOf(ArcadeUser user, Item item, string uniqueId = null)
            => DataOf(user, item.Id, uniqueId);

        public static ItemTag GetTag(string itemId)
            => GetItem(itemId)?.Tag ?? 0;

        public static UsageResult UseItem(ArcadeUser user, ItemData data, string input = null)
        {
            var isBroken = false;
            Item item = GetItem(data.Id);

            // If a usage isn't specified
            if (item.Usage == null)
                return UsageResult.FromError("This item is not usable.");

            // If the item is sealed
            if (data.Seal != null)
            {
                // If the item has requirements to unlock
                if (Check.NotNullOrEmpty(data.Seal.ToUnlock))
                {
                    foreach ((string id, VarProgress progress) in data.Seal.ToUnlock)
                        if (progress.Current < progress.Required)
                            return UsageResult.FromError("You have not met all of the criteria needed to open this seal.");
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

                return UsageResult.FromSuccess($"You have released the seal and found a {NameOf(data.Id)}");
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
                        return UsageResult.FromError("This item has expired and cannot be used.");

                // If the item is broken
                if (data.Data.Durability <= 0)
                {
                    if (item.Usage.DeleteMode.HasFlag(DeleteMode.Break))
                    {
                        user.Items.Remove(data);
                        return UsageResult.FromError("This item is broken and will be removed.");
                    }

                    return UsageResult.FromError("This item is broken and cannot be used.");
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

        private static string GetBestUniqueId(ArcadeUser user, string itemId)
            => GetBestUniqueId(user, GetItem(itemId));

        private static string GetBestUniqueId(ArcadeUser user, Item item)
        {
            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (!IsUnique(item))
                throw new ArgumentException("The specified item is not unique.");

            // Slim down to all matching item entries of the same item
            var matching = user.Items.Where(x => x.Id == item.Id && !x.Locked);

            if (user.Items.All(x => x.Id != item.Id))
                return null;

            if (item.Usage?.Durability != null)
            {
                matching = matching.OrderBy(x => x.Data.Durability);
            }

            return matching.First().Id;
        }
    }
}
