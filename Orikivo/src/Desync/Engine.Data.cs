using Orikivo.Drawing;
using Orikivo.Drawing.Graphics2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

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
        {
            Id = "world0",
            Name = "Okonos",
            Perimeter = new Vector2(512, 512),
            Scale = 1.0f, // 1 pixel in the world would take 1 minute at default ratio.
            // sectors are 1/4 of the distance ratio
            Sectors = new List<Sector>
            {
                new Sector(new Vector2(16, 356), SectorScale.Small)
                {
                    Id = "sector0",
                    Name = "Sector 0",
                    Exterior = new Sprite(@"..\assets\exterior\exterior_test.png"),
                    Entrance = new Vector2(0, 16),
                    Structures = new List<Structure>
                    {
                        new Structure
                        {
                            Id = "str_decor",
                            Name = "The Devoid Fountain",
                            Perimeter = new RegionF(35, 23, 1, 1),
                            Type = StructureType.Decoration
                        },
                        new Structure
                        {
                            Id = "str_tent",
                            Name = "Tent",
                            Perimeter = new RegionF(63, 63, 1, 1),
                            Type = StructureType.Tent
                        }
                    },
                    Areas = new List<Area>
                    {
                        new Area
                        {
                            Id = "area0",
                            Name = "Area A",
                            Perimeter = new RegionF(0, 0, 32, 32),
                            Entrances = new List<Vector2>
                            {
                                new Vector2(32, 15)
                            },
                            Constructs = new List<Construct>
                            {
                                new Market
                                {
                                    Id = "mk0",
                                    Name = "Market A1",
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
                                            Personality = new Personality
                                            {
                                                Mind = MindType.Extravert,
                                                Energy = EnergyType.Intuitive,
                                                Nature = NatureType.Thinking,
                                                Tactics = TacticType.Judging,
                                                Identity = IdentityType.Assertive
                                            },
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
                                    },
                                    Npcs = new List<Character>
                                    {
                                        new Character
                                        {
                                            Id = "theta",
                                            Name = "Theta",
                                            Personality = new Personality
                                            {
                                                Mind = MindType.Extravert,
                                                Energy = EnergyType.Intuitive,
                                                Nature = NatureType.Thinking,
                                                Tactics = TacticType.Judging,
                                                Identity = IdentityType.Assertive
                                            }
                                        },
                                    }
                                },
                                new Construct
                                {
                                    Id = "ctr0",
                                    Name = "Construct A1",
                                    Npcs = new List<Character>
                                    {
                                        new Character
                                        {
                                            Id = "alpha",
                                            Name = "Alpha",
                                            Personality = new Personality
                                            {
                                                Mind = MindType.Extravert,
                                                Energy = EnergyType.Intuitive,
                                                Nature = NatureType.Thinking,
                                                Tactics = TacticType.Judging,
                                                Identity = IdentityType.Assertive
                                            }
                                        },
                                        new Character
                                        {
                                            Id = "beta",
                                            Name = "Beta",
                                            Personality = new Personality
                                            {
                                                Mind = MindType.Extravert,
                                                Energy = EnergyType.Intuitive,
                                                Nature = NatureType.Thinking,
                                                Tactics = TacticType.Judging,
                                                Identity = IdentityType.Assertive
                                            }
                                        }
                                    }
                                },
                                new Highrise
                                {
                                    Id = "construct0",
                                    Name = "Construct G1",
                                    Floors = new List<Floor>
                                    {
                                        new Floor
                                        {
                                            Id = "floor0",
                                            Index = 0,
                                            Npcs = new List<Character>
                                            {
                                                new Character
                                                {
                                                    Id = "npc0",
                                                    Name = "NPC A1.1",
                                                    Personality = new Personality
                                                    {
                                                        Mind = MindType.Extravert,
                                                        Energy = EnergyType.Intuitive,
                                                        Nature = NatureType.Thinking,
                                                        Tactics = TacticType.Judging,
                                                        Identity = IdentityType.Assertive
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            Npcs = new List<Character>
                            {
                                new Character
                                {
                                    Id = "guide",
                                    Name = "Area Guide",
                                    Personality = new Personality
                                    {
                                        Mind = MindType.Extravert,
                                        Energy = EnergyType.Intuitive,
                                        Nature = NatureType.Thinking,
                                        Tactics = TacticType.Judging,
                                        Identity = IdentityType.Assertive
                                    }
                                }
                            }
                        },
                        new Area
                        {
                            Id = "area1",
                            Name = "Area B",
                            Perimeter = new RegionF(47, 47, 16, 16),
                            Entrances = new List<Vector2>
                            {
                                new Vector2(47, 55)
                            },
                            Constructs = new List<Construct>
                            {
                                new Highrise
                                {
                                    Id = "construct1",
                                    Name = "Construct B1",
                                    Floors = new List<Floor>
                                    {
                                        new Floor
                                        {
                                            Index = 0,
                                            Npcs = new List<Character>
                                            {
                                                new Character
                                                {
                                                    Id = "npc1",
                                                    Name = "NPC B1.1",
                                                    Personality = new Personality
                                                    {
                                                        Mind = MindType.Extravert,
                                                        Energy = EnergyType.Intuitive,
                                                        Nature = NatureType.Thinking,
                                                        Tactics = TacticType.Judging,
                                                        Identity = IdentityType.Assertive
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            Npcs = new List<Character>
                            {

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

        // TODO: Compress Dialogue to be stored within a .json file or something easier to make
        // loading them easier.
        public static Dictionary<string, DialoguePool> Dialogue => new Dictionary<string, DialoguePool>
        {
            ["test"] = new DialoguePool
            {
                Generic = true,
                Entry = "Hello.",
                Exit = "Sorry, but I have to go.",
                Timeout = "Sorry, but I have to go.",
                Dialogue = new List<Dialogue>
                {
                    new Dialogue
                    {
                        Type = DialogType.Initial,
                        Tone = DialogTone.Neutral,
                        Id = "1",
                        Entries = new List<string>
                        {
                            "How are you?"
                        },
                        ReplyIds = new List<string>
                        {
                            "2"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogType.Answer,
                        Tone = DialogTone.Neutral,
                        Id = "2",
                        Entries = new List<string>
                        {
                            "I'm okay. Thank you for asking."
                        },
                        ReplyIds = new List<string>
                        {
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogType.Initial,
                        Tone = DialogTone.Neutral,
                        Id = "3",
                        Entries = new List<string>
                        {
                            "how much wood can a woodchuck chuck if a woodchuck could chuck wood"
                        },
                        ReplyIds = new List<string>
                        {
                            "4"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogType.Reply,
                        Tone = DialogTone.Confused,
                        Id = "4",
                        Entries = new List<string>
                        {
                            "what does that even mean"
                        },
                        ReplyIds = new List<string>
                        {
                            "5"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogType.Reply,
                        Tone = DialogTone.Neutral,
                        Id = "5",
                        Entries = new List<string>
                        {
                            "/shrug"
                        },
                        ReplyIds = new List<string>
                        {
                            "6"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogType.Answer,
                        Tone = DialogTone.Neutral,
                        Id = "6",
                        Entries = new List<string>
                        {
                            "ok boomer"
                        },
                        ReplyIds = new List<string>
                        {
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogType.Initial,
                        Tone = DialogTone.Neutral,
                        Id = "7",
                        Entries = new List<string>
                        {
                            "Do you like water?"
                        },
                        ReplyIds = new List<string>
                        {
                            "8",
                            "9"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogType.Answer,
                        Tone = DialogTone.Confused,
                        Id = "8",
                        Entries = new List<string>
                        {
                            "Considering we need it to live, yeah. Anything else?"
                        },
                        ReplyIds = new List<string>
                        {
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogType.Question,
                        Tone = DialogTone.Happy,
                        Id = "9",
                        Entries = new List<string>
                        {
                            "Water is pretty epic. What about you?"
                        },
                        ReplyIds = new List<string>
                        {
                            "10",
                            "11"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogType.Reply,
                        Tone = DialogTone.Neutral,
                        Id = "10",
                        Entries = new List<string>
                        {
                            "it be vibin"
                        },
                        ReplyIds = new List<string>
                        {
                            "12"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogType.Reply,
                        Tone = DialogTone.Neutral,
                        Id = "11",
                        Entries = new List<string>
                        {
                            "I do not like it. Too water-y."
                        },
                        ReplyIds = new List<string>
                        {
                            "6"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogType.Answer,
                        Tone = DialogTone.Confused,
                        Id = "12",
                        Entries = new List<string>
                        {
                            "..."
                        },
                        ReplyIds = new List<string>
                        {
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogType.Initial,
                        Tone = DialogTone.Neutral,
                        Id = "13",
                        Entries = new List<string>
                        {
                            "Anything new?"
                        },
                        ReplyIds = new List<string>
                        {
                            "14",
                            "19"
                        }
                    },
                    new Dialogue
                    {
                        Type = DialogType.Reply,
                        Tone = DialogTone.Neutral,
                        Id = "14",
                        Entries = new List<string>
                        {
                            "I met up with someone called a 'Pocket Lawyer'. They were the size of my palm, but got me out of debt."
                        },
                        ReplyIds = new List<string>
                        {
                            "15"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogType.Reply,
                        Tone = DialogTone.Neutral,
                        Id = "15",
                        Entries = new List<string>
                        {
                            "woah wait what?"
                        },
                        ReplyIds = new List<string>
                        {
                            "16"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogType.Reply,
                        Tone = DialogTone.Happy,
                        Id = "16",
                        Entries = new List<string>
                        {
                            "Yeah, it was bizarre. ORS got nothin' on me now."
                        },
                        ReplyIds = new List<string>
                        {
                            "17"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogType.Reply,
                        Tone = DialogTone.Neutral,
                        Id = "17",
                        Entries = new List<string>
                        {
                            "ORS?"
                        },
                        ReplyIds = new List<string>
                        {
                            "18"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogType.Answer,
                        Tone = DialogTone.Neutral,
                        Id = "18",
                        Entries = new List<string>
                        {
                            "I'd prefer if we don't delve into the topic. Anything else on your mind?"
                        },
                        ReplyIds = new List<string>
                        {
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogType.Reply,
                        Tone = DialogTone.Shocked,
                        Id = "19",
                        Entries = new List<string>
                        {
                            "Did you know there's so much more to this world than the sector we live in? I hope I can go out there one day."
                        },
                        ReplyIds = new List<string>
                        {
                            "20"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogType.Reply,
                        Tone = DialogTone.Neutral,
                        Id = "20",
                        Entries = new List<string>
                        {
                            "I wish you luck."
                        },
                        ReplyIds = new List<string>
                        {
                            "21"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogType.Answer,
                        Tone = DialogTone.Happy,
                        Id = "21",
                        Entries = new List<string>
                        {
                            "Thanks. Anything else you wanted to talk about?"
                        },
                        ReplyIds = new List<string>
                        {
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogType.Initial,
                        Tone = DialogTone.Neutral,
                        Id = "22",
                        Entries = new List<string>
                        {
                            "I gotta go."
                        },
                        ReplyIds = new List<string>
                        {
                            "23"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogType.End,
                        Tone = DialogTone.Neutral,
                        Id = "23",
                        Entries = new List<string>
                        {
                            "See ya."
                        },
                        ReplyIds = new List<string>
                        {
                        }
                    }

                }
            },
            ["rand0"] = new DialoguePool
            {
                Generic = true,
                Entry = "Yo.",
                Timeout = "Gotta bounce.",
                Dialogue = new List<Dialogue>
                {
                    new Dialogue
                    {
                        Id = "0",
                        Type = DialogType.Initial,
                        Tone = DialogTone.Neutral,
                        Entries = new List<string>
                        {
                            "What the heck is this?"
                        },
                        ReplyIds = new List<string>
                        {
                            "3"
                        }
                    },
                    new Dialogue
                    {
                        Id = "3",
                        Type = DialogType.Answer,
                        Tone = DialogTone.Neutral,
                        Entries = new List<string>
                        {
                            "Nobody knows. Why'd you even walk up to me in the first place?"
                        },
                        ReplyIds = new List<string>
                        {
                        }
                    },
                    new Dialogue
                    {
                        Id = "1",
                        Type = DialogType.Initial,
                        Tone = DialogTone.Neutral,
                        Entries = new List<string>
                        {
                            "I'm outta here."
                        },
                        ReplyIds = new List<string>
                        {
                            "2"
                        }
                    },
                    new Dialogue
                    {
                        Id = "2",
                        Type = DialogType.End,
                        Tone = DialogTone.Happy,
                        Entries = new List<string>
                        {
                            "See you around!"
                        },
                        ReplyIds = new List<string>
                        {

                        }
                    }
                },
                Exit = "Peace."
            },
            ["rand1"] = new DialoguePool
            {
                Generic = true,
                Entry = "Yo.",
                Timeout = "Gotta bounce.",
                Dialogue = new List<Dialogue>
                {
                    new Dialogue
                    {
                        Id = "0",
                        Type = DialogType.Initial,
                        Tone = DialogTone.Neutral,
                        Entries = new List<string>
                        {
                            "Who are you?"
                        },
                        ReplyIds = new List<string>
                        {
                            "3"
                        }
                    },
                    new Dialogue
                    {
                        Id = "3",
                        Type = DialogType.Answer,
                        Tone = DialogTone.Neutral,
                        Entries = new List<string>
                        {
                            "Does it matter?"
                        },
                        ReplyIds = new List<string>
                        {
                        }
                    },
                    new Dialogue
                    {
                        Id = "1",
                        Type = DialogType.Initial,
                        Tone = DialogTone.Neutral,
                        Entries = new List<string>
                        {
                            "I need to go."
                        },
                        ReplyIds = new List<string>
                        {
                            "2"
                        }
                    },
                    new Dialogue
                    {
                        Id = "2",
                        Type = DialogType.End,
                        Tone = DialogTone.Neutral,
                        Entries = new List<string>
                        {
                            "Goodbye."
                        },
                        ReplyIds = new List<string>
                        {

                        }
                    }
                },
                Exit = "Peace."
            }
        };

        public static readonly float TimePerPixel = 60.0f;

    }
}
