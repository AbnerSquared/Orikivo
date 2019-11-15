using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    /// <summary>
    /// A custom command context that supports JSON container data and accounts.
    /// </summary>
    public class OriCommandContext : SocketCommandContext
    {
        public OriUser Account { get; internal set; } // TODO: Override SocketUser with OriUser variant.
        public OriGuild Server { get; }
        public OriGlobal Global { get; }
        public OriJsonContainer Container { get; }

        // public AccountUpdatePacket Packet { get; }

        public OriCommandContext(DiscordSocketClient client, OriJsonContainer container, SocketUserMessage msg) : base(client, msg)
        {
            Console.WriteLine("[Debug] -- Constructing command context. --");
            Container = container; // ensured in container.
            Global = Container.Global;
            if (Guild != null)
            {
                Server = Container.GetOrAddGuild(Guild);
                Server.TryUpdateName(Guild.Name);
                Server.TryUpdateOwner(Guild.OwnerId);
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
