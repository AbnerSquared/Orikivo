using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Orikivo;

namespace Arcadia.Games
{
    // [Action("start_day")]
    // This attribute is used to mark a method as a game action
    public class ActionAttribute : Attribute
    {
        // Name
        // UpdateOnExecute
        // 
    }

    // [Criterion("wolf_greater_equals_villager")]
    // This attribute is used to mark a method as a game criterion
    public class CriterionAttribute : Attribute
    {
        // Name
    }

    // [Input("hunt", 0)]
    // This attribute is used to mark a method as an input to a specified frequency
    public class InputAttribute : Attribute
    {
        // Name
        // Frequency
    }

    // CONFIG PROPERTIES
    // <bool> Reveal roles on death: If true, a player's role is revealed when they die
    // <bool> Use random names: If true, a player's name will be randomized, hiding their identity
    // <bool> Shared peeking: If true, all of the seers have a shared decision on who they choose. Otherwise, each seer only knows what they see
    // <WerewolfPeekMode> Reveal peeks: If true, a player's role is revealed globally 
    // <WerewolfEntryMode> Starting method: This determines how the game of werewolf starts
    // <WerewolfRoleDeny> Denied roles: This specifies all of the roles the game cannot generate
    // <WerewolfChatMode> Chat mode: This determines the chat method of this session
    // During the night time, everyone is automatically muted

    public enum WerewolfChatMode
    {
        Text = 1, // Default: the game is played in text mode, from which people can chat with each other, and so forth
        Voice = 2 // All players are required to join voice chat before starting
    }

    // This is used to deny certain roles from being selected
    public enum WerewolfRoleDeny
    {
        None = 0,
        Werewolf = 1,
        Seer = 2,
        Villager = 4,
        Hunter = 8,
        Tanner = 16,
        Tough = 32,
        Copycat = 64,
        Lycan = 128
    }

    public enum WerewolfEntryMode
    {
        Default = 1, // The default: A starting test night is initialized
        Immediate = 2 // The game immediately starts on the day phase, which prevents anyone from knowing who they are
    }

    public enum WerewolfPeekMode
    {
        Hidden = 1, // The default: All of the information a seer is given stays with them
        Player = 2, // When a seer identifies someone, the person they chose is publicly announced at the start of a day (ONLY THE PLAYER)
        Role = 3 // When a seer identifies someone, everyone will be told if they selected a werewolf or not
    }

    public enum InvokerType
    {
        // This context was invoked from a user
        User = 1,

        // This context was invoked in a session
        Session = 2
    }

    // context to use for gameAction
    // this way, it can be inherited
    public class GameContext
    {
        // By default, the context is assumed to originate from a session
        public GameContext()
        {
            Type = InvokerType.Session;
        }

        // However, this can be overridden by providing an input context
        public GameContext(InputContext ctx)
        {
            Type = InvokerType.User;
            Invoker = ctx.Player;
            Connection = ctx.Connection;
            Session = ctx.Session;
            Server = ctx.Server;
        }

        public InvokerType Type { get; set; }

        public PlayerData Invoker { get; set; }

        // The connection that this was called in, if any
        public ServerConnection Connection { get; set; }

        public GameSession Session { get; set; }

        public GameServer Server { get; set; }
    }

    public enum WerewolfPhase
    {
        Unknown = 0,
        Start, // Starting phase
        Day, // Day phase
        Trial, // Trial phase
        Death, // Death phase
        Night, // Night phase
        End // Closing phase
    }

    public enum WerewolfVote
    {
        // This vote is currently pending
        Pending = 0,

        // This votes for the player to live
        Live = 1,

        // This votes for the player to die
        Die = 2
    }

    public enum WerewolfWinState
    {
        // The game is still going
        Pending = 0,

        // The villagers win
        Villager = 1,

        // The werewolves win
        Wolf = 2
    }

    internal static class WolfVars
    {
        internal static readonly string Role = "role";

        internal static readonly string IsWinner = "is_winner";

        // if true, the player is dead and CANNOT interact with the game at all
        internal static readonly string IsDead = "is_dead";

        internal static readonly string DeathFrom = "death_from";

        // if true, the player will die at the end of the night
        internal static readonly string IsHurt = "is_hurt";

        // if a player was protected at the start of a day phase, set to false and prevent death
        internal static readonly string IsProtected = "is_protected";

        // If true, this player has already been showcased as dead
        internal static readonly string HasClosure = "has_closure";

