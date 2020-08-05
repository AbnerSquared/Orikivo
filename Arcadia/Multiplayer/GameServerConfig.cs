using System.Collections.Generic;
using System.Linq;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents the properties of a <see cref="GameServer"/>.
    /// </summary>
    public class ServerProperties
    {
        public const int MaxTitleLength = 42;

        private GameBuilder Game;

        /// <summary>
        /// Gets or sets the value that represents the name of a <see cref="GameServer"/>.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the game that the <see cref="GameServer"/> will play.
        /// </summary>
        public string GameId { get; set; }

        /// <summary>
        /// Gets or sets the visibility of the <see cref="GameServer"/>.
        /// </summary>
        public Privacy Privacy { get; set; }

        public List<ConfigProperty> GameConfig { get; internal set; }

        public GameDetails GameDetails => GameManager.Games.ContainsKey(GameId) ? GameManager.GetGame(GameId).Details : null;

        public ConfigProperty GetConfigProperty(string id)
        {
            if (GameConfig.All(x => x.Id != id))
                throw new System.Exception("Could not find the specified configuration value.");

            return GameConfig.First(x => x.Id == id);
        }

        public void SetOption(string optionId, object value)
        {
            ConfigProperty option = GetConfigProperty(optionId);

            if (option.ValueType.IsEquivalentTo(value.GetType()))
                option.Value = value;
        }

        public bool IsValidGame()
        {
            return IsValidGame(GameId);
        }

        public bool IsValidGame(string id)
        {
            return !string.IsNullOrWhiteSpace(id) && GameManager.Games.ContainsKey(id);
        }

        public GameBuilder LoadGame()
        {
            if (Game != null && GameId == Game.Id)
                return Game;

            if (!IsValidGame())
                return null;

            if (Game != null && GameId != Game.Id)
                Game = GameManager.GetGame(GameId);
            
            Game ??= GameManager.GetGame(GameId);
            GameConfig = Game.Config;

            return Game;
        }
    }
}
