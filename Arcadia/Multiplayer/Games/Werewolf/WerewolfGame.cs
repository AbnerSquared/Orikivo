using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Orikivo;
using Format = Orikivo.Format;

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

            Config = new List<ConfigProperty>
            {
                ConfigProperty.Create("revealrolesondeath", "Reveal roles on death", false),
                ConfigProperty.Create("roledeny", "Denied roles", WerewolfRoleDeny.None)
            };
        }
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

        private List<WerewolfRole> GenerateRoles(int playerCount)
        {
            var roles = new List<WerewolfRole>();
            // In here, you have to generate the roles based on the configurations set
            var roleDeny = GetConfigProperty("roledeny");

            // In here, you have to incorporate a max allowed per session.
            for (int i = 0; i < playerCount; i++)
                roles.Add(Randomizer.ChooseAny(WerewolfRole.Villager));

            // This is where roles are generated based on the number of players
            return roles;
        }

        public override List<PlayerData> OnBuildPlayers(List<Player> players)
        {
            // generate roles and apply to each player here
            List<WerewolfRole> roles = GenerateRoles(players.Count);
            // Make sure to use Randomizer.Take() later on
            return players.Select((x, i) => CreatePlayer(x, Randomizer.Choose(roles), i)).ToList();
            // PLAYER ATTRIBUTES
        }

        private static PlayerData CreatePlayer(Player player, WerewolfRole role, int index)
        {
            Console.WriteLine("Creating player...");
            return new PlayerData
            {
                Player = player,
                Properties = new List<GameProperty>
                {
                    // BASE PROPERTIES: These are base variables that are used across all players constantly
                    GameProperty.Create(WolfVars.Index, index),
                    GameProperty.Create(WolfVars.Role, role),
                    GameProperty.Create(WolfVars.IsDead, false),
                    GameProperty.Create(WolfVars.IsWinner, true),
                    GameProperty.Create(WolfVars.HasClosure, false),
                    GameProperty.Create(WolfVars.Vote, WerewolfVote.Pending),
                    GameProperty.Create(WolfVars.IsHurt, false),
                    GameProperty.Create(WolfVars.MarkedForDeath, false),
                    GameProperty.Create(WolfVars.IsProtected, false),
                    GameProperty.Create(WolfVars.LoverId, 0UL),
                    GameProperty.Create(WolfVars.Kills, new List<WerewolfKill>()),
                    // ROLE PROPERTIES: These are variables that are used to determine a players available actions
                    GameProperty.Create(WolfVars.PeekedPlayerIds, new Dictionary<ulong, bool>()),
                    GameProperty.Create(WolfVars.DeathFrom, WerewolfKillMethod.Unknown)
                }
            };
        }

        private static string WritePeekList(PlayerData player, GameSession session)
        {
            var peeks = new StringBuilder();

            var peekedPlayers = player.GetPropertyValue<Dictionary<ulong, bool>>(WolfVars.PeekedPlayerIds);
            foreach (PlayerData data in session.Players)
            {
                if (data.Player.User.Id == player.Player.User.Id)
                    continue;

                bool? peekState = null;

                if (peekedPlayers.ContainsKey(data.Player.User.Id))
                    peekState = peekedPlayers[data.Player.User.Id];
                // If the other player is a peeker AND shared peeking is enabled,
                // ignore
                // if (IsPeeker(data))
                //    continue;
                peeks.AppendLine(WritePeek(data, peekState));
            }

            return peeks.ToString();
        }

        private static string WritePeek(PlayerData player, bool? peekState = null)
        {
            string state = peekState.HasValue ? peekState.Value ? "🐺" : "😇" : "🎭";

            return $"{state} • {player.Player.User.Username}";
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

        private static bool TryGetAbility(PlayerData player, out WerewolfAbility ability)
        {
            ability = player.GetPropertyValue<WerewolfRole>(WolfVars.Role).Ability;

            return ability != WerewolfAbility.None;
        }

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
                GameProperty.Create(WolfVars.TotalRounds, 0),
                GameProperty.Create(WolfVars.ReadInputs, WerewolfAbility.None),
                GameProperty.Create(WolfVars.HasTrial, false),
                GameProperty.Create(WolfVars.AwaitDefense, false)
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
                new GameAction(WolfVars.TrySkipPhase, TrySkipPhase),
                new GameAction(WolfVars.RemoveSuspect, RemoveSuspect),
                new GameAction(WolfVars.OnDeath, OnDeath),
                new GameAction(WolfVars.TryEndVote, TryEndVote)
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
            // Set all of the primary channels to the death frequency
            foreach (ServerConnection connection in ctx.Server.GetGroup("primary"))
                connection.Frequency = WolfChannel.Death;

            ctx.Session.BlockInput = true;
            // Get the last player killed
            var kill = ctx.Session.GetPropertyValue<WerewolfKill>(WolfVars.LastPlayerKilled);

            var player = ctx.Session.GetPlayerData(kill.UserId);

            bool hasGameEnded = ctx.Session.MeetsCriterion(WolfVars.WolfGreaterEqualsVillager) ||
                                ctx.Session.MeetsCriterion(WolfVars.AllDeadWolf);


            var extraText = hasGameEnded ? $"> **{GetLivingCount(ctx.Session):##,0}** {Format.TryPluralize("resident", GetLivingCount(ctx.Session))} remain. Tread carefully." : "";

            ctx.Server.GetDisplayChannel(WolfChannel.Death).GetComponent("header").Draw(player.Player.User.Username);
            ctx.Server.GetDisplayChannel(WolfChannel.Death).GetComponent("summary").Draw(GetDeathText(kill.Method));
            ctx.Server.GetDisplayChannel(WolfChannel.Death).GetComponent("reveal")
                .Draw(player.GetPropertyValue<WerewolfRole>(WolfVars.Role).Name, extraText);

            // If null, throw an exception
            if (kill == null)
                throw new Exception("Attempted to invoke a death sequence with no player killed");

            // Write to the main channel the information about who died

            // Marked the player with closure
            ctx.Session.GetPlayerData(kill.UserId).SetPropertyValue(WolfVars.HasClosure, true);

            // Check winning conditions
            if (ctx.Session.MeetsCriterion(WolfVars.WolfGreaterEqualsVillager))
            {
                ctx.Session.SetPropertyValue(WolfVars.WinningGroup, WerewolfGroup.Werewolf);
                ctx.Session.InvokeAction(WolfVars.GetResults);
                return;
            }

            if (ctx.Session.MeetsCriterion(WolfVars.AllDeadWolf))
            {
                ctx.Session.SetPropertyValue(WolfVars.WinningGroup, WerewolfGroup.Villager);
                ctx.Session.InvokeAction(WolfVars.GetResults);
                return;
            }

            // check win conditions here, and if true, end the game
            // Check if werewolfCount == villagerCount
            // Check if werewolfCount == 0

            // otherwise, continue invoking GetDeaths
            ctx.Session.InvokeAction(WolfVars.HandleDeaths);
        }

        private static string GetDeathText(WerewolfKillMethod method)
        {
            return method switch
            {
                WerewolfKillMethod.Hunted => "While sleeping sound, the echoes of a rifle pierced the nightly atmosphere, putting an end to their breathing.",
                WerewolfKillMethod.Wolf => "They were mauled by werewolves, leaving barely anything to identify them by.",
                WerewolfKillMethod.Hang => "They have been left to hang from the suspicion of the village.",
                _ => "They have been eliminated from an unknown source."
            };
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
                        Method = player.GetPropertyValue<WerewolfKillMethod>(WolfVars.DeathFrom)
                    };

                    ctx.Session.SetPropertyValue(WolfVars.LastPlayerKilled, killed);
                    ctx.Session.InvokeAction(WolfVars.OnDeath, true);
                    return;
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
                        ctx.Server.GetDisplayChannel(WolfChannel.Main).Content.GetGroup("console").Append($"{player.Player.User.Username} was protected from the dangers that lurked last night.");
                        ctx.Server.GetDisplayChannel(WolfChannel.Main).Content.GetComponent("console").Draw();
                        // Make a notice that this player was protected
                        continue;
                    }

                    // If the player is a tough person
                    if (GetPassive(player).HasFlag(WerewolfPassive.Tough))
                    {
                        player.SetPropertyValue(WolfVars.MarkedForDeath, false);
                        player.SetPropertyValue(WolfVars.IsHurt, true);
                        ctx.Server.GetDisplayChannel(WolfChannel.Main).Content.GetGroup("console").Append($"{player.Player.User.Username} has been injured, but lives to tell the tale.");
                        ctx.Server.GetDisplayChannel(WolfChannel.Main).Content.GetComponent("console").Draw();
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
                    return;
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
            ctx.Session.SetPropertyValue(WolfVars.AwaitDefense, false);
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
                else if (GetPassive(data).HasFlag(WerewolfPassive.Militarist))
                    data.SetPropertyValue(WolfVars.Vote, WerewolfVote.Die);
            }
            ctx.Session.InvokeAction(WolfVars.TryEndVote);
            ctx.Session.BlockInput = false;
        }

        private static bool CanVote(PlayerData player)
            => !GetPassive(player).HasFlag(WerewolfPassive.Pacifist | WerewolfPassive.Militarist);

        private static void TryEndVote(GameContext ctx)
        {
            // If all of the votes are secured, end the vote
            if (ctx.Session.Players.All(x => x.GetPropertyValue<WerewolfVote>(WolfVars.Vote) != WerewolfVote.Pending))
            {
                // Cancel the current voting timer
                ctx.Session.CancelQueuedAction();
                
                // End the current vote
                ctx.Session.InvokeAction(WolfVars.EndVoteInput);
            }
        }

        private static void EndVoteInput(GameContext ctx)
        {
            // Get the vote counts
            int toLive = GetLiveVotes(ctx.Session);
            int toDie = GetDieVotes(ctx.Session);
            int pending = GetPendingVotes(ctx.Session);

            // Reset all of the votes once you've received the vote counts
            foreach (PlayerData player in ctx.Session.Players.Where(CanVote))
                player.SetPropertyValue(WolfVars.Vote, WerewolfVote.Pending);

            ctx.Session.BlockInput = true;
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
                ctx.Session.BlockInput = false;
                ctx.Session.SetPropertyValue(WolfVars.HasTrial, false);
                ctx.Session.InvokeAction(WolfVars.OnDeath, true);
            }
            else
            {
                // Otherwise, go directly to the night phase
                ctx.Session.BlockInput = false;
                ctx.Session.SetPropertyValue(WolfVars.HasTrial, false);
                ctx.Session.InvokeAction(WolfVars.StartNight, true);
            }
        }

        private static void StartTrial(GameContext ctx)
        {
            // Set the primary channel to the trial frequency

            // Start a timer for 30 seconds that invokes the action 'start_vote_input'
            ctx.Session.QueueAction(TimeSpan.FromSeconds(30), WolfVars.StartVoteInput);
            ctx.Session.SetPropertyValue(WolfVars.AwaitDefense, true);

            // While this timer runs, it waits for the suspect to write their defense piece
            //     - If RandomNames is enabled, this will be handled in their direct messages
            //     - Otherwise, the suspect writes their defense in the chat

            // If the suspect does not reply, they will be ignored
            // Likewise, they can also decide to remain silent
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
            ctx.Server.GetDisplayChannel(WolfChannel.Peek).GetComponent("header").Draw(GetPeekHeader());
            foreach (PlayerData player in ctx.Session.Players.Where(IsPeeker))
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

        private static bool IsPeeker(PlayerData player)
            => player.GetPropertyValue<WerewolfRole>(WolfVars.Role).Ability.HasFlag(WerewolfAbility.Peek);

        private static bool IsFeaster(PlayerData player)
            => player.GetPropertyValue<WerewolfRole>(WolfVars.Role).Ability.HasFlag(WerewolfAbility.Feast);

        private static void EndPeekInput(GameContext ctx)
        {
            foreach (PlayerData player in ctx.Session.Players.Where(IsPeeker))
            {
                // The channel ID saved will be the player's own ID.
                ServerConnection connection = ctx.Server.Connections.FirstOrDefault(x => x.ChannelId == player.Player.User.Id);
                connection.BlockInput = true;
            }

            ctx.Session.SetPropertyValue(WolfVars.ReadInputs, ctx.Session.GetPropertyValue<WerewolfAbility>(WolfVars.ReadInputs) | WerewolfAbility.Peek);
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
            foreach (PlayerData player in ctx.Session.Players.Where(IsFeaster))
            {
                // The channel ID saved will be the player's own ID.
                ServerConnection connection = ctx.Server.Connections.FirstOrDefault(x => x.ChannelId == player.Player.User.Id);
                connection.BlockInput = true;
            }

            ctx.Session.SetPropertyValue(WolfVars.ReadInputs, ctx.Session.GetPropertyValue<WerewolfAbility>(WolfVars.ReadInputs) | WerewolfAbility.Feast);
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
            foreach (ServerConnection connection in ctx.Server.GetGroup("primary"))
                connection.Frequency = WolfChannel.Main;

            // Otherwise, go directly to the day phase
            ctx.Session.InvokeAction(WolfVars.StartDay);
        }

        private static void GetResults(GameContext ctx)
        {
            var group = ctx.Session.GetPropertyValue<WerewolfGroup>(WolfVars.WinningGroup);

            foreach (PlayerData player in ctx.Session.Players.Where(IsDeadTanner))
                player.SetPropertyValue(WolfVars.IsWinner, true);

            foreach (PlayerData player in ctx.Session.Players.Where(x => group.HasFlag(x.GetPropertyValue<WerewolfRole>(WolfVars.Role).Group)))
                player.SetPropertyValue(WolfVars.IsWinner, true);

            ctx.Server.GetDisplayChannel(WolfChannel.Results).GetComponent("header").Draw(group, ctx.Session.GetPropertyValue(WolfVars.TotalRounds));
            ctx.Server.GetDisplayChannel(WolfChannel.Results).GetComponent("summary").Draw(GetRandomSummaryForGroup(group));
            ctx.Server.GetDisplayChannel(WolfChannel.Results).GetComponent("facts").Draw(GetRandomFacts(ctx.Session));

            ctx.Session.QueueAction(TimeSpan.FromSeconds(15), "end");
        }

        private static string GetAccuseText(PlayerData accuser, PlayerData suspect)
        {
            // Instead of referencing usernames, store it in a property value,
            // just in case Randomize names is enabled.
            return $"{accuser.Player.User.Username} has accused {suspect.Player.User.Username} of being a werewolf. Does anyone else agree?";
        }

        private static object[] GetRandomFacts(GameSession session)
        {
            var strings = new List<string>();

            for (int i = 0; i < 3; i++)
            {
                strings.Add(GetRandomFact(session));
            }

            return strings.ToArray();
        }

        private static string GetRandomFact(GameSession session)
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

        private static string GetRandomSummaryForGroup(WerewolfGroup group)
        {
            return group switch
            {
                WerewolfGroup.Villager => "The villagers were able to snuff out all of the werewolves, cleansing their village once and for all.",
                WerewolfGroup.Werewolf => "After a long struggle, the werewolves were able to overthrow the village, providing a feast to last months.",
                _ => throw new Exception("Invalid winning group specified.")
            };
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
            ctx.Session.SetPropertyValue(WolfVars.CurrentPhase, WerewolfPhase.Day);
            // Update the list of players
            UpdatePlayerList(ctx.Session, ctx.Server);
            SetMainHeader(ctx.Server, ctx.Session);

            // If there was anyone that recently died or is marked for deaths, handle those first
            if (ctx.Session.Players.Any(HasRecentlyDied) || ctx.Session.Players.Any(IsMarkedForDeath))
            {
                ctx.Session.InvokeAction(WolfVars.HandleDeaths, true);
                return;
            }

            // Set the current phase to day
            ctx.Session.SetPropertyValue(WolfVars.CurrentPhase, WerewolfPhase.Day);

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
            => session.GetPropertyValue<ulong>(WolfVars.Suspect) != 0;

        private static void StartNight(GameContext ctx)
        {
            // Set the current phase to night
            ctx.Session.SetPropertyValue(WolfVars.CurrentPhase, WerewolfPhase.Night);

            // If there is anyone injured, kill them.
            foreach (PlayerData player in ctx.Session.Players.Where(IsHurt))
            {
                player.SetPropertyValue(WolfVars.IsHurt, false);
                player.SetPropertyValue(WolfVars.IsDead, true);
            }

            // Handle all abilities
        }

        private static bool HasPrivateChannel(PlayerData player, GameServer server)
            => server.Connections.Any(x => x.ChannelId == player.Player.User.Id);

        private static void HandleAbilities(GameContext ctx)
        {
            var readInputs = ctx.Session.GetPropertyValue<WerewolfAbility>(WolfVars.ReadInputs);

            // If anyone can peek, do it now
            if (!readInputs.HasFlag(WerewolfAbility.Peek))
            {
                if (ctx.Session.Players.Any(IsPeeker))
                    ctx.Session.InvokeAction(WolfVars.StartPeekInput);
            }

            // If anyone can feast
            if (!readInputs.HasFlag(WerewolfAbility.Feast))
            {
                if (ctx.Session.Players.Any(IsFeaster))
                    ctx.Session.InvokeAction(WolfVars.StartFeastInput);
            }

            ctx.Session.InvokeAction(WolfVars.EndNight);
        }

        private static void EndNight(GameContext ctx)
        {
            // Reset read inputs to none
            ctx.Session.SetPropertyValue(WolfVars.ReadInputs, WerewolfAbility.None);

            // Handle all text display updates here
            ctx.Server.GetDisplayChannel(WolfChannel.Main).Content.GetGroup("console").Append("The night has ended.");

            // Add 1 to the total rounds completed counter.
            ctx.Session.AddToProperty(WolfVars.TotalRounds, 1);

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
                new GameCriterion(WolfVars.WolfGreaterEqualsVillager, WolfGreaterEqualsVillager),
                new GameCriterion(WolfVars.AllDeadWolf, AllDeadWolf)
            };
        }

        private static bool WolfGreaterEqualsVillager(GameSession session)
            => GetWolfCount(session) >= GetVillagerCount(session);

        private static bool AllDeadWolf(GameSession session)
            => GetWolfCount(session) == 0;

        private static bool IsOnTrial(GameSession session)
            => session.GetPropertyValue<bool>(WolfVars.HasTrial);

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

            // This gets the queued action of the specified ID, and pauses it.
            ctx.Session.GetQueuedAction("day_timer").Pause();
            ctx.Session.SetPropertyValue(WolfVars.HasTrial, true);
            // Otherwise, start trial
            ctx.Session.InvokeAction(WolfVars.StartTrial);
        }

        private static string WriteFeastPartners(PlayerData player, GameSession session)
        {
            // Filter out non-feaster and self
            var partners = session.Players.Where(IsFeaster).Where(x => x.Player.User.Id != player.Player.User.Id);

            if (partners.Count() == 0)
                return "> You are a lone wolf.";

            var feastInfo = new StringBuilder();

            feastInfo.AppendLine("> These are your partners:");

            foreach (PlayerData partner in partners)
                feastInfo.AppendLine($"• {partner.Player.User.Username}");

            return feastInfo.ToString();
        }

        private static bool IsDefending(GameSession session)
            => session.GetPropertyValue<bool>(WolfVars.AwaitDefense);
        
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
            if (ctx.Session.GetPropertyValue<ulong>(WolfVars.Suspect) != ctx.Player.Player.User.Id)
                return;

            // Otherwise, skip the initial command input and write what they said:
            string statement = ctx.Input.Source.Substring(3);

            // If their statement is empty, ignore input
            if (!Check.NotNull(statement))
                return;

            ctx.Server.GetDisplayChannel(WolfChannel.Main).Content.GetGroup("console").Append($"The suspect has said in their defense: {statement}");
            ctx.Session.InvokeAction(WolfVars.StartVoteInput);
        }

        private static bool IsNight(GameSession session)
            => session.GetPropertyValue<WerewolfPhase>(WolfVars.CurrentPhase) == WerewolfPhase.Night;

        // 'live'
        private static void OnLive(InputContext ctx)
        {
            // If there isn't a current trial active, ignore it
            if (!IsOnTrial(ctx.Session))
                return;

            // If their vote is not pending, ignore their input
            if (ctx.Player.GetPropertyValue<WerewolfVote>(WolfVars.Vote) != WerewolfVote.Pending)
                return;

            // Otherwise, set their vote to live
            ctx.Player.SetPropertyValue(WolfVars.Vote, WerewolfVote.Live);
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

            IEnumerable<PlayerData> players = ctx.Session.Players.Where(x => TryGetPlayer(x, user));

            if (players.Count() == 1)
            {
                ctx.Session.SetPropertyValue(WolfVars.Suspect, players.First().Player.User.Id);
                ctx.Server.GetDisplayChannel(WolfChannel.Main).Content.GetGroup("console").Append(GetAccuseText(ctx.Player, players.First()));
                ctx.Server.GetDisplayChannel(WolfChannel.Main).Content.GetComponent("console").Draw();

                // This gets the queued action of the specified ID, and pauses it.
                ctx.Session.GetQueuedAction("day_timer").Pause();
                ctx.Session.QueueAction(TimeSpan.FromSeconds(5), WolfVars.RemoveSuspect);
            }

            // If a user could be found, mark them as the suspect and return
            // Otherwise, ignore input
        }

        private static void RemoveSuspect(GameContext ctx)
        {
            ctx.Session.SetPropertyValue(WolfVars.Suspect, 0UL);
            ctx.Server.GetDisplayChannel(WolfChannel.Main).Content.GetGroup("console").Append("The accusation has subsided.");
            ctx.Server.GetDisplayChannel(WolfChannel.Main).GetComponent("console").Draw();
            // This gets the queued action of the specified ID, and resumes it.
            ctx.Session.GetQueuedAction("day_timer").Resume();
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

            // If the current phase does not supports skipping, ignore input
            if (!CanSkipCurrentPhase(ctx.Session))
                return;
            
            // Otherwise, add one to the total requested skips
            ctx.Session.AddToProperty(WolfVars.RequestedSkips, 1);

            // Attempt to skip the current phase
            ctx.Session.InvokeAction(WolfVars.TrySkipPhase);
        }

        private static string GetPeekHeader()
            => "🔍 Choose a player to inspect:";

        // 'peek <user>'
        private static void OnPeek(InputContext ctx)
        {
            string user = ctx.Input.Args.First();

            var validPlayers = ctx.Session.Players.Where(x => TryGetPlayer(x, user));
            
            if (validPlayers.Count() > 1 || !validPlayers.Any())
            {
                Console.WriteLine("invalid players returned");
                return;
            }

            PlayerData player = validPlayers.First();

            var peeks = ctx.Player.GetPropertyValue<Dictionary<ulong, bool>>(WolfVars.PeekedPlayerIds);

            if (peeks.ContainsKey(player.Player.User.Id))
            {
                Console.WriteLine("Player has already been peeked. Ignoring input...");
                return;
            }

            peeks.Add(player.Player.User.Id, player.GetPropertyValue<WerewolfRole>(WolfVars.Role).IsWolfLike);
            ctx.Server.GetDisplayChannel(WolfChannel.Peek).GetComponent("header").Draw(GetPeekResult(player));
        }

        private static void SetMainHeader(GameServer server, GameSession session)
        {
            var main = server.GetDisplayChannel(WolfChannel.Main);

            int totalRounds = session.GetPropertyValue<int>(WolfVars.TotalRounds);
            WerewolfPhase currentPhase = session.GetPropertyValue<WerewolfPhase>(WolfVars.CurrentPhase);

            string phaseInfo = WritePhaseHeader(currentPhase);

            main.Content.GetComponent("header").Draw(totalRounds.ToString("##,0"), phaseInfo);
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

        private static string GetPeekResult(PlayerData player)
        {
            if (player.GetPropertyValue<WerewolfRole>(WolfVars.Role).IsWolfLike)
                return $"**{player.Player.User.Username}** is a werewolf.";

            return $"**{player.Player.User.Username}** is innocent.";
        }

        private static bool TryGetPlayer(PlayerData player, string input)
        {
            bool isId = ulong.TryParse(input, out ulong id);

            if (isId)
                return id == player.Player.User.Id;

            return player.Player.User.Username.Equals(input, StringComparison.OrdinalIgnoreCase);
        }

        // 'feast <user>'
        private static void OnFeast(InputContext ctx)
        {

        }

        // 'protect <user>'
        private static void OnProtect(InputContext ctx)
        {

        }

        // 'shoot <user>'
        private static void OnShoot(InputContext ctx)
        {

        }

        public override async Task OnSessionStartAsync(GameServer server, GameSession session)
        {
            // Set all of the currently connected channels to the specified frequency
            server.SetFrequencyForState(GameState.Playing, WolfChannel.Main);
            server.GroupAll("primary");

            var main = server.GetDisplayChannel(WolfChannel.Main).Content;

            foreach (PlayerData player in session.Players)
            {
                Console.WriteLine("Loading player...");
                // Set this stuff up AFTER you point all of the connections to the right frequency
                if (TryGetAbility(player, out WerewolfAbility ability))
                {
                    // This initializes all of the displays to be set up.
                    DisplayChannel display = server.GetDisplayChannel(GetFrequencyFor(ability));
                    ServerConnection connection = await player.Player.GetConnectionAsync(display);
                    connection.Group = "secondary";
                }

                int index = player.GetPropertyValue<int>(WolfVars.Index);
                // On the main channel, initialize all of the player slots
                main.GetGroup("players").Set(index, GetPlayerInfo(player, session));
            }

            // Synchronize the specified config to the game
            Config = server.Config.GameConfig;
            main.GetComponent("players").Draw();
            // Start the game
            session.InvokeAction(WolfVars.Start, true);
        }

        private static void UpdatePlayerList(GameSession session, GameServer server)
        {
            foreach (PlayerData player in session.Players)
            {
                // On the main channel, initialize all of the player slots
                server.GetDisplayChannel(WolfChannel.Main)
                    .Content.GetGroup("players")
                    .Set(player.GetPropertyValue<int>(WolfVars.Index),
                        GetPlayerInfo(player, session));
            }

            server.GetDisplayChannel(WolfChannel.Main).Content.GetComponent("players").Draw();
        }

        private int GetFrequencyFor(WerewolfAbility ability)
        {
            return ability switch
            {
                WerewolfAbility.Peek => WolfChannel.Peek,
                WerewolfAbility.Feast => WolfChannel.Feast,
                WerewolfAbility.Hunt => WolfChannel.Hunt,
                // WerewolfAbility.Protect => WolfChannel.Protect,
                _ => throw new Exception("The specified ability does not have a dedicated frequency")
            };
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
                                    // 1: Phase icon
                                    // 2: Phase name
                                    // 3: Phase extra details (can be left empty)
                                    BaseFormatter = "> **Round {0}**\n> {1}",
                                    OverrideBaseValue = true
                                }
                            },

                            // main/console
                            new ComponentGroup
                            {
                                Active = true,
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
                                Values = new string[6] { "", "", "", "", "", "" }
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
                        new TextInput("skip", OnSkip)
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
                                Capacity = 3,
                                Active = true,
                                Position = 2,
                                Formatter = new ComponentFormatter
                                {
                                    BaseFormatter = "{0}",
                                    ElementFormatter = "• {0}",
                                    Separator = "\n",
                                    OverrideBaseValue = true
                                }
                            }
                        }
                    }
                }
            };
        }

        private static string GetPlayerInfo(PlayerData player, GameSession session)
        {
            var info = new StringBuilder();

            // Get public expression icon
            info.Append(GetExpression(player, session));

            info.Append($" • ");

            if (!IsAlive(player))
                info.Append($"~~*{player.Player.User.Username}*~~ (Dead)");
            else
            {
                info.Append($"**{player.Player.User.Username}**#{player.GetPropertyValue<int>(WolfVars.Index)}");

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

        private static bool IsSuspect(PlayerData player, GameSession session)
            => session.GetPropertyValue<ulong>(WolfVars.Suspect) == player.Player.User.Id;

        private static string GetExpression(PlayerData player, GameSession session)
        {
            if (IsSuspect(player, session))
                return "😟";

            if (IsHurt(player))
                return "🤕";

            return "😐";
        }
    }
}
