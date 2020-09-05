using System;
using System.Collections.Generic;
using System.Linq;
using Arcadia.Graphics;
using Orikivo;

namespace Arcadia
{
    public static class Assets
    {
        public static readonly List<Recipe> Recipes = new List<Recipe>
        {
            new Recipe
            {
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Arcadia.Items.PaletteGlass, 1),
                    new RecipeComponent(Arcadia.Items.PaletteWumpite, 1),
                    new RecipeComponent(Arcadia.Items.ComponentSmearKit, 1)
                },
                Result = new RecipeComponent(Arcadia.Items.PaletteGlossyWumpite, 1)
            },
            new Recipe
            {
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Arcadia.Items.PaletteCrimson, 1),
                    new RecipeComponent(Arcadia.Items.PaletteLemon, 1),
                    new RecipeComponent(Arcadia.Items.ComponentSmearKit, 1)
                },
                Result = new RecipeComponent(Arcadia.Items.PaletteBurntLemon, 1)
            },
            new Recipe
            {
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Arcadia.Items.PaletteLemon, 1),
                    new RecipeComponent(Arcadia.Items.PaletteCrimson, 1),
                    new RecipeComponent(Arcadia.Items.ComponentBlendKit, 1)
                },
                Result = new RecipeComponent(Arcadia.Items.PaletteAmber, 1)
            },
            new Recipe
            {
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Arcadia.Items.PaletteWumpite, 1),
                    new RecipeComponent(Arcadia.Items.PaletteLemon, 1),
                    new RecipeComponent(Arcadia.Items.ComponentBlendKit, 1)
                },
                Result = new RecipeComponent(Arcadia.Items.PaletteChocolate, 1)
            },
            new Recipe
            {
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Arcadia.Items.PaletteCrimson, 1),
                    new RecipeComponent(Arcadia.Items.PaletteGammaGreen, 1),
                    new RecipeComponent(Arcadia.Items.ComponentBlendKit, 1)
                },
                Result = new RecipeComponent(Arcadia.Items.PaletteChocolate, 1)
            },
            new Recipe
            {
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Arcadia.Items.PaletteCrimson, 1),
                    new RecipeComponent(Arcadia.Items.PaletteLemon, 1),
                    new RecipeComponent(Arcadia.Items.PaletteOceanic, 1),
                    new RecipeComponent(Arcadia.Items.ComponentBlendKit, 1)
                },
                Result = new RecipeComponent(Arcadia.Items.PaletteChocolate, 1)
            },
            new Recipe
            {
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Arcadia.Items.PaletteAmber, 1),
                    new RecipeComponent(Arcadia.Items.PaletteOceanic, 1),
                    new RecipeComponent(Arcadia.Items.ComponentBlendKit, 1)
                },
                Result = new RecipeComponent(Arcadia.Items.PaletteChocolate, 1)
            },
            new Recipe
            {
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Arcadia.Items.PaletteCrimson, 1),
                    new RecipeComponent(Arcadia.Items.ComponentNeonKit, 2)
                },
                Result = new RecipeComponent(Arcadia.Items.PaletteTaffy, 1)
            },
            new Recipe
            {
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Arcadia.Items.PaletteLemon, 1),
                    new RecipeComponent(Arcadia.Items.PaletteOceanic, 1),
                    new RecipeComponent(Arcadia.Items.ComponentBlendKit, 1)
                },
                Result = new RecipeComponent(Arcadia.Items.PaletteGammaGreen, 1)
            }
            ,
            new Recipe
            {
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Arcadia.Items.ComponentNeonKit, 8),
                    new RecipeComponent(Arcadia.Items.ComponentBlendKit, 1),
                },
                Result = new RecipeComponent(Arcadia.Items.PalettePolarity, 1)
            }
        };

        public static List<Item> Items =>
            new List<Item>
            {
                new Item
                {
                    Id = Arcadia.Items.InternalSealedItem,
                    GroupId = "$",
                    Icon = "📦",
                    Name = "Sealed Item",
                    Quotes = new List<string>
                    {
                        "A mysterious container with unknown contents."
                    }
                },
                new Item
                {
                    Id = "t_nt",
                    Icon = "🉐",
                    Name = "Name Tag",
                    Rarity = ItemRarity.Uncommon,
                    Tag = ItemTag.Tool,
                    Value = 750,
                    Size = 50
                },
                new Item
                {
                    Id = "t_gc",
                    Icon = "🧺",
                    Name = "Gift Catapult",
                    Summary = "Randomly gifts an account with the specified item.",
                    Rarity = ItemRarity.Rare,
                    Tag = ItemTag.Tool,
                    Value = 1250,
                    Size = 100
                },
                new Item
                {
                    Id = Arcadia.Items.ComponentNeonKit,
                    GroupId = ItemGroups.Component,
                    Name = "Neon Kit",
                    Summary = "A toolkit used to brighten the value of a color.",
                    Quotes = new List<string>
                    {
                        "It glows with a strong vibrant request for a color."
                    },
                    Rarity = ItemRarity.Rare,
                    Tag = ItemTag.Ingredient,
                    Value = 1250,
                    Size = 250
                },
                new Item
                {
                    Id = Arcadia.Items.ComponentDimmerKit,
                    GroupId = ItemGroups.Component,
                    Name = "Dimmer Kit",
                    Summary = "A toolkit used to darken the value of a color.",
                    Quotes = new List<string>
                    {
                        "It absorbs every color surrounding it."
                    },
                    Rarity = ItemRarity.Rare,
                    Tag = ItemTag.Ingredient,
                    Value = 1250,
                    Size = 250
                },
                new Item
                {
                    Id = Arcadia.Items.ComponentSmearKit,
                    GroupId = ItemGroups.Component,
                    Name = "Smear Kit",
                    Summary = "Often used to create a hybrid of your favorite colors.",
                    Quotes = new List<string>
                    {
                        "It swings open to reveal two color slots with a churning device in the center."
                    },
                    Rarity = ItemRarity.Uncommon,
                    Tag = ItemTag.Ingredient,
                    Value = 750,
                    Size = 250
                },
                new Item
                {
                    Id = Arcadia.Items.ComponentBlendKit,
                    GroupId = ItemGroups.Component,
                    Name = "Blend Kit",
                    Summary = "Used to give way to a brand new color from two existing ones.",
                    Quotes = new List<string>
                    {
                        "It pops open to reveal two color slots that point towards a rapidly spinning motor."
                    },
                    Rarity = ItemRarity.Rare,
                    Tag = ItemTag.Ingredient,
                    Value = 1000,
                    Size = 250
                },
                new Item
                {
                    Id = Arcadia.Items.AutomatonGimi,
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
                    OwnLimit = 1
                },
                new Item
                {
                    Id = Arcadia.Items.ToolGiftWrap,
                    Icon = "🎀",
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

                            if (!ItemHelper.Exists(id) && !isUniqueId)
                            {
                                return UsageResult.FromError("Unknown data reference specified.");
                            }

                            string itemId = isUniqueId ? ItemHelper.ItemOf(ctx.User, id).Id : id;
                            string uniqueId = isUniqueId ? id : null;

                            bool isUnique = ItemHelper.IsUnique(itemId);

                            if (isUnique && !isUniqueId)
                                return UsageResult.FromError("This item is marked as unique and must be specified by its unique ID.");

                            ItemData data = ItemHelper.DataOf(ctx.User, itemId, uniqueId);

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

                            return UsageResult.FromSuccess($"> Successfully wrapped {amount:##,0} **{ItemHelper.NameOf(itemId)}**.");
                        }
                    }
                },
                new Item
                {
                    Id = Arcadia.Items.BoosterDebtBlocker,
                    Name = "Debt Blocker",
                    Quotes = new List<string>
                    {
                        "It creates a small shield when near debt."
                    },
                    GroupId = ItemGroups.Booster,
                    Rarity = ItemRarity.Uncommon,
                    Tag = ItemTag.Booster,
                    Value = 525,
                    BypassCriteriaOnTrade = true,
                    Size = 75,
                    Usage = new ItemUsage
                    {
                        Durability = 1,
                        DeleteMode = DeleteMode.Break,
                        Action = delegate(UsageContext ctx)
                        {
                            var booster = new BoostData(Arcadia.Items.BoosterDebtBlocker, BoostType.Debt, -0.2f, TimeSpan.FromHours(12), 20);

                            if (!TryApplyBooster(ctx.User, booster))
                                return UsageResult.FromError("> You already have too many active modifiers.");

                            return UsageResult.FromSuccess("> The **Debt Blocker** opens up, revealing a crystal clear shield that surrounds you.");
                        },
                        OnBreak = user => user.Boosters.Add(new BoostData(BoostType.Debt, 0.1f, 20))
                    },
                    OwnLimit = 2
                },
                new Item
                {
                    Id = Arcadia.Items.BoosterOriteBooster,
                    Name = "Orite Booster",
                    Quotes = new List<string>
                    {
                        "It amplifies the value of Orite when given close exposure."
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
                            var booster = new BoostData(Arcadia.Items.BoosterOriteBooster, BoostType.Money, 0.2f, TimeSpan.FromHours(12), 20);

                            if (!TryApplyBooster(ctx.User, booster))
                                return UsageResult.FromError("> You already have too many active modifiers.");

                            return UsageResult.FromSuccess("> The **Orite Booster** cracks open and infuses with your very well-being.");
                        },
                        OnBreak = user => user.Boosters.Add(new BoostData(BoostType.Debt, 0.1f, 20))
                    },
                    OwnLimit = 2
                },
                new Item
                {
                    Id = Arcadia.Items.PocketLawyer,
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
                                return UsageResult.FromError("> You called for help, but the request remains unanswered. You aren't seen as in need of assistance.");

                            ctx.User.Debt = 0;
                            return UsageResult.FromSuccess("> ⛓️ After a tough fight with **ORS**, **Mr. Pocket** was able to prove your innocence and wipe your debt. You have been freed from the shackles of debt.");
                        }
                    },
                    OwnLimit = 3
                },
                new Item
                {
                    Id = Arcadia.Items.PaletteGammaGreen,
                    Name = "Gamma Green",
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
                    Id = Arcadia.Items.PaletteCrimson,
                    Name = "Crimson",
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
                    Id = Arcadia.Items.PaletteWumpite,
                    Name = "Wumpite",
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
                    Id = Arcadia.Items.PalettePolarity,
                    Name = "Polarity",
                    Quotes = new List<string>
                    {
                        "It reminds you of a desolate atmosphere, now frozen over."
                    },
                    GroupId = ItemGroups.Palette,
                    DeniedHandles = ItemDeny.Buy | ItemDeny.Clone,
                    Tag = ItemTag.Palette | ItemTag.Decorator,
                    Value = 6000,
                    Size = 325,
                    BypassCriteriaOnTrade = true,
                    Rarity =  ItemRarity.Myth,
                    Usage = new ItemUsage
                    {
                        Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, PaletteType.Polarity)),
                        DeleteMode = DeleteMode.Break
                    },
                    OwnLimit = 10
                },
                new Item
                {
                    Id = Arcadia.Items.PaletteGlass,
                    Name = "Glass",
                    Quotes = new List<string>
                    {
                        "It refracts a mixture of light blue to white light."
                    },
                    GroupId = ItemGroups.Palette,
                    Tag = ItemTag.Palette | ItemTag.Decorator,
                    Value = 3000,
                    Size = 75,
                    BypassCriteriaOnTrade = true,
                    Rarity =  ItemRarity.Rare,
                    Usage = new ItemUsage
                    {
                        Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, PaletteType.Glass)),
                        DeleteMode = DeleteMode.Break
                    },
                    OwnLimit = 10
                },
                new Item
                {
                    Id = Arcadia.Items.PaletteLemon,
                    Name = "Lemon",
                    Quotes = new List<string>
                    {
                        "It exudes a wave of citrus in the air."
                    },
                    GroupId = ItemGroups.Palette,
                    Tag = ItemTag.Palette | ItemTag.Decorator,
                    Value = 2500,
                    Size = 75,
                    BypassCriteriaOnTrade = true,
                    Rarity =  ItemRarity.Rare,
                    Usage = new ItemUsage
                    {
                        Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, PaletteType.Lemon)),
                        DeleteMode = DeleteMode.Break
                    },
                    OwnLimit = 10
                },
                new Item
                {
                    Id = Arcadia.Items.PaletteBurntLemon,
                    Name = "Burnt Lemon",
                    Quotes = new List<string>
                    {
                        "The citrus wave it once provided sparks under the embers."
                    },
                    GroupId = ItemGroups.Palette,
                    Tag = ItemTag.Palette | ItemTag.Decorator,
                    DeniedHandles = ItemDeny.Buy | ItemDeny.Clone,
                    Value = 5000,
                    Size = 75,
                    BypassCriteriaOnTrade = true,
                    Rarity =  ItemRarity.Rare,
                    Usage = new ItemUsage
                    {
                        Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, new ComponentPalette(PaletteType.Crimson, PaletteType.Lemon))),
                        DeleteMode = DeleteMode.Break
                    },
                    OwnLimit = 2
                },
                new Item
                {
                    Id = Arcadia.Items.PaletteOceanic,
                    Name = "Oceanic",
                    Quotes = new List<string>
                    {
                        "It crashes down on land, whispering the secrets of the sea."
                    },
                    GroupId = ItemGroups.Palette,
                    Tag = ItemTag.Palette | ItemTag.Decorator,
                    Value = 2500,
                    Size = 150,
                    BypassCriteriaOnTrade = true,
                    Rarity =  ItemRarity.Rare,
                    Usage = new ItemUsage
                    {
                        Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, PaletteType.Oceanic)),
                        DeleteMode = DeleteMode.Break
                    },
                    OwnLimit = 10
                },
                new Item
                {
                    Id = Arcadia.Items.PaletteTaffy,
                    Name = "Taffy",
                    Quotes = new List<string>
                    {
                        "It coats itself in a swirl of flexible sugar."
                    },
                    GroupId = ItemGroups.Palette,
                    DeniedHandles = ItemDeny.Buy | ItemDeny.Clone,
                    Tag = ItemTag.Palette | ItemTag.Decorator,
                    Value = 3500,
                    Size = 150,
                    BypassCriteriaOnTrade = true,
                    Rarity =  ItemRarity.Rare,
                    Usage = new ItemUsage
                    {
                        Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, PaletteType.Taffy)),
                        DeleteMode = DeleteMode.Break
                    },
                    OwnLimit = 5
                },
                new Item
                {
                    Id = Arcadia.Items.PaletteGlossyWumpite,
                    Name = "Glossy Wumpite",
                    Quotes = new List<string>
                    {
                        "It refracts a mixture of light that absorbs the color of a Wumpus."
                    },
                    GroupId = ItemGroups.Palette,
                    Tag = ItemTag.Palette | ItemTag.Decorator,
                    DeniedHandles = ItemDeny.Buy | ItemDeny.Clone,
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
                },
                new Item
                {
                    Id = Arcadia.Items.PaletteGold,
                    GroupId = ItemGroups.Palette,
                    Name = "Gold",
                    Quotes = new List<string>
                    {
                        "It gleams a golden radiant of hope."
                    },
                    Tag = ItemTag.Palette | ItemTag.Decorator,
                    DeniedHandles = ItemDeny.Buy | ItemDeny.Sell | ItemDeny.Clone,
                    Value = 7500,
                    Size = 400,
                    TradeLimit = 0,
                    Rarity = ItemRarity.Desolate,
                    OwnLimit = 1
                },
                new Item
                {
                    Id = Arcadia.Items.PaletteAmber,
                    GroupId = ItemGroups.Palette,
                    Name = "Amber",
                    Quotes = new List<string>
                    {
                        "It preserves the life form of something hidden in the past."
                    },
                    Tag = ItemTag.Palette | ItemTag.Decorator,
                    DeniedHandles = ItemDeny.Buy | ItemDeny.Clone,
                    Value = 5000,
                    Size = 300,
                    TradeLimit = 0,
                    Rarity = ItemRarity.Myth,
                    Usage = new ItemUsage
                    {
                        Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, new ComponentPalette(PaletteType.Amber))),
                        DeleteMode = DeleteMode.Break
                    },
                    OwnLimit = 2
                },
                new Item
                {
                    Id = Arcadia.Items.PaletteChocolate,
                    GroupId = ItemGroups.Palette,
                    Name = "Chocolate",
                    Quotes = new List<string>
                    {
                        "It reminds you of a simpler time, where sweets meant everything."
                    },
                    Tag = ItemTag.Palette | ItemTag.Decorator,
                    DeniedHandles = ItemDeny.Buy | ItemDeny.Clone,
                    Value = 3000,
                    Size = 350,
                    TradeLimit = 0,
                    Rarity = ItemRarity.Rare,
                    Usage = new ItemUsage
                    {
                        Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, new ComponentPalette(PaletteType.Chocolate))),
                        DeleteMode = DeleteMode.Break
                    },
                    OwnLimit = 2
                },
                new Item
                {
                    Id = "ap_mc",
                    GroupId = ItemGroups.AccessPass,
                    Name = "Midonian Casino",
                    Quotes = new List<string>
                    {
                        "It absorbs the addiction of desperate gamblers."
                    },
                    Tag = ItemTag.Pass,
                    DeniedHandles = ItemDeny.Buy | ItemDeny.Sell | ItemDeny.Clone,
                    Rarity = ItemRarity.Desolate,
                    OwnLimit = 1,
                    TradeLimit = 0
                },
                new Item
                {
                    Id = "cf_ft",
                    Name = "Foxtrot",
                    Quotes = new List<string>
                    {
                        "It represents a strongly typed font face with a clean design."
                    },
                    Tag = ItemTag.Font | ItemTag.Decorator
                }
            };

        public static readonly List<ItemGroup> Groups = new List<ItemGroup>
        {
            new ItemGroup
            {
                ShortId = "c",
                Id = "component",
                Name = "Component",
                Prefix = "Component: ",
                Summary = "Helpful building blocks for the creation of new items."
            },
            new ItemGroup
            {
                Id = "$",
                Name = "Internal",
                Summary = "Items that do not exist as an actual obtainable object."
            },
            new ItemGroup
            {
                Id = "tool",
                Name = "Tool",
                Summary = "Items that are used on other instances of items."
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
                Icon = new Icon
                {
                    Fallback = Icons.Palette,
                    Aliases = new List<string>
                    {
                        ":test_tube:"
                    }
                },
                Name = "Palette",
                Prefix = "Card Palette: ",
                Summary = "Modifies the color scheme that is displayed on a card."
            },
            new ItemGroup
            {
                Id = "font",
                Icon = Icons.Palette,
                Name = "Font",
                Prefix = "Card Font: ",
                Summary = "Modifies the text font that is displayed on a card."
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
                Id = "access_pass",
                Name = "Access Pass",
                Prefix = "Access Pass: ",
                Summary = "Provides access to undisclosed entries."
            },
            new ItemGroup
            {
                Id = "automaton",
                Icon = "⚙️",
                Name = "Automaton",
                Prefix = "Automaton: ",
                Summary = "Grants the ability to automate specific actions."
            }
        };

        private static string SetOrSwapPalette(ArcadeUser user, ComponentPalette palette)
        {
            if (user.Card.Palette == palette)
                return Format.Warning($"You already have **{ItemHelper.NameFor(palette.Primary, palette.Secondary)}** equipped on your **Card Palette**.");

            string result = $"> 📟 Equipped **{ItemHelper.NameFor(palette.Primary, palette.Secondary)}** to your **Card Palette**.";
            if (user.Card.Palette.Primary != PaletteType.Default)
            {
                ItemHelper.GiveItem(user, ItemHelper.IdFor(user.Card.Palette.Primary, user.Card.Palette.Secondary));
                result = $"📟 Swapped out **{ItemHelper.NameFor(user.Card.Palette.Primary)}** with **{ItemHelper.NameFor(palette.Primary, palette.Secondary)}** for your **Card Palette**.";
            }

            ItemHelper.TakeItem(user, ItemHelper.IdFor(palette.Primary, palette.Secondary));
            user.Card.Palette = palette;
            return result;
        }

        private static bool TryApplyBooster(ArcadeUser user, BoostData booster)
        {
            if (Var.GetOrSet(user, Vars.BoosterLimit, 1) - user.Boosters.Count <= 0)
                return false;

            if (booster == null)
                throw new Exception("Expected a booster data to be specified but returned null");

            user.Boosters.Add(booster);
            return true;
        }
    }
}