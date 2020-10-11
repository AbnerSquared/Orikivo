using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orikivo;
using Orikivo.Framework;
using Format = Orikivo.Format;
using Arcadia.Multiplayer.Games.Werewolf;

namespace Arcadia.Multiplayer.Games
{
    // When generating roles, one Werewolf-base role is required, and one Ability-based role is required (Seer)
    // In short, you want to include roles, but also try to keep the balance.
    // Never have the same amount of werewolf equal to player count
    // One werewolf per 3 people?

    public class WerewolfGame : GameBase
    {
        public WerewolfGame()
        {
            Id = "werewolf";

            Details = new GameDetails
            {
                Name = "Werewolf",
                Icon = "🐺",
                Summary = "Seek out the werewolves and save the village.",
                RequiredPlayers = 3,
                PlayerLimit = 16
            };

            Options = new List<GameOption>
            {
                GameOption.Create(WolfConfig.RevealRolesOnDeath, "Reveal roles on death", true, "Determines if a player's role is revealed when they die."),
                GameOption.Create(WolfConfig.RoleDeny, "Denied roles", WerewolfRoleDeny.None, "Specifies a set of roles to exclude from the game (unimplemented)."),
                GameOption.Create(WolfConfig.RandomizeNames, "Randomize names", false, "Toggles the randomization of names to hide the actual name of each player."),
                GameOption.Create(WolfConfig.SharedPeeking, "Shared peeking", false, "Determines if seers share the same peeking mechanic."),
                GameOption.Create(WolfConfig.RevealPartners, "Reveal partners", true, "Determines if all of your partners are revealed to you when the game first starts.")
            };
        }

        private List<string> RandomNames { get; set; }

        private List<WerewolfRole> GenerateRoles(WerewolfRoleDeny roleDeny, int playerCount)
        {
            // Load up all available default roles
            List<WerewolfRole> availableRoles = WerewolfRole.GetPack(WerewolfRolePack.Custom);

            // Remove all of the specified denied roles from the list of available roles
            foreach (WerewolfRoleDeny deny in roleDeny.GetFlags())
                availableRoles.RemoveAll(x => x.Id.Equals(deny.ToString(), StringComparison.OrdinalIgnoreCase));

            // Initialize the new list of available roles
            var roles = new List<WerewolfRole>();

            int werewolfCount = (int) Math.Floor(playerCount / (double) 3);
            int currentWolf = 0;
            // Handle game balance here
            for (var i = 0; i < playerCount; i++)
            {
                if (werewolfCount - currentWolf > 0)
                {
                    roles.Add(WerewolfRole.Werewolf);
                    currentWolf++;
                }
                else
                    roles.Add(WerewolfRole.Villager);
            }

            // Return the list of generated roles (this is a bit heavy on operations
            return Randomizer.Shuffle(roles).ToList();
        }

        #region Helpers
        private static int CountSkips(GameSession session)
            => session.Players.Count(x => x.ValueOf<bool>(WolfVars.HasRequestedSkip));

        private static int CountPlayers(GameSession session)
            => session.Players.Count;

        private static int CountLiving(GameSession session)
            => session.Players.Count(IsAlive);

        private static int CountWolves(GameSession session)
            => session.Players.Count(x => IsWolf(x) && IsAlive(x));

        private static int CountVillagers(GameSession session)
            => session.Players.Count(x => IsVillager(x) && IsAlive(x));

        private static int CountPendingVotes(GameSession session)
            => session.Players.Count(x => x.ValueOf<WerewolfVote>(WolfVars.Vote) == WerewolfVote.Pending);

        private static int CountLiveVotes(GameSession session)
            => session.Players.Count(x => x.ValueOf<WerewolfVote>(WolfVars.Vote) == WerewolfVote.Live);

        private static int CountDeathVotes(GameSession session)
            => session.Players.Count(x => x.ValueOf<WerewolfVote>(WolfVars.Vote) == WerewolfVote.Die);

        private static bool HasAbility(PlayerData player, WerewolfAbility ability)
            => player.ValueOf<WerewolfRole>(WolfVars.Role).Ability.HasFlag(ability);

        internal static bool IsAbilityShared(GameSession session, WerewolfAbility ability)
        {
            return ability switch
            {
                WerewolfAbility.Feast => true,
                WerewolfAbility.Peek => session.GetConfigValue<bool>(WolfConfig.SharedPeeking),
                _ => false
            };
        }

        private static bool CanUseAbilityOnSelf(WerewolfAbility ability)
        {
            return ability switch
            {
                WerewolfAbility.Protect => true,
                _ => false
            };
        }

        private static bool HasPassive(PlayerData player)
            => player.ValueOf<WerewolfRole>(WolfVars.Role).Passive != WerewolfPassive.None;

        private static bool HasPassive(PlayerData player, WerewolfPassive passive)
            => player.ValueOf<WerewolfRole>(WolfVars.Role).Passive.HasFlag(passive);

        private static bool HasAbility(PlayerData player)
            => player.ValueOf<WerewolfRole>(WolfVars.Role).Ability != WerewolfAbility.None;

        public static bool IsInGroup(PlayerData player, WerewolfGroup group)
            => player.ValueOf<WerewolfRole>(WolfVars.Role).Group.HasFlag(group);

        private static bool IsVillager(PlayerData player)
            => IsInGroup(player, WerewolfGroup.Villager);

        private static bool IsWolf(PlayerData player)
            => IsInGroup(player, WerewolfGroup.Werewolf);

        private static bool IsSuspect(PlayerData player, GameSession session)
            => session.ValueOf<ulong>(WolfVars.Suspect) == player.Source.User.Id;

        private static bool IsAlive(PlayerData player)
            => !GetStatus(player).HasFlag(WerewolfStatus.Dead);

        private static bool IsProtected(PlayerData player)
            => GetStatus(player).HasFlag(WerewolfStatus.Protected);

        private static void ClearAbilityVotes(GameSession session)
            => session.ValueOf<List<WerewolfVoteData>>(WolfVars.AbilityVotes).ForEach(x => x.VoterIds.Clear());

        private static int CountAbilityVotes(GameSession session)
            => session.ValueOf<List<WerewolfVoteData>>(WolfVars.AbilityVotes).Select(x => x.VoterIds.Count).Sum();

        private static List<WerewolfVoteData> GetAbilityVotes(GameSession session)
            => session.ValueOf<List<WerewolfVoteData>>(WolfVars.AbilityVotes);

        internal static WerewolfVoteData GetAbilityVote(GameSession session, PlayerData player)
            => session.ValueOf<List<WerewolfVoteData>>(WolfVars.AbilityVotes).FirstOrDefault(x => x.UserId == player.Source.User.Id);

        private static WerewolfPassive GetPassive(PlayerData player)
            => player.ValueOf<WerewolfRole>(WolfVars.Role).Passive;

