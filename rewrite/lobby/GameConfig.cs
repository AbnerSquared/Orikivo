namespace Orikivo
{
    // instead of creating yet another service for the lobby to manage
    // you would create a separate display dedicated to the game.
    // in this case, if someone joins the lobby and the game is in progress, they can bring up the old lobby display, and focus on updating that
    // receivers would have to change their focus, maybe a targetDisplay, which would specify what display they read off of.
    // due to the new node organization, you could technically make targeted node classes for specific purposes.


    public class GameConfig
    {
        public string Name { get; set; }
        public LobbyPrivacy Privacy { get; set; }
        public string Password { get; set; }
        public GameMode Mode { get; set; }
    }
}