        internal static readonly string Vote = "vote";

        // if they can inherit peek, can_peek is set to TRUE if a seer dies
        internal static readonly string InheritPeek = "inherit_peek";

        // if they are a peeker, initialize a list of user ids to track
        internal static readonly string CanPeek = "can_peek";

        // if they can feast, they vote for a player to kill alongside everyone else that can feast
        internal static readonly string CanFeast = "can_feast";

        // if they can protect, they choose a player to keep safe
        internal static readonly string CanProtect = "can_protect";

        // if not dead and injured, they will day upon the end of the next day phase.
        
        
        // if a player is marked for death at the start of a day phase AND they are not protected, kill the player
        internal static readonly string MarkedForDeath = "marked_for_death";

        // if a player has this value set AND is killed, the lover must also die
        internal static readonly string LoverId = "lover_id";

        // Dictionary<ulong, bool>
        internal static readonly string PeekedPlayerIds = "peeked_player_ids";

        // this keeps a track of all players killed, stored as WerewolfKill.
        internal static readonly string Kills = "kills";

        // GLOBAL PROPERTIES


        internal static readonly string LastPlayerKilled = "last_player_killed";
        internal static readonly string CurrentPhase = "current_phase";
        internal static readonly string NextPhase = "next_phase";
        internal static readonly string Suspect = "suspect";
        internal static readonly string RequestedSkips = "requested_skips";
        internal static readonly string ReadInputs = "read_inputs";

        // if true, this will not invoke the GetDeaths call on each round.
        internal static readonly string HasCheckedDeaths = "has_checked_deaths";

        // this keeps track of all of the nights:
        // if NightCount == 0:
        // 
        internal static readonly string TotalNights = "total_nights";

        // ACTIONs

        // This starts the game as a whole
        internal static readonly string Start = "start";

        // This is called once a criteria has been met 
        internal static readonly string GetResults = "get_results";

        internal static readonly string StartDay = "start_day";
        internal static readonly string EndDay = "end_day";
        internal static readonly string StartTrial = "start_trial";

        internal static readonly string StartNight = "start_night";

        // This is invoked for when the night ends
        internal static readonly string EndNight = "end_night";

        internal static readonly string StartVoteInput = "start_vote_input";
        internal static readonly string EndVoteInput = "end_vote_input";

        // This is called from handle_deaths OR end_vote_input

        // This reads the LastPlayerKilled property to get that specific player to:
        //    - Reveal their role
        //    - What they were killed by
        internal static readonly string OnDeath = "on_death";

        // This is called at the start of a day
        internal static readonly string HandleDeaths = "handle_deaths";


        // This is called once the night starts AND there is a seer
        internal static readonly string StartPeekInput = "start_peek_input";
        internal static readonly string EndPeekInput = "end_peek_input";

        // This is called once the night starts AND there are werewolves
        internal static readonly string StartFeastInput = "start_feast_input";
        internal static readonly string EndFeastInput = "end_feast_input";

        internal static readonly string StartProtectInput = "start_protect_input";
        internal static readonly string EndProtectInput = "end_protect_input";

        internal static readonly string StartHuntInput = "start_hunt_input";
        internal static readonly string EndHuntInput = "end_hunt_input";

        internal static readonly string TrySkipPhase = "try_skip_phase";

        internal static readonly string WolfGreaterEqualsVillager = "wolf_greater_equals_villager";
        internal static readonly string AllDeadWolf = "all_dead_wolf";
    }

    public enum WerewolfKillMethod
    {
        Unknown = 0, // We aren't sure how the player died
        Hang = 1, // The player was hung by vote
        Wolf = 2,  // The player was killed by werewolves
        Hunted = 3 // The player was killed by the hunter
    }

    public class WerewolfKill
    {
        public ulong UserId { get; set; }
        public WerewolfKillMethod Method { get; set; }

        // the time at which they were killed
        public DateTime DiedAt { get; set; }
    }

    public enum WerewolfGroup
    {
        Unknown = 0,

        Villager = 1,
        
        // If their group is tanner, the only win condition is that they are hung
        Tanner = 2,
        
        Werewolf = 3
    }

    public class WerewolfRole
    {
        public static readonly WerewolfRole Villager = new WerewolfRole();
        public static readonly WerewolfRole Wolf = new WerewolfRole();
        public static readonly WerewolfRole Seer = new WerewolfRole();

