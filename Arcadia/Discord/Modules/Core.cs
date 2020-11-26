using Discord.Commands;
using Orikivo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Format = Orikivo.Format;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Arcadia.Modules
{
    [Icon("💽")]
    [Name("Core")]
    [Summary("Contains all root commands for Orikivo Arcade.")]
    public class Core : BaseModule<ArcadeContext>
    {
        private readonly InfoService _info;

        public Core(InfoService info)
        {
            _info = info;
        }

        // [Id("")]: For use in locale.
        [DoNotNotify]
        [Command("about")]
        [Summary("View basic information about **Orikivo Arcade**.")]
        public async Task AboutAsync()
        {
            await Context.Channel.SendMessageAsync(AboutViewer.View());
        }

        [DoNotNotify]
        [Command("changelog")]
        [Summary("Returns the most recent changelog for **Orikivo Arcade**.")]
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
        [Summary("Returns the round-trip latency for both the current client and local connections.")]
        public async Task GetLatencyAsync()
            => await CoreService.PingAsync(Context.Channel, Context.Client);

        [DoNotNotify]
        [Command("help"), Alias("h")]
        [Summary("A guide to understanding everything **Orikivo Arcade** has to offer.")]
        public async Task HelpAsync(
            [Remainder, Summary("The **InfoContext** that defines your search.")]string context = null)
        {
            try
            {
                await Context.Channel.SendMessageAsync(_info.GetPanel(context, prefix: Context.GetPrefix()));
            }
            catch (Exception ex)
            {
                await Context.Channel.CatchAsync(ex);
            }
        }

        [DoNotNotify]
        [RequireGuild(AccountHandling.ReadOnly)]
        [RequireUser(AccountHandling.ReadOnly)]
        [Command("guildoptions"), Alias("guildconfig", "gcfg"), Priority(0)]
        [Summary("Returns all of the current guild's customized preferences.")]
        public async Task GetGuildOptionsAsync()
        {
            await Context.Channel.SendMessageAsync(Context.Server.Config.Display(Context.Account.Config.Tooltips));
        }

        [DoNotNotify]
        [RequireGuild(AccountHandling.ReadOnly)]
        [RequireUser(AccountHandling.ReadOnly)]
        [Command("guildoptions"), Alias("guildconfig", "gcfg"), Priority(1)]
        [Summary("View more details for the specified guild option.")]
        public async Task ViewGuildOptionAsync([Summary("The ID of the option to inspect.")]string id)
        {
            await Context.Channel.SendMessageAsync(Context.Server.Config.ViewOption(id, Context.Account.Config.Tooltips));
        }

        [DoNotNotify]
        [RequireGuild]
        [RequireUser(AccountHandling.ReadOnly)]
        [Command("guildoptions"), Alias("guildconfig", "gcfg"), Priority(2)]
        [Summary("Updates the guild option to the specified value.")]
        public async Task SetGuildOptionAsync([Summary("The ID of the option to update.")]string id, [Summary("The new value to set for this option.")]string value)
        {
            if (Context.Account.Id != Context.Server.OwnerId && Context.Account.Id != Constants.DevId)
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You do not have authority to update guild options."));
                return;
            }

            string result = Context.Server.Config.SetOrUpdateValue(id, value);
            await Context.Channel.SendMessageAsync(result);
        }

        [DoNotNotify]
        [RequireUser(AccountHandling.ReadOnly)]
        [Command("options"), Alias("config", "cfg"), Priority(0)]
        [Summary("Returns a summary of your current personal configuration.")]
        public async Task GetOptionsAsync()
        {
            string content = Context.Account.Config.Display(Context.Account.Config.Tooltips);
            await Context.Channel.SendMessageAsync(content);
        }

        [DoNotNotify]
        [RequireUser(AccountHandling.ReadOnly)]
        [Command("options"), Alias("config", "cfg"), Priority(1)]
        [Summary("View more details for the specified option.")]
        public async Task ViewOptionAsync(string name)
        {
            await Context.Channel.SendMessageAsync(Context.Account.Config.ViewOption(name));
        }

        [DoNotNotify]
        [RequireUser]
        [Command("options"), Alias("config", "cfg"), Priority(2)]
        [Summary("Updates the option to the specified value.")]
        public async Task SetOptionAsync([Summary("The ID of the option to update.")]string id, [Summary("The new value to set for this option.")]string value)
        {
            string result = Context.Account.Config.SetOrUpdateValue(id, value);
            await Context.Channel.SendMessageAsync(result);
        }
    }
}
