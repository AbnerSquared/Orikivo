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
using Orikivo.Drawing;
using SysColor = System.Drawing.Color;
using Color = Discord.Color;
using System.Reflection;
using Orikivo.Unstable;

namespace Orikivo
{
    // TODO: Place actual command functions into a Service variant. (MiscService.cs)
    [Name("Misc")]
    [Summary("A group of commands that have no distinct category.")]
    public class MiscModule : OriModuleBase<OriCommandContext>
    {
        private readonly DiscordSocketClient _client;
        private InfoService _help;
        private readonly CommandService _commandService;
        //private readonly LogService _logger;
        private readonly GameManager _gameManager;
        public MiscModule(DiscordSocketClient client, CommandService commandService,
            /*LogService logger,*/ GameManager gameManager)
        {
            _client = client;
            _commandService = commandService;
            //_logger = logger;
            _gameManager = gameManager;
        }

        // make generic events
        [Group("greetings")]
        public class GreetingsGroup : OriModuleBase<OriCommandContext>
        {
            [Command("")]
            [Summary("Shows the list of all greetings used for this guild."), RequireContext(ContextType.Guild)]
            public async Task GetGreetingsAsync(int page = 1)
            {
                await Context.Channel.SendMessageAsync($"{(Context.Server.Options.UseEvents ? "" : $"> Greetings are currently disabled.\n")}```autohotkey\n{(Context.Server.Options.Greetings.Count > 0 ? string.Join('\n', Context.Server.Options.Greetings.Select(x => $"[{Context.Server.Options.Events.IndexOf(x)}] :: {x.Message}")) : "There are currently no greetings set.")}```");
            }

            [Command("add")]
            [Access(AccessLevel.Inherit), RequireContext(ContextType.Guild)]
            [Summary("Adds a greeting to the collection of greetings for this guild.")]
            public async Task AddGreetingAsync([Remainder]string greeting)
            {
                try
                {
                    Context.Server.Options.Events.Add(new GuildEvent(EventType.UserJoin) { Message = greeting });
                    await Context.Channel.SendMessageAsync($"> Greeting **#{Context.Server.Options.Greetings.Count - 1}** has been included.");
                }
                catch(Exception e)
                {
                    await Context.Channel.CatchAsync(e);
                }
            }

            [Command("remove"), Alias("rm")]
            [Access(AccessLevel.Inherit)]
            [Summary("Removes the greeting at the specified index (zero-based)."), RequireContext(ContextType.Guild)]
            public async Task RemoveGreetingAsync(int index)
            {
                Context.Server.Options.Events.RemoveAt(index);
                // this will throw if outside of bounds
                await Context.Channel.SendMessageAsync($"> Greeting **#{index}** has been removed.");
            }

            [Command("clear")]
            [Access(AccessLevel.Owner)]
            [Summary("Clears all custom greetings written for this guild."), RequireContext(ContextType.Guild)]
            public async Task ClearGreetingsAsync()
            {
                Context.Server.Options.Events.RemoveAll(x => x.Type == EventType.UserJoin);
                await Context.Channel.SendMessageAsync($"> All greetings have been cleared.");
            }

            [Command("toggle")]
            [Access(AccessLevel.Owner)]
            [Summary("Toggles the ability to use greetings whenever a user joins."), RequireContext(ContextType.Guild)]
            public async Task ToggleGreetingsAsync()
            {
                Context.Server.Options.UseEvents = !Context.Server.Options.UseEvents;
                await Context.Channel.SendMessageAsync($"> **Greetings** {(Context.Server.Options.UseEvents ? "enabled" : "disabled")}.");
            }
        }

        [Command("defaultrole"), Priority(1)]
        [Access(AccessLevel.Owner), RequireContext(ContextType.Guild)]
        public async Task SetDefaultRoleAsync(SocketRole role)
        {
            Context.Server.Options.DefaultRoleId = role.Id;
            await Context.Channel.SendMessageAsync("The default role has been set.");
        }

        [Command("defaultrole"), Priority(0), RequireContext(ContextType.Guild)]
        public async Task GetDefaultRoleAsync()
        {
            await Context.Channel.SendMessageAsync($"The current default role id: `{Context.Server.Options.DefaultRoleId ?? 0}`");
        }

