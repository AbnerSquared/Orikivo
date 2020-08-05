using Orikivo.Drawing;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents the details of a game.
    /// </summary>
    public class GameDetails
    {
        // what is the name of this game?
        public string Name { get; set; }

        // What is the icon that this game uses?
        public Sprite Icon { get; set; }

        // what is this game about?
        public string Summary { get; set; }

        // how many players are needed for this game?
        public int RequiredPlayers { get; set; } = 1;

        // what is the limit of players that can join?
        public int PlayerLimit { get; set; } = 16;
    }
}
