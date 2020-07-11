using Orikivo.Drawing;
using System;
using System.Collections.Generic;

namespace Orikivo.Desync
{
    // TODO: Once the methods are figured out, make this class non-static, from which is loaded from a cache.
    /// <summary>
    /// Represents the central process manager for a DesyncClient.
    /// </summary>
    public static partial class Engine
    {
        // TODO: Transfer to .JSON file.
        public static World World => new World()
        {   // Okonos
            Id = "world0",
            Name = "Okonos",
            Perimeter = new Vector2(512, 512),
            Scale = 1.0f, // 1 pixel in the world would take 1 minute at default ratio.
            // sectors are 1/4 of the distance ratio
            Sectors = new List<Sector>
            {
                new Sector("sector0", "Sector 0", new Vector2(16, 356), SectorScale.Small)
                {
                    Exterior = new Sprite(@"..\assets\exterior\exterior_test.png"),
                    Entrance = new Vector2(0, 16),
                    Structures = new List<Structure>
                    {
                        new Structure
                        {
                            Id = "str_decor",
                            Name = "The Devoid Fountain",
                            Shape = new RegionF(35, 23, 1, 1),
                            Type = StructureType.Decoration
                        },
                        new Structure
                        {
                            Id = "str_tent",
                            Name = "Tent",
                            Shape = new RegionF(63, 63, 1, 1),
                            Type = StructureType.Tent
                        }
                    },
                    Areas = new List<Area>
                    {
                        new Area
                        {
                            Id = "area0",
                            Name = "Area A",
                            Shape = new RegionF(0, 0, 32, 32),
                            Entrances = new List<Vector2>
                            {
                                new Vector2(32, 15)
                            },
                            Constructs = new List<Construct>
                            {
                                new Market("mk0", "Market A1")
                                {
                                    CanBuyFrom = true,
                                    CanSellFrom = true,
                                    SellRate = 0.7f,
                                    Tag = ConstructType.Market,
                                    Catalog = new CatalogGenerator
                                    {
                                        Size = 4,
                                        Dimension = ItemDimension.Physical,
                                        MaxStack = 2
                                    },
                                    Vendors = new List<Vendor>
                                    {
                                        new Vendor
                                        {
                                            Id = "vendor0",
                                            Name = "Vendor",
                                            // 0b MIND ENERGY NATURE TACTICS IDENTITY
                                            // 0b    1      1      0       0        0
                                            Personality = new Personality(0b11000),
                                            Schedule = new Schedule
                                            {
                                                Shifts = new List<Shift>
                                                {
                                                    new Shift(DayOfWeek.Saturday, 0, 0, TimeSpan.FromHours(22)),
                                                    new Shift(DayOfWeek.Sunday, 0, 0, TimeSpan.FromHours(16)),
                                                    new Shift(DayOfWeek.Tuesday, 0, 0, TimeSpan.FromHours(22)),
                                                    new Shift(DayOfWeek.Wednesday, 0, 0, TimeSpan.FromHours(22))
                                                }
                                            }
                                        }
                                    }
                                },
                                new Construct("ctr0", "Construct A1"),
                                new Highrise("construct0", "Construct G1")
                                {
                                    Floors = new List<Floor>
                                    {
                                        new Floor(0, "floor0")
                                    }
                                }
                            }
                        },
                        new Area
                        {
                            Id = "area1",
                            Name = "Area B",
                            Shape = new RegionF(47, 47, 16, 16),
                            Entrances = new List<Vector2>
                            {
                                new Vector2(47, 55)
                            },
                            Constructs = new List<Construct>
                            {
                                new Highrise("construct1", "Construct B1")
                                {
                                    Floors = new List<Floor>
                                    {
                                        new Floor(0)
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        public static Dictionary<string, Item> Items => new Dictionary<string, Item>
        {
            ["test_item_0"] = new Item
            {
                Id = "test_item_0",
                Name = "Test Item 0",
                Summary = "This is a test item.",
                Quotes = new List<string>
                {
                    "This is a quote 1.",
                    "This is a quote 2."
                },
                Value = 2,
                CanBuy = true,
                CanSell = true,
                Rarity = ItemRarity.Common,
                Dimension = ItemDimension.Physical,
            },
            ["test_item_2"] = new Item
            {
                Id = "test_item_2",
                Name = "Test Item 2",
                Summary = "This is a test item.",
                Quotes = new List<string>
                {
                    "This is a quote 1.",
                    "This is a quote 2."
                },
                Value = 4,
                CanBuy = true,
                CanSell = true,
                Rarity = ItemRarity.Common,
                Dimension = ItemDimension.Physical,
            },
            ["test_item_1"] = new Item
            {
                Id = "test_item_1",
                Name = "Test Item 1",
                Summary = "This is a test item.",
                Quotes = new List<string>
                {
                    "This is a quote 1.",
                    "This is a quote 2."
                },
                Value = 3,
                CanBuy = true,
                CanSell = true,
                Rarity = ItemRarity.Common,
                Dimension = ItemDimension.Physical,
            },
            ["pocket_lawyer"] = new Item
            {
                Id = "pocket_lawyer",
                Name = "Pocket Lawyer",
                Summary = "Constantly puts up with ORS to keep you safe.",
                Quotes = new List<string>
                {
                    "You'll get the chance to dispute in court, you'll see.",
                    "ORS doesn't stand a chance."
                },
                BypassCriteriaOnGift = true,
                GiftLimit = 1,
                Rarity = ItemRarity.Common,
                Tag = ItemType.Summon,
                Dimension = ItemDimension.Digital,
                ToOwn = u => u.Debt >= 1000,
                Value = 40,
                CanBuy = true,
                CanSell = false,
                Action = new ItemAction
                {
                    Durability = 1,
                    DeleteOnBreak = true,
                    Cooldown = TimeSpan.FromHours(24),
                    OnUse = u => u.Debt = 0
                }
            }
        };

        public static Dictionary<string, Merit> Merits => new Dictionary<string, Merit>
        {
            ["test"] = new Merit
            {
                Id = "test",
                Criteria = x => x.GetStat("times_cried") >= 1,
                Name = "Shedding Tears",
                Summary = "Cry.",
                Group = MeritGroup.Misc,
                Reward = new Reward
                {
                    Money = 10
                }
            }
        };

        public static Dictionary<string, Claimable> Claimables => new Dictionary<string, Claimable>();

        public static Dictionary<string, Booster> Boosters => new Dictionary<string, Booster>();

        public static Dictionary<string, DialogTree> Dialogs => new Dictionary<string, DialogTree>
        {
            ["test"] = new DialogTree("test", true)
            {
                OnTimeout = "Sorry, but I have to go.",
                OnUnavailable = "Look, something came up. I'll talk later.",
                Branches = new List<DialogBranch>
                {
                    new DialogBranch("branch0", DialogUsage.Always)
                    {
                        Values = new List<Dialog>
                        {
                            new Dialog("1", DialogType.Initial, DialogTone.Neutral, "How are you?")
                            { ReplyIds = new List<string>{ "2" } },

                            new Dialog("2", DialogType.Answer, DialogTone.Neutral, "I'm okay. Thank you for asking."),

                            new Dialog("3", DialogType.Initial, DialogTone.Neutral, "How much wood can a woodchuck chuck if a woodchuck could chuck wood?")
                            { ReplyIds = new List<string>{ "4" } },

                            new Dialog("4", DialogType.Reply, DialogTone.Confused, "What does that even mean?")
                            { ReplyIds = new List<string>{ "5" } },

                            new Dialog("5", DialogType.Reply, DialogTone.Neutral, "/shrug")
                            { ReplyIds = new List<string> { "6" } },

                            new Dialog("6", DialogType.Answer, DialogTone.Neutral, "Yeah. Okay, boomer."),

                            new Dialog("7", DialogType.Initial, DialogTone.Neutral, "Do you like water?")
                            { ReplyIds = new List<string> { "8", "9" } },

                            new Dialog("8", DialogType.Answer, DialogTone.Confused, "Considering that we need it to live, yeah. Anything else?"),

                            new Dialog("9", DialogType.Question, DialogTone.Happy, "Water is pretty epic. What about you?")
                            { ReplyIds = new List<string> { "10", "11" } },

                            new Dialog("10", DialogType.Reply, DialogTone.Neutral, "It really do be vibin'.")
                            { ReplyIds = new List<string> { "12" } },

                            new Dialog("11", DialogType.Reply, DialogTone.Neutral, "I don't think I do. It's too water-y.")
                            { ReplyIds = new List<string> { "6" } },

                            new Dialog("12", DialogType.Answer, DialogTone.Confused, "..."),

                            new Dialog("13", DialogType.Initial, DialogTone.Neutral, "Anything new?")
                            { ReplyIds = new List<string> { "14", "19" } },

                            new Dialog("14", DialogType.Reply, DialogTone.Neutral,
                                new DialogEntry("I met up with someone that calls themselves a 'pocket lawyer'.",
                                    "They were the size of my palm, but the debt that they got me out of was insane."))
                            { ReplyIds = new List<string> { "15" } },

                            new Dialog("15", DialogType.Reply, DialogTone.Neutral, "Woah... Wait, what?")
                            { ReplyIds = new List<string> { "16" } },

                            new Dialog("16", DialogType.Reply, DialogTone.Happy, "Yeah, it was bizzare. ORS has no trace of me as of now!")
                            { ReplyIds = new List<string> { "17" } },

                            new Dialog("17", DialogType.Reply, DialogTone.Neutral, "What's ORS?")
                            { ReplyIds = new List<string> { "18" } },

                            new Dialog("18", DialogType.Answer, DialogTone.Neutral, "Uh... I'd prefer if we don't talk about them. Anything else on your mind?"),

                            new Dialog("19", DialogType.Reply, DialogTone.Shocked,
                                new DialogEntry("There's so much more to this world than the sector we live in.",
                                    "I hope I can go out there one day."))
                            { ReplyIds = new List<string> { "20" } },

                            new Dialog("20", DialogType.Reply, DialogTone.Neutral, "I wish you luck.")
                            { ReplyIds = new List<string> { "21" } },

                            new Dialog("21", DialogType.Answer, DialogTone.Happy, "Thanks. Anything else?"),

                            new Dialog("22", DialogType.Initial, DialogTone.Neutral, "I gotta go.")
                            { ReplyIds = new List<string> { "23" } },

                            new Dialog("23", DialogType.End, DialogTone.Neutral, "See ya.")
                        }
                    }
                }
            },
            ["rand0"] = new DialogTree("rand0", true)
            {
                OnTimeout = "Gotta bounce.",
                OnUnavailable = "Gotta bounce.",
                Branches = new List<DialogBranch>
                {
                    new DialogBranch("r0b0", DialogUsage.Always)
                    {
                        Values = new List<Dialog>
                        {
                            new Dialog("0", DialogType.Initial, DialogTone.Neutral, "What the heck is this?")
                            { ReplyIds = new List<string> { "3" } },

                            new Dialog("1", DialogType.Initial, DialogTone.Neutral, "I'm outta here.")
                            { ReplyIds = new List<string> { "2" } },

                            new Dialog("2", DialogType.End, DialogTone.Neutral, "See ya."),

                            new Dialog("3", DialogType.Answer, DialogTone.Shocked, "Nobody knows. Why'd you even walk up to me in the first place?")
                        }
                    }
                }
            },
            ["rand1"] = new DialogTree("rand1", true)
            {
                OnTimeout = "Peace.",
                OnUnavailable = "Peace.",
                Branches = new List<DialogBranch>
                {
                    new DialogBranch("r0b0", DialogUsage.Always)
                    {
                        Values = new List<Dialog>
                        {
                            new Dialog("0", DialogType.Initial, DialogTone.Neutral, "Who are you?")
                            { ReplyIds = new List<string> { "3" } },

                            new Dialog("1", DialogType.Initial, DialogTone.Neutral, "I need to go.")
                            { ReplyIds = new List<string> { "2" } },

                            new Dialog("2", DialogType.End, DialogTone.Neutral, "Goodbye."),

                            new Dialog("3", DialogType.Answer, DialogTone.Neutral, "Does it matter?")
                        }
                    }
                }
            }
        };

        public static Dictionary<string, Character> Characters => new Dictionary<string, Character>
        {
            ["foxtrot"] = new Character
            {
                Id = "foxtrot",
                Name = "Foxtrot",
                Personality = new Personality(0b11000),
                DefaultLocation = new Locator("", "sector0", 32, 32)
            },
            ["theta"] = new Character
            {
                Id = "theta",
                Name = "Theta",
                Personality = new Personality(0b11000),
                DefaultLocation = new Locator("", "mk0", 0, 0)
            },
            ["alpha"] = new Character
            {
                Id = "alpha",
                Name = "Alpha",
                Personality = new Personality(0b11000),
                DefaultLocation = new Locator("", "area1", 0, 0)
            },
            ["beta"] = new Character
            {
                Id = "beta",
                Name = "Beta",
                Personality = new Personality(0b11000),
                DefaultLocation = new Locator("", "ctr0", 0, 0)
            },
            ["npc0"] = new Character
            {
                Id = "npc0",
                Name = "NPC A1.1",
                Personality = new Personality(0b11000),
                DefaultLocation = new Locator("", "construct1", 0, 0)
            },
            ["guide"] = new Character
            {
                Id = "guide",
                Name = "Area Guide",
                Personality = new Personality(0b11000),
                DefaultLocation = new Locator("", "area0", 0, 0)
            },
            ["npc1"] = new Character
            {
                Id = "npc1",
                Name = "NPC B1.1",
                Personality = new Personality(0b11000),
                DefaultLocation = new Locator("", "construct0", 0, 0)
            }
        };

        public static readonly float TimePerPixel = 60.0f;

    }
}
