namespace Orikivo
{
    // this is the general options that can be changed in a lobby.
    public class LobbyConfig
    {
        public LobbyConfig()
        {
            Name = "New Lobby";
            Privacy = LobbyPrivacy.Local;
            Mode = GameMode.Werewolf;
            ReceiverConfig = new ReceiverConfig(Name);
        }

        public string Name { get; set; } // the name of the lobby.
        public LobbyPrivacy Privacy { get; set; } // if the lobby can be seen by other servers.
        public string Password { get; set; } // the password of the lobby
        public GameMode Mode { get; set; } // the game mode being played in the lobby.
        public ReceiverConfig ReceiverConfig { get; set; } // the default config that is used.
    }
}
