using Discord;
using Discord.WebSocket;
using Orikivo.Storage;
using System.Collections.Generic;

namespace Orikivo
{
    public class ServerUser
    {
        public bool Active { get; set; } // if the user is currently active.
        public ulong Id { get; set; } // the id of the user in this server.
        public ulong Span { get; set; } // the span of time they have spent.
        public int Leaves { get; set; } // amount of times they have left this server.
        public int Joins { get; set; } // amount of times they have joined this server.
    }

    public class Server : IStorable
    {
        // Make DataContainer static, to allow any class to access.
        public Server()
        {
            Name = "New Server";
            Id = 0;
        }

        public Server(IGuild guild)
        {
            Name = guild.Name;
            Id = guild.Id;
        }

        public string Name {get; set;}
        public ulong Id {get; set;}
        public ServerConfig Config {get; set;} = new ServerConfig(); // Used to configure how your server works.
        //public Queue<Song> Queue { get; set; } = new Queue<Song>(); // Used for queuing songs.
        public List<GameSession> OpenGameSessions { get; set; } = new List<GameSession>();
        // Build a server card template.
        // These should only be handled by this class only.
        public bool Updated { get; set; } = true;
        public bool Building { get; set; } // check if the card is still building
        
        public void CloseSessions(SocketGuild g)
        {
            foreach (GameSession session in OpenGameSessions)
            {
                session.Close(g);
            }
            OpenGameSessions.Clear();
        }

        public void Save()
            => Manager.Save(this, FileManager.TryGetPath(this));
        

        public void UpdateCard()
        {
            if (Updated || Building) return;
            // Use this to update the server card
        }

        //To return it as a SocketGuild object.
        public SocketGuild Guild(DiscordSocketClient client)
            => client.GetGuild(Id);
        
        // Every new object for a server must be above this.
        public override string ToString()
            => $"guild-{Name.ToLower()}.{Id}";

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            return Equals(obj as IStorable);
        }

        public bool Equals(IStorable storable)
            => Id == storable.Id;

        public override int GetHashCode()
            => unchecked((int)Id);
    }
}