using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo
{
    // this handles all games built, etc...
    public class GameManager
    {
        public GameManager()
        {
            GameCreated += OnGameCreatedAsync;
        }

        public bool IsEmpty => Games.Count == 0;
        // Id, Game
        public ConcurrentDictionary<string, Game> Games { get; } = new ConcurrentDictionary<string, Game>();


        public event Func<Game, Task> GameCreated { add { _gameCreatedEvent.Add(value); } remove { _gameCreatedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Game, Task>> _gameCreatedEvent = new AsyncEvent<Func<Game, Task>>();

        public event Func<Game, Task> GameDeleted { add { _gameDeletedEvent.Add(value); } remove { _gameDeletedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Game, Task>> _gameDeletedEvent = new AsyncEvent<Func<Game, Task>>();

        public async Task OnGameCreatedAsync(Game game)
        {
            Console.WriteLine($"-- GameManager.GameCreated = #{game.Id} --");
        }

        public async Task<Game> CreateGameAsync(OriCommandContext context, GameConfig config)
        {
            Game game = new Game(context.Client, config);
            await game.BootAsync(context);
            if (Games.ContainsKey(game.Id))
                throw new Exception("There is already a game using this exact id."); // handle
            Games.AddOrUpdate(game.Id, game, (id, currentGame) => game);
            await _gameCreatedEvent.InvokeAsync(game);
            return game;
        }

        internal async Task StartGameSessionAsync(string id)
        {
            Game game = Games[id];
            await game.StartSessionAsync(); // in short, the game session task should always be open then??
        }

        // starting the game needs to occur outside of the MessageReceived.
        internal async Task<GameResult> StartGameAsync(string id)
        {
            Game game = Games[id];
            GameResult result = game.StartAsync().Result;//.ConfigureAwait(false);
            return result;
        }

        public bool ContainsUser(ulong userId)
            => Games.Values.Any(x => x.Lobby.ContainsUser(userId));

        public async Task DeleteGameAsync(string id)
        {
            await Games[id].CloseAsync();
            if (Games.TryRemove(id, out Game old))
                Console.WriteLine($"-- GameManager.GameDeleted = #{id} --"); // try to remove
        }
        // adds the user from the executed command context into a game
        public async Task<bool> AddUserAsync(OriCommandContext context, string gameId)
        {
            Game game = this[gameId];
            // if the receiver the user belongs to is currently actively playing, prevent joining from their guild.
            if (game.Receivers.Any(x => x.Id == context.Guild.Id && x.State is GameState.Active))
                throw new Exception("This guild is currently playing a game and cannot support lobby idling.");

            await game.Lobby.AddUserAsync(context.Account, context.Guild);
            return true;
        }

        public Game this[string id]
        {
            get
            {
                if (Games.ContainsKey(id))
                    return Games[id];
                throw new Exception($"There weren't any existing games matching #{OriFormat.CropGameId(id)}");
            }
        }
    }
}

// the game should be able to determine all of the active users playing, and all of the users in the lobby.
// receivers should keep track of all users linked from themselves to be able to properly determine if the users are currently playing or not.
// spectators can only watch what happens in the game
// they cannot interact or affect anything within the game.
// spectating can be forced for everyone connected to a receiver, based if the main user for a receiver is the leader

// .Host => The leader of the game.
// .ReceiverHost => The leader of the connected receiver.

// this way, if the receiver host wishes to spectate, they make everyone in the receiver do so.

// games should also be spectated within DMs.
