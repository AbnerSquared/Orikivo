using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Orikivo.Desync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo
{
    // TODO: Implement ignoring when a MessageCollector is currently in use for a user.
    public class CommandHandler
    {
        private readonly CommandService _service;
        private readonly DiscordSocketClient _client;
        private readonly LogService _logger;
        private readonly OriJsonContainer _container;
        private readonly IServiceProvider _provider;

        public CommandHandler(DiscordSocketClient client, CommandService service, OriJsonContainer container,
            IServiceProvider provider, LogService logger)
        {
            _client = client;
            _service = service;
            _container = container;
            _provider = provider;
            _logger = logger;
            _client.MessageReceived += ReadInputAsync;
            _service.CommandExecuted += OnExecutedAsync;
        }

        public string GetPrefix(OriCommandContext ctx)
            => ctx.Account?.Config.Prefix ??
               ctx.Server?.Options.Prefix ??
               ctx.Global.Prefix;

        public async Task ReadInputAsync(SocketMessage arg)
        {
            // Ignore bots
            if (arg.Author.IsBot)
                return;

            // Set up initial values
            SocketUserMessage source = arg as SocketUserMessage;
            OriCommandContext ctx = new OriCommandContext(_client, _container, source);

            // TODO: Handle message filters here to prevent additional command execution within another listener

            /*
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
             */

            // Check all possible prefix formats
            int i = 0;
            if (source.HasMentionPrefix(_client.CurrentUser, ref i))
            {
                await ExecuteAsync(ctx, i);
                return;
            }

            // Delete the message after executing the command
            i = 2;
            if (source.HasStringPrefix(GetPrefix(ctx) + "d]", ref i))
            {
                await ExecuteAsync(ctx, i);
                // TODO: Make a permissions check before attempting to delete
                await ctx.Message.DeleteAsync();
                return;
            }

            // Execute the command
            i = 0;
            if (source.HasStringPrefix(GetPrefix(ctx), ref i))
                await ExecuteAsync(ctx, i);
        }

        public async Task ExecuteAsync(OriCommandContext ctx, int argPos)
        {
            // TODO: Handle global cooldowns

            // TODO: Handle command cooldowns

            // TODO: Handle custom guild command references

            // TODO: Handle option parsing for commands

            // TODO: It might be required to create a custom parser and execution service separate from CommandService in order to properly
            // allow specific parsing methods
            await _service.ExecuteAsync(ctx, argPos, _provider, MultiMatchHandling.Exception);
        }

        private async Task OnExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            OriCommandContext ctx = context as OriCommandContext;

            // Attempt to set a global cooldown on the account that executed this command

            // If the command failed
            if (!result.IsSuccess)
            {
                if (result is ExecuteResult)
                {
                    if (!result.IsSuccess)
                        await ctx.Channel.CatchAsync(((ExecuteResult)result).Exception);
                }
                else
                    await ctx.Channel.ThrowAsync(result.ErrorReason);

                return;
            }

            // Otherwise
            if (command.IsSpecified)
            {
                // Handle all data
                await UpdateAsync(command.Value, ctx);
            }
        }

        // update all accounts and users accordingly based on the command
        private async Task UpdateAsync(CommandInfo command, OriCommandContext ctx)
        {
            // Manage or update cooldowns
            CooldownAttribute cooldown = command.Attributes.GetAttribute<CooldownAttribute>();

            if (cooldown != null)
            {
                string id = ContextNode.GetId(command, true);
                ctx.Account?.SetCooldown(CooldownType.Command, id, cooldown.Duration);
            }

            if (ctx.Account?.Husk != null)
            {
                WorldEngine.CanMove(ctx.Account, ctx.Account.Husk);
            }

            bool notified = ctx.Account?.Notifier.LastNotified.HasValue ?? false;

            if (notified)
            {
                _logger.Debug("User was notified.");
                ctx.Account.Notifier.LastNotified = null;
            }
            // Clear all expired boosters and such
            // TODO: Create expiration check

            // Check if the user was updated or doesn't exist to save
            RequireUserAttribute requireUser = command.Preconditions.GetAttribute<RequireUserAttribute>();

            if (requireUser?.Handling.EqualsAny(AccountHandling.ReadWrite, AccountHandling.WriteOnly) ?? false
                || cooldown != null
                || notified)
            {
                // Check if any stats now meet certain criteria
                CheckStats(ctx, ctx.Account);

                //Console.WriteLine("User updated. Now saving...");
                _logger.Debug("User updated. Now saving...");
                ctx.Container.TrySaveUser(ctx.Account);

                
            }
            else if (!JsonHandler.JsonExists<User>(ctx.User.Id))
            {
                if (ctx.Account == null)
                    ctx.Container.GetOrAddUser(ctx.User);

                ctx.Container.TrySaveUser(ctx.Account);
            }

            // Check if the guild was updated or doesn't exist to save
            RequireGuildAttribute requireGuild = command.Preconditions.GetAttribute<RequireGuildAttribute>();
            if (requireGuild?.Handling.EqualsAny(AccountHandling.ReadWrite, AccountHandling.WriteOnly) ?? false)
            {
                ctx.Container.TrySaveGuild(ctx.Server);
            }
            else if (!JsonHandler.JsonExists<OriGuild>(ctx.Guild.Id))
            {
                if (ctx.Guild == null)
                    ctx.Container.GetOrAddGuild(ctx.Guild);

                ctx.Container.TrySaveGuild(ctx.Server);
            }

            // For now, just save global data until a workaround is found
            JsonHandler.Save(ctx.Container.Global, "global.json");
        }

        // TODO: make the notification system for merits apply to the next message on the command that the user executes
        public void CheckStats(OriCommandContext ctx, User user)
        {
            // Check the stats for any possible merits
            var merits = WorldEngine.Merits.Where(x => x.Value.Criteria.Invoke(user) && !user.HasMerit(x.Key));

            if (merits.Count() > 0)
            {
                foreach (KeyValuePair<string, Merit> merit in merits)
                {
                    user.Merits.Add(merit.Key, merit.Value.GetData());

                    if (!user.Config.Notifier.HasFlag(NotifyDeny.Merit))
                        user.Notifier.Append($"Merit unlocked: **{merit.Value.Name}**");
                }
            }
        }
    }
}
