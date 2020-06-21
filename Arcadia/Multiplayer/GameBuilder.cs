using System.Collections.Generic;

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
        public abstract List<PlayerSessionData> OnBuildPlayers(List<Player> players);

        // these are all of the attributes that this game will utilize.
        // attributes must be static, so no references are needed here
        public abstract List<GameProperty> OnBuildAttributes();

        // these are all of the actions that this game will utilize
        // since some actions might require a specific player ID to reference
        // references to the generated players and the server can be used.
        public abstract List<GameAction> OnBuildActions(List<PlayerSessionData> players);

        // these are all of the rules that this game will utilize
        public abstract List<GameRule> OnBuildRules(List<PlayerSessionData> players);

        // what are all of the display channels to show?
        public abstract List<DisplayChannel> OnBuildDisplays(List<PlayerSessionData> players);

        // what is done to the server that this session is now starting for?
        // at this point, the session is already generated
        // all you need to now is to handle what to display
        public abstract void OnSessionStart(GameServer server);

        // What happens to the session whenever SessionState.Finish is called?
        public abstract SessionResult OnSessionFinish(GameSession session);


        // how to set up the game server
        public virtual void Build(GameServer server)
        {
            // generate the players and pass them to the other building handlers
            List<PlayerSessionData> players = OnBuildPlayers(server.Players);


            var session = new GameSession
            {
                Actions = OnBuildActions(players),
                Rules = OnBuildRules(players),
                Rulesets = new List<RuleAction>(),
                Timer = null,
                Players = players,
                Displays = OnBuildDisplays(players),
                Attributes = OnBuildAttributes(),
                OnTimer = null
            };

            server.Session = session;

            OnSessionStart(server);
        }
    }

    // this is the result of a GameSession that is used to update EXP, give rewards, etc.
    public class SessionResult
    {
        // Experience
        // Rewards
        // Stats
        // Money
    }
}
