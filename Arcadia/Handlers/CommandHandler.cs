using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Orikivo;
using Orikivo.Desync;
using Orikivo.Framework;
using System;
using System.Threading.Tasks;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Arcadia
{
    public class CommandHandler
    {
        private readonly CommandService _service;
        private readonly DiscordSocketClient _client;
        private readonly ArcadeContainer _container;
        private readonly IServiceProvider _provider;
        private readonly InfoService _info;

        public CommandHandler(DiscordSocketClient client, CommandService service, ArcadeContainer container,
            IServiceProvider provider, InfoService info)
        {
            _client = client;
            _service = service;
            _container = container;
            _provider = provider;
            _client.MessageReceived += ReadInputAsync;
            _service.CommandExecuted += OnExecutedAsync;
            _info = info;
        }

        public string GetPrefix(ArcadeContext ctx)
            => ctx.Account?.Config.Prefix ??
               ctx.Server?.Config.Prefix ??
               OriGlobal.DEFAULT_PREFIX;

        public async Task ReadInputAsync(SocketMessage arg)
        {
            // Ignore bots
            if (arg.Author.IsBot)
                return;

            // Set up initial values
            SocketUserMessage source = arg as SocketUserMessage;
            ArcadeContext ctx = new ArcadeContext(_client, _container, source);
            bool deleteInput = false;
            bool prefixFound = false;

            // TODO: Handle message filters here to prevent additional command execution within another listener

            // Check all possible prefix formats
            int i = 0;

            string prefix = "";

            if (source.HasMentionPrefix(_client.CurrentUser, ref i))
            {
                prefix = _client.CurrentUser.Mention;
                prefixFound = true;
                await ExecuteAsync(ctx, i);
                return;
            }

            // Delete the message after executing the command
            i = 2;

            if (source.HasStringPrefix(GetPrefix(ctx) + "d]", ref i))
            {
                prefix = GetPrefix(ctx) + "d]";
                prefixFound = true;
                // TODO: Make a permissions check before attempting to delete
                deleteInput = true;
            }

            // Execute the command
            if (!prefixFound)
            {
                i = 0;

                if (source.HasStringPrefix(GetPrefix(ctx), ref i))
                {
                    prefix = GetPrefix(ctx);
                    prefixFound = true;
                }
            }

            if (prefixFound)
            {
                await ExecuteAsync(ctx, i);

                // TODO: Make a permissions check before attempting to delete
                if (deleteInput)
                    await ctx.Message.DeleteAsync();
            }
        }

        public async Task ExecuteAsync(ArcadeContext ctx, int argPos)
        {
            // TODO: Handle global cooldowns

            // TODO: Handle command cooldowns

            // TODO: Handle custom guild command references

            // TODO: Handle option parsing for commands

            // TODO: It might be required to create a custom parser and execution service separate from CommandService in order to properly
            // allow specific parsing methods
            await _service.ExecuteAsync(ctx, argPos, _provider);
        }

        private async Task OnExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // TODO: Make the specific context exchangeable.
            ArcadeContext ctx = context as ArcadeContext;

            // Attempt to set a global cooldown on the account that executed this command

            // If the command failed
            if (!result.IsSuccess)
            {
                if (result is ExecuteResult execute)
                {
                    if (!result.IsSuccess)
                        await context.Channel.CatchAsync((execute).Exception);
                }
                else
                    await context.Channel.ThrowAsync(result.ErrorReason);

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
        private async Task UpdateAsync(CommandInfo command, ArcadeContext ctx)
        {
            
            /*
            // Manage or update cooldowns
            CooldownAttribute cooldown = command.Attributes.FirstAttribute<CooldownAttribute>();

            if (cooldown != null)
            {
                string id = ContextNode.GetId(command, true);
                //ctx.Account?.SetCooldown(CooldownType.Command, id, cooldown.Duration);
            }
            */

            // Check if the user was updated or doesn't exist to save
            var requireUser = command.Preconditions.FirstOrDefault<RequireUserAttribute>();

            if (requireUser?.Handling.EqualsAny(AccountHandling.ReadWrite, AccountHandling.WriteOnly) ?? false)
            {
                MeritHelper.TryGiveMerits(ctx.Account);

                Logger.Debug("User updated. Now saving...");
                ctx.Data.Users.TrySave(ctx.Account);


            }
            else if (!JsonHandler.JsonExists<ArcadeUser>(ctx.User.Id))
            {
                if (ctx.Account == null)
                    ctx.GetOrAddUser(ctx.User);

                ctx.Data.Users.TrySave(ctx.Account);
            }

            // Check if the guild was updated or doesn't exist to save
            var requireGuild = command.Preconditions.FirstOrDefault<RequireGuildAttribute>();

            if (requireGuild?.Handling.EqualsAny(AccountHandling.ReadWrite, AccountHandling.WriteOnly) ?? false)
            {
                ctx.Data.Guilds.TrySave(ctx.Server);
            }
            else if (!JsonHandler.JsonExists<BaseGuild>(ctx.Guild.Id))
            {
                if (ctx.Guild == null)
                    ctx.GetOrAddGuild(ctx.Guild);

                ctx.Data.Guilds.TrySave(ctx.Server);
            }

            // For now, just save global data until a workaround is found
            // JsonHandler.Save(ctx.Container.Global, "global.json");
        }
    }
}
