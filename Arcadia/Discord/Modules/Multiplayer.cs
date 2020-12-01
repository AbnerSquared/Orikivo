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
    public class Multiplayer : ArcadeModule
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
            await Context.Channel.SendMessageAsync(SGameViewer.ViewInvites(Context.Account, _games, --page));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("servers")]
        [Summary("View all currently open game servers.")]
        public async Task ViewServersAsync(int page = 1) // use the page to view through multiple servers, if there is too many to show on one page
        {
            if (!_games.GetServersFor(Context.User.Id, Context.Guild?.Id ?? 0).Any())
            {
                await Context.Channel.SendMessageAsync(Format.Warning("There aren't any public game servers to show.")).ConfigureAwait(false);
                return;
            }

            await Context.Channel.SendMessageAsync(SGameViewer.View(_games.GetServersFor(Context.User.Id, Context.Guild?.Id ?? 0), --page)).ConfigureAwait(false);
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [RequireGuild(AccountHandling.ReadOnly)]
        [Command("localservers")]
        [Summary("View all currently open game servers in this guild.")]
        public async Task ViewLocalServersAsync(int page = 1) // use the page to view through multiple servers, if there is too many to show on one page
        {
            await Context.Channel.SendMessageAsync(SGameViewer.View(_games.GetServersFor(Context.User.Id, Context.Guild.Id), --page)).ConfigureAwait(false);
        }

        [RequireUser]
        [RequireNoSession]
        [Command("hostserver")]
        [Summary("Host a new game server.")]
        public async Task HostServerAsync([Name("game_id")][Summary("The ID of the **Game** that you want to host.")][Tooltip("Type `games` to view the collection games that you can currently host.")]string gameId = null, [Summary("The privacy of the new server.")]Privacy privacy = Privacy.Public)
        {
            if (_games.ReservedUsers.ContainsKey(Context.User.Id))
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You are already in a server.")).ConfigureAwait(false);
                return;
            }

            if (_games.ReservedChannels.ContainsKey(Context.Channel.Id))
            {
                await Context.Channel.SendMessageAsync(Format.Warning("This channel is already dedicated to a server.")).ConfigureAwait(false);
                return;
            }

            if (Check.NotNull(gameId))
            {
                if (!_games.Games.ContainsKey(gameId))
                {
                    await Context.Channel.SendMessageAsync(Format.Warning("Unable to initialize a server for the specified game mode.")).ConfigureAwait(false);
                    return;
                }
            }

            if (Context.Server != null && _games.GetGuildServerCount(Context.Server.Id) >= Context.Server.Config.GameServerLimit)
            {
                await Context.Channel.SendMessageAsync(Format.Warning("This guild already has too many active servers."));
                return;
            }

            await _games.CreateServerAsync(Context.User, Context.Channel, gameId, privacy, Context.Guild?.Id ?? 0).ConfigureAwait(false);
        }

        [RequireUser]
        [RequireNoSession]
        [Command("joinserver")]
        [Summary("Join an existing game server.")]
        public async Task JoinServerAsync([Name("server_id")][Summary("The ID of the server that you wish to join.")][Tooltip("Type `servers` to view a collection of available game servers to join.")]string serverId)
        {
            if (string.IsNullOrWhiteSpace(serverId))
            {
                await Context.Channel.SendMessageAsync("You have to specify a server ID.").ConfigureAwait(false);
                return;
            }

            if (_games.ReservedUsers.ContainsKey(Context.User.Id))
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You are already in a server.")).ConfigureAwait(false);
                return;
            }

            if (Context.Server != null && _games.GetGuildServerCount(Context.Server.Id) >= Context.Server.Config.GameServerLimit)
            {
                if (_games.Servers.ContainsKey(serverId))
                {
                    foreach (ServerConnection connection in _games.Servers[serverId].Connections.Where(x => x.GuildId == Context.Server.Id))
                    {
                        if (await connection.Channel.GetUserAsync(Context.User.Id) != null)
                        {
                            bool isSuccess = await _games.JoinServerAsync(Context.User, connection.Channel, serverId, Context.Guild?.Id ?? 0);
                            await Context.Channel.SendMessageAsync($"> ☑️ You have joined **{_games.Servers[serverId].Name}**. Move over to <#{connection.Channel.Id}>.").ConfigureAwait(false);

                            return;
                        }
                    }
                }

                await Context.Channel.SendMessageAsync(Format.Warning("This guild already has too many server connections."));
                return;
            }

            await _games.JoinServerAsync(Context.User, Context.Channel, serverId, Context.Guild?.Id ?? 0).ConfigureAwait(false);
        }

        [RequireUser]
        [RequireNoSession]
        [Command("quickjoin"), Priority(0)]
        [Summary("Quickly attempts to finds an available game server to join.")]
        public async Task QuickJoinAsync()
        {
            if (_games.Servers.Count == 0)
            {
                await Context.Channel.SendMessageAsync(Format.Warning("Unable to find any available servers.")).ConfigureAwait(false);
                return;
            }

            if (_games.ReservedUsers.ContainsKey(Context.User.Id))
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You are already in a server.")).ConfigureAwait(false);
                return;
            }

            string serverId = _games.GetRandomServer();

            if (Context.Server != null && _games.GetGuildServerCount(Context.Server.Id) >= Context.Server.Config.GameServerLimit)
            {
                if (_games.Servers.ContainsKey(serverId))
                {
                    foreach (ServerConnection connection in _games.Servers[serverId].Connections.Where(x => x.GuildId == Context.Server.Id))
                    {
                        if (await connection.Channel.GetUserAsync(Context.User.Id) != null)
                        {
                            await _games.JoinServerAsync(Context.User, connection.Channel, serverId, Context.Guild?.Id ?? 0).ConfigureAwait(false);
                            return;
                        }
                    }
                }

                await Context.Channel.SendMessageAsync(Format.Warning("This guild already has too many server connections."));
                return;
            }

            await _games.JoinServerAsync(Context.User, Context.Channel, serverId, Context.Guild?.Id ?? 0).ConfigureAwait(false);
        }

        [RequireUser]
        [RequireNoSession]
        [Command("quickjoin"), Priority(1)]
        [Summary("Quickly finds a server to join for the specified **Game**.")]
        public async Task QuickJoinAsync([Name("game_id")][Summary("The ID of the **Game** that you want to join.")]string gameId)
        {
            if (_games.Servers.Count == 0)
            {
                await Context.Channel.SendMessageAsync(Format.Warning("Unable to find any available servers.")).ConfigureAwait(false);
                return;
            }

            if (_games.ReservedUsers.ContainsKey(Context.User.Id))
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You are already in a server.")).ConfigureAwait(false);
                return;
            }

            if (Check.NotNull(gameId))
            {
                if (!_games.Games.ContainsKey(gameId))
                {
                    await Context.Channel.SendMessageAsync(Format.Warning("An invalid game mode was specified.")).ConfigureAwait(false);
                    return;
                }
            }

            string serverId = _games.GetRandomServer(gameId);

            if (Context.Server != null && _games.GetGuildServerCount(Context.Server.Id) >= Context.Server.Config.GameServerLimit)
            {
                if (_games.Servers.ContainsKey(serverId))
                {
                    foreach (ServerConnection connection in _games.Servers[serverId].Connections.Where(x => x.GuildId == Context.Server.Id))
                    {
                        if (await connection.Channel.GetUserAsync(Context.User.Id) != null)
                        {
                            await _games.JoinServerAsync(Context.User, connection.Channel, serverId, Context.Guild?.Id ?? 0).ConfigureAwait(false);
                            return;
                        }
                    }
                }

                await Context.Channel.SendMessageAsync(Format.Warning("This guild already has too many server connections."));
                return;
            }

            await _games.JoinServerAsync(Context.User, Context.Channel, serverId, Context.Guild?.Id ?? 0).ConfigureAwait(false);
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("leaveserver")]
        [Summary("Leave the current server you are in.")]
        public async Task LeaveServerAsync()
        {
            GameServer server = _games.GetServerFor(Context.User.Id);

            if (server == null)
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You are not in a game server."));
                return;
            }

            await server.RemovePlayerAsync(Context.User.Id);
        }

        [RequireUser]
        [RequireAccess(AccessLevel.Dev)]
        [Command("destroyserver")]
        [Summary("Destroys the specified server.")]
        public async Task DestroyServerAsync([Name("server_id")][Summary("The ID of the server to destroy.")]string serverId)
        {
            if (Context.User.Id != Constants.DevId)
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

            await Context.Channel.SendMessageAsync(Format.Warning("Unable to find the specified server.")).ConfigureAwait(false);
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("destroysession")]
        [Summary("Destroys the current session for the specified server, if any.")]
        public async Task DestroySessionAsync([Name("server_id")][Summary("The ID of the server to destroy the session for.")]string serverId)
        {
            if (_games.Servers.ContainsKey(serverId))
            {
                if (_games.Servers[serverId].Session != null)
                {
                    if (Context.User.Id != _games.Servers[serverId].HostId && Context.User.Id != Constants.DevId)
                    {
                        await Context.Channel.SendMessageAsync(Format.Warning("You are not the server host or the developer of this bot.")).ConfigureAwait(false);
                        return;
                    }

                    await _games.DestroySessionAsync(_games.Servers[serverId]).ConfigureAwait(false);
                    await Context.Channel.SendMessageAsync($"The current session for #`{serverId}` has been destroyed.").ConfigureAwait(false);
                    return;
                }

                await Context.Channel.SendMessageAsync(Format.Warning($"The server #`{serverId}` does not have an existing session to destroy.")).ConfigureAwait(false);
                return;
            }

            await Context.Channel.SendMessageAsync(Format.Warning("Unable to find the specified server.")).ConfigureAwait(false);
        }

        [RequireGlobalData]
        [Command("games")]
        [Summary("View the list of all available multiplayer games that a server can play.")]
        public async Task ViewGamesAsync(int page = 1)
        {
            await Context.Channel.SendMessageAsync(SGameViewer.ViewGames(Context.Data.Data, _games, --page, Context.Account));
        }

        [RequireGlobalData]
        [Command("game")]
        [Summary("View all of the proper details for the specified game.")]
        public async Task ViewGameAsync([Name("game_id")][Summary("The ID of the **Game** to view more information for.")]string gameId, int page = 1)
        {
            await Context.Channel.SendMessageAsync(SGameViewer.ViewGame(_games.GetGame(gameId), --page, Context.Account));
        }
    }
}
