using Discord.Commands;
using Orikivo;
using System.Text;
using System.Threading.Tasks;

namespace Arcadia
{
    [Name("Multiplayer")]
    [Summary("Come play with others.")]
    public class Multiplayer : OriModuleBase<OriCommandContext>
    {
        private readonly GameManager _games;

        public Multiplayer(GameManager games)
        {
            _games = games;
        }

        [Command("servers")]
        [Summary("View all currently open game servers.")]
        public async Task ViewServersAsync()
        {
            if (_games.Servers.Count == 0)
            {
                await Context.Channel.SendMessageAsync("There aren't any public game servers to show.");
                return;
            }

            var servers = new StringBuilder();
            foreach ((string key, GameServer server) in _games.Servers)
            {
                servers.AppendLine(server.Id);
            }

            await Context.Channel.SendMessageAsync(servers.ToString());
        }

        [Command("hostserver")]
        [Summary("Host a new game server.")]
        public async Task HostServerAsync()
        {
            if (_games.ReservedUsers.ContainsKey(Context.User.Id))
            {
                await Context.Channel.SendMessageAsync("You are already in a server. You must leave the current one before you can host.");
                return;
            }

            await _games.CreateServerAsync(Context.User, Context.Channel);
            await Context.Channel.SendMessageAsync("You have created your server.");
        }
    }
}