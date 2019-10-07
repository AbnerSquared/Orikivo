using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Orikivo
{
    // this is used to keep data across games.
    public class GameAttribute
    {
        public GameAttribute(string id, int defaultValue = 0)
        {
            Id = id;
            Value = DefaultValue = defaultValue;
        }

        public string Id { get; } // werewolf:werewolvesLeft
        public int Value { get; internal set; } // 2
        public int DefaultValue { get; } // 0
    }

    public interface ICriterion<in T>
    {
        bool Check(T value);
    }

    public class AttributeCriterion : ICriterion<GameAttribute>
    {
        internal AttributeCriterion(string requiredId, int requiredValue)
        {
            RequiredId = requiredId;
            RequiredValue = requiredValue;
        }
        public string RequiredId { get; }
        public int RequiredValue { get; }

        public bool Check(GameAttribute attribute)
            => attribute.Id == RequiredId && attribute.Value == RequiredValue;
    }

    public class GameCriterion
    {
        public List<AttributeCriterion> Criteria { get; }

        public bool Check(List<GameAttribute> attributes)
        {
            foreach (AttributeCriterion criterion in Criteria)
            {
                if (!attributes.Any(x => x.Id == criterion.RequiredId))
                    throw new Exception($"The GameCriterion is requesting an attribute ({criterion.RequiredId}) that doesn't exist.");
                if (!criterion.Check(attributes.First(x => x.Id == criterion.RequiredId)))
                {
                    Console.WriteLine($"An attribute failed to meet the criterion's required value. {criterion.RequiredId}: {criterion.RequiredValue}");
                    return false;
                }
                Console.WriteLine($"Criterion has been met. ({criterion.RequiredId})");
            }
            Console.WriteLine("All criteria have been met.");
            return true;
        }
    }

    // defines the requirements to start a game.
    public class GameBootCriteria
    {
        private GameBootCriteria() { }
        public static GameBootCriteria FromMode(GameMode mode)
        {
            GameBootCriteria clientCriteria = new GameBootCriteria();
            switch(mode)
            {
                case GameMode.Werewolf:
                    clientCriteria.RequiredUsers = 5;
                    clientCriteria.UserLimit = 15;
                    break;
                default:
                    throw new Exception("An unknown game mode has been specified.");
            }
            return clientCriteria;
        }
        public int RequiredUsers { get; internal set; }
        public int UserLimit { get; internal set; }
        public bool Check(int userCount)
        {
            return UserLimit >= userCount && userCount >= RequiredUsers;
        }
    }

    // container for game displays.
    public class GameMonitor
    {

    }

    // container for display content.
    public class GameDisplay
    {
        // defines what display is correlated to what game state it's meant for.
        public GameChannel Type { get; }
        public string SyncKey { get; private set; }
    }

    // the type of display it's meant for
    public enum GameChannel
    {
        Lobby = 1,
        Game = 2,
        Spectate = 3
    }

    public class Game
    {
        private BaseSocketClient _client;
        private GameEventHandler _events;
        private GameConfig _config;

        public Game(BaseSocketClient client, GameConfig config)
        {
            _client = client;
            _events = new GameEventHandler();
            Id = KeyBuilder.Generate(8);
            Logger = new GameLogger();
            Lobby = new GameLobby(config, _events);
            Monitor = new GameMonitor(); // this needs to build the generic lobby display
            // the game display can be built in game properties.
            // _events.UserJoined += ...

        }

        public string Id { get; }
        public string SessionName { get; private set; }
        public GamePrivacy Privacy { get; private set; }
        public string Password { get; private set; }
        public bool IsProtected => Checks.NotNull(Password);
        public GameMode Mode { get; private set; }
        public GameState State { get; private set; }
        public GameMonitor Monitor { get; } // the root display for all receivers.
        // create UpdateDisplay(...);
        public GameLogger Logger { get; } // logs all events in the game.
        public GameLobby Lobby { get; } // contains all of the users and receivers.
        // attach the client onto the game, delete it when done.
        internal GameClient Client { get; private set; }
        public List<User> Users => Lobby.Users;
        public List<GameReceiver> Receivers => Lobby.Receivers;

        // detect message mechanics
        private async Task OnMessageReceivedAsync(SocketMessage message) { }
        //if (userId == _client.CurrentUser.Id)

        // this is what's used to start the lobby client.
        public async Task BootAsync(OriCommandContext context)
        {
            if (State == GameState.Active)
                throw new Exception("The game has already started.");

            await Lobby.BootAsync(context);
            // create the lobby handle here...
        }

        public async Task<GameResult> StartAsync()
        {
            if (State == GameState.Active)
                throw new Exception("The game has already started.");

            if (!Lobby.IsInitialized)
                throw new Exception("The game hasn't been initialized yet.");

            if (!Lobby.CanStart)
                throw new Exception("The game does not meet the criteria to start.");

            Client = new GameClient(this, _client, _events);
            return await Client.StartAsync();
        }

        internal async Task CloseAsync()
        {
            if (State == GameState.Active)
                await Client.StopAsync("The game is being closed due to unexpected reasons.", TimeSpan.FromSeconds(3));

            _client.MessageReceived -= OnMessageReceivedAsync;
            await Lobby.ClearAsync();
        }

        public bool ContainsUser(ulong userId)
            => Lobby.ContainsUser(userId);
        public bool ContainsGuild(ulong guildId)
            => Lobby.ContainsGuild(guildId);

        public override string ToString()
            => $"**{Lobby.Name}** #{Id}\n**{(State == GameState.Active ? "In Progress" : "Open")}** • (**{Lobby.UserCount}** of **{Lobby.UserLimit}**)\n`{Lobby.Mode}`";
    }

    // this is the root game client data that is passed down to each game client.
    public class GameData
    {
        // attributes and triggers can be left empty.
        internal GameData(List<GameAttribute> attributes, List<UserGameData> userData, List<GameTrigger> triggers)
        {
            Attributes = attributes;
            UserData = userData;
            Triggers = triggers;
        }
        public List<GameAttribute> Attributes { get; }
        public List<UserGameData> UserData { get; }
        public List<GameTrigger> Triggers { get; }
    }

    public class GameClient
    {
        private GameEventHandler _events;
        private BaseSocketClient _client;
        private GameLobby _lobby; // contains the root info.
        private bool _active = false;
        public GameClient(Game game, BaseSocketClient client, GameEventHandler events)
        {
            _events = events;
            _lobby = game.Lobby;
            GameProperties properties = GameProperties.Create(game.Mode, game.Users);
            EntryTask = properties.EntryTask;
            Tasks = properties.Tasks;
            ExitTask = properties.ExitTask;
            Data = properties.BaseData;
        }

        // the client's own data that is passed along each game.
        private GameData Data { get; }

        private CancellationTokenSource GameToken { get; } = new CancellationTokenSource();

        private GameTask EntryTask { get; }
        private GameTask ExitTask { get; }
        private List<GameTask> Tasks { get; }

        private GameTask CurrentTask { get; set; }

        internal async Task<GameResult> StartAsync()
        {
            try
            {
                _active = true;
                CurrentTask = EntryTask;
                do
                {
                    GameRoute route = await CurrentTask.StartAsync(_client, _lobby, _events, Data, GameToken.Token);

                    if (!Checks.NotNull(route.TaskId))
                    {
                        if (CurrentTask.Id == ExitTask.Id)
                            _active = false;
                        else
                            CurrentTask = ExitTask; // polish the exit mechanic
                    }
                    else
                    {
                        if (!Tasks.Any(x => x.Id == route.TaskId))
                            throw new Exception("A route attempted to go to a task that doesn't exist.");
                        CurrentTask = Tasks.First(x => x.Id == route.TaskId);
                    }
                } while (_active);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await StopAsync("An exception has occured.", TimeSpan.FromSeconds(3));
                return GameResult.FromException(ex);
            }

            return GameResult.Empty;
        }

        internal async Task StopAsync(string reason = null, TimeSpan? delay = null)
        {
            GameToken.Cancel();
            // catch all client msg handles.
        }
    }

    // everything that is to be accompanied for a user.
    public class UserDataFrame
    {

    }

    public enum AttributeUpdateType
    {
        Append = 1, // value += this
        Set = 2 // value = this
    }

    public enum AttributeMatch
    {
        Equals = 1,
        NotEquals = 2,
        Greater = 3,
        Lesser = 4,
        GreaterOrEquals = 5,
        LesserOrEquals = 6
    }

    // since the lobby is a required static component for a game, make it hardcoded
    // as opposed to designing all of the features within the game.
    public enum LobbyAction
    {
        Kick = 1,

    }
}
