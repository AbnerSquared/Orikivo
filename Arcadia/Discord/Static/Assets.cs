using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arcadia.Graphics;
using Arcadia.Multiplayer.Games;
using Arcadia.Services;
using Orikivo;
using Orikivo.Text;

namespace Arcadia
{
    // NOTE: Using this as a means to figure out what to unlock on Orikivo Arcade is frowned upon.
    // Nonetheless, I can't stop you, so do what you will. :(
    // -- This will be stored locally in the future, so do what you can now.
    public static class Assets
    {
        // This is used with Deresin, an item that can restore the original style of an existing item
        internal static readonly Dictionary<string, string> DegradeMap = new Dictionary<string, string>
        {
            [Ids.Items.PaletteGammaGreen] = Ids.Items.PaletteFadedGreen
        };

        public static readonly HashSet<Guide> Guides = new HashSet<Guide>
        {
            new Guide
            {
                Id = "beginner",
                Icon = "🏳️",
                Title = "Beginning Your Journey",
                Summary = "Learn how to get started with **Orikivo Arcade**.",
                // Content = "> **Getting Started**\nWelcome to **Orikivo Arcade**! This is a bot that aims to provide a wide variety of unique ways to collect, game, and more! To initialize an account, simply call any basic command that would require one (eg. `card`, `stats`, `balance`), and your account will be automatically initialized!<#>> **Gaining XP**\nExperience is gained through playing multiplayer games, unlocking merits, completing quests, and through various other activities. You can view your current level at anytime by typing `level`. Raising your level provides access to additional mechanics and/or features that would otherwise be hidden.<#>> :warning: **Notice**\nAscending is not currently implemented.\n\nWhen you reach level 100, you can ascend. Ascending resets your level back to 0, but raises your ascent by 1. The feature of this mechanics is yet to be explained.<#>> **Earning Money**\nAs of now, there isn't too many ways to earn money with **Orikivo Arcade**, but the current 100% safe methods are:\n\n- Using `daily`, which rewards :money_with_wings: **15** per day, with streak rewards\n- Using `assign` to be given new objectives, from which reward a random amount of :money_with_wings: based on its difficulty\n- Unlocking **Merits**, which are milestones based on what you've accomplished on **Orikivo Arcade** so far<#>While those may be the safe methods, there are a few other riskier methods that can be done to earn more funds:\n- Using `gimi`, which can reward up to :money_with_wings: **10** or :page_with_curl: **10** based on chance<#>> **What Is Debt?**\nIn order to keep the balance for funds in check, the implementation of :page_with_curl: **Debt** was added. **Debt** is a negative form of money that automatically blocks all incoming funds until it is paid off. **Debt** is mainly received from the **Casino**, but can come as a surprise at random if you're not careful.",
                Pages = new List<string>
                {
                    "> **Getting Started**\nWelcome to **Orikivo Arcade**! This is a bot that aims to provide a wide variety of unique ways to collect, game, and more! To initialize an account, simply call any basic command that would require one (eg. `card`, `stats`, `balance`), and your account will be automatically initialized!\n\n> **Gaining XP**\nExperience is gained through playing multiplayer games, unlocking merits, completing quests, and through various other activities. You can view your current level at anytime by typing `level`. Raising your level provides access to additional mechanics and/or features that would otherwise be hidden.\n\n> :warning: **Notice**\nAscending is not currently implemented.\n\nWhen you reach level 100, you can ascend. Ascending resets your level back to 0, but raises your ascent by 1. The feature of this mechanics is yet to be explained.\n\n> **Earning Money**\nAs of now, there isn't too many ways to earn money with **Orikivo Arcade**, but the current 100% safe methods are:\n\n- Using `daily`, which rewards :money_with_wings: **15** per day, with streak rewards\n- Using `assign` to be given new objectives, from which reward a random amount of :money_with_wings: based on its difficulty\n- Unlocking **Merits**, which are milestones based on what you've accomplished on **Orikivo Arcade** so far",
                    "While those may be the safe methods, there are a few other riskier methods that can be done to earn more funds:\n- Using `gimi`, which can reward up to :money_with_wings: **10** or :page_with_curl: **10** based on chance\n\n> **What Is Debt?**\nIn order to keep the balance for funds in check, the implementation of :page_with_curl: **Debt** was added. **Debt** is a negative form of money that automatically blocks all incoming funds until it is paid off. **Debt** is mainly received from the **Casino**, but can come as a surprise at random if you're not careful.",
                    "> **Merits**\nMerits are milestone achievements that can be unlocked by meeting certain criteria (usually provided as a hint for the merit's quote). Some merits reward :money_with_wings: **Orite**, while some can even reward items! As you unlock more merits, your **Merit Score** will increase.\n\nKeep in mind that there are more merits out there than meets the eye. Mess around and find hidden accomplishments!",
                    "> **Boosters**\nBoosters are a good way to increase the amount of money you earn based on the activities you execute. Some can decrease the amount of :page_with_curl: **Debt** you receive, while others can boost the amount of :money_with_wings: **Orite** you earn! You can view your income rate at anytime by typing `boosters`.\n\nHowever, note that while some boosters can provide great benefits, others can bind you with a nasty inhibitor that may increase the amount of :page_with_curl: **Debt** you receive!\n\n> **Crafting**\nCertain items can be crafted once a collection of items have been found. Crafting can be used to create brand-new collectibles from existing ones, as well as increase the strength of certain base items!\n\n> :warning: Cards are currently disabled due to an issue with **Ubuntu**. This will be fixed ASAP.\n\n> **Card Design**\nA **Card** represents an overall summary of your account on **Orikivo Arcade**, and can be customized to your own desire. Change the color of your **Card**, set the font face to use, and set up new layouts! The ability to customize your card is planned to expand based on your progress. To view your card, type `card`. To view other cards, type `card <user>`.",
                    "> **Quests**\nQuests are a fun way to earn :money_with_wings: **Orite** and :small_red_triangle: **EXP** by accomplishing random challenges that are provided to you. Each quest has a specified difficulty which determines the amount you receive.\n\nEveryone starts off with **1** quest slot, but as they keep accomplishing more and utilizing the bot, more slots become available, alongside a wider range of quests.\n\n> **Profiles**\nProfiles are another alternative to a **Card**, and they are always available to view. You can view your profile by typing `profile`. You are also able to view another user's profile by typing `profile <user>`.\n\n> **Gifting and Trading**\nIf you wish to be a kind-hearted soul, you are allowed to gift items of any kind (provided they are tradable). To gift an item to other users, type `gift <slot_id | unique_id | item_id>`, and the best match for the selected item will be chosen.",
                    "> **Inventory Management**\nEach user has an inventory that can store a wide range of collectibles. Some items can be used to escape :page_with_curl: **Debt**, while others can change or alter how your **License** (or **Card**) is shown. Items can have unique use cases and properties that can alter your passive income, provide access to hidden commands and games, and more! You can view your inventory by typing `inventory [page]`.",
                    "> **Shops**\n**Orikivo Arcade** has an ever-so expanding range of shops available to the user! As you progress, more shops will become available, while other shops may expand their inventory!\n\nAll shops refresh their contents on the start of a new day (in **UTC** time). Certain shops may provide the ability to sell specific items, with the downside of certain deductions as tax. Most shops are designated to a specific set of items, while others can simply be a wild card. Each shop has a limited stock, so once you purchase everything in the store, you may not be able to purchase anything again until the shop is refreshed. As you fulfill certain requirements or milestones, shops will increase with what they tend to offer, most notably by adding more unique items in their item pool.",
                    "> **Casino**\nThe **Casino** is a risky, but possible way to gain riches in this world! To get started, convert some of your :money_with_wings: **Orite** into :jigsaw: **Chips** (the official gambling currency) by typing `getchips <amount>`. For more information on how the :jigsaw: **Chip** rate conversion works, type `getchips` to read more details.\n\nAs of now, the **Casino** offers the following games:\n- **Doubler**, a game mode with plenty of risk and a very high possible reward",
                    "> **Leaderboards**\nChallenge yourself to be at the top of five ranked sections:\n- **The Field of Wealth**: Tracks users by the amount of :money_with_wings: **Orite** they make in a cycle \n- **Wisdom's Canyon**: Ranks users based on the amount of **XP** they earn in a cycle\n- **Challenger's Approach**: Ranks users based on the amount of **Quest Points** they earn in a cycle\n- **The Arcade**: Ranks users based on their **Arcade Score** earned in a cycle\n- **The Prediction Pool**: Ranks users based on their :jigsaw: **Chips** earned in a cycle\n\nA leaderboard cycle is one month. At the end of each cycle, all of the leaders are given rewards, and the scores reset.\n\nHowever, ff you wish to view an unranked leaderboard for a specific stat, you may do so by typing the ID of a stat in place of the section ID (eg. `leaderboard gimi:times_played`).",
                    "> **Stats**\nTrack and keep note of all of your statistics for every game mode and activity in **Orikivo Arcade**! To view a list of all of your stats, type `stats [page]` to get started. To view a group of stats instead, you can type `statgroup <group>`, where the group is the first part of a stat (eg. `gimi` in `gimi:times_played`).",
                    "> **Configuration**\nDue to how complex **Orikivo Arcade** gets at times, there is a configuration panel that allows you to changes various settings. These include:\n- Tooltips\n- Notifications\n- Error Handling\n- Prefix\n\nTooltips are handy little tips that appear in most commands that you use to help guide you towards anything that you might want to do. To disable them, You can type `config tooltips false`, which will hide all of them. Keep in mind that this will hide all tips that appear, which can be confusing at times if you don't know how to use **Orikivo Arcade** just yet.\n\nNotifications are used to help notify a user about specific things. This can be fine-tuned for each user, so that they won't be overwhelmed with notifications on every command they execute.\n\nError handling determines how an error is shown to a user. It can be simplified to hide developer information to keep the focus solely on the bot. However, in order for the developers to figure out problems that might occur, it is recommended to at least leave it on **Simple**, so that the direct cause of the issue that occured can quickly be found.\n\nYour prefix is by default either set to the server's default, or the global default, which is `[`. If you wish to override this prefix with your own, you can type `config prefix <value>` to do so. Keep in mind that the limit for how long a prefix can be is 16 characters.",
                    "> **Closing**\nWhile this doesn't exactly cover everything, this should hopefully provide a good kickstart on how to use **Orikivo Arcade** to the best of your extent. Get out there and explore. You may find more than you even knew existed."
                }
            },

            new Guide
            {
                Id = "trading",
                Icon = "",
                Title = "Trading & Offers",
                Summary = "Learn about the basics of trade offers.",
                Pages = new List<string>
                {
                    "> **Trade Offers**\nIf a player wishes to quickly send an offer instead, they may do so by typing `offer <user> <input>`, from which an offer is built as `<inbound> for <outbound>`. If you wish to only request items, you may do so by typing `offer <user> for <outbound>`. Otherwise, you may use `offer <user> <inbound> for <outbound>`. To view a list of possible offers, simply type `offers [page]`, where the default page is **1**.",
                    "Trade offers are parsed by typing the item's ID alongside a specified amount, typed as `<item_id>,<amount>`. An example is `su_pl,1`, which translates to **Summon: Pocket Lawyer** (x**1**). If the amount is unspecified, it is always set to **1** by default.\nAn example offer is `su_pl,2 p_gg for p_gl p_wu`, which translates to:\n\nYou will lose:\n- **Summon: Pocket Lawyer** (x**1**)\n- **Card Palette: Gamma Green** (x**1**)\n\nYou will gain:\n+ **Card Palette: Glass** (x**1**)\n+ **Card Palette: Wumpite** (x**1**)\n\nIf you find an offer that you wish to accept, you may do so by typing `acceptoffer <offer_id>`, from which the offer ID is shown in `offers [page]`.\n\n> :warning: **Keep In Mind**\nYou may only have up to **5** inbound and outbound offers at a time. Trade offers expire in **24**h, and are saved per session. If **Orikivo Arcade** were to suddenly go offline, the trade offers would not be stored."
                }
            },

            new Guide
            {
                Id = "multiplayer",
                Icon = "⚔️",
                Title = "Multiplayer",
                Summary = "Learn how to use the multiplayer system.",
                Pages = new List<string>
                {
                    $"> {Icons.Warning} Please note that in some multiplayer games, you are required to have the option **Allow direct message from server members** enabled. This can be found in **Privacy & Safety/Server Privacy Defaults**.\n\nSo you've come here to learn about multiplayer? You're in the right place.\n\n> **Game Modes**\nYou can view all of the games **Orikivo Arcade** currently offers by typing `games`.\n\n> {Icons.Warning} **Beware!**\nGames that are marked with `(Beta)` may have bugs and/or issues that could make the game unplayable. If a session becomes soft-locked, the server host can force end the session by typing `destroysession`.\n\n> **Hosting a Server**\nTo host a session, simply type `hostserver` to start up a default server. If you have a game in mind that you wish to play, type `hostserver <game_id>` instead to automatically launch the server for the specified game mode.\n\n> **Joining a Server**\nJoining a server has been made as easy as possible! To join an existing server, you can use the **Server Browser** to find a server to join (`servers [page]`), from which you can join by typing `joinserver <server_id>`. If you were invited to a server, you can view those invites by typing `invites`, and accepting the specified invite by its unique index (`acceptinvite <index>`). Likewise, if you just wish to quickly get into a game, you can type `quickjoin` or `quickjoin <game_id>` to hop into a random available server! You can also join existing servers by typing `join` (no prefix) in an existing server connection.",
                }
            }
        };