        [Command("mute")]
        [Access(AccessLevel.Owner)]
        [RequireBotPermission(ChannelPermission.ManageRoles), RequireContext(ContextType.Guild)]
        public async Task MuteUserAsync(SocketGuildUser user, double seconds)
        {
            if (!Context.Server.Options.MuteRoleId.HasValue)
            {
                RestRole muteRole = Context.Guild.CreateRoleAsync("Muted", new GuildPermissions(66560), null, false, false).Result;
                Context.Server.Options.MuteRoleId = muteRole.Id;
            }

            SocketRole role = Context.Guild.GetRole(Context.Server.Options.MuteRoleId.Value);
            if (role == null)
            {
                RestRole muteRole = Context.Guild.CreateRoleAsync("Muted", new GuildPermissions(66560), null, false, false).Result;
                Context.Server.Options.MuteRoleId = muteRole.Id;
            }

            if (Context.Server.HasMuted(user.Id))
            {
                Context.Server.Mute(user.Id, seconds);
                await Context.Channel.SendMessageAsync($"> {OriFormat.Username(user)} has been muted for another {OriFormat.GetShortTime(seconds)}.");
                return;
            }

            await user.AddRoleAsync(Context.Guild.GetRole(Context.Server.Options.MuteRoleId.Value));
            Context.Server.Mute(user.Id, seconds);
            await Context.Channel.SendMessageAsync($"> {OriFormat.Username(user)} has been muted for {OriFormat.GetShortTime(seconds)}.");

        }

        [Command("unmute")]
        [Access(AccessLevel.Owner)]
        [RequireBotPermission(ChannelPermission.ManageRoles), RequireContext(ContextType.Guild)]
        public async Task UnmuteUserAsync(SocketGuildUser user)
        {
            if (Context.Server.HasMuted(user.Id))
            {
                await user.RemoveRoleAsync(Context.Guild.GetRole(Context.Server.Options.MuteRoleId.Value));
                Context.Server.Unmute(user.Id);
                await Context.Channel.SendMessageAsync($"> {OriFormat.Username(user)} has been unmuted.");
                return;
            }

            await Context.Channel.SendMessageAsync($"> {OriFormat.Username(user)} is already unmuted.");
        }

        [Command("setrole")]
        [Access(AccessLevel.Owner)]
        [RequirePermissions(GuildPermission.ManageRoles), RequireContext(ContextType.Guild)]
        public async Task SetRoleAsync(SocketGuildUser user, SocketRole role)
        {
            if (user.Roles.Contains(role))
            {
                await Context.Channel.SendMessageAsync($"> {OriFormat.Username(user)} already has this role.");
                return;
            }

            await user.AddRoleAsync(role);
            await Context.Channel.SendMessageAsync($"> Gave {OriFormat.Username(user)} **{role.Name}**.");
        }

        // make a seperate field for the help menu with custom commands.
        [Command("customcommands"), RequireContext(ContextType.Guild)]
        public async Task GetCustomCommandsAsync(int page = 1)
        {
            if (Context.Server.Options.Commands.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"**Custom Commands**");
                sb.Append(string.Join(' ', Context.Server.Options.Commands.Select(x => $"`{x.Name}`")));
                await Context.Channel.SendMessageAsync(sb.ToString());
            }
            else
                await Context.Channel.SendMessageAsync($"There are currently no custom commands for this guild.");
        }

        // this is used to give the specified user the trust role
        //[Command("trust")]
        [Access(AccessLevel.Owner), RequireContext(ContextType.Guild)]
        public async Task TrustUserAsync(SocketGuildUser user) {}

        [Command("newcustomcommand")]
        [Access(AccessLevel.Inherit), RequireContext(ContextType.Guild)]
        public async Task SetCustomCommandAsync(string name, bool isEmbed, string imageUrl, [Remainder] string content = null)
        {
            GuildCommand command = new GuildCommand(Context.User, name);
            MessageBuilder message = new MessageBuilder(content, imageUrl);
            
            if (isEmbed)
                message.Embedder = Embedder.Default;

            command.Message = message;
            Context.Server.AddCommand(command);
            await Context.Channel.SendMessageAsync($"Your custom command (**{name}**) has been set.");
        }

        [Command("deletecustomcommand")]
        [Access(AccessLevel.Inherit), RequireContext(ContextType.Guild)]
        public async Task DeleteCustomCommandAsync(string name)
        {
            if (Context.Server.Options.Commands.Any(x => x.Name.ToLower() == name.ToLower()))
            {
                Context.Server.RemoveCommand(name);
                await Context.Channel.SendMessageAsync($"The custom command (**{name}**) has been deleted.");
            }
            else
                await Context.Channel.SendMessageAsync($"There aren't any custom commands in **{Context.Guild.Name}** that match '{name}'.");
        }

        [Command("closereport")]
        [Access(AccessLevel.Dev)]
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

