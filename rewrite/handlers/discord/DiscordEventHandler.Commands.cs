using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo
{
    public partial class DiscordEventHandler /* #CommandHandler */
    {
        private async Task InvokeCommandAsync(OriCommandContext context, int argPos)
        {
            // Check for Global Cooldown
            // Check for Guild Commands
            // Check for Command Cooldown
        }

        private async Task ExecuteCommandAsync(OriCommandContext Context, int argPos)
        {
            _logger.Debug("Analyzing context...");
            string baseMsg = GetBaseMessage(Context.Message);

            // Check for Global Cooldown
            if (Context.Account?.ProcessCooldowns.ContainsKey(Cooldown.GLOBAL) ?? false)
                await Context.Channel.WarnCooldownAsync(Context.Account, Context.Account.ProcessCooldowns[Cooldown.GLOBAL]);

            if (Context.Account != null)
            {
                // check command for cooldowns.
                InfoService helperService = new InfoService(_commandService, Context.Global);
                // in the case of custom command cooldowns, you would need to ignore them here;
                foreach (KeyValuePair<string, DateTime> pair in Context.Account.GetCooldownsFor(CooldownType.Command))
                {
                    List<string> aliases = helperService.GetAliasesFor(pair.Key);
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
            // CONCEPT: PERFORM a test run, in which it checks if everything correctly runs before actually committing to it
            // CONCEPT: INSTEAD of sending the messages in the command, send it in the results?
            IResult result = await _commandService.ExecuteAsync(Context, argPos, _provider, MultiMatchHandling.Exception);
            //if (!((ExecuteResult)result).IsSuccess)
            //    await Context.Channel.CatchAsync(((ExecuteResult)result).Exception);
        }

        private string GetBaseMessage(SocketUserMessage message)
        {
            return message.Content.Contains(' ') ? message.Content.Split(' ')[0] : message.Content;
        }

        private async Task InvokeGuildCommandAsync(OriGuild guild)
        {

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
