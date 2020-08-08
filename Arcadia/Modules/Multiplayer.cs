using System.Collections.Generic;
using System.Linq;
using Discord.Commands;
using Orikivo;
using System.Text;
using System.Threading.Tasks;
using Arcadia.Multiplayer;
using Orikivo.Text;

namespace Arcadia.Modules
{
    public static class ServerBrowser
    {
        // Make sure to include servers from which the user was invited to.
        public static string View(IEnumerable<GameServer> servers, int page = 0, int pageSize = 6)
        {
            var browser = new StringBuilder();

            browser.Append($"> 🔍 **Server Browser**");

            int pageCount = Paginate.GetPageCount(servers.Count(), pageSize);

            if (pageCount > 1)
                browser.Append($" (Page {page + 1:##,0}/{pageCount:##,0})");

            browser.AppendLine();
            browser.AppendLine("> View all of the public game servers.");

            foreach (GameServer server in Paginate.GroupAt(servers, page, pageSize))
                browser.AppendLine(WriteServerInfo(server));

            return browser.ToString();
        }

        public static string WriteServerInfo(GameServer server)
        {
            var lobby = new StringBuilder();

            lobby.AppendLine($"> `{server.Id}` • **{server.Config.Name}** (**{server.Players.Count:##,0}** {Format.TryPluralize("player", server.Players.Count)})");
            lobby.AppendLine($"> {WriteGameName(server)}: {WriteActivity(server)}");
            return lobby.ToString();
        }

        private static string WriteGameName(GameServer server)
        {
            if (server.Config.IsValidGame(server.Config.GameId))
                return GameManager.GetGame(server.Config.GameId).Details.Name;

            return "Unknown Game";
        }

        private static string WriteActivity(GameServer server)
        {
            if (server.Session != null)
                return server.Session.ActivityDisplay;

            return "In Lobby";
        }
    }

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
            if (!_games.GetPublicServers().Any())
            {
                await Context.Channel.WarnAsync("There aren't any public game servers to show.");
                return;
            }

            var servers = new StringBuilder();

            await Context.Channel.SendMessageAsync(ServerBrowser.View(_games.GetPublicServers(), page - 1));
        }

        [Command("hostserver")]
        [Summary("Host a new game server.")]
        public async Task HostServerAsync([Name("game_id")]string gameId = null)
        {
            if (_games.ReservedUsers.ContainsKey(Context.User.Id))
            {
                await Context.Channel.WarnAsync("You are already in a server.");
                return;
            }

            if (_games.ReservedChannels.ContainsKey(Context.Channel.Id))
            {
                await Context.Channel.WarnAsync("This channel is already dedicated to a server.");
                return;
            }

            if (Check.NotNull(gameId))
            {
                if (!GameManager.Games.ContainsKey(gameId))
                {
                    await Context.Channel.WarnAsync("Unable to initialize a server for the specified game mode");
                    return;
                }
            }

            await _games.CreateServerAsync(Context.User, Context.Channel, gameId);
        }

        [Command("joinserver")]
        [Summary("Join an existing game server.")]
        public async Task JoinServerAsync([Name("server_id")]string serverId)
        {
            if (string.IsNullOrWhiteSpace(serverId))
            {
                await Context.Channel.SendMessageAsync("You have to specify a server ID.");
                return;
            }

            if (_games.ReservedUsers.ContainsKey(Context.User.Id))
            {
                await Context.Channel.WarnAsync("You are already in a server.");
                return;
            }

            await _games.JoinServerAsync(Context.User, Context.Channel, serverId);
        }

        [Command("quickjoin"), Priority(0)]
        [Summary("Quickly attempts to finds a game to join.")]
        public async Task QuickJoinAsync()
        {
            if (_games.Servers.Count == 0)
            {
                await Context.Channel.WarnAsync("Unable to find any available servers.");
                return;
            }

            if (_games.ReservedUsers.ContainsKey(Context.User.Id))
            {
                await Context.Channel.WarnAsync("You are already in a server.");
                return;
            }

            await _games.JoinServerAsync(Context.User, Context.Channel, _games.GetRandomServer());
        }

        [Command("quickjoin"), Priority(1)]
        [Summary("Quickly finds a server to join for the specified game mode.")]
        public async Task QuickJoinAsync([Name("game_id")]string gameId)
        {
            if (_games.Servers.Count == 0)
            {
                await Context.Channel.WarnAsync("Unable to find any available servers.");
                return;
            }

            if (_games.ReservedUsers.ContainsKey(Context.User.Id))
            {
                await Context.Channel.WarnAsync("You are already in a server.");
                return;
            }

            if (Check.NotNull(gameId))
            {
                if (!GameManager.Games.ContainsKey(gameId))
                {
                    await Context.Channel.WarnAsync("An invalid game mode was specified.");
                    return;
                }
            }

            await _games.JoinServerAsync(Context.User, Context.Channel, _games.GetRandomServer(gameId));
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
        public async Task DestroyServerAsync([Name("server_id")] string serverId)
        {
            if (Context.User.Id != OriGlobal.DevId)
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You are not the developer of this bot."));
                return;
            }

            if (_games.Servers.ContainsKey(serverId))
            {
                await _games.DestroyServerAsync(_games.Servers[serverId]);
                await Context.Channel.SendMessageAsync($"The specified server #`{serverId}` has been destroyed.");
                return;
            }

            await Context.Channel.WarnAsync("Unable to find the specified server.");
        }

        [Command("destroysession")]
        [Summary("Destroys the current session for the specified server, if any.")]
        public async Task DestroySessionAsync([Name("server_id")] string serverId)
        {
            if (Context.User.Id != OriGlobal.DevId)
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You are not the developer of this bot."));
                return;
            }

            if (_games.Servers.ContainsKey(serverId))
            {
                if (_games.Servers[serverId].Session != null)
                {
                    await _games.DestroySessionAsync(_games.Servers[serverId]);
                    await Context.Channel.SendMessageAsync($"The current session for #`{serverId}` has been destroyed.");
                    return;
                }

                await Context.Channel.WarnAsync($"The server #`{serverId}` does not have an existing session to destroy.");
                return;
            }

            await Context.Channel.WarnAsync("Unable to find the specified server.");
        }
    }
}