        public string Id { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }

        // if < 0, is an enemy, if 0, neutral, if > 0, is good
        public int Moral { get; set; }

        // If true, this role is a wolf-related entry.
        public bool IsWolfLike { get; set; }

        // If a seer is killed, this role will become a seer
        public bool InheritSeer { get; set; }

        // This is the group that the role is using
        public WerewolfGroup Group { get; set; }

        public WerewolfInitial Initial { get; set; }
        public WerewolfPassive Passive { get; set; } // this is the role's passive ability
        public WerewolfAbility Ability { get; set; } // this is the role's nightly ability

        // figure out role distribution based on players
    }

    // Starting abilities are only executed when the first night happens
    public enum WerewolfInitial
    {
        None = 0, // this role does not do anything at the start
        
        Copy = 1, // This role copies the role of the player they select

        Awake = 2 // This ability allows the player to look at their card AT THE END of the night
    }

    // Passive abilities are abilities that modify your actions or rules as a whole
    [Flags]
    public enum WerewolfPassive
    {
        None = 0,
        Tough = 1, // if attacked by wolves, they can live through the day, dying at the end of the night
        
        // if a hunter is about to be hanged, they can choose someone to kill
        // if a hunter was marked for death, they can choose someone to kill before they die
        Hunt = 2,
        
        // This always forces the player to vote to live
        Pacifist = 4,

        // This always forces the player to vote for death
        Militarist = 8,

        // they can see everything that is happening, being able to write 1 letter at the start of each day
        Ghost = 16 
    }

    // normal abilities are only executed at night
    [Flags]
    public enum WerewolfAbility
    {
        None = 0,
        Feast = 1, // on each night, every role with this ability chooses a player to kill
        Peek = 2, // on each night, this role can see if a player is a werewolf
        Protect = 4 // on each night, this role can choose something to protect
    }

    public class WerewolfGame : GameBuilder
    {
        // CONFIG

        // ON DAY START
        // - If the previous phase was NIGHT, check the following:
        //    - Who did the werewolves choose to kill?
        //    - Was the target protected by a bodyguard?
        //        - If they were protected, set marked_for_death to false AND is_protected to false
        //        - If they were not protected:
        //            - Set marked_for_death to false
        //            - Set is_dead to true
        //            - Set global last_player_killed to the killed user id
        //            - Invoke action on_dead
        //

        // ON DAY
        // - Start a timer for 3 minutes
        // - If anyone decides to accuse someone, check the following:
        //     - Was the person picked already picked by that same player?
        //         - If they were, ignore this
        //         - If they weren't, initiate a trial phase
        //             - Wait 5 seconds to see if anyone agrees:
        //                 - If someone agrees, invoke on_trial_start
        // ON DAY END

        // ON NIGHT START
        // - Say the simple night-time statements
        // - If someone was injured, have them die here.
        // - Get role action priority

        // ON NIGHT
        // - While everyone is sleeping, message the players that have abilities in this order:
        //    - Seer
        //        - They are given 30 seconds to pick a player to peek
        //            - If they don't pick a player, ignore their actions
        //            - If they do pick a player, check their role:
        //                - If the role is marked as IsVisibleWolf:
        //                    - Add the user id picked with the boolean TRUE
        //                - Otherwise, add the user id picked with the boolean FALSE
        //        - End their turn
        //    - Bodyguard
        //        - They are given 30 seconds to pick a player to protect, including themselves
        //            - If they do not pick anyone, ignore their actions
        //            - Otherwise, mark the selected player for death, and continue
        //    - Werewolf
        //        - Set up a channel for all werewolves
        //            - Create a voting mechanic that does the following:
        //                - If all werewolves pick a single user:
        //                    - If that user was a hunter:
        //                        - If they were chosen to be protected, simply mark the user for death
        //                        - Otherwise, let the hunter know and allow him to kill any player of his choice
        //                    - Mark that user for death and end the werewolf action phase

        // ON NIGHT END
        // - Say the simple nightly end statements

        private static List<WerewolfRole> GenerateRoles(int playerCount)
        {
            // This is where roles are generated based on the number of players
            return new List<WerewolfRole>();
        }

        public override List<PlayerData> OnBuildPlayers(List<Player> players)
        {
            // generate roles and apply to each player here
            List<WerewolfRole> roles = GenerateRoles(players.Count);
            return players.Select(x => CreatePlayer(x, Randomizer.Take(roles))).ToList();
            // PLAYER ATTRIBUTES
        }

