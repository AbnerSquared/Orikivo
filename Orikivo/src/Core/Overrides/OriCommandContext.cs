using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Orikivo.Desync;
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
        //public User Account { get; internal set; }
        public User Account
        {
            get
            {
                Container.TryGetUser(User.Id, out User u);
                return u;
            }
            set => Container.AddOrUpdateUser(User.Id, value);
        }

        public Husk Husk
        {
            get => Account?.Husk;
        }

        //public OriGuild Server { get; internal set; }
        public OriGuild Server
        {
            get
            {
                Container.TryGetGuild(Guild.Id, out OriGuild s);
                return s;
            }
            set => Container.AddOrUpdateGuild(Guild.Id, value);
        }

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
                Container.GetOrAddGuild(Guild);
                Server.TryUpdateName(Guild.Name);
                Server.TryUpdateOwner(Guild.OwnerId);
                Console.WriteLine("[Debug] -- Guild account found or built. --");
            }
            Console.WriteLine($"[Debug] -- User {(Account == null ? "does not have an" : "has an")} account. --");
        }
    }
}
