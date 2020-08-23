﻿using System;
using System.Collections.Generic;
using System.Linq;
using Arcadia.Graphics;
using Arcadia.Services;
using Orikivo;
using PaletteType = Arcadia.Graphics.PaletteType;

namespace Arcadia
{
    public enum CatalogStatus
    {
        Unknown = 0,
        Seen = 1,
        Known = 2
    }

    // TODO: Implement item attribute reading and data population
    public static class ItemHelper
    {
        public static IEnumerable<Item> GetSeenItems(ArcadeUser user)
        {
            return user.Stats
                .Where(x => x.Key.StartsWith("catalog:") && x.Value == (long)CatalogStatus.Seen)
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
                .Where(x => x.Key.StartsWith("catalog:") && GroupOf(Var.GetKey(x.Key)) == itemGroupId && x.Value == (long)CatalogStatus.Seen)
                .Select(x => GetItem(Var.GetKey(x.Key)));
        }

        public static IEnumerable<Item> GetKnownItems(ArcadeUser user, string itemGroupId)
        {
            return user.Stats
                .Where(x => x.Key.StartsWith("catalog:") && GroupOf(Var.GetKey(x.Key)) == itemGroupId && x.Value == (long)CatalogStatus.Known)
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
                .Count(x => x.Key.StartsWith("catalog:") && x.Value == (long)CatalogStatus.Known);
        }

        public static int GetKnownCount(ArcadeUser user, string itemGroup)
        {
            return user.Stats
                .Count(x => x.Key.StartsWith("catalog:") && GroupOf(Var.GetKey(x.Key)) == itemGroup && x.Value == (long)CatalogStatus.Known);
        }

        public static int GetSeenCount(ArcadeUser user)
        {
            return user.Stats
                .Count(x => x.Key.StartsWith("catalog:") && x.Value == (long)CatalogStatus.Seen);
        }

        public static int GetSeenCount(ArcadeUser user, string itemGroup)
        {
            return user.Stats
                .Count(x => x.Key.StartsWith("catalog:") && GroupOf(Var.GetKey(x.Key)) == itemGroup && x.Value == (long)CatalogStatus.Seen);
        }

        private static string GetCatalogId(string itemId)
            => $"catalog:{itemId}";

        public static CatalogStatus GetCatalogStatus(ArcadeUser user, string itemId)
        {
            return (CatalogStatus)user.GetVar(GetCatalogId(itemId));
        }

        public static CatalogStatus GetCatalogStatus(ArcadeUser user, Item item)
            => GetCatalogStatus(user, item.Id);

