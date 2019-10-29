using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo
{
    /// <summary>
    /// The event handler deriving from all events relating to the Discord API.
    /// </summary>
    public class DiscordEventHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _provider;

        private readonly OriJsonContainer _container;
        private readonly OriConsoleService _logger;
        private readonly GameManager _games;
        public DiscordEventHandler(DiscordSocketClient client, CommandService commandService, IServiceProvider provider,
            OriJsonContainer container, OriConsoleService logger, GameManager gameManager)
        {
            Console.WriteLine("-- Initializing event handler. --");
            _client = client;
            _commandService = commandService;
            _provider = provider;

            _container = container;
            _logger = logger;
            _games = gameManager;
            _client.Ready += OnReadyAsync;
            _client.MessageReceived += HandleCommandAsync;
            _client.UserJoined += OnUserJoinAsync;
            _client.ChannelDestroyed += OnChannelDeletedAsync;
            _commandService.CommandExecuted += OnCommandExecutedAsync;
        }

        private async Task OnUserJoinAsync(SocketGuildUser user)
        {
            // Create or get the guild account.
            OriGuild server = _container.GetOrAddGuild(user.Guild);
            // Check if the server has a default role set AND if the bot is allowed to set roles.
            if (server.Options.DefaultRoleId.HasValue) // check for verify
                await user.AddRoleAsync(user.Guild.GetRole(server.Options.DefaultRoleId.Value));
            if (server.Options.UseEvents)
            {
                SocketTextChannel defaultChannel = user.Guild.SystemChannel;
                if (server.Options.SystemChannelId.HasValue)
                    defaultChannel = user.Guild.GetTextChannel(server.Options.SystemChannelId.Value) ?? defaultChannel;

                if (defaultChannel != null && server.Options.Greetings.Count > 0)
                    await defaultChannel.SendMessageAsync(server.Greet(user.Guild, user));
            }
        }

        private async Task OnChannelDeletedAsync(SocketChannel channel)
        {
            Game game = _games.Games.Values.FirstOrDefault(x => x.Receivers.Any(y => y.ChannelId == channel.Id));
            if (game.Receivers.Count - 1 == 0) // if the receiver count - the one about to be deleted results in an empty game.
                await _games.DeleteGameAsync(game.Id);

        }

        private async Task OnReadyAsync()
        {
            _logger.Debug("Orikivo has connected to Discord.");
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {

            _logger.Debug($"Orikivo has {(arg.Author.Id == _client.CurrentUser.Id ? "sent" : "received")} a message.");

            // command syntax
            
            // always ignore bots.
            if (arg.Author.IsBot)
                return;

            SocketUserMessage source = arg as SocketUserMessage;
            OriCommandContext Context = new OriCommandContext(_client, _container, source);
            // TODO: Instead of handling here, if the game manager does have this user,
            //       create a new GameTriggerContext that is then passed into the GameManager as its own internal event.
            if (Context.Account != null)
            {
                Game game = _games.GetGameFrom(Context.User.Id);
                if (game != null)
                {
                    if (game.ContainsChannel(Context.Channel.Id))
                    {
                        _logger.Debug("User sent a message while in a game. Ignoring.");
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

        // Gets the prefix that the client should be looking for.
        private string GetContextPrefix(OriCommandContext Context)
            => Context.Account?.Options.Prefix ?? Context.Server?.Options.Prefix ?? Context.Global.Prefix;

        // PROCESS: 1. Check for cooldowns. 2. Check within custom commands. 3. Attempt to execute, catch if there was an exception error.
        private async Task ExecuteCommandAsync(OriCommandContext Context, int argPos)
        {
            try
            {
                _logger.Debug("Reading command context.");
                string baseMsg = Context.Message.Content.Contains(' ') ? Context.Message.Content.Split(' ')[0] : Context.Message.Content;
                if (Context.Account != null)
                {
                    // check command for cooldowns.
                    OriHelpService helperService = new OriHelpService(_commandService, Context.Global); // Instead of constantly creating this, create a dependency.
                    // in the case of custom command cooldowns, you would need to ignore them here;
                    foreach (KeyValuePair<string, DateTime> pair in Context.Account.GetCooldownsFor(CooldownType.Command))
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

                // Custom Commands: command:name.guild_id
                // custom command logic
                // make sure to apply a global cooldown in the case of a successful custom command.
                // custom command cooldowns can be specified such as: command:yoshikill.456195057373020160
                if (Context.Server != null)
                {
                    if (Context.Server.Options.Commands != null)
                    {
                        foreach (GuildCommand customCommand in Context.Server.Options.Commands)
                        {
                            if (baseMsg == GetContextPrefix(Context) + customCommand.Name)
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

        /// <summary>
        /// These are the set of tasks that are executed when a user successfully calls a command.
        /// </summary>
        private async Task OnCommandExecutedAsync(Optional<CommandInfo> commandInfo, ICommandContext context, IResult result)
        {
            OriCommandContext Context = context as OriCommandContext;

            if (!result.IsSuccess)
            {
                // TODO: Since an Exception is never thrown from here,
                // we need to find a workaround outside of _commandService.ExecuteAsync();
                // A solution could be to execute all commands from a command that catches exceptions to display.
                _logger.Debug("Command.Failed");
                await Context.Channel.ThrowAsync(result.ErrorReason);
                _logger.WriteLine(result.ErrorReason);
                return;
            }

            _logger.Debug("Command.Success");
                
            /* Determine if there's a cooldown to be set. */
            if (commandInfo.IsSpecified)
            {
                CooldownAttribute attribute = commandInfo.Value.Attributes.GetAttribute<CooldownAttribute>();
                if (attribute != null)
                {
                    _logger.Debug("Cooldown found.");
                    Context.Account?.SetCooldown(CooldownType.Command, $"{(Checks.NotNull(commandInfo.Value.Name) ? commandInfo.Value.Name : commandInfo.Value.Module.Group)}+{commandInfo.Value.Priority}", attribute.Seconds);
                }
            }


            // SAVING

            // CONCEPT: Instead of configuring users at handle, we need to be able to track user updates.
            // A solution would be to use a UserManager.UpdateUser(UserProperties), which could change all the values
            // there and keep track of them to place into a list, from which can be invoked for an event.

            OriJsonHandler.Save(Context.Container.Global, "global.json");
            Context.Container.TrySaveGuild(Context.Server);
            Context.Account?.UpdateStat(Stat.CommandsUsed, 1);

            // TODO: Create UserHandler.CompareCriteriaAsync(); 
            //await _merits.CheckUserAsync(Context.Account);





            Context.Container.TrySaveUser(Context.Account);
        }
    }
}