        private static PlayerData CreatePlayer(Player player, WerewolfRole role)
        {
            return new PlayerData
            {
                Player = player,
                Properties = new List<GameProperty>
                {
                    // BASE PROPERTIES: These are base variables that are used across all players constantly
                    GameProperty.Create(WolfVars.Role, role),
                    GameProperty.Create(WolfVars.IsDead, false),
                    GameProperty.Create(WolfVars.IsWinner, true),
                    GameProperty.Create(WolfVars.HasClosure, false),
                    GameProperty.Create(WolfVars.Vote, WerewolfVote.Pending),
                    GameProperty.Create(WolfVars.IsHurt, false),
                    GameProperty.Create(WolfVars.MarkedForDeath, false),
                    GameProperty.Create(WolfVars.IsProtected, false),
                    GameProperty.Create(WolfVars.Kills, new List<WerewolfKill>()),
                    // ROLE PROPERTIES: These are variables that are used to determine a players available actions
                    GameProperty.Create(WolfVars.PeekedPlayerIds, new Dictionary<ulong, bool>())
                }
            };
        }

        private static void InheritRole(PlayerData player, WerewolfRole role)
        {
            player.SetPropertyValue(WolfVars.Role, role);
        }

        private static bool HasAbility(PlayerData player, WerewolfAbility ability)
            => player.GetPropertyValue<WerewolfRole>(WolfVars.Role).Ability.HasFlag(ability);

        // implement rulesets that can modify a player's info
        private static bool CanFeast(PlayerData player)
            => HasAbility(player, WerewolfAbility.Feast);

        private static bool CanPeek(PlayerData player)
            => HasAbility(player, WerewolfAbility.Peek);

        private static bool CanProtect(PlayerData player)
            => HasAbility(player, WerewolfAbility.Protect);

        // this gets the count of all players
        private static int GetPlayerCount(GameSession session)
            => session.Players.Count;

        private static int GetLivingCount(GameSession session)
            => session.Players.Count(IsAlive);

        // this gets the count of all werewolves left
        private static int GetWolfCount(GameSession session)
            => session.Players.Count(IsWolf);

        private static int GetVillagerCount(GameSession session)
            => session.Players.Count(IsVillager);

        private static bool IsVillager(PlayerData player)
            => player.GetPropertyValue<WerewolfRole>(WolfVars.Role).Group == WerewolfGroup.Villager;

        // if true, this player is a werewolf
        private static bool IsWolf(PlayerData player)
            => IsDeadlyWolf(player.GetPropertyValue<WerewolfRole>(WolfVars.Role));

        private static bool IsDeadlyWolf(WerewolfRole role)
            => role.IsWolfLike && role.Ability.HasFlag(WerewolfAbility.Feast);

        private static IEnumerable<PlayerData> GetLivingPlayers(GameSession session)
            => session.Players.Where(IsAlive);

        // if true, this player is alive
        private static bool IsAlive(PlayerData player)
            => !player.GetPropertyValue<bool>(WolfVars.IsDead);

        private static bool IsProtected(PlayerData player)
            => player.GetPropertyValue<bool>(WolfVars.IsProtected);

        private static bool HasAbility(PlayerData player)
            => player.GetPropertyValue<WerewolfRole>(WolfVars.Role).Ability != WerewolfAbility.None;

        private static bool CanSkipCurrentPhase(GameSession session)
            => session.GetPropertyValue<WerewolfPhase>(WolfVars.CurrentPhase).HasFlag(WerewolfPhase.Day);

        public override List<GameProperty> OnBuildProperties()
        {
            // GLOBAL PROPERTIES
            return new List<GameProperty>
            {
                GameProperty.Create<WerewolfKill>(WolfVars.LastPlayerKilled),
                GameProperty.Create(WolfVars.Suspect, 0UL),
                GameProperty.Create(WolfVars.CurrentPhase, WerewolfPhase.Unknown),
                GameProperty.Create(WolfVars.NextPhase, WerewolfPhase.Unknown),
                GameProperty.Create(WolfVars.RequestedSkips, 0),
                GameProperty.Create(WolfVars.TotalNights, 0),
                GameProperty.Create(WolfVars.ReadInputs, WerewolfAbility.None)
            };
        }

