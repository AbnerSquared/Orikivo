using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orikivo.Poxel;
using SysColor = System.Drawing.Color;
using Color = Discord.Color;
using System.Reflection;

namespace Orikivo
{
    // TODO: Place actual command functions into a Service variant. (MiscService.cs)
    [Name("Misc")]
    [Summary("A group of commands that have no distinct category.")]
    public class MiscModule : OriModuleBase<OriCommandContext>
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly OriConsoleService _logger;
        private readonly GameManager _gameManager;
        public MiscModule(DiscordSocketClient client, CommandService commandService,
            OriConsoleService logger, GameManager gameManager)
        {
            _client = client;
            _commandService = commandService;
            _logger = logger;
            _gameManager = gameManager;
        }

        // make generic events
        [Group("greetings")]
        public class GreetingsGroup : OriModuleBase<OriCommandContext>
        {
            [RequireGuild]
            [Command("")]
            [Summary("Shows the list of all greetings used for this guild.")]
            public async Task GetGreetingsAsync(int page = 1)
            {
                await Context.Channel.SendMessageAsync($"{(Context.Server.Options.AllowEvents ? "" : $"> Greetings are currently disabled.\n")}```autohotkey\n{(Context.Server.Options.Greetings.Count > 0 ? string.Join('\n', Context.Server.Options.Greetings.Select(x => $"[{Context.Server.Options.Events.IndexOf(x)}] :: {x.Message}")) : "There are currently no greetings set.")}```");
            }

            [RequireGuild]
            [Command("add")]
            [BindTo(TrustLevel.Inherit)]
            [Summary("Adds a greeting to the collection of greetings for this guild.")]
            public async Task AddGreetingAsync([Remainder]string greeting)
            {
                try
                {
                    Context.Server.Options.Events.Add(new GuildEventData(GuildEvent.UserJoin) { Message = greeting });
                    await Context.Channel.SendMessageAsync($"> Greeting **#{Context.Server.Options.Greetings.Count - 1}** has been included.");
                }
                catch(Exception e)
                {
                    await Context.Channel.CatchAsync(e);
                }
            }
            [RequireGuild]
            [Command("remove"), Alias("rm")]
            [BindTo(TrustLevel.Inherit)]
            [Summary("Removes the greeting at the specified index (zero-based).")]
            public async Task RemoveGreetingAsync(int index)
            {
                Context.Server.Options.Events.RemoveAt(index);
                // this will throw if outside of bounds
                await Context.Channel.SendMessageAsync($"> Greeting **#{index}** has been removed.");
            }
            [RequireGuild]
            [Command("clear")]
            [BindTo(TrustLevel.Owner)]
            [Summary("Clears all custom greetings written for this guild.")]
            public async Task ClearGreetingsAsync()
            {
                Context.Server.Options.Events.RemoveAll(x => x.Type == GuildEvent.UserJoin);
                await Context.Channel.SendMessageAsync($"> All greetings have been cleared.");
            }
            [RequireGuild]
            [Command("toggle")]
            [BindTo(TrustLevel.Owner)]
            [Summary("Toggles the ability to use greetings whenever a user joins.")]
            public async Task ToggleGreetingsAsync()
            {
                Context.Server.Options.AllowEvents = !Context.Server.Options.AllowEvents;
                await Context.Channel.SendMessageAsync($"> **Greetings** {(Context.Server.Options.AllowEvents ? "enabled" : "disabled")}.");
            }
        }

        [RequireGuild]
        [Command("defaultrole"), Priority(1)]
        [BindTo(TrustLevel.Owner)]
        public async Task SetDefaultRoleAsync(SocketRole role)
        {
            Context.Server.Options.DefaultRoleId = role.Id;
            await Context.Channel.SendMessageAsync("The default role has been set.");
        }

