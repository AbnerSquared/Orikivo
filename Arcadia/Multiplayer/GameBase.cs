using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arcadia.Multiplayer.Games;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents a generic base structure for a game.
    /// </summary>
    public abstract class GameBase
    {
        internal void SetGameConfig(GameServer server)
            => Options = server.Config.GameOptions;

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
        public List<GameOption> Options { get; protected set; } = new List<GameOption>();

        /// <summary>
        /// When specified, handles building the required data for every player in a <see cref="GameSession"/>.
        /// </summary>
        public abstract List<PlayerData> OnBuildPlayers(List<Player> players);

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
        public abstract List<GameCriterion> OnBuildRules(List<PlayerData> players);

        /// <summary>
        /// When specified, builds all of the required display channels for this <see cref="GameBase"/> and returns them.
        /// </summary>
        /// <param name="players">Represents the collection of players for this <see cref="GameBase"/>.</param>
        public abstract List<DisplayChannel> OnBuildDisplays(List<PlayerData> players);

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
        public abstract SessionResult OnSessionFinish(GameSession session);

        // TODO: Implement usage of OnPlayerRemoved in games to handle random disconnects
        public virtual void OnPlayerRemoved(Player player)
        {

        }

        //public abstract IBaseSession OnBuildSession(GameServer server);

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

        public GameOption GetConfigProperty(string id)
        {
            if (Options.All(x => x.Id != id))
                throw new System.Exception("Could not find the specified configuration value.");

            return Options.First(x => x.Id == id);
        }

        public object GetConfigValue(string id)
        {
            return GetConfigProperty(id)?.Value;
        }

        public T GetConfigValue<T>(string id)
        {
            GameOption property = GetConfigProperty(id);

            if (property.ValueType == null)
                throw new System.Exception($"The specified config value '{id}' does not have an explicitly defined type");

            if (property.ValueType.IsEquivalentTo(typeof(T)))
                return (T) property.Value;

            throw new System.Exception("The implicit type specified does not match the explicitly defined type of the specified property");
        }
    }
}
