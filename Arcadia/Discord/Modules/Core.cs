using Discord.Commands;
using Orikivo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Newtonsoft.Json;
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

        //[DoNotNotify]
        //[Command("about"), Priority(0)]
        public async Task AboutAsync()
        {

        }

        //[DoNotNotify]
        //[Command("about"), Priority(1)]
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

        [RequireUser(AccountHandling.ReadOnly)]
        [DoNotNotify]
        [Command("options"), Alias("config", "cfg"), Priority(0)]
        [Summary("Returns all of your customized preferences.")]
        public async Task GetOptionsAsync()
        {
            await Context.Channel.SendMessageAsync(Context.Account.Config.Display());
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [DoNotNotify]
        [Command("options"), Alias("config", "cfg"), Priority(1)]
        [Summary("View more details for the specified option.")]
        public async Task ViewOptionAsync(string name)
        {
            await Context.Channel.SendMessageAsync(Context.Account.Config.ViewOption(name));
        }

        [RequireUser]
        [DoNotNotify]
        [Command("options"), Alias("config", "cfg"), Priority(2)]
        [Summary("Updates the option to the specified value.")]
        public async Task SetOptionAsync(string name, [Name("value")]string unparsed)
        {
            PropertyInfo option = ClassHelper.GetProperty(Context.Account.Config, name);

            if (option == null)
            {
                await Context.Channel.SendMessageAsync(Format.Warning("Unknown option specified."));
                return;
            }

            Type type = Context.Account.Config.GetOptionType(name);
            ClampAttribute clamp = option.GetCustomAttribute<ClampAttribute>();
            bool useFlags = type.GetCustomAttribute<FlagsAttribute>() != null;

            var panel = new StringBuilder();
            panel.AppendLine($"> **{Context.Account.Config.GetOptionName(name)}**");

            switch (unparsed)
            {
                case "--default":
                    Context.Account.Config.SetOptionDefault(name);
                    panel.AppendLine("> The specified option has been reset.");
                    break;

                case "--min":
                {
                    if (type != typeof(int))
                    {
                        panel.AppendLine("> This method can only be used on a `Number` with a specified minimum range.");
                    }
                    else if (clamp == null || !clamp.HasMin)
                    {
                        panel.AppendLine("> This `Number` does not have a specified minimum range.");
                    }
                    else
                    {
                        Context.Account.Config.SetOption(name, clamp.Min);
                        panel.AppendLine("> The specified option has been set to its lowest possible value.");
                    }

                    break;
                }

                case "--max":
                {
                    if (type != typeof(int))
                    {
                        panel.AppendLine("> This method can only be used on a `Number` with a specified maximum range.");
                    }
                    else if (clamp == null)
                    {
                        panel.AppendLine("> This `Number` does not have a specified maximum range.");
                    }
                    else
                    {
                        Context.Account.Config.SetOption(name, clamp.Max);
                        panel.AppendLine("> The specified option has been set to its highest possible value.");
                    }

                    break;
                }

                case "--none":
                {
                    if (!type.IsEnum || !useFlags)
                    {
                        panel.AppendLine("> This method can only be used on a `Flag`.");
                    }
                    else if (!Enum.TryParse(type, "0", out object e))
                    {
                        panel.AppendLine("> An error occurred while attempted to clear all flags.");
                    }
                    else
                    {
                        Context.Account.Config.SetOption(name, e);
                        panel.AppendLine("> Cleared all flags.");
                    }

                    break;
                }

                case "--all":
                {
                    if (!type.IsEnum || !useFlags)
                    {
                        panel.AppendLine("> This method can only be used on a `Flag`.");
                    }
                    else if (!Enum.TryParse(type, $"{type.GetEnumValues().Cast<Enum>().Select(Convert.ToInt64).Sum()}", out object e))
                    {
                        panel.AppendLine("> An error occurred while attempted to activate all flags.");
                    }
                    else
                    {
                        Context.Account.Config.SetOption(name, e);
                        panel.AppendLine("> Activated all flags.");
                    }

                    break;
                }

                default:
                {
                    if (TypeParser.TryParse(type, unparsed, out object result))
                    {
                        if (type.IsEnum)
                        {
                                long flagValue = Convert.ToInt64(result);
                                if (flagValue < 0)
                                {
                                    panel.AppendLine("> Flags cannot be negative.");
                                    break;
                                }

                                long partialSum = EnumUtils.GetFlags(result).Select(Convert.ToInt64).Sum();


                                if (flagValue > 0)
                                {
                                    if (flagValue - partialSum > 0)
                                    {
                                        panel.AppendLine("> The flag summation contains an invalid flag.");
                                        break;
                                    }
                                }

                        }
                        if (type == typeof(string) && result is string s)
                        {
                            if (s.Contains("\n"))
                            {
                                panel.AppendLine("> The specified value cannot contain any line breaks.");
                                break;
                            }

                            if (clamp != null)
                            {
                                if (s.Length > clamp.Max)
                                {
                                        panel.AppendLine($"> The specified value cannot be larger than `{clamp.Max}`.");
                                        break;
                                }
                            }
                        }
                        if (type == typeof(int) && result is int i)
                        {
                            if (clamp != null)
                            {
                                if (clamp.HasMin && (i < clamp.Min || i > clamp.Max))
                                {
                                        panel.AppendLine($"> The specified value is out of range (`{clamp.Min} to {clamp.Max}`).");
                                        break;
                                }

                                if (i > clamp.Max)
                                {
                                        panel.AppendLine($"> The specified value cannot be larger than `{clamp.Max}`.");
                                        break;
                                }
                            }
                        }

                        Context.Account.Config.SetOption(name, result);
                        panel.Append($"> The specified value has been set to `{Context.Account.Config.WriteOptionValue(result)}`.");
                    }
                    else
                    {
                        panel.AppendLine("> The specified value could not be parsed.");

                        if (type.IsEnum)
                        {
                                panel.AppendLine();

                                List<string> names = type.GetEnumNames().ToList();
                                List<long> values = type.GetEnumValues().Cast<object>().Select(Convert.ToInt64).ToList();

                                IEnumerable<string> groups = names.Join(values,
                                    a => names.IndexOf(a),
                                    b => values.IndexOf(b),
                                    (a, b) => $"{a} = {b}");

                                if (groups.Any())
                                {
                                    panel.AppendLine($"> **Values**\n```cs");
                                    panel.AppendJoin(",\n", groups);
                                    panel.AppendLine("```");
                                }
                        }
                    }

                    break;
                }
            }

            await Context.Channel.SendMessageAsync(panel.ToString());
        }

        // TODO: Implement GuildConfig, and replace OriGuild with Guild.

        [DoNotNotify]
        [Command("version")]
        public async Task GetVersionAsync()
            => await Context.Channel.SendMessageAsync(OriGlobal.ClientVersion);
    }
}