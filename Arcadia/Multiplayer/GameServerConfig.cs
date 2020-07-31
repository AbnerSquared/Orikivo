using System.Collections.Generic;

namespace Arcadia.Multiplayer
{
    public class GameServerConfig
    {

        private GameBuilder Game;

        /// <summary>
        /// Gets or sets the value that represents the title for a <see cref="GameServer"/>.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the game that the <see cref="GameServer"/> will play.
        /// </summary>
        public string GameId { get; set; }

        /// <summary>
        /// Gets or sets the visibility of the <see cref="GameServer"/>.
        /// </summary>
        public Privacy Privacy { get; set; }

        public List<ConfigProperty> GameConfig { get; internal set; }

        // for now, we can just simply refer to a basic array.
        public bool ValidateGame()
        {
            if (string.IsNullOrWhiteSpace(GameId))
                return false;

            return GameManager.Games.ContainsKey(GameId);
        }

        public bool ValidateGame(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;

            return GameManager.Games.ContainsKey(id);
        }

        public GameBuilder LoadGame()
        {
            if (!ValidateGame())
                return null;

            if (Game != null)
            {
                if (GameId != Game.Id)
                    Game = GameManager.GetGame(GameId);
                else
                    Game ??= GameManager.GetGame(GameId);

                GameConfig = Game.Config;
            }
            else
            {
                Game ??= GameManager.GetGame(GameId);
                GameConfig = Game.Config;
            }

            return Game;
        }
    }
}
