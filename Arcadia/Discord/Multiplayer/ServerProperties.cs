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

        public static readonly ServerProperties Default = new ServerProperties
        {
            GameId = "Trivia",
            Privacy = Privacy.Public,
            Name = "New Game Server",
            AllowedActions = ServerAllow.Chat | ServerAllow.Spectate
        };

        public static ServerProperties GetDefault(string hostName)
        {
            ServerProperties properties = Default;
            properties.Name = $"{hostName}'s Server";

            return properties;
        }

        public ServerProperties()
        {
        }


        public string Name { get; set; }

        public string GameId { get; set; }

        public Privacy Privacy { get; set; }

        public ServerAllow AllowedActions { get; set; }
    }
}
