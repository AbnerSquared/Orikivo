using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    public class OriCommandContext : SocketCommandContext
    {
        public OriUser Account { get; internal set; }
        public OriGuild Server { get; }
        public OriGlobal Global { get; }
        public OriJsonContainer Container { get; }
        //public CommandInfo Command { get; internal set; } // maybe contain executed command info here.

        public OriCommandContext(DiscordSocketClient client, OriJsonContainer container, SocketUserMessage msg) : base(client, msg)
        {
            Console.WriteLine("[Debug] -- Constructing command context. --");
            Container = container;
            Global = Container.Global;
            if (Guild != null)
            {
                Server = Container.GetOrAddGuild(Guild);
                if (Server.Name != Guild.Name)
                    Server.Name = Guild.Name;
                if (Server.OwnerId != Guild.OwnerId)
                    Server.OwnerId = Guild.OwnerId;
                Console.WriteLine("[Debug] -- Guild account found or built. --");
            }
            if (User != null)
            {
                if (Container.TryGetUser(User.Id, out OriUser account))
                {
                    Account = account;
                    Console.WriteLine("[Debug] -- User account found. --");
                }
                else
                    Console.WriteLine("[Debug] -- User account not found. --");
            }
        }
    }
}