        public static void SetCatalogStatus(ArcadeUser user, string itemId, CatalogStatus status)
        {
            // If the user has already seen or known about this item, return;
            if (GetCatalogStatus(user, itemId) >= status)
                return;


            user.SetVar(GetCatalogId(itemId), (long)status);
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
                   | item.BypassCriteriaOnGift
                   | item.OwnLimit.HasValue
                   | (item.GiftLimit > 0 || !item.GiftLimit.HasValue)
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
                Id = "recipe:glossy_green",
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Items.PaletteGlass, 1),
                    new RecipeComponent(Items.PaletteGammaGreen, 1)
                },
                Result = new RecipeComponent(Items.PaletteGlossyGreen, 1)
            },
            new Recipe
            {
                Id = "recipe:glossy_wumpite",
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Items.PaletteGlass, 1),
                    new RecipeComponent(Items.PaletteWumpite, 1)
                },
                Result = new RecipeComponent(Items.PaletteGlossyWumpite, 1)
            }
        };

        // TODO: Implement recipe knowledge and criteria
        public static IEnumerable<Recipe> GetKnownRecipes(ArcadeUser user)
        {
            return LRecipes;
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

        /*
        public static long GetUniqueId()
        {
            var offset = new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return (DateTime.UtcNow.Ticks - offset.Ticks);
        }*/

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
            if (LRecipes.Count(x => x.Id == id) > 1)
                throw new ArgumentException("There are more than one recipes with the specified ID.");

            return LRecipes.FirstOrDefault(x => x.Id == id);
        }

        public static bool KnowsRecipe(ArcadeUser user, string recipeId)
            => KnowsRecipe(user, GetRecipe(recipeId));

        // TODO: Implement recipe knowledge criteria
        public static bool KnowsRecipe(ArcadeUser user, Recipe recipe)
        {
            return true;
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
            => Check.NotNull(id) && LRecipes.Any(x => x.Id == id);

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

        public static Item ItemOf(ArcadeUser user, string uniqueId)
        {
            if (user.Items.Any(x => x.Data?.Id == uniqueId))
                return null;

            return GetItem(user.Items.FirstOrDefault(x => x.Data?.Id == uniqueId)?.Id);
        }

        public static List<Item> LItems =>
            new List<Item>
            {
                new Item
                {
                    Id = "au_gimi",
                    Name = "Gimi",
                    Summary = "Gives you the ability to execute the Gimi command a specified number of times. Please note that you are required to wait a second on each execution.",
                    Quotes = new List<string>
                    {
                        "It echoes a tick fast enough to break the sound barrier."
                    },
                    GroupId = ItemGroups.Automaton,
                    Rarity = ItemRarity.Myth,
                    Tag = ItemTag.Tool | ItemTag.Automaton,
                    Value = 100000,
                    CanBuy = false,
                    CanSell = false,
                    CanDestroy = true,
                    TradeLimit = 0,
                    GiftLimit = 0,
                    Size = 1000,
                    OwnLimit = 1,
                    MarketCriteria = null,
                    ToOwn = null,
                    ToUnlock = null
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
                    CanBuy = true,
                    CanSell = true,
                    CanDestroy = true,
                    BypassCriteriaOnGift = true,
                    Size = 75,
                    Usage = new ItemAction
                    {
                        Durability = 1,
                        DeleteOnBreak = true,
                        DeleteMode = ItemDeleteMode.Break,
                        Criteria = user => Var.GetOrSet(user, Vars.BoosterLimit, 1) - user.Boosters.Count > 0,
                        Action = delegate(ArcadeUser user)
                        {
                            user.Boosters.Add(new BoosterData("b_db", BoosterType.Debt, -0.2f, TimeSpan.FromHours(12), 20));

                            return new UsageResult("You have applied a booster.");
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
                    Value = 1000,
                    CanBuy = false,
                    CanSell = true,
                    CanDestroy = true,
                    TradeLimit = 0,
                    BypassCriteriaOnGift = true,
                    Size = 100,
                    Usage = new ItemAction
                    {
                        Criteria = user => user.Debt >= 500,
                        Durability = 1,
                        Cooldown = TimeSpan.FromDays(3),
                        DeleteOnBreak = true,
                        DeleteMode = ItemDeleteMode.Break,
                        Action = delegate (ArcadeUser user)
                        {
                            user.Debt = 0;
                            return new UsageResult("> You have been freed from the shackles of debt.");
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
                    CanBuy = true,
                    CanSell = true,
                    BypassCriteriaOnGift = true,
                    Rarity =  ItemRarity.Common,
                    Usage = new ItemAction
                    {
                        Action = user => new UsageResult(SetOrSwapPalette(user, PaletteType.GammaGreen))
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
                    CanBuy = true,
                    CanSell = true,
                    BypassCriteriaOnGift = true,
                    Rarity =  ItemRarity.Common,
                    Usage = new ItemAction
                    {
                        Action = user => new UsageResult(SetOrSwapPalette(user, PaletteType.Crimson))
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
                    CanBuy = true,
                    CanSell = true,
                    BypassCriteriaOnGift = true,
                    Rarity =  ItemRarity.Uncommon,
                    Usage = new ItemAction
                    {
                        Action = user => new UsageResult(SetOrSwapPalette(user, PaletteType.Wumpite))
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
                    CanBuy = true,
                    CanSell = true,
                    BypassCriteriaOnGift = true,
                    Rarity =  ItemRarity.Uncommon,
                    Usage = new ItemAction
                    {
                        Action = user => new UsageResult(SetOrSwapPalette(user, PaletteType.Glass))
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
                    Value = 3000,
                    Size = 150,
                    TradeLimit = 0,
                    GiftLimit = 0,
                    Rarity =  ItemRarity.Rare,
                    Usage = new ItemAction
                    {
                        Action = user => new UsageResult(SetOrSwapPalette(user, new ComponentPalette(PaletteType.GammaGreen, PaletteType.Glass)))
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
                    Value = 3500,
                    Size = 175,
                    TradeLimit = 0,
                    GiftLimit = 0,
                    Rarity =  ItemRarity.Rare,
                    Usage = new ItemAction
                    {
                        Action = user => new UsageResult(SetOrSwapPalette(user, new ComponentPalette(PaletteType.Wumpite, PaletteType.Glass)))
                    },
                    OwnLimit = 5
                }
            };

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

        public static bool CanGift(string itemId, UniqueItemData data)
            => CanGift(GetItem(itemId), data);

        public static bool CanGift(Item item, UniqueItemData data)
        {
            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (item.GiftLimit.HasValue)
            {
                if (data != null)
                    if (data.GiftCount.GetValueOrDefault(0) >= item.GiftLimit.Value)
                        return false;
            }

            return true;
        }

        public static bool CanTrade(string itemId, UniqueItemData data)
            => CanTrade(GetItem(itemId), data);

        public static bool CanTrade(Item item, UniqueItemData data)
        {
            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (item.TradeLimit.HasValue)
            {
                if ((data?.TradeCount ?? 0) >= item.TradeLimit.Value)
                    return false;
            }

            return true;
        }

        public static DateTime? GetLastUsed(ArcadeUser user, string itemId, string uniqueId = null)
        {
            Item item = GetItem(itemId);

            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (item.Usage == null)
                throw new Exception("Expected item usage to be specified but returned null");

            string statGroup = item.Usage?.CooldownCategory switch
            {
                CooldownCategory.Instance => uniqueId ?? throw new Exception("Expected a unique item reference but was unspecified"),
                CooldownCategory.Item => itemId,
                CooldownCategory.Group => item.GroupId ?? throw new Exception("Expected an item group but was empty"),
                CooldownCategory.Global => "global",
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

        public static bool CanUse(ArcadeUser user, string itemId, UniqueItemData data = null)
            => CanUse(user, GetItem(itemId), data);

        public static bool CanUse(ArcadeUser user, Item item, UniqueItemData data = null)
        {
            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (item.Usage == null)
                return false;

            DateTime? lastUsed = GetLastUsed(user, item.Id);

            if (lastUsed.HasValue && item.Usage.Cooldown.HasValue)
            {
                // if the cooldown has expired, allow use.
                if ((DateTime.UtcNow - lastUsed.Value.Add(item.Usage.Cooldown.Value)) >= TimeSpan.Zero)
                    return true;

                return false;
            }

            if (data != null)
            {
                if (data.ExpiresOn.HasValue)
                    if (DateTime.UtcNow >= data.ExpiresOn.Value)
                        return false;

                if (data.Durability.HasValue)
                    if (data.Durability == 0)
                        return false;
            }

            return item.Usage.Criteria?.Invoke(user) ?? true;
        }

        public static bool IsUnique(string itemId)
            => IsUnique(GetItem(itemId));

        public static bool IsUnique(Item item)
        {
            if (item.TradeLimit.HasValue)
            // does this item have a specified trade limitation?
            if (item.TradeLimit > 0)
                return true;

            if (item.GiftLimit.HasValue)
            // does this item have a specified gift limitation?
            if (item.GiftLimit > 0)
                return true;

            if (item.Usage?.Durability != null)
            {
                // can it be used more than once?
                if (item.Usage.Durability > 1)
                    return true;

                // is this item left behind when the durability is broken?
                if (!item.Usage.DeleteOnBreak)
                    return true;
            }

            //if (item.OnUse.Cooldown.HasValue)
            //    return true;

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

            // This method only works for non-unique items.
            if (IsUnique(item))
            {
                for (int i = 0; i < amount; i++)
                {
                    string uniqueId = GetBestUniqueId(user, item.Id);


                }
                TakeItem(user, item, GetBestUniqueId(user, item.Id));
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

            /*
            if (!HasItem(user, item.Id) && item.Tag.HasFlag(ItemTag.Palette))
            {
                PaletteType type = PaletteOf(item.Id);

                // If the specified palette could not be resolved.
                if (type == PaletteType.Default)
                    return;

                if (type == user.Card.Palette.Primary)
                    user.Card.Palette = PaletteType.Default; // If the palette was taken away, set to default palette.

            }
            */
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
                ItemData selected = DataOf(user, item.Id);

                if (selected == null)
                {
                    user.Items.Add(new ItemData(item.Id, amount));
                }
                else
                {
                    selected.StackCount += amount;
                }


                //user.Items.First(x => x.Id == item.Id).StackCount += amount;
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
                TradeCount = null,
                GiftCount = null,
                ExpiresOn = null,
                LastUsed = null
            };

            // No need to initialize if the item cannot be traded
            if (item.TradeLimit.HasValue && item.TradeLimit > 0)
                data.TradeCount = 0;

            // No need to initialize if the item cannot be gifted
            if (item.GiftLimit.HasValue && item.GiftLimit > 0)
                data.GiftCount = 0;

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

        public static UniqueItemData Peek(ArcadeUser user, string uniqueId)
            => user.Items.FirstOrDefault(x => x.Data?.Id == uniqueId)?.Data;

        public static ItemData DataOf(ArcadeUser user, string itemId, string uniqueId = null)
        {
            return user.Items.FirstOrDefault(x => !string.IsNullOrWhiteSpace(uniqueId)
                ? x.Data?.Id == uniqueId
                : x.Id == itemId);
        }

        public static ItemData DataOf(ArcadeUser user, Item item, string uniqueId = null)
            => DataOf(user, item.Id, uniqueId);

        public static ItemTag GetTag(string itemId)
            => GetItem(itemId)?.Tag ?? 0;

        public static UsageResult UseItem(ArcadeUser user, string itemId, string uniqueId = null)
            => UseItem(user, GetItem(itemId), uniqueId);

        public static UsageResult UseItem(ArcadeUser user, Item item, string uniqueId = null)
        {
            var isBroken = false;

            UsageResult result = new UsageResult(false);

            // if there is no available action.
            if (item.Usage == null)
                return result;

            // if the user doesn't even have an item.
            if (!HasItem(user, item))
                return result;

            // otherwise, check if the user can use the item
            if (IsUnique(item))
            {
                // If a unique ID wasn't specified, get the unique ID of the one closest to being broken.
                uniqueId ??= GetBestUniqueId(user, item);

                // There isn't an available item to use in this case.
                if (string.IsNullOrWhiteSpace(uniqueId))
                    return result;

                var data = Peek(user, uniqueId);

                if (!CanUse(user, item, data))
                {
                    return result;
                }

                // first, check if the item will be deleted on use
                if (item.Usage.Durability.HasValue)
                {
                    if (item.Usage.DeleteOnBreak)
                    {
                        // if it's going to be deleted on use, remove it
                        if (data.Durability - 1 <= 0)
                            TakeItem(user, item, uniqueId);
                    }

                    if (data.Durability > 0)
                        data.Durability--; // remove 1 from the durability.

                    if (data.Durability == 0)
                        isBroken = true;
                }
            }
            else
            {
                if (!CanUse(user, item))
                    return result;

                if (item.Usage.Durability == 1)
                {
                    if (item.Usage.DeleteOnBreak)
                        TakeItem(user, item);

                    isBroken = true;
                }
            }

            if (item.Usage.Cooldown.HasValue)
            {
                if (item.Usage.CooldownCategory == CooldownCategory.Instance)
                {
                    if (!IsUnique(item))
                        throw new Exception("Expected an item to be unique");

                    if (!HasItem(user, item, uniqueId))
                        throw new Exception("Expected to find a unique item but does not exist");

                    DataOf(user, item, uniqueId).Data.LastUsed = DateTime.UtcNow;
                }
                else if (item.Usage.CooldownCategory == CooldownCategory.Group)
                {
                    user.SetVar($"{item.GroupId}:last_used", DateTime.UtcNow.Ticks);
                }
                else
                {
                    // finally, if all of the checks pass, use up the item.
                    user.SetVar(GetCooldownId(item.Id), DateTime.UtcNow.Ticks);
                }
            }

            // As the final step, invoke the action defined on the item.
            // If the usage wasn't successful, don't update the item.
            // That way, the criteria can be merged with the usage
            result = item.Usage.Action(user);

            // If the item broke, invoke that action too.
            if (isBroken)
            {
                // Only invoke breaking if the group is not a booster
                // This is because boosters reference this method when used up
                if (item.GroupId != ItemGroups.Booster)
                    item.Usage.OnBreak?.Invoke(user);

                user.AddToVar(Stats.ItemsUsed);
            }

            return result;
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
            var matching = user.Items.Where(x => x.Id == item.Id && !x.Data.Locked);

            if (user.Items.All(x => x.Id != item.Id))
                return "";

            if (item.Usage?.Durability != null)
            {
                matching = matching.OrderBy(x => x.Data.Durability);
            }

            return matching.First().Id;
        }
    }
}
