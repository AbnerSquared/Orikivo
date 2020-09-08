using System.Linq;
using Discord.Commands;
using Orikivo;
using System.Threading.Tasks;
using Arcadia.Multiplayer;

namespace Arcadia.Modules
{
    [Icon("⚔️")]
    [Name("Multiplayer")]
    [Summary("Come play with others.")]
    public class Multiplayer : OriModuleBase<ArcadeContext>
    {
        private readonly GameManager _games;

        public Multiplayer(GameManager games)
        {
            _games = games;
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("invites")]
        [Summary("View all of your current server invites.")]
        public async Task ViewInvitesAsync(int page = 1)
        {
            page--;
            await Context.Channel.SendMessageAsync(ServerBrowser.ViewInvites(Context.Account, _games, page));
        }

        [Command("servers")]
        [Summary("View all currently open game servers.")]
        public async Task ViewServersAsync(int page = 1) // use the page to view through multiple servers, if there is too many to show on one page
        {
            if (!_games.GetServersFor(Context.User.Id, Context.Guild?.Id ?? 0).Any())
            {
                await Context.Channel.WarnAsync("There aren't any public game servers to show.").ConfigureAwait(false);
                return;
            }

            await Context.Channel.SendMessageAsync(ServerBrowser.View(_games.GetServersFor(Context.User.Id, Context.Guild?.Id ?? 0), page - 1)).ConfigureAwait(false);
        }

        [RequireUser]
        [Command("hostserver")]
        [Summary("Host a new game server.")]
        public async Task HostServerAsync([Name("game_id")]string gameId = null)
        {
            if (_games.ReservedUsers.ContainsKey(Context.User.Id))
            {
                await Context.Channel.WarnAsync("You are already in a server.").ConfigureAwait(false);
                return;
            }

            if (_games.ReservedChannels.ContainsKey(Context.Channel.Id))
            {
                await Context.Channel.WarnAsync("This channel is already dedicated to a server.").ConfigureAwait(false);
                return;
            }

            if (Check.NotNull(gameId))
            {
                if (!GameManager.Games.ContainsKey(gameId))
                {
                    await Context.Channel.WarnAsync("Unable to initialize a server for the specified game mode.").ConfigureAwait(false);
                    return;
                }
            }

            await _games.CreateServerAsync(Context.User, Context.Channel, gameId).ConfigureAwait(false);
        }

        [Command("joinserver")]
        [Summary("Join an existing game server.")]
        public async Task JoinServerAsync([Name("server_id")]string serverId)
        {
            if (string.IsNullOrWhiteSpace(serverId))
            {
                await Context.Channel.SendMessageAsync("You have to specify a server ID.").ConfigureAwait(false);
                return;
            }

            if (_games.ReservedUsers.ContainsKey(Context.User.Id))
            {
                await Context.Channel.WarnAsync("You are already in a server.").ConfigureAwait(false);
                return;
            }

            await _games.JoinServerAsync(Context.User, Context.Channel, serverId).ConfigureAwait(false);
        }

        [Command("quickjoin"), Priority(0)]
        [Summary("Quickly attempts to finds a game to join.")]
        public async Task QuickJoinAsync()
        {
            if (_games.Servers.Count == 0)
            {
                await Context.Channel.WarnAsync("Unable to find any available servers.").ConfigureAwait(false);
                return;
            }

            if (_games.ReservedUsers.ContainsKey(Context.User.Id))
            {
                await Context.Channel.WarnAsync("You are already in a server.").ConfigureAwait(false);
                return;
            }

            await _games.JoinServerAsync(Context.User, Context.Channel, _games.GetRandomServer()).ConfigureAwait(false);
        }

        [Command("quickjoin"), Priority(1)]
        [Summary("Quickly finds a server to join for the specified game mode.")]
        public async Task QuickJoinAsync([Name("game_id")]string gameId)
        {
            if (_games.Servers.Count == 0)
            {
                await Context.Channel.WarnAsync("Unable to find any available servers.").ConfigureAwait(false);
                return;
            }

            if (_games.ReservedUsers.ContainsKey(Context.User.Id))
            {
                await Context.Channel.WarnAsync("You are already in a server.").ConfigureAwait(false);
                return;
            }

            if (Check.NotNull(gameId))
            {
                if (!GameManager.Games.ContainsKey(gameId))
                {
                    await Context.Channel.WarnAsync("An invalid game mode was specified.").ConfigureAwait(false);
                    return;
                }
            }

            await _games.JoinServerAsync(Context.User, Context.Channel, _games.GetRandomServer(gameId)).ConfigureAwait(false);
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
                await Context.Channel.SendMessageAsync(Format.Warning("You are not the developer of this bot.")).ConfigureAwait(false);
                return;
            }

            if (_games.Servers.ContainsKey(serverId))
            {
                await _games.DestroyServerAsync(_games.Servers[serverId]).ConfigureAwait(false);
                await Context.Channel.SendMessageAsync($"The specified server #`{serverId}` has been destroyed.").ConfigureAwait(false);
                return;
            }

            await Context.Channel.WarnAsync("Unable to find the specified server.").ConfigureAwait(false);
        }

        [Command("destroysession")]
        [Summary("Destroys the current session for the specified server, if any.")]
        public async Task DestroySessionAsync([Name("server_id")] string serverId)
        {
            if (Context.User.Id != OriGlobal.DevId)
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You are not the developer of this bot.")).ConfigureAwait(false);
                return;
            }

            if (_games.Servers.ContainsKey(serverId))
            {
                if (_games.Servers[serverId].Session != null)
                {
                    await _games.DestroySessionAsync(_games.Servers[serverId]).ConfigureAwait(false);
                    await Context.Channel.SendMessageAsync($"The current session for #`{serverId}` has been destroyed.").ConfigureAwait(false);
                    return;
                }

                await Context.Channel.WarnAsync($"The server #`{serverId}` does not have an existing session to destroy.").ConfigureAwait(false);
                return;
            }

            await Context.Channel.WarnAsync("Unable to find the specified server.").ConfigureAwait(false);
        }
    }
}