        public override List<GameAction> OnBuildActions(List<PlayerData> players)
        {
            // GAME ACTIONS
            return new List<GameAction>
            {
                // start
                new GameAction(WolfVars.Start, Start),
                new GameAction(WolfVars.GetResults, GetResults),
                new GameAction(WolfVars.StartDay, StartDay),
                new GameAction(WolfVars.EndDay, EndDay),
                new GameAction(WolfVars.StartTrial, StartTrial),
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
                new GameAction(WolfVars.TrySkipPhase, TrySkipPhase)
            };
        }

        private static int GetPendingVotes(GameSession session)
            => session.Players.Count(x => x.GetPropertyValue<WerewolfVote>(WolfVars.Vote) == WerewolfVote.Pending);

        private static int GetLiveVotes(GameSession session)
            => session.Players.Count(x => x.GetPropertyValue<WerewolfVote>(WolfVars.Vote) == WerewolfVote.Live);

        private static int GetDieVotes(GameSession session)
            => session.Players.Count(x => x.GetPropertyValue<WerewolfVote>(WolfVars.Vote) == WerewolfVote.Die);

        private static WerewolfAbility GetAbility(PlayerData player)
            => player.GetPropertyValue<WerewolfRole>(WolfVars.Role).Ability;

        private static WerewolfPassive GetPassive(PlayerData player)
            => player.GetPropertyValue<WerewolfRole>(WolfVars.Role).Passive;

        private static void OnDeath(PlayerData player, GameSession session, GameServer server)
        {
            // ON ENTRY:

            // Get the last player killed
            var kill = session.GetPropertyValue<WerewolfKill>(WolfVars.LastPlayerKilled);

            // If null, throw an exception
            if (kill == null)
                throw new Exception("Attempted to invoke a death sequence with no player killed");

            // Write to the main channel the information about who died

            // Marked the player with closure
            session.GetPlayerData(kill.UserId).SetPropertyValue(WolfVars.HasClosure, true);

            // Check winning conditions
            if (session.MeetsCriterion(WolfVars.WolfGreaterEqualsVillager))
            {
                session.InvokeAction(WolfVars.GetResults, true);
            }
            // check win conditions here, and if true, end the game
            // Check if werewolfCount == villagerCount
            // Check if werewolfCount == 0

            // otherwise, continue invoking GetDeaths
        }

        private static void HandleDeaths(PlayerData invoker, GameSession session, GameServer server)
        {
            // If there aren't any deaths, proceed through with the current phase
            session.BlockInput = true;
            // BEFORE we handle marked users, HANDLE all recently dead players
            if (session.Players.Any(HasRecentlyDied))
            {
                foreach (PlayerData player in session.Players.Where(HasRecentlyDied))
                {
                    // for each recently killed player, invoke the action on death
                    var killed = new WerewolfKill
                    {
                        UserId = player.Player.User.Id,
                        Method = WerewolfKillMethod.Hang
                    };
                }
            }
            // don't forget more than one player can die!
            if (session.Players.Any(IsMarkedForDeath))
            {
                foreach (PlayerData player in session.Players.Where(IsMarkedForDeath))
                {
                    // If the player was protected
                    if (IsProtected(player))
                    {
                        player.SetPropertyValue(WolfVars.MarkedForDeath, false);
                        player.SetPropertyValue(WolfVars.IsProtected, false);
                        // Make a notice that this player was protected
                        continue;
                    }

                    // If the player is a tough person
                    if (GetPassive(player).HasFlag(WerewolfPassive.Tough))
                    {
                        player.SetPropertyValue(WolfVars.MarkedForDeath, false);
                        player.SetPropertyValue(WolfVars.IsHurt, true);
                        // Make a notice that this player was injured
                        continue;
                    }

                    var killed = new WerewolfKill
                    {
                        Method = WerewolfKillMethod.Wolf,
                        UserId = player.Player.User.Id
                    };

                    // set the last player killed to the newest killed player
                    session.SetPropertyValue(WolfVars.LastPlayerKilled, killed);
                    session.InvokeAction(WolfVars.OnDeath, true);
                    // invoke the action OnDeath, overriding timer
                }
            }
            session.BlockInput = false;
        }

