using System.Collections.Generic;
using System.Linq;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents a generic constructor for a game.
    /// </summary>
    public abstract class GameBuilder
    {
        // CANNOT BE NULL
        /// <summary>
        /// Represents the global identifier for this <see cref="GameBuilder"/>.
        /// </summary>
        public string Id { get; protected set; }

        // CANNOT BE NULL
        /// <summary>
        /// Represents the details of this <see cref="GameBuilder"/>.
        /// </summary>
        public GameDetails Details { get; protected set; }

        // CAN BE NULL
        /// <summary>
        /// Represents all of the possible configurations that this <see cref="GameBuilder"/> allows.
        /// </summary>
        public List<ConfigProperty> Config { get; protected set; } = new List<ConfigProperty>();

        /// <summary>
        /// When specified, handles building the required data for every player in a <see cref="GameSession"/>.
        /// </summary>
        public abstract List<PlayerData> OnBuildPlayers(List<Player> players);

        /// <summary>
        /// When specified, handles collecting all of the required global properties for this <see cref="GameBuilder"/>.
        /// </summary>
        /// <returns></returns>
        public abstract List<GameProperty> OnBuildProperties();

        // NOTE: Player references are included to allow you to require specific player IDs on certain actions
        /// <summary>
        /// When specified, handling collection all of the required actions for this <see cref="GameBuilder"/>.
        /// </summary>
        public abstract List<GameAction> OnBuildActions(List<PlayerData> players);

        /// <summary>
        /// When specified, handles collecting all of the required criterion for this <see cref="GameBuilder"/>.
        /// </summary>
        public abstract List<GameCriterion> OnBuildRules(List<PlayerData> players);

        /// <summary>
        /// When specified, builds all of the required display channels for this <see cref="GameBuilder"/> and returns them.
        /// </summary>
        /// <param name="players">Represents the collection of players for this <see cref="GameBuilder"/>.</param>
        public abstract List<DisplayChannel> OnBuildDisplays(List<PlayerData> players);

        // NOTE: At this point, the GameSession is already built. All you need to do is set the game up for the GameServer
        /// <summary>
        /// Represents the method used to start a <see cref="GameSession"/>.
        /// </summary>
        public abstract void OnSessionStart(GameServer server, GameSession session);

        // NOTE: This is what Arcadia retrieves when a GameSession finishes, allowing you to update stats, money, and items.
        // NOTE: THIS CANNOT GIVE YOU ITEMS THAT AREN'T MEANT TO ORIGINATE FROM A GAME
        /// <summary>
        /// Represents the method used to safely end a <see cref="GameSession"/>.
        /// </summary>
        public abstract SessionResult OnSessionFinish(GameSession session);

        /// <summary>
        /// Builds the <see cref="GameSession"/> for this <see cref="GameBuilder"/> on the specified <see cref="GameServer"/>.
        /// </summary>
        public virtual void Build(GameServer server)
        {
            // Initialize the new game session
            var session = new GameSession(server, this);
            server.Session = session;

            // Set all of the server connections to playing
            foreach (ServerConnection connection in server.Connections)
                connection.State = GameState.Playing;

            // Read the method used when a session starts
            OnSessionStart(server, session);
        }

        public ConfigProperty GetConfigProperty(string id)
        {
            if (Config.All(x => x.Id != id))
                throw new System.Exception("Could not find the specified configuration value.");

            return Config.First(x => x.Id == id);
        }

        public object GetConfigValue(string id)
        {
            return GetConfigProperty(id)?.Value;
        }

        public T GetConfigValue<T>(string id)
        {
            ConfigProperty property = GetConfigProperty(id);

            if (property.ValueType == null)
                throw new System.Exception($"The specified config value '{id}' does not have an explicitly defined type");

            if (property.ValueType.IsEquivalentTo(typeof(T)))
                return (T) property.Value;

            throw new System.Exception("The implicit type specified does not match the explicitly defined type of the specified property");
        }
    }
}