        [RequireGuild]
        [Command("defaultrole"), Priority(0)]
        public async Task GetDefaultRoleAsync()
        {
            await Context.Channel.SendMessageAsync($"The current default role id: `{Context.Server.Options.DefaultRoleId ?? 0}`");
        }

        [RequireGuild]
        [Command("mute")]
        [BindTo(TrustLevel.Owner)]
        [RequireBotPermission(ChannelPermission.ManageRoles)]
        public async Task MuteUserAsync(SocketGuildUser user, double seconds)
        {
            if (!Context.Server.Options.MuteRoleId.HasValue)
            {
                RestRole muteRole = Context.Guild.CreateRoleAsync("Muted", new GuildPermissions(66560)).Result;
                Context.Server.Options.MuteRoleId = muteRole.Id;
            }

            SocketRole role = Context.Guild.GetRole(Context.Server.Options.MuteRoleId.Value);
            if (role == null)
            {
                RestRole muteRole = Context.Guild.CreateRoleAsync("Muted", new GuildPermissions(66560)).Result;
                Context.Server.Options.MuteRoleId = muteRole.Id;
            }
            role = Context.Guild.GetRole(Context.Server.Options.MuteRoleId.Value);

            if (Context.Server.HasMuted(user.Id))
            {
                Context.Server.Mute(user.Id, seconds);
                await Context.Channel.SendMessageAsync($"> {OriFormat.ReadUserName(user)} has been muted for another {OriFormat.GetShortTime(seconds)}.");
                return;
            }

            await user.AddRoleAsync(Context.Guild.GetRole(Context.Server.Options.MuteRoleId.Value));
            Context.Server.Mute(user.Id, seconds);
            await Context.Channel.SendMessageAsync($"> {OriFormat.ReadUserName(user)} has been muted for {OriFormat.GetShortTime(seconds)}.");

        }

        [RequireGuild]
        [Command("unmute")]
        [BindTo(TrustLevel.Owner)]
        [RequireBotPermission(ChannelPermission.ManageRoles)]
        public async Task UnmuteUserAsync(SocketGuildUser user)
        {
            if (Context.Server.HasMuted(user.Id))
            {
                await user.RemoveRoleAsync(Context.Guild.GetRole(Context.Server.Options.MuteRoleId.Value));
                Context.Server.Unmute(user.Id);
                await Context.Channel.SendMessageAsync($"> {OriFormat.ReadUserName(user)} has been unmuted.");
                return;
            }

            await Context.Channel.SendMessageAsync($"> {OriFormat.ReadUserName(user)} is already unmuted.");
        }

        [RequireGuild]
        [Command("setrole")]
        [BindTo(TrustLevel.Owner)]
        [RequirePermissions(GuildPermission.ManageRoles)]
        public async Task SetRoleAsync(SocketGuildUser user, SocketRole role)
        {
            if (user.Roles.Contains(role))
            {
                await Context.Channel.SendMessageAsync($"> {OriFormat.ReadUserName(user)} already has this role.");
                return;
            }

            await user.AddRoleAsync(role);
            await Context.Channel.SendMessageAsync($"> Gave {OriFormat.ReadUserName(user)} **{role.Name}**.");
        }



        [Command("reply")]
        [BindTo(TrustLevel.Dev)]
        public async Task InvokeReactiveAsync()
        {
            await ReplyAsync("Just type anything:");
            SocketMessage response = await GetMessageAsync();
            if (response != null)
                await Context.Channel.SendMessageAsync($"Ok, cool.");
            else
                await Context.Channel.SendMessageAsync("Alright, I guess don't.");
        }

