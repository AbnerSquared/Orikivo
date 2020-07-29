using Discord.Commands;
using Orikivo;
using System.Text;
using System.Threading.Tasks;

namespace Arcadia.Modules
{

    [Name("Multiplayer")]
    [Summary("Come play with others.")]
    public class Multiplayer : OriModuleBase<ArcadeContext>
    {
        private readonly GameManager _games;

        public Multiplayer(GameManager games)
        {
            _games = games;
        }

        [Command("servers")]
        [Summary("View all currently open game servers.")]
        public async Task ViewServersAsync(int page = 1) // use the page to view through multiple servers, if there is too many to show on one page
        {
            if (_games.Servers.Count == 0)
            {
                await Context.Channel.SendMessageAsync("There aren't any public game servers to show.");
                return;
            }

            var servers = new StringBuilder();
            foreach (GameServer server in _games.Servers.Values)
            {
                servers.AppendLine($"{server.Id} | {server.Config.Title} ({server.Players.Count} {Format.TryPluralize("player", server.Players.Count)})");
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

            await _games.CreateServerAsync(Context.User, Context.Channel, Context.Guild);
        }

        [Command("joinserver")]
        [Summary("Join an existing game server.")]
        public async Task JoinServerAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                await Context.Channel.SendMessageAsync("You have to specify an ID.");
                return;
            }

            await _games.JoinServerAsync(Context.User, Context.Channel, id);
        }

        /*
        [Command("leaveserver")]
        [Summary("Leave the current server you are in.")]
        public async Task LeaveServerAsync()
        {

        }*/

        //[Access(AccessLevel.Dev)]
        [Command("destroyserver")]
        [Summary("Destroys the specified server.")]
        public async Task DestroyServerAsync(string id)
        {
            if (Context.User.Id != OriGlobal.DevId)
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You are not the developer of this bot."));
                return;
            }

            if (_games.Servers.ContainsKey(id))
            {
                await _games.DestroyServerAsync(_games.Servers[id]);
                await Context.Channel.SendMessageAsync("The specified server has been destroyed.");
            }
            else
            {
                await Context.Channel.SendMessageAsync("Could not find the specified server.");
            }
        }
    }
}