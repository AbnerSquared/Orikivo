using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using Orikivo;
using Format = Orikivo.Format;

namespace Arcadia.Multiplayer.Games
{
    
    public class WerewolfGame : GameBase
    {
        private static readonly List<string> DefaultRandomNames = new List<string>
        {
            "Tom", "Rachel", "Jerry", "Nathan", "Richard", "Joshua", "Nicholas", "Julian", "Abigail"
        };

        public WerewolfGame()
        {
            Id = "Werewolf";

            Details = new GameDetails
            {
                Name = "Werewolf",
                Summary = "Figure out the werewolves and save the village.",
                RequiredPlayers = 1,
                PlayerLimit = 8
            };

            Options = new List<GameOption>
            {
                GameOption.Create(WolfConfig.RevealRolesOnDeath, "Reveal roles on death", false),
                GameOption.Create(WolfConfig.RoleDeny, "Denied roles", WerewolfRoleDeny.None),
                GameOption.Create(WolfConfig.RandomizeNames, "Randomize names", false)
            };
        }

        private List<string> RandomNames { get; set; }

        private List<WerewolfRole> GenerateRoles(int playerCount)
        {
            // Load up all available default roles
            List<WerewolfRole> availableRoles = WerewolfRole.GetPack(WerewolfRolePack.Custom);

            // Load all denied roles
            var roleDeny = GetConfigValue<WerewolfRoleDeny>(WolfConfig.RoleDeny);

            // Remove all of the specified denied roles from the list of available roles
            foreach (WerewolfRoleDeny deny in roleDeny.GetActiveFlags())
                availableRoles.RemoveAll(x => x.Id.Equals(deny.ToString(), StringComparison.OrdinalIgnoreCase));

            // Initialize the new list of available roles
            var roles = new List<WerewolfRole>();


            bool hasWerewolf = false;
            // Handle game balance here
            for (var i = 0; i < playerCount; i++)
            {
                if (!hasWerewolf)
                    roles.Add(WerewolfRole.Werewolf);
                else
                    roles.Add(WerewolfRole.Villager);
            }

            // Return the list of generated roles (this is a bit heavy on operations
            return Randomizer.Shuffle(roles).ToList();
        }

        #region Helpers

        private static int CountPlayers(GameSession session)
            => session.Players.Count;

        private static int CountLiving(GameSession session)
            => session.Players.Count(IsAlive);

        private static int CountWolves(GameSession session)
            => session.Players.Count(IsWolf);

        private static int CountVillagers(GameSession session)
            => session.Players.Count(IsVillager);

        private static int CountPendingVotes(GameSession session)
            => session.Players.Count(x => x.ValueOf<WerewolfVote>(WolfVars.Vote) == WerewolfVote.Pending);

        private static int CountLiveVotes(GameSession session)
            => session.Players.Count(x => x.ValueOf<WerewolfVote>(WolfVars.Vote) == WerewolfVote.Live);

        private static int CountDeathVotes(GameSession session)
            => session.Players.Count(x => x.ValueOf<WerewolfVote>(WolfVars.Vote) == WerewolfVote.Die);

        private static bool HasAbility(PlayerData player, WerewolfAbility ability)
            => player.ValueOf<WerewolfRole>(WolfVars.Role).Ability.HasFlag(ability);

        private static bool HasPassive(PlayerData player)
            => player.ValueOf<WerewolfRole>(WolfVars.Role).Passive != WerewolfPassive.None;

        private static bool HasAbility(PlayerData player)
            => player.ValueOf<WerewolfRole>(WolfVars.Role).Ability != WerewolfAbility.None;

        public static bool IsInGroup(PlayerData player, WerewolfGroup group)
            => player.ValueOf<WerewolfRole>(WolfVars.Role).Group.HasFlag(group);

        private static bool IsVillager(PlayerData player)
            => IsInGroup(player, WerewolfGroup.Villager);

        private static bool IsWolf(PlayerData player)
            => IsInGroup(player, WerewolfGroup.Werewolf);

        private static bool IsSuspect(PlayerData player, GameSession session)
            => session.ValueOf<ulong>(WolfVars.Suspect) == player.Player.User.Id;

        private static bool IsAlive(PlayerData player)
            => !player.ValueOf<bool>(WolfVars.IsDead);

        private static bool IsProtected(PlayerData player)
            => player.ValueOf<bool>(WolfVars.IsProtected);

        private static bool CanFeast(PlayerData player)
            => HasAbility(player, WerewolfAbility.Feast);

        private static bool CanPeek(PlayerData player)
            => HasAbility(player, WerewolfAbility.Peek);

        private static bool CanHunt(PlayerData player)
            => HasAbility(player, WerewolfAbility.Hunt);

        private static bool CanProtect(PlayerData player)
            => HasAbility(player, WerewolfAbility.Protect);

        private static WerewolfPassive GetPassive(PlayerData player)
            => player.ValueOf<WerewolfRole>(WolfVars.Role).Passive;

        private static WerewolfAbility GetAbility(PlayerData player)
            => player.ValueOf<WerewolfRole>(WolfVars.Role).Ability;

        private static string GetName(PlayerData player)
            => player.ValueOf<string>(WolfVars.Name);

        private static void InheritRole(PlayerData player, WerewolfRole role)
            => player.SetValue(WolfVars.Role, role);

        private static bool CanSkipCurrentPhase(GameSession session)
            => session.ValueOf<WerewolfPhase>(WolfVars.CurrentPhase).HasFlag(WerewolfPhase.Day);

        #endregion

        private PlayerData CreatePlayer(Player player, WerewolfRole role, int index)
        {
            Console.WriteLine("Creating player...");
            var data = new PlayerData
            {
                Player = player,
                Properties = new List<GameProperty>
                {
                    // index: Used to keep track of the player as a unique indexer
                    GameProperty.Create(WolfVars.Index, index),

                    // name: The player's name.
                    GameProperty.Create(WolfVars.Name, GetConfigValue<bool>(WolfConfig.RandomizeNames) ? Randomizer.Take(RandomNames) : player.User.Username),

                    // initial_role: The ID of their initial role given
                    GameProperty.Create(WolfVars.InitialRole, role.Id),

                    // role: Their current role
                    GameProperty.Create(WolfVars.Role, role),

                    // is_dead: If the player is currently dead
                    GameProperty.Create(WolfVars.IsDead, false),

                    // has_closure: If the player received closure for their death
                    GameProperty.Create(WolfVars.HasClosure, false),

                    // is_winner: If the player was a winner in this game
                    GameProperty.Create(WolfVars.IsWinner, true),

                    // vote: The current vote state of the player
                    GameProperty.Create(WolfVars.Vote, WerewolfVote.Pending),

                    // is_hurt: If the player is currently hurt
                    GameProperty.Create(WolfVars.IsHurt, false),

                    // marked_for_death: If the player was marked for death by the werewolves
                    GameProperty.Create(WolfVars.MarkedForDeath, false),

                    // is_protected: If the player was currently protected
                    GameProperty.Create(WolfVars.IsProtected, false),

                    // lover_id: If specified, when this player dies, the specified ID is also killed
                    GameProperty.Create(WolfVars.LoverId, 0UL),

                    // kills: Represents a list of kills this player has achieved
                    GameProperty.Create(WolfVars.Kills, new List<WerewolfDeath>()),

                    // peeked_player_ids: Represents a dictionary of peeked players, if able
                    GameProperty.Create(WolfVars.PeekedPlayerIds, new Dictionary<ulong, bool>()),

                    // death_info: The details of how a player died, if any
                    GameProperty.Create<WerewolfDeath>(WolfVars.DeathInfo),

                    // requested_skip: Checks if the player has already requested to skip
                    GameProperty.Create(WolfVars.RequestedSkip, false)
                }
            };

            // Remove this once this is assured to be okay
            Console.WriteLine(data.ToString());
            return data;
        }


