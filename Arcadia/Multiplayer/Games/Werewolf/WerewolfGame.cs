using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Orikivo;

namespace Arcadia.Multiplayer.Games
{
    

    // [Criterion("wolf_greater_equals_villager")]
    // This attribute is used to mark a method as a game criterion

    // [Input("hunt", 0)]
    // This attribute is used to mark a method as an input to a specified frequency

    // CONFIG PROPERTIES
    // <bool> Reveal roles on death: If true, a player's role is revealed when they die
    // <bool> Use random names: If true, a player's name will be randomized, hiding their identity
    // <bool> Shared peeking: If true, all of the seers have a shared decision on who they choose. Otherwise, each seer only knows what they see
    // <WerewolfPeekMode> Reveal peeks: If true, a player's role is revealed globally 
    // <WerewolfEntryMode> Starting method: This determines how the game of werewolf starts
    // <WerewolfRoleDeny> Denied roles: This specifies all of the roles the game cannot generate
    // <WerewolfChatMode> Chat mode: This determines the chat method of this session
    // During the night time, everyone is automatically muted

    // This is used to deny certain roles from being selected

    // context to use for gameAction
    // this way, it can be inherited

    // Starting abilities are only executed when the first night happens

    // Passive abilities are abilities that modify your actions or rules as a whole

    // normal abilities are only executed at night

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

        private static void OnDeath(GameContext ctx)
        {
            // ON ENTRY:

            // Get the last player killed
            var kill = ctx.Session.GetPropertyValue<WerewolfKill>(WolfVars.LastPlayerKilled);

            // If null, throw an exception
            if (kill == null)
                throw new Exception("Attempted to invoke a death sequence with no player killed");

            // Write to the main channel the information about who died

            // Marked the player with closure
            ctx.Session.GetPlayerData(kill.UserId).SetPropertyValue(WolfVars.HasClosure, true);

            // Check winning conditions
            if (ctx.Session.MeetsCriterion(WolfVars.WolfGreaterEqualsVillager))
            {
                ctx.Session.InvokeAction(WolfVars.GetResults, true);
            }
            // check win conditions here, and if true, end the game
            // Check if werewolfCount == villagerCount
            // Check if werewolfCount == 0

            // otherwise, continue invoking GetDeaths
        }

        private static void HandleDeaths(GameContext ctx)
        {
            // If there aren't any deaths, proceed through with the current phase
            ctx.Session.BlockInput = true;
            // BEFORE we handle marked users, HANDLE all recently dead players
            if (ctx.Session.Players.Any(HasRecentlyDied))
            {
                foreach (PlayerData player in ctx.Session.Players.Where(HasRecentlyDied))
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
            if (ctx.Session.Players.Any(IsMarkedForDeath))
            {
                foreach (PlayerData player in ctx.Session.Players.Where(IsMarkedForDeath))
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
                    ctx.Session.SetPropertyValue(WolfVars.LastPlayerKilled, killed);
                    ctx.Session.InvokeAction(WolfVars.OnDeath, true);
                    // invoke the action OnDeath, overriding timer
                }
            }
            ctx.Session.BlockInput = false;
        }

        private static void StartVoteInput(GameContext ctx)
        {
            // ON ENTRY:
            // Start a timer for 30 seconds that invokes the action EndVoteInput
            ctx.Session.QueueAction(TimeSpan.FromSeconds(30), WolfVars.EndVoteInput);
            ctx.Session.BlockInput = true;

            // Automatically set the votes of each LIVING
            foreach (PlayerData data in ctx.Session.Players)
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

            ctx.Session.BlockInput = false;
        }

        private static bool CanVote(PlayerData player)
            => !GetPassive(player).HasFlag(WerewolfPassive.Pacifist | WerewolfPassive.Hunt);