        private static WerewolfAbility GetAbility(PlayerData player)
            => player.ValueOf<WerewolfRole>(WolfVars.Role).Ability;

        private static string GetName(PlayerData player)
            => player.ValueOf<string>(WolfVars.Name);

        private static PlayerData GetSuspect(GameSession session)
            => session.Players.FirstOrDefault(x => IsSuspect(x, session));

        private static WerewolfStatus GetStatus(PlayerData player)
            => player.ValueOf<WerewolfStatus>(WolfVars.Status);

        private static void AddStatus(PlayerData player, WerewolfStatus status)
            => player.SetValue(WolfVars.Status, GetStatus(player) | status);

        private static void RemoveStatus(PlayerData player, WerewolfStatus status)
            => player.SetValue(WolfVars.Status, GetStatus(player) & ~status);

        private static void SetStatus(PlayerData player, WerewolfStatus status)
            => player.SetValue(WolfVars.Status, status);

        private static void ClearStatus(PlayerData player)
            => player.SetValue(WolfVars.Status, WerewolfStatus.None);

        private static bool IsDeadTanner(PlayerData player)
            => player.ValueOf<WerewolfRole>(WolfVars.Role).Group == WerewolfGroup.Tanner && !IsAlive(player);

        private static bool IsMarked(PlayerData player)
            => GetStatus(player).HasFlag(WerewolfStatus.Marked);

        private static bool IsHurt(PlayerData player)
            => GetStatus(player).HasFlag(WerewolfStatus.Hurt);

        private static bool IsAccuser(PlayerData player, GameSession session)
            => session.ValueOf<ulong>(WolfVars.Accuser) == player.Source.User.Id;

        private static bool IsOnTrial(GameSession session)
            => session.ValueOf<bool>(WolfVars.IsOnTrial);

        private static bool IsNight(GameSession session)
            => session.ValueOf<WerewolfPhase>(WolfVars.CurrentPhase) == WerewolfPhase.Night;

        private static bool HasAnyRecentlyDied(GameSession session)
            => session.ValueOf<List<WerewolfDeath>>(WolfVars.Deaths).Any(x => !x.Handled);

        private static bool HasSuspect(GameSession session)
            => session.ValueOf<ulong>(WolfVars.Suspect) > 0UL;

        private static bool HasPrivateChannel(PlayerData player, GameServer server)
            => server.Connections.Any(x => x.ChannelId == player.Source.User.Id);

        private static void InheritRole(PlayerData player, WerewolfRole role)
            => player.SetValue(WolfVars.Role, role);

        private static bool CanSkipCurrentPhase(GameSession session)
            => session.ValueOf<WerewolfPhase>(WolfVars.CurrentPhase).HasFlag(WerewolfPhase.Day);

        private static bool AllDeadWolf(GameSession session)
            => CountWolves(session) == 0;

        private static bool WolfGreaterEqualsVillager(GameSession session)
            => CountWolves(session) >= CountVillagers(session);

        private static void SetChannel(GameServer server, int frequency)
        {
            foreach (ServerConnection connection in server.GetGroup("primary"))
                connection.Frequency = frequency;
        }

        private static WerewolfAbility GetCurrentAbility(GameSession session)
            => session.ValueOf<WerewolfAbility>(WolfVars.CurrentAbility);

        private static WerewolfAbility GetActiveAbilities(GameSession session)
        {
            return GetLivingPlayers(session)
                .Select(x => x.ValueOf<WerewolfRole>(WolfVars.Role).Ability)
                .Aggregate(WerewolfAbility.None, (current, ability) => current | ability);
        }

        private static bool CanUseAbility(PlayerData invoker, GameSession session)
            => HasAbility(invoker, GetCurrentAbility(session));

        private static IEnumerable<PlayerData> GetLivingPlayers(GameSession session)
            => session.Players.Where(IsAlive);

        private static IEnumerable<PlayerData> GetLivingWolves(GameSession session)
            => session.Players.Where(x => IsAlive(x) && IsWolf(x));

        private static DisplayContent BuildAbilityContent()
        {
            return new DisplayContent
            {
                new Component("header", 0)
                {
                    Formatter = new ComponentFormatter("> {0}", true)
                },
                new Component("players", 1)
                {
                    Formatter = new ComponentFormatter("\n{0}", true)
                }
            };
        }

