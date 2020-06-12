using System.Collections.Generic;

namespace Arcadia
{
    // this handles everything related to the actual in-game process.
    /// <summary>
    /// Represents a generic constructor for a game.
    /// </summary>
    public abstract class GameBuilder
    {
        public string Id { get; set; }
        // this is all of the basic information for a game
        public GameDetails Details { get; set; }

        // This is all of the configurations that can be set for a game
        public List<GameProperty> Config { get; set; } = new List<GameProperty>();

        // These are all of the base attributes the game globally stores
        public List<GameProperty> Attributes { get; set; }

        public abstract List<GameAction> OnLoadActions();

        public abstract List<GameRule> OnLoadRules();

        // what are all of the display channels to show?
        public abstract List<DisplayChannel> OnLoadDisplays();

        // what is the method that is used to initialize a set of player data
        public abstract List<PlayerSessionData> OnLoadPlayers(List<Player> players);

        // what is done to the session when the game is started
        public abstract void OnSessionStart(GameSession session);

        public abstract List<GameProperty> OnLoadAttributes();

        // how to set up the game server
        public virtual void CreateSession(GameServer server)
        {
            var session = new GameSession
            {
                Actions = OnLoadActions(),
                Rules = OnLoadRules(),
                Rulesets = new List<RuleAction>(),
                Timer = null,
                Players = OnLoadPlayers(server.Players),
                Displays = OnLoadDisplays(),
                Attributes = OnLoadAttributes(),
                OnTimer = null
            };

            OnSessionStart(session);
        }
    }
}