        #region Required
        public override List<GameCriterion> OnBuildRules(List<PlayerData> players)
        {
            return new List<GameCriterion>
            {
                new GameCriterion(WolfVars.WolfGreaterEqualsVillager, WolfGreaterEqualsVillager),
                new GameCriterion(WolfVars.AllDeadWolf, AllDeadWolf)
            };
        }

        public override List<PlayerData> OnBuildPlayers(List<Player> players)
        {
            if (GetConfigValue<bool>(WolfConfig.RandomizeNames))
                RandomNames = Randomizer.ChooseMany(DefaultRandomNames, players.Count).ToList();

            List<WerewolfRole> roles = GenerateRoles(players.Count);

            return players.Select((x, i) => CreatePlayer(x, Randomizer.Choose(roles), i)).ToList();
        }

        public override List<GameProperty> OnBuildProperties()
        {
            return new List<GameProperty>
            {
                // last_player_killed: The death info of the last player killed, if any
                GameProperty.Create<WerewolfDeath>(WolfVars.LastPlayerKilled),

                // suspect: The ID of the current suspect
                GameProperty.Create(WolfVars.Suspect, 0UL),

                GameProperty.Create(WolfVars.Accuser, 0UL),

                // current_input: The current input being handled
                GameProperty.Create(WolfVars.CurrentInput, WerewolfAbility.None),

                // current_phase: The current phase being handled
                GameProperty.Create(WolfVars.CurrentPhase, WerewolfPhase.Unknown),

                // next_phase: The next phase to set at the end of the current phase
                GameProperty.Create(WolfVars.NextPhase, WerewolfPhase.Unknown),

                // requested_skips: The total counter of requested skips on the current phase
                GameProperty.Create(WolfVars.RequestedSkips, 0),

                // total_rounds: The total counter of passed rounds
                GameProperty.Create(WolfVars.TotalRounds, 0),

                // handled_inputs: The total set of abilities handled
                GameProperty.Create(WolfVars.HandledInputs, WerewolfAbility.None),

                // has_trial: Checks to see if there is a trial currently in progress
                GameProperty.Create(WolfVars.HasTrial, false),

                // await_defense: Checks to see if the game is currently waiting for the suspect's defense
                GameProperty.Create(WolfVars.AwaitDefense, false),

                // winning_group
                GameProperty.Create(WolfVars.WinningGroup, WerewolfGroup.Unknown)
            };
        }

        public override List<GameAction> OnBuildActions(List<PlayerData> players)
        {
            return new List<GameAction>
            {
                new GameAction(WolfVars.Start, Start, false),
                new GameAction(WolfVars.GetResults, GetResults),
                new GameAction(WolfVars.StartDay, StartDay),
                new GameAction(WolfVars.EndDay, EndDay),
                new GameAction(WolfVars.StartTrial, StartTrial),

                // Instead of Start<Ability>Input, do StartInput, referencing the ability in WolfVars.CurrentInput

                new GameAction(WolfVars.StartVoteInput, StartVoteInput),
                new GameAction(WolfVars.EndVoteInput, EndVoteInput),
                new GameAction(WolfVars.StartNight, StartNight),
                new GameAction(WolfVars.EndNight, EndNight),
                new GameAction(WolfVars.StartPeekInput, StartPeekInput),
                new GameAction(WolfVars.EndPeekInput, EndPeekInput),
                new GameAction(WolfVars.StartFeastInput, StartFeastInput),
                new GameAction(WolfVars.EndFeastInput, EndFeastInput),
                new GameAction(WolfVars.StartProtectInput, StartProtectInput),
                new GameAction(WolfVars.EndProtectInput, EndProtectInput),
                new GameAction(WolfVars.StartHuntInput, StartHuntInput),
                new GameAction(WolfVars.EndHuntInput, EndHuntInput),
                new GameAction(WolfVars.HandleDeaths, HandleDeaths),
                new GameAction(WolfVars.TrySkipPhase, TrySkipPhase),
                new GameAction(WolfVars.RemoveSuspect, RemoveSuspect),
                new GameAction(WolfVars.HandleDeath, HandleDeath),
                new GameAction(WolfVars.TryEndVote, TryEndVote, false),
                new GameAction(WolfVars.HandleAbilities, HandleAbilities)
            };
        }

        public override async Task OnSessionStartAsync(GameServer server, GameSession session)
        {
            // Set all of the currently connected channels to the specified frequency and group them
            server.SetFrequencyForState(GameState.Playing, WolfChannel.Main);
            server.GroupAll("primary");

            DisplayContent main = server.GetDisplayChannel(WolfChannel.Main).Content;

            foreach (PlayerData player in session.Players)
            {
                Console.WriteLine("Loading player...");
                // Set this stuff up AFTER you point all of the connections to the right frequency
                if (HasAbility(player))
                {
                    // This initializes all of the displays to be set up.
                    DisplayChannel display = server.GetDisplayChannel(GetFrequencyFor(GetAbility(player)));
                    display.Content.ValueOverride = $"> You are a **{player.ValueOf<WerewolfRole>(WolfVars.Role).Name}**.";
                    var properties = ConnectionProperties.Default;
                    properties.ContentOverride = $"> You are a **{player.ValueOf<WerewolfRole>(WolfVars.Role).Name}**.";

                    var connection = await ServerConnection.CreateAsync(player.Player, display, properties);
                    connection.Origin = OriginType.Session;
                    connection.Group = GetAbility(player).ToString().ToLower();
                    server.Connections.Add(connection);
                }

                var index = player.ValueOf<int>(WolfVars.Index);

                // Initialize all of the player slots on the main channel
                main.GetGroup("players").Set(index, WritePlayerInfo(player, session));
            }

            // Synchronize the configurations set
            Options = server.Config.GameOptions;

            main.GetComponent("players").Draw();
            // Start the game
            session.InvokeAction(WolfVars.Start, true);
        }

        public override SessionResult OnSessionFinish(GameSession session)
        {
            var result = new SessionResult();

            // For each player, update stats if they won, played, killed, and voted

            return result;
        }