        private static bool TryParsePlayer(PlayerData player, string input)
        {
            if (ulong.TryParse(input, out ulong id))
                return id == player.Source.User.Id || (ulong)player.ValueOf<int>(WolfVars.Index) == id;

            return player.Source.User.Username.Equals(input, StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        private PlayerData CreatePlayer(Player player, WerewolfRole role, int index)
        {
            Logger.Debug("Creating player...");
            var data = new PlayerData(player)
            {
                Properties = new List<GameProperty>
                {
                    // index: Used to keep track of the player as a unique indexer
                    GameProperty.Create(WolfVars.Index, index),

                    // name: The player's name.
                    GameProperty.Create(WolfVars.Name, (((bool?) Options.FirstOrDefault(x => x.Id == WolfConfig.RandomizeNames)?.Value) ?? false) ? Randomizer.Take(RandomNames) : player.User.Username),

                    // initial_role: The ID of their initial role given
                    GameProperty.Create(WolfVars.InitialRoleId, role.Id),

                    // role: Their current role
                    GameProperty.Create(WolfVars.Role, role),

                    // is_winner: If the player was a winner in this game
                    GameProperty.Create(WolfVars.IsWinner, true),

                    // vote: The current vote state of the player
                    GameProperty.Create(WolfVars.Vote, WerewolfVote.Pending),

                    // status: The current status of the player
                    GameProperty.Create(WolfVars.Status, WerewolfStatus.None),

                    // has_requested_skip: Checks if the player has already requested to skip
                    GameProperty.Create(WolfVars.HasRequestedSkip, false),

                    GameProperty.Create(WolfVars.HasUsedAbility, false)
                }
            };

            // Remove this once this is assured to be okay
            Console.WriteLine(data.ToString());
            return data;
        }

        #region Required
        public override List<PlayerData> OnBuildPlayers(in IEnumerable<Player> players)
        {
            if ((bool?) Options.FirstOrDefault(x => x.Id == WolfConfig.RandomizeNames)?.Value ?? false)
                RandomNames = Randomizer.ChooseMany(WolfFormat.DefaultRandomNames, players.Count()).ToList();

            List<WerewolfRole> roles = GenerateRoles((WerewolfRoleDeny?) Options.FirstOrDefault(x => x.Id == WolfConfig.RoleDeny)?.Value ?? WerewolfRoleDeny.None, players.Count());

            return players.Select((x, i) => CreatePlayer(x, Randomizer.Take(roles), i)).ToList();
        }

        public override void OnPlayerRemoved(Player player)
        {
            // If a player was removed:

            var death = new WerewolfDeath(player.User.Id, WerewolfDeathMethod.Unknown);

            // If the game has already ended, ignore it

            // If the current phase is day-time AND there isn't an accusation in place, handle their death immediately

            // If the current phase is night, add their information as an unhandled death
            // If an ability is being handled, remove their vote data
            // If the ability was already called on someone AND they left, allow the player to choose another
            // If an ability vote session is called and someone leaves, remove all of the votes on that player
            // AND if there was at least one vote on that player, extend the ability vote timer by 10 seconds.
        }

        public override List<GameProperty> OnBuildProperties()
        {
            return new List<GameProperty>
            {
                // deaths: A list of all deaths in werewolf
                GameProperty.Create(WolfVars.Deaths, new List<WerewolfDeath>()),

                // peeks: A list of all peek information
                GameProperty.Create(WolfVars.Peeks, new List<WerewolfPeekData>()),

                // lovers: a dictionary of all specified lovers
                GameProperty.Create(WolfVars.Lovers, new Dictionary<ulong, ulong>()),

                // suspect: The ID of the current suspect
                GameProperty.Create(WolfVars.Suspect, 0UL),

                GameProperty.Create(WolfVars.Accuser, 0UL),

                GameProperty.Create<WerewolfDeath>(WolfVars.CurrentDeath),

                // current_input: The current input being handled
                GameProperty.Create(WolfVars.CurrentAbility, WerewolfAbility.None),

                // current_phase: The current phase being handled
                GameProperty.Create(WolfVars.CurrentPhase, WerewolfPhase.Unknown),

                // next_phase: The next phase to set at the end of the current phase
                GameProperty.Create(WolfVars.NextPhase, WerewolfPhase.Unknown),

                // current_round: The total counter of passed rounds
                GameProperty.Create(WolfVars.CurrentRound, 0),

                // handled_inputs: The total set of abilities handled
                GameProperty.Create(WolfVars.HandledAbilities, WerewolfAbility.None),

                // ability_votes: Keeps track of all of the players voted for during an ability
                GameProperty.Create(WolfVars.AbilityVotes, new List<WerewolfVoteData>()),

                // is_on_trial: Checks to see if the game is currently waiting for the suspect's defense
                GameProperty.Create(WolfVars.IsOnTrial, false),

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
                new GameAction(WolfVars.HandleTrial, StartTrial),
                new GameAction(WolfVars.StartVote, StartVote),
                new GameAction(WolfVars.EndVote, EndVote),
                new GameAction(WolfVars.StartNight, StartNight),
                new GameAction(WolfVars.EndNight, EndNight),
                new GameAction(WolfVars.HandleDeaths, HandleDeaths),
                new GameAction(WolfVars.TryEndPhase, TrySkipPhase),
                new GameAction(WolfVars.EndConviction, EndConviction),
                new GameAction(WolfVars.HandleDeath, HandleDeath),
                new GameAction(WolfVars.TryEndVote, TryEndVote, false),
                new GameAction(WolfVars.StartAbility, StartAbility),
                new GameAction(WolfVars.EndAbility, EndAbility),
                new GameAction(WolfVars.TryEndAbility, TryEndAbility),
                new GameAction(WolfVars.HandleAbilities, HandleAbilities)
            };
        }

        public override async Task OnSessionStartAsync(GameServer server, GameSession session)
        {
            // Set all of the currently connected channels to the specified frequency and group them
            server.SetStateFrequency(GameState.Playing, WolfChannel.Main);
            server.GroupAll("primary");
            session.SpectateFrequency = WolfChannel.Main;

            DisplayContent main = server.GetBroadcast(WolfChannel.Main).Content;

            foreach (PlayerData player in session.Players)
            {
                session.ValueOf<List<WerewolfPeekData>>(WolfVars.Peeks).Add(new WerewolfPeekData(player.Source.User.Id, HasPassive(player, WerewolfPassive.Wolfish)));
                session.ValueOf<List<WerewolfVoteData>>(WolfVars.AbilityVotes).Add(new WerewolfVoteData(player.Source.User.Id));
                Logger.Debug("Loading player...");
                // Set this stuff up AFTER you point all of the connections to the right frequency
                if (HasAbility(player))
                {
                    var properties = ConnectionProperties.Default;
                    properties.Frequency = -1;
                    properties.State = GameState.Playing;
                    properties.ContentOverride = BuildAbilityContent();
                    properties.ContentOverride.ValueOverride = WolfFormat.WriteRoleInfo(player, session);
                    properties.Inputs = new List<IInput> { new TextInput("pick", OnPick, true) };
                    properties.Origin = OriginType.Session;

                    await server.AddConnectionAsync(player.Source, properties);
                }

                var index = player.ValueOf<int>(WolfVars.Index);

                // Initialize all of the player slots on the main channel
                main.GetGroup("players").Set(index, WolfFormat.WritePlayerInfo(player, session));
            }

            main["players"].Draw();
            session.InvokeAction(WolfVars.Start, true);
        }

        public override SessionResult OnSessionFinish(GameSession session)
        {
            var result = new SessionResult();

            // For each player, update stats if they won, played, killed, and voted

            return result;
        }

        public override List<DisplayBroadcast> OnBuildBroadcasts(List<PlayerData> players)
        {
            return new List<DisplayBroadcast>
            {
                new DisplayBroadcast(WolfChannel.Main)
                {
                    Content = new DisplayContent
                    {
                        new Component("header", 0)
                        {
                            Formatter = new ComponentFormatter("> **Round {0}**\n> {1}", true)
                        },
                        new ComponentGroup("console", 1, 6)
                        {
                            Formatter = new ComponentFormatter("```{0}```", "• {0}", "\n"),
                            Values = new string[6],
                            AutoDraw = true
                        },
                        new ComponentGroup("players", 2, players.Count)
                        {
                            Formatter = new ComponentFormatter("> **Residents**\n{0}", "{0}", "\n"),
                            Values = new string[players.Count]
                        }
                    },
                    Inputs = new List<IInput>
                    {
                        new TextInput("agree", OnAgree),
                        new TextInput("accuse", OnAccuse, true),
                        new TextInput("skip", OnSkip),
                        new TextInput("say", OnSay, true),
                        new TextInput("silent", OnSilent, true)
                    }
                },
                new DisplayBroadcast(WolfChannel.Death)
                {
                    Content = new DisplayContent
                    {
                        new Component("header", 0)
                        {
                            Formatter = new ComponentFormatter("> 💀 **{0}** has died.", true)
                        },
                        new Component("summary", 1)
                        {
                            Formatter = new ComponentFormatter("```{0}```", true)
                        },
                        new Component("reveal", 2)
                        {
                            Formatter = new ComponentFormatter("> {0}{1}", true)
                        }
                    }
                },
                new DisplayBroadcast(WolfChannel.Vote)
                {
                    Content = new DisplayContent
                    {
                        new Component("header", 0)
                        {
                            Formatter = new ComponentFormatter("> Should **{0}** be killed for their suspicion?", true)
                        },
                        new Component("counter", 1)
                        {
                            Formatter = new ComponentFormatter("> {0}", true)
                        }
                    },
                    Inputs = new List<IInput>
                    {
                        new TextInput("live", OnLive, true),
                        new TextInput("die", OnDie, true)
                    }
                },
                new DisplayBroadcast(WolfChannel.Results)
                {
                    Content = new DisplayContent
                    {
                        new Component("header", 0)
                        {
                            Formatter = new ComponentFormatter("> The **{0}** win.\n> **Rounds: {1}**", true)
                        },
                        new Component("summary", 1)
                        {
                            Formatter = new ComponentFormatter("```{0}```", true)
                        },
                        new ComponentGroup("facts", 2, 1, false)
                        {
                            Formatter = new ComponentFormatter("{0}","• {0}", "\n"),
                            Values = new[] { "This is a random fact placeholder." }
                        }
                    }
                }
            };
        }

        #endregion

        #region Actions

        private static void HandleDeath(GameContext ctx)
        {
            DisplayContent death = ctx.Server.GetBroadcast(WolfChannel.Death).Content;

            // Set all of the primary channels to the death frequency
            foreach (ServerConnection connection in ctx.Server.GetGroup("primary"))
                connection.Frequency = WolfChannel.Death;

            ctx.Session.BlockInput = true;

            var kill = ctx.Session.ValueOf<WerewolfDeath>(WolfVars.CurrentDeath);

            // If null, throw an exception
            if (kill == null)
                throw new Exception("Attempted to invoke a death sequence with no player killed");

            PlayerData player = ctx.Session.DataOf(kill.UserId);
            bool hasGameEnded = WolfGreaterEqualsVillager(ctx.Session) || AllDeadWolf(ctx.Session);
            string extraText = hasGameEnded ? "" : WolfFormat.WriteDeathRemainText(ctx.Session);

            // Write to the main channel the information about who died
            death["header"].Draw(player.Source.User.Username);
            death["summary"].Draw(WolfFormat.WriteDeathText(kill.Method));
            if (Check.NotNull(extraText) && !ctx.Session.GetConfigValue<bool>(WolfConfig.RevealRolesOnDeath))
            {
                death["reveal"].Active = false;
            }
            else
            {
                death["reveal"].Draw(ctx.Session.GetConfigValue<bool>(WolfConfig.RevealRolesOnDeath) ? $"They were a **{player.ValueOf<WerewolfRole>(WolfVars.Role).Name}**." : "", extraText);
            }

            // $"They were a **{player.ValueOf<WerewolfRole>(WolfVars.Role).Name}**."
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
            DisplayContent main = ctx.Server.GetBroadcast(WolfChannel.Main).Content;

            // If there aren't any deaths, proceed through with the current phase
            ctx.Session.BlockInput = true;
            var deaths = ctx.Session.ValueOf<List<WerewolfDeath>>(WolfVars.Deaths);


            foreach (WerewolfDeath death in deaths.Where(x => !x.Handled))
            {
                if (!IsAlive(ctx.Session.DataOf(death.UserId)))
                    throw new Exception("Expected dead player but is still alive");

                ctx.Session.BlockInput = false;
                ctx.Session.SetValue(WolfVars.CurrentDeath, death);
                ctx.Session.InvokeAction(WolfVars.HandleDeath, true);
                return;
            }

            // If anyone was marked for death
            foreach (PlayerData player in ctx.Session.Players.Where(IsMarked))
            {
                if (IsProtected(player))
                {
                    RemoveStatus(player, WerewolfStatus.Marked | WerewolfStatus.Protected);
                    main.GetGroup("console").Append(WolfFormat.WriteProtectText(player));
                    continue;
                }

                // If the player is a tough person
                if (GetPassive(player).HasFlag(WerewolfPassive.Tough))
                {
                    RemoveStatus(player, WerewolfStatus.Marked);
                    AddStatus(player, WerewolfStatus.Hurt);
                    main.GetGroup("console").Append(WolfFormat.WriteHurtText(player));
                    continue;
                }

                var death = new WerewolfDeath(player.Source.User.Id, WerewolfDeathMethod.Wolf,
                    GetLivingWolves(ctx.Session).Select(x => x.Source.User.Id).ToList());

                SetStatus(player, WerewolfStatus.Dead);
                ctx.Session.ValueOf<List<WerewolfDeath>>(WolfVars.Deaths).Add(death);
                ctx.Session.SetValue(WolfVars.CurrentDeath, death);
                ctx.Session.BlockInput = false;
                ctx.Session.InvokeAction(WolfVars.HandleDeath, true);
                return;
            }

            ctx.Session.BlockInput = false;
            ctx.Session.InvokeAction(WolfVars.StartDay);
        }

        private static void HandleAbilities(GameContext ctx)
        {
            var handled = ctx.Session.ValueOf<WerewolfAbility>(WolfVars.HandledAbilities);

            // Get the active abilities, and exclude the already handled abilities
            foreach (WerewolfAbility ability in (GetActiveAbilities(ctx.Session) & ~handled).GetFlags())
            {
                // Ignore the empty ability
                if (ability == WerewolfAbility.None)
                    continue;

                Logger.Debug($"Handling ability {ability}");
                ctx.Session.SetValue(WolfVars.CurrentAbility, ability);
                ctx.Session.InvokeAction(WolfVars.StartAbility, true);
                return;
            }

            ctx.Session.InvokeAction(WolfVars.EndNight);
        }

        private static void Start(GameContext ctx)
        {
            // If the game configuration supports night zero, handle it here.
            ctx.Session.CanSpectate = true;
            // Write the initial texts
            ctx.Server.GetBroadcast(WolfChannel.Main).Content.GetGroup("console").Append(WolfFormat.WriteStartText());

            // Otherwise, go directly to the day phase
            ctx.Session.InvokeAction(WolfVars.StartDay);
        }

        private static void StartDay(GameContext ctx)
        {
            // Set the current phase to day
            ctx.Session.SetValue(WolfVars.CurrentPhase, WerewolfPhase.Day);

            // Update the display
            SetChannel(ctx.Server, WolfChannel.Main);
            WolfFormat.WritePlayerList(ctx.Session, ctx.Server);
            WolfFormat.WriteMainHeader(ctx.Server, ctx.Session);

            // If there was anyone that recently died or is marked for deaths, handle those first
            if (HasAnyRecentlyDied(ctx.Session)
                || ctx.Session.Players.Any(IsMarked))
            {
                // Set the next phase to day
                ctx.Session.SetValue(WolfVars.NextPhase, WerewolfPhase.Day);

                // Handle deaths
                ctx.Session.InvokeAction(WolfVars.HandleDeaths, true);
                return;
            }

            // Check winning conditions
            if (WolfGreaterEqualsVillager(ctx.Session))
            {
                ctx.Session.SetValue(WolfVars.WinningGroup, WerewolfGroup.Werewolf);
                ctx.Session.InvokeAction(WolfVars.GetResults);
                return;
            }

            if (AllDeadWolf(ctx.Session))
            {
                ctx.Session.SetValue(WolfVars.WinningGroup, WerewolfGroup.Villager);
                ctx.Session.InvokeAction(WolfVars.GetResults);
                return;
            }

            // Set the current phase to day
            ctx.Session.SetValue(WolfVars.CurrentPhase, WerewolfPhase.Day);
            ctx.Session.BlockInput = false;

            // Start a timer for 3 minutes that will invoke the action 'end_day'
            ctx.Session.QueueAction("day_timer", TimeSpan.FromMinutes(3), WolfVars.EndDay);
        }

        private static void StartTrial(GameContext ctx)
        {
            // Set the primary channel to the trial frequency

            // Start a timer for 30 seconds that invokes the action 'start_vote_input'
            ctx.Session.QueueAction(TimeSpan.FromSeconds(15), WolfVars.StartVote);
            ctx.Session.SetValue(WolfVars.IsOnTrial, true);
            ctx.Session.SetValue(WolfVars.Accuser, 0UL);
            ctx.Server.GetBroadcast(WolfChannel.Main).Content.GetGroup("console").Append("The suspect has 15 seconds to say their defense.");
            // While this timer runs, it waits for the suspect to write their defense piece
            //     - If RandomNames is enabled, this will be handled in their direct messages
            //     - Otherwise, the suspect writes their defense in the chat

            // If the suspect does not reply, they will be ignored
            // Likewise, they can also decide to remain silent
        }

        private static void StartVote(GameContext ctx)
        {
            // Set all connections to the vote channel
            SetChannel(ctx.Server, WolfChannel.Vote);
            ctx.Session.SetValue(WolfVars.IsOnTrial, false);
            ctx.Session.BlockInput = true;

            if (GetSuspect(ctx.Session) == null)
                throw new Exception("Expected suspect but returned null");

            // Write the initial texts
            DisplayContent vote = ctx.Server.GetBroadcast(WolfChannel.Vote).Content;

            vote["header"].Draw(GetName(GetSuspect(ctx.Session)));
            vote["counter"].Draw(WolfFormat.WriteVoteCounter(ctx.Session));

            if (GetLivingPlayers(ctx.Session).Any(x => HasPassive(x, WerewolfPassive.Militarist | WerewolfPassive.Pacifist)))
                throw new Exception("The specified player has a conflicting passive ability");

            ctx.Session.BlockInput = false;

            // Start a timer for 30 seconds that invokes the action 'end_vote_input'
            ctx.Session.QueueAction(TimeSpan.FromSeconds(30), WolfVars.EndVote);
            ctx.Session.InvokeAction(WolfVars.TryEndVote);
        }

        private static void StartNight(GameContext ctx)
        {
            ctx.Session.BlockInput = false;
            // Set the current phase to night
            ctx.Session.SetValue(WolfVars.CurrentPhase, WerewolfPhase.Night);
            ctx.Session.SetForEachPlayer(WolfVars.HasRequestedSkip, false);
            // Update the list of players
            WolfFormat.WritePlayerList(ctx.Session, ctx.Server);
            WolfFormat.WriteMainHeader(ctx.Server, ctx.Session);

            // If there is anyone injured, kill them.
            foreach (PlayerData player in ctx.Session.Players.Where(IsHurt))
            {
                var death = new WerewolfDeath(player.Source.User.Id, WerewolfDeathMethod.Injury);

                SetStatus(player, WerewolfStatus.Dead);
                ctx.Session.ValueOf<List<WerewolfDeath>>(WolfVars.Deaths).Add(death);
            }

            // Handle all abilities
            ctx.Session.InvokeAction(WolfVars.HandleAbilities, true);
        }

        private static void StartAbility(GameContext ctx)
        {
            Logger.Debug("Now handling ability...");
            WerewolfAbility current = GetCurrentAbility(ctx.Session);

            foreach (PlayerData player in ctx.Session.Players.Where(x => HasAbility(x, current)))
            {
                Logger.Debug("Initializing player connection...");
                // The channel ID saved will be the player's own ID.
                ServerConnection connection = ctx.Server.Connections.FirstOrDefault(x => x.UserId == player.Source.User.Id);

                if (connection == null)
                    throw new Exception("Expected player connection but returned null");

                // Remove the override, if any
                connection.ContentOverride.ValueOverride = null;
                connection.ContentOverride["header"].Draw(WolfFormat.WriteAbilityHeader(ctx.Session));
                connection.ContentOverride["players"].Draw(WolfFormat.WriteAbilityList(player, ctx.Session));
                connection.BlockInput = false;
            }

            // Start a timer for 30 seconds that invokes the action 'end_hunt_input'
            ctx.Session.QueueAction(TimeSpan.FromSeconds(30), WolfVars.EndAbility);
        }

        private static void GetResults(GameContext ctx)
        {
            SetChannel(ctx.Server, WolfChannel.Results);
            var group = ctx.Session.ValueOf<WerewolfGroup>(WolfVars.WinningGroup);

            foreach (PlayerData player in ctx.Session.Players.Where(x => IsDeadTanner(x) || group.HasFlag(x.ValueOf<WerewolfRole>(WolfVars.Role).Group)))
                player.SetValue(WolfVars.IsWinner, true);

            DisplayContent results = ctx.Server.GetBroadcast(WolfChannel.Results).Content;
            results["header"].Draw(Format.TryPluralize(group.ToString(), 2), ctx.Session.ValueOf(WolfVars.CurrentRound));
            results["summary"].Draw(WolfFormat.WriteRandomWinSummary(group));
            results["facts"].Draw();

            ctx.Session.QueueAction(TimeSpan.FromSeconds(15), "end");
        }

        private static PlayerData GetBestTarget(GameSession session)
        {
            if (!IsAbilityShared(session, GetCurrentAbility(session)))
                return null;

            if (session.ValueOf<List<WerewolfVoteData>>(WolfVars.AbilityVotes).Count == 0)
                throw new Exception("Expected to find value in ability votes but returned null");

            // Group by the count of voters and randomly select the best one if there's more than one value
            WerewolfVoteData best = Randomizer.Choose(session
                .ValueOf<List<WerewolfVoteData>>(WolfVars.AbilityVotes)
                .Where(x => IsAlive(session.DataOf(x.UserId)) && !HasAbility(session.DataOf(x.UserId), GetCurrentAbility(session)))
                .GroupBy(x => x.VoterIds.Count)
                .OrderByDescending(x => x.Key).First());

            return session.DataOf(best.UserId);
        }

        private static void EndAbility(GameContext ctx)
        {
            var current = ctx.Session.ValueOf<WerewolfAbility>(WolfVars.CurrentAbility);

            ClearAbilityVotes(ctx.Session);
            ctx.Session.SetForEachPlayer(WolfVars.HasUsedAbility, false);
            PlayerData target = GetBestTarget(ctx.Session);

            // If the ability is shared, invoke it as a group
            if (IsAbilityShared(ctx.Session, current))
               UseAbility(null, target, ctx.Session);

            foreach (PlayerData player in ctx.Session.Players.Where(x => IsAlive(x) && HasAbility(x, current)))
            {
                Logger.Debug($"Reading server connection for {player.Source.User.Username}");
                // The channel ID saved will be the player's own ID.
                ServerConnection connection = ctx.Server.Connections.FirstOrDefault(x => x.UserId == player.Source.User.Id);

                if (connection == null)
                    throw new Exception("Expected player connection but returned null");

                connection.BlockInput = true;

                if (IsAbilityShared(ctx.Session, current))
                {
                    Logger.Debug($"Updating shared ability content...");
                    connection.ContentOverride.GetComponent("header").Draw(WolfFormat.WriteAbilityResult(target, ctx.Session));
                    connection.ContentOverride.GetComponent("players").Draw(WolfFormat.WriteAbilityList(player, ctx.Session));
                }
                else if (!player.ValueOf<bool>(WolfVars.HasUsedAbility))
                {
                    Logger.Debug("Notifying that they ran out of time...");
                    connection.ContentOverride.GetComponent("header").Draw("You have run out of time.");
                }

                Logger.Debug("Handled player connection for closing ability");
            }

            // Update the handled abilities and continue checking
            ctx.Session.SetValue(WolfVars.HandledAbilities, ctx.Session.ValueOf<WerewolfAbility>(WolfVars.HandledAbilities) | current);
            ctx.Session.InvokeAction(WolfVars.HandleAbilities, true);
        }

        private static void EndVote(GameContext ctx)
        {
            // Get the vote counts
            int toLive = CountLiveVotes(ctx.Session);
            int toDie = CountDeathVotes(ctx.Session);
            int pending = CountPendingVotes(ctx.Session);

            var killers = new List<ulong>();

            // Reset all of the votes once you've received the vote counts
            foreach (PlayerData player in GetLivingPlayers(ctx.Session))
            {
                if (player.ValueOf<WerewolfVote>(WolfVars.Vote) == WerewolfVote.Die)
                    killers.Add(player.Source.User.Id);

                player.SetValue(WolfVars.Vote, WerewolfVote.Pending);
            }

            ctx.Session.BlockInput = true;
            ctx.Session.GetInQueue("day_timer").Cancel();
            // If the votes to die is more than the votes to live
            if (toDie > toLive + pending)
            {
                PlayerData suspect = ctx.Session.DataOf(ctx.Session.ValueOf<ulong>(WolfVars.Suspect));

                if (suspect == null)
                    throw new Exception("Expected suspect but returned null");

                // Mark everyone who voted for their death as a killer
                var death = new WerewolfDeath(suspect.Source.User.Id, WerewolfDeathMethod.Hang, killers);

                // Kill the suspect
                SetStatus(suspect, WerewolfStatus.Dead);

                // Add their death to the list
                ctx.Session.ValueOf<List<WerewolfDeath>>(WolfVars.Deaths).Add(death);
                ctx.Session.SetValue(WolfVars.CurrentDeath, death);

                // Set the next phase to night
                ctx.Session.SetValue(WolfVars.NextPhase, WerewolfPhase.Night);

                // Handle the suspect's death
                ctx.Session.SetValue(WolfVars.IsOnTrial, false);
                ctx.Session.SetValue(WolfVars.Suspect, 0UL);
                ctx.Session.SetValue(WolfVars.Accuser, 0UL);
                ctx.Session.BlockInput = false;
                ctx.Session.InvokeAction(WolfVars.HandleDeath, true);
            }
            else
            {
                // Otherwise, go directly to the night phase
                SetChannel(ctx.Server, WolfChannel.Main);
                ctx.Session.SetValue(WolfVars.IsOnTrial, false);
                ctx.Session.SetValue(WolfVars.Suspect, 0UL);
                ctx.Session.SetValue(WolfVars.Accuser, 0UL);
                ctx.Server.GetBroadcast(WolfChannel.Main).Content.GetGroup("console").Append("There weren't enough votes to allow the kill to go through.");
                ctx.Session.BlockInput = false;
                ctx.Session.QueueAction(TimeSpan.FromSeconds(5), WolfVars.EndDay);
            }
        }

        private static void EndDay(GameContext ctx)
        {
            ctx.Session.BlockInput = true;
            // specify the end of day
            ctx.Server.GetBroadcast(WolfChannel.Main).Content.GetGroup("console").Append("The day has ended.");
            WolfFormat.WritePlayerList(ctx.Session, ctx.Server);
            ctx.Session.QueueAction(TimeSpan.FromSeconds(10), WolfVars.StartNight);
            // ON ENTRY:
            // Start a timer for 10 seconds that invokes the action StartNight
        }

        private static void EndNight(GameContext ctx)
        {
            // Reset read inputs to none
            ctx.Session.SetValue(WolfVars.HandledAbilities, WerewolfAbility.None);
            ctx.Session.SetValue(WolfVars.CurrentAbility, WerewolfAbility.None);
            ctx.Session.SetValue(WolfVars.Suspect, 0UL);
            ctx.Session.SetValue(WolfVars.Accuser, 0UL);
            ctx.Session.AddToValue(WolfVars.CurrentRound, 1);

            // Handle all text display updates here
            ctx.Server.GetBroadcast(WolfChannel.Main).Content.GetGroup("console").Append("The night has ended.");
            WolfFormat.WritePlayerList(ctx.Session, ctx.Server);
            // Add 1 to the total rounds completed counter.
            Logger.Debug("Ending night...");
            // Start a timer for 5 seconds that invokes the action 'start_day'
            ctx.Session.QueueAction(TimeSpan.FromSeconds(5), WolfVars.StartDay);
        }

        private static void EndConviction(GameContext ctx)
        {
            ctx.Session.SetValue(WolfVars.Suspect, 0UL);
            ctx.Session.SetValue(WolfVars.Accuser, 0UL);
            ctx.Server.GetBroadcast(WolfChannel.Main).Content.GetGroup("console").Append("The conviction has subsided.");
            WolfFormat.WritePlayerList(ctx.Session, ctx.Server);
            ctx.Session.GetInQueue("day_timer").Resume();
            Logger.Debug($"Conviction canceled");
        }

        private static void TryEndAbility(GameContext ctx)
        {
            if (!IsAbilityShared(ctx.Session, GetCurrentAbility(ctx.Session)))
            {
                // Check if each player has used their ability
                if (ctx.Session.Players.Where(x => HasAbility(x, GetCurrentAbility(ctx.Session))).All(x => x.ValueOf<bool>(WolfVars.HasUsedAbility)))
                {
                    Logger.Debug("Ability has been used, now closing...");
                    ctx.Session.CancelNewestInQueue();
                    ctx.Session.InvokeAction(WolfVars.EndAbility, true);
                    return;
                }
            }

            int maxCount = ctx.Session.Players.Count(x => HasAbility(x, GetCurrentAbility(ctx.Session)));
            int abilityVoteCount = CountAbilityVotes(ctx.Session);

            Logger.Debug($"{abilityVoteCount}/{maxCount} votes found");

            // Otherwise, count to see if each player has voted
            if (abilityVoteCount != maxCount)
                return;

            Logger.Debug("Ability has been used, now closing...");
            ctx.Session.CancelNewestInQueue();
            ctx.Session.InvokeAction(WolfVars.EndAbility, true);
        }

        private static void TryEndVote(GameContext ctx)
        {
            // If all of the votes are secured, end the vote
            if (ctx.Session.Players
                .Where(x => !IsSuspect(x, ctx.Session))
                .All(x => x.ValueOf<WerewolfVote>(WolfVars.Vote) != WerewolfVote.Pending))
            {
                // Cancel the current voting timer
                ctx.Session.CancelNewestInQueue();

                // End the current vote
                ctx.Session.InvokeAction(WolfVars.EndVote);
            }
        }

        private static void TrySkipPhase(GameContext ctx)
        {
            if (CountSkips(ctx.Session) >= (CountLiving(ctx.Session) / (double) 2))
            {
                var current = ctx.Session.ValueOf<WerewolfPhase>(WolfVars.CurrentPhase);

                // If it's day time, end the day

                if (current == WerewolfPhase.Day)
                {
                    // Is there is currently a suspect specified, ignore
                    if (HasSuspect(ctx.Session))
                        return;

                    ctx.Session.GetInQueue("day_timer").Cancel();
                    ctx.Session.InvokeAction(WolfVars.EndDay);
                    ctx.Session.SetForEachPlayer(WolfVars.HasRequestedSkip, false);
                    return;
                }

                if (current == WerewolfPhase.Death)
                {
                    ctx.Session.CancelNewestInQueue();

                    var next = ctx.Session.ValueOf<WerewolfPhase>(WolfVars.NextPhase);
                    if (next == WerewolfPhase.Night)
                    {
                        ctx.Session.InvokeAction(WolfVars.StartNight);
                        return;
                    }

                    if (next == WerewolfPhase.Day)
                    {
                        ctx.Session.InvokeAction(WolfVars.StartDay);
                        ctx.Session.SetForEachPlayer(WolfVars.HasRequestedSkip, false);
                        return;
                    }
                }
            }
        }

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
                return;

            // If the trial has started, ignore
            if (IsOnTrial(ctx.Session))
                return;

            Logger.Debug("Starting trial...");
            ctx.Session.CancelNewestInQueue();
            ctx.Session.GetInQueue("day_timer").Pause();
            ctx.Session.SetValue(WolfVars.IsOnTrial, true);
            ctx.Session.InvokeAction(WolfVars.HandleTrial);
        }

        private static void OnSay(InputContext ctx)
        {
            // If there isn't a current trial active, ignore it.
            if (!HasSuspect(ctx.Session))
                return;

            // If the defend isn't active, ignore input.
            if (!IsOnTrial(ctx.Session))
                return;

            // If not the suspect, ignore input
            if (!IsSuspect(ctx.Player, ctx.Session))
                return;

            // Otherwise, skip the initial command input and write what they said:
            string statement = ctx.Input.Source.Substring(3);

            // If their statement is empty, ignore input
            if (!Check.NotNull(statement))
                return;

            Logger.Debug("Defense received");
            // Cancel the currently queued action (from 'start_trial', 30 seconds => 'start_vote_input')
            ctx.Session.CancelNewestInQueue();
            ctx.Server.GetBroadcast(WolfChannel.Main).Content.GetGroup("console").Append($"The suspect has spoken: {statement}");
            ctx.Session.SetValue(WolfVars.IsOnTrial, false);
            ctx.Session.QueueAction(TimeSpan.FromSeconds(5), WolfVars.StartVote);
        }

        private static void OnSilent(InputContext ctx)
        {
            // If there isn't a current trial active, ignore it.
            if (!HasSuspect(ctx.Session))
                return;

            // If the defend isn't active, ignore input.
            if (!IsOnTrial(ctx.Session))
                return;

            // If not the suspect, ignore input
            if (!IsSuspect(ctx.Player, ctx.Session))
                return;

            Logger.Debug($"Suspect chose to remain silent");
            // Cancel the currently queued action (from 'start_trial', 30 seconds => 'start_vote_input')
            ctx.Session.CancelNewestInQueue();
            ctx.Server.GetBroadcast(WolfChannel.Main).Content.GetGroup("console").Append("The suspect chose to remain silent.");
            ctx.Session.SetValue(WolfVars.IsOnTrial, false);
            ctx.Session.QueueAction(TimeSpan.FromSeconds(5), WolfVars.StartVote);
        }

        private static void OnLive(InputContext ctx)
        {
            // Ignore if there is no suspect
            if (!HasSuspect(ctx.Session))
                return;

            // Ignore if the suspect is on trial
            if (IsOnTrial(ctx.Session))
                return;

            // Ignore if the suspect is voting
            if (IsSuspect(ctx.Player, ctx.Session))
                return;

            // Ignore if their vote is not pending
            if (ctx.Player.ValueOf<WerewolfVote>(WolfVars.Vote) != WerewolfVote.Pending)
                return;

            // Otherwise, set their vote
            ctx.Player.SetValue(WolfVars.Vote, HasPassive(ctx.Player, WerewolfPassive.Militarist) ? WerewolfVote.Die : WerewolfVote.Live);

            // Update the vote counter
            ctx.Server.GetBroadcast(WolfChannel.Vote).Content.GetComponent("counter").Draw(WolfFormat.WriteVoteCounter(ctx.Session));

            // Try to end the vote session
            ctx.Session.InvokeAction(WolfVars.TryEndVote);
        }

        private static void OnDie(InputContext ctx)
        {
            // Ignore if there is no suspect
            if (!HasSuspect(ctx.Session))
                return;

            // Ignore if the suspect is on trial
            if (IsOnTrial(ctx.Session))
                return;

            // Ignore if the suspect is voting
            if (IsSuspect(ctx.Player, ctx.Session))
                return;

            // Ignore if their vote is not pending
            if (ctx.Player.ValueOf<WerewolfVote>(WolfVars.Vote) != WerewolfVote.Pending)
                return;

            // Otherwise, set their vote
            ctx.Player.SetValue(WolfVars.Vote, HasPassive(ctx.Player, WerewolfPassive.Pacifist) ? WerewolfVote.Live : WerewolfVote.Die);

            // Update the vote counter
            ctx.Server.GetBroadcast(WolfChannel.Vote).Content.GetComponent("counter").Draw(WolfFormat.WriteVoteCounter(ctx.Session));

            // Try to end the vote session
            ctx.Session.InvokeAction(WolfVars.TryEndVote);
        }

        private static void OnAccuse(InputContext ctx)
        {
            Logger.Debug("Accusation called");

            // Ignore if a player could not be found
            if (ctx.Player == null)
                return;

            // If the current phase is night, ignore input
            if (IsNight(ctx.Session))
                return;

            // If a suspect is already specified, ignore input
            if (HasSuspect(ctx.Session))
                return;

            // Otherwise, read input and attempt to find the specified player
            string reference = ctx.Input.Args.FirstOrDefault();

            if (!Check.NotNull(reference))
                return;

            if (ctx.Session.Players.Count(x => TryParsePlayer(x, reference)) == 1)
            {
                PlayerData suspect = ctx.Session.Players.FirstOrDefault(x => TryParsePlayer(x, reference));

                // If a player wasn't found
                if (suspect == null)
                    return;

                // If it is the same player
                if (suspect.Source.User.Id == ctx.Player.Source.User.Id)
                    return;

                DisplayContent main = ctx.Server.GetBroadcast(WolfChannel.Main).Content;

                ctx.Session.SetValue(WolfVars.Suspect, suspect.Source.User.Id);
                ctx.Session.SetValue(WolfVars.Accuser, ctx.Invoker.Id);

                main.GetGroup("console").Append(WolfFormat.WriteAccuseText(ctx.Player, suspect));
                main["console"].Draw();

                WolfFormat.WritePlayerList(ctx.Session, ctx.Server);

                // This gets the queued action of the specified ID, and pauses it.
                ctx.Session.GetInQueue("day_timer").Pause();
                ctx.Session.QueueAction(TimeSpan.FromSeconds(5), WolfVars.EndConviction);

                Logger.Debug("A conviction has started");
            }
        }

        private static void OnSkip(InputContext ctx)
        {
            // If the current phase is night, ignore input
            if (IsNight(ctx.Session))
                return;

            // Ensure that the invoker is in the current session
            if (ctx.Player == null)
                return;

            // If the current phase does not supports skipping, ignore input
            if (!CanSkipCurrentPhase(ctx.Session))
                return;

            // Mark the player with the request to skip
            ctx.Player.SetValue(WolfVars.HasRequestedSkip, true);

            // Attempt to skip the current phase
            ctx.Session.InvokeAction(WolfVars.TryEndPhase);
        }

        private static void OnPick(InputContext ctx)
        {
            if (ctx.Session.ValueOf<WerewolfPhase>(WolfVars.CurrentPhase) != WerewolfPhase.Night)
                return;

            if (!CanUseAbility(ctx.Player, ctx.Session))
                return;

            string input = ctx.Input.Args.First();
            Logger.Debug($"Parsing '{input}' for pick <input>");

            if (ctx.Session.Players.Count(x => TryParsePlayer(x, input)) != 1)
            {
                Logger.Debug("invalid players returned");
                return;
            }

            PlayerData target = ctx.Session.Players.First(x => TryParsePlayer(x, input));

            // If they cannot use the ability on themselves and it's currently pointing at themselves
            if (!CanUseAbilityOnSelf(GetCurrentAbility(ctx.Session)) && target.Source.User.Id == ctx.Invoker.Id)
                return;

            // If the ability is shared
            if (IsAbilityShared(ctx.Session, GetCurrentAbility(ctx.Session)))
            {
                // If the target also has the same ability
                if (ctx.Session.Players.Any(x => HasAbility(x, GetCurrentAbility(ctx.Session)) && target.Source.User.Id == x.Source.User.Id))
                    return;

                // If the invoker has already voted
                if (ctx.Session.ValueOf<List<WerewolfVoteData>>(WolfVars.AbilityVotes).Any(x => x.VoterIds.Contains(ctx.Invoker.Id)))
                    return;

                Logger.Debug("Adding vote to ability votes");

                var votes = ctx.Session.ValueOf<List<WerewolfVoteData>>(WolfVars.AbilityVotes);
                // Get the user's vote data
                WerewolfVoteData voteData = votes.FirstOrDefault(x => x.UserId == target.Source.User.Id);

                if (voteData == null)
                    throw new Exception("Expected vote data for target but returned null");

                voteData.VoterIds.Add(ctx.Invoker.Id);
                ctx.Session.SetValue(WolfVars.AbilityVotes, votes);
                ctx.Session.InvokeAction(WolfVars.TryEndAbility, true);
                return;
            }

            // Otherwise, if the ability is not shared

            // Use the specified ability
            UseAbility(ctx.Player, target, ctx.Session);
            ctx.Player.SetValue(WolfVars.HasUsedAbility, true);

            ServerConnection connection = ctx.Server.Connections.FirstOrDefault(x => x.UserId == ctx.Player.Source.User.Id);

            if (connection == null)
                throw new Exception("Expected player connection but returned null");

            // If shared peeking is disabled, draw individually
            connection.ContentOverride["header"].Draw(WolfFormat.WriteAbilityResult(target, ctx.Session));
            connection.ContentOverride["players"].Draw(WolfFormat.WriteAbilityList(ctx.Player, ctx.Session));
            connection.BlockInput = true;

            ctx.Session.InvokeAction(WolfVars.TryEndAbility, true);
        }

        // This is after the presumed target has been handled
        private static void UseAbility(PlayerData invoker, PlayerData target, GameSession session)
        {
            WerewolfAbility current = GetCurrentAbility(session);
            bool isShared = IsAbilityShared(session, current);

            if (!isShared && invoker == null)
                throw new Exception("Expected invoker for individual ability but returned null");

            switch (current)
            {
                case WerewolfAbility.Peek:
                    WerewolfPeekData peek = session.ValueOf<List<WerewolfPeekData>>(WolfVars.Peeks)
                        .FirstOrDefault(x => x.UserId == target.Source.User.Id);

                    if (peek == null)
                        throw new Exception("Expected peek data for target but returned null");

                    if (isShared)
                        peek.RevealedTo.AddRange(session.Players.Where(x => HasAbility(x, current)).Select(x => x.Source.User.Id));
                    else
                        peek.RevealedTo.Add(invoker.Source.User.Id);

                    return;

                case WerewolfAbility.Feast:
                    AddStatus(target, WerewolfStatus.Marked);
                    return;

                case WerewolfAbility.Protect:
                    AddStatus(target, WerewolfStatus.Protected);
                    return;

                default:
                    throw new Exception("Unable to use the selected ability");
            }
        }
        #endregion
    }
}
