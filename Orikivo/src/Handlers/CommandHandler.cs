using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Orikivo.Desync;
using Orikivo.Framework;
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
        private readonly DesyncContainer _container;
        private readonly IServiceProvider _provider;
        private readonly InfoService _info;

        public CommandHandler(DiscordSocketClient client, CommandService service, DesyncContainer container,
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

        public string GetPrefix(DesyncContext ctx)
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
            DesyncContext ctx = new DesyncContext(_client, _container, source);
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

            if (!prefixFound)
                i = 2;

            if (source.HasStringPrefix(GetPrefix(ctx) + "d]", ref i) && !prefixFound)
            {
                prefix = GetPrefix(ctx) + "d]";
                prefixFound = true;
                // TODO: Make a permissions check before attempting to delete
                deleteInput = true;
            }

            // Execute the command
            if (!prefixFound)
                i = 0;

            if (source.HasStringPrefix(GetPrefix(ctx), ref i) && !prefixFound)
            {
                prefix = GetPrefix(ctx);
                prefixFound = true;
            }
            /*
            // Handle option parsing here... (set argPos to before options
            // [drawtext -f monori Hello world!
            // ^        ^Options  ^Args
            // 
            var reader = new StringReader(source.Content);
            reader.Skip(prefix.Length); // skip the length of the prefix
            string name = reader.ReadUnquotedString(); // this should get the command name

            // Then, you need to trim the whitespace of the command
            reader.SkipWhiteSpace();

            CommandInfo command = _info.GetCommand(name);


            // the raw collection of options.
            List<string> rawOptions = new List<string>();

            string rawOption = "";
            bool isStart = false;
            bool hasValue = false;
            int spacing = 0;

            while (reader.CanRead())
            {
                char c = reader.Peek();

                if (isStart)
                {
                    if (reader.Contains(' '))
                    {
                        rawOption += reader.ReadUntil(' ');
                        reader.SkipWhiteSpace();
                    }
                    else
                    {
                        rawOption += reader.GetRemaining();
                    }

                    isStart = false;
                }

                // This marks a new option.
                if (c == '-')
                {
                    isStart = true;
                    continue;
                }

                rawOption += reader.Read();
            }

            // 
            // with the search given, get all of the options for a command.
            IEnumerable<OptionAttribute> options = command.Attributes.FindAttributes<OptionAttribute>();

            if (!Check.NotNullOrEmpty(options))
            {
                foreach (OptionAttribute option in options)
                {
                    bool requireValue = option.Type != null; // if this option requires a value.

                    if (requireValue)
                    {

                    }
                }
            }


            // Now, using the command ref given, use InfoService to get its data.


            // PREFIXCOMMAND --OPTION VALUE --OPTION VALUE --OPTION VALUE ARG ARG ARG
            //       ^
            */
            if (prefixFound)
            {
                await ExecuteAsync(ctx, i);

                // TODO: Make a permissions check before attempting to delete
                if (deleteInput)
                    await ctx.Message.DeleteAsync();
            }
        }

        public async Task ExecuteAsync(DesyncContext ctx, int argPos)
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
            DesyncContext ctx = context as DesyncContext;

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
        private async Task UpdateAsync(CommandInfo command, DesyncContext ctx)
        {
            // Manage or update cooldowns
            CooldownAttribute cooldown = command.Attributes.FirstAttribute<CooldownAttribute>();

            if (cooldown != null)
            {
                string id = ContextNode.GetId(command, true);
                ctx.Account?.SetCooldown(CooldownType.Command, id, cooldown.Duration);
            }

            if (ctx.Account?.Husk != null)
            {
                Engine.CanMove(ctx.Account, ctx.Account.Husk);
            }

            bool notified = ctx.Account?.Notifier.LastNotified.HasValue ?? false;

            if (notified)
            {
                Logger.Debug("User was notified.");
                ctx.Account.Notifier.LastNotified = null;
            }

            // Check if the user was updated or doesn't exist to save
            RequireUserAttribute requireUser = command.Preconditions.FirstAttribute<RequireUserAttribute>();

            if (requireUser?.Handling.EqualsAny(AccountHandling.ReadWrite, AccountHandling.WriteOnly) ?? false
                || cooldown != null
                || notified)
            {
                // Check if any stats now meet certain criteria
                CheckStats(ctx, ctx.Account);

                Logger.Debug("User updated. Now saving...");
                ctx.Container.TrySaveUser(ctx.Account);

                
            }
            else if (!JsonHandler.JsonExists<User>(ctx.User.Id))
            {
                if (ctx.Account == null)
                    ctx.Container.GetOrAddUser(ctx.User);

                ctx.Container.TrySaveUser(ctx.Account);
            }

            // Check if the guild was updated or doesn't exist to save
            RequireGuildAttribute requireGuild = command.Preconditions.FirstAttribute<RequireGuildAttribute>();

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
        public void CheckStats(DesyncContext ctx, User user)
        {
            // Check the stats for any possible merits
            var merits = Engine.Merits.Where(x => x.Value.Criteria.Invoke(user) && !user.HasMerit(x.Key));

            if (merits.Count() > 0)
            {
                foreach ((string id, Merit merit) in merits)
                {
                    user.Merits.Add(id, merit.GetData());

                    if (user.Config.Notifier.HasFlag(NotifyDeny.Merit))
                        break;

                    user.Notifier.Append($"Merit unlocked: **{merit.Name}**");
                }
            }
        }
    }
}