        public override List<DisplayChannel> OnBuildDisplays(List<PlayerData> players)
        {
            return new List<DisplayChannel>
            {
                new DisplayChannel
                {
                    Frequency = WolfChannel.Peek,
                    Content = new DisplayContent
                    {
                        Components = new List<IComponent>
                        {
                            new Component
                            {
                                Id = "header",
                                Position = 0,
                                Active = true,
                                Formatter = new ComponentFormatter
                                {
                                    BaseFormatter = "> {0}",
                                    OverrideBaseValue = true
                                }
                            },
                            // main/players
                            new ComponentGroup
                            {
                                Active = true,
                                Position = 1,
                                Id = "players",
                                Formatter = new ComponentFormatter
                                {
                                    BaseFormatter = "> **Players**\n{0}",
                                    ElementFormatter = "{0}", // the formatting is handled elsewhere
                                    Separator = "\n"
                                },

                                Capacity = players.Count,
                                Values = new string[players.Count]
                            }
                        }
                    }
                },
                new DisplayChannel
                {
                    Frequency = WolfChannel.Hunt,
                    Content = new DisplayContent
                    {
                        Components = new List<IComponent>
                        {
                            new Component
                            {
                                Id = "header",
                                Position = 0,
                                Active = true,
                                Formatter = new ComponentFormatter
                                {
                                    BaseFormatter = "> {0}",
                                    OverrideBaseValue = true
                                }
                            },
                            // main/players
                            new ComponentGroup("players", 1, players.Count)
                            {
                                Formatter = new ComponentFormatter
                                {
                                    BaseFormatter = "> **Players**\n{0}",
                                    ElementFormatter = "{0}", // the formatting is handled elsewhere
                                    Separator = "\n"
                                },

                                Values = new string[players.Count]
                            }
                        }
                    }
                },
                new DisplayChannel
                {
                    Frequency = WolfChannel.Protect,
                    Content = new DisplayContent
                    {
                        Components = new List<IComponent>
                        {
                            new Component
                            {
                                Id = "header",
                                Position = 0,
                                Active = true,
                                Formatter = new ComponentFormatter
                                {
                                    BaseFormatter = "> {0}",
                                    OverrideBaseValue = true
                                }
                            },
                            // main/players
                            new ComponentGroup
                            {
                                Active = true,
                                Position = 1,
                                Id = "players",
                                Formatter = new ComponentFormatter
                                {
                                    BaseFormatter = "> **Players**\n{0}",
                                    ElementFormatter = "{0}", // the formatting is handled elsewhere
                                    Separator = "\n"
                                },

                                Capacity = players.Count,
                                Values = new string[players.Count]
                            }
                        }
                    }
                },
                new DisplayChannel
                {
                    Frequency = WolfChannel.Feast,
                    Content = new DisplayContent
                    {
                        Components = new List<IComponent>
                        {
                            new Component
                            {
                                Id = "header",
                                Position = 0,
                                Active = true,
                                Formatter = new ComponentFormatter
                                {
                                    BaseFormatter = "> {0}",
                                    OverrideBaseValue = true
                                }
                            },
                            // main/players
                            new ComponentGroup
                            {
                                Active = true,
                                Position = 1,
                                Id = "players",
                                Formatter = new ComponentFormatter
                                {
                                    BaseFormatter = "> **Players**\n{0}",
                                    ElementFormatter = "{0}", // the formatting is handled elsewhere
                                    Separator = "\n"
                                },

                                Capacity = players.Count,
                                Values = new string[players.Count]
                            }
                        }
                    }
                },
                // initial
                new DisplayChannel
                {
                    Frequency = WolfChannel.Initial,
                    Content = new DisplayContent
                    {
                        // initial/header
                        Components = new List<IComponent>
                        {
                            new Component
                            {
                                Id = "header",
                                Position = 0,
                                Value = "> Welcome to **Werewolf**."
                            }
                        }
                    }
                },

                // main
                new DisplayChannel
                {
                    Frequency = WolfChannel.Main,
                    Content = new DisplayContent
                    {
                        Components = new List<IComponent>
                        {
                            // main/header
                            new Component
                            {
                                Active = true,
                                Position = 0,
                                Id = "header",
                                Formatter = new ComponentFormatter
                                {
                                    // 0: Round count
                                    // 1: Phase header
                                    BaseFormatter = "> **Round {0}**\n> {1}",
                                    OverrideBaseValue = true
                                }
                            },

                            // main/console
                            new ComponentGroup
                            {
                                Active = true,
                                AutoDraw = true,
                                Position = 1,
                                Id = "console",
                                Formatter = new ComponentFormatter
                                {
                                    BaseFormatter = "```{0}```",
                                    ElementFormatter = "• {0}",
                                    Separator = "\n",
                                    OverrideBaseValue = false
                                },

                                Capacity = 6,
                                Values = new[] { "", "", "", "", "", "" }
                            },

                            // main/players
                            new ComponentGroup
                            {
                                Active = true,
                                Position = 2,
                                Id = "players",
                                Formatter = new ComponentFormatter
                                {
                                    BaseFormatter = "> **Residents**\n{0}",
                                    ElementFormatter = "{0}", // the formatting is handled elsewhere
                                    Separator = "\n"
                                },

                                Capacity = players.Count,
                                Values = new string[players.Count]
                            }
                        }
                    },
                    Inputs = new List<IInput>
                    {
                        new TextInput("agree", OnAgree),
                        new TextInput("accuse", OnAccuse),
                        new TextInput("skip", OnSkip),
                        new TextInput("say", OnSpeak),
                        new TextInput("silent", OnSilent)
                    }
                },

                // death
                new DisplayChannel
                {
                    Frequency = WolfChannel.Death,
                    Content = new DisplayContent
                    {
                        Components = new List<IComponent>
                        {
                            // death/header
                            new Component
                            {
                                Id = "header",
                                Position = 0,
                                Active = true,
                                Formatter = new ComponentFormatter
                                {
                                    BaseFormatter = "> 💀 **{0}** has died.",
                                    OverrideBaseValue = true
                                }
                            },

                            // death/summary
                            new Component
                            {
                                Id = "summary",
                                Position = 1,
                                Active = true,
                                Formatter = new ComponentFormatter
                                {
                                    BaseFormatter = "```{0}```",
                                    OverrideBaseValue = true
                                }
                            },

                            // death/reveal
                            new Component
                            {
                                Id = "reveal",
                                Position = 2,
                                Active = true,
                                Formatter = new ComponentFormatter
                                {
                                    // \n> {1}
                                    BaseFormatter = "> They were a **{0}**.{1}",
                                    OverrideBaseValue = true
                                }
                            }
                        }
                    }
                },

                // vote
                new DisplayChannel
                {
                    Frequency = WolfChannel.Vote,
                    Content = new DisplayContent
                    {
                        Components = new List<IComponent>
                        {
                            // vote/header
                            new Component
                            {
                                Id = "header",
                                Active = true,
                                Position = 0,
                                Formatter = new ComponentFormatter
                                {
                                    BaseFormatter = "> Should **{0}** be killed for their suspicion?",
                                    OverrideBaseValue = true
                                }
                            },
                            // main/counter
                            new Component
                            {
                                Active = true,
                                Position = 1,
                                Id = "counter",
                                Formatter = new ComponentFormatter
                                {
                                    BaseFormatter = "> {0}"
                                }
                            }
                        }
                    },
                    Inputs = new List<IInput>()
                    {
                        new TextInput("live", OnLive),
                        new TextInput("die", OnDie)
                    }
                },

                // Inputs:
                // - skip: If they are the host, this is automatically skipped
                //    Otherwise, increase 'requested_skips' by 1 and attempt to skip


                // These are initialized as the game proceeds
                // Private Channels: Wolf Channel, Seer Channel, Hunt Channel, Bodyguard Channel, Ghost Channel

                // Vote Channels: If the configuration 'Random names' is true, each player must vote in their own channel
                // If they already have a private channel established, store the previous frequency, and swap to the voting frequency
                // After they vote or they time out, set the previous frequency back

                // results
                new DisplayChannel
                {
                    Frequency = WolfChannel.Results,
                    Content = new DisplayContent
                    {
                        Components = new List<IComponent>
                        {
                            // results/header
                            new Component
                            {
                                Id = "header",
                                Active = true,
                                Position = 0,
                                Formatter = new ComponentFormatter
                                {
                                    BaseFormatter = "> The **{0}** win.\n> **Rounds: {1}**",
                                    OverrideBaseValue = true
                                }
                            },

                            // results/summary
                            new Component
                            {
                                Id = "summary",
                                Active = true,
                                Position = 1,
                                Formatter = new ComponentFormatter
                                {
                                    BaseFormatter = "```{0}```",
                                    OverrideBaseValue = true
                                }
                            },

                            // results/facts
                            new ComponentGroup
                            {
                                Id = "facts",
                                Capacity = 1,
                                Active = true,
                                Position = 2,
                                Formatter = new ComponentFormatter
                                {
                                    BaseFormatter = "{0}",
                                    ElementFormatter = "• {0}",
                                    Separator = "\n"
                                },
                                Values = new []{ "This is a random fact placeholder." }
                            }
                        }
                    }
                }
            };
        }

        #endregion

        #region Actions

