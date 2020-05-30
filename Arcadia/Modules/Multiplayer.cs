using Discord.Commands;
using Orikivo;
using System.Threading.Tasks;

namespace Arcadia
{
    [Name("Multiplayer")]
    [Summary("Come play with others.")]
    public class Multiplayer : OriModuleBase<OriCommandContext>
    {
        [Command("servers")]
        [Summary("View all currently open game servers.")]
        public async Task ViewServersAsync()
        {

        }

        [Command("hostserver")]
        [Summary("Host a new game server.")]
        public async Task HostServerAsync()
        {

        }
    }
}