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

    public partial class DiscordEventHandler /* #CommandHandler */
    {
        private async Task ExecuteCommandAsync(OriCommandContext Context, int argPos)
        {
            //_logger.Debug("Analyzing context...");
            string baseMsg = GetBaseMessage(Context.Message);

           

            if (Context.Account != null)
            {
                Console.WriteLine($"Checking account...");
                // Check for Global Cooldown
                if (Context.Account.InternalCooldowns.ContainsKey(Cooldown.GLOBAL))
                    await Context.Channel.WarnCooldownAsync(Context.Account, Context.Account.InternalCooldowns[Cooldown.GLOBAL]);

                // check command for cooldowns.
                InfoService helperService = new InfoService(_commandService, Context.Global);
                // in the case of custom command cooldowns, you would need to ignore them here;
                foreach (CooldownData data in Context.Account.GetCooldownsOfType(CooldownType.Command))
                {
                    List<string> aliases = helperService.GetAliases(data.Id.Substring(VarBase.COMMAND_SUBST.Length));
                    if (aliases.Count == 0)
                        aliases.Add(data.Id.Substring("command:".Length));

                    foreach (string alias in aliases)
                    {
                        if (baseMsg == GetContextPrefix(Context) + alias)
                        {
                            if (Context.Account.IsOnCooldown(data.Id))
                            {
                                await Context.Channel.WarnCooldownAsync(Context.Account, data);
                                return;
                            }
                        }
                    }
                }
            }

            // custom command logic
            // make sure to apply a global cooldown in the case of a successful custom command.
            if (Context.Server != null)
            {
                Console.WriteLine($"Checking server...");
                if (Context.Server.Options.Commands != null)
                {
                    foreach (GuildCommand custom in Context.Server.Options.Commands)
                    {
                        if (baseMsg == GetContextPrefix(Context) + custom.Name)  // [yoshikill
                        {
                            if (custom.Message == null)
                                break;
                            await InvokeGuildCommandAsync(Context, custom);
                            return;
                        }
                    }
                }
            }

            // if not on a cooldown OR has a custom command within, proceed with default execution.
            // CONCEPT: PERFORM a test run, in which it checks if everything correctly runs before actually committing to it
            // CONCEPT: INSTEAD of sending the messages in the command, send it an IResult class called CommandResult?

            Console.WriteLine($"Executing command...");
            await _commandService.ExecuteAsync(Context, argPos, _provider, MultiMatchHandling.Exception);
        }

        private string GetBaseMessage(SocketUserMessage message)
        {
            return message.Content.Contains(' ') ? message.Content.Split(' ')[0] : message.Content;
        }

        private async Task InvokeGuildCommandAsync(OriCommandContext context, GuildCommand command)
        {
            await context.Channel.SendMessageAsync(command.Message.Build());
        }
        /// <summary>
        /// These are the set of tasks that are executed when a user successfully calls a command.
        /// </summary>
        private async Task OnCommandExecutedAsync(Optional<CommandInfo> info, ICommandContext context, IResult result)
        {
            OriCommandContext Context = context as OriCommandContext;

            Context.Account?.SetCooldown(CooldownType.Global, "base", TimeSpan.FromSeconds(1));
            Console.WriteLine($"Command executed...");
            if (!result.IsSuccess)
            {
                if (result is ExecuteResult)
                {
                    if (!result.IsSuccess)
                        await Context.Channel.CatchAsync(((ExecuteResult)result).Exception);
                }
                else
                {
                    await Context.Channel.ThrowAsync(result.ErrorReason);
                }

                return;
            }

            //_logger.Debug("Command.Success");

            /* Determine if there's a cooldown to be set. */
            if (info.IsSpecified)
            {
                CooldownAttribute cooldown = info.Value.Attributes.GetAttribute<CooldownAttribute>();
                if (cooldown != null)
                {
                    string commandId = $"{(Checks.NotNull(info.Value.Name) ? info.Value.Name : info.Value.Module.Group)}+{info.Value.Priority}";
                    Context.Account?.SetCooldown(CooldownType.Command, commandId, cooldown.Duration);
                }

                RequireUserAttribute requireUser = info.Value.Preconditions.GetAttribute<RequireUserAttribute>();
                if (requireUser?.Handling.EqualsAny(AccountHandling.ReadWrite, AccountHandling.WriteOnly) ?? false || cooldown != null)
                {
                    Context.Container.TrySaveUser(Context.Account);
                    await CheckStatsAsync(Context, Context.Account);
                }
                else if (!OriJsonHandler.JsonExists<User>(Context.User.Id)) // if a file doesn't exist
                {
                    if (Context.Account == null)
                        Context.Container.GetOrAddUser(Context.User);

                    Context.Container.TrySaveUser(Context.Account);
                }

                RequireGuildAttribute requireGuild = info.Value.Preconditions.GetAttribute<RequireGuildAttribute>();
                if (requireGuild != null)
                {
                    if (requireGuild.Handling.EqualsAny(AccountHandling.ReadWrite, AccountHandling.WriteOnly))
                        Context.Container.TrySaveGuild(Context.Server);
                    //else if (requireGuild.Handling == AccountHandling.ReadOnly)
                    //    if (!OriJsonHandler.JsonExists<OriGuild>(Context.Server.Id))
                    //        Context.Container.TrySaveGuild(Context.Server);
                }

                // RequireGlobalAttribute
                OriJsonHandler.Save(Context.Container.Global, "global.json");
            }
        }

        private async Task CheckStatsAsync(OriCommandContext context, User user)
        {
            var matchedMerits = GameDatabase.Merits.Where(x => x.Value.Criteria.Invoke(user) && !user.HasMerit(x.Key));
            if (matchedMerits.Count() > 0)
            {
                foreach (KeyValuePair<string, Merit> merit in matchedMerits)
                    user.Merits.Add(merit.Key, merit.Value.GetData());

                if (!user.Config.Notifier.HasFlag(NotifyDeny.Merit))
                    await context.Channel.NotifyMeritAsync(user, matchedMerits);
            }
        }
    }
}
