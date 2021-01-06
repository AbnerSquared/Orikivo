using Discord.Commands;
using Orikivo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Format = Orikivo.Format;
using Orikivo.Text;
using System.Text;
using Arcadia.Services;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Arcadia.Modules
{
    [Icon("💽")]
    [Name("Core")]
    [Summary("Contains all root commands for Orikivo Arcade.")]
    public class Core : ArcadeModule
    {
        private readonly InfoService _info;
        private readonly LocaleProvider _locale;

        public Core(InfoService info, LocaleProvider locale)
        {
            _info = info;
            _locale = locale;
        }

        [Command("stop")]
        [RequireAccess(AccessLevel.Dev)]
        public async Task StopAsync()
        {
            Environment.Exit(0);
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("guilderase")]
        [Summary("Deletes all data **Orikivo Arcade** stores from this server.")]
        public async Task EraseGuildAsync(string code = null)
        {
            if (Context.Account.Id != Context.Server.OwnerId && Context.Account.Id != Orikivo.Constants.DevId)
            {
                await Context.Channel.SendMessageAsync(Format.Warning(_locale.GetValue("warning_missing_option_authority", Context.Account.Config.Language)));
                return;
            }

            if (Context.Server == null)
            {
                await Context.Channel.SendMessageAsync("This server does not have any data stored on **Orikivo Arcade**.");
                return;
            }

            if (Check.NotNull(Context.Account.EraseGuildKey) && code == Context.Account.EraseGuildKey)
            {
                Context.Data.Guilds.Delete(Context.Server);
                await Context.Channel.SendMessageAsync("> All server configuration data has been erased.");
                return;
            }

            var result = new StringBuilder();

            if (!Check.NotNull(Context.Account.EraseGuildKey) || code != Context.Account.EraseGuildKey)
            {
                Context.Account.EraseGuildKey = KeyBuilder.Generate(12);

                if (code != Context.Account.EraseGuildKey)
                    result.AppendLine(Format.Warning("You have incorrectly specified the erase code.")).AppendLine();
            }

            result.AppendLine($"Once you erase server data, **ALL** server configuration data will be deleted. Please keep in mind that guild data is created the moment any of these commands are executed:")
                .AppendLine("`guildoption` `guildoptions`").AppendLine()
                .AppendLine("This is because configuration options are updated when using these commands.")
                .AppendLine($"To confirm deletion, type `guilderase {Context.Account.EraseGuildKey}`.");

            await Context.Channel.SendMessageAsync(result.ToString());
        }

        [Command("erase")]
        [Summary("Clear all of your data from **Orikivo Arcade**.")]
        public async Task EraseAsync(string code = null)
        {
            if (Context.Account == null)
            {
                await Context.Channel.SendMessageAsync("> You do not have any data stored on **Orikivo Arcade**.");
                return;
            }

            if (Check.NotNull(Context.Account.EraseConfirmKey) && code == Context.Account.EraseConfirmKey)
            {
                Context.Data.Users.Delete(Context.Account);
                await Context.Channel.SendMessageAsync("> All of your data has been successfully erased.");
                return;
            }

            var result = new StringBuilder();

            if (!Check.NotNull(Context.Account.EraseConfirmKey) || code != Context.Account.EraseConfirmKey)
            {
                Context.Account.EraseConfirmKey = KeyBuilder.Generate(12);
                
                if (Check.NotNull(code) && code != Context.Account.EraseConfirmKey)
                    result.AppendLine(Format.Warning("You have incorrectly specified the erase code.")).AppendLine();
            }

            result.AppendLine($"> **Erase Data**")
                  .AppendLine($"> Confirmation Code: `{Context.Account.EraseConfirmKey}`")
                  .AppendLine();

            int itemCount = Context.Account.Items.Sum(x => x.Count);
            result.AppendLine($"Once you erase your account, **ALL** data will be completely deleted. Please keep this in mind. This includes:");

            if (Context.Account.Balance > 0)
                result.AppendLine($"• Your wallet ({CurrencyHelper.WriteCost(Context.Account.Balance, CurrencyType.Money)})");

            if (Context.Account.Merits.Count > 0)
                result.AppendLine($"• Your completed merits (**{Context.Account.Merits.Count:##,0}**)");

            if (Context.Account.Level > 0)
                result.AppendLine($"• Your current level ({LevelViewer.GetLevel(Context.Account.Level, Context.Account.Ascent)})");

            if (Context.Account.Items.Count > 0)
                result.AppendLine($"• Your inventory (**{itemCount}** {Format.TryPluralize("item", itemCount)})");
            
            result.AppendLine($"• Your stats");

            result.AppendLine($"\nLikewise, if you're still certain, you can erase your account by typing `erase {Context.Account.EraseConfirmKey}`");

            await Context.Channel.SendMessageAsync(result.ToString());
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

            await Context.Channel.SendMessageAsync(Format.Warning(_locale.GetValue("warning_invalid_changelog", Context.Account.Config.Language)));
        }

        [DoNotNotify]
        [Cooldown(10)]
        [Command("latency"), Alias("ping")]
        [Summary("Returns the round-trip latency for both the current client and local connections.")]
        public async Task GetLatencyAsync()
            => await CoreService.PingAsync(Context.Channel, Context.Client);

        [DoNotNotify]
        [Command("help"), Alias("h")]
        [Summary("A service used to understand everything **Orikivo Arcade** has to offer.")]
        public async Task HelpAsync(
            [Remainder, Summary("The input used to generate a search result.")]string input = null)
        {
            try
            {
                await Context.Channel.SendMessageAsync(_info.SearchAndView(input, Context.Account, Context.GetPrefix()));
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
            if (Context.Account.Id != Context.Server.OwnerId && Context.Account.Id != Orikivo.Constants.DevId)
            {
                await Context.Channel.SendMessageAsync(Format.Warning(_locale.GetValue("warning_missing_option_authority", Context.Account.Config.Language)));
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