        private static void HandleDeath(GameContext ctx)
        {
            DisplayContent death = ctx.Server.GetDisplayChannel(WolfChannel.Death).Content;

            // Set all of the primary channels to the death frequency
            foreach (ServerConnection connection in ctx.Server.GetGroup("primary"))
                connection.Frequency = WolfChannel.Death;

            ctx.Session.BlockInput = true;

            var kill = ctx.Session.ValueOf<WerewolfDeath>(WolfVars.LastPlayerKilled);

            // If null, throw an exception
            if (kill == null)
                throw new Exception("Attempted to invoke a death sequence with no player killed");

            PlayerData player = ctx.Session.DataOf(kill.UserId);
            bool hasGameEnded = ctx.Session.MeetsCriterion(WolfVars.WolfGreaterEqualsVillager) || ctx.Session.MeetsCriterion(WolfVars.AllDeadWolf);
            string extraText = hasGameEnded ? WriteDeathRemainText(ctx.Session) : "";

            // Write to the main channel the information about who died
            death.GetComponent("header").Draw(player.Player.User.Username);
            death.GetComponent("summary").Draw(WriteDeathText(kill.Method));
            death.GetComponent("reveal").Draw(player.ValueOf<WerewolfRole>(WolfVars.Role).Name, extraText);

            // Marked the player with closure
            ctx.Session.DataOf(kill.UserId).SetValue(WolfVars.HasClosure, true);

            // Check winning conditions
            if (WolfGreaterEqualsVillager(ctx.Session))
            {
                ctx.Session.SetValue(WolfVars.WinningGroup, WerewolfGroup.Werewolf);
                ctx.Session.QueueAction(TimeSpan.FromSeconds(10), WolfVars.GetResults);
                return;
            }

            if (AllDeadWolf(ctx.Session))
            {
                ctx.Session.SetValue(WolfVars.WinningGroup, WerewolfGroup.Villager);
                ctx.Session.QueueAction(TimeSpan.FromSeconds(10), WolfVars.GetResults);
                return;
            }

            // If the next phase is night, start the night
            if (ctx.Session.ValueOf<WerewolfPhase>(WolfVars.NextPhase) == WerewolfPhase.Night)
            {
                ctx.Session.QueueAction(TimeSpan.FromSeconds(10), WolfVars.StartNight);
                return;
            }

            // Otherwise, continue handling deaths
            ctx.Session.QueueAction(TimeSpan.FromSeconds(10), WolfVars.HandleDeaths);
        }

        private static void HandleDeaths(GameContext ctx)
        {
            DisplayContent main = ctx.Server.GetDisplayChannel(WolfChannel.Main).Content;

            // If there aren't any deaths, proceed through with the current phase
            ctx.Session.BlockInput = true;

            // If anyone has recently died
            if (ctx.Session.Players.Any(HasRecentlyDied))
            {
                foreach (PlayerData player in ctx.Session.Players.Where(HasRecentlyDied))
                {
                    var deathInfo = player.ValueOf<WerewolfDeath>(WolfVars.DeathInfo);

                    if (deathInfo == null)
                        throw new Exception("Expected death info but returned null");

                    ctx.Session.SetValue(WolfVars.LastPlayerKilled, deathInfo);
                    ctx.Session.InvokeAction(WolfVars.HandleDeath, true);
                    return;
                }
            }

            // If anyone was marked for death
            if (ctx.Session.Players.Any(IsMarkedForDeath))
            {
                foreach (PlayerData player in ctx.Session.Players.Where(IsMarkedForDeath))
                {
                    
                    // If the player was protected
                    if (IsProtected(player))
                    {
                        player.SetValue(WolfVars.MarkedForDeath, false);
                        player.SetValue(WolfVars.IsProtected, false);

                        // Clarify that the player was protected
                        main.GetGroup("console").Append(WriteProtectText(player));
                        continue;
                    }

                    // If the player is a tough person
                    if (GetPassive(player).HasFlag(WerewolfPassive.Tough))
                    {
                        player.SetValue(WolfVars.MarkedForDeath, false);
                        player.SetValue(WolfVars.IsHurt, true);

                        // Clarify that the player was hurt
                        main.GetGroup("console").Append(WriteHurtText(player));
                        continue;
                    }

                    var death = new WerewolfDeath
                    {
                        Method = WerewolfDeathMethod.Wolf,
                        UserId = player.Player.User.Id
                    };

                    // Mark the player as dead
                    player.SetValue(WolfVars.IsDead, true);
                    player.SetValue(WolfVars.DeathInfo, death);

                    // Update all kills for each werewolf
                    foreach (PlayerData wolf in ctx.Session.Players.Where(IsWolf))
                    {
                        // TODO: Ensure that kills are updated when adding values; otherwise, add and set list
                        var kills = player.ValueOf<List<WerewolfDeath>>(WolfVars.Kills);
                        Console.WriteLine(kills.Count);
                        kills.Add(death);
                        Console.WriteLine(kills.Count);
                    }

                    // Update the last player killed and handle their death
                    ctx.Session.SetValue(WolfVars.LastPlayerKilled, death);
                    ctx.Session.InvokeAction(WolfVars.HandleDeath, true);
                    return;
                }
            }

            ctx.Session.BlockInput = false;
            ctx.Session.InvokeAction(WolfVars.StartDay);
        }

        private static string WriteVoteCounter(GameSession session)
        {
            int yes = CountDeathVotes(session);
            int no = CountLiveVotes(session);
            int pending = CountPendingVotes(session);

            var counter = new StringBuilder();

            // pending
            counter.Append("⚪", 0, pending);

            // no
            counter.Append("🔴", 0, no);

            // yes
            counter.Append("🔵", 0, yes);

            return counter.ToString();
        }

        private static void SetChannel(GameServer server, int frequency)
        {
            foreach (ServerConnection connection in server.GetGroup("primary"))
                connection.Frequency = frequency;
        }

        private static void StartVoteInput(GameContext ctx)
        {
            // Start a timer for 30 seconds that invokes the action 'end_vote_input'
            ctx.Session.QueueAction(TimeSpan.FromSeconds(30), WolfVars.EndVoteInput);
            ctx.Session.SetValue(WolfVars.AwaitDefense, false);
            ctx.Session.BlockInput = true;

            // Set all connections to the vote channel
            SetChannel(ctx.Server, WolfChannel.Vote);

            // If the game configuration supports night zero, handle it here.

            // Write the initial texts
            DisplayContent vote = ctx.Server.GetDisplayChannel(WolfChannel.Vote).Content;

            vote.GetComponent("header").Draw(GetSuspect(ctx.Session));
            vote.GetComponent("counter").Draw(WriteVoteCounter(ctx.Session));


            // Automatically set the votes of each LIVING
            foreach (PlayerData data in ctx.Session.Players)
            {
                // If the player is dead, continue.
                if (!IsAlive(data))
                    continue;

                if (GetPassive(data).HasFlag(WerewolfPassive.Militarist | WerewolfPassive.Pacifist))
                    throw new Exception("The specified player has a conflicting passive ability");

                if (GetPassive(data).HasFlag(WerewolfPassive.Pacifist))
                    data.SetValue(WolfVars.Vote, WerewolfVote.Live);
                else if (GetPassive(data).HasFlag(WerewolfPassive.Militarist))
                    data.SetValue(WolfVars.Vote, WerewolfVote.Die);
            }

            ctx.Session.InvokeAction(WolfVars.TryEndVote);
            ctx.Session.BlockInput = false;

        }

        private static bool CanVote(PlayerData player)
            => !GetPassive(player).HasFlag(WerewolfPassive.Pacifist | WerewolfPassive.Militarist);

        private static void TryEndVote(GameContext ctx)
        {
            // If all of the votes are secured, end the vote
            if (ctx.Session.Players.All(x => x.ValueOf<WerewolfVote>(WolfVars.Vote) != WerewolfVote.Pending))
            {
                // Cancel the current voting timer
                ctx.Session.CancelNewestInQueue();
                
                // End the current vote
                ctx.Session.InvokeAction(WolfVars.EndVoteInput);
            }
        }

