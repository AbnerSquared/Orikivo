using Discord.Commands;
using Discord.WebSocket;
using Orikivo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Format = Orikivo.Format;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Arcadia.Modules
{
    [Icon("💽")]
    [Name("Core")]
    [Summary("Contains all root commands for Orikivo Arcade.")]
    public class Core : OriModuleBase<ArcadeContext>
    {
        private readonly InfoService _info;

        public Core(InfoService info)
        {
            _info = info;
        }

        [DoNotNotify]
        [Command("about"), Priority(0)]
        public async Task AboutAsync()
        {

        }

        [DoNotNotify]
        [Command("about"), Priority(1)]
        public async Task SearchAboutAsync(string input)
        {

        }

        [DoNotNotify]
        [Command("changelog")]
        public async Task ViewChangelogAsync()
        {
            IChannel channel = Context.Client.GetChannel(Context.Data.Data.LogChannelId);

            if (channel is IMessageChannel mChannel)
            {
                IEnumerable<IMessage> messages = await mChannel.GetMessagesAsync(1).FlattenAsync();
                IMessage message = messages.FirstOrDefault();

                if (message != null)
                {
                    await message.CloneAsync(Context.Channel);
                    return;
                }
            }

            await Context.Channel.SendMessageAsync(Format.Warning("Unable to find a previous changelog to reference."));
        }

        [DoNotNotify]
        [Cooldown(10)]
        [Command("latency"), Alias("ping")]
        public async Task GetLatencyAsync()
            => await CoreService.PingAsync(Context.Channel, Context.Client);

        [DoNotNotify]
        [Command("help"), Alias("h")]
        [Summary("A guide to understanding everything **Orikivo** has to offer.")]
        public async Task HelpAsync(
            [Remainder]
            [Summary("The **InfoContext** that defines your search.")]
            string context = null)
        {
            try
            {
                await Context.Channel.SendMessageAsync(_info.GetPanel(context, prefix: Context.GetPrefix())); // Context.Account
            }
            catch (Exception ex)
            {
                await Context.Channel.CatchAsync(ex);
            }
        }

        // TODO: Create a context system to allow for getting specific option values alongside being able to set them.
        [RequireUser(AccountHandling.ReadOnly)]
        [DoNotNotify]
        [Command("options"), Alias("config", "cfg"), Priority(0)]
        [Summary("Returns all of your customized preferences.")]
        public async Task GetOptionsAsync()
        {
            await Context.Channel.SendMessageAsync(Context.Account.Config.Display());
        }

        [RequireUser]
        [DoNotNotify]
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
                var panel = new StringBuilder();

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

        [DoNotNotify]
        [Command("version")]
        public async Task GetVersionAsync()
            => await Context.Channel.SendMessageAsync(OriGlobal.ClientVersion);
    }
}