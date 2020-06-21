namespace Arcadia
{
    public class GameServerConfig
    {
        // what is the name used for this lobby?
        public string Title { get; set; }

        // what is the ID of the game that is being played?
        public string GameId { get; set; }

        // what is the privacy of this game server?
        public Privacy Privacy { get; set; }

        // for now, we can just simply refer to a basic array.
        public bool ValidateGame()
        {
            if (string.IsNullOrWhiteSpace(GameId))
                return false;

            return GameManager.Games.ContainsKey(GameId);
        }

        public GameBuilder GetGame()
        {
            if (!ValidateGame())
                return null;

            return GameManager.GetGame(GameId);
        }
    }
}
