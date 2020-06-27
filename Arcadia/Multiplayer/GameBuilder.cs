using System.Collections.Generic;
using System.Linq;

namespace Arcadia
{
    /// <summary>
    /// Represents a generic constructor for a game.
    /// </summary>
    public abstract class GameBuilder
    {
        // CANNOT BE NULL
        // this is the identifier for the game.
        public string Id { get; protected set; }

        // CANNOT BE NULL
        // this is all of the basic information for a game
        public GameDetails Details { get; protected set; }

        // This is all of the configurations that can be set for a game
        public List<ConfigProperty> Config { get; protected set; } = new List<ConfigProperty>();

        

        // what is the method that is used to initialize a set of player data
        public abstract List<PlayerData> OnBuildPlayers(List<Player> players);

        // these are all of the attributes that this game will utilize.
        // attributes must be static, so no references are needed here
        public abstract List<GameProperty> OnBuildProperties();

        // these are all of the actions that this game will utilize
        // since some actions might require a specific player ID to reference
        // references to the generated players and the server can be used.
        public abstract List<GameAction> OnBuildActions(List<PlayerData> players);

        // these are all of the rules that this game will utilize
        public abstract List<GameCriterion> OnBuildRules(List<PlayerData> players);

        // what are all of the display channels to show?
        public abstract List<DisplayChannel> OnBuildDisplays(List<PlayerData> players);

        // what is done to the server that this session is now starting for?
        // at this point, the session is already generated
        // all you need to now is to handle what to display
        public abstract void OnSessionStart(GameServer server, GameSession session);

        // What happens to the session whenever SessionState.Finish is called?
        public abstract SessionResult OnSessionFinish(GameSession session);


        // how to set up the game server
        public virtual void Build(GameServer server)
        {
            var session = new GameSession(server, this);

            server.Session = session;

            foreach (ServerConnection connection in server.Connections)
            {
                connection.State = GameState.Playing;
            }

            OnSessionStart(server, session);
        }

        public ConfigProperty GetConfigProperty(string id)
        {
            if (!Config.Any(x => x.Id == id))
                throw new System.Exception("Could not find the specified configuration value.");

            return Config.First(x => x.Id == id);
        }

        public object GetConfigValue(string id)
        {
            return GetConfigProperty(id)?.Value;
        }

        public T GetConfigValue<T>(string id)
        {
            var property = GetConfigProperty(id);

            if (property.ValueType != null)
            {
                if (property.ValueType.IsEquivalentTo(typeof(T)))
                {
                    return (T) property.Value;
                }

                throw new System.Exception("The implicit type specified does not match the explicitly defined type of the specified property");
            }

            throw new System.Exception("The specified config value does not have an explicitly defined type");
        }
    }
}
