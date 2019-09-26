﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    // the class that handles all events that can occur on discord
    // this is where commands are executed and whatknot
    public class OriEventHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _provider;

        private readonly OriJsonContainer _container;
        private readonly OriLoggerService _logger;

        public OriEventHandler(DiscordSocketClient client, CommandService commandService, IServiceProvider provider,
            OriJsonContainer container, OriLoggerService logger)
        {
            Console.WriteLine("-- Initializing event handler. --");
            _client = client;
            _commandService = commandService;
            _provider = provider;

            _container = container;
            _logger = logger;

            _client.Ready += OnReadyAsync;
            _client.MessageReceived += HandleCommandAsync;
            _client.UserJoined += OnUserJoinAsync;
            _commandService.CommandExecuted += OnCommandExecutedAsync;
        }

        public async Task OnUserJoinAsync(SocketGuildUser user)
        {
            OriGuild server = _container.GetOrAddGuild(user.Guild);
            if (server.Options.DefaultRoleId.HasValue)
                await user.AddRoleAsync(user.Guild.GetRole(server.Options.DefaultRoleId.Value));
            if (server.Options.AllowGreeting)
            {
                SocketTextChannel defaultChannel = user.Guild.SystemChannel;
                if (server.Options.SystemChannelId.HasValue)
                    defaultChannel = user.Guild.GetTextChannel(server.Options.SystemChannelId.Value) ?? defaultChannel;

                if (defaultChannel != null)
                    await defaultChannel.SendMessageAsync(server.Greet(user));
            }
        }

        public async Task OnReadyAsync()
        {
            _logger.Debug("Orikivo has connected to Discord.");
        }

        public async Task HandleCommandAsync(SocketMessage arg)
        {
            // logging
            if (arg.Author.Id == _client.CurrentUser.Id)
                _logger.Debug("Orikivo has sent a message.");
            else
                _logger.Debug("Orikivo has received a message.");

            // command syntax
            
            // always ignore bots.
            if (arg.Author.IsBot)
                return;

            SocketUserMessage source = arg as SocketUserMessage;
            OriCommandContext Context = new OriCommandContext(_client, _container, source);

            if (Context.Account != null)
            {
                if (Context.Global.Lobbies.Any(x => x.Users.Any(y => y.Id == Context.Account.Id)))
                {
                    OriLobbyInvoker lobby = Context.Global.Lobbies.First(x => x.Users.Any(y => y.Id == Context.Account.Id));
                    if (lobby.Receivers.Any(x => x.ChannelId == Context.Channel.Id))
                    {
                        _logger.Debug("User in lobby. Closing event.");
                        return;
                    }
                }
            }

            _logger.Debug("Now comparing prefixes.");
            int i = 0;
            if (source.HasMentionPrefix(_client.CurrentUser, ref i))
            {
                _logger.Debug("Orikivo called by mention.");
                await ExecuteCommandAsync(Context, i);
                return;
            }
            // order of prefix importance: userPrefix > serverPrefix > globalPrefix > defaultPrefix
            if (source.HasStringPrefix(GetContextPrefix(Context), ref i))
            {
                _logger.Debug("Orikivo called by prefix.");
                await ExecuteCommandAsync(Context, i);
            }
            else
                _logger.Debug("Incorrect prefix used. Now closing.");
        }

        private string GetContextPrefix(OriCommandContext Context)
        {
            string prefix = Context.Global.Prefix;
            if (Context.Server != null)
            {
                if (Context.Server.Options.HasPrefix)
                    prefix = Context.Server.Options.Prefix;
            }
            if (Context.Account != null)
            {
                if (Context.Account.Options.HasPrefix)
                    prefix = Context.Account.Options.Prefix;
            }

            return prefix;
        }

        public async Task ExecuteCommandAsync(OriCommandContext Context, int argPos)
        {
            try
            {
                _logger.Debug("Reading command context.");
                string baseMsg = Context.Message.Content.Contains(' ') ? Context.Message.Content.Split(' ')[0] : Context.Message.Content;
                if (Context.Account != null)
                {
                    // check command for cooldowns.
                    OriHelpService helperService = new OriHelpService(_commandService);
                    // in the case of custom command cooldowns, you would need to ignore them here;
                    foreach (KeyValuePair<string, CooldownInfo> pair in Context.Account.GetCommandCooldowns())
                    {
                        // could probably make static
                        List<string> aliases = helperService.GetAliases(pair.Key.Substring("command:".Length));
                        if (aliases.Count == 0)
                            aliases.Add(pair.Key.Substring("command:".Length));
                        
                        foreach (string alias in aliases)
                        {
                            if (baseMsg == GetContextPrefix(Context) + alias)
                            {
                                if (Context.Account.IsOnCooldown(pair.Key))
                                {
                                    await Context.Channel.WarnCooldownAsync(Context.Account, pair);
                                    return;
                                }
                            }
                        }
                    }
                }

                // custom command logic
                // make sure to apply a global cooldown in the case of a successful custom command.
                // custom command cooldowns can be specified such as: command:yoshikill.456195057373020160
                if (Context.Server != null)
                {
                    foreach (CustomGuildCommand customCommand in Context.Server.CustomCommands)
                    {
                        foreach(string alias in customCommand.Aliases.Append(customCommand.Name))
                        {
                            if (baseMsg == GetContextPrefix(Context) + alias)
                            {
                                if (customCommand.Message == null)
                                    break;
                                await Context.Channel.SendMessageAsync(customCommand.Message.Build());
                                return;
                            }
                        }
                    }
                }

                // if not on a cooldown OR has a custom command within, proceed with default execution.
                IResult task = await _commandService.ExecuteAsync(Context, argPos, _provider, MultiMatchHandling.Exception);
            }
            catch(Exception ex)
            {
                // the problem is that command server automatically catches the command service.
                // this is never utilized
                await Context.Channel.CatchAsync(ex);
            }
        }

        // this is used when a command is executed.
        // used to save OriGuild and OriUser to the actual computer if they exist
        public async Task OnCommandExecutedAsync(Optional<CommandInfo> commandInfo, ICommandContext context, IResult result)
        {
            if (result.IsSuccess)
            {
                _logger.Debug("Command.Success");
                OriCommandContext Context = context as OriCommandContext;

                // checking for cooldowns
                if (commandInfo.IsSpecified)
                {
                    Attribute attribute = commandInfo.Value.Attributes.Where(x => x.GetType() == typeof(CooldownAttribute)).FirstOrDefault();
                    if (attribute != null)
                    {
                        _logger.Debug("Cooldown found.");
                        if (Context.Account != null)
                        {
                            Context.Account.SetCooldown($"command:{commandInfo.Value.Name}", (attribute as CooldownAttribute).Seconds);
                        }
                    }
                }

                // saving/updating accounts if they need to
                Context.Container.Global = Context.Global; // set the root global to the current global, if updated.
                OriJsonHandler.Save(Context.Container.Global, "global.json"); // if it was updated; want to conserve amount of memory used.
                if (Context.Server != null)
                {
                    Context.Container.SaveGuild(Context.Server); // if it was updated; want to conserve amount of memory used.
                }
                if (Context.Account != null)
                {
                    Context.Account.UpdateStat(Stat.CommandsUsed, 1);
                    Context.Container.SaveUser(Context.Account);
                }
                // if it was updated; want to conserve amount of memory used.
            }
            else
            {
                _logger.Debug("Command.Failed");
                await (context as OriCommandContext).Channel.ThrowAsync(result.ErrorReason);
                Console.WriteLine(result.ErrorReason);
            }
        }
    }
}
