namespace Orikivo
{
    /*
    public class GameManager
    {
        public List<GameSession> Sessions;
    }

    public class GameSession
    {
        public Game Game;
    }
    
    public class Game
    {
        string Name;
        int PlayerCounter;
        bool AddPlayer(GameUserClient client);
        void RemovePlayer(GameUserClient client);
    }

    public class GameUserClient
    {

    }

    public class GameServer
    {
        private GameClientListener Listener;
        private List<GameUserClient> Clients;
        private List<GameUserClient> WaitingLobby;

        private Dictionary<GameUserClient, Game> ClientsInGame;
        private List<Game> Games;
        private List<GameThread> GameThreads;
        private Game NextGame;

        public string Id;
        public bool Running;

        public GameServer(string id)
        {
            Id = id;
            Running = false;
        }

        Listener = new GameClientListener(Id);

        public void Close()
        {
            if (Running)
                Running = false;
        }

        public void Run()
        {
            NextGame = new Game(this);

            Listener.Start();
            Running = true;
            List<Task> NewConnectionTasks = new List<Task>();

            while (Running)
            {
                if (Listener.Pending())
                    NewConnectionTasks.Add(HandleNewConnection());

                if (WaitingLobby.Count >= NextGame.RequiredPlayers)
                {
                    int playerCount = 0;
                    while (playerCount < NextGame.RequiredPlayers)
                    {
                        GameUserClient user = WaitingLobby[0];
                        WaitingLobby.RemoveAt(0);
                        
                        if (NextGame.AddPlayer(user))
                            playerCount++;
                        else
                            WaitingLobby.Add(user);
                    }

                    GameThread gameThread = new GameThread(new ThreadStart(NextGame.Run));
                    gameThread.Start();
                    Games.Add(NextGame);
                    GameThreads.Add(gameThread);

                    NextGame = new Game(this);
                }

                foreach(GameUserClient user in WaitingLobby.ToArray())
                {
                    GameEndPoint endPoint = user.Client.RemoteEndPoint;
                    bool disconnected = false;

                    GamePacket packet = GetPacket(user).GetAwaiter().GetResult();
                    disconnected = (packet?.Command == "exit");

                    if (disconnected)
                        HandleClosingClient(user);
                }
            
                Thread.Sleep(10);
            }

            Task.WaitAll(NewConnectionTasks.ToArray(), 1000);

            foreach (GameThread gameThread in GameThreads)
                gameThread.Abort();

            Parallel.ForEach(Clients, (client) =>
            {
                CloseClient(GameClientListener, "GAME_SERVER_CLOSED");
            });

            Listener.Stop(); // bye bye server.
        }

        private async Task HandleNewConnection()
        {
            GameUserClient userClient = await Listener.AcceptUserClientAsync();

            Clients.Add(userClient);
            WaitingLobby.Add(userClient);

            await SendPacket(userClient, new Packet("message", msg));
        }

        public void CloseClient(GameUserClient userClient, string message="")
        {
            Task closePacket = SendPacket(userClient, new Packet("bye", message));

            try
            {
                ClientsInGame[userClient]?.CloseClient(userClient);
            } catch(KeyNotFoundException) {}

            Thread.Sleep(100);

            closePacket.GetAwaiter().GetResult();
            HandleClosingClient(userClient);
        }

        public void HandleClosingClient(GameUserClient userClient)
        {
            Clients.Remove(userClient);
            WaitingLobby.Remove(userClient);
            CleanupClient(userClient);
        }

        public async Task SendPacket(GameUserClient userClient, Packet packet)
        {
            await Client.GetManager.ThrowPacket(packet);
        }

        public async Task<Packet> GetPacket(GameUserClient userClient)
        {
            Packet packet = null;
            try
            {
                if (userClient.Available == 0)
                    return null;


            }
        }
    }

    // https://16bpp.net/tutorials/csharp-networking/04e

    public class GameClientListener
    {

    }
    
    public interface IGame
    {
        
    }

    // start up the game manager at launch.
    */
    // figure it out, really lol
}