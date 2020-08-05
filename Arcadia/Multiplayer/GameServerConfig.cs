using System.Collections.Generic;
using System.Linq;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents the properties of a <see cref="GameServer"/>.
    /// </summary>
    public class ServerProperties
    {
        // This is the max length of a server name
        public const int MaxNameLength = 42;

        private GameBase Game;

        // This is the name of this server
        public string Name { get; set; }

        // This represents the ID of the game that will be played
        public string GameId { get; set; }

        // This handles the visibility of this server
        public Privacy Privacy { get; set; }

        // This can probably be renamed to CustomOptions
        public List<GameOption> GameOptions { get; internal set; }

        public GameDetails GameDetails => GameManager.Games.ContainsKey(GameId) ? GameManager.GetGame(GameId).Details : null;


        // TODO: Move these methods into a class that it fits for better
        public GameOption GetConfigProperty(string id)
        {
            if (GameOptions.All(x => x.Id != id))
                throw new System.Exception("Could not find the specified configuration value.");

            return GameOptions.First(x => x.Id == id);
        }

        public void SetOption(string optionId, object value)
        {
            GameOption option = GetConfigProperty(optionId);

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

        public GameBase LoadGame()
        {
            if (Game != null && GameId == Game.Id)
                return Game;

            if (!IsValidGame())
                return null;

            if (Game != null && GameId != Game.Id)
                Game = GameManager.GetGame(GameId);
            
            Game ??= GameManager.GetGame(GameId);
            GameOptions = Game.Options;

            return Game;
        }
    }
}