        private static void EndVoteInput(GameContext ctx)
        {
            // Get the vote counts
            int toLive = GetLiveVotes(ctx.Session);
            int toDie = GetDieVotes(ctx.Session);
            int pending = GetPendingVotes(ctx.Session);

            // Reset all of the votes once you've received the vote counts
            foreach (PlayerData player in ctx.Session.Players.Where(CanVote))
                player.SetPropertyValue(WolfVars.Vote, WerewolfVote.Pending);

            // If the votes to die is more than the votes to live
            if (toDie > toLive + pending)
            {
                PlayerData suspect = ctx.Session.GetPlayerData(ctx.Session.GetPropertyValue<ulong>(WolfVars.Suspect));

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
                ctx.Session.SetPropertyValue(WolfVars.LastPlayerKilled, death);

                // Set the next phase to night
                ctx.Session.SetPropertyValue(WolfVars.NextPhase, WerewolfPhase.Night);

                // Handle the suspect's death
                ctx.Session.InvokeAction(WolfVars.OnDeath, true);
            }
            else
            {
                // Otherwise, go directly to the night phase
                ctx.Session.InvokeAction(WolfVars.StartNight, true);
            }
        }

        private static void StartTrial(GameContext ctx)
        {
            // Set the primary channel to the trial frequency

            // Start a timer for 30 seconds that invokes the action 'start_vote_input'
            ctx.Session.QueueAction(TimeSpan.FromSeconds(30), WolfVars.StartVoteInput);

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

        private static void StartHuntInput(GameContext ctx)
        {
            // Start a timer for 30 seconds that invokes the action 'end_hunt_input'
            ctx.Session.QueueAction(TimeSpan.FromSeconds(30), WolfVars.EndHuntInput);

        }

        private static void EndHuntInput(GameContext ctx)
        {
            // Add the Hunt ability to the list of Read inputs, which determines what is left to check
            // Once everything has finished, block input to that channel, and continue with the night
        }

        private static void StartPeekInput(GameContext ctx)
        {
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
            // Add the Peek ability to the list of Read inputs, which determines what is left to check
            // Once everything has finished, block input to that channel, and continue with the night
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
            // If the configuration allows for the test night and day, handle it here

            // Otherwise, go directly to the day phase
            ctx.Session.InvokeAction(WolfVars.StartDay);
        }

        private static void GetResults(GameContext ctx)
        {
            foreach (PlayerData player in ctx.Session.Players.Where(IsDeadTanner))
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
        private static void StartDay(GameContext ctx)
        {
            // Set the primary channel to the frequency used to handle the day phase

            // If there was anyone that recently died or is marked for deaths, handle those first
            if (ctx.Session.Players.Any(HasRecentlyDied) || ctx.Session.Players.Any(IsMarkedForDeath))
            {
                ctx.Session.InvokeAction(WolfVars.HandleDeaths, true);
                return;
            }

            // Set the current phase to day
            ctx.Session.SetPropertyValue(WolfVars.CurrentPhase, WerewolfPhase.Day);

            // Start a timer for 3 minutes that will invoke the action 'end_day'
            ctx.Session.QueueAction(TimeSpan.FromMinutes(3), WolfVars.EndDay);
        }

        // ACTION end_day
        private static void EndDay(GameContext ctx)
        {
            ctx.Session.QueueAction(TimeSpan.FromSeconds(10), WolfVars.StartNight);
            // ON ENTRY:
            // Start a timer for 10 seconds that invokes the action StartNight
        }

        private static bool HasSuspect(GameSession session)
            => session.GetPropertyValue<ulong>(WolfVars.Suspect) != 0;

        private static void StartNight(GameContext ctx)
        {
            // ON ENTRY:
            // If there is anyone injured, kill them.
            foreach (PlayerData player in ctx.Session.Players.Where(IsHurt))
            {
                player.SetPropertyValue(WolfVars.IsHurt, false);
                player.SetPropertyValue(WolfVars.IsDead, true);
            }

            // If there is anyone with the ability
        }

        private static void EndNight(GameContext ctx)
        {
            // Reset read inputs to none
            ctx.Session.SetPropertyValue(WolfVars.ReadInputs, WerewolfAbility.None);

            // Handle all text display updates here

            // Start a timer for 10 seconds that invokes the action 'start_day'
            ctx.Session.QueueAction(TimeSpan.FromSeconds(10), WolfVars.StartDay);

        }

        private static void TrySkipPhase(GameContext ctx)
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
