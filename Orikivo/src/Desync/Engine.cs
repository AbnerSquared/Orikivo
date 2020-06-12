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
    public static class Engine
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
                                    Npcs = new List<Npc>
                                    {
                                        new Npc
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
                                    Npcs = new List<Npc>
                                    {
                                        new Npc
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
                                        new Npc
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
                                            Npcs = new List<Npc>
                                            {
                                                new Npc
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
                            Npcs = new List<Npc>
                            {
                                new Npc
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
                                            Npcs = new List<Npc>
                                            {
                                                new Npc
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
                            Npcs = new List<Npc>
                            {

                            }
                        }
                    }
                }
            }
        };

        public static void Initialize(User user)
        {
            var location = World.Find("area0");

            if (location == null)
                throw new ArgumentException("The specified initial location ID does not point to a location.");

            user.Husk = new Husk(location.GetLocator());
            user.Brain.SetFlag(DesyncFlags.Initialized);
        }

        public static byte[] CompressMap(Grid<bool> data)
        {
            // amount of bytes that need to be stored
            int count = (int)Math.Ceiling(((double)data.Count / (double)8));

            int i = 0;
            int b = 0;

            byte[] bytes = new byte[count];
            bool[] bits = new bool[8];
            foreach (bool bit in data)
            {
                bits[i] = bit;
                i++;

                if (i >= 8)
                {
                    bytes[b] = bits.ToByte();
                    bits = new bool[8];
                    b++;
                    i = 0;
                }
            }

            if (i != 0)
            {
                for (int u = i; u < 8; u++)
                    bits[u] = false;

                bytes[b] = bits.ToByte();
            }

            return bytes;
        }

        public static Grid<bool> DecompressMap(int width, int height, byte[] data)
        {
            Grid<bool> progress = new Grid<bool>(width, height, false);
            int i = 0;
            foreach (byte fragment in data)
            {
                foreach (bool bit in fragment.GetBits())
                {
                    progress[i] = bit;
                    i++;

                    if (i >= progress.Count)
                        break;
                }
            }

            return progress;
        }

        // TODO: Use Orikivo.Desync.Map instead.
        public static Bitmap GetMap(string id, HuskBrain brain, GammaPalette palette)
        {
            if (!(World.Id == id) && !World.Sectors.Any(x => x.Id == id))
                throw new ArgumentException("The specified ID does not exists for any Sector or World.");

            if (!brain.Maps.ContainsKey(id))
                brain.Maps[id] = new byte[] { };

            Grid<bool> progress = GetMapData(id, brain.Maps[id]);

            if (World.Id == id)
            {
                Drawable map = new Drawable(World.Map.Width, World.Map.Height);
                map.Palette = palette;
                map.AddLayer(new BitmapLayer { Source = World.Map.GetImage() });
                map.AddLayer(new BitmapLayer { Source = GraphicsUtils.CreateToggledBitmap(progress.Values, Color.Transparent, GammaPalette.Default[Gamma.Min]) });

                return map.BuildAndDispose();
            }

            if (World.Sectors.Any(x => x.Id == id))
            {
                Sector sector = World.GetSector(id);

                Drawable map = new Drawable(World.Map.Width, World.Map.Height);
                map.Palette = palette;
                map.AddLayer(new BitmapLayer { Source = World.Map.GetImage() });
                map.AddLayer(new BitmapLayer { Source = GraphicsUtils.CreateToggledBitmap(progress.Values, Color.Transparent, GammaPalette.Default[Gamma.Min]) });


                return map.BuildAndDispose();
            }

            throw new ArgumentException("The specified ID does not exists for any Sector or World.");
        }

        public static Grid<bool> GetMapData(string id, byte[] data)
        {
            if (World.Id == id)
            {
                if (World.Map != null)
                {
                    return DecompressMap(World.Map.Width, World.Map.Height, data);
                }
            }

            if (World.Sectors.Any(x => x.Id == id))
            {
                Sector sector = World.GetSector(id);

                if (sector.Map != null)
                    return DecompressMap(sector.Map.Width, sector.Map.Height, data);
            }

            throw new ArgumentException("The specified ID does not exists for any Sector or World.");
        }

        // TODO: Finish this. This should make imagining the visual mapping of a world much easier.
        // and allows you to see how the constructor for regions and stuff works.
        public static Bitmap DebugDraw(Location location)
        {
            throw new NotImplementedException("Incomplete.");
        }

        public static Grid<Color> DebugDraw(Sector sector, Husk husk)
        {
            var canvas = new Canvas((int)MathF.Floor(sector.Perimeter.Width),
                (int)MathF.Floor(sector.Perimeter.Height),
                GammaPalette.GammaGreen[Gamma.Min]);

            canvas.DrawCircle((int)MathF.Floor(husk.Location.X),
                (int)MathF.Floor(husk.Location.Y),
                husk.Status.Sight,
                GammaPalette.GammaGreen[Gamma.Standard]);

            if (canvas.Pixels.Contains((int)MathF.Floor(husk.Location.X), (int)MathF.Floor(husk.Location.Y)))
                canvas.Pixels.SetValue(GammaPalette.Alconia[Gamma.Min], (int)MathF.Floor(husk.Location.X), (int)MathF.Floor(husk.Location.Y));

                if (sector.Regions?.Count > 0)
                foreach (Region region in sector.Regions)
                    canvas.DrawRectangle((int)MathF.Floor(region.Perimeter.X),
                        (int)MathF.Floor(region.Perimeter.Y),
                        (int)MathF.Floor(region.Perimeter.Width),
                        (int)MathF.Floor(region.Perimeter.Height),
                        GammaPalette.GammaGreen[Gamma.Dim]);

            foreach (Area area in sector.Areas)
            {
                canvas.DrawRectangle((int)MathF.Floor(area.Perimeter.X),
                    (int)MathF.Floor(area.Perimeter.Y),
                    (int)MathF.Floor(area.Perimeter.Width),
                    (int)MathF.Floor(area.Perimeter.Height),
                    GammaPalette.GammaGreen[Gamma.Bright]);

                if (area.Entrances?.Count > 0)
                    foreach (var entrance in area.Entrances)
                    {
                        if (canvas.Pixels.Contains((int)MathF.Floor(entrance.X), (int)MathF.Floor(entrance.Y)))
                            canvas.Pixels.SetValue(GammaPalette.GammaGreen[Gamma.Max],
                                (int)MathF.Floor(entrance.X),
                                (int)MathF.Floor(entrance.Y));
                    }
            }

            foreach (Structure structure in sector.Structures)
                canvas.DrawRectangle((int)MathF.Floor(structure.Perimeter.X),
                    (int)MathF.Floor(structure.Perimeter.Y),
                    (int)MathF.Floor(structure.Perimeter.Width),
                    (int)MathF.Floor(structure.Perimeter.Height),
                    GammaPalette.NeonRed[Gamma.Max]);

            

            return canvas.Pixels;

        }

        // TODO: Handle definitions in a .JSON file or somewhere else.
        #region Dictionaries
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

        // return a random dialogue pool for any pool that is marked as random.
        public static DialoguePool NextPool()
        {
            var pools = Dialogue.Values.Where(x => x.Generic);

            return Randomizer.Choose(pools);
        }

        // get a specified dialogue pool.
        public static DialoguePool GetPool(string id)
        {
            return Dialogue[id];
        }

        public static Claimable GetClaimable(string id)
            => Claimables[id];

        public static Merit GetMerit(string id)
            => Merits[id];

        public static IEnumerable<Merit> GetMerits(MeritGroup group)
            => Merits.Values.Where(x => x.Group == group);

        public static Booster GetBooster(string id)
            => Boosters[id];

        public static Item GetItem(string id)
            => Items[id];

        public static IEnumerable<Item> GetItemsByDimension(ItemDimension dimension)
            => Items.Values.Where(x => x.Dimension == dimension);
        #endregion

        // TODO: Handle if the market is currently open in these methods.
        public static bool CanShopAt(Husk husk, Construct construct)
        {
            if (IsInLocation(husk, out Market market))
            {
                return true;
            }

            return false;
        }

        public static bool CanShopAtCurrentLocation(Husk husk, out Market market)
        {
            if (IsInLocation(husk, out market))
            {
                return true;
            }

            return false;
        }

        public static bool CanChatWithNpc(Husk husk, string npcId, out Npc npc)
        {
            npc = null;
            var npcs = GetVisibleNpcs(husk);

            if (npcs.Any(x => x.Id == npcId))
            {
                npc = npcs.First(x => x.Id == npcId);
                return true;
            }

            return false;
        }

        public static bool TryLeave(Husk husk)
        {
            Location location = husk.Location.GetLocation();
            if (!location.Type.EqualsAny(LocationType.Area, LocationType.Construct))
                return false;


            if (location.Type == LocationType.Area)
            {
                Area area = (location as Area);
                Vector2 entrance = area.Entrances?.FirstOrDefault() ?? area.Perimeter.Position;
                husk.Location.X = entrance.X;
                husk.Location.Y = entrance.Y;

                // this should point back to the sector.
                husk.Location.Id = location.GetParent().Id;
            }

            if (location.Type == LocationType.Construct)
            {
                var parent = location.GetParent();

                if (parent == null)
                    throw new ArgumentException("The specified location does not have a parent to exit.");

                if (parent?.Type == LocationType.Area)
                {
                    var pos = parent.Perimeter.Origin;
                    husk.Location.X = pos.X;
                    husk.Location.Y = pos.Y;

                    husk.Location.Id = parent.Id;
                }

                else
                {
                    // just move their position to the outskirts of the inner location.
                    // TODO: Implement entrance references for locations.
                    husk.Location.X = location.Perimeter.X;
                    husk.Location.Y = location.Perimeter.Y;
                    husk.Location.Id = parent.Id;
                }
            }

            return true;
        }
          
        public static TravelResult TryGoTo(Husk husk, float x, float y, out Destination attempted)
        {
            attempted = null;

            if (!(LocationType.Sector | LocationType.Field | LocationType.World).HasFlag(husk.Location.GetInnerType()))
                throw new ArgumentException("The specified Husk is not within a coordinate-based location.");

            if (husk.Destination != null)
            {
                if (!husk.Destination.Complete)
                    throw new ArgumentException("Husk is currently in transit.");
                else
                    UpdateLocation(husk, husk.Destination);
            }

            Route route = CreateRoute(husk.Location.X, husk.Location.Y, x, y);
            var now = DateTime.UtcNow;

            // TODO: Implement region naming
            // sector.GetInnerRegion(float x, float y);
            // iterates through all specified regions, and returns the one that contains those coordinates
            Destination info = new Destination(husk.Location.WorldId, husk.Location.Id, x, y, now, now.Add(route.Time));
            attempted = info;

            // if the travel time is short enough, just instantly go to the location.
            if (route.GetTime().TotalSeconds <= 1f)
            {
                UpdateLocation(husk, info);
                return TravelResult.Instant;
            }

            husk.Destination = info;

            return TravelResult.Start;
        }

        // TODO: Merge try go to methods together
        public static TravelResult TryGoToInSector(Husk husk, HuskBrain brain, string id, out Region attempted)
        {
            attempted = null;

            if (husk.Location.GetInnerType() != LocationType.Sector)
                throw new ArgumentException("The specified Husk is not within a sector.");

            Sector sector = husk.Location.GetLocation() as Sector;

            if (husk.Destination != null)
            {
                if (!husk.Destination.Complete)
                    throw new ArgumentException("Husk is currently in transit.");
                else
                    UpdateLocation(husk, husk.Destination);
            }

            foreach (Area area in sector.Areas)
            {
                if (area.Id == id)
                {
                    attempted = area;

                    Route route = CreateRoute(husk.Location.X, husk.Location.Y, area.Perimeter);
                    var now = DateTime.UtcNow;

                    Destination info = new Destination(area.GetLocator(), now, now.Add(route.Time));

                    // if the travel time is short enough, just instantly go to the location.
                    if (route.GetTime().TotalSeconds <= 1f)
                    {
                        UpdateLocation(husk, info);
                        return TravelResult.Instant;
                    }

                    husk.Destination = info;
                    return TravelResult.Start;
                }
            }

            foreach (Structure structure in sector.Structures.Where(x => brain.HasDiscoveredRegion(x.Id)))
            {
                if (structure.Id == id)
                {
                    attempted = structure;

                    Route route = CreateRoute(husk.Location.X, husk.Location.Y, structure.Perimeter);
                    var now = DateTime.UtcNow;

                    Destination info = new Destination(husk.Location.WorldId, husk.Location.Id, structure.Perimeter.Origin.X, structure.Perimeter.Origin.Y, now, now.Add(route.Time));

                    // if the travel time is short enough, just instantly go to the location.
                    if (route.GetTime().TotalSeconds <= 1f)
                    {
                        UpdateLocation(husk, info);
                        return TravelResult.Instant;
                    }

                    husk.Destination = info;
                    return TravelResult.Start;
                }
            }

            return TravelResult.Invalid;
        }

        public static TravelResult TryGoToInArea(Husk husk, string id, out Construct attempted)
        {
            attempted = null;

            Location location = husk.Location.GetLocation();
            if (location.Type != LocationType.Area)
                throw new ArgumentException("The specified Husk is not within an area.");

            foreach (Construct construct in (location as Area).Constructs)
            {
                if (construct.Id == id)
                {
                    attempted = construct;
                    if (construct is Market)
                    {
                        if (!((Market)construct).IsActive())
                        {
                            return TravelResult.Closed;
                        }
                    }

                    husk.Location.Id = construct.Id;
                    return TravelResult.Instant;
                }
            }

            return TravelResult.Invalid;
        }

        public static void Recover(User user, Memorial memorial)
        {
            if (!user.Brain.Memorials.Contains(memorial))
                return;

            // TODO: create a system that allows you to selectively recover items
            user.Husk.Backpack = memorial.Backpack;

            user.Brain.Memorials.Remove(memorial);
        }

        public static bool CanAct(User user)
        {
            if (user.Husk == null)
            {
                if (user.Brain.ResyncAt.HasValue)
                {
                    if ((DateTime.UtcNow - user.Brain.ResyncAt.Value).TotalSeconds > 0)
                    {
                        user.Husk = new Husk(World.Find("ctr0").GetLocator());
                        user.Brain.ResyncAt = null;
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public static bool CanMove(User user, Husk husk)
        {
            if (husk.Destination != null)
            {
                if (!husk.Destination.Complete)
                    return false;

                if (!user.Config.Notifier.HasFlag(NotifyDeny.Travel))
                {
                    // GetLocationName(husk.Location.GetInnerType(), husk.Location.Id, husk.Destination.Id)
                    user.Notifier.Append($"You have arrived at **{husk.Destination.GetInnerName()}**.");
                }

                UpdateLocation(husk, husk.Destination);
                husk.Destination = null;
            }

            return true;
        }

        public static void UpdateLocation(Husk husk, Locator info)
        {
            if (husk.Location.Id == info.Id)
                husk.Location = info;
            else
                husk.Location = info.GetLocation().GetLocator();
        }

        public static string ShowNpcs(Husk husk)
        {
           var npcs = GetVisibleNpcs(husk);
            
           if (npcs?.Count() > 0)
           {
                var result = new StringBuilder();
                var summaries = npcs.Select(x => $"> `{x.Id}` • {x.Name}");

                result.AppendLine($"**Available NPCs** ({husk.Location.GetInnerName()}):");
                result.AppendJoin("\n", summaries);

                return result.ToString();
           }

            return $"There isn't anyone to talk to in **{husk.Location.GetInnerName()}**.";
        }

        public static string ShowLocations(Husk husk, HuskBrain brain)
        {
            Location location = husk.Location.GetLocation();
            var locations = new StringBuilder();

            switch (location.Type)
            {
                case LocationType.Area:
                    var area = location as Area;
                    locations.AppendLine($"**Available Locations** ({area.Name}):");

                    if (area.Constructs?.Count > 0)
                    {
                        locations.AppendLine($"**Available Locations** ({area.Name}):");
                        locations.AppendJoin("\n", area.Constructs.Select(x => $"> `{x.Id}` • {x.Name}"));
                        return locations.ToString();
                    }

                    return "There isn't anything close by. Maybe try going to a different area?";

                case LocationType.Sector:
                    var sector = location as Sector;
                    var structures = GetVisibleStructures(husk, sector);

                    if (sector.Areas?.Count == 0 && structures?.Count() == 0)
                        return "There isn't anything close by. Try looking around!";

                    if (sector.Areas?.Count > 0)
                    {
                        locations.AppendLine($"**Available Areas** ({sector.Name}):");
                        locations.AppendJoin("\n", sector.Areas.Select(x => $"> `{x.Id} • {x.Name}`"));
                    }

                    if (structures?.Count() > 0)
                    {
                        locations.AppendLine();
                        locations.AppendLine($"**Points of Interest**:");

                        var summaries = structures.Select(x =>
                        brain.HasDiscoveredRegion(x.Id)
                        ? $"> `{x.Id}` • {x.Name}"
                        : $"> `({x.Perimeter.Position.X}, {x.Perimeter.Position.Y})` • Unknown Structure");

                        locations.AppendJoin("\n", summaries);
                    }

                    if (brain.Memorials?.Where(x => x.Location.Id == sector.Id).Count() > 0)
                    {
                        locations.Append("\n");
                        var summaries = brain.Memorials.Select(x =>
                        $">` ({x.Location.X}, {x.Location.Y})` • Memorial");

                        locations.AppendJoin("\n", summaries);
                    }

                    return locations.ToString();

                default:
                    throw new Exception("The specified Husk is currently at an invalid location.");
            }
        }

        public static string GetLocationSummary(string id)
        {
            Location location = World.Find(id);

            if (location == null)
                return "You are currently in the **void**.";

            StringBuilder summary = new StringBuilder();

            summary.Append("You are currently in ");

            if (location.Type == LocationType.Construct)
            {
                if ((location as Construct).Tag == ConstructType.Floor)
                {
                    summary.Append($"**{location.Name}** at **{location.GetParent()?.Name} - {(location as Floor).Index}F**");
                }
                else
                {
                    summary.Append($"**{location.Name}**");
                }
            }
            else
            {
                summary.Append($"**{location.Name}**");
            }

            Location parent = location.GetParent();
            int index = 0;

            if (parent != null)
            {
                summary.Append(" (");
                int insertPoint = summary.Length;

                while (parent != null)
                {
                    if (index > 0)
                    {
                        summary.Insert(insertPoint, $"{parent.Name}, ");
                    }
                    else
                    {
                        summary.Append(parent.Name);
                    }

                    parent = parent.GetParent();
                    index++;
                }

                summary.Append(")");
            }

            summary.Append(".");

            return summary.ToString();
        }

        public static readonly float TimePerPixel = 60.0f;

        public static float GetBaseSpeed()
            => 1.0f / (TimePerPixel * World.Scale);

        public static float GetBaseSpeedAt(LocationType type)
            => 1.0f / (TimePerPixel * World.Scale * GetScaleMultiplier(type));

        // TODO: This method is meant for BindToRegion, as it will get all visible regions that a husk can currently see.
        public static IEnumerable<Region> GetVisibleRegions(Husk husk, HuskBrain brain)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<Npc> GetVisibleNpcs(Husk husk)
        {
            Location location = husk.Location.GetLocation();

            switch (location.Type)
            {
                case LocationType.Area:
                    return (location as Area).Npcs;

                case LocationType.Construct:
                    var construct = location as Construct;

                    if (construct.Tag.HasFlag(ConstructType.Highrise))
                        return (construct as Highrise).Npcs;

                    return construct.Npcs;

                case LocationType.Sector:
                    return GetVisibleNpcs(husk, location as Sector);

                default:
                    throw new ArgumentException("The specified Husk is currently at a location that does not support NPCs.");
            }
        }

        private static IEnumerable<Npc> GetVisibleNpcs(Husk husk, Sector sector)
        {
            CircleF sight = GetSightHitbox(husk);

            foreach ((Vector2 Position, Npc Npc) in sector.Npcs)
            {
                if (sight.Contains(Position))
                    yield return Npc;
            }
        }

        public static bool IsInLocation<TLocation>(Husk husk, out TLocation location)
            where TLocation : Location
        {
            location = default;
            var current = husk.Location.GetLocation();

            if (current is TLocation)
            {
                location = current as TLocation;
                return true;
            }

            return false;
        }

        public static bool IsInLocation(Husk husk, LocationType type, out Location location)
        {
            location = null;
            var current = husk.Location.GetLocation();

            if (current.Type == type)
            {
                location = current;
                return true;
            }

            return false;
        }

        private static CircleF GetSightHitbox(Husk husk)
            => new CircleF(husk.Location.X, husk.Location.Y, husk.Status.Sight * GetScaleMultiplier(husk.Location.GetInnerType()));

        public static bool TryIdentifyStructure(Husk husk, HuskBrain brain, Structure structure)
        {
            if (structure != null)
            {
                if (brain.HasDiscoveredRegion(structure.Id))
                    return false;

                brain.IdentifyRegion(structure.Id);
                return true;
            }

            return false;
        }

        public static bool IsNearStructure(Husk husk, Structure structure)
        {
            CircleF sight = GetSightHitbox(husk);

            if (structure != null)
                return sight.Intersects(structure.Perimeter);

            return false;
        }

        public static bool IsNearPoint(Husk husk, Vector2 point)
        {
            CircleF sight = GetSightHitbox(husk);

            return sight.Contains(point);
        }

        public static bool IsNearPoint(Husk husk, float x, float y)
        {
            CircleF sight = GetSightHitbox(husk);

            return sight.Contains(new Vector2(x, y));
        }

        public static bool IsNearClosestStructure(Husk husk, out Structure closest)
        {
            CircleF sight = GetSightHitbox(husk);
            closest = GetClosestStructure(husk);

            if (closest != null)
                return sight.Intersects(closest.Perimeter);

            return false;
        }

        internal static IEnumerable<Structure> GetVisibleStructures(Husk husk, Sector sector)
        {
            CircleF sight = GetSightHitbox(husk);

            if (!(sector.Structures?.Count > 0))
                yield break;

            foreach (Structure structure in sector.Structures)
            {
                if (sight.Intersects(structure.Perimeter))
                    yield return structure;
            }
        }

        internal static Structure GetClosestStructure(Husk husk)
        {
            CircleF sight = GetSightHitbox(husk);

            if (!IsInLocation(husk, out Sector sector))
                throw new ArgumentException("The specified Husk is not currently within a valid location.");

            Structure closest = null;
            float nearest = float.MaxValue;

            foreach (Structure structure in sector.Structures)
            {
                Line line = HitboxEngine.GetLineFromOriginToRegion(sight, structure);
                float distance = line.GetLength();

                if (distance < nearest)
                {
                    nearest = distance;
                    closest = structure;
                }
            }

            return closest;
        }

        

        private static Route CreateRoute(float x, float y, RegionF region)
            => new Route
            {
                Enforce = true,
                From = new Vector2(x, y),
                To = region.Origin
            };

        private static Route CreateRoute(float x, float y, float u, float v)
            => new Route
            {
                Enforce = true,
                From = new Vector2(x, y),
                To = new Vector2(u, v)
            };

        // TODO: Finish creating method.
        public static List<Route> GetRoutes(Husk husk)
        {
            // ensure that the husk travelling is in a location that supports routes.
            // routes are a path reference from one location to the other.
            // route progression is determined by if the path intersects with a barrier
            // otherwise, Routes can be placed, and could be enforced.
            // TODO: Account for possible barriers in the surrounding area.

            // use user's current position.
            var routes = new List<Route>();
            // if the user is in an AREA, do not account for travel time when listing CONSTRUCTS.
            // if the user is in a SECTOR, account for AREAS


            return routes;
            // foreach route, you would want to get the travel time based on position.
        }

        public static float GetScaleMultiplier(LocationType type)
        {
            return type switch
            {
                LocationType.World => 1.0f,
                LocationType.Field => 0.5f,
                LocationType.Sector => 0.25f,
                _ => throw new ArgumentException("Invalid scale.")
            };
        }
    }
}