        public static readonly HashSet<LootTable> LootTables = new HashSet<LootTable>
        {
            new LootTable
            {
                Id = Ids.Items.CapsuleDaily1,
                Entries = new List<LootEntry>
                {
                    new LootEntry
                    {
                        ItemId = Ids.Items.PaletteGammaGreen,
                        Weight = 3
                    },
                    new LootEntry
                    {
                        ItemId = Ids.Items.PaletteCrimson,
                        Weight = 1
                    },
                    new LootEntry
                    {
                        Money = 10,
                        Weight = 10
                    },
                    new LootEntry
                    {
                        Money = 25,
                        Weight = 35
                    },
                    new LootEntry
                    {
                        Money = 50,
                        Weight = 25
                    },
                    new LootEntry
                    {
                        Money = 75,
                        Weight = 10
                    },
                    new LootEntry
                    {
                        Money = 100,
                        Weight = 5
                    },
                    new LootEntry
                    {
                        ItemId = Ids.Items.BoosterOriteBooster,
                        Weight = 2
                    },
                    new LootEntry
                    {
                        ItemId = Ids.Items.PaletteOceanic,
                        Weight = 1
                    },
                    new LootEntry
                    {
                        ItemId = Ids.Items.PaletteLemon,
                        Weight = 1
                    }
                }
            },
            new LootTable
            {
                Id = Ids.Items.CapsuleDaily2,
                Entries = new List<LootEntry>
                {
                    new LootEntry
                    {
                        ItemId = Ids.Items.PaletteGammaGreen,
                        Weight = 3
                    },
                    new LootEntry
                    {
                        ItemId = Ids.Items.PaletteCrimson,
                        Weight = 2
                    },
                    new LootEntry
                    {
                        Money = 25,
                        Weight = 12
                    },
                    new LootEntry
                    {
                        Money = 50,
                        Weight = 15
                    },
                    new LootEntry
                    {
                        Money = 75,
                        Weight = 23
                    },
                    new LootEntry
                    {
                        Money = 100,
                        Weight = 10
                    },
                    new LootEntry
                    {
                        Money = 250,
                        Weight = 1
                    },
                    new LootEntry
                    {
                        ItemId = Ids.Items.BoosterOriteBooster,
                        Weight = 4
                    },
                    new LootEntry
                    {
                        ItemId = Ids.Items.PaletteOceanic,
                        Weight = 3
                    },
                    new LootEntry
                    {
                        ItemId = Ids.Items.PaletteLemon,
                        Weight = 3
                    }
                }
            },
            new LootTable
            {
                Id = Ids.Items.CapsuleDaily3,
                Entries = new List<LootEntry>
                {
                    new LootEntry
                    {
                        ItemId = Ids.Items.PaletteGammaGreen,
                        Weight = 4
                    },
                    new LootEntry
                    {
                        ItemId = Ids.Items.PaletteCrimson,
                        Weight = 5
                    },
                    new LootEntry
                    {
                        Money = 50,
                        Weight = 10
                    },
                    new LootEntry
                    {
                        Money = 75,
                        Weight = 35
                    },
                    new LootEntry
                    {
                        Money = 100,
                        Weight = 25
                    },
                    new LootEntry
                    {
                        Money = 250,
                        Weight = 5
                    },
                    new LootEntry
                    {
                        ItemId = Ids.Items.BoosterOriteBooster,
                        Weight = 10
                    },
                    new LootEntry
                    {
                        ItemId = Ids.Items.PaletteOceanic,
                        Weight = 4
                    },
                    new LootEntry
                    {
                        ItemId = Ids.Items.BoosterDaily,
                        Weight = 1
                    },
                    new LootEntry
                    {
                        ItemId = Ids.Items.PaletteLemon,
                        Weight = 5
                    }
                }
            },
            new LootTable
            {
                Id = Ids.Items.CapsuleCasinoI,
                Entries = new List<LootEntry>
                {
                }
            }
        };

        public static readonly HashSet<Challenge> Challenges = new HashSet<Challenge>
        {
            /*
            new Challenge
            {
                Id = "task_roulette:001",
                Name = "Play Roulette 5 Times",
                Difficulty = 0,
                Triggers = CriteriaTriggers.Command,
                Criterion = new VarCriterion(Stats.Roulette.TimesPlayed, 5)
            },
            new Challenge
            {
                Id = "task_gimi:001",
                Name = "Play Gimi 5 Times",
                Difficulty = 0,
                Triggers = CriteriaTriggers.Command,
                Criterion = new VarCriterion(Stats.Gimi.TimesPlayed, 5)
            },
            new Challenge
            {
                Id = "task_blackjack:001",
                Name = "Play Blackjack 5 Times",
                Difficulty = 0,
                Triggers = CriteriaTriggers.Command,
                Criterion = new VarCriterion(Stats.BlackJack.TimesPlayed, 5)
            },
            new Challenge
            {
                Id = "task_doubler:001",
                Name = "Play Doubler 5 times",
                Difficulty = 0,
                Triggers = CriteriaTriggers.Command,
                Criterion = new VarCriterion(Stats.Doubler.TimesPlayed, 5)
            },
            */
            new Challenge
            {
                Id = "task_trivia:001",
                Name = "Play a game of **Trivia** (5 question minimum)",
                Difficulty = 0,
                Triggers = CriterionTriggers.Game,
                Criterion = new Criterion
                {
                    Judge = delegate (CriterionContext ctx)
                    {
                        if (ctx.User == null)
                            throw new Exception("Expected ArcadeUser but is unspecified");

                        if (ctx.Result == null)
                            throw new Exception("Expected GameResult but is unspecified");

                        if (ctx.Result.GameId != "trivia")
                            return false;

                        if (!ctx.Result.ValueExists(TriviaConfig.QuestionCount))
                            throw new Exception("Expected game property but does not exist");

                        return ctx.Result.ValueOf<int>(TriviaConfig.QuestionCount) >= 5
                        && ctx.Result.Players.ContainsKey(ctx.User.Id);
                    }
                }
            },
            new Challenge
            {
                Id = "task_trivia:002",
                Name = "Play a game of **Trivia** (10 question minimum)",
                Difficulty = 1,
                Triggers = CriterionTriggers.Game,
                Criterion = new Criterion
                {
                    Judge = delegate (CriterionContext ctx)
                    {
                        if (ctx.User == null)
                            throw new Exception("Expected ArcadeUser but is unspecified");

                        if (ctx.Result == null)
                            throw new Exception("Expected GameResult but is unspecified");

                        if (ctx.Result.GameId != "trivia")
                            return false;

                        if (!ctx.Result.ValueExists(TriviaConfig.QuestionCount))
                            throw new Exception("Expected game property but does not exist");

                        return ctx.Result.ValueOf<int>(TriviaConfig.QuestionCount) >= 10
                        && ctx.Result.Players.ContainsKey(ctx.User.Id);
                    }
                }
            },
            new Challenge
            {
                Id = "task_trivia:003",
                Name = "Play a game of **Trivia** (15 question minimum)",
                Difficulty = 2,
                Triggers = CriterionTriggers.Game,
                Criterion = new Criterion
                {
                    Judge = delegate (CriterionContext ctx)
                    {
                        if (ctx.User == null)
                            throw new Exception("Expected ArcadeUser but is unspecified");

                        if (ctx.Result == null)
                            throw new Exception("Expected GameResult but is unspecified");

                        if (ctx.Result.GameId != "trivia")
                            return false;

                        if (!ctx.Result.ValueExists(TriviaConfig.QuestionCount))
                            throw new Exception("Expected game property but does not exist");

                        return ctx.Result.ValueOf<int>(TriviaConfig.QuestionCount) >= 15
                        && ctx.Result.Players.ContainsKey(ctx.User.Id);
                    }
                }
            },
            new Challenge
            {
                Id = "task_trivia:004",
                Name = "Play a game of **Trivia** (20 question minimum)",
                Difficulty = 3,
                Triggers = CriterionTriggers.Game,
                Criterion = new Criterion
                {
                    Judge = delegate (CriterionContext ctx)
                    {
                        if (ctx.User == null)
                            throw new Exception("Expected ArcadeUser but is unspecified");

                        if (ctx.Result == null)
                            throw new Exception("Expected GameResult but is unspecified");

                        if (ctx.Result.GameId != "trivia")
                            return false;

                        if (!ctx.Result.ValueExists(TriviaConfig.QuestionCount))
                            throw new Exception("Expected game property but does not exist");

                        return ctx.Result.ValueOf<int>(TriviaConfig.QuestionCount) >= 20
                        && ctx.Result.Players.ContainsKey(ctx.User.Id);
                    }
                }
            }
        };

