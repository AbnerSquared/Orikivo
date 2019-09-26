using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Newtonsoft.Json;
using Orikivo.Providers;
using Orikivo.Systems.Presets;
using Orikivo.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    // Make sure that spectators don't know who anyone is, until the game ends.
    // Make sure that spectators can't spectate a game happening in their own guilds.

    // Remember rate limits
    /*
        5 Messages every 5 seconds, make each edit 1.5s at minimum.
        1 reaction every second.
        5 messages every second. This can be averted by toggling a slow mode.
        5 presence updates every minute.
    */

    /// <summary>
    /// A basis for games on Orikivo.
    /// </summary>
    public interface IGame : IEquatable<IGame>
    {
        string Name { get; }
        string ChannelName { get; }
        string Summary { get; }
        Range PlayerLimit { get; }
        List<ulong> Group { get; set; }
        bool TryConstruct(out IGameConstructor constructor);
        IGameConstructor Construct();
        IGameEmbedder Embedder { get; }
    }

    public interface IGameEmbedder
    {
        EmbedBuilder DefaultPanel();
    }

    /// <summary>
    /// Represents a constructor for an Orikivo.Game.
    /// </summary>
    public interface IGameConstructor
    {
        void Assign(ulong id);
    }

    /// <summary>
    /// Represents a base for game players.
    /// </summary>
    public interface IGamePlayer : IEquatable<IGamePlayer>
    {
        ulong Id { get; set; }
    }

    /// <summary>
    /// Represents a base for game information.
    /// </summary>
    public interface IGameBase
    {
        string GameId { get; }
        List<IGamePlayer> Players { get; set; }
    }

    /// <summary>
    /// Represents as a handler for starting games.
    /// </summary>
    public class GameSession
    {
        public static TextChannelProperties GameChannelProperties = new TextChannelProperties { SlowModeInterval = 5 };
        public GameSession() { }

        public GameSession(ulong channelId, ulong messageId, EmbedBuilder panel)
        {
            ChannelId = channelId;
            MessageId = messageId;
            Panel = panel;
        }

        public GameSession(ulong channelId, RestUserMessage message, EmbedBuilder panel)
        {
            ChannelId = channelId;
            Message = message;
            MessageId = message.Id;
            Panel = panel;
        }

        public void Close(SocketGuild g)
        {
            if (g.TryGetTextChannel(ChannelId, out SocketTextChannel c))
            {
                c.DeleteAsync();
            }
        }

        public static GameSession Build(SocketGuild g, Server s, IGame game)
        {
            EmbedBuilder panel = game.Embedder.DefaultPanel();
            RestTextChannel channel = g.CreateTextChannelAsync(game.ChannelName, x => { x = GameChannelProperties; }).Result;
            RestUserMessage msg = channel.SendMessageAsync(embed: panel.Build()).Result;
            GameSession session = new GameSession(channel.Id, msg, panel);
            if (!s.OpenGameSessions.Exists())
            {
                s.OpenGameSessions = new List<GameSession>();
            }
            s.OpenGameSessions.Add(session);
            return session;
        }

        public async void Refresh(SocketGuild g, string title, string description, string footerText)
        {
            EmbedFooterBuilder f = new EmbedFooterBuilder();
            f.WithText(footerText);

            Panel.WithTitle(title);
            Panel.WithDescription(description);
            Panel.WithFooter(f);

            if (!Message.Exists())
            {
                if (g.TryGetTextChannel(ChannelId, out SocketTextChannel c))
                {
                    Message = c.SendMessageAsync(embed: Panel.Build()).Result;
                }
            }
            else
            {
                await Message.ModifyAsync(x => { x.Embed = Panel.Build(); });
            }
        }

        /// <summary>
        /// The location of where the game is being set-up.
        /// </summary>
        public ulong ChannelId { get; set; }

        public ulong MessageId { get; set; }

        public EmbedBuilder Panel { get; set; }

        /// <summary>
        /// The game that is being played.
        /// </summary>
        public IGame Game { get; set; }

        [JsonIgnore]
        public RestUserMessage Message { get; set; }
    }

    /// <summary>
    /// Represents a spectator in a game.
    /// </summary>
    public class GameSpectator
    {
        public string Name { get; set; }
        public ulong Id { get; set; }
    }

    /// <summary>
    /// A class representing base GameEmbed models for Werewolf.
    /// </summary>
    public class WerewolfEmbedder : IGameEmbedder
    {
        public EmbedBuilder DefaultPanel()
        {
            return EmbedData.DefaultEmbed.WithTitle("Werewolf Embed Test").WithDescription("Not complete.".MarkdownBold());
        }
        public static string GetFact()
        {
            List<string> facts = new List<string>
            {
                "Werewolves are a dangerous, yet shady bunch. Keep an eye out for unneeded remarks.",
                "As a Tanner, it is best to keep your wish for death concealed.",
                "The man who gets killed every first night is actually named Kent. Quite a good fellow, he is.",
                "This game was inspired from a take on Ultimate Werewolf. Some rules were altered to better fit Discord, however.",
                "Equinox is the term for the balance point in a game. The closer to 0, the better.",
                "Your raw power is measured by your Point Value. The larger the absolute number, the stronger you are against the opposite group."
            };

            return facts[RandomProvider.Instance.Next(1, facts.Count) - 1];
        }
        public static string GetRandomDeathCause()
        {
            List<string> facts = new List<string>
            {
                "The werewolves have proven to be a threat once more, mauling this poor morsel of a human.",
                "The suspicion of fear overpowered, and their life was cut short with a tweed rope."
            };

            return facts[RandomProvider.Instance.Next(1, facts.Count) - 1];
        }
        public static Embed RoleGenerationTemplate()
        {
            EmbedBuilder e = new EmbedBuilder();
            e.WithColor(EmbedData.GetColor("yield"));
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Generating Roles...".MarkdownBold());
            e.WithDescription(sb.ToString());
            e.WithFooter(GetFact());
            return e.Build();
        }
        public static Embed RoleAssignTemplate()
        {
            List<WerewolfRole> roles = WerewolfManager.GenerateRoles(8, out WerewolfTickGenerator t);
            List<string> addedroles = new List<string>();
            string shift = $"{(t.Shift > 0 ? "+" : "")}{t.Shift} Equinox";
            EmbedBuilder e = new EmbedBuilder();
            e.WithColor(EmbedData.GetColor("error"));
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("**Assigning Roles...**\n");

            foreach (WerewolfRole role in roles)
            {
                if (addedroles.Contains(role.Name))
                {
                    continue;
                }
                string pv = role.PointValue > 0 ? $"+{role.PointValue}" : $"{role.PointValue}";
                sb.AppendLine($"**x{roles.InstanceCount(role)}** | {role.Name.MarkdownBold()} ({pv})");
                sb.AppendLine($"{role.Summary}");

                addedroles.Add(role.Name);
            }
            e.WithDescription(sb.ToString());
            e.WithFooter($"{shift} | {t.PlayerCount} Players");
            return e.Build();
        }
        public static Embed FirstNightTemplate()
        {
            EmbedBuilder e = new EmbedBuilder();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("The first night falls, and everyone lay back in bed, with their eyes shut.");
            sb.AppendLine("However, it broke quite abruptly as someone was dragged out of their house!");
            sb.AppendLine("<This is when Werewolves get their queue that they are what they are>");
            sb.AppendLine("A mystic individual decides to take a gander but to no avail.");
            sb.AppendLine("<This is when the Seer gets their queue that they are what they are");
            sb.AppendLine("In the end, everyone decides to wait until morning, since it's the safest route.");
            e.WithDescription(sb.ToString());
            return e.Build();
        }
        public static Embed NightTemplate()
        {
            EmbedBuilder e = new EmbedBuilder();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("As night falls, everyone eventually closes shut.");
            sb.AppendLine("<Have changing dialogue based on the game's route>");
            sb.AppendLine("<Werewolves/Seers get their queue to choose a player for their ability.>");
            e.WithDescription(sb.ToString());
            return e.Build();
        }
        public static Embed FirstDeathTemplate()
        {
            EmbedBuilder e = new EmbedBuilder();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Oh my goodness, someone has been killed!");
            sb.AppendLine("We ought to avenge him, and put an end to this madness.");
            sb.AppendLine("<queue Suspicion Phase>");
            e.WithDescription(sb.ToString());
            return e.Build();
        }
        public static Embed DeathTemplate()
        {
            EmbedBuilder e = new EmbedBuilder();
            e.WithColor(EmbedData.GetColor("error"));
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"**PLAYER_NAME** has died. {GetRandomDeathCause()}");
            sb.AppendLine($"PLAYER_NAME proved themselves to be many things, but they preferred that you call them a  **{WerewolfManager.GetAnyRole().Name}**."); // <Description based on player's influence>
            e.WithDescription(sb.ToString());
            return e.Build();
        }
        public static Embed SuspicionTemplate()
        {
            EmbedBuilder e = new EmbedBuilder();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Nighttime falls quick, so you must hurry.");
            sb.AppendLine("If anyone odd catches your eye, go all out.");
            sb.AppendLine("<30 seconds to choose a convict>");
            e.WithDescription(sb.ToString());
            return e.Build();
        }
        public static Embed SecondMotionTemplate()
        {
            EmbedBuilder e = new EmbedBuilder();
            StringBuilder sb = new StringBuilder();
            e.WithTitle("An accusation has been made!");
            e.WithColor(EmbedData.GetColor("owo"));
            sb.AppendLine("**PLAYER_NAME_A** has seen enough of **PLAYER_NAME_B**'s tactics, and has accused them of being a **Werewolf**!");
            sb.AppendLine("Is there anybody else that follows through?");
            //sb.AppendLine("<10 seconds for anyone else to agree for trial>");
            e.WithDescription(sb.ToString());
            EmbedFooterBuilder f = new EmbedFooterBuilder();
            f.WithText("10 seconds until the window closes.");
            e.WithFooter(f);
            return e.Build();
        }
        public static Embed DefenseTemplate()
        {
            EmbedBuilder e = new EmbedBuilder();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("**PLAYER_NAME** has been brought up to trial, and would like to say a few words."); // <if they said anything>
            sb.AppendLine("```lol im not the wolf you big dumb```");
            e.WithDescription(sb.ToString());
            return e.Build();
        }
        public static Embed VotingTemplate()
        {
            EmbedBuilder e = new EmbedBuilder();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("And so the trial begins. Shall this disgrace of a human be hung?");
            sb.AppendLine("<Vote sequence, if player doesn't vote, it counts as a no>");
            e.WithDescription(sb.ToString());
            return e.Build();
        }
        public static Embed SeerScanTemplate()
        {
            EmbedBuilder e = new EmbedBuilder();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Who do you wish to inspect?");
            sb.AppendLine("<A collection of users, with icons on already scanned players>");
            e.WithDescription(sb.ToString());
            return e.Build();
        }
        public static Embed SeerOutcomeTemplate()
        {
            EmbedBuilder e = new EmbedBuilder();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("PLAYERA is <is or is not> a werewolf");
            sb.AppendLine("Head back to <Main game channel>");
            e.WithDescription(sb.ToString());
            return e.Build();
        }
        public static Embed WerewolfSelectionTemplate()
        {
            EmbedBuilder e = new EmbedBuilder();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("It's time to feast. Whose life shall <we/you if just you> take?");
            sb.AppendLine("<A list of players to kill>");
            e.WithDescription(sb.ToString());
            return e.Build();
        }
        public static Embed WerewolfBreakdownTemplate()
        {
            EmbedBuilder e = new EmbedBuilder();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Since there were so many tasty morsels, here's the rundown.");
            sb.AppendLine("<A cycling collection of chosen users, automatically choosing the first one without conflict>");
            e.WithDescription(sb.ToString());
            return e.Build();
        }
        public static Embed WerewolfOutcomeTemplate()
        {
            EmbedBuilder e = new EmbedBuilder();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("You've had your fill, and all is done.");
            sb.AppendLine("PLAYERA has been killed.");
            sb.AppendLine("Head back to <Main game channel>");
            e.WithDescription(sb.ToString());
            return e.Build();
        }

        // General Embeds
        // Role Generation Embed - A temporary embed showing that the roles are being generated.
        // Role Assign Embed - A temporary embed showing that roles are being assigned, with a list of all currently existing roles in the panel.
        // First Night Embed - A quick blurb of nighttime terrors, describing the event that had occured.
        // Night Embed - An embed showing that it's nighttime, acting quiet until the actions pursued by the skillsets were performed.
        // First Death Embed - An quick blurb showing that someone had died, with the markings of a werewolf, therefore starting the game.
        // Death Embed - A visual showing the played that died, and the role that they had. This also shows the cause of death.
        // Suspicion Embed - A visual showing all users on a list, where anyone can call someone out. A collection of message boxs is shown in the embed as messages are typed.
        // Second Motion Embed - A visual showing that a user has convicted <blank> of being a werewolf. If anyone else responds.... defense phase
        // Defense Embed - A user writes anything they want within 10 seconds, and once sent, the message is deleted, and showcased on the Defense Embed.
        // Voting Embed - A visual showing LEDs of all players voting or declining.

        // Skillset Embeds
        // Seer Scan Embed - A visual showing all users in a list, with an icon showing the previously scanned ones. typing a name 
        // Seer Scan Result Embed - A visual showing if the selected user was a werewolf or not. Notifies the user to return to their game.

        // Werewolf Selection Embed - A visual showing the list of users to kill. If there are more than one werewolf, the embed will update upon a werewolf voting.
        // Werewolf Breakdown Embed - A visual showing the conflicted set of users chosen to kill. This iterates through all voted users, and if more than 66% of werewolves choose to skip,
        // it continues until all are happy.
        // Werewolf Confirmation Embed - A visual showing that the person that they chose has been killed.
    }

    /// <summary>
    /// Represents a game handling display for any Orikivo.IGame.
    /// </summary>
    public class GameDisplay
    {
        /*
        A game display should accomplish the following:
        
            Make a game time limit.
            <All messages sent are deleted. To prevent rate limit, there's a slight cooldown.>
            a. When a new game starts, construct a category with all required channels, and make a queue in the primary channel. (Build an embed displaying the game, and player count)
            b. Players can opt in with [join while in the channel. They can also opt out with [leave.
            c. Once the game is allowed to start, it can be started by the game host.
            d. This embed will now be edited to the following game that is being played.

            ^ this embed is showing that the game is being built.

            in this stage, have the custom roles be generated, and assigned to each user.
            Notify their role in direct messages

            [Make embeds based on responses, and build listeners]
            allow hidden channels for users with abilities, such as the werewolf, and the seer.
            
             
             
             
             
             
        */
        
    }

    /// <summary>
    /// Represents as the base constructor for Orikivo.Werewolf.
    /// </summary>
    public class WerewolfConstructor : IGameConstructor
    {
        public WerewolfConstructor(List<ulong> players)
        {
            Roles = WerewolfManager.GenerateRoles(players.Count, out WerewolfTickGenerator tick);
            Tick = tick;
            Players = new List<IGamePlayer>();
            
            foreach (ulong player in players)
                Assign(player);
        }

        /// <summary>
        /// The balance manager of this Werewolf game.
        /// </summary>
        public WerewolfTickGenerator Tick { get; set; }

        /// <summary>
        /// The collection of generated roles for this Werewolf game.
        /// </summary>
        public List<WerewolfRole> Roles { get; set; }

        /// <summary>
        /// The collection of players in the Werewolf game.
        /// </summary>
        public List<IGamePlayer> Players { get; set; } = new List<IGamePlayer>();

        /// <summary>
        /// The count of all current players.
        /// </summary>
        public int PlayerCount { get { return Players.Count; } }

        public void Assign(ulong id)
        {
            Players.Add(new WerewolfPlayer(id));
        }

        public IGameBase Build()
        {
            return new WerewolfBase(Players);
        }

        // IGameConstructor: Defines player models.
        // IGameCore : Stores the information passed from a constructor.
        // IGameManager : Handles the game using IGameCore.
    }

    public enum GameStatus
    {
        Stable = 1,
        Bugged = 2,
        Unstable = 4,
        Incomplete = 8
    }


    /// <summary>
    /// The entry point for a Werewolf game.
    /// </summary>
    public class WerewolfGame : IGame
    {
        public WerewolfGame()
        {

        }

        public GameStatus Status { get { return GameStatus.Incomplete; } }
        public string Name { get { return "Werewolf"; } }
        public string ChannelName { get { return "werewolf-session"; } }
        public string Summary { get { return "A game where a group of individuals crack down the werewolf situation.";  } }
        public Range PlayerLimit { get { return new Range(2, 8); } } // real min is 5; 2 is for testing.
        public List<ulong> Group { get; set; } = new List<ulong>();
        public int Capacity { get { return Group.Count() - PlayerLimit.Max; } }
        public bool IsOpen { get { return (PlayerLimit.Max - Group.Count()) > 0; } }
        public bool IsPlayable { get { return (PlayerLimit.Min - Group.Count()) <= 0;} }
        public IGameEmbedder Embedder { get { return new WerewolfEmbedder(); } }
        /// <summary>
        /// Includes a player into the group.
        /// </summary>
        private void Include(ulong id)
        {
            Group.Add(id);
        }

        /// <summary>
        /// Attempts to include a player into the group.
        /// </summary>
        public bool TryInclude(ulong id)
        {
            if (HasPlayer(id))
                return false;
            if (!IsOpen)
                return false;

            Include(id);
            return true;
        }

        /// <summary>
        /// If the group currently has a player matching a specified id.
        /// </summary>
        public bool HasPlayer(ulong id)
        {
            if (Group.Any(x=>x == id))
                return true;
            return false;
        }

        /// <summary>
        /// Attempts to construct a game.
        /// </summary>
        public bool TryConstruct(out IGameConstructor construct)
        {
            construct = null;
            if (!IsPlayable)
                return false;
            construct = Construct();
            return true;
        }

        public IGameConstructor Construct()
        {
            return new WerewolfConstructor(Group);
        }

        public bool Equals(IGame constructor)
        {
            if (constructor.Name == Name)
            {
                return true;
            }

            return false;
        }
    }

    public class WerewolfTickGenerator
    {
        public const int RequiredWerewolves = 1;
        public const int MaxTanners = 1;
        public int Seers { get; set; }
        public int Werewolves { get; set; }
        public int Villagers { get; set; }
        public int Tanners { get; set; }
        public int Shift { get; set; }
        public int PlayerCount { get; set; }
        public int PlayersLeft { get; set; }
    }

    /// <summary>
    /// Represents the core process for handling Orikivo.WerewolfGame.
    /// </summary>
    public class WerewolfManager
    {
        /// <summary>
        /// A collection of game spectators.
        /// </summary>
        public List<GameSpectator> Spectators { get; set; }

        /// <summary>
        /// Contains the complete collection of assignable roles in Werewolf.
        /// </summary>
        public static List<WerewolfRole> RoleData = new List<WerewolfRole>
        {
            new WerewolfRole("Seer", "As the dark presence hides in the mist, you have the gift to scan the identities to see if they're a werewolf. You side with the Villagers.",
                WerewolfSkillset.Scan, WerewolfGroup.Villager, 6),
            new WerewolfRole("Villager", "You live in a fairly nice town, and you want to see those werewolves burn to the ground. You win upon eliminating all werewolves.",
                WerewolfSkillset.Empty, WerewolfGroup.Villager, 1),
            new WerewolfRole("Tanner", "You hate everything and just wanna die. You win upon getting hung.",
                WerewolfSkillset.Empty, WerewolfGroup.Neutral, 0, WerewolfWinFlag.Hung),
            new WerewolfRole("Werewolf", "You and your merry band of cursed individuals are hungry. You win when the villager count equals the werewolf count.",
                WerewolfSkillset.Kill, WerewolfGroup.Werewolf, -7)
        };

        private static bool HasNeutralRoles { get { return RoleData.Any(x => x.PointValue == 0); } }
        private static bool HasPositiveRoles { get { return RoleData.Any(x => x.PointValue > 0); } }
        private static bool HasNegativeRoles { get { return RoleData.Any(x => x.PointValue < 0); } }

        /// <summary>
        /// Returns a positive role that follows off of the shift balance.
        /// </summary>
        private static WerewolfRole GetPostiveRole(WerewolfTickGenerator tick)
        {
            return RoleData.Where(x => x.PointValue > 0).OrderBy(x => Math.Abs(x.PointValue - tick.Shift)).GetAny();
        }

        /// <summary>
        /// Gets a neutral role that provides no benefit to any side.
        /// </summary>
        private static WerewolfRole GetNeutralRole(WerewolfTickGenerator tick)
        {
            return RoleData.Where(x => x.PointValue == 0).GetAny();
        }

        /// <summary>
        /// Returns a negative role that follows off of the shift balance.
        /// </summary>
        private static WerewolfRole GetNegativeRole(WerewolfTickGenerator tick)
        {
            List<WerewolfRole> roles = RoleData;
            WerewolfRole r;
            Generate: r = roles.Where(x => x.PointValue < 0).OrderBy(x => Math.Abs(x.PointValue + tick.Shift)).GetAny();
            return r;
        }

        public static WerewolfRole GetAnyRole()
        {
            return RoleData.GetAny();
        }

        /// <summary>
        /// Gets the weakest role that least affects the shift balance.
        /// </summary>
        private static WerewolfRole GetWeakestRole(WerewolfTickGenerator tick)
        {
            return RoleData.OrderBy(x => Math.Abs(x.PointValue)).First();
        }
        
        /// <summary>
        /// Returns a role based on the balance shift and remaining players to assign a role for.
        /// </summary>
        private static WerewolfRole GetRole(WerewolfTickGenerator tick)
        {
            if (tick.Shift == 0 && tick.PlayersLeft == 0)
            {
                if (HasNeutralRoles)
                {
                    return GetNeutralRole(tick);
                }
            }

            /*if (tick.Shift.IsInRange(-2, -1) || tick.Shift.IsInRange(1, 2))
            {
                return GetWeakestRole(tick);
            }*/

            if (tick.Shift > 0)
            {
                return GetNegativeRole(tick);
            }
            if (tick.Shift < 0)
            {
                return GetPostiveRole(tick);
            }

            return GetAnyRole();
        }

        /// <summary>
        /// Generates a collection of roles suitable for the player count.
        /// </summary>
        public static List<WerewolfRole> GenerateRoles(int count, out WerewolfTickGenerator tick)
        {
            tick = new WerewolfTickGenerator();
            tick.Shift = 0; // Used to balance roles.
            tick.PlayerCount = count;
            List<WerewolfRole> roles = new List<WerewolfRole>();
            for (int i = 0; i < count; i++)
            {
                tick.PlayersLeft = count - i;
                WerewolfRole role = GetRole(tick); // Make a better system in trying to keep things fair.
                role.Name.Debug();
                roles.Add(role);
                tick.Shift += role.PointValue;
            }

            tick.Werewolves = roles.Where(x => x.Name == "Werewolf").Count();
            tick.Seers = roles.Where(x => x.Name == "Seer").Count();
            tick.Villagers = roles.Where(x => x.Name == "Villager").Count();
            tick.Tanners = roles.Where(x => x.Name == "Tanner").Count();
            return roles;
        }
        
        /// <summary>
        /// A constant that keeps track of how many convictions that can occur per day cycle.
        /// </summary>
        public const int ConvictionLimit = 3;

        public WerewolfBase Game { get; set; }

        /// <summary>
        /// The current round tick.
        /// </summary>
        public int Round { get; set; }

        /// <summary>
        /// Keeps track of what the current round has accomplished.
        /// </summary>
        public WerewolfRound RoundData { get; set; }

        /// <summary>
        /// Keeps track of the phases accomplished.
        /// </summary>
        private WerewolfTracker Phases { get; set; }
        
        /// <summary>
        /// The current game phase.
        /// </summary>
        public WerewolfPhase Phase { get; set; }

        /// <summary>
        /// Begins a new Orikivo.IWerewolf game, and stores the base data.
        /// </summary>
        public void Start()
        {

        }

        /// <summary>
        /// Reloads an Orikivo.IWerewolf game.
        /// </summary>
        /// <param name="id">The identifier for an Orikivo.IWerewolf game.</param>
        public void Reload(ulong id)
        {
            // Server.TryGetGameData(id); // Check if all users are still in the guild, and call all of them forth.
        }

        public List<WerewolfUser> Convictions {get; set;}
        public int Accusations { get { return Convictions.Count; } }
    }
    
    /// <summary>
    /// Keeps track of all phases that have occured in a game instance.
    /// </summary>
    public class WerewolfTracker
    {
        private const int SecondMotionPenalty = 5;
        private const int VotingPenalty = 10;

        // Role Assignment => First Night Phase
        // First Night Phase => First Death Phase
        // First Death Phase => Suspicion Phase
        // Night Phase => Day Phase
        // Day Phase => Death Phase || Suspicion Phase
        // Death Phase => Suspicion Phase || Night Phase || End Game
        // Suspicion Phase => Second Motion Phase
        // Second Motion Phase => Defense Phase || Suspicion Phase+(penalty)
        // Defense Phase => Voting Phase
        // Voting Phase => Death Phase || Suspicion Phase++(penalty, votelockconvict)

        /*
            Order of Phases:

            Assignment, at the very start of a game.
            Night, right after role assignment, and then right after each day.
            Day, (Death)?, Suspicion, Second Motion, Defense, Voting, (Death)?.
             
             
             */
    }

    /// <summary>
    /// Represents the outcome of a werewolf game.
    /// </summary>
    public class WerewolfResult
    {
        /// <summary>
        /// Provides the base information about who everyone was, and the roles they had.
        /// </summary>
        public WerewolfBase Base { get; set; }

        /// <summary>
        /// Contains every round that occured in this game instance.
        /// </summary>
        public List<WerewolfRound> Rounds { get; set; }

        /// <summary>
        /// Displays if the user won the game instance or not.
        /// </summary>
        public bool Won { get; set; }
    }

    public class WerewolfRound
    {
        /// <summary>
        /// All deaths that have occured in a round.
        /// </summary>
        public List<WerewolfDeath> Deaths { get; set; }
    }

    /// <summary>
    /// A log that contains information on the player that died.
    /// </summary>
    public class WerewolfDeath
    {
        public WerewolfUser Player { get; set; }

        /// <summary>
        /// The reason this player died.
        /// </summary>
        public WerewolfDeathCause Cause { get; set; }
    }

    /// <summary>
    /// An enumerator representing a toggle of possible deaths in Orikivo.IWerewolf.
    /// </summary>
    public enum WerewolfDeathCause
    {
        /// <summary>
        /// Death caused from a werewolf.
        /// </summary>
        Werewolf = 1,

        /// <summary>
        /// Death caused by being hung.
        /// </summary>
        Hanging = 2,

        /// <summary>
        /// Death caused by leaving the game prematurely.
        /// </summary>
        Quit = 4
    }

    /// <summary>
    /// Represents the core information of a Werewolf game.
    /// </summary>
    public class WerewolfBase : IGameBase
    {
        public WerewolfBase(List<IGamePlayer> players)
        {
            GameId = KeyBuilder.Generate();
            Players = players;
        }

        public string GameId { get; set; }
        public List<IGamePlayer> Players { get; set; }
        public List<WerewolfRole> Roles { get; set; }
    }

    /// <summary>
    /// Represents game progress and management for an Orikivo.Account in Orikivo.IWerewolf.
    /// </summary>
    public class WerewolfData
    {
        public ulong WinCount { get; set; }
        List<WerewolfResult> History { get; set; }
    }

    /// <summary>
    /// Represents an active player in Orikivo.IWerewolf.
    /// </summary>
    public class WerewolfPlayer : IGamePlayer
    {
        public WerewolfPlayer(ulong id)
        {
            Id = id;
        }
        public ulong Id { get; set; }
        public WerewolfRole Role { get; set; }
        public bool Alive { get; set; }
        public int Kills { get; set; }
        public int Protections { get; set; }
        public List<WerewolfConviction> Convictions { get; set; }

        public bool Equals(IGamePlayer p)
        {
            if (p.Id == Id)
            {
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Represents a base player in Orikivo.IWerewolf.
    /// </summary>
    public class WerewolfUser
    {
        public ulong Id { get; set; }
        public WerewolfRole Role { get; set; }
    }

    /// <summary>
    /// Represents a conviction for Orikivo.IWerewolf.
    /// </summary>
    public class WerewolfConviction
    {
        public int Round { get; set; }
        public WerewolfUser Convictor { get; set; } // the person that started the notion.
        public bool Passed { get; set; } // if the conviction was a success.
        public string DefenseStatement { get; set; } // the player's defense statement.
        public int Against { get; set; } // how many people were against the hanging
        public int For { get; set; } // how many people were for the hanging.
    }

    /// <summary>
    /// Represents the base information of a phase.
    /// </summary>
    public class WerewolfPhaseData
    {
        public WerewolfPhaseData(WerewolfPhase p)
        {

        }

        public TimeSpan GetPhaseDuration(WerewolfPhase p)
        {
            switch(p)
            {
                case WerewolfPhase.Night:
                    return new TimeSpan(0, 0, 60); // 60 seconds for all actions to be accomplished.
                case WerewolfPhase.Day:
                    return new TimeSpan(0, 0, 5); // 5 seconds to display a new day.
                case WerewolfPhase.Death:
                    return new TimeSpan(0, 0, 10); // 10 seconds to display a death.
                case WerewolfPhase.Conviction:
                    return new TimeSpan(0, 0, 30); // 30 seconds to attempt to convict.
                                                   // - 10 seconds per failed conviction.
                                                   // - 5 seconds per failed second motion.
                case WerewolfPhase.SecondMotion:
                    return new TimeSpan(0, 0, 10); // 10 seconds for anyone to chime in.
                case WerewolfPhase.Defense:
                    return new TimeSpan(0, 0, 15); // 15 seconds to say anything they wish to.
                case WerewolfPhase.Voting:
                    return new TimeSpan(0, 0, 20); // 20 seconds to vote to hand.
                default:
                    return new TimeSpan(0, 0, 1); // an unknown error.
            }
        }

        public TimeSpan Duration { get; set; }
    }

    /// <summary>
    /// An enumerator that defines the current game phase.
    /// </summary>
    public enum WerewolfPhase
    {
        Night = 1, // represents the skillset usage phase.
        Day = 2, // represents the new day phase.
        Death = 4, // represents a player death.
        Conviction = 8, // represents the choosing a convict.
        SecondMotion = 16, // represents the conviction confirmation.
        Defense = 32, // represents the convict's defense.
        Voting = 64, // represents the voting for a convict.
    }

    /// <summary>
    /// An enumerator that defines the current skillset in possession.
    /// </summary>
    public enum WerewolfSkillset
    {
        /// <summary>
        /// Nothing is altered.
        /// </summary>
        Empty = 1,

        /// <summary>
        /// The ability to scan other players to verify if they're a werewolf.
        /// </summary>
        Scan = 2,
        /// <summary>
        /// The ability to protect another player.
        /// </summary>
        Protect = 4,

        /// <summary>
        /// The ability to kill another player.
        /// </summary>
        Kill = 8
    }
    
    /// <summary>
    /// An enumerator that provides ways to win in Werewolf.
    /// </summary>
    public enum WerewolfWinFlag
    {
        /// <summary>
        /// You win by being hung.
        /// </summary>
        Hung = 1,
        /// <summary>
        /// You win upon the extermination of all werewolves.
        /// </summary>
        NoWerewolves = 2,
        /// <summary>
        /// You win by having your team size equal the opposite team's.
        /// </summary>
        EqualTeam = 4
    }

    /// <summary>
    /// Defines a collection of groups that Werewolf splits into.
    /// </summary>
    public enum WerewolfGroup
    {
        /// <summary>
        /// A villager hunts down werewolves.
        /// </summary>
        Villager = 1,

        /// <summary>
        /// A neutralist is in neither category, but instead pursues their own route.
        /// </summary>
        Neutral = 2,

        /// <summary>
        /// A werewolf's goal is to kill all villagers.
        /// </summary>
        Werewolf = 4
    }

    /// <summary>
    /// Represents a selectable role for Orikivo.Werewolf.
    /// </summary>
    public class WerewolfRole
    {
        public WerewolfRole(string name, string summary, WerewolfSkillset skillset, WerewolfGroup group, int pointvalue, WerewolfWinFlag? winFlag = null)
        {
            Name = name;
            Summary = summary;
            Skillset = skillset;
            Group = group;
            PointValue = pointvalue;
            WinRequirement = winFlag ?? GetWinFlag();
        }

        public string Name { get; set; }
        public string Summary { get; set; }
        public WerewolfSkillset Skillset { get; set; }
        public WerewolfGroup Group { get; set; }
        public WerewolfWinFlag WinRequirement { get; set; }
        public int PointValue { get; set; }

        private WerewolfWinFlag GetWinFlag()
        {
            switch (Group)
            {
                case WerewolfGroup.Werewolf:
                    return WerewolfWinFlag.EqualTeam;
                case WerewolfGroup.Villager:
                    return WerewolfWinFlag.NoWerewolves;
                default:
                    return WerewolfWinFlag.NoWerewolves;
            }
        }
    }
}