        [RequireGuild]
        // make a seperate field for the help menu with custom commands.
        [Command("customcommands")]
        public async Task GetCustomCommandsAsync(int page = 1)
        {
            if (Context.Server.CustomCommands.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"**Custom Commands**");
                sb.Append(string.Join(' ', Context.Server.CustomCommands.Select(x => $"`{x.Name}`")));
                await Context.Channel.SendMessageAsync(sb.ToString());
            }
            else
                await Context.Channel.SendMessageAsync($"There are currently no custom commands for this guild.");
        }

        [RequireGuild]
        // this is used to give the specified user the trust role
        //[Command("trust")]
        [BindTo(TrustLevel.Owner)]
        public async Task TrustUserAsync(SocketGuildUser user)
        {

        }

        [RequireGuild]
        [Command("newcustomcommand")]
        [BindTo(TrustLevel.Inherit)]
        public async Task SetCustomCommandAsync(string name, bool isEmbed, string imageUrl, [Remainder] string content = null)
        {
            CustomGuildCommand command = new CustomGuildCommand(name);
            CustomCommandMessage msg = new CustomCommandMessage(imageUrl, content, isEmbed ? OriEmbedOptions.Default : null);
            command.Message = msg;
            Context.Server.AddCommand(command);
            await Context.Channel.SendMessageAsync($"Your custom command (**{name}**) has been set.");
        }

        [RequireGuild]
        [Command("deletecustomcommand")]
        [BindTo(TrustLevel.Inherit)]
        public async Task DeleteCustomCommandAsync(string name)
        {
            if (Context.Server.CustomCommands.Any(x => x.Name.ToLower() == name.ToLower()))
            {
                Context.Server.TryRemoveCommand(name);
                await Context.Channel.SendMessageAsync($"The custom command (**{name}**) has been deleted.");
            }
            else
                await Context.Channel.SendMessageAsync($"There aren't any custom commands in **{Context.Guild.Name}** that match '{name}'.");
        }

        [Command("closereport")]
        [BindTo(TrustLevel.Dev)]
        public async Task SetReportStatusAsync(int id, string reason = null)
        {
            if (Context.Global.Reports.Contains(id))
            {
                Context.Global.Reports.Close(id, reason);
                await Context.Channel.SendMessageAsync($"> **Report**#{id} has been closed.");
                return;
            }
            await Context.Channel.SendMessageAsync($"> I could not find any reports matching #{id}.");
        }

        [Command("reports")]
        public async Task ReportsAsync()
        {
            await Context.Channel.SendMessageAsync($"> There are {Context.Global.Reports.Count} reports.");
        }

        [Cooldown(300)]
        [RequireUserAccount]
        [ArgSeparatorChar(',')]
        [Command("report"), Priority(1)]
        [Summary("Create a **Report** for a specified **Command**.")]
        public async Task ReportAsync([Summary("The **Command** to report.")]string context, string title, string content, params ReportTag[] tags)
        {
            ContextInfo ctx = ContextInfo.Parse(content);
            ContextSearchResult result = new OriHelpService(_commandService).Search(context);

            if (result.IsSuccess && (result.Type == ContextInfoType.Command || result.Type == ContextInfoType.Overload))
            {
                int id = Context.Global.Reports.Open(Context.Account, result.Type == ContextInfoType.Command ? result.Command.Default : result.Overload, new ReportBodyInfo(title, content), tags);
                await Context.Channel.SendMessageAsync($"> **Report** #{id} has been submitted.");
                return;
            }
            await Context.Channel.SendMessageAsync("> I could not find any ContextValue objects matching your context.");
        }

        [Command("report"), Priority(0)]
        [Summary("Get the **Report** submitted with the corresponding id.")]
        public async Task GetReportAsync(int id)
        {
            await Context.Channel.SendMessageAsync(Context.Global.Reports.Contains(id) ? Context.Global.Reports[id].ToString() : $"> I could not find any reports matching #{id}.");
        }

        [Group("gimi")]
        public class GimiGroup : OriModuleBase<OriCommandContext>
        {
            // TODO: Figure out how to handle group commands with an empty subvalue.
            // TODO: Create an easier way to handle group commands that can execute without a subvalue.
            [Command("")]
            [Cooldown(10)]
            [RequireUserAccount]
            [Summary("A **CasinoType** activity that randomly offers a reward value.")]
            public async Task GimiAsync()
            {
                GimiService gimi = new GimiService(Context.Account);
                int returns = gimi.Get();
                Context.Account.SetStat(returns > 0 ? GimiStat.CurrentLossStreak : GimiStat.CurrentWinStreak, 0);
                Context.Account.UpdateStat(returns > 0 ? GimiStat.CurrentWinStreak : GimiStat.CurrentLossStreak, 1);
                Context.Account.UpdateStat(returns > 0 ? GimiStat.TimesWon : GimiStat.TimesLost, 1);
            
                await Context.Channel.SendMessageAsync($"You got: {returns}");
            }

            [Command("risk"), Priority(0)]
            [RequireUserAccount]
            public async Task GetGimiRiskAsync()
            {
                await Context.Channel.SendMessageAsync($"> Your **Risk** is currently set to **{Context.Account.Gimi.Risk}**%.");
            }

            [Command("risk"), Priority(1)]
            [RequireUserAccount]
            public async Task SetGimiRiskAsync(int risk)
            {
                Context.Account.Gimi.SetRisk(risk);
                await Context.Channel.SendMessageAsync($"> Your **Risk** has been set to **{Context.Account.Gimi.Risk}**%.");
            }

            // find names that sound and work better.
            [RequireUserAccount]
            [Command("earn"), Priority(0)]
            public async Task SetGimiEarnAsync()
            {
                await Context.Channel.SendMessageAsync($"> Your **Earn** is currently set to {Context.Account.Gimi.Earn}.");
            }

            [RequireUserAccount]
            [Command("earn"), Priority(1)]
            public async Task SetGimiEarnAsync(int earn)
            {
                Context.Account.Gimi.SetEarn(earn);
                await Context.Channel.SendMessageAsync($"> Your **Earn** has been set to **{Context.Account.Gimi.Earn}**.");
            }

            [RequireUserAccount]
            [Command("slots")]
            public async Task GetGimiSlotsAsync()
            {
                await Context.Channel.SendMessageAsync($">>> **Gold**#{Context.Account.Gimi.GoldSlot}\n**Curse**#{Context.Account.Gimi.CurseSlot}");
            }

            [Command("clear")]
            [RequireUserAccount]
            [Cooldown(86400)]
            public async Task RegenGimiSlotsAsync()
            {
                Context.Account.Gimi.ClearSlots();
                await Context.Channel.SendMessageAsync("> **Gimi** slots cleared.");
            }
        }

        [Group("stats")]
        public class StatsGroup : OriModuleBase<OriCommandContext>
        {
            [Command("")]
            [RequireUserAccount]
            [Summary("Get information on all of the generic stats stored.")]
            public async Task GetStatsAync()
            {
                await Context.Channel.SendMessageAsync($"**Commands Used**: {Context.Account.GetStat(Stat.CommandsUsed)}");
            }

            [RequireUserAccount]
            [Command("gimi")]
            [Summary("Get all of the stats related to **Gimi**.")]
            public async Task GetGimiStatsAsync()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"> Gimi");
                sb.AppendLine($"> **Stats**");
                sb.AppendLine($"**{Context.Account.GetStat(GimiStat.TimesWon)}+{Context.Account.GetStat(GimiStat.TimesWonGold)}**W : **{Context.Account.GetStat(GimiStat.TimesLost)}+{Context.Account.GetStat(GimiStat.TimesLostCursed)}**L : **{Context.Account.GetStat(GimiStat.TimesPlayed)}**P");
                sb.AppendLine($"${Context.Account.GetStat(GimiStat.TotalAmountWon)} Earned : ${Context.Account.GetStat(GimiStat.TotalAmountLost)} Lost");
                sb.AppendLine($"Longest Win Streak: {Context.Account.GetStat(GimiStat.LargestWinStreakLength)} (${Context.Account.GetStat(GimiStat.LargestWinStreakAmount)}");
                sb.AppendLine($"Longest Gold Streak: {Context.Account.GetStat(GimiStat.LargestGoldStreakLength)}");
                sb.AppendLine($"Longest Loss Streak: {Context.Account.GetStat(GimiStat.LargestLossStreakLength)} (${Context.Account.GetStat(GimiStat.LargestLossStreakAmount)})");
                sb.AppendLine($"Longest Curse Streak: {Context.Account.GetStat(GimiStat.LargestCurseStreakLength)}");

                await Context.Channel.SendMessageAsync(sb.ToString());
            }
        }

        [Command("help"), Alias("h")]
        [Summary("A guide to understanding everything **Orikivo** has to offer.")]
        public async Task GetHelpInfoAsync([Remainder][Summary("The **ContextInfo** containing what to search for.")]string context = null)
        {
            try
            {
                OriHelpService helper = new OriHelpService(_commandService);
                await Context.Channel.SendMessageAsync(helper.Search(context).GetResultInfo() ?? helper.GetDefaultInfo());
                helper.Dispose(); // Still needs something that can be disposed for this to have purpose.
            }
            catch (Exception ex)
            {
                await Context.Channel.CatchAsync(ex);
            }
        }

        [Command("profile"), Alias("pf")]
        [Summary("Gets the **OriUser** object from yourself.")]
        [RequireUserAccount]
        public async Task GetUserTestAsync()
        {
            await Context.Channel.SendMessageAsync(Context.Account.GetDisplay(Context.Account.Options.DisplayFormat));
        }

        // TODO: Create a context system to allow for getting specific option values alongside being able to set them.
        [RequireUserAccount]
        [Command("options"), Alias("config", "cfg")]
        [Summary("Returns all of your customized preferences.")]
        public async Task GetUserOptionsAsync()
        {
            OriUserOptions options = Context.Account.Options;
            // TODO: Separate into a formatting class.
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"> **Prefix** `{options.Prefix ?? "null"}`");
            sb.AppendLine($"> This is an optional property that can be used as your personal prefix with **Orikivo.**");
            sb.AppendLine();
            sb.AppendLine($"> **Privacy** `{options.Privacy}`");
            sb.AppendLine($"> This property defines how public your Discord account is.");
            sb.AppendLine();
            sb.AppendLine($"> **Nickname** `{options.Nickname ?? "null"}`");
            sb.AppendLine($"> This property defines an optional name that can be set across **Orikivo**.");

            await Context.Channel.SendMessageAsync(sb.ToString());

        }



        [Command("guildprofile"), Alias("server", "gpf")]
        [Summary("Gets the **OriGuild** object for the current **SocketGuild**.")]
        [RequireUserAccount]
        public async Task GetGuildTestAsync()
        {
            await Context.Channel.SendMessageAsync(Context.Server.GetDisplay(Context.Account.Options.DisplayFormat));
        }

        // TODO: Might scrap all around. The DisplayEntityFormat is kind of a mess.
        [Command("displayformat"), Alias("dispfmt"), Priority(0)]
        [Summary("Shows your current **EntityDisplayFormat**.")]
        [RequireUserAccount]
        public async Task GetDisplayFormatAsync()
        {
            await Context.Channel.SendMessageAsync(Context.Account.Options.DisplayFormat.ToString());
        }

        [Command("displayformat"), Alias("dispfmt"), Priority(1)]
        [Summary("Set your **EntityDisplayFormat** to the specified type.")]
        [RequireUserAccount]
        public async Task SetDisplayFormatAsync([Summary("The new **EntityDisplayFormat** to set.")]EntityDisplayFormat format)
        {
            Context.Account.Options.DisplayFormat = format;
            await Context.Channel.SendMessageAsync("Display format set.");
        }

        [Command("version")]
        public async Task GetVersionAsync()
            => await Context.Channel.SendMessageAsync(Assembly.GetEntryAssembly().GetName().Version.ToString());
    }
}