        [Group("gimi")]
        public class GimiGroup : OriModuleBase<OriCommandContext>
        {
            // TODO: Figure out how to handle group commands with an empty subvalue.
            // TODO: Create an easier way to handle group commands that can execute without a subvalue.
            [Command("")]
            //[Cooldown(5)]
            [RequireUser]
            [Summary("A **CasinoType** activity that randomly offers a reward value (if you're lucky enough).")]
            public async Task GimiAsync()
            {
                Gimi gimi = new Gimi(Context.Account);
                GimiResult result = gimi.Next();

                await Context.Channel.SendMessageAsync(result.ApplyAndDisplay(Context.Account));
            }

            [Command("risk"), Priority(0)]
            [RequireUser]
            public async Task GetGimiRiskAsync()
            {
                //await Context.Channel.SendMessageAsync($"> Your **Risk** is currently set to **{Context.Account.Gimi.Risk}**%.");
            }

            [Command("risk"), Priority(1)]
            [RequireUser]
            public async Task SetGimiRiskAsync(int risk)
            {
                //Context.Account.Gimi.SetRisk(risk);
                //await Context.Channel.SendMessageAsync($"> Your **Risk** has been set to **{Context.Account.Gimi.Risk}**%.");
            }

            // find names that sound and work better.
            [RequireUser]
            [Command("range"), Priority(0)]
            public async Task SetGimiRangeAsync()
            {
                //await Context.Channel.SendMessageAsync($"> Your **Earn** is currently set to {Context.Account.Gimi.Earn}.");
            }

            [RequireUser]
            [Command("range"), Priority(1)]
            public async Task SetGimiRangeAsync(int earn)
            {
                //Context.Account.Gimi.SetEarn(earn);
                //await Context.Channel.SendMessageAsync($"> Your **Earn** has been set to **{Context.Account.Gimi.Earn}**.");
            }
        }

        [Command("help"), Alias("h")]
        [Summary("A guide to understanding everything **Orikivo** has to offer.")]
        public async Task GetHelpInfoAsync([Remainder][Summary("The **InfoContext** that defines your search.")]string context = null)
        {
            try
            {
                _help ??= new InfoService(_commandService, Context.Global, Context.Server);
                await Context.Channel.SendMessageAsync(_help.GetPanel(context));
            }
            catch (Exception ex)
            {
                await Context.Channel.CatchAsync(ex);
            }
        }

        [Command("profile"), Alias("pf")]
        [Summary("Gets the **OriUser** object from yourself.")]
        [RequireUser(AccountHandling.ReadOnly)]
        public async Task GetUserTestAsync()
        {
            await Context.Channel.SendMessageAsync(Context.Account.ToString());
        }

        // TODO: Create a context system to allow for getting specific option values alongside being able to set them.
        [RequireUser]
        [Command("options"), Alias("config", "cfg")]
        [Summary("Returns all of your customized preferences.")]
        public async Task GetOptionsAsync()
        {
            UserConfig config = Context.Account.Config;

            // TODO: Separate into a formatting class.
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"> **Prefix** • `{config.Prefix ?? "null"}`");
            sb.AppendLine($"> An optional property that allows you to set your own personal prefix when using **Orikivo.**");
            sb.AppendLine();
            sb.AppendLine($"> **Notifier** • `{config.Notifier}`");
            sb.AppendLine($"> Controls the notifiers that have permission to notify you.");
            sb.AppendLine();
            sb.AppendLine($"> **Tooltips** `{config.Tooltips}`");
            sb.AppendLine($"> A marker that determines if tooltips will be shown when executing certain commands.");
            sb.AppendLine();
            sb.AppendLine($"> **Debug** `{config.Debug}`");
            sb.AppendLine($"> A marker that labels you as a debug tester. ");

            await Context.Channel.SendMessageAsync(sb.ToString());
        }

        [Command("guildprofile"), Alias("server", "gpf")]
        [Summary("Gets the **OriGuild** object for the current **SocketGuild**.")]
        [RequireUser]
        public async Task GetGuildTestAsync()
        {
            await Context.Channel.SendMessageAsync(Context.Server.ToString());
        }

        // TODO: Might scrap all around. The DisplayEntityFormat is kind of a mess.
        //[Command("displayformat"), Alias("dispfmt"), Priority(0)]
        //[Summary("Shows your current **EntityDisplayFormat**.")]

        //[Command("displayformat"), Alias("dispfmt"), Priority(1)]
        //[Summary("Set your **EntityDisplayFormat** to the specified type.")]

        [Command("version")]
        public async Task GetVersionAsync()
            => await Context.Channel.SendMessageAsync(OriGlobal.ClientVersion);
    }
}