        private static void EndVoteInput(GameContext ctx)
        {
            // Get the vote counts
            int toLive = CountLiveVotes(ctx.Session);
            int toDie = CountDeathVotes(ctx.Session);
            int pending = CountPendingVotes(ctx.Session);

            // Reset all of the votes once you've received the vote counts
            foreach (PlayerData player in ctx.Session.Players.Where(CanVote))
                player.SetValue(WolfVars.Vote, WerewolfVote.Pending);

            ctx.Session.BlockInput = true;
            // If the votes to die is more than the votes to live
            if (toDie > toLive + pending)
            {
                PlayerData suspect = ctx.Session.DataOf(ctx.Session.ValueOf<ulong>(WolfVars.Suspect));

                if (suspect == null)
                    throw new Exception("Expected suspect but returned null");

                var death = new WerewolfDeath
                {
                    UserId = suspect.Player.User.Id,
                    DiedAt = DateTime.UtcNow,
                    Method = WerewolfDeathMethod.Hang
                };

                // Kill the suspect
                suspect.SetValue(WolfVars.IsDead, true);
                ctx.Session.SetValue(WolfVars.LastPlayerKilled, death);

                // Set the next phase to night
                ctx.Session.SetValue(WolfVars.NextPhase, WerewolfPhase.Night);

                // Handle the suspect's death
                ctx.Session.BlockInput = false;
                ctx.Session.SetValue(WolfVars.HasTrial, false);
                ctx.Session.SetValue(WolfVars.Suspect, 0UL);
                ctx.Session.SetValue(WolfVars.Accuser, 0UL);
                ctx.Session.InvokeAction(WolfVars.HandleDeath, true);
            }
            else
            {
                // Otherwise, go directly to the night phase
                ctx.Session.BlockInput = false;
                SetChannel(ctx.Server, WolfChannel.Main);
                ctx.Session.SetValue(WolfVars.HasTrial, false);
                ctx.Session.SetValue(WolfVars.Suspect, 0UL);
                ctx.Session.SetValue(WolfVars.Accuser, 0UL);
                ctx.Server.GetDisplayChannel(WolfChannel.Main).Content.GetGroup("console").Append("There weren't enough votes to allow the kill to go through.");
                ctx.Session.QueueAction(TimeSpan.FromSeconds(5), WolfVars.EndDay);
            }
        }

        private static void StartTrial(GameContext ctx)
        {
            // Set the primary channel to the trial frequency

            // Start a timer for 30 seconds that invokes the action 'start_vote_input'
            ctx.Session.QueueAction(TimeSpan.FromSeconds(30), WolfVars.StartVoteInput);
            ctx.Session.SetValue(WolfVars.AwaitDefense, true);

            // While this timer runs, it waits for the suspect to write their defense piece
            //     - If RandomNames is enabled, this will be handled in their direct messages
            //     - Otherwise, the suspect writes their defense in the chat

            // If the suspect does not reply, they will be ignored
            // Likewise, they can also decide to remain silent
        }

        private static void OnSilent(InputContext ctx)
        {
            // Cancel the currently queued action (from 'start_trial', 30 seconds => 'start_vote_input')
            ctx.Session.CancelNewestInQueue();

            // Invoke the action 'start_vote_input'
            ctx.Session.InvokeAction(WolfVars.StartVoteInput);
        }

        private static void StartHuntInput(GameContext ctx)
        {
            ctx.Server.GetDisplayChannel(WolfChannel.Hunt).GetComponent("header").Draw(WriteHuntHeader());
            foreach (PlayerData player in ctx.Session.Players.Where(CanHunt))
            {
                // The channel ID saved will be the player's own ID.
                ServerConnection connection = ctx.Server.Connections.FirstOrDefault(x => x.ChannelId == player.Player.User.Id);
                connection.BlockInput = false;
            }

            // Start a timer for 30 seconds that invokes the action 'end_hunt_input'
            ctx.Session.QueueAction(TimeSpan.FromSeconds(30), WolfVars.EndHuntInput);
        }

        private static void EndHuntInput(GameContext ctx)
        {
            // Add the Hunt ability to the list of Read inputs, which determines what is left to check
            // Once everything has finished, block input to that channel, and continue with the night
            foreach (PlayerData player in ctx.Session.Players.Where(CanPeek))
            {
                // The channel ID saved will be the player's own ID.
                ServerConnection connection = ctx.Server.Connections.FirstOrDefault(x => x.ChannelId == player.Player.User.Id);
                connection.BlockInput = true;
            }

            ctx.Session.SetValue(WolfVars.HandledInputs, ctx.Session.ValueOf<WerewolfAbility>(WolfVars.HandledInputs) | WerewolfAbility.Hunt);
            // Handle all abilities
            ctx.Session.InvokeAction(WolfVars.HandleAbilities, true);
        }

        private static void StartPeekInput(GameContext ctx)
        {
            ctx.Server.GetDisplayChannel(WolfChannel.Peek).GetComponent("header").Draw(WritePeekHeader());
            foreach (PlayerData player in ctx.Session.Players.Where(CanPeek))
            {
                // The channel ID saved will be the player's own ID.
                ServerConnection connection = ctx.Server.Connections.FirstOrDefault(x => x.ChannelId == player.Player.User.Id);
                connection.BlockInput = false;
            }


            // Start a timer for 30 seconds that invokes the action 'end_peek_input'
            // Increase the timer for each seer in the game
            ctx.Session.QueueAction(TimeSpan.FromSeconds(30), WolfVars.EndPeekInput);
            // If nobody is able to peek, return

            // Get all of the players that are able to peek

            // If shared peeking is enabled, mimic the feast input

            // Otherwise, wait for all seers to finish peeking


        }

        private static void EndPeekInput(GameContext ctx)
        {
            foreach (PlayerData player in ctx.Session.Players.Where(CanPeek))
            {
                // The channel ID saved will be the player's own ID.
                ServerConnection connection = ctx.Server.Connections.FirstOrDefault(x => x.ChannelId == player.Player.User.Id);
                connection.BlockInput = true;
            }

            ctx.Session.SetValue(WolfVars.HandledInputs, ctx.Session.ValueOf<WerewolfAbility>(WolfVars.HandledInputs) | WerewolfAbility.Peek);
            // Add the Peek ability to the list of Read inputs, which determines what is left to check
            // Once everything has finished, block input to that channel, and continue with the night
            // Handle all abilities
            ctx.Session.InvokeAction(WolfVars.HandleAbilities, true);
        }

        private static void StartFeastInput(GameContext ctx)
        {
            // Start a timer for 30 seconds that invokes the action 'end_feast_input'
            // Increase the timer for each feast in the game
            ctx.Session.QueueAction(TimeSpan.FromSeconds(30), WolfVars.EndFeastInput);
            // If nobody is able to feast, return

            // Get all of the players that are able to feast

            // Set each player's private channel to the Feast Channel

            // Wait for every player in that channel to finish voting


        }

        private static void EndFeastInput(GameContext ctx)
        {
            // Once they are finished voting, block input to that channel, and continue with the night
            foreach (PlayerData player in ctx.Session.Players.Where(CanFeast))
            {
                // The channel ID saved will be the player's own ID.
                ServerConnection connection = ctx.Server.Connections.FirstOrDefault(x => x.ChannelId == player.Player.User.Id);
                connection.BlockInput = true;
            }

            ctx.Session.SetValue(WolfVars.HandledInputs, ctx.Session.ValueOf<WerewolfAbility>(WolfVars.HandledInputs) | WerewolfAbility.Feast);
            // Handle all abilities
            ctx.Session.InvokeAction(WolfVars.HandleAbilities, true);
        }

        private static void StartProtectInput(GameContext ctx)
        {
            // Start a timer for 30 seconds that invokes the action 'end_protect_input'
            // Increase the timer for each feast in the game
            ctx.Session.QueueAction(TimeSpan.FromSeconds(30), WolfVars.EndProtectInput);
        }

        private static void EndProtectInput(GameContext ctx)
        {

        }

        private static void Start(GameContext ctx)
        {
            // Set all connections to the main channel
            foreach (ServerConnection connection in ctx.Server.GetGroup("primary"))
                connection.Frequency = WolfChannel.Main;

            // If the game configuration supports night zero, handle it here.

            // Write the initial texts
            ctx.Server.GetDisplayChannel(WolfChannel.Main).Content.GetGroup("console").Append(WriteStartText());

            // Otherwise, go directly to the day phase
            ctx.Session.InvokeAction(WolfVars.StartDay);
        }

        private static string WriteStartText()
            => "A new dawn begins. Your village has been overrun with werewolves, and it is up to you to eliminate the threat before it becomes too much.";

