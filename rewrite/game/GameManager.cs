using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo
{
    /// <summary>
    /// A root manager that handles and contains all of the games created.
    /// </summary>
    public class GameManager
    {
        public GameManager()
        {
            GameCreated += OnGameCreatedAsync;
            GameDeleted += OnGameDeletedAsync;
        }

        /// <summary>
        /// Returns a value defining if there are any currently active games in the dictionary.
        /// </summary>
        public bool IsEmpty => Games.Count == 0;

        /// <summary>
        /// A dictionary defining all of the currently active games.
        /// </summary>
        public ConcurrentDictionary<string, Game> Games { get; } = new ConcurrentDictionary<string, Game>();

        private readonly AsyncEvent<Func<Game, Task>> _gameCreatedEvent = new AsyncEvent<Func<Game, Task>>();
        private readonly AsyncEvent<Func<Game, Task>> _gameDeletedEvent = new AsyncEvent<Func<Game, Task>>();

        /// <summary>
        /// The event that is fired whenever a Game is created.
        /// </summary>
        public event Func<Game, Task> GameCreated
        {
            add => _gameCreatedEvent.Add(value);
            remove => _gameCreatedEvent.Remove(value);
        }
        
        /// <summary>
        /// The event that is fired whenever a Game is deleted.
        /// </summary>
        public event Func<Game, Task> GameDeleted
        {
            add => _gameDeletedEvent.Add(value);
            remove => _gameDeletedEvent.Remove(value);
        }
        

        private async Task OnGameCreatedAsync(Game game)
        {
            Console.WriteLine($"-- GameManager.GameCreated = #{game.Id} --");
        }
        private async Task OnGameDeletedAsync(Game game)
        {
            Console.WriteLine($"-- GameManager.GameDeleted = #{game.Id} --");
        }

        /// <summary>
        /// Creates a new Game using the specified parameters.
        /// </summary>
        /// <param name="context">The context that is passed down from a OriModuleBase.</param>
        /// <param name="config">The configuration set for a game.</param>
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

        /// <summary>
        /// Tells a Game to start the lobby. (This is bound to MessageReceived and will stay active)
        /// </summary>
        /// <param name="id">The identity of a Game.</param>
        /// <returns></returns>
        internal async Task StartGameSessionAsync(string id)
        {
            Game game = Games[id];
            await game.StartSessionAsync(); // in short, the game session task should always be open then??
        }

        /// <summary>
        /// Returns a value defining if the specified user identity was found across all of the current game sessions.
        /// </summary>
        /// <param name="userId">The identity of a Discord user.</param>
        public bool ContainsUser(ulong userId)
            => Games.Values.Any(x => x.Lobby.ContainsUser(userId));

        /// <summary>
        /// Deletes a game and wipes all references toward it.
        /// </summary>
        /// <param name="id">The identity of a Game.</param>
        public async Task DeleteGameAsync(string id)
        {
            await Games[id].CloseAsync();
            if (Games.TryRemove(id, out Game old))
                Console.WriteLine($"-- GameManager.GameDeleted = #{id} --"); // try to remove
        }

        /// <summary>
        /// Adds a user into a current game from the context of a OriModuleBase.
        /// </summary>
        /// <param name="context">The context that is passed down from a OriModuleBase.</param>
        /// <param name="gameId">The identity of a Game.</param>
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