        public static readonly HashSet<Quest> Quests = new HashSet<Quest>
        {
            new Quest
            {
                Id = "quest:trivia_001",
                Name = "Trivia Solo 5Q",
                Summary = "Play a game of 5 question **Trivia**.",
                Difficulty = 0,
                Type = QuestType.Daily,
                Criteria = new List<Criterion>
                {
                    new Criterion
                    {
                        Id = "trivia:play_5q",
                        Name = "Play **Trivia** (**5** Questions)",
                        Triggers = CriterionTriggers.Game,
                        Judge = delegate (CriterionContext ctx)
                        {
                            if (ctx.User == null)
                                throw new Exception("Expected ArcadeUser but is unspecified");

                            if (ctx.Result == null)
                                throw new Exception("Expected GameResult but is unspecified");

                            if (ctx.Result.GameId != "trivia")
                                return false;

                            if (!ctx.Result.ValueExists(TriviaConfig.QuestionCount))
                                throw new Exception("Expected game property but does not exist");

                            return ctx.Result.ValueOf<int>(TriviaConfig.QuestionCount) >= 5
                            && ctx.Result.Players.ContainsKey(ctx.User.Id);
                        }
                    }
                }
            },
            new Quest
            {
                Id = "quest:trivia_002",
                Name = "Trivia Solo 10Q",
                Summary = "Play a game of 10 question **Trivia**.",
                Difficulty = 1,
                Type = QuestType.Daily,
                Criteria = new List<Criterion>
                {
                    new Criterion
                    {
                        Id = "trivia:play_10q",
                        Name = "Play **Trivia** (**10** Questions)",
                        Triggers = CriterionTriggers.Game,
                        Judge = delegate (CriterionContext ctx)
                        {
                            if (ctx.User == null)
                                throw new Exception("Expected ArcadeUser but is unspecified");

                            if (ctx.Result == null)
                                throw new Exception("Expected GameResult but is unspecified");

                            if (ctx.Result.GameId != "trivia")
                                return false;

                            if (!ctx.Result.ValueExists(TriviaConfig.QuestionCount))
                                throw new Exception("Expected game property but does not exist");

                            return ctx.Result.ValueOf<int>(TriviaConfig.QuestionCount) >= 10
                            && ctx.Result.Players.ContainsKey(ctx.User.Id);
                        }
                    }
                }
            },
            new Quest
            {
                Id = "quest:trivia_003",
                Name = "Trivia Solo 15Q",
                Summary = "Play a game of 15 question **Trivia**.",
                Difficulty = 2,
                Type = QuestType.Daily,
                Criteria = new List<Criterion>
                {
                    new Criterion
                    {
                        Id = "trivia:play_15q",
                        Name = "Play **Trivia** (**15** Questions)",
                        Triggers = CriterionTriggers.Game,
                        Judge = delegate (CriterionContext ctx)
                        {
                            if (ctx.User == null)
                                throw new Exception("Expected ArcadeUser but is unspecified");

                            if (ctx.Result == null)
                                throw new Exception("Expected GameResult but is unspecified");

                            if (ctx.Result.GameId != "trivia")
                                return false;

                            if (!ctx.Result.ValueExists(TriviaConfig.QuestionCount))
                                throw new Exception("Expected game property but does not exist");

                            return ctx.Result.ValueOf<int>(TriviaConfig.QuestionCount) >= 15
                            && ctx.Result.Players.ContainsKey(ctx.User.Id);
                        }
                    }
                }
            },
            new Quest
            {
                Id = "quest:trivia_004",
                Name = "Trivia Solo 20Q",
                Summary = "Play a game of 20 question **Trivia**.",
                Difficulty = 2,
                Type = QuestType.Daily,
                Criteria = new List<Criterion>
                {
                    new Criterion
                    {
                        Id = "trivia:play_20q",
                        Name = "Play **Trivia** (**20** Questions)",
                        Triggers = CriterionTriggers.Game,
                        Judge = delegate (CriterionContext ctx)
                        {
                            if (ctx.User == null)
                                throw new Exception("Expected ArcadeUser but is unspecified");

                            if (ctx.Result == null)
                                throw new Exception("Expected GameResult but is unspecified");

                            if (ctx.Result.GameId != "trivia")
                                return false;

                            if (!ctx.Result.ValueExists(TriviaConfig.QuestionCount))
                                throw new Exception("Expected game property but does not exist");

                            return ctx.Result.ValueOf<int>(TriviaConfig.QuestionCount) >= 20
                            && ctx.Result.Players.ContainsKey(ctx.User.Id);
                        }
                    }
                }
            },
            new Quest
            {
                Id = "quest:trivia_011",
                Name = "Trivia Duel 5Q",
                Summary = "Win a game of 5 question **Trivia** with at least 2 players.",
                Difficulty = 1,
                Type = QuestType.Challenge,
                Criteria = new List<Criterion>
                {
                    new Criterion
                    {
                        Id = "trivia:win_2p5q",
                        Name = "Win first place in **Trivia** (**5** Questions)",
                        Triggers = CriterionTriggers.Game,
                        Judge = delegate (CriterionContext ctx)
                        {
                            if (ctx.User == null)
                                throw new Exception("Expected ArcadeUser but is unspecified");

                            if (ctx.Result == null)
                                throw new Exception("Expected GameResult but is unspecified");

                            if (ctx.Result.GameId != "trivia")
                                return false;

                            if (!ctx.Result.ValueExists(TriviaConfig.QuestionCount))
                                throw new Exception("Expected game property but does not exist");

                            if (ctx.Result.GetPlayerResult(ctx.User.Id) == null)
                                return false;

                            return ctx.Result.ValueOf<int>(TriviaConfig.QuestionCount) >= 5
                            && ctx.Result.Players.ContainsKey(ctx.User.Id)
                            && ctx.Result.Players.Count >= 2
                            && ctx.Result.Players.Values.OrderByDescending(p => p.ValueOf<int>(TriviaVars.Score)).First() == ctx.Result.GetPlayerResult(ctx.User.Id);
                        }
                    }
                }
            },
            new Quest
            {
                Id = "quest:weekly_attendance",
                Name = "Weekly Attendance",
                Summary = "Ensure your status for a week.",
                Difficulty = 4,
                Criteria = new List<Criterion>
                {
                    new VarCriterion(Stats.Common.DailyStreak, 7)
                    {
                        Name = "Maintain a daily streak of **7** days"
                    }
                },
                Type = QuestType.Daily,
                Reward = new Reward
                {
                    Money = 105,
                    Exp = 500
                }
            },
            
            new Quest
            {
                Id = "quest:low_stakes",
                Name = "Low Stakes",
                Summary = "The Casino has some classics for you to participate in.",
                Difficulty = 1,
                Criteria = new List<Criterion>
                {
                    new VarCriterion(Stats.Roulette.TimesPlayed, 10)
                    {
                        Name = "Play **10** rounds of **Roulette**"
                    },
                    new VarCriterion(Stats.BlackJack.TimesPlayed, 10)
                    {
                        Name = "Play **10** rounds of **Blackjack**"
                    },
                },
                Type = QuestType.Daily,
                Reward = new Reward
                {
                    Money = 20,
                    Exp = 40
                }
            },
            new Quest
            {
                Id = "quest:casino_field_day",
                Name = "Casino Field Day",
                Summary = "It's a wonderful day to gamble your happiness away!",
                Difficulty = 1,
                Criteria = new List<Criterion>
                {
                    new VarCriterion(Stats.Gimi.TimesPlayed, 6)
                    {
                        Name = "Play **6** rounds of **Gimi**"
                    },
                    new VarCriterion(Stats.Doubler.TimesPlayed, 6)
                    {
                        Name = "Play **6** rounds of **Doubly**"
                    },
                    new VarCriterion(Stats.Roulette.TimesPlayed, 6)
                    {
                        Name = "Play **6** rounds of **Roulette**"
                    },
                    new VarCriterion(Stats.BlackJack.TimesPlayed, 6)
                    {
                        Name = "Play **6** rounds of **Blackjack**"
                    }
                },
                Type = QuestType.Daily,
                Reward = new Reward
                {
                    Money = 25,
                    Exp = 50
                }
            },
            new Quest
            {
                Id = "quest:trivial_pursuit",
                Name = "Trivial Pursuit",
                Summary = "Test your brain power and push through.",
                Difficulty = 1,
                Criteria = new List<Criterion>
                {
                    new VarCriterion(Stats.Trivia.TimesPlayed, 5)
                    {
                        Name = "Play **5** games of **Trivia**"
                    },
                    new VarCriterion(Stats.Trivia.TimesWon, 1)
                    {
                        Name = "Win a game of **Trivia**"
                    },
                },
                Type = QuestType.Daily,
                Reward = new Reward
                {
                    Money = 25,
                    Exp = 50
                }
            }
        };

        public static readonly HashSet<Vendor> Vendors = new HashSet<Vendor>
        {
            new Vendor
            {
                Name = "V3-NDR",
                PreferredGroups = new []
                {
                    Ids.Groups.Palette
                },
                OnEnter = new []
                {
                    "Welcome.",
                    "What can I do for you on this fine hour?"
                }
            }
        };

