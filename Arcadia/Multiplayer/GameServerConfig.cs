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
    }
}