        private static void StartVoteInput(PlayerData invoker, GameSession session, GameServer server)
        {
            // ON ENTRY:
            // Start a timer for 30 seconds that invokes the action EndVoteInput
            session.QueueAction(TimeSpan.FromSeconds(30), WolfVars.EndVoteInput);
            session.BlockInput = true;

            // Automatically set the votes of each LIVING
            foreach (PlayerData data in session.Players)
            {
                // If the player is dead, continue.
                if (!IsAlive(data))
                    continue;

                if (GetPassive(data).HasFlag(WerewolfPassive.Militarist | WerewolfPassive.Pacifist))
                    throw new Exception("The specified player has a conflicting passive ability");

                if (GetPassive(data).HasFlag(WerewolfPassive.Pacifist))
                    data.SetPropertyValue(WolfVars.Vote, WerewolfVote.Live);

                if (GetPassive(data).HasFlag(WerewolfPassive.Militarist))
                    data.SetPropertyValue(WolfVars.Vote, WerewolfVote.Die);
            }

            session.BlockInput = false;
        }

        private static bool CanVote(PlayerData player)
            => !GetPassive(player).HasFlag(WerewolfPassive.Pacifist | WerewolfPassive.Hunt);

        private static void EndVoteInput(PlayerData invoker, GameSession session, GameServer server)
        {
            // Get the vote counts
            int toLive = GetLiveVotes(session);
            int toDie = GetDieVotes(session);
            int pending = GetPendingVotes(session);

            // Reset all of the votes once you've received the vote counts
            foreach (PlayerData player in session.Players.Where(CanVote))
                player.SetPropertyValue(WolfVars.Vote, WerewolfVote.Pending);

            // If the votes to die is more than the votes to live
            if (toDie > toLive + pending)
            {
                PlayerData suspect = session.GetPlayerData(session.GetPropertyValue<ulong>(WolfVars.Suspect));

                if (suspect == null)
                    throw new Exception("Expected suspect but returned null");

                var death = new WerewolfKill
                {
                    UserId = suspect.Player.User.Id,
                    DiedAt = DateTime.UtcNow,
                    Method = WerewolfKillMethod.Hang
                };

                // Kill the suspect
                suspect.SetPropertyValue(WolfVars.IsDead, true);
                session.SetPropertyValue(WolfVars.LastPlayerKilled, death);

                // Set the next phase to night
                session.SetPropertyValue(WolfVars.NextPhase, WerewolfPhase.Night);

                // Handle the suspect's death
                session.InvokeAction(WolfVars.OnDeath, true);
            }
            else
            {
                // Otherwise, go directly to the night phase
                session.InvokeAction(WolfVars.StartNight, true);
            }
        }

        private static void StartTrial(PlayerData invoker, GameSession session, GameServer server)
        {
            // Set the primary channel to the trial frequency

            // Start a timer for 30 seconds that invokes the action 'start_vote_input'
            session.QueueAction(TimeSpan.FromSeconds(30), WolfVars.StartVoteInput);

            // While this timer runs, it waits for the suspect to write their defense piece
            //     - If RandomNames is enabled, this will be handled in their direct messages
            //     - Otherwise, the suspect writes their defense in the chat

            // If the suspect does not reply, they will be ignored
            // Likewise, they can also decide to remain silent
        }

        private static void OnReply(InputContext ctx)
        {
            // Cancel the currently queued action (from 'start_trial', 30 seconds => 'start_vote_input')
            ctx.Session.CancelQueuedAction();

            // Update the primary channel to display their defense statement

            // Start a timer for 10 seconds that invokes the action 'start_vote_input'
            ctx.Session.QueueAction(TimeSpan.FromSeconds(10), WolfVars.StartVoteInput);
        }

        private static void OnSilent(InputContext ctx)
        {
            // Cancel the currently queued action (from 'start_trial', 30 seconds => 'start_vote_input')
            ctx.Session.CancelQueuedAction();

            // Invoke the action 'start_vote_input'
            ctx.Session.InvokeAction(WolfVars.StartVoteInput);
        }

        private static void StartHuntInput(PlayerData invoker, GameSession session, GameServer server)
        {
            // Start a timer for 30 seconds that invokes the action 'end_hunt_input'
            session.QueueAction(TimeSpan.FromSeconds(30), WolfVars.EndHuntInput);

        }

        private static void EndHuntInput(PlayerData invoker, GameSession session, GameServer server)
        {
            // Add the Hunt ability to the list of Read inputs, which determines what is left to check
            // Once everything has finished, block input to that channel, and continue with the night
        }

