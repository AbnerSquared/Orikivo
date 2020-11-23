using Discord;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents the information provided from an <see cref="IInput"/>.
    /// </summary>
    public class InputContext
    {
        public InputContext(IUser invoker, ServerConnection connection, GameServer server, InputResult input)
        {
            Invoker = invoker;
            Connection = connection;
            Server = server;
            Input = input;
        }

        public IUser Invoker { get; set; }

        public ServerConnection Connection { get; set; }

        public GameServer Server { get; set; }

        public GameSession Session => Server.Session;

        public Player Player => Server.GetPlayer(Invoker.Id);

        public InputResult Input { get; set; }
    }
}