        public static readonly HashSet<Shop> Shops = new HashSet<Shop>
        {
            /*
            new Shop
            {
                Id = "token_emporium",
                Name = "Token Emporium",
                Quote = "Purchase specialized goods here.",
                Allow = ShopAllow.Buy,
                Catalog = new CatalogGenerator
                {
                    Size = 1,
                    MaxDiscountsAllowed = 0,
                    MaxSpecialsAllowed = 0,
                    Entries = new List<CatalogEntry>
                    {
                        new CatalogEntry
                        {

                        }
                    }
                }
            },
            */
            new Shop
            {
                Id = "typo_tavern",
                Name = "Typograph Tavern",
                Quote = "Purchase font families and text components here.",
                Allow = ShopAllow.Buy,
                SellDeduction = 40,
                AllowedSellGroups = new List<string>
                {
                    Ids.Groups.Font
                },
                AllowedCurrency = CurrencyType.Money,
                ToVisit = u => u.Level == 10 && Var.All(u, VarOp.GEQ, 2, ShopHelper.GetTierId(Ids.Shops.ChromeCove), ShopHelper.GetTierId(Ids.Shops.TinkerTent)),
                Catalog = new CatalogGenerator
                {
                    Size = 2,
                    MaxDiscountsAllowed = 0,
                    MaxSpecialsAllowed = 1,
                    Entries = new List<CatalogEntry>
                    {
                        new CatalogEntry
                        {
                            ItemId = Ids.Items.FontMonori,
                            Weight = 10,

                        },
                        new CatalogEntry
                        {
                            ItemId = Ids.Items.FontFoxtrot,
                            Weight = 4,
                            IsSpecial = true,
                            RequiredTier = 2
                        },
                        new CatalogEntry
                        {
                            ItemId = Ids.Items.FontDelta,
                            Weight = 8
                        }
                    }
                }
            },
            new Shop
            {
                Id = "tinker_tent",
                Name = "Tinker's Tent",
                Quote = "Purchase components and crafting materials here.",
                Allow = ShopAllow.Buy,
                SellDeduction = 60,
                CriteriaTiers = new Dictionary<long, List<VarCriterion>>
                {
                    [2] = new List<VarCriterion>
                    {
                        new VarCriterion(ShopHelper.GetUniqueVisitId(Ids.Shops.TinkerTent), 10),
                        new VarCriterion(ShopHelper.GetTotalBoughtId(Ids.Shops.TinkerTent), 5)
                    }
                },
                AllowedSellGroups = new List<string>
                {
                    Ids.Groups.Component,
                    Ids.Groups.Tool
                },
                Catalog = new CatalogGenerator
                {
                    Size = 2,
                    MaxDiscountsAllowed = 0,
                    MaxSpecialsAllowed = 1,
                    Entries = new List<CatalogEntry>
                    {
                        new CatalogEntry
                        {
                            ItemId = Ids.Items.ToolGiftWrap,
                            Weight = 20
                        },
                        new CatalogEntry
                        {
                            ItemId = Ids.Items.ComponentSmearKit,
                            Weight = 8,
                            IsSpecial = true
                        },
                        new CatalogEntry
                        {
                            ItemId = Ids.Items.ComponentBlendKit,
                            Weight = 2,
                            IsSpecial = true
                        },
                        new CatalogEntry
                        {
                            ItemId = Ids.Items.ComponentNeonKit,
                            Weight = 4,
                            IsSpecial = true
                        },
                        new CatalogEntry
                        {
                            ItemId = Ids.Items.ComponentDimmerKit,
                            Weight = 4,
                            IsSpecial = true
                        }
                    }
                }
            },
            new Shop
            {
                Id = "boost_blight",
                Name = "Booster's Blight",
                Quote = "Purchase an assortment of boosters here.",
                Allow = ShopAllow.All,
                SellDeduction = 50,
                AllowedSellGroups = new List<string>
                {
                    Ids.Groups.Booster
                },
                Catalog = new CatalogGenerator
                {
                    Size = 1,
                    MaxDiscountsAllowed = 0,
                    MaxSpecialsAllowed = 0,
                    Entries = new List<CatalogEntry>
                    {
                        new CatalogEntry
                        {
                            ItemId = Ids.Items.BoosterDebtBlocker,
                            Weight = 10
                        },
                        new CatalogEntry
                        {
                            ItemId = Ids.Items.BoosterOriteBooster,
                            Weight = 5
                        }
                    }
                }
            },
            new Shop
            {
                Id = Ids.Shops.ChromeCove,
                Name = "Chromatic Cove",
                Quote = "The reliable place to purchase color palettes.",
                CriteriaTiers = new Dictionary<long, List<VarCriterion>>
                {
                    [2] = new List<VarCriterion>
                    {
                        new VarCriterion(ShopHelper.GetUniqueVisitId(Ids.Shops.ChromeCove), 10),
                        new VarCriterion(ShopHelper.GetTotalBoughtId(Ids.Shops.ChromeCove), 5)
                    },
                    [3] = new List<VarCriterion>
                    {
                        new VarCriterion(ShopHelper.GetUniqueVisitId(Ids.Shops.ChromeCove), 20),
                        new VarCriterion(ShopHelper.GetTotalSpentId(Ids.Shops.ChromeCove), 7500),
                        new VarCriterion(ShopHelper.GetTotalBoughtId(Ids.Shops.ChromeCove), 10)
                    }
                },
                Catalog = new CatalogGenerator
                {
                    Size = 2,
                    MaxDiscountsAllowed = 1,
                    MaxSpecialsAllowed = 1,
                    Entries = new List<CatalogEntry>
                    {
                        new CatalogEntry
                        {
                            ItemId = Ids.Items.PaletteGammaGreen,
                            Weight = 60,
                            MinDiscount = 5,
                            MaxDiscount = 10,
                            DiscountChance = 0.4f
                        },
                        new CatalogEntry
                        {
                            ItemId = Ids.Items.PaletteWumpite,
                            Weight = 15,
                            MinDiscount = 1,
                            MaxDiscount = 5,
                            DiscountChance = 0.3f,
                            MaxAllowed = 1,
                            RequiredTier = 2,
                            IsSpecial = true
                        },
                        new CatalogEntry
                        {
                            ItemId = Ids.Items.PaletteCrimson,
                            Weight = 75,
                            MinDiscount = 5,
                            MaxDiscount = 10,
                            DiscountChance = 0.5f
                        },
                        new CatalogEntry
                        {
                            ItemId = Ids.Items.PaletteGlass,
                            Weight = 4,
                            MaxAllowed = 1,
                            IsSpecial = true
                        },
                        new CatalogEntry
                        {
                            ItemId = Ids.Items.PaletteLemon,
                            Weight = 8,
                            MaxAllowed = 1,
                            RequiredTier = 2
                        },
                        new CatalogEntry
                        {
                            ItemId = Ids.Items.PaletteOceanic,
                            Weight = 6,
                            MaxAllowed = 1,
                            RequiredTier = 2
                        },
                        new CatalogEntry
                        {
                            ItemId = Ids.Items.PaletteAmber,
                            Weight = 3,
                            CostScale = 1.25f,
                            MaxAllowed = 1,
                            RequiredTier = 2,
                            ForceAtTier = true
                        },
                        new CatalogEntry
                        {
                            ItemId = Ids.Items.PaletteTaffy,
                            Weight = 3,
                            CostScale = 1.25f,
                            MaxAllowed = 1,
                            RequiredTier = 2,
                            ForceAtTier = true
                        }
                    }
                },
                Allow = ShopAllow.All,
                SellDeduction = 50,
                AllowedSellGroups = new List<string>
                {
                    Ids.Groups.Palette
                }
            }
        };

        public static readonly HashSet<Merit> Merits = new HashSet<Merit>
        {
            new Merit
            {
                Id = "currency:tidal_taxes",
                Icon = "🌊",
                Name = "Tidal Taxes",
                Rank = MeritRank.Gold,
                Tags = MeritTag.Milestone | MeritTag.Secret,
                Score = 100,
                LockQuote = "Drown in the currency that prevents hope.",
                Quote = $"Reached {Icons.Debt} **10,000** at one point.",
                Criteria = user => user.Debt >= 10000
            },
            new Merit
            {
                Id = "currency:raging_riches",
                Icon = "🔥",
                Name = "Raging Riches",
                Rank = MeritRank.Gold,
                Tags = MeritTag.Milestone | MeritTag.Secret,
                Score = 100,
                LockQuote = "Bathe yourself in **Orite**.",
                Quote = $"Reached {Icons.Balance} **10,000** at one point.",
                Criteria = user => user.Balance >= 10000
            },
            new Merit
            {
                Id = "currency:chip_collector",
                Icon = "💰",
                Name = "Chip Collector",
                Rank = MeritRank.Gold,
                Tags = MeritTag.Milestone | MeritTag.Secret,
                Score = 100,
                LockQuote = "Go against the odds of the casino.",
                Quote = $"Reached {Icons.Chips} **10,000** at one point.",
                Criteria = user => user.ChipBalance >= 10000
            },
            new Merit
            {
                Id = "exp:freshly_grown",
                Icon = "🌱",
                Name = "Freshly Grown",
                Rank = MeritRank.Bronze,
                Tags = MeritTag.Milestone,
                Score = 25,
                LockQuote = "You have escaped the depths of the commonfolk, reaching a new tier of firmware.",// "Your level means nothing.",
                Quote = "Reached v**0.10**.",
                Criteria = user => user.Level >= 10
            },
            new Merit
            {
                Id = "exp:rising_rookie",
                Icon = "🌾",
                Name = "Rising Rookie",
                Rank = MeritRank.Silver,
                Tags = MeritTag.Milestone | MeritTag.Secret,
                Score = 50,
                LockQuote = "Climb up wild lands and make yourself known.", // "Your level has not yet proved your worth.",
                Quote = "Reached v**0.25**",
                Criteria = user => user.Level >= 25
            },
            new Merit
            {
                Id = "exp:astral_advocate",
                Icon = "🎋",
                Name = "Astral Advocate",
                Rank = MeritRank.Gold,
                Tags = MeritTag.Milestone | MeritTag.Secret,
                Score = 100,
                LockQuote = "Reach what many consider the limit.",// "Your level status must reach for the clouds.",
                Quote = "Reached v**0.50**",
                Criteria = user => user.Level >= 50
            },
            new Merit
            {
                Id = "exp:space_veteran",
                Icon = "🌸",
                Name = "Celestial Veteran",
                Rank = MeritRank.Platinum,
                Tags = MeritTag.Milestone | MeritTag.Secret,
                Score = 250,
                LockQuote = "Strive past the moon.",
                Quote = "Reached v**0.100**.",
                Criteria = user => user.Level >= 100
            },
            new Merit
            {
                Id = "common:prisma_infusion",
                Icon = "🌈",
                Name = "Prisma Infusion",
                Rank = MeritRank.Diamond,
                Tags = MeritTag.Secret,
                Score = 500,
                LockQuote = "Reveal the secrets of the prismatic horizon.",
                Quote = "Collected all known colors."
            },
            new Merit
            {
                Id = "common:color_theory",
                Icon = "🟧",
                Name = "Color Theory",
                Rank = MeritRank.Silver,
                Score = 25,
                LockQuote = "Reveal a new piece of the color wheel.",
                Quote = "Blended two colors together to create a new color."
            },
            new Merit
            {
                Id = "common:tinkerer",
                Icon = "🔨",
                Name = "Tinker Tot",
                Rank = MeritRank.Bronze,
                Tags = MeritTag.Milestone,
                Score = 5,
                LockQuote = "Grab your materials and make something new.",
                Quote = "Crafted an item.",
                Criteria = user => user.GetVar(Stats.Common.ItemsCrafted) > 0
            },
            new Merit
            {
                Id = "common:trade_beginner",
                Icon = "🔂",
                Name = "Trading Beginner",
                Rank = MeritRank.Bronze,
                Tags = MeritTag.Milestone,
                Score = 5,
                Quote = "Traded with another user.",
                Criteria = user => user.GetVar(Stats.Common.TimesTraded) > 0
            },
            new Merit
            {
                Id = "common:bronze_heart",
                Icon = "🤎",
                Name = "Bronze Heart",
                Rank = MeritRank.Bronze,
                Tags = MeritTag.Milestone,
                Score = 5,
                LockQuote = "You are a kind and giving entity.",
                Quote = "Gifted an item.",
                Criteria = user => user.GetVar(Stats.Common.ItemsGifted) > 0
            },
            new Merit
            {
                Id = "common:silver_heart",
                Icon = "🤍",
                Name = "Silver Heart",
                Rank = MeritRank.Silver,
                Tags = MeritTag.Milestone,
                Score = 50,
                LockQuote = "Your heart grows with generosity.",
                Quote = "Gifted over **100** items away.",
                Criteria = user => user.GetVar(Stats.Common.ItemsGifted) > 100
            },
            new Merit
            {
                Id = "common:golden_heart",
                Icon = "💛",
                Name = "Golden Heart",
                Rank = MeritRank.Gold,
                Tags = MeritTag.Milestone | MeritTag.Secret,
                Score = 50,
                LockQuote = "Your giving heart must radiate gold.",
                Quote = "Gifted over **500** items away.",
                Criteria = user => user.GetVar(Stats.Common.ItemsGifted) > 500
            },
            new Merit
            {
                Id = "common:ignition",
                Icon = "🕯️",
                Name = "Ignition",
                Rank = MeritRank.Bronze,
                Tags = MeritTag.Milestone,
                Score = 5,
                LockQuote = "Light the fuse to boost.",
                Quote = "Activated a booster.",
                Criteria = user => user.Boosters.Count > 0
            },
            new Merit
            {
                Id = "common:progress_pioneer",
                Icon = "🚝",
                Name = "Progression Pioneer",
                Rank = MeritRank.Diamond,
                Tags = MeritTag.Exclusive | MeritTag.Secret,
                Score = 100,
                LockQuote = "You may not have always been, but your contribution is still noted.",
                Quote = "You were there at the start, carving the path to the future."
            },
            new Merit
            {
                Id = "casino:liquidation",
                Name = "Liquidation",
                Rank = MeritRank.Bronze,
                Score = 5,
                LockQuote = "Your requests have been met with liquid assets.",
                Quote = "You were given gold in **Gimi**.",
                Criteria = user => user.GetVar(Stats.Gimi.TimesGold) > 0
            },
            new Merit
            {
                Id = "casino:deprivation",
                Name = "Deprivation",
                Rank = MeritRank.Bronze,
                Score = 5,
                LockQuote = "Your greed has led you to perish under the moonlight.",
                Quote = "You were cursed in **Gimi**.",
                Criteria = user => user.GetVar(Stats.Gimi.TimesCursed) > 0
            },
            new Merit
            {
                Id = "casino:golden_touch",
                Name = "Golden Touch",
                Rank = MeritRank.Gold,
                Tags = MeritTag.Secret,
                Score = 50,
                LockQuote = "Midas has blessed you with riches.",
                Quote = "You were given gold twice in a row in **Gimi**.",
                Criteria = user => user.GetVar(Stats.Gimi.LongestGold) >= 2
            },
            new Merit
            {
                Id = "casino:pandoras_box",
                Name = "Pandora's Box",
                Rank = MeritRank.Gold,
                Tags = MeritTag.Secret,
                Score = 50,
                LockQuote = "Your ruthless requests released the worst of this world.",
                Quote = "You were cursed twice in a row in **Gimi**.",
                Criteria = user => user.GetVar(Stats.Gimi.LongestCurse) >= 2
            },
            new Merit
            {
                Id = "casino:lucky_guesses",
                Name = "Doubly Doubt",
                Rank = MeritRank.Silver,
                Tags = MeritTag.Secret,
                Score = 25,
                LockQuote = "Accuracy and consistency is needed in **Doubly**.",
                Quote = "You have won 3 rounds of **Doubly** in a row, where the dying tick was your exact guess.",
                Criteria = user => user.GetVar(Stats.Doubler.LongestWinExact) >= 3
            },
            new Merit
            {
                Id = "casino:gimi_beginner",
                Name = "Gimi Beginner",
                Rank = MeritRank.Bronze,
                Tags = MeritTag.Milestone,
                Score = 10,
                LockQuote = "Your addiction shall soon be revealed.",
                Quote = "Played **100** rounds of **Gimi**.",
                Criteria = user => user.GetVar(Stats.Gimi.TimesPlayed) >= 100
            },
            new Merit
            {
                Id = "casino:gimi_clover",
                Icon = "☘️",
                Name = "Clover of Gimi",
                Rank = MeritRank.Bronze,
                Tags = MeritTag.Secret,
                Score = 10,
                LockQuote = "This clover symbolizes luck, as you shall soon see.",
                Quote = "Won over **20** times in a row playing **Gimi**.",
                Criteria = user => user.GetVar(Stats.Gimi.LongestWin) >= 20,
                Hidden = true
            },
            new Merit
            {
                Id = "casino:gimi_curse",
                Icon = "🧿",
                Name = "Curse of Gimi",
                Rank = MeritRank.Bronze,
                Tags = MeritTag.Secret,
                Score = 10,
                LockQuote = "It seems a curse of negated luck hovers close.",
                Quote = "Lost over **20** times in a row playing **Gimi**.",
                Criteria = user => user.GetVar(Stats.Gimi.LongestLoss) >= 20,
                Hidden = true
            },
            new Merit
            {
                Id = "casino:tick_clover",
                Icon = "☘️",
                Name = "Clover of Doubler",
                Rank = MeritRank.Silver,
                Tags = MeritTag.Secret,
                Score = 25,
                LockQuote = "Moderate luck is needed in **Doubly**.",
                Quote = "Won over **20** times in a row in **Doubly**.",
                Criteria = user => user.GetVar(Stats.Doubler.LongestWin) >= 20
            },
            new Merit
            {
                Id = "casino:tick_exact_clover",
                Icon = "🏵️",
                Name = "Golden Clover of Doubler",
                Rank = MeritRank.Gold,
                Tags = MeritTag.Secret,
                Score = 50,
                LockQuote = "Extreme luck is needed in **Doubly**.",
                Quote = "Won over **20** times in a row in **Doubly**, guessing the exact tick.",
                Criteria = user => user.GetVar(Stats.Doubler.LongestWinExact) >= 20
            },
            new Merit
            {
                Id = "casino:tick_beginner",
                Name = "Doubler Beginner",
                Rank = MeritRank.Bronze,
                Tags = MeritTag.Milestone,
                Score = 10,
                Quote = "Played **100** rounds of **Doubly**.",
                Criteria = user => user.GetVar(Stats.Doubler.TimesPlayed) >= 100
            },
            new Merit
            {
                Id = "casino:gimi_advocate",
                Name = "Gimi Advocate",
                Rank = MeritRank.Silver,
                Tags = MeritTag.Milestone,
                Score = 25,
                LockQuote = "Despite the possible financial ruin, you keep playing.",
                Quote = "Played **1,000** rounds of **Gimi**.",
                Criteria = user => user.GetVar(Stats.Gimi.TimesPlayed) >= 1000
            },
            new Merit
            {
                Id = "casino:gimi_expert",
                Name = "Gimi Expert",
                Rank = MeritRank.Gold,
                Tags = MeritTag.Milestone | MeritTag.Secret,
                Score = 50,
                LockQuote = "The addiction for the wealth you seek scares me.",
                Quote = "Played **5,000** rounds of **Gimi**.",
                Criteria = user => user.GetVar(Stats.Gimi.TimesPlayed) >= 5000,
                Reward = new Reward
                {
                    Money = 1000,
                    Exp = 2000
                }
            },
            new Merit
            {
                Id = "casino:gimi_maniac",
                Icon = "⚗️",
                Name = "Gimi Maniac",
                Rank = MeritRank.Diamond,
                Tags = MeritTag.Milestone | MeritTag.Secret,
                Score = 250,
                LockQuote = "You have reached the end of the road. Blessings and curses are part of your life now.",
                Quote = "Played **10,000** rounds of **Gimi**.",
                Criteria = user => user.GetVar(Stats.Gimi.TimesPlayed) >= 10000,
                Reward = new Reward
                {
                    Money = 250,
                    Exp = 4000,
                    ItemIds = new Dictionary<string, int>
                    {
                        [Ids.Items.AutomatonGimi] = 1
                    }
                }
            },
            new Merit
            {
                Id = "common:weekly_worker",
                Icon = "✨",
                Name = "Weekly Worker",
                Rank = MeritRank.Bronze,
                Tags = MeritTag.Milestone,
                Score = 7,
                LockQuote = "Maybe if you stopped by a bit more often, I'd get to know you better.",
                Quote = "Maintained a daily streak for **7** days.",
                Criteria = user => user.GetVar(Stats.Common.LongestDailyStreak) >= 7
            },
            new Merit
            {
                Id = "common:monthly_advocate",
                Icon = "⭐",
                Name = "Monthly Advocate",
                Rank = MeritRank.Gold,
                Tags = MeritTag.Milestone,
                Score = 30,
                LockQuote = "You've been becoming a lot more active, and I like to see that.",
                Quote = "Maintained a daily streak for **30** days.",
                Criteria = user => user.GetVar(Stats.Common.LongestDailyStreak) >= 30
            },
            new Merit
            {
                Id = "common:daily_automaton",
                Icon = "💫",
                Name = "Daily Automation",
                Rank = MeritRank.Platinum,
                Tags = MeritTag.Milestone | MeritTag.Secret,
                Score = 100,
                LockQuote = "You're still here. For that, I am grateful.",
                Quote = "Maintained a daily streak for **100** days.",
                Criteria = user => user.GetVar(Stats.Common.LongestDailyStreak) >= 100
            },
            new Merit
            {
                Id = "common:perfect_attendance",
                Icon = "🌟",
                Name = "Perfect Attendance",
                Rank = MeritRank.Diamond,
                Tags = MeritTag.Milestone | MeritTag.Secret,
                Score = 365,
                LockQuote = "You were always here when it was needed.",
                Quote = "Maintained a daily streak for *365** days.",
                Criteria = user => user.GetVar(Stats.Common.LongestDailyStreak) >= 365
            },
            new Merit
            {
                Id = "common:escaping_trouble",
                Icon = "☎️",
                Name = "Escaping Trouble",
                Rank = MeritRank.Bronze,
                Score = 10,
                LockQuote = "With a simple call to the guardian of debt, your troubles will soon fade into the void.",
                Quote = "Used a **Pocket Lawyer** to remove any excess debt.",
                // TODO: Make sure that the user's debt was changed as well
                // Could be a trigger activation that is specified in the item usage
                Criteria = user => user.GetVar($"{Ids.Items.PocketLawyer}:times_used") >= 1
            } // Create a merit for recovering funds from a heavy loss
        };

        public static readonly HashSet<Recipe> Recipes = new HashSet<Recipe>
        {
            new Recipe
            {
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Ids.Items.PaletteGlass, 1),
                    new RecipeComponent(Ids.Items.PaletteWumpite, 1),
                    new RecipeComponent(Ids.Items.ComponentSmearKit, 1)
                },
                Result = new RecipeComponent(Ids.Items.PaletteGlossyWumpite, 1)
            },
            new Recipe
            {
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Ids.Items.PaletteCrimson, 1),
                    new RecipeComponent(Ids.Items.PaletteLemon, 1),
                    new RecipeComponent(Ids.Items.ComponentSmearKit, 1)
                },
                Result = new RecipeComponent(Ids.Items.PaletteBurntLemon, 1)
            },
            new Recipe
            {
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Ids.Items.PaletteLemon, 1),
                    new RecipeComponent(Ids.Items.PaletteCrimson, 1),
                    new RecipeComponent(Ids.Items.ComponentBlendKit, 1)
                },
                Result = new RecipeComponent(Ids.Items.PaletteAmber, 1)
            },
            new Recipe
            {
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Ids.Items.PaletteWumpite, 1),
                    new RecipeComponent(Ids.Items.PaletteLemon, 1),
                    new RecipeComponent(Ids.Items.ComponentBlendKit, 1)
                },
                Result = new RecipeComponent(Ids.Items.PaletteChocolate, 1)
            },
            new Recipe
            {
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Ids.Items.PaletteCrimson, 1),
                    new RecipeComponent(Ids.Items.PaletteGammaGreen, 1),
                    new RecipeComponent(Ids.Items.ComponentBlendKit, 1)
                },
                Result = new RecipeComponent(Ids.Items.PaletteChocolate, 1)
            },
            new Recipe
            {
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Ids.Items.PaletteCrimson, 1),
                    new RecipeComponent(Ids.Items.PaletteLemon, 1),
                    new RecipeComponent(Ids.Items.PaletteOceanic, 1),
                    new RecipeComponent(Ids.Items.ComponentBlendKit, 1)
                },
                Result = new RecipeComponent(Ids.Items.PaletteChocolate, 1)
            },
            new Recipe
            {
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Ids.Items.PaletteAmber, 1),
                    new RecipeComponent(Ids.Items.PaletteOceanic, 1),
                    new RecipeComponent(Ids.Items.ComponentBlendKit, 1)
                },
                Result = new RecipeComponent(Ids.Items.PaletteChocolate, 1)
            },
            new Recipe
            {
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Ids.Items.PaletteCrimson, 1),
                    new RecipeComponent(Ids.Items.ComponentNeonKit, 2)
                },
                Result = new RecipeComponent(Ids.Items.PaletteTaffy, 1)
            },
            new Recipe
            {
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Ids.Items.PaletteLemon, 1),
                    new RecipeComponent(Ids.Items.PaletteOceanic, 1),
                    new RecipeComponent(Ids.Items.ComponentBlendKit, 1)
                },
                Result = new RecipeComponent(Ids.Items.PaletteGammaGreen, 1)
            },
            new Recipe
            {
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent(Ids.Items.ComponentNeonKit, 8),
                    new RecipeComponent(Ids.Items.ComponentBlendKit, 1),
                },
                Result = new RecipeComponent(Ids.Items.PalettePolarity, 1)
            }
        };

        public static readonly HashSet<Item> Items = new HashSet<Item>
        {
            new Item
            {
                Id = Ids.Items.InternalSealedItem,
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
                Id = Ids.Items.InternalPackage,
                GroupId = "$",
                Icon = "📦",
                Name = "Package",
                Quotes = new List<string>
                {
                    "An sealed container received from the catalog."
                }
            },
            new Item
            {
                Id = Ids.Items.InternalUnknown,
                GroupId = "$",
                Name = "Unknown Cluster",
                Quotes = new List<string>
                {
                    "This item no longer seems to be of this world.",
                    "It emits a garbled signal."
                }
            },
            new Item
            {
                Id = "t_nt",
                GroupId = Ids.Groups.Tool,
                Icon = "🉐",
                Name = "Name Tag",
                Rarity = ItemRarity.Uncommon,
                Tags = ItemTag.Equipment | ItemTag.Usable | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable,
                Value = 750,
                Size = 50,
                Usage = new ItemUsage
                {
                    Durability = 1,
                    DeleteTriggers = DeleteTriggers.Break,
                    Action = delegate(UsageContext ctx)
                    {
                        if (!Check.NotNull(ctx.Input))
                            return UsageResult.FromError("An item data instance must be specified.");

                        var reader = new StringReader(ctx.Input);

                        if (!reader.CanRead())
                            return UsageResult.FromError("An item data instance must be specified.");

                        string id = reader.ReadUnquotedString();

                        if (!reader.CanRead())
                        {
                            return UsageResult.FromError("A name must be specified.");
                        }

                        reader.SkipWhiteSpace();
                        string name = reader.GetRemaining();

                        ItemData data = ItemHelper.GetItemData(ctx.User, id);

                        if (data == null)
                            return  UsageResult.FromError("An unknown data instance was specified.");

                        Item item = ItemHelper.GetItem(data.Id);

                        if (!(ItemHelper.IsUnique(item) && item.Tags.HasFlag(ItemTag.Renamable)))
                            return UsageResult.FromError("The specified data instance does not support renaming.");

                        if (!VerifyName(name, out string reason))
                            return UsageResult.FromError(reason);

                        data.Data.Name = name;

                        return UsageResult.FromSuccess($"Successfully renamed **{item.GetName()}** to **{data.Data.Name}**");
                    }
                }
            },
            new Item
            {
                Id = "t_gc",
                GroupId = Ids.Groups.Tool,
                Icon = "🧺",
                Name = "Gift Catapult",
                Summary = "Randomly gifts an account with the specified item.",
                Rarity = ItemRarity.Rare,
                Tags = ItemTag.Equipment | ItemTag.Usable | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable,
                Value = 1250,
                Size = 100
            },
            new Item
            {
                Id = Ids.Items.ComponentNeonKit,
                GroupId = Ids.Groups.Component,
                Name = "Neon Kit",
                Summary = "A component used to brighten the value of a color.",
                Quotes = new List<string>
                {
                    "It cries with a strong vibrant call of chromacy."
                },
                Rarity = ItemRarity.Rare,
                Tags = ItemTag.Material | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable | ItemTag.Orderable,
                Value = 1250,
                Size = 250
            },
            new Item
            {
                Id = Ids.Items.ComponentDimmerKit,
                GroupId = Ids.Groups.Component,
                Tags = ItemTag.Material | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable | ItemTag.Orderable,
                Rarity = ItemRarity.Rare,
                Value = 1250,
                Size = 250,

                Name = "Dimmer Kit",
                Summary = "A component used to darken the value of a color.",
                Quotes = new List<string>
                {
                    "It absorbs every color surrounding it."
                }
            },
            new Item
            {
                Id = Ids.Items.ComponentSmearKit,
                GroupId = Ids.Groups.Component,
                Tags = ItemTag.Material | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable | ItemTag.Orderable,
                Rarity = ItemRarity.Uncommon,
                Value = 750,
                Size = 250,

                Name = "Smear Kit",
                Summary = "Often used to create a hybrid of your favorite colors.",
                Quotes = new List<string>
                {
                    "It swings open to reveal two color slots with a churning device in the center."
                }
            },
            new Item
            {
                Id = Ids.Items.ComponentBlendKit,
                GroupId = Ids.Groups.Component,
                Tags = ItemTag.Material | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable | ItemTag.Orderable,
                Rarity = ItemRarity.Rare,
                Value = 1000,
                Size = 250,

                Name = "Blend Kit",
                Summary = "Used to give way to a brand new color from two existing ones.",
                Quotes = new List<string>
                {
                    "It pops open to reveal two color slots that point towards a rapidly spinning motor."
                }
            },
            new Item
            {
                Id = Ids.Items.AutomatonGimi,
                Name = "Gimi",
                Summary = "Gives you the ability to execute the Gimi command a specified number of times. Please note that you are required to wait a second on each execution.",
                Quotes = new List<string>
                {
                    "It echoes a tick loud enough to break the sound barrier."
                },
                GroupId = Ids.Groups.Automaton,
                Rarity = ItemRarity.Myth,
                Tags = ItemTag.Equipment | ItemTag.Cloneable | ItemTag.Sealable | ItemTag.Renamable,
                Value = 100000,
                TradeLimit = 0,
                Size = 1000,
                OwnLimit = 1
            },
            new Item
            {
                Id = Ids.Items.LayoutClassic,
                Name = "Classic",
                Quotes = new List<string>
                {
                    "The originating card format that always delivers clean and direct information."
                },
                GroupId = Ids.Groups.Layout,
                Rarity = ItemRarity.Common,
                Tags = ItemTag.Decorator | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable,
                Value = 0,
                TradeLimit = 0,
                Size = 10,
                OwnLimit = 1
            },
            new Item
            {
                Id = Ids.Items.LayoutMinimal,
                Name = "Minimal",
                Quotes = new List<string>
                {
                    "Simplicity in a condensed format."
                },
                GroupId = Ids.Groups.Layout,
                Rarity = ItemRarity.Myth,
                Tags = ItemTag.Decorator | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable,
                Value = 0,
                TradeLimit = 0,
                Size = 10,
                OwnLimit = 1
            },
            new Item
            {
                Id = Ids.Items.ToolGiftWrap,
                GroupId = Ids.Groups.Tool,
                Icon = "🎀",
                Name = "Gift Wrap",
                Quotes = new List<string>
                {
                    "Add a little mystery to your gifting!"
                },
                Summary = "Wraps an item to seal its contents.",
                Rarity = ItemRarity.Common,
                Tags = ItemTag.Equipment | ItemTag.Usable | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable | ItemTag.Orderable,
                Value = 25,
                BypassCriteriaOnTrade = true,
                Size = 10,
                Usage = new ItemUsage
                {
                    Durability = 1,
                    DeleteTriggers = DeleteTriggers.Break,
                    Action = delegate(UsageContext ctx)
                    {
                        if (!Check.NotNull(ctx.Input))
                            return UsageResult.FromError("An item data instance must be specified.");

                        var reader = new StringReader(ctx.Input);

                        if (!reader.CanRead())
                            return UsageResult.FromError("An item data instance must be specified.");

                        string id = reader.ReadUnquotedString();

                        // The amount to wrap. If unspecified, the default is 1.
                        int amount = 1;

                        if (reader.CanRead())
                        {
                            reader.SkipWhiteSpace();
                            int.TryParse(reader.ReadUnquotedString(), out amount);
                        }

                        ItemData data = ItemHelper.GetItemData(ctx.User, id);

                        if (data == null)
                            return  UsageResult.FromError("An unknown data instance was specified.");

                        if (amount < 0)
                            amount = 1;
                        else if (amount > data.Count)
                            amount = data.Count;

                        if (ItemHelper.IsUnique(data.Id))
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

                        return UsageResult.FromSuccess($"> Successfully wrapped {amount:##,0} **{ItemHelper.NameOf(data.Id)}**.");
                    }
                }
            },
            new Item
            {
                Id = Ids.Items.BoosterDebtBlocker,
                Name = "Debt Blocker",
                Quotes = new List<string>
                {
                    "It creates a small shield when near debt."
                },
                GroupId = Ids.Groups.Booster,
                Rarity = ItemRarity.Uncommon,
                Tags = ItemTag.Usable | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable | ItemTag.Modifier | ItemTag.Orderable,
                Value = 525,
                BypassCriteriaOnTrade = true,
                Size = 75,
                Usage = new ItemUsage
                {
                    Durability = 1,
                    DeleteTriggers = DeleteTriggers.Break,
                    Action = delegate(UsageContext ctx)
                    {
                        var booster = new BoostData(Ids.Items.BoosterDebtBlocker, BoostTarget.Debt, -0.2f, 20);

                        if (!TryApplyBooster(ctx.User, booster))
                            return UsageResult.FromError("> You already have too many active modifiers.");

                        return UsageResult.FromSuccess("> The **Debt Blocker** opens up, revealing a crystal clear shield that surrounds you.");
                    },
                    OnBreak = user => user.Boosters.Add(new BoostData(BoostTarget.Debt, 0.1f, 5))
                },
                OwnLimit = 99
            },
            new Item
            {
                Id = Ids.Items.BoosterDaily,
                Name = "Daily Influence",
                Summary = "Boosts the Orite you receive from dailies by 100%.", // "A booster that heavily amplifies the Orite you receive from dailies.",
                Quotes = new List<string>
                {
                    "It shows that you know your way around perfect attendance."
                },
                GroupId = Ids.Groups.Booster,
                Rarity = ItemRarity.Rare,
                Tags = ItemTag.Usable | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable | ItemTag.Modifier,
                Value = 1000,
                Size = 150,
                Usage = new ItemUsage
                {
                    Action = delegate(UsageContext ctx)
                    {
                        var booster = new BoostData(Ids.Items.BoosterDaily, BoostTarget.Money, 1, 30);

                        if (!TryApplyBooster(ctx.User, booster))
                            return UsageResult.FromError("> You already have too many active modifiers.");

                        return UsageResult.FromSuccess("The influence of this slip is carefully fused with your attendance slip.");
                    },
                    EquipTarget = EquipTarget.Charm
                },
                OwnLimit = 5
            },
            new Item
            {
                Id = Ids.Items.BoosterOriteBooster,
                Name = "Orite Booster",
                Summary = "Boosts any income related to Orite by 20%.",
                Quotes = new List<string>
                {
                    "It amplifies the value of Orite when given close exposure."
                },
                GroupId = Ids.Groups.Booster,
                Rarity = ItemRarity.Uncommon,
                Tags = ItemTag.Usable | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable | ItemTag.Modifier | ItemTag.Orderable,
                Value = 500,
                BypassCriteriaOnTrade = true,
                Size = 75,
                Usage = new ItemUsage
                {
                    Durability = 1,
                    DeleteTriggers = DeleteTriggers.Break,
                    Action = delegate(UsageContext ctx)
                    {
                        // TODO: Instead of applying a boost data, mark the ITEM DATA INSTANCE as equipped in a slot.
                        // You can then call a trigger, which will return all equipped items that activate from that trigger
                        var booster = new BoostData(Ids.Items.BoosterOriteBooster, BoostTarget.Money, 0.2f, 20);

                        if (!TryApplyBooster(ctx.User, booster))
                            return UsageResult.FromError("> You already have too many active modifiers.");

                        return UsageResult.FromSuccess("> The **Orite Booster** cracks open and infuses with your very well-being.");
                    }
                    // OnBreak = user => user.Boosters.Add(new BoostData(BoostType.Debt, 0.1f, 20))
                },
                OwnLimit = 99
            },
            new Item
            {
                Id = Ids.Items.PocketLawyer,
                Name = "Pocket Lawyer",
                Summary = "A summon that completely wipes all debt from a user.",
                Quotes = new List<string>
                {
                    "With my assistance, ORS doesn't stand a chance.",
                    "You'll get the chance to dispute in court, don't worry."
                },
                GroupId = Ids.Groups.Summon,
                Rarity = ItemRarity.Myth,
                Tags = ItemTag.Usable | ItemTag.Cloneable | ItemTag.Sealable,
                Value = 750,
                TradeLimit = 0,
                BypassCriteriaOnTrade = true,
                Size = 100,
                MemoId = 1,
                Memo = "The pocket-sized lawyer is a hollow shell of what it once was. Before ORS conquered over all, Po Ket was just an average fellow, amazed by the wonders of financial advancement.",
                ResearchTiers = new Dictionary<int, string>
                {
                    [2] = $"Removal of {Format.Number(750, Icons.Debt)} requirement to use",
                    [3] = $"New usage ability (`use su_pl recover`): **25**% chance of recovering previously lost funds (**4** day cooldown if successful)"
                },
                Usage = new ItemUsage
                {
                    Durability = 1,
                    Cooldown = TimeSpan.FromDays(3),
                    DeleteTriggers = DeleteTriggers.Break,
                    Action = delegate (UsageContext ctx)
                    {
                        if (ctx.Input.ToLower() == "recover")
                        {
                            if (ResearchHelper.GetResearchTier(ctx.User, ctx.Item.Id) < 3)
                                return UsageResult.FromError("> You have yet to understand the concept of recovery.");

                            if (ctx.User.LastFundsLost == null)
                                return UsageResult.FromError("> There was nothing to recover.");

                            if (RandomProvider.Instance.Next(0, 100) <= 25)
                            {
                                ctx.User.Give(ctx.User.LastFundsLost.Value, ctx.User.LastFundsLost.Currency);
                                return UsageResult.FromSuccessCooldown(TimeSpan.FromDays(4),
                                    "> Your funds have been recovered from the abyss.",
                                    CooldownTarget.Item);
                            }

                            return UsageResult.FromSuccess("> After several attempted hours of recovery, your funds fade into the darkness.");
                        }

                        if (ctx.User.Debt < 750 && ResearchHelper.GetResearchTier(ctx.User, ctx.Item.Id) < 2)
                            return UsageResult.FromError("> You called for help, but the request remains unanswered. You aren't seen as in need of assistance.");

                        ctx.User.Debt = 0;
                        return UsageResult.FromSuccess("> ⛓️ After a tough fight with **ORS**, **Mr. Pocket** was able to prove your innocence and wipe your debt. You have been freed from the shackles of debt.");
                    }
                },
                OwnLimit = 10
            },
            new Item
            {
                Id = Ids.Items.PaletteGammaGreen,
                Name = "Gamma Green",
                Quotes = new List<string>
                {
                    "It glows with a shade of green similar to uranium."
                },
                GroupId = Ids.Groups.Palette,
                Tags = ItemTag.Equipment | ItemTag.Decorator | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable,
                Value = 1000,
                Size = 50,
                BypassCriteriaOnTrade = true,
                Rarity =  ItemRarity.Common,
                Usage = new ItemUsage
                {
                    Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, PaletteType.GammaGreen)),
                    DeleteTriggers = DeleteTriggers.Break
                },
                OwnLimit = 10
            },
            new Item
            {
                Id = Ids.Items.PaletteCrimson,
                Name = "Crimson",
                Quotes = new List<string>
                {
                    "It thrives in a neon glow of a reddish-purple hue."
                },
                GroupId = Ids.Groups.Palette,
                Tags = ItemTag.Equipment | ItemTag.Decorator | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable | ItemTag.Orderable,
                Value = 1000,
                Size = 50,
                BypassCriteriaOnTrade = true,
                Rarity =  ItemRarity.Common,
                Usage = new ItemUsage
                {
                    Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, PaletteType.Crimson)),
                    DeleteTriggers = DeleteTriggers.Break
                },
                OwnLimit = 10
            },
            new Item
            {
                Id = Ids.Items.PaletteWumpite,
                Name = "Wumpite",
                Quotes = new List<string>
                {
                    "Crafted with the shades of a Wumpus."
                },
                GroupId = Ids.Groups.Palette,
                Tags = ItemTag.Equipment | ItemTag.Decorator | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable,
                Value = 1500,
                Size = 75,
                BypassCriteriaOnTrade = true,
                Rarity =  ItemRarity.Uncommon,
                Usage = new ItemUsage
                {
                    Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, PaletteType.Wumpite)),
                    DeleteTriggers = DeleteTriggers.Break
                },
                OwnLimit = 10
            },
            new Item
            {
                Id = Ids.Items.PalettePolarity,
                Name = "Polarity",
                Quotes = new List<string>
                {
                    "It reminds you of a desolate atmosphere, now frozen over."
                },
                GroupId = Ids.Groups.Palette,
                Tags = ItemTag.Equipment | ItemTag.Decorator | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable,
                Value = 6000,
                Size = 325,
                BypassCriteriaOnTrade = true,
                Rarity =  ItemRarity.Myth,
                Usage = new ItemUsage
                {
                    Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, PaletteType.Polarity)),
                    DeleteTriggers = DeleteTriggers.Break
                },
                OwnLimit = 10
            },
            new Item
            {
                Id = Ids.Items.PaletteGlass,
                Name = "Glass",
                Quotes = new List<string>
                {
                    "It refracts a mixture of light blue to white light."
                },
                GroupId = Ids.Groups.Palette,
                Tags = ItemTag.Equipment | ItemTag.Decorator | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable,
                Value = 3000,
                Size = 75,
                BypassCriteriaOnTrade = true,
                Rarity =  ItemRarity.Rare,
                Usage = new ItemUsage
                {
                    Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, PaletteType.Glass)),
                    DeleteTriggers = DeleteTriggers.Break
                },
                OwnLimit = 10
            },
            new Item
            {
                Id = Ids.Items.PaletteLemon,
                Name = "Lemon",
                Quotes = new List<string>
                {
                    "It exudes a wave of citrus in the air."
                },
                GroupId = Ids.Groups.Palette,
                Tags = ItemTag.Equipment | ItemTag.Decorator | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable | ItemTag.Orderable,
                Value = 2500,
                Size = 75,
                BypassCriteriaOnTrade = true,
                Rarity =  ItemRarity.Rare,
                Usage = new ItemUsage
                {
                    Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, PaletteType.Lemon)),
                    DeleteTriggers = DeleteTriggers.Break
                },
                OwnLimit = 10
            },
            new Item
            {
                Id = Ids.Items.PaletteBurntLemon,
                Name = "Burnt Lemon",
                Quotes = new List<string>
                {
                    "The citrus wave it once provided sparks under the embers."
                },
                GroupId = Ids.Groups.Palette,
                Tags = ItemTag.Equipment | ItemTag.Decorator | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable,
                Value = 5000,
                Size = 75,
                BypassCriteriaOnTrade = true,
                Rarity =  ItemRarity.Rare,
                Usage = new ItemUsage
                {
                    Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, new ColorPalette(PaletteType.Crimson, PaletteType.Lemon))),
                    DeleteTriggers = DeleteTriggers.Break
                },
                OwnLimit = 2
            },
            new Item
            {
                Id = Ids.Items.PaletteOceanic,
                Name = "Oceanic",
                Quotes = new List<string>
                {
                    "It crashes down on land, whispering the secrets of the sea."
                },
                GroupId = Ids.Groups.Palette,
                Tags = ItemTag.Equipment | ItemTag.Decorator | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable | ItemTag.Orderable,
                Value = 2500,
                Size = 150,
                BypassCriteriaOnTrade = true,
                Rarity =  ItemRarity.Rare,
                Usage = new ItemUsage
                {
                    Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, PaletteType.Oceanic)),
                    DeleteTriggers = DeleteTriggers.Break
                },
                OwnLimit = 10
            },
            new Item
            {
                Id = Ids.Items.PaletteTaffy,
                Name = "Taffy",
                Quotes = new List<string>
                {
                    "It coats itself in a swirl of flexible sugar."
                },
                GroupId = Ids.Groups.Palette,
                Tags = ItemTag.Equipment | ItemTag.Decorator | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable,
                Value = 3500,
                Size = 150,
                BypassCriteriaOnTrade = true,
                Rarity =  ItemRarity.Rare,
                Usage = new ItemUsage
                {
                    Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, PaletteType.Taffy)),
                    DeleteTriggers = DeleteTriggers.Break
                },
                OwnLimit = 5
            },
            new Item
            {
                Id = Ids.Items.PaletteGlossyWumpite,
                Name = "Glossy Wumpite",
                Quotes = new List<string>
                {
                    "It refracts a mixture of light that absorbs the color of a Wumpus."
                },
                GroupId = Ids.Groups.Palette,
                Tags = ItemTag.Equipment | ItemTag.Decorator | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable,
                Value = 3500,
                Size = 175,
                TradeLimit = 0,
                Rarity =  ItemRarity.Rare,
                Usage = new ItemUsage
                {
                    Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, new ColorPalette(PaletteType.Wumpite, PaletteType.Glass))),
                    DeleteTriggers = DeleteTriggers.Break
                },
                OwnLimit = 5
            },
            new Item
            {
                Id = Ids.Items.PaletteGold,
                GroupId = Ids.Groups.Palette,
                Name = "Gold",
                Quotes = new List<string>
                {
                    "It gleams a golden radiant of hope."
                },
                Tags = ItemTag.Equipment | ItemTag.Decorator | ItemTag.Sealable,
                Value = 7500,
                Size = 400,
                TradeLimit = 0,
                Rarity = ItemRarity.Desolate,
                OwnLimit = 1
            },
            new Item
            {
                Id = Ids.Items.PaletteAmber,
                GroupId = Ids.Groups.Palette,
                Name = "Amber",
                Quotes = new List<string>
                {
                    "It preserves the life form of something hidden in the past."
                },
                Tags = ItemTag.Equipment | ItemTag.Decorator | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable,
                Value = 5000,
                Size = 300,
                TradeLimit = 0,
                Rarity = ItemRarity.Myth,
                Usage = new ItemUsage
                {
                    Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, new ColorPalette(PaletteType.Amber))),
                    DeleteTriggers = DeleteTriggers.Break
                },
                OwnLimit = 2
            },
            new Item
            {
                Id = Ids.Items.PaletteChocolate,
                GroupId = Ids.Groups.Palette,
                Name = "Chocolate",
                Quotes = new List<string>
                {
                    "It reminds you of a simpler time where sweets meant everything."
                },
                Tags = ItemTag.Equipment | ItemTag.Decorator | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable,
                Value = 3000,
                Size = 350,
                TradeLimit = 0,
                Rarity = ItemRarity.Rare,
                Usage = new ItemUsage
                {
                    Action = ctx => UsageResult.FromSuccess(SetOrSwapPalette(ctx.User, new ColorPalette(PaletteType.Chocolate))),
                    DeleteTriggers = DeleteTriggers.Break
                },
                OwnLimit = 2
            },
            new Item
            {
                Id = Ids.Items.LicenseInvertedCasino,
                GroupId = Ids.Groups.License,
                Name = "Inverted Casino",
                Quotes = new List<string>
                {
                    "It harnesses the addiction of desperate gamblers to fuel the risky business."
                },
                Tags = ItemTag.Equipment,
                Rarity = ItemRarity.Desolate,
                OwnLimit = 1,
                TradeLimit = 0
            },
            new Item
            {
                Id = Ids.Items.FontFoxtrot,
                Name = "Foxtrot",
                Quotes = new List<string>
                {
                    "It represents a strongly typed font face with a clean design."
                },
                Value = 2500,
                Currency = CurrencyType.Money,
                Tags = ItemTag.Equipment | ItemTag.Decorator | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable,
                Usage = new ItemUsage
                {
                    Action = ctx => UsageResult.FromSuccess(SetOrSwapFont(ctx.User, FontType.Foxtrot))
                }
            },
            new Item
            {
                Id = Ids.Items.FontMonori,
                Name = "Monori",
                Quotes = new List<string>
                {
                    "It resists the automation of auto-width characters.",
                    "It used to translate scripture at the speed of sound back in the past."
                },
                Value = 2500,
                Currency = CurrencyType.Money,
                Tags = ItemTag.Equipment | ItemTag.Decorator | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable,
                Usage = new ItemUsage
                {
                    Action = ctx => UsageResult.FromSuccess(SetOrSwapFont(ctx.User, FontType.Monori))
                }
            },
            new Item
            {
                Id = Ids.Items.FontOrikos,
                Name = "Orikos",
                Quotes = new List<string>
                {
                    "A system default that holds up to this day."
                },
                Tags = ItemTag.Equipment | ItemTag.Decorator | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable,
                Value = 2500,
                Currency = CurrencyType.Money,
                Usage = new ItemUsage
                {
                    Action = ctx => UsageResult.FromSuccess(SetOrSwapFont(ctx.User, FontType.Orikos))
                }
            },
            new Item
            {
                Id = Ids.Items.FontDelta,
                Name = "Delta",
                Quotes = new List<string>
                {
                    "It showcases a sharp range of tiny characters."
                },
                Tags = ItemTag.Equipment | ItemTag.Decorator | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable,
                Value = 2500,
                Currency = CurrencyType.Money,
                Usage = new ItemUsage
                {
                    Action = ctx => UsageResult.FromSuccess(SetOrSwapFont(ctx.User, FontType.Delta))
                }
            },
            new Item
            {
                Id = Ids.Items.CapsuleDaily1,
                Icon = "🏮",
                Name = "Daily Capsule A1",
                Summary = "A small goodie bag with a ribbon of hope.",
                Quotes = new List<string>
                {
                    "The contents inside glow with hope."
                },
                Tags = ItemTag.Container | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable,
                Rarity = ItemRarity.Common,
                Currency = CurrencyType.Money,
                Value = 50,
                Usage = new ItemUsage
                {
                    Durability = 1,
                    Action = ctx => GetLoot(ctx.User, ctx.Item, ctx.Item.Id, 1),
                    DeleteTriggers = DeleteTriggers.Break
                },
                TradeLimit = 0
            },
            new Item
            {
                Id = Ids.Items.CapsuleDaily2,
                Icon = "🏮",
                Name = "Daily Capsule A2",
                Summary = "A heartfelt goodie bag with a bronze seal.",
                Quotes = new List<string>
                {
                    "The contents inside glow with hope."
                },
                Tags = ItemTag.Container | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable,
                Rarity = ItemRarity.Uncommon,
                Currency = CurrencyType.Money,
                Value = 100,
                Usage = new ItemUsage
                {
                    Durability = 1,
                    // TODO: Add a custom property for capsules called roll count, which can be modified by a export cloning kit to add another roll count
                    Action = ctx => GetLoot(ctx.User, ctx.Item, ctx.Item.Id, 1),
                    DeleteTriggers = DeleteTriggers.Break
                },
                TradeLimit = 0
            },
            new Item
            {
                Id = Ids.Items.CapsuleDaily3,
                Icon = "🏮",
                Name = "Daily Capsule A3",
                Summary = "A carefully sealed bag enclosed with silver casing.",
                Quotes = new List<string>
                {
                    "The container protects something valuable."
                },
                Tags = ItemTag.Container | ItemTag.Cloneable | ItemTag.Disposable | ItemTag.Sealable,
                Rarity = ItemRarity.Rare,
                Currency = CurrencyType.Money,
                Value = 250,
                Usage = new ItemUsage
                {
                    Durability = 1,
                    // TODO: Add a custom property for capsules called roll count, which can be modified by a export cloning kit to add another roll count
                    Action = ctx => GetLoot(ctx.User, ctx.Item, ctx.Item.Id, 1),
                    DeleteTriggers = DeleteTriggers.Break
                },
                TradeLimit = 0
            }
        };

        public static readonly HashSet<ItemGroup> Groups = new HashSet<ItemGroup>
        {
            new ItemGroup
            {
                ShortId = "$",
                Id = "$",
                Name = "Internal",
                Summary = "This shouldn't even be visible to you."
            },
            new ItemGroup
            {
                ShortId = "c",
                Icon = "🍬",
                Id = "component",
                Rarity = ItemRarity.Uncommon,
                Name = "Component",
                Summary = "Helpful building blocks for the creation of new items."
            },
            new ItemGroup
            {
                Id = Ids.Groups.Tool,
                Name = "Tool",
                Rarity = ItemRarity.Uncommon,
                Summary = "Items that are used on other instances of items."
            },
            new ItemGroup
            {
                Id = Ids.Groups.Booster,
                Icon = Icons.Booster,
                Name = "Booster",
                Prefix = "Booster: ",
                Rarity = ItemRarity.Common,
                Summary = "Modifies the multiplier for a specified form of income."
            },
            new ItemGroup
            {
                Id = Ids.Groups.Palette,
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
                Rarity = ItemRarity.Uncommon,
                Summary = "Modifies the color scheme that is displayed on a card."
            },
            new ItemGroup
            {
                Id = Ids.Groups.Font,
                Icon = Icons.Palette,
                Name = "Font",
                Prefix = "Card Font: ",
                Rarity = ItemRarity.Rare,
                Summary = "Modifies the text font that is displayed on a card."
            },
            new ItemGroup
            {
                Id = Ids.Groups.Summon,
                Icon = Icons.Summon,
                Name = "Summon",
                Prefix = "Summon: ",
                Rarity = ItemRarity.Rare
            },
            new ItemGroup
            {
                Id = Ids.Groups.License,
                Name = "License",
                Prefix = "License: ",
                Summary = "A passive tool that grants you access to secret locations or tasks.",
                Rarity = ItemRarity.Desolate
            },
            new ItemGroup
            {
                Id = "automaton",
                Icon = "⚙️",
                Name = "Automaton",
                Prefix = "Automaton: ",
                Summary = "Grants the ability to automate specific actions.",
                Rarity = ItemRarity.Desolate,
                CanResearch = false
            },
            new ItemGroup
            {
                ShortId = "ly",
                Id = Ids.Groups.Layout,
                Name = "Layout",
                Prefix = "Card Layout: ",
                Summary = "Modifies the base formatting of a card.",
                Rarity = ItemRarity.Myth,
                CanResearch = false
            },
            new ItemGroup
            {
                ShortId = "ma",
                Icon = "🍬",
                Id = Ids.Groups.Material,
                Name = "Material",
                Summary = "Base resources that can be used to craft new items.",
                Rarity = ItemRarity.Uncommon,
                CanResearch = false
            }
        };

        private static string SetOrSwapFont(ArcadeUser user, FontType font)
        {
            if (user.Card.Font == font)
                return Format.Warning($"You already have **{ItemHelper.NameFor(font)}** equipped on your **Card Font**.");

            ItemHelper.GiveItem(user, ItemHelper.IdFor(user.Card.Font));
            ItemHelper.TakeItem(user, ItemHelper.IdFor(font));
            string result = $"> 📟 Swapped out **{ItemHelper.NameFor(user.Card.Font)}** with **{ItemHelper.NameFor(font)}** for your **Card Font**.";

            user.Card.Font = font;
            return result;
        }

        private static string SetOrSwapPalette(ArcadeUser user, ColorPalette palette)
        {
            if (user.Card.Palette == palette)
                return Format.Warning($"You already have **{ItemHelper.NameFor(palette.Primary, palette.Secondary)}** equipped on your **Card Palette**.");

            string result = $"> 📟 Equipped **{ItemHelper.NameFor(palette.Primary, palette.Secondary)}** to your **Card Palette**.";
            if (user.Card.Palette.Primary != PaletteType.Default)
            {
                ItemHelper.GiveItem(user, ItemHelper.IdFor(user.Card.Palette.Primary, user.Card.Palette.Secondary));
                result = $"📟 Swapped out **{ItemHelper.NameFor(user.Card.Palette.Primary, user.Card.Palette.Secondary)}** with **{ItemHelper.NameFor(palette.Primary, palette.Secondary)}** for your **Card Palette**.";
            }

            ItemHelper.TakeItem(user, ItemHelper.IdFor(palette.Primary, palette.Secondary));
            user.Card.Palette = palette;
            return result;
        }

        private static string SetOrSwapLayout(ArcadeUser user, LayoutType layout)
        {
            throw new NotImplementedException();
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

        private static UsageResult GetLoot(ArcadeUser user, Item capsule, string lootId, int rollCount)
        {
            LootTable table = LootTables.FirstOrDefault(x => x.Id == lootId);

            if (table == null)
                return UsageResult.FromError(Format.Warning("An unknown loot table ID was specified."));

            if (rollCount <= 0)
                return UsageResult.FromError(Format.Warning("Expected the roll count to be greater than 0."));

            Reward reward = table.Next(rollCount);

            var result = new StringBuilder();
            result.AppendLine($"> **{capsule.Name}**");
            result.AppendLine($"> You have opened this capsule to reveal:\n");

            result.Append(ViewReward(reward));
            reward.Apply(user);

            return UsageResult.FromSuccess(result.ToString());
        }

        private static string ViewReward(Reward reward)
        {
            if (reward == null)
                return "";

            var result = new StringBuilder();

            if (reward.Money > 0)
                result.AppendLine($"> {CurrencyHelper.WriteCost(reward.Money, CurrencyType.Money)} Orite");

            // if (reward.Exp > 0)
            //     result.AppendLine($"> {Icons.Exp} **{reward.Exp:##,0}**");

            if (Check.NotNullOrEmpty(reward.ItemIds))
            {
                foreach ((string itemId, int amount) in reward.ItemIds)
                    result.AppendLine($"> {GetItemPreview(itemId, amount)}");
            }

            return result.ToString();
        }

        private static string GetItemPreview(string itemId, int amount)
        {
            string icon = ItemHelper.GetIconOrDefault(itemId) ?? "•";
            string name = Check.NotNull(icon) ? ItemHelper.GetBaseName(itemId) : ItemHelper.NameOf(itemId);
            string counter = amount > 1 ? $" (x**{amount:##,0}**)" : "";
            return $"`{itemId}` {icon} **{name}**{counter}";
        }

        private static bool VerifyName(string name, out string reason)
        {
            reason = "";

            if (string.IsNullOrWhiteSpace(name))
            {
                reason = "There is no name to use.";
                return false;
            }

            if (name.Length > 24)
            {
                reason = "The name that was specified is larger than 24 characters.";
                return false;
            }

            if (!name.Where(x => !x.EqualsAny(' ', '_', '+', '\'')).All(StringReader.IsUnquotedStringValid))
            {
                reason = "Names can only use alphanumeric characters with basic punctuation.";
                return false;
            }

            return true;
        }
    }
}
