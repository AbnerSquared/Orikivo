using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents a generic game structure.
    /// </summary>
    /// <typeparam name="TPlayer"></typeparam>
    public abstract class GameBase<TPlayer> where TPlayer : IPlayer
    {
        public string Id { get; protected set; } // What is the ID for this game?
        public GameDetails Details { get; protected set; } // What are the details for this game?
        public List<GameAction> Actions { get; protected set; } // What are the actions for this game?
        public List<GameOption> Options { get; protected set; } // What are the options for this game?
        public List<TPlayer> Players { get; protected set; } // The data for the players?
        public abstract Task<List<TPlayer>> OnBuildPlayers(in IEnumerable<Player> players); // What to do to build players?
        public abstract Task OnPlayerRemoved(TPlayer player); // What to do when a player leaves/is removed?
        public abstract Task OnPlayerJoined(TPlayer player); // What to do when a player joins?
        public abstract Task OnConnectionRemoved(ServerConnection connection); // What to do when a connection was removed?
        public abstract Task OnConnectionCreated(ServerConnection connection); // What to do when a connection was created?
        public abstract Task OnGameBuild(); // What to do when this game is initialized
        public abstract Task OnGameComplete(); // What to do when this game is complete
        public abstract Task OnGameStart(); // How does this game start
    }

    public class TriviaPlayer
    {
        // This is all specified from the base player
        public ulong Id { get; }
        public string Name { get; }
        public DateTime JoinedAt { get; }
        public bool IsPlaying { get; }

        public int Score { get; internal set; }

        public int Streak { get; internal set; }

        public int TotalCorrect { get; internal set; }
    }

    /// <summary>
    /// Represents a generic structure for a game.
    /// </summary>
    public abstract class GameBase
    {
        internal void SetGameConfig(GameServer server)
            => Options = server.Options;

        // CANNOT BE NULL
        /// <summary>
        /// Represents the global identifier for this <see cref="GameBase"/>.
        /// </summary>
        public string Id { get; protected set; }

        // CANNOT BE NULL
        /// <summary>
        /// Represents the details of this <see cref="GameBase"/>.
        /// </summary>
        public GameDetails Details { get; protected set; }

        // CAN BE NULL
        /// <summary>
        /// Represents all of the possible configurations that this <see cref="GameBase"/> allows.
        /// </summary>
        public List<GameOption> Options { get; internal set; } = new List<GameOption>();

        /// <summary>
        /// When specified, handles building the required data for every player in a <see cref="GameSession"/>.
        /// </summary>
        public abstract List<PlayerData> OnBuildPlayers(in IEnumerable<Player> players);

        /// <summary>
        /// When specified, handles collecting all of the required global properties for this <see cref="GameBase"/>.
        /// </summary>
        /// <returns></returns>
        public abstract List<GameProperty> OnBuildProperties();

        // NOTE: Player references are included to allow you to require specific player IDs on certain actions
        /// <summary>
        /// When specified, handling collection all of the required actions for this <see cref="GameBase"/>.
        /// </summary>
        public abstract List<GameAction> OnBuildActions(List<PlayerData> players);

        /// <summary>
        /// When specified, handles collecting all of the required criterion for this <see cref="GameBase"/>.
        /// </summary>
        public virtual List<GameCriterion> OnBuildRules(List<PlayerData> players)
            => new List<GameCriterion>();

        /// <summary>
        /// When specified, builds all of the required display channels for this <see cref="GameBase"/> and returns them.
        /// </summary>
        /// <param name="players">Represents the collection of players for this <see cref="GameBase"/>.</param>
        public abstract List<DisplayBroadcast> OnBuildBroadcasts(List<PlayerData> players);

        // NOTE: At this point, the GameSession is already built. All you need to do is set the game up for the GameServer
        /// <summary>
        /// Represents the method used to start a <see cref="GameSession"/>.
        /// </summary>
        public abstract Task OnSessionStartAsync(GameServer server, GameSession session);

        // NOTE: This is what Arcadia retrieves when a GameSession finishes, allowing you to update stats, money, and items.
        // NOTE: THIS CANNOT GIVE YOU ITEMS THAT AREN'T MEANT TO ORIGINATE FROM A GAME
        /// <summary>
        /// Represents the method used to safely end a <see cref="GameSession"/>.
        /// </summary>
        public abstract GameResult OnSessionFinish(GameSession session);

        protected void FinalizeResult(GameResult result, GameSession session)
        {
            var properties = new List<GameProperty>();
            properties.AddRange(session.Properties);
            properties.AddRange(session.Options);

            result.GameId = session.Game.Id;
            result.SessionProperties = properties;
            result.SessionDuration = DateTime.UtcNow - session.StartedAt;
        }

        // Determine a player's exp amount based on their results
        protected virtual ulong CalculateExp(GameSession session, PlayerData player)
            => 0;

        // TODO: Implement usage of OnPlayerRemoved in games to handle random disconnects
        public virtual void OnPlayerRemoved(Player player)
        {

        }

        /// <summary>
        /// Builds the <see cref="GameSession"/> for this <see cref="GameBase"/> on the specified <see cref="GameServer"/>.
        /// </summary>
        public virtual async Task BuildAsync(GameServer server)
        {
            // Initialize the new game session
            var session = new GameSession(server, this);
            server.Session = session;

            // Set all of the server connections to playing
            foreach (ServerConnection connection in server.Connections)
                connection.State = GameState.Playing;

            // Read the method used when a session starts
            await OnSessionStartAsync(server, session);
        }
    }
}
