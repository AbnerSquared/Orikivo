using System;

namespace Orikivo
{
    public class OriGuild
    {
        public bool? IsBlacklisted { get; set; }
        public ulong Id { get; }
        public DateTime CreatedAt { get; }
        public string Name { get; set; }
        //public Mailbox? Mailbox {get; set;}
    }
    /*
    public class Server
    {
        public ulong Id;
        public ulong OwnerId;
        public string Name;
        public ServerQueue Queue;
        public ServerExperience LocalExperience;
        public ServerCooldown Cooldowns { get; set; }
        public ServerOptions Options { get; set; }
        public EventCollection Events { get; set; }
        public CustomCommandCollection CustomCommands { get; set; }
    }
    */
}