        private static void GetResults(GameContext ctx)
        {
            SetChannel(ctx.Server, WolfChannel.Results);
            var group = ctx.Session.ValueOf<WerewolfGroup>(WolfVars.WinningGroup);

            foreach (PlayerData player in ctx.Session.Players.Where(IsDeadTanner))
                player.SetValue(WolfVars.IsWinner, true);

            foreach (PlayerData player in ctx.Session.Players.Where(x => group.HasFlag(x.ValueOf<WerewolfRole>(WolfVars.Role).Group)))
                player.SetValue(WolfVars.IsWinner, true);


            ctx.Server.GetDisplayChannel(WolfChannel.Results).GetComponent("header").Draw(group, ctx.Session.ValueOf(WolfVars.TotalRounds));
            ctx.Server.GetDisplayChannel(WolfChannel.Results).GetComponent("summary").Draw(WriteRandomWinSummary(group));
            ctx.Server.GetDisplayChannel(WolfChannel.Results).GetComponent("facts").Draw();

            ctx.Session.QueueAction(TimeSpan.FromSeconds(15), "end");
        }

        

        private static bool IsDeadTanner(PlayerData player)
            => player.ValueOf<WerewolfRole>(WolfVars.Role).Group == WerewolfGroup.Tanner && !IsAlive(player);

        private static bool IsMarkedForDeath(PlayerData player)
            => player.ValueOf<bool>(WolfVars.MarkedForDeath);

        private static bool IsHurt(PlayerData player)
            => player.ValueOf<bool>(WolfVars.IsHurt);

        private static bool HasRecentlyDied(PlayerData player)
            => player.ValueOf<bool>(WolfVars.IsDead) && !player.ValueOf<bool>(WolfVars.HasClosure);

        // ACTION start_day
        private static void StartDay(GameContext ctx)
        {
            // Set all of the primary channels to the main frequency
            foreach (ServerConnection connection in ctx.Server.GetGroup("primary"))
                connection.Frequency = WolfChannel.Main;

            // Set the current phase to day
            ctx.Session.SetValue(WolfVars.CurrentPhase, WerewolfPhase.Day);

            // Update the list of players
            WritePlayerList(ctx.Session, ctx.Server);
            WriteMainHeader(ctx.Server, ctx.Session);

            // If there was anyone that recently died or is marked for deaths, handle those first
            if (ctx.Session.Players.Any(HasRecentlyDied)
                || ctx.Session.Players.Any(IsMarkedForDeath))
            {
                // Set the next phase to day
                ctx.Session.SetValue(WolfVars.NextPhase, WerewolfPhase.Day);

                // Handle deaths
                ctx.Session.InvokeAction(WolfVars.HandleDeaths, true);
                return;
            }

            // Check winning conditions
            if (ctx.Session.MeetsCriterion(WolfVars.WolfGreaterEqualsVillager))
            {
                ctx.Session.SetValue(WolfVars.WinningGroup, WerewolfGroup.Werewolf);
                ctx.Session.InvokeAction(WolfVars.GetResults);
                return;
            }

            if (ctx.Session.MeetsCriterion(WolfVars.AllDeadWolf))
            {
                ctx.Session.SetValue(WolfVars.WinningGroup, WerewolfGroup.Villager);
                ctx.Session.InvokeAction(WolfVars.GetResults);
                return;
            }
            

            // Set the current phase to day
            ctx.Session.SetValue(WolfVars.CurrentPhase, WerewolfPhase.Day);

            // Start a timer for 3 minutes that will invoke the action 'end_day'
            ctx.Session.QueueAction("day_timer", TimeSpan.FromMinutes(3), WolfVars.EndDay);
        }

        // ACTION end_day
        private static void EndDay(GameContext ctx)
        {
            // specify the end of day
            ctx.Server.GetDisplayChannel(WolfChannel.Main)
                .Content.GetGroup("console").Append("The day has ended.");

            ctx.Session.QueueAction(TimeSpan.FromSeconds(10), WolfVars.StartNight);
            // ON ENTRY:
            // Start a timer for 10 seconds that invokes the action StartNight
        }

        private static bool HasSuspect(GameSession session)
            => session.ValueOf<ulong>(WolfVars.Suspect) != 0;

        private static void StartNight(GameContext ctx)
        {
            // Set the current phase to night
            ctx.Session.SetValue(WolfVars.CurrentPhase, WerewolfPhase.Night);

            // If there is anyone injured, kill them.
            foreach (PlayerData player in ctx.Session.Players.Where(IsHurt))
            {
                player.SetValue(WolfVars.IsHurt, false);
                player.SetValue(WolfVars.IsDead, true);
            }

            // Handle all abilities
            // ctx.Session.InvokeAction(WolfVars.HandleAbilities, true);

            // Since abilities don't work right now, only work with the voting mechanic
            ctx.Session.QueueAction(TimeSpan.FromSeconds(10), WolfVars.EndNight);
        }

        private static bool HasPrivateChannel(PlayerData player, GameServer server)
            => server.Connections.Any(x => x.ChannelId == player.Player.User.Id);

        private static void HandleAbilities(GameContext ctx)
        {
            var read = ctx.Session.ValueOf<WerewolfAbility>(WolfVars.HandledInputs);

            // Is there anyone that has the ability to peek
            if (!read.HasFlag(WerewolfAbility.Peek))
            {
                if (ctx.Session.Players.Any(CanPeek))
                    ctx.Session.InvokeAction(WolfVars.StartPeekInput);
            }

            // Is there anyone that has the ability to feast
            if (!read.HasFlag(WerewolfAbility.Feast))
            {
                if (ctx.Session.Players.Any(CanFeast))
                    ctx.Session.InvokeAction(WolfVars.StartFeastInput);
            }

            // Is there anyone that has the ability to protect
            if (!read.HasFlag(WerewolfAbility.Protect))
            {
                if (ctx.Session.Players.Any(CanProtect))
                    ctx.Session.InvokeAction(WolfVars.StartProtectInput);
            }

            ctx.Session.InvokeAction(WolfVars.EndNight);
        }

        private static void EndNight(GameContext ctx)
        {
            // Reset read inputs to none
            ctx.Session.SetValue(WolfVars.HandledInputs, WerewolfAbility.None);

            // Handle all text display updates here
            ctx.Server.GetDisplayChannel(WolfChannel.Main).Content.GetGroup("console").Append("The night has ended.");

            // Add 1 to the total rounds completed counter.
            ctx.Session.AddToValue(WolfVars.TotalRounds, 1);

            // Start a timer for 5 seconds that invokes the action 'start_day'
            ctx.Session.QueueAction(TimeSpan.FromSeconds(5), WolfVars.StartDay);
        }

        private static void TrySkipPhase(GameContext ctx)
        {
            // If enough requested skips were made, skip the current phase and start the new one

            // Start the new phase based on what the current phase was
        }

        

        private static bool WolfGreaterEqualsVillager(GameSession session)
            => CountWolves(session) >= CountVillagers(session);

        private static bool AllDeadWolf(GameSession session)
            => CountWolves(session) == 0;

        private static bool IsOnTrial(GameSession session)
            => session.ValueOf<bool>(WolfVars.HasTrial);

        // 'agree'
        private static void OnAgree(InputContext ctx)
        {
            // If the current phase is night, ignore input
            if (IsNight(ctx.Session))
                return;

            // Ensure that the invoker is in the current session
            if (ctx.Player == null)
                return;

            // If a suspect is not specified, ignore input
            if (!HasSuspect(ctx.Session))
                return;

            // If the suspect agrees, ignore input
            if (IsSuspect(ctx.Player, ctx.Session))
                return;

            // If the accuser agrees, ignore input
            if (IsAccuser(ctx.Player, ctx.Session))

            // This gets the queued action of the specified ID, and pauses it.
            ctx.Session.GetInQueue("day_timer").Pause();
            ctx.Session.SetValue(WolfVars.HasTrial, true);
            // Otherwise, start trial
            ctx.Session.InvokeAction(WolfVars.StartTrial);
        }

