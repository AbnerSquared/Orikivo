namespace Orikivo.Gaming
{
    // public delegate void LiterallyAnyMethodInANamespace();
    /// <summary>
    /// Represents a generic game.
    /// </summary>
    public class Game
    {
        public GameDetails Details { get; protected set; }
        public GameCriteria Criteria { get; protected set; }

        // this needs to be able to generate
        // all attributes for a game
        // as well as generate the display panel.

    }

    // Represents a generated game.
    public class GameClient
    {

        // the game client should store:
        // - root attributes and predicates that execute
        //      actions

        // - all players with their attributes and predicates
        //      that execute actions.

        // - the base display that auto-updates based on predicates

    }

    // these are base requirements needed to start the game.
    public class GameCriteria
    {
        public int PlayerCount;
        public int? PlayerLimit;
    }

    /// <summary>
    /// Represents the details for a <see cref="Game"/>.
    /// </summary>
    public class GameDetails
    {
        // used to identify the game mode
        public string Id { get; set; }

        public string Name { get; set; }

        public string Summary { get; set; }

    }
}