        private static void StartPeekInput(PlayerData invoker, GameSession session, GameServer server)
        {
            // Start a timer for 30 seconds that invokes the action 'end_peek_input'
            // Increase the timer for each seer in the game
            session.QueueAction(TimeSpan.FromSeconds(30), WolfVars.EndPeekInput);
            // If nobody is able to peek, return

            // Get all of the players that are able to peek

            // If shared peeking is enabled, mimic the feast input

            // Otherwise, wait for all seers to finish peeking


        }

        private static void EndPeekInput(PlayerData invoker, GameSession session, GameServer server)
        {
            // Add the Peek ability to the list of Read inputs, which determines what is left to check
            // Once everything has finished, block input to that channel, and continue with the night
        }

        private static void StartFeastInput(PlayerData invoker, GameSession session, GameServer server)
        {
            // Start a timer for 30 seconds that invokes the action 'end_feast_input'
            // Increase the timer for each feast in the game
            session.QueueAction(TimeSpan.FromSeconds(30), WolfVars.EndFeastInput);
            // If nobody is able to feast, return

            // Get all of the players that are able to feast

            // Set each player's private channel to the Feast Channel

            // Wait for every player in that channel to finish voting


        }

        private static void EndFeastInput(PlayerData invoker, GameSession session, GameServer server)
        {
            // Once they are finished voting, block input to that channel, and continue with the night
        }

        private static void StartProtectInput(PlayerData invoker, GameSession session, GameServer server)
        {
            // Start a timer for 30 seconds that invokes the action 'end_protect_input'
            // Increase the timer for each feast in the game
            session.QueueAction(TimeSpan.FromSeconds(30), WolfVars.EndProtectInput);
        }

        private static void EndProtectInput(PlayerData invoker, GameSession session, GameServer server)
        {

        }

        private static void Start(PlayerData invoker, GameSession session, GameServer server)
        {
            // If the configuration allows for the test night and day, handle it here

            // Otherwise, go directly to the day phase
            session.InvokeAction(WolfVars.StartDay);
        }

        private static void GetResults(PlayerData invoker, GameSession session, GameServer server)
        {
            foreach (PlayerData player in session.Players.Where(IsDeadTanner))
                player.SetPropertyValue(WolfVars.IsWinner, true);
        }

        private static bool IsDeadTanner(PlayerData player)
            => player.GetPropertyValue<WerewolfRole>(WolfVars.Role).Group == WerewolfGroup.Tanner && !IsAlive(player);

        private static bool IsMarkedForDeath(PlayerData player)
            => player.GetPropertyValue<bool>(WolfVars.MarkedForDeath);

        private static bool IsHurt(PlayerData player)
            => player.GetPropertyValue<bool>(WolfVars.IsHurt);

        private static bool HasRecentlyDied(PlayerData player)
            => player.GetPropertyValue<bool>(WolfVars.IsDead) && !player.GetPropertyValue<bool>(WolfVars.HasClosure);

        // ACTION start_day
        private static void StartDay(PlayerData invoker, GameSession session, GameServer server)
        {
            // Set the primary channel to the frequency used to handle the day phase

            // If there was anyone that recently died or is marked for deaths, handle those first
            if (session.Players.Any(HasRecentlyDied) || session.Players.Any(IsMarkedForDeath))
            {
                session.InvokeAction(WolfVars.HandleDeaths, true);
                return;
            }

            // Set the current phase to day
            session.SetPropertyValue(WolfVars.CurrentPhase, WerewolfPhase.Day);

            // Start a timer for 3 minutes that will invoke the action 'end_day'
            session.QueueAction(TimeSpan.FromMinutes(3), WolfVars.EndDay);
        }

        // ACTION end_day
        private static void EndDay(PlayerData invoker, GameSession session, GameServer server)
        {
            session.QueueAction(TimeSpan.FromSeconds(10), WolfVars.StartNight);
            // ON ENTRY:
            // Start a timer for 10 seconds that invokes the action StartNight
        }

        private static bool HasSuspect(GameSession session)
            => session.GetPropertyValue<ulong>(WolfVars.Suspect) != 0;

        private static void StartNight(PlayerData invoker, GameSession session, GameServer server)
        {
            // ON ENTRY:
            // If there is anyone injured, kill them.
            foreach (PlayerData player in session.Players.Where(IsHurt))
            {
                player.SetPropertyValue(WolfVars.IsHurt, false);
                player.SetPropertyValue(WolfVars.IsDead, true);
            }

            // If there is anyone with the ability
        }

