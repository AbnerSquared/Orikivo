using Orikivo;
using System.Collections.Generic;

namespace Arcadia

{
    public class GameServer
    {
        public GameServer()
        {
            Id = KeyBuilder.Generate(8);
            DisplayChannels = new List<DisplayChannel>();
            DisplayChannels.Add(new LobbyDisplayChannel());
            Players = new List<Player>();
            Connections = new List<ServerConnection>();
        }

        // the unique id of this lobby
        public string Id;

        // all base displays for the game server
        public List<DisplayChannel> DisplayChannels;

        // everyone connected to the lobby
        public List<Player> Players;

        // all of the channels that this lobby is connected to
        public List<ServerConnection> Connections; 
        // whenever a message or reaction is sent into any of these channels, attempt to figure out who sent it
    }
}
