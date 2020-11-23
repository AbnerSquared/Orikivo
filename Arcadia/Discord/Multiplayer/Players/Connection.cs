using Discord;

namespace Arcadia.Multiplayer
{
    public class Connection : Receiver
    {
        public Connection(GameServer server, IGuild guild, IMessageChannel channel) : base(channel)
        {
            Server = server;
            Guild = guild;
        }

        public GameServer Server { get; }

        public IGuild Guild { get; }
    }
}