        private static bool IsAccuser(PlayerData player, GameSession session)
            => session.ValueOf<ulong>(WolfVars.Accuser) == player.Player.User.Id;

        private static string WriteFeastPartners(PlayerData player, GameSession session)
        {
            // Filter out non-feaster and self
            var partners = session.Players.Where(CanFeast).Where(x => x.Player.User.Id != player.Player.User.Id);

            if (partners.Count() == 0)
                return "> You are a lone wolf.";

            var feastInfo = new StringBuilder();

            feastInfo.AppendLine("> These are your partners:");

            foreach (PlayerData partner in partners)
                feastInfo.AppendLine($"• {partner.Player.User.Username}");

            return feastInfo.ToString();
        }

        private static bool IsDefending(GameSession session)
            => session.ValueOf<bool>(WolfVars.AwaitDefense);
        
        // 'say' <defense>
        private static void OnSpeak(InputContext ctx)
        {
            // If there isn't a current trial active, ignore it.
            if (!IsOnTrial(ctx.Session))
                return;

            // If the defend isn't active, ignore input.
            if (!IsDefending(ctx.Session))
                return;

            // If the person that executed this command is not the suspect, ignore input
            if (ctx.Session.ValueOf<ulong>(WolfVars.Suspect) != ctx.Player.Player.User.Id)
                return;

            // Otherwise, skip the initial command input and write what they said:
            string statement = ctx.Input.Source.Substring(3);

            // If their statement is empty, ignore input
            if (!Check.NotNull(statement))
                return;

            ctx.Server.GetDisplayChannel(WolfChannel.Main).Content.GetGroup("console").Append($"The suspect has said in their defense: {statement}");
            ctx.Session.QueueAction(TimeSpan.FromSeconds(5), WolfVars.StartVoteInput);
        }

        private static bool IsNight(GameSession session)
            => session.ValueOf<WerewolfPhase>(WolfVars.CurrentPhase) == WerewolfPhase.Night;

        // 'live'
        private static void OnLive(InputContext ctx)
        {
            // If there isn't a current trial active, ignore it
            if (!IsOnTrial(ctx.Session))
                return;

            // If their vote is not pending, ignore their input
            if (ctx.Player.ValueOf<WerewolfVote>(WolfVars.Vote) != WerewolfVote.Pending)
                return;

            // Otherwise, set their vote to live
            ctx.Player.SetValue(WolfVars.Vote, WerewolfVote.Live);

            // Update the vote counter
            ctx.Server.GetDisplayChannel(WolfChannel.Vote).Content.GetComponent("counter").Draw(WriteVoteCounter(ctx.Session));

            // Try to end the vote session
            ctx.Session.InvokeAction(WolfVars.TryEndVote);
        }

        // 'live'
        private static void OnDie(InputContext ctx)
        {
            // If there isn't a current trial active, ignore it
            if (!IsOnTrial(ctx.Session))
                return;

            // If their vote is not pending, ignore their input
            if (ctx.Player.ValueOf<WerewolfVote>(WolfVars.Vote) != WerewolfVote.Pending)
                return;

            // Otherwise, set their vote to live
            ctx.Player.SetValue(WolfVars.Vote, WerewolfVote.Die);

            // Update the vote counter
            ctx.Server.GetDisplayChannel(WolfChannel.Vote).Content.GetComponent("counter").Draw(WriteVoteCounter(ctx.Session));

            // Try to end the vote session
            ctx.Session.InvokeAction(WolfVars.TryEndVote);
        }

        // 'accuse'
        private static void OnAccuse(InputContext ctx)
        {
            // If the current phase is night, ignore input
            if (IsNight(ctx.Session))
                return;

            // Ensure that the invoker is in the current session
            if (ctx.Player == null)
                return;

            // If a suspect is already specified, ignore input
            if (HasSuspect(ctx.Session))
                return;

            // Otherwise, read input and attempt to find the specified player
            // use StringReader class

            string user = ctx.Input.Args.FirstOrDefault();

            // If the specified user is not valid
            if (Check.NotNull(user))
                return;

            IEnumerable<PlayerData> players = ctx.Session.Players.Where(x => TryParsePlayer(x, user));

            if (players.Count() == 1)
            {
                // If it is the same player
                if (players.First().Player.User.Id == ctx.Player.Player.User.Id)
                    return;

                ctx.Session.SetValue(WolfVars.Suspect, players.First().Player.User.Id);
                ctx.Server.GetDisplayChannel(WolfChannel.Main).Content.GetGroup("console").Append(WriteAccuseText(ctx.Player, players.First()));
                ctx.Server.GetDisplayChannel(WolfChannel.Main).Content.GetComponent("console").Draw();

                // This gets the queued action of the specified ID, and pauses it.
                ctx.Session.GetInQueue("day_timer").Pause();
                ctx.Session.QueueAction(TimeSpan.FromSeconds(5), WolfVars.RemoveSuspect);
            }

            // If a user could be found, mark them as the suspect and return
            // Otherwise, ignore input
        }

        private static PlayerData GetSuspect(GameSession session)
            => session.Players.First(x => IsSuspect(x, session));

        private static void RemoveSuspect(GameContext ctx)
        {
            ctx.Session.SetValue(WolfVars.Suspect, 0UL);
            ctx.Session.SetValue(WolfVars.Accuser, 0UL);
            ctx.Server.GetDisplayChannel(WolfChannel.Main).Content.GetGroup("console").Append("The accusation has subsided.");
            ctx.Server.GetDisplayChannel(WolfChannel.Main).GetComponent("console").Draw();
            // This gets the queued action of the specified ID, and resumes it.
            ctx.Session.GetInQueue("day_timer").Resume();
        }

        // 'skip'
        private static void OnSkip(InputContext ctx)
        {
            // If the current phase is night, ignore input
            if (IsNight(ctx.Session))
                return;

            // Ensure that the invoker is in the current session
            if (ctx.Player == null)
                return;

            // Mark the player with the request to skip

            // If the current phase does not supports skipping, ignore input
            if (!CanSkipCurrentPhase(ctx.Session))
                return;
            
            // Otherwise, add one to the total requested skips
            ctx.Session.AddToValue(WolfVars.RequestedSkips, 1);

            // Attempt to skip the current phase
            ctx.Session.InvokeAction(WolfVars.TrySkipPhase);
        }

        private static string WritePeekHeader()
            => "🔍 Choose a player to inspect:";

        private static string WriteHuntHeader()
            => "🔫 You have been chosen for tonight's feast. Choose someone to kill before this occurs!";

        // 'peek <user>'
        private static void OnPeek(InputContext ctx)
        {
            string user = ctx.Input.Args.First();

            IEnumerable<PlayerData> validPlayers = ctx.Session.Players.Where(x => TryParsePlayer(x, user));
            
            if (validPlayers.Count() != 1)
            {
                Console.WriteLine("invalid players returned");
                return;
            }

            PlayerData player = validPlayers.First();

            // If it is the same player
            if (player.Player.User.Id == ctx.Player.Player.User.Id)
                return;

            var peeks = ctx.Player.ValueOf<Dictionary<ulong, bool>>(WolfVars.PeekedPlayerIds);

            if (peeks.ContainsKey(player.Player.User.Id))
            {
                Console.WriteLine("Player has already been peeked. Ignoring input...");
                return;
            }

            peeks.Add(player.Player.User.Id, GetPassive(player).HasFlag(WerewolfPassive.Wolfish));
            ctx.Server.GetDisplayChannel(WolfChannel.Peek).GetComponent("header").Draw(WritePeekResult(player));
        }

        // 'feast <user>'
        private static void OnFeast(InputContext ctx)
        {
            string user = ctx.Input.Args.First();

            IEnumerable<PlayerData> validPlayers = ctx.Session.Players.Where(x => TryParsePlayer(x, user));

            if (validPlayers.Count() != 1)
            {
                Console.WriteLine("invalid players returned");
                return;
            }

            PlayerData player = validPlayers.First();

            // If it is the same player
            if (player.Player.User.Id == ctx.Player.Player.User.Id)
                return;

            // handle feast input here
        }

        // 'protect <user>'
        private static void OnProtect(InputContext ctx)
        {
            string user = ctx.Input.Args.First();

            IEnumerable<PlayerData> validPlayers = ctx.Session.Players.Where(x => TryParsePlayer(x, user));

            if (validPlayers.Count() != 1)
            {
                Console.WriteLine("invalid players returned");
                return;
            }

            PlayerData player = validPlayers.First();

            // If it is the same player
            if (player.Player.User.Id == ctx.Player.Player.User.Id)
                return;

            // handle protect input here
        }

        // 'shoot <user>'
        private static void OnShoot(InputContext ctx)
        {
            string user = ctx.Input.Args.First();

            IEnumerable<PlayerData> validPlayers = ctx.Session.Players.Where(x => TryParsePlayer(x, user));

            if (validPlayers.Count() != 1)
            {
                Console.WriteLine("invalid players returned");
                return;
            }

            PlayerData player = validPlayers.First();

            // If it is the same player
            if (player.Player.User.Id == ctx.Player.Player.User.Id)
                return;

            // Handle shoot input here
        }

        #endregion

        

        private static void WritePlayerList(GameSession session, GameServer server)
        {
            DisplayContent main = server.GetDisplayChannel(WolfChannel.Main).Content;

            // Initialize all of the player slots
            foreach (PlayerData player in session.Players)
                main.GetGroup("players").Set(player.ValueOf<int>(WolfVars.Index), WritePlayerInfo(player, session));

            main.GetComponent("players").Draw();
        }

        private static string WriteDeathText(WerewolfDeathMethod method)
        {
            return method switch
            {
                WerewolfDeathMethod.Hunted => "While sleeping sound, the echoes of a rifle pierced the nightly atmosphere, putting an end to their breathing.",
                WerewolfDeathMethod.Wolf => "They were mauled by werewolves, leaving barely anything to identify them by.",
                WerewolfDeathMethod.Hang => "They have been left to hang from the suspicion of the village.",
                _ => "They have been eliminated from an unknown source."
            };
        }

        private static string WriteProtectText(PlayerData player)
            => $"{GetName(player)} was protected from the dangers that lurked last night.";

        private static string WriteHurtText(PlayerData player)
            => $"{GetName(player)} has been injured, but lives to tell the tale.";

        private static string WriteDeathRemainText(GameSession session)
            => $"> **{CountLiving(session):##,0}** {Format.TryPluralize("resident", CountLiving(session))} remain. Tread carefully.";

        private static string WriteAccuseText(PlayerData accuser, PlayerData suspect)
        {
            // Instead of referencing usernames, store it in a property value,
            // just in case Randomize names is enabled.
            return $"{accuser.Player.User.Username} has accused {suspect.Player.User.Username} of being a werewolf. Does anyone else agree?";
        }

        private static string[] WriteRandomFacts(GameSession session)
        {
            var strings = new List<string>();

            for (int i = 0; i < 3; i++)
            {
                strings.Add(WriteRandomFact(session));
            }

            return strings.ToArray();
        }

        private static string WriteRandomFact(GameSession session)
        {
            // get random facts??
            int i = RandomProvider.Instance.Next(0, 2);

            return i switch
            {
                // Who was the first to die?
                // Who got the most people killed?
                // How long did this game last?
                _ => "This is a random fact placeholder."
            };
        }

        private static string WriteRandomWinSummary(WerewolfGroup group)
        {
            return group switch
            {
                WerewolfGroup.Villager => "The villagers were able to snuff out all of the werewolves, cleansing their village once and for all.",
                WerewolfGroup.Werewolf => "After a long struggle, the werewolves were able to overthrow the village, providing a feast to last months.",
                _ => throw new Exception("Invalid winning group specified.")
            };
        }

        private static void WriteMainHeader(GameServer server, GameSession session)
        {
            DisplayContent main = server.GetDisplayChannel(WolfChannel.Main).Content;
            var totalRounds = session.ValueOf<int>(WolfVars.TotalRounds);
            var currentPhase = session.ValueOf<WerewolfPhase>(WolfVars.CurrentPhase);

            string phaseInfo = WritePhaseHeader(currentPhase);
            main.GetComponent("header").Draw(totalRounds.ToString("##,0"), phaseInfo);
        }

        private static string WritePhaseHeader(WerewolfPhase phase)
        {
            return phase switch
            {
                WerewolfPhase.Day => "🌤️ **Day** (**3:00** until **Night**)",
                WerewolfPhase.Night => "☄️ **Night**",
                _ => throw new Exception("Invalid phase specified")
            };
        }

        private static string WritePeekResult(PlayerData player)
        {
            if (GetPassive(player).HasFlag(WerewolfPassive.Wolfish))
                return $"**{player.Player.User.Username}** is a werewolf.";

            return $"**{player.Player.User.Username}** is innocent.";
        }

        private static string WritePlayerInfo(PlayerData player, GameSession session)
        {
            var info = new StringBuilder();

            // Get public expression icon
            info.Append(WriteExpression(player, session));

            info.Append($" • ");

            if (!IsAlive(player))
                info.Append($"~~*{player.Player.User.Username}*~~ (Dead)");
            else
            {
                info.Append($"**{player.Player.User.Username}**#{player.ValueOf<int>(WolfVars.Index)}");

                if (IsSuspect(player, session))
                {
                    info.Append(" (Suspect)");
                }
                else if (IsHurt(player))
                {
                    info.Append("(Hurt)");
                }
            }

            return info.ToString();
        }


        private static string WriteExpression(PlayerData player, GameSession session)
        {
            if (IsSuspect(player, session))
                return "😟";

            if (IsHurt(player))
                return "🤕";

            return "😐";
        }

        // This writes the list of peeks for a single seer
        private string WritePeekList(PlayerData peeker, GameSession session)
        {
            // If the specified player is not a peeker, throw an error
            if (!HasAbility(peeker, WerewolfAbility.Peek))
                throw new Exception("Expected a peeker, but is missing peek ability");

            var peeks = new StringBuilder();
            var peekedPlayers = peeker.ValueOf<Dictionary<ulong, bool>>(WolfVars.PeekedPlayerIds);

            foreach (PlayerData player in session.Players)
            {
                // If the specified player is the same as the peeker, ignore them
                if (player.Player.User.Id == peeker.Player.User.Id)
                    continue;

                // If shared peeking is enabled and the player can also peek, ignore them
                if (GetConfigValue<bool>("shared_peeking"))
                {
                    if (CanProtect(player))
                        continue;
                }

                // Get the player's peek state for the current peeker
                bool? peekState = null;

                if (peekedPlayers.ContainsKey(player.Player.User.Id))
                    peekState = peekedPlayers[player.Player.User.Id];
                
                peeks.AppendLine(WritePeekState(player, peekState));
            }

            return peeks.ToString();
        }

        // This writes a peek state for a single player on the current seer.
        private static string WritePeekState(PlayerData player, bool? peekState = null)
        {
            string state = peekState.HasValue ? peekState.Value ? "🐺" : "😇" : "🎭";

            return $"{state} • {player.Player.User.Username}";
        }

        private static bool TryParsePlayer(PlayerData player, string input)
        {
            bool isId = ulong.TryParse(input, out ulong id);

            if (isId)
                return id == player.Player.User.Id || (ulong)player.ValueOf<int>(WolfVars.Index) == id;

            return player.Player.User.Username.Equals(input, StringComparison.OrdinalIgnoreCase);
        }

        private int GetFrequencyFor(WerewolfAbility ability)
        {
            return ability switch
            {
                WerewolfAbility.Peek => WolfChannel.Peek,
                WerewolfAbility.Feast => WolfChannel.Feast,
                WerewolfAbility.Hunt => WolfChannel.Hunt,
                WerewolfAbility.Protect => WolfChannel.Protect,
                _ => throw new Exception("The specified ability does not have a dedicated frequency")
            };
        }
    }
}
