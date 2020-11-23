namespace Arcadia.Multiplayer
{
    public class GameContext
    {
        // By default, the context is assumed to originate from a session
        public GameContext()
        {
            Type = InvokerType.Session;
        }

        // However, this can be overridden by providing an input context
        public GameContext(InputContext ctx)
        {
            Type = InvokerType.User;
            Invoker = ctx.Player;
            Connection = ctx.Connection;
            Session = ctx.Session;
            Server = ctx.Server;
        }

        public GameContext(Player invoker, GameSession session, GameServer server)
        {
            Invoker = invoker;
            Session = session;
            Server = server;
        }

        public InvokerType Type { get; set; }

        public Player Invoker { get; set; }

        // The connection that this was called in, if any
        public ServerConnection Connection { get; set; }

        public GameSession Session { get; set; }

        public GameServer Server { get; set; }
    }
}