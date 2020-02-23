using Orikivo.Drawing;
using Orikivo.Drawing.Graphics2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Orikivo.Unstable
{
    /// <summary>
    /// Represents the main process of a <see cref="Unstable.World"/>.
    /// </summary>
    public static class WorldEngine
    {
        public static World World => new World
        {
            Id = "world0",
            Name = "Test World",
            Boundary = new System.Drawing.SizeF(512, 512),
            Scale = 1.0f, // 1 pixel in the world would take 1 minute at default ratio.
            // sectors are 1/4 of the distance ratio
            Sectors = new List<Sector>
            {
                new Sector
                {
                    Id = "sector0",
                    Name = "Sector 0",
                    Position = new Vector2(16, 356),
                    Entrance = new Vector2(0, 16),
                    // Small 64, Medium 128, Large 256
                    Scale = SectorScale.Small,
                    Structures = new List<Structure>
                    {
                        new Structure
                        {
                            Position = new Vector2(36, 24),
                            Type = StructureType.Decoration
                        },
                        new Structure
                        {
                            Position = new Vector2(64, 64),
                            Type = StructureType.Tent
                        }
                    },
                    Areas = new List<Area>
                    {
                        new Area
                        {
                            Id = "area0",
                            Name = "Area A",
                            Region = new RegionF(0, 0, 32, 32),
                            Entrances = new List<Vector2>
                            {
                                new Vector2(32, 16)
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
                                    Tag = ConstructTag.Market,
                                    Table = new LootTable
                                    {
                                        Capacity = 4,
                                        RequiredTags = ItemTag.Physical,
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
                                                Archetype = Archetype.Generic
                                            },
                                            Schedule = new Schedule
                                            {
                                                Shifts = new List<Shift>
                                                {
                                                    new Shift(DayOfWeek.Saturday, 0, 0, TimeSpan.FromHours(22)),
                                                    new Shift(DayOfWeek.Sunday, 0, 0, TimeSpan.FromHours(16))
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
                                                Archetype = Archetype.Generic
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
                                                Archetype = Archetype.Generic
                                            }
                                        },
                                        new Npc
                                        {
                                            Id = "beta",
                                            Name = "Beta",
                                            Personality = new Personality
                                            {
                                                Archetype = Archetype.Generic
                                            }
                                        }
                                    }
                                },
                                new ConstructGroup
                                {
                                    Id = "construct0",
                                    Name = "Construct G1",
                                    Layers = new List<ConstructLayer>
                                    {
                                        new ConstructLayer
                                        {
                                            Level = 0,
                                            Npcs = new List<Npc>
                                            {
                                                new Npc
                                                {
                                                    Id = "npc0",
                                                    Name = "NPC A1.1",
                                                    Personality = new Personality
                                                    {
                                                        Archetype = Archetype.Generic
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
                                        Archetype = Archetype.Generic
                                    }
                                }
                            }
                        },
                        new Area
                        {
                            Id = "area1",
                            Name = "Area B",
                            Region = new RegionF(48, 48, 16, 16),
                            Entrances = new List<Vector2>
                            {
                                new Vector2(48, 56)
                            },
                            Constructs = new List<Construct>
                            {
                                new ConstructGroup
                                {
                                    Id = "construct1",
                                    Name = "Construct B1",
                                    Layers = new List<ConstructLayer>
                                    {
                                        new ConstructLayer
                                        {
                                            Level = 0,
                                            Npcs = new List<Npc>
                                            {
                                                new Npc
                                                {
                                                    Id = "npc1",
                                                    Name = "NPC B1.1",
                                                    Personality = new Personality
                                                    {
                                                        Archetype = Archetype.Generic
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

        // This starts out the husk at the starting location of the world.
        public static void Initialize(User user)
        {
            user.Husk = new Husk(GetLocator("area0"));
            user.Husk.Position = GetAreaPosition("area0");
            user.Brain.SetFlag(HuskFlags.Initialized);
        }

        public static Locator GetLocator(string id, int? layer = null)
        {
            Locator result = new Locator { WorldId = World.Id };

            foreach (Sector sector in World.Sectors)
            {
                result.SectorId = sector.Id;
                foreach (Area area in sector.Areas)
                {
                    if (id == area.Id)
                    {
                        result.AreaId = area.Id;
                        return result;
                    }

                    foreach (Construct construct in area.Constructs)
                    {
                        if (construct.Tag.HasFlag(ConstructTag.Group))
                        {
                            if (layer.HasValue)
                            {
                                foreach (ConstructLayer cLayer in ((ConstructGroup)construct).Layers)
                                {
                                    if (layer.Value == cLayer.Level)
                                    {
                                        result.ConstructId = construct.Id;
                                        result.ConstructLayer = cLayer.Level;

                                        return result;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (id == construct.Id)
                            {
                                result.ConstructId = construct.Id;
                                result.ConstructLayer = null;

                                return result;
                            }
                        }
                    }
                }
            }

            throw new ArgumentException("No matching ID could be found in any of the underlying world locations.");
        }
        
        #region Mapping
        /// <summary>
        /// Returns an <see cref="Array"/> of bytes that represents a <see cref="User"/>'s compressed map progress.
        /// </summary>
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

        /// <summary>
        /// Returns a <see cref="Grid{bool}"/> that represents a <see cref="User"/>'s exploration progress of a map.
        /// </summary>
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

        /// <summary>
        /// Returns a map for a specified location that includes a <see cref="User"/>'s completion progress.
        /// </summary>
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
        #endregion

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
                Tag = ItemTag.Physical
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
                Tag = ItemTag.Physical
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
                Tag = ItemTag.Physical
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
                Tag = ItemTag.Callable,
                ToOwn = u => u.Debt >= 1000,
                Value = 40,
                CanBuy = true,
                CanSell = false,
                Action = new ItemAction
                {
                    UseLimit = 1,
                    BreakOnLastUse = true,
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
                Criteria = x => x.GetStat("times_cried") == 1,
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
                Generic = false,
                Entry = "Hello.",
                Exit = "Sorry, but I have to go.",
                Timeout = "Sorry, but I have to go.",
                Dialogue = new List<Dialogue>
                {
                    new Dialogue
                    {
                        Type = DialogueType.Initial,
                        Tone = DialogueTone.Neutral,
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
                        Type = DialogueType.Answer,
                        Tone = DialogueTone.Neutral,
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
                        Type = DialogueType.Initial,
                        Tone = DialogueTone.Neutral,
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
                        Type = DialogueType.Reply,
                        Tone = DialogueTone.Confused,
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
                        Type = DialogueType.Reply,
                        Tone = DialogueTone.Neutral,
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
                        Type = DialogueType.Answer,
                        Tone = DialogueTone.Neutral,
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
                        Type = DialogueType.Initial,
                        Tone = DialogueTone.Neutral,
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
                        Type = DialogueType.Answer,
                        Tone = DialogueTone.Confused,
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
                        Type = DialogueType.Question,
                        Tone = DialogueTone.Happy,
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
                        Type = DialogueType.Reply,
                        Tone = DialogueTone.Neutral,
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
                        Type = DialogueType.Reply,
                        Tone = DialogueTone.Neutral,
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
                        Type = DialogueType.Answer,
                        Tone = DialogueTone.Confused,
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
                        Type = DialogueType.Initial,
                        Tone = DialogueTone.Neutral,
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
                        Type = DialogueType.Reply,
                        Tone = DialogueTone.Neutral,
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
                        Type = DialogueType.Reply,
                        Tone = DialogueTone.Neutral,
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
                        Type = DialogueType.Reply,
                        Tone = DialogueTone.Happy,
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
                        Type = DialogueType.Reply,
                        Tone = DialogueTone.Neutral,
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
                        Type = DialogueType.Answer,
                        Tone = DialogueTone.Neutral,
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
                        Type = DialogueType.Reply,
                        Tone = DialogueTone.Shocked,
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
                        Type = DialogueType.Reply,
                        Tone = DialogueTone.Neutral,
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
                        Type = DialogueType.Answer,
                        Tone = DialogueTone.Happy,
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
                        Type = DialogueType.Initial,
                        Tone = DialogueTone.Neutral,
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
                        Type = DialogueType.End,
                        Tone = DialogueTone.Neutral,
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
                        Type = DialogueType.Initial,
                        Tone = DialogueTone.Neutral,
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
                        Type = DialogueType.Answer,
                        Tone = DialogueTone.Neutral,
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
                        Type = DialogueType.Initial,
                        Tone = DialogueTone.Neutral,
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
                        Type = DialogueType.End,
                        Tone = DialogueTone.Happy,
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
                        Type = DialogueType.Initial,
                        Tone = DialogueTone.Neutral,
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
                        Type = DialogueType.Answer,
                        Tone = DialogueTone.Neutral,
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
                        Type = DialogueType.Initial,
                        Tone = DialogueTone.Neutral,
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
                        Type = DialogueType.End,
                        Tone = DialogueTone.Neutral,
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

        public static Booster GetBooster(string id)
            => Boosters[id];

        public static Item GetItem(string name)
            => Items[name];
        #endregion

        #region Checks
        public static bool CanShop(Husk husk, out Market market)
        {
            market = null;

            if (husk.Location.GetInnerType() == LocationType.Construct)
            {
                if (husk.Location.GetConstruct().Tag.HasFlag(ConstructTag.Market))
                {
                    market = (Market)husk.Location.GetConstruct();
                    return true;
                }
            }

            return false;
        }

        public static bool CanChat(Husk husk, string npcId, out Npc npc)
        {
            npc = null;
            List<Npc> npcs = GetNpcs(husk);

            if (npcs.Any(x => x.Id == npcId))
            {
                npc = npcs.First(x => x.Id == npcId);
                return true;
            }

            return false;
        }

        public static bool TryLeave(Husk husk)
        {
            if (!husk.Location.GetInnerType().EqualsAny(LocationType.Area, LocationType.Construct))
                return false;


            if (husk.Location.GetInnerType() == LocationType.Area)
            {
                
                Vector2 entrance = husk.Location.GetArea().Entrances?.FirstOrDefault() ?? husk.Location.GetArea().Region.Point;
                husk.Location.X = entrance.X;
                husk.Location.Y = entrance.Y;
                husk.Location.AreaId = null;
            }

            if (husk.Location.GetInnerType() == LocationType.Construct)
                husk.Location.ConstructId = null;

            return true;
        }
        public static TravelResult TryGoTo(Husk husk, string id, out Area attempted)
        {
            attempted = null;

            if (husk.Location.GetInnerType() != LocationType.Sector)
                throw new ArgumentException("The specified Husk is not within a sector.");


            if (husk.Movement != null)
            {
                if (!husk.Movement.Complete)
                    throw new ArgumentException("Husk is currently in transit.");
                else
                    UpdateLocation(husk, husk.Movement);
            }

            foreach (Area area in husk.Location.GetSector().Areas)
            {
                if (area.Id == id)
                {
                    attempted = area;

                    Route route = CreateRoute(husk.Location.X, husk.Location.Y, area.Region);

                    
                    var now = DateTime.UtcNow;

                    MovementInfo info = new MovementInfo(LocationType.Area, area.Id, now, now.Add(route.Time));

                    // if the travel time is short enough, just instantly go to the location.
                    if (route.GetTime().TotalSeconds <= 1f)
                    {
                        UpdateLocation(husk, info);
                        return TravelResult.Success;
                    }

                    husk.Movement = info;
                    return TravelResult.Start;
                }
            }

            // TODO: Handle Structure.Id and Structure.Position to determine if they can travel there.

            // foreach structure
            // if (visible)
            // if (structure.id == id) => true
            
            /*
             
            foreach (Structure structure in husk.Location.GetSector().Structures)
            {
                if (structure.Position.X == x && structure.Position.Y == y)
            } 

             */

            return TravelResult.Invalid;

                // 1. search each area to see if an id matches
                // - if an id does match, calculate the route to go from your position to the desired area

                // 2. if the specified information are coordinates
                // - if the coordinates are accessible, calculate the route to go from your position to the coords

                // 3. if the specified id for a structure matches
                // - pass the coordinates of the structure onto the route, and calculate the route

                // otherwise, if it is out of bounds or the point is inaccessible, the game will let you know.

                // once all of the needed information is found, apply the ArrivalData to a husk

                // TODO: create travel time calculation and Arrival data
        }

        public static bool CanMove(Husk husk)
        {
            if (husk.Movement != null)
            {
                if (!husk.Movement.Complete)
                    return false;
                else
                    UpdateLocation(husk, husk.Movement);
            }

            return true;
        }
        public static Route CreateRoute(float x, float y, RegionF region)
        {
            // TODO: create point and other route implementation at locations.
            // for now, it's just direct routes.
            Route route = new Route { Enforce = true, From = new Vector2(x, y), To = region.Point };
            return route;
        }

        // assumed for area.
        public static void UpdateLocation(Husk husk, MovementInfo info)
        {
            husk.Location.AreaId = info.LocationId;
            if (info.Type == LocationType.Area)
            {
                Vector2 pos = GetAreaPosition(info.LocationId);
                husk.Location.X = pos.X;
                husk.Location.Y = pos.Y;
                husk.Movement = null;
            }
        }

        public static TravelResult TryGoTo(Husk husk, string id, out Construct attempted)
        {
            attempted = null;

            if (husk.Location.GetInnerType() != LocationType.Area)
                throw new ArgumentException("The specified Husk is not within an area.");

            foreach (Construct construct in husk.Location.GetArea().Constructs)
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

                    husk.Location.ConstructId = construct.Id;
                    return TravelResult.Success;
                }
            }

            return TravelResult.Invalid;
        }
        #endregion

        #region Descriptors

        //  return the list of routes they themselves can go to.
        public static string ShowNpcs(Husk husk)
        {
            StringBuilder npcs = new StringBuilder();

            npcs.Append("**Available NPCs** (");
            if (husk.Location.GetInnerType() == LocationType.Area)
            {
                if (husk.Location.GetArea().Npcs.Count == 0)
                    return $"There isn't anyone to talk to in **{husk.Location.GetInnerName()}**.";

                Area area = husk.Location.GetArea();
                npcs.AppendLine($"{area.Name}):");
                npcs.AppendJoin("\n", GetNpcs(husk).Select(x => $"> `{x.Id}` • {x.Name}"));
            }
            else if (husk.Location.GetInnerType() == LocationType.Construct)
            {
                Construct construct = husk.Location.GetConstruct();
                npcs.AppendLine($"{construct.Name}):");
                npcs.AppendJoin("\n", GetNpcs(husk).Select(x => $"> `{x.Id}` • {x.Name}"));
            }
            else
                throw new Exception("The specified Husk is currently not within an area or construct.");

            return npcs.ToString();
        }

        // an area shows a list of constructs
        // a sector shows a list of areas AND points of interest based on a users view radius.
        public static string ShowLocations(Husk husk)
        {
            StringBuilder routes = new StringBuilder();
            if (husk.Location.GetInnerType() == LocationType.Area)
            {
                Area area = husk.Location.GetArea();

                routes.AppendLine($"**Available Locations** ({area.Name}):");

                if (area.Constructs.Count > 0)
                    routes.AppendJoin("\n", area.Constructs.Select(x => $"> `{x.Id}` • {x.Name}"));
                else
                    routes.Append("There isn't anything close by. Maybe try going to a different area?");

                return routes.ToString();
            }
            else if (husk.Location.GetInnerType() == LocationType.Sector)
            {
                Sector sector = husk.Location.GetSector();

                routes.AppendLine($"**Available Areas** ({sector.Name}):");

                if (sector.Areas.Count > 0)
                    routes.AppendJoin("\n", sector.Areas.Select(x => $"> `{x.Id} • {x.Name}`"));

                List<Structure> structures = GetVisibleStructures(husk.Attributes.MaxSight, husk.Location.X, husk.Location.Y, sector);
                if (structures.Count > 0)
                {
                    routes.AppendLine();
                    routes.AppendLine($"**Points of Interest**:");
                    // TODO: Implement structure naming and ID ref once discovered.
                    routes.AppendJoin("\n", structures.Select(x => $"> `({x.Position.X}, {x.Position.Y})`  • Structure"));
                }

                if (sector.Areas.Count == 0 && structures.Count == 0)
                {
                    routes.Append("There isn't anything close by. Try looking around!");
                }

                return routes.ToString();
            }
            else
                throw new Exception("The specified Husk is currently at an invalid location.");
        }

        public static string GetLocationSummary(string worldId, string sectorId, string areaId, string constructId = null, int? constructLayer = null)
        {
            StringBuilder summary = new StringBuilder();
            summary.Append("You are currently in **");

            Sector sector = World.GetSector(sectorId);

            if (!Checks.NotNull(areaId))
            {
                summary.Append(sector.Name);
                summary.Append("**.");
                return summary.ToString();
            }


            Area area = sector.GetArea(areaId);

            if (Checks.NotNull(constructId))
            {
                Construct construct = area.GetConstruct(constructId);

                if (construct.Tag.HasFlag(ConstructTag.Group))
                {
                    ConstructLayer level = ((ConstructGroup)construct).GetLevel(constructLayer.GetValueOrDefault(0));
                    summary.Append($"{level.Name} ({construct.Name} {level.Level}F)");
                }
                else
                    summary.Append(construct.Name);
            }
            else
            {
                summary.Append(area.Name);
            }

            summary.Append("** (");
            summary.Append(sector.Name);

            if (Checks.NotNull(constructId))
            {
                summary.Append(", ");
                summary.Append(area.Name);
            }

            summary.Append(").");

            return summary.ToString();
        }

        #endregion

        #region Calculations
        public static float TravelRatioSeconds = 60.0f;

        public static float GetVelocity()
        {
            //use Husk.MaxSpeed
            throw new NotImplementedException();
        }
        private static RegionF GetSectorArea(SectorScale scale)
        {
            return scale switch
            {
                SectorScale.Small => new RegionF(0, 0, 64, 64),
                SectorScale.Medium => new RegionF(0, 0, 128, 128),
                SectorScale.Large => new RegionF(0, 0, 256, 256),
                _ => throw new ArgumentException("Unknown sector scale.")
            };
        }

        /// <summary>
        /// Returns an <see cref="Npc"/> collection that a <see cref="Husk"/> can currently interact with.
        /// </summary>
        public static List<Npc> GetNpcs(Husk husk)
        {
            if (husk.Location.GetInnerType() == LocationType.Area)
            {
                return husk.Location.GetArea().Npcs;
            }
            else if (husk.Location.GetInnerType() == LocationType.Construct)
            {
                Construct construct = husk.Location.GetConstruct();

                if (construct.Tag.HasFlag(ConstructTag.Group))
                {
                    return ((ConstructGroup)construct).GetLevel(husk.Location.ConstructLayer.GetValueOrDefault(0)).Npcs;
                }

                return husk.Location.GetConstruct().Npcs;
            }

            throw new Exception("The specified Husk is currently not within an area or construct.");
        }

        // for view radius, sector is 1.0, field is 0.5, and world is 0.25 strength.
        // this is used for sectors, as NPCs can travel and whatknot
        private static List<Npc> GetVisibleNpcs(float viewRadius, Vector2 pos, Sector sector)
        {
            Circle viewable = new Circle(pos, viewRadius);

            List<Npc> visible = new List<Npc>();

            foreach ((Vector2 Position, Npc Npc) in sector.Npcs)
                if (viewable.Contains(Position))
                    visible.Add(Npc);

            return visible;
        }

        // TODO: Create GetVisibleRoutes(), which would utilize an intersection check for routes and circles.
        // public static List<Route> GetVisibleRoutes(float viewRadius, Vector2 pos, Sector sector)

        
        // returns a list of IDs that are visible in a specified area
        private static List<Structure> GetVisibleStructures(float viewRadius, float x, float y, Sector sector)
        {
            Circle viewable = new Circle(x, y, viewRadius);

            List<Structure> visible = new List<Structure>();

            if (Checks.NotNullOrEmpty(sector.Structures))
            {
                foreach (Structure structure in sector.Structures)
                {
                    if (viewable.Contains(structure.Position))
                        visible.Add(structure);
                }
            }

            return visible;
        }

        // ensure that the husk travelling is in a location that supports routes.
        // routes are a path reference from one location to the other.
        public static List<Route> GetRoutes(Husk husk)
        {
            // use user's current position.
            List<Route> routes = new List<Route>();
            // if the user is in an AREA, do not account for travel time when listing CONSTRUCTS.
            // if the user is in a SECTOR, account for AREAS


            return routes;
            // foreach route, you would want to get the travel time based on position.
        }

        // TODO: Account for SECTOR, FIELD, or AREA scaled position.
        //  this returns a locations coordinates for an area in a sector, or sector in a world.
        // this is used to easily store a user's position if they are in an area.
        private static Vector2 GetAreaPosition(string id)
        {
            Locator locator = GetLocator(id);

            Sector s = World.GetSector(locator.SectorId);
            Area a = s.GetArea(locator.AreaId);

            return new Vector2(a.Region.X + (a.Region.Width / 2), a.Region.Y + (a.Region.Height / 2));
        }

        // route progression is determined by if the path intersects with a barrier
        // otherwise, Routes can be placed, and could be enforced.
        // TODO: Account for possible barriers in the surrounding area.

        public static Vector2 GetCurrentPosition(Vector2 from, Vector2 to, DateTime startedAt, TimeSpan travelTime)
        {
            TimeSpan remaining = DateTime.UtcNow - startedAt;
            
            float progress = RangeF.Convert(0.0f, (float)travelTime.TotalSeconds, 0.0f, 1.0f, (float)remaining.TotalSeconds);

            float xDiff = (to.X - from.X) * progress;
            float yDiff = (to.Y - from.Y) * progress;

            // This should get the current position the user would be at if they stopped, based on their starting time and location.
            return new Vector2(from.X + xDiff, from.Y + yDiff);
        }
        private static TimeSpan GetTravelTime(Vector2 from, Vector2 to, MapScale scale)
        {
            // get x diff
            float dx = MathF.Abs(to.X - from.X);
            
            // get y diff
            float dy = MathF.Abs(to.Y - from.Y);
            
            // get map scale multiplier
            float scalar = GetScaleMultiplier(scale);

            // this should be the direct distance (pythagorean's theorem)
            float c = MathF.Sqrt((dx * dx) + (dy * dy));

            // 20 ticks * TravelRatio (60 seconds per tick) = 1200 seconds (20 minutes)
            float ticks = c * scalar;

            return TimeSpan.FromSeconds(ticks * TravelRatioSeconds);
        }

        public static float GetScaleMultiplier(MapScale scale)
        {
            return scale switch
            {
                MapScale.World => 1.0f,
                MapScale.Field => 0.5f,
                MapScale.Sector => 0.25f,
                _ => throw new ArgumentException("Invalid scale.")
            };
        }
        #endregion
    }
}
