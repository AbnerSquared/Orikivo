using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Orikivo.Utility;
using Orikivo.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo.Logging
{
    /// <summary>
    /// Represents a Manager that handles all events that occur on Orikivo.
    /// </summary>
    public class EventManager
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _service;
        private readonly DataContainer _data;
        private readonly IServiceProvider _provider;
        private readonly IConfigurationRoot _config;

        public EventManager(DiscordSocketClient client, DataContainer data,
            CommandService service, IServiceProvider provider, IConfigurationRoot config)
        {
            _client = client;
            _service = service;
            _data = data;
            _provider = provider;
            _config = config;

            SyncEvents();
        }

        /// <summary>
        /// A method used to link all event occurrences to the client.
        /// </summary>
        private void SyncEvents()
        {
            _client.Ready += ReadyAsync;
            _client.Connected += ConnectAsync;

            _client.MessageReceived += CommandAsync;
        }

        public async Task ReadyAsync()
        {
            Logger.Log($"Orikivo", "Client.Ready");
            // Enable once ready....
            await (new LockedDblWrapper(_client.CurrentUser.Id, _config["api:dbl"])).UpdateStatsAsync(_client.Guilds.Count);
        }

        public async Task ConnectAsync()
        {
            Logger.Log("Orikivo", "Client.Connected");
        }

        public async Task DisconnectAsync()
        {
            Logger.Log("Orikivo", "Client.Disconnected");
        }

        // Whenever Orikivo updates in terms of username, avatar, status, etc.
        public async Task ClientUpdateAsync(SocketSelfUser p, SocketSelfUser n)
        {
            // Add user differences
            Logger.Log("Orikivo", "Client.Updated");
        }

        public async Task ClientGuildJoinAsync(SocketGuild g)
        {
            Logger.Log("Orikivo", $"Client.Guild.Joined({g.Name})");
        }

        public async Task ClientGuildLeaveAsync(SocketGuild g)
        {
            Logger.Log("Orikivo", $"Client.Guild.Left({g.Name})");
        }

        private bool EnsureAuthor(SocketMessage arg)
        {
            if (arg.Author.IsBot)
                return false;
            return true;
        }

        // Used for determining what a command is.
        public async Task CommandAsync(SocketMessage arg)
        {
            if (!EnsureAuthor(arg))
                return;

            SocketUserMessage source = arg as SocketUserMessage;
            OrikivoCommandContext Context = new OrikivoCommandContext(_client, _data, source);

            // check if the server has blacklisted the author

            // always check mentions first.

            int i = 0;//pfx.LengthAsIndex();

            if (source.HasMentionPrefix(_client.CurrentUser, ref i))
            {
                await ExecuteAsync(Context, i);
                return;
            }

            // if the server allows prefixes, continue checking.
            if (Context.Server.Config.UsePrefixes)
            {
                string pfx = Context.Server.Config.Prefix;
                string pfxc = Context.Server.Config.ClosingPrefix;

                // pfx{command}
                // pfx|d|pfxc{command}
                bool ends = arg.Content.Contains(pfxc);
                if (ends)
                {
                    i = arg.Content.IndexOf(pfxc);
                    bool b = Context.HasPermission(ChannelPermission.ManageMessages);

                    // pfx|i|pfxc{command}
                    string pfxd = $"{pfx}d{pfxc}";

                    if (source.HasStringPrefix(pfxd, ref i))
                    {
                        if (b)
                            await source.DeleteAsync();
                        await ExecuteAsync(Context, i);
                        return;
                    }
                }
                if (source.HasStringPrefix(pfx, ref i))
                    await ExecuteAsync(Context, i);
            }

            // check if the server allows prefixes.

            

            // check if they match a sub-parameter.

            // catch errors, and ignore the problems.

            // check to see if the message correctly parses any command structure.
            
        }

        public async Task ExecuteAsync(OrikivoCommandContext Context, int argPos)
        {
            IResult task = await _service.ExecuteAsync(Context, argPos, _provider, MultiMatchHandling.Exception);
            if (!task.IsSuccess)
            {
                if (task.Error.HasValue)
                    if (task.Error == CommandError.UnknownCommand)
                        if (!Context.Server.Config.ThrowUnknown)
                            return;

                if (Context.Server.Config.Throw)
                    await Context.Channel.ThrowAsync(task.ErrorReason.MarkdownBold());
            }
        }
    }
}
