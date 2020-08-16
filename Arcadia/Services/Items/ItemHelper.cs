using System;
using System.Collections.Generic;
using System.Linq;
using Arcadia.Services;
using Orikivo;
using PaletteType = Arcadia.Graphics.PaletteType;

namespace Arcadia
{
    // TODO: Implement item attribute reading and data population
    public static class ItemHelper
    {
        public static readonly List<Recipe> Recipes = new List<Recipe>
        {
            new Recipe
            {
                Id = "recipe:glossy_green",
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Arcadia.Items.PaletteGlass, 1),
                    new RecipeComponent(Arcadia.Items.PaletteGammaGreen, 1)
                },
                Result = new RecipeComponent(Arcadia.Items.PaletteGlossyGreen, 1)
            }
        };

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
            double rate = 1;

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
                    parent?.OnUse?.OnBreak?.Invoke(user);
                }
            }

            // Unless the rate is 0%, keep it at a minimum of 1
            long result = (long) Math.Round(value * (isNegative ? 1 + (1 - rate) : rate), MidpointRounding.AwayFromZero);
            return result == 0 && (Math.Abs(rate) < 0.001) ? 1 : result;
        }

        public static IEnumerable<Recipe> RecipesFor(string itemId)
            => RecipesFor(GetItem(itemId));

        public static IEnumerable<Recipe> RecipesFor(Item item)
        {
            return Recipes.Where(x => x.Result.ItemId == item.Id);
        }

        public static readonly List<ItemGroup> Groups = new List<ItemGroup>
        {
            new ItemGroup
            {
                Id = "booster",
                Icon = Icons.Booster,
                Prefix = "Booster: ",
                Summary = "Modifies the multiplier for a specified form of income."
            },

            new ItemGroup
            {
                Id = "palette",
                Icon = Icons.Palette,
                Prefix = "Card Palette: ",
                Summary = "Modifies the color scheme that is displayed on a card."
            },
            new ItemGroup
            {
                Id = "summon",
                Icon = Icons.Summon,
                Prefix = "Summon: "
            },
            new ItemGroup
            {
                Id = "automaton",
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

        public static void Craft(ArcadeUser user, Recipe recipe)
        {
            if (!CanCraft(user, recipe))
                throw new ArgumentException("Cannot craft this recipe");
        }

        public static ItemGroup GetGroup(string id)
        {
            if (Groups.Count(x => x.Id == id) > 1)
                throw new ArgumentException("There are more than one groups with the specified ID.");

            return Groups.FirstOrDefault(x => x.Id == id);
        }

        public static Recipe GetRecipe(string id)
        {
            if (Recipes.Count(x => x.Id == id) > 1)
                throw new ArgumentException("There are more than one recipes with the specified ID.");

            return Recipes.FirstOrDefault(x => x.Id == id);
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
            => Check.NotNull(id) && Recipes.Any(x => x.Id == id);

        public static bool CanCraft(ArcadeUser user, string recipeId)
            => CanCraft(user, GetRecipe(recipeId));

        public static bool CanCraft(ArcadeUser user, Recipe recipe)
        {
            foreach ((string itemId, int amount) in recipe.Components)
            {
                if (!HasItem(user, itemId) || GetOwnedAmount(user, itemId) != amount)
                    return false;
            }

            return true;
        }

        public static string DetailsOf(Item item)
            => Catalog.WriteItem(item);

        public static string ItemOf(ArcadeUser user, string uniqueId)
        {
            if (user.Items.Any(x => x.Data?.Id == uniqueId))
                return "";

            return user.Items.FirstOrDefault(x => x.Data?.Id == uniqueId)?.Id;
        }

        public static List<Item> Items =>
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
                    ToUnlock = null,
                    ToExpire = null
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
                    OnUse = new ItemAction
                    {
                        Durability = 1,
                        DeleteOnBreak = true,
                        Criteria = user => StatHelper.GetOrAdd(user, Vars.BoosterLimit, 1) - user.Boosters.Count > 0,
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
                    OnUse = new ItemAction
                    {
                        Criteria = user => user.Debt >= 500,
                        Durability = 1,
                        Cooldown = TimeSpan.FromHours(72),
                        DeleteOnBreak = true,
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
                    OnUse = new ItemAction
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
                    OnUse = new ItemAction
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
                        "Crafted with the shades of a bluish-purple pig-like entity."
                    },
                    GroupId = ItemGroups.Palette,
                    Tag = ItemTag.Palette | ItemTag.Decorator,
                    Value = 1500,
                    Size = 75,
                    CanBuy = true,
                    CanSell = true,
                    BypassCriteriaOnGift = true,
                    Rarity =  ItemRarity.Uncommon,
                    OnUse = new ItemAction
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
                    OnUse = new ItemAction
                    {
                        Action = user => new UsageResult(SetOrSwapPalette(user, PaletteType.Glass))
                    },
                    OwnLimit = 10
                }
            };

        private static string SetOrSwapPalette(ArcadeUser user, PaletteType palette)
        {
            if (user.Card.Palette.Primary == palette)
                return Format.Warning($"You already have **{palette.ToString()}** equipped on your **Card Palette**.");

            string result = $"> 📟 Equipped **{palette.ToString()}** to your **Card Palette**.";
            if (user.Card.Palette.Primary != PaletteType.Default)
            {
                GiveItem(user, IdFor(user.Card.Palette.Primary));
                result = $"📟 Swapped out **{user.Card.Palette}** for {palette} on your **Card Palette**.";
            }

            TakeItem(user, IdFor(palette));
            user.Card.Palette = palette;
            return result;
        }

        public static bool CanGift(string itemId, UniqueItemData data)
        {
            var item = GetItem(itemId);

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
        {
            var item = GetItem(itemId);

            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (item.TradeLimit.HasValue)
            {
                if ((data?.TradeCount ?? 0) >= item.TradeLimit.Value)
                    return false;
            }

            return true;
        }

        public static DateTime? GetLastUsed(ArcadeUser user, string itemId)
        {
            Item item = GetItem(itemId);
            var ticks = user.GetStat(GetCooldownId(itemId));

            if (item.OnUse?.CooldownType == UsageCooldownType.Group)
                ticks = user.GetStat(GetCooldownId(item.GroupId));

            if (ticks == 0)
                return null;

            return new DateTime(ticks);
        }

        internal static string GetCooldownId(string itemId)
            => $"{itemId}:last_used";

        public static TimeSpan? GetCooldownRemainder(ArcadeUser user, string itemId)
        {
            Item item = GetItem(itemId);

            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (item.OnUse != null)
            {
                DateTime? lastUsed = GetLastUsed(user, itemId);

                if (item.OnUse.Cooldown.HasValue && lastUsed.HasValue)
                {
                    return (DateTime.UtcNow - lastUsed.Value.Add(item.OnUse.Cooldown.Value));
                }
            }

            return null;
        }

        public static bool CanUse(ArcadeUser user, string itemId, UniqueItemData data = null)
        {
            Item item = GetItem(itemId);

            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (item.OnUse == null)
                return false;

            DateTime? lastUsed = GetLastUsed(user, itemId);

            if (lastUsed.HasValue && item.OnUse.Cooldown.HasValue)
            {
                // if the cooldown has expired, allow use.
                if ((DateTime.UtcNow - lastUsed.Value.Add(item.OnUse.Cooldown.Value)) >= TimeSpan.Zero)
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

            return item.OnUse.Criteria?.Invoke(user) ?? true;
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
                return true; // TODO: Revert to true when ready

            if (item.OnUse?.Durability != null)
            {
                // can it be used more than once?
                if (item.OnUse.Durability > 1)
                    return true;

                // is this item left behind when the durability is broken?
                if (!item.OnUse.DeleteOnBreak)
                    return true;
            }

            //if (item.OnUse.Cooldown.HasValue)
            //    return true;

            return false;
        }

        public static Item GetItem(string id)
        {
            if (Items.Count(x => x.Id == id) > 1)
                throw new ArgumentException("There are more than one items with the specified ID.");

            return Items.FirstOrDefault(x => x.Id == id);
        }

        public static bool Exists(string itemId)
            => Items.Any(x => x.Id == itemId);


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

            if (!HasItem(user, item.Id) && item.Tag.HasFlag(ItemTag.Palette))
            {
                PaletteType type = PaletteOf(item.Id);

                // If the specified palette could not be resolved.
                if (type == PaletteType.Default)
                    return;

                if (type == user.Card.Palette.Primary)
                    user.Card.Palette = PaletteType.Default; // If the palette was taken away, set to default palette.

            }
        }

        internal static string IdFor(PaletteType palette)
        {
            return palette switch
            {
                PaletteType.GammaGreen => Arcadia.Items.PaletteGammaGreen,
                PaletteType.Crimson => Arcadia.Items.PaletteCrimson,
                PaletteType.Glass => Arcadia.Items.PaletteGlass,
                PaletteType.Wumpite => Arcadia.Items.PaletteWumpite,
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

            if (paletteId == Arcadia.Items.PaletteCrimson)
                return PaletteType.Crimson;

            if (paletteId == Arcadia.Items.PaletteGammaGreen)
                return PaletteType.GammaGreen;

            if (paletteId == Arcadia.Items.PaletteGlass)
                return PaletteType.Glass;

            if (paletteId == Arcadia.Items.PaletteWumpite)
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
                    user.Items.Add(new ItemData(item.Id, GetUniqueData(item)));
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

        public static UniqueItemData GetUniqueData(Item item)
        {
            if (!IsUnique(item))
                throw new Exception("The specified item is not unique.");

            var data = new UniqueItemData()
            {
                Durability = item.OnUse?.Durability,
                TradeCount = null,
                GiftCount = null,
                ExpiresOn = null
            };

            if (item.TradeLimit.HasValue)
                data.TradeCount = 0;

            if (item.GiftLimit.HasValue)
                data.GiftCount = 0;

            return data;
        }

        public static bool HasItem(ArcadeUser user, string itemId)
            => user.Items.Any(x => x.Id == itemId);

        public static bool HasItemAt(ArcadeUser user, string itemId, Func<ItemData, bool> criterion)
            => HasItem(user, itemId) && user.Items.Any(criterion);

        public static UniqueItemData Peek(ArcadeUser user, string uniqueId)
            => user.Items.FirstOrDefault(x => x.Data?.Id == uniqueId)?.Data;

        public static ItemData DataOf(ArcadeUser user, string itemId, string uniqueId = null)
        {
            return user.Items.FirstOrDefault(x => !string.IsNullOrWhiteSpace(uniqueId)
                ? x.Data?.Id == uniqueId
                : x.Id == itemId);
        }

        public static UsageResult UseItem(ArcadeUser user, string itemId, string uniqueId = null)
        {
            Item item = GetItem(itemId);
            var isBroken = false;

            UsageResult result = new UsageResult(false);

            // if there is no available action.
            if (item.OnUse == null)
                return result;

            // if the user doesn't even have an item.
            if (!HasItem(user, itemId))
                return result;

            // otherwise, check if the user can use the item
            if (IsUnique(item))
            {
                // If a unique ID wasn't specified, get the unique ID of the one closest to being broken.
                uniqueId ??= GetBestUniqueId(user, itemId);

                // There isn't an available item to use in this case.
                if (string.IsNullOrWhiteSpace(uniqueId))
                    return result;

                var data = Peek(user, uniqueId);

                if (!CanUse(user, itemId, data))
                {
                    return result;
                }

                // first, check if the item will be deleted on use
                if (item.OnUse.Durability.HasValue)
                {
                    if (item.OnUse.DeleteOnBreak)
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
                if (!CanUse(user, itemId))
                    return result;

                if (item.OnUse.Durability == 1)
                {
                    if (item.OnUse.DeleteOnBreak)
                        TakeItem(user, item);

                    isBroken = true;
                }
            }

            if (item.OnUse.Cooldown.HasValue)
            {
                if (item.OnUse.CooldownType == UsageCooldownType.Group)
                {
                    user.SetStat($"{item.GroupId}:last_used", DateTime.UtcNow.Ticks);
                }
                else
                {
                    // finally, if all of the checks pass, use up the item.
                    user.SetStat(GetCooldownId(itemId), DateTime.UtcNow.Ticks);
                }
            }

            // As the final step, invoke the action defined on the item.
            result = item.OnUse.Action(user);

            // If the item broke, invoke that action too.
            if (isBroken)
            {
                // Only invoke breaking if the group is not a booster
                // This is because boosters reference this method when used up
                if (item.GroupId != ItemGroups.Booster)
                    item.OnUse.OnBreak?.Invoke(user);
            }

            return result;
        }

        private static string GetBestUniqueId(ArcadeUser user, string itemId)
        {
            Item item = GetItem(itemId);

            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (!IsUnique(item))
                throw new ArgumentException("The specified item is not unique.");

            // Slim down to all matching item entries of the same item
            var matching = user.Items.Where(x => x.Id == itemId);

            if (matching.Any())
                return "";

            if (item.OnUse?.Durability != null)
            {
                matching = matching.OrderBy(x => x.Data.Durability);
            }

            return matching.First().Id;
        }
    }
}
