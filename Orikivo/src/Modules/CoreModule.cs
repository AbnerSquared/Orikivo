using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable CS1998

namespace Orikivo.Modules
{
    [Name("Core")]
    [Summary("Defines all core commands.")]
    public class CoreModule : OriModuleBase<DesyncContext>
    {
        private readonly DiscordSocketClient _client;
        private readonly InfoService _info;
        public CoreModule(DiscordSocketClient client, InfoService info)
        {
            _client = client;
            _info = info;
        }

        [Command("latency"), Alias("ping")]
        public async Task PingAsync()
            => await CoreProvider.PingAsync(Context.Channel, Context.Client);

        [Command("help"), Alias("h")]
        [Summary("A guide to understanding everything **Orikivo** has to offer.")]
        public async Task HelpAsync([Remainder][Summary("The **InfoContext** that defines your search.")]string context = null)
        {
            try
            {
                _info.SetGuild(Context.Server);
                await Context.Channel.SendMessageAsync(_info.GetPanel(context, Context.Account));
                _info.ClearGuildInfo();
            }
            catch (Exception ex)
            {
                await Context.Channel.CatchAsync(ex);
            }
        }

        

        // TODO: Create a context system to allow for getting specific option values alongside being able to set them.
        [RequireUser(AccountHandling.ReadOnly)]
        [Command("options"), Alias("config", "cfg"), Priority(0)]
        [Summary("Returns all of your customized preferences.")]
        public async Task GetOptionsAsync()
        {
            await Context.Channel.SendMessageAsync(Context.Account.Config.Display());
        }

        [RequireUser]
        [Command("options"), Alias("config", "cfg"), Priority(1)]
        [Summary("Updates the specified option to the specified value.")]
        public async Task SetOptionAsync(string name, [Name("value")]string unparsed)
        {
            Type requiredType = Context.Account.Config.GetOptionType(name);
            
            if (requiredType == null)
            {
                await Context.Channel.SendMessageAsync("I could not find an option that matches the name you specified.");
                return;
            }

            if (TypeParser.TryParse(requiredType, unparsed, out object result))
            {
                StringBuilder panel = new StringBuilder();

                panel.AppendLine($"`{name}`: Updated value.");
                panel.Append($"**{Context.Account.Config?.GetOption(name)?.ToString() ?? "null"}** ⇛ **{result.ToString()}**");

                Context.Account.Config.SetOption(name, result);

                await Context.Channel.SendMessageAsync(panel.ToString());
            }
            else
            {
                await Context.Channel.SendMessageAsync("I could not parse the **Type** that this option specifies.");
            }
        }

        // TODO: Implement GuildConfig, and replace OriGuild with Guild.

        // TODO: Figure out how to display a guild profile.
        //[Command("guildprofile"), Alias("server", "gpf")]
        //[Summary("Returns a brief summary of your guild's profile.")]
        public async Task GetGuildProfileAsync()
            => throw new NotImplementedException();

        [Command("version")]
        public async Task GetVersionAsync()
            => await Context.Channel.SendMessageAsync(OriGlobal.ClientVersion);
    }
}