        private static void EndNight(PlayerData invoker, GameSession session, GameServer server)
        {
            // Reset read inputs to none
            session.SetPropertyValue(WolfVars.ReadInputs, WerewolfAbility.None);

            // Handle all text display updates here

            // Start a timer for 10 seconds that invokes the action 'start_day'
            session.QueueAction(TimeSpan.FromSeconds(10), WolfVars.StartDay);

        }

        private static void TrySkipPhase(PlayerData invoker, GameSession session, GameServer server)
        {
            // If enough requested skips were made, skip the current phase and start the new one

            // Start the new phase based on what the current phase was
        }

        public override List<GameCriterion> OnBuildRules(List<PlayerData> players)
        {
            return new List<GameCriterion>
            {
                new GameCriterion(WolfVars.WolfGreaterEqualsVillager, WolfGreaterEqualsVillager)
            };

            // GAME RULESETS
        }

        private static bool WolfGreaterEqualsVillager(GameSession session)
            => GetWolfCount(session) >= GetVillagerCount(session);

        private static bool AllDeadWolf(GameSession session)
            => GetWolfCount(session) == 0;

        public override List<DisplayChannel> OnBuildDisplays(List<PlayerData> players)
        {
            return new List<DisplayChannel>
            {
                // Initial Channel
                // This is the channel used when starting the game

                // Main Channel
                // This is the channel used during the day and night phase
                new DisplayChannel
                {
                    Inputs = new List<IInput>
                    {
                        new TextInput("agree", OnAgree),
                        new TextInput("accuse", OnAccuse),
                        new TextInput("skip", OnSkip)
                    }
                }

                // Death Channel
                // This is the channel used when someone has died

                // Inputs:
                // - skip: If they are the host, this is automatically skipped
                //    Otherwise, increase 'requested_skips' by 1 and attempt to skip


                // These are initialized as the game proceeds
                // Private Channels: Wolf Channel, Seer Channel, Hunt Channel, Bodyguard Channel, Ghost Channel

                // Vote Channels: If the configuration 'Random names' is true, each player must vote in their own channel
                // If they already have a private channel established, store the previous frequency, and swap to the voting frequency
                // After they vote or they time out, set the previous frequency back

                // Result Channel
                // This is the channel used for when the game ends
            };
        }

        // 'agree'
        private static void OnAgree(InputContext ctx)
        {
            // Ensure that the invoker is in the current session
            if (ctx.Player == null)
                return;

            // If a suspect is not specified, ignore input
            if (!HasSuspect(ctx.Session))
                return;

            // Otherwise, start trial
            ctx.Session.InvokeAction(WolfVars.StartTrial);
        }

        // 'accuse'
        private static void OnAccuse(InputContext ctx)
        {
            // Ensure that the invoker is in the current session
            if (ctx.Player == null)
                return;

            // If a suspect is already specified, ignore input
            if (HasSuspect(ctx.Session))
                return;

            // Otherwise, read input and attempt to find the specified player
            // use StringReader class

            // If a user could be found, mark them as the suspect and return
            // Otherwise, ignore input
        }

        // 'skip'
        private static void OnSkip(InputContext ctx)
        {
            // Ensure that the invoker is in the current session
            if (ctx.Player == null)
                return;

            // If the current phase does not supports skipping, ignore input
            if (!CanSkipCurrentPhase(ctx.Session))
                return;
            
            // Otherwise, add one to the total requested skips
            ctx.Session.AddToProperty(WolfVars.RequestedSkips, 1);

            // Attempt to skip the current phase
            ctx.Session.InvokeAction(WolfVars.TrySkipPhase);
        }

        // 'peek <user>'
        private static void OnPeek(InputContext ctx)
        {

        }

        // 'feast <user>'
        private static void OnFeast(InputContext ctx)
        {

        }

        // 'protect <user>'
        private static void OnProtect(InputContext ctx)
        {

        }

        // This is for the hunter
        // 'shoot <user>'
        private static void OnShoot(InputContext ctx)
        {

        }

        public override void OnSessionStart(GameServer server, GameSession session)
        {
            // Synchronize the specified config to the game
            Config = server.Config.GameConfig;

            // Start the game
            session.InvokeAction(WolfVars.Start, true);
        }

        public override SessionResult OnSessionFinish(GameSession session)
        {
            var result = new SessionResult();

            // For each player, update stats if they won, played, killed, and voted

            return result;
        }
    }
}
