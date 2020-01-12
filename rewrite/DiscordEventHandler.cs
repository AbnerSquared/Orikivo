using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Orikivo.Unstable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo
{
    // could store all of the resource for Orikivo.
    public class Cache
    {
        public static Cache Load()
        {
            // use JsonHandler to check in assets and retrieve everything within that.
            return null;
        }

        public IReadOnlyList<Merit> Merits { get; private set; }
    }

    /// <summary>
    /// The event handler deriving from all events relating to the Discord API.
    /// </summary>
    public partial class DiscordEventHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _provider;
        //private readonly Cache _cache;
        private readonly OriJsonContainer _container;
        //private readonly LogService _logger;
        private readonly GameManager _games;

        public DiscordEventHandler(DiscordSocketClient client, CommandService commandService, IServiceProvider provider,
            /*Cache cache,*/ OriJsonContainer container, /*LogService logger,*/ GameManager gameManager)
        {
            Console.WriteLine("-- Initializing event handler. --");
            _client = client;
            _commandService = commandService;
            _provider = provider;
            //_cache = cache;
            _container = container;
            //_logger = logger;
            _games = gameManager;
            _client.Ready += OnReadyAsync;
            _client.MessageReceived += CheckCommandAsync;
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
            //_logger.Debug("Orikivo has connected to Discord.");
        }

        private async Task CheckCommandAsync(SocketMessage arg)
        {
            Console.WriteLine($"Checking command...");
            //_logger.Debug($"Orikivo has {(arg.Author.Id == _client.CurrentUser.Id ? "sent" : "received")} a message.");

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
                        //_logger.Debug("User sent a message while in a game. Ignoring.");
                        return;
                    }
                }
            }

            // Prefix checker.
            //_logger.Debug("Now comparing prefixes.");
            int i = 0;
            if (source.HasMentionPrefix(_client.CurrentUser, ref i))
            {
                //_logger.Debug("Orikivo called by mention.");
                await ExecuteCommandAsync(Context, i);
                return;
            }
            i = 2;
            // order of prefix importance: userPrefix > serverPrefix > globalPrefix > defaultPrefix
            if (source.HasStringPrefix(GetContextPrefix(Context) + "d]", ref i))
            {
                //_logger.Debug("Orikivo called by prefix.");
                await ExecuteCommandAsync(Context, i);
                await Context.Message.DeleteAsync();
                return;
            }
            i = 0;
            if (source.HasStringPrefix(GetContextPrefix(Context), ref i))
            {
                //_logger.Debug("Orikivo called by prefix.");
                await ExecuteCommandAsync(Context, i);
            }
            //else
                //_logger.Debug("Incorrect prefix used. Now closing.");
        }

        // Gets the prefix that the client should be looking for.
        private string GetContextPrefix(OriCommandContext Context)
            => Context.Account?.Config.Prefix ?? Context.Server?.Options.Prefix ?? Context.Global.Prefix;

        // PROCESS: 1. Check for cooldowns. 2. Check within custom commands. 3. Attempt to execute, catch if there was an exception error.
        
    }

    
}
