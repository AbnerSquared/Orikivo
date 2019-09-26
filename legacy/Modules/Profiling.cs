using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Linq;
using Orikivo.Static;
using System.Threading.Tasks;
using Orikivo.Systems.Presets;
using Microsoft.Extensions.Configuration;
using Discord.Rest;

namespace Orikivo.Modules
{
    [Name("Profiling")]
    [Summary("Manage and analyze account data.")]
    [DontAutoLoad]
    public class Profiling : ModuleBase<OrikivoCommandContext>
    {
        // make sure all command structures end with ResponseAsync()

        private IConfigurationRoot _config;
        private DiscordSocketClient _client;
        private CommandService _service;
        private const ulong _owner = 181605794159001601;
        private const ulong _bugs = 525764709312495619;
        private const ulong _changelogs = 525443919484289035;

        public Profiling(DiscordSocketClient client, IConfigurationRoot config, CommandService service)
        {
            _client = client;
            _config = config;
            _service = service;
        }

        private const string JUMP_BASE = "https://discordapp.com/channels/";
        private const string JUMP_URL = "https://discordapp.com/channels/{0}/{1}/{2}";

        [Command("verbosemoduletoggle"), Alias("vmt")]
        public async Task ToggleModuleVerbosityAsync()
        {
            Context.Account.ToggleVerboseModules();
            await ReplyAsync(Context.Account.VerboseModules ? "Modules will show their summary." : "Modules will now hide their summary when available.");
        }


        #region Profiling#Tasks
        public async Task RestoreReportAsync(OrikivoCommandContext ctx, OldGlobal g, string jumpUrl)
        {
            string[] ids = jumpUrl.Substring(JUMP_BASE.Length).Split('/');
            ulong guildId = ulong.Parse(ids[0]);
            ulong channelId = ulong.Parse(ids[1]);
            ulong messageId = ulong.Parse(ids[2]);
            await RestoreReportAsync(ctx, g, guildId, channelId, messageId);

        }

        public async Task RestoreReportAsync(OrikivoCommandContext ctx, OldGlobal g, ulong guildId, ulong channelId, ulong messageId)
        {
            if (!_client.Guilds.Any(x => x.Id == guildId))
            {
                await ctx.Channel.SendMessageAsync("I need to be in the server that this report was sent in.");
                return;
            }

            SocketGuild guild = _client.GetGuild(guildId);
            if (!guild.TextChannels.Any(x => x.Id == channelId))
            {
                await ctx.Channel.SendMessageAsync("The channel that was specified no longer exists in this server.");
                return;
            }

            SocketTextChannel channel = guild.GetTextChannel(channelId);
            RestUserMessage message = (channel.GetMessageAsync(messageId).Result as RestUserMessage);

            await RestoreReportAsync(ctx, g, message);
        }

        public async Task RestoreReportAsync(OrikivoCommandContext ctx, OldGlobal g, RestUserMessage msg)
        {
            if (msg.Author.Id != _client.CurrentUser.Id)
            {
                await ReplyAsync("A report can only be restored from the messages I send.");
                return;

            }
            Report r = new Report(ctx, msg);

            if (g.Reports.Any(x => x.Id == r.Id) || g.AcceptedReports.Any(x => x.Id == r.Id))
            {
                await ReplyAsync("A report of this id already exists.");
                return;
            }

            g.LogReport(r);

            await ReplyAsync($"Report {r.Id} has been restored.");
        }

        public async Task RestoreReportAsync(OrikivoCommandContext ctx, OldGlobal g, ulong id)
        {
            RestUserMessage msg = await (_client.GetChannel(_bugs) as SocketTextChannel).GetMessageAsync(id) as RestUserMessage;
            await RestoreReportAsync(ctx, g, msg);
        }

        public async Task GetChangelogAsync(OldGlobal g, ulong id)
        {
            if (g.TryGetChangelog(id, out Changelog changelog))
            {
                await ReplyAsync(embed: changelog.Generate().Build());
                return;
            }

            await ReplyAsync("This changelog doesn't exist.");

        }

        public async Task GetChangelogsAsync(OldGlobal g, int page = 1)
        {
            if (!g.Changelogs.Exists())
            {
                await ReplyAsync("There are no changelogs in the storage.");
                return;
            }
            if (g.Changelogs.Count == 0)
            {
                await ReplyAsync("There are no changelogs in the storage.");
                return;
            }

            List<Changelog> changelogs = g.Changelogs.OrderByDescending(x => x.Date).ToList();

            page = page.InRange(1, changelogs.Count) - 1;

            EmbedBuilder e = changelogs[page].Generate();

            EmbedFooterBuilder f = new EmbedFooterBuilder();
            f.WithText(e.Footer.Text + $" | Page {page} of {changelogs.Count}");

            e.WithFooter(f);

            await ReplyAsync(embed: e.Build());
        }

        public async Task AddChangelogAsync(OldGlobal g, string name, string content, UpdateType type, ulong id)
        {
            if (g.Changelogs.Any(x => x.Id == id))
            {
                await ReplyAsync("A changelog of this id already exists.");
                return;
            }

            Changelog c = new Changelog(g, name, content, type, id);
            g.AddChangelog(c);

            await (_client.GetChannel(_changelogs) as SocketTextChannel).SendMessageAsync(embed: c.Generate().Build());
            await ReplyAsync($"Changelog built. You may view it at `changelog {c.Id}`.");

        }

        public async Task BuildReportAsync(OldGlobal g, OldAccount a, OriReportPriorityType type, string command, ulong id, string content, string subject = null)
        {
            if (g.Reports.Any(x => x.Id == id) || g.AcceptedReports.Any(x => x.Id == id))
            {
                await ReplyAsync("A report of this id already exists.");
                return;
            }

            Report r = new Report(a, type, id, command, content, subject);
            g.LogReport(r);

            await (_client.GetChannel(_bugs) as SocketTextChannel).SendMessageAsync(embed: r.Generate(a).Build());
            await ReplyAsync($"Your report ({id}) has been built.");
        }

        public async Task CompleteReportAsync(OrikivoCommandContext ctx, OldGlobal g, ulong id, ulong changelogId)
        {
            if (g.TryGetReport(id, out Report report))
            {
                OldAccount a = ctx.Data.GetOrAddAccount(report.Author.Id);
                await g.CompleteReport(a, ctx, id, changelogId);
                ctx.Data.Update(a);
                ctx.Data.Update(g);
            }
        }

        public async Task DeclineReportAsync(OrikivoCommandContext ctx, OldGlobal g, ulong id, string reason)
        {
            if (g.TryGetReport(id, out Report report))
            {
                OldAccount a = ctx.Data.GetOrAddAccount(report.Author.Id);
                await g.DeclineReport(a, ctx, id, reason);
                ctx.Data.Update(a);
                ctx.Data.Update(g);
            }
        }

        public async Task AcceptReportAsync(OrikivoCommandContext ctx, OldGlobal g, ulong id)
        {
            if (g.TryGetReport(id, out Report report))
            {
                OldAccount a = ctx.Data.GetOrAddAccount(report.Author.Id);
                await g.AcceptReport(a, ctx, id);
                ctx.Data.Update(a);
                ctx.Data.Update(g);
            }
        }

        public async Task GetReportAsync(OldGlobal g, ulong id)
        {
            OldAccount a = Context.Account;
            if (g.TryGetReport(id, out Report r))
            {
                await ReplyAsync(embed: r.Generate(a).Build());
                return;
            }

            Embed e = EmbedData.Throw(Context, "Invalid report.", $"R({id}) could not be found in the report collection.", false);
            await ReplyAsync(embed: e);
        }

        public async Task GetReportsAsync(OldGlobal g, int page = 1)
        {
            EmbedBuilder e = EmbedData.DefaultEmbed;

            EmbedFooterBuilder f = new EmbedFooterBuilder();
            f.WithText($"{EmojiIndex.Report} Reports");
            e.WithFooter(f);

            const int MAX_DESC = 1024;
            string desc = "";

            List<string> list = new List<string>();
            List<string> descriptions = new List<string>();

            List<Report> reports = g.Reports;
            List<Report> accepted = g.AcceptedReports;


            foreach (Report accept in accepted)
            {
                list.Add("**+** " + accept.ToString(Context.Account));
            }
            foreach (Report r in reports)
            {
                list.Add(r.ToString(Context.Account));
            }
            if (!list.Funct())
            {
                await ReplyAsync(embed: EmbedData.Throw(Context, "Empty collection.", "There are currently no reports.", false));
                return;
            }

            Embed q = EmbedData.GenerateEmbedList(list, page, e);

            await ReplyAsync(embed: q);
        }

        public async Task ReportBugAsync(OldAccount a, Server s, SocketGuild g, Emoji flag, CommandInfo command, string subject, string summary)
        {
            List<Emoji> flags = new List<Emoji>
            {
                EmojiIndex.VisualFlag,
                EmojiIndex.SuggestFlag,
                EmojiIndex.SpeedFlag,
                EmojiIndex.PriorityFlag,
                EmojiIndex.ExceptionFlag
            };

            string pfx = (s.Config.UsePrefixes ? s.Config.Prefix : Context.Client.CurrentUser.Mention + " ");

            if (!flag.EqualsAny(flags))
            {
                await ReplyAsync($"You must specify a flag type for the bug you wish to report. `{pfx}flags` for more info.");
                return;
            }

            OriReportPriorityType type = flag.GetFlagType();
            if (type == OriReportPriorityType.Unknown)
            {
                await ReplyAsync($"You must specify a flag type for the bug you wish to report. `{pfx}flags` for more info.");
                return;
            }

            Report r = new Report(Context.Account, type, Context.Message.Id, command, subject);

            Context.Data.Global.LogReport(r);
            await (Context.Client.GetChannel(_bugs) as SocketTextChannel).SendMessageAsync(embed: r.Generate(a).Build());
            await ReplyAsync($"Your input has been noted. You may view your report with `{pfx}getreport {r.Id}`");
        }

        public async Task GetServerPrefixAsync(Server s)
        {
            if (!s.Config.UsePrefixes)
            {
                await ReplyAsync("Prefixes are disabled.");
                return;
            }

            string prefix = s.Config.Prefix;

            string message = $"{s.Name} currently uses `{prefix}` as their prefix.";
            await ReplyAsync(message);
        }

        public async Task ToggleServerPrefixAsync(SocketGuildUser u, Server s, SocketGuild g)
        {
            if (!s.Config.ModeratorRoles.Exists())
            {
                s.Config.ModeratorRoles = new List<ulong>();
            }

            if (!u.HasAnyRole(s.Config.ModeratorRoles))
            {
                if (g.Owner != u && _owner != u.Id)
                {
                    await ReplyAsync("You must have a moderator role to edit server config.");
                    return;
                }
            }

            s.Config.UsePrefixes = !s.Config.UsePrefixes;

            if (s.Config.UsePrefixes)
            {
                await ReplyAsync($"Prefixes have been enabled. You may use `{s.Config.Prefix}` to execute commands.");
            }
            else
            {
                await ReplyAsync("Prefixes have been disabled. You may mention me in order to execute commands.");
            }
        }

        public async Task ChangeServerPrefixAsync(SocketGuildUser u, Server s, SocketGuild g, string prefix)
        {
            if (!s.Config.UsePrefixes)
            {
                await ReplyAsync("Prefixes are disabled.");
                return;
            }

            List<ulong> roles = s.Config.ModeratorRoles;
            if (!roles.Exists())
            {
                s.Config.ModeratorRoles = new List<ulong>();
            }

            if (!u.EnsureRank(s, g))
            {
                await ReplyAsync("In order to change prefixes, you must be an owner or moderator.");
                return;
            }

            s.Config.UpdatePrefix(prefix);
            await ReplyAsync($"Commands will now be executed with `{prefix}`.");
        }

        public async Task AddModRoleAsync(SocketGuildUser u, Server s, SocketGuild g, SocketRole r)
        {
            if (g.Owner != u)
            {
                await ReplyAsync("You must be the owner of the server in order to edit moderator roles.");
                return;
            }
            List<ulong> roles = s.Config.ModeratorRoles;

            if (!roles.Exists())
            {
                s.Config.ModeratorRoles = new List<ulong>();
                roles = s.Config.ModeratorRoles;
            }

            if (!g.HasRole(r.Id))
            {
                await ReplyAsync("This server doesn't have this role.");
                return;
            }

            if (roles.Contains(r.Id))
            {
                await ReplyAsync("This role already exists in the list of mod roles.");
                return;
            }

            s.Config.ModeratorRoles.Add(r.Id);
            await ReplyAsync($"{r.Name} ({r.Id}) has been placed as the moderator role.");
        }

        public async Task RemoveModRoleAsync(SocketGuildUser u, Server s, SocketGuild g, SocketRole r)
        {
            if (g.Owner != u)
            {
                await ReplyAsync("You must be the owner of the server in order to edit moderator roles.");
                return;
            }
            List<ulong> roles = s.Config.ModeratorRoles;

            if (!roles.Exists())
            {
                s.Config.ModeratorRoles = new List<ulong>();
                roles = s.Config.ModeratorRoles;
            }

            if (!g.HasRole(r.Id))
            {
                await ReplyAsync("This server doesn't have this role.");
                return;
            }

            if (!roles.Contains(r.Id))
            {
                await ReplyAsync("This role doesn't exist in the list of mod roles.");
                return;
            }

            s.Config.ModeratorRoles.Remove(r.Id);
            await ReplyAsync($"{r.Name} ({r.Id}) has been taken out of the moderator roles.");
        }

        public async Task GetServerModRolesAsync(Server s, SocketGuild g)
        {
            List<ulong> roles = s.Config.ModeratorRoles;
            List<SocketRole> ensured = new List<SocketRole>();

            if (!roles.Exists())
            {
                await ReplyAsync("this server doesn't have any set moderator roles.");
                return;
            }

            if (roles.Count == 0)
            {
                await ReplyAsync("this server doesn't have any set moderator roles.");
                return;
            }

            foreach (ulong role in roles)
            {
                if (!g.HasRole(role))
                {
                    roles.Remove(role);
                }

                SocketRole r = g.GetRole(role);

                if (ensured.Contains(r))
                {
                    continue;
                }

                ensured.Add(r);
            }


            StringBuilder sb = new StringBuilder();
            foreach (SocketRole r in ensured)
            {
                sb.Append($"{r.Name}\n");
            }

            await ReplyAsync(sb.ToString());
        }
        #endregion
        // Construct an ID system.

        public async Task ToggleErrorAsync(SocketGuildUser u, Server s, SocketGuild g)
        {
            if (!u.EnsureRank(s, g))
            {
                await ReplyAsync("You must be the bot dev, server owner, or moderator to toggle exceptions.");
                return;
            }

            s.Config.ToggleExceptions();
            await ReplyAsync($"{(s.Config.Throw ? "Exceptions will now be thrown when it occurs.": "Exceptions will not be displayed when executed.")}");
            Context.Data.Update(s);
        }

        //[Command("resetcasecounter")]
        //Summary("should only be used for the old id replacement")]
        public async Task FixCounterAsync()
        {
            Context.Global.EnsureCaseIncrement();
            await ReplyAsync("Case IDs have been clean and reset.");
            Context.Data.Update(Context.Global);
        }

        // birthday bois

        [Command("exceptions")]
        [Summary("Toggles exceptions in a server.")]
        public async Task ToggleErrorResponseAsync()
        {
            await ToggleErrorAsync(Context.GetGuildUser(), Context.Server, Context.Guild);
        }

        [Command("offloadreport")]
        [Summary("Offloads issues into the issues channel.")]
        public async Task OffloadIssueResponseAsync()
        {

        }

        [Command("restorereport"), Priority(1)]
        [Summary("Restore a report using a message link.")]
        public async Task RestoreReportResponseAsync(string jumpLink)
        {
            await RestoreReportAsync(Context, Context.Global, jumpLink);
            Context.Data.Update(Context.Global);
        }

        [Command("restorereport"), Priority(2)]
        [Summary("Restore a report using a message ID from the #reports channel in the Orikivo Community Server.")]
        public async Task RestoreReportResponseAsync(ulong id)
        {
            await RestoreReportAsync(Context, Context.Global, id);
            Context.Data.Update(Context.Global);
        }

        [Command("restorereport"), Priority(0)]
        [Summary("Restore a report using the server ID, channel ID, and message ID.")]
        public async Task RestoreReportResponseAsync(ulong server, ulong channel, ulong message)
        {
            await RestoreReportAsync(Context, Context.Global, server, channel, message);
            Context.Data.Update(Context.Global);
        }

        [Command("changelogs")]
        [Summary("View all changelogs.")]
        public async Task GetChangelogsResponseAsync(int page = 1)
        {
            await GetChangelogsAsync(Context.Global, page);
            
        }

        [Command("changelog"), Priority(1)]
        [Summary("Get a changelog with a specific id.")]
        public async Task GetChangelogResponseAsync(ulong id)
        {
            await GetChangelogAsync(Context.Global, id);
        }

        [Command("changelog"), Priority(0)]
        [Summary("View the most recent changelog.")]
        public async Task GetChangelogResponseAsync()
        {
            Changelog c = Context.Global.GetRecentChangelog();
            if (!c.Exists())
            {
                await ReplyAsync("There are currently no changelogs.");
            }

            await ReplyAsync(embed: c.Generate().Build());
        }

        [RequireOwner]
        [Command("addchangelog")]
        [Summary("Submit a new changelog.")]
        public async Task AddChangelogResponseAsync(string name, string content, UpdateType type)
        {
            await AddChangelogAsync(Context.Global, name, content, type, Context.Message.Id);
            Context.Data.Update(Context.Global);
        }

        public Emoji GetFlagType(string flag)
        {
            switch (flag)
            {
                case "p0":
                    return OriReportPriorityType.Critical.Icon();
                case "p1":
                    return OriReportPriorityType.Warning.Icon();
                case "p2":
                    return OriReportPriorityType.Speed.Icon();
                case "p3":
                    return OriReportPriorityType.Visual.Icon();
                default:
                    return OriReportPriorityType.Unknown.Icon();
            }
        }

        [Command("report"), Priority(0)]
        [Summary("Submit a report.")]
        public async Task SubmitReportRepsonseAsync(string flag, string command, string subject, [Remainder]string content)
        {
            Emoji ico = new Emoji(flag);
            if (flag.ToLower().EqualsAny("p0", "p1", "p2", "p3", "p4"))
            {
                ico = GetFlagType(flag);
            }

            SearchResult result = _service.Search(command);
            if (!result.IsSuccess)
            {
                await ReplyAsync("There are no commands that match what you typed.");
                return;
            }

            CommandInfo cmd = _service.Search(command).Commands.FirstOrDefault().Command;
            string name = ($"{cmd.Module.Name ?? cmd.Module.ToString()}.{cmd.Name}").ToLower().Replace(" ", "-");

            await BuildReportAsync(Context.Global, Context.Account, ico.GetFlagType(), name, Context.Global.CaseIncrement, content, subject);
            Context.Data.Update(Context.Account);
            Context.Data.Update(Context.Global);
        }

        [Command("acceptreport")]
        [Summary("Accept a report.")]
        public async Task AcceptReportResponseAsync(ulong id)
        {
            await AcceptReportAsync(Context, Context.Data.Global, id);
        }

        [Command("declinereport")]
        [Summary("Decline a report with a following reason.")]
        public async Task DeclineReportResponseAsync(ulong id, [Remainder]string reason)
        {
            await DeclineReportAsync(Context, Context.Data.Global, id, reason);
        }

        [Command("completeissue")]
        [Summary("Complete a report with its corresponding changelog.")]
        public async Task CompleteReportResponseAsync(ulong id, ulong changelogId)
        {
            await CompleteReportAsync(Context, Context.Data.Global, id, changelogId);
        }

        [Command("report"), Priority(1)]
        [Summary("Search for an issue with an id.")]
        public async Task GetReportResponseAsync(ulong id)
        {
            await GetReportAsync(Context.Data.Global, id);
        }

        [Command("reports")]
        [Summary("View all open reports.")]
        public async Task GetReportsResponseAsync(int page = 1)
        {
            await GetReportsAsync(Context.Data.Global, page);
        }

        [Command("modroles"), Alias("mr")]
        [Summary("View the list of available moderator roles on this server.")]
        public async Task ModRoleResponseAsync()
        {
            await GetServerModRolesAsync(Context.Server, Context.Guild);
        }

        [Command("addmodrole"), Alias("amr")]
        public async Task AddModRoleResponseAsync(SocketRole role)
        {
            await AddModRoleAsync(Context.Guild.GetUser(Context.User.Id), Context.Server, Context.Guild, role);
            Context.Data.Update(Context.Server);
        }

        [Command("removemodrole"), Alias("rmmr")]
        public async Task RemoveModRoleResponseAsync(SocketRole role)
        {
            await RemoveModRoleAsync(Context.Guild.GetUser(Context.User.Id), Context.Server, Context.Guild, role);
            Context.Data.Update(Context.Server);
        }

        [Command("prefixtoggle"), Alias("pfxt")]
        [Summary("Toggles the usage of prefixes")]
        public async Task TogglePrefixResponseAsync()
        {
            await ToggleServerPrefixAsync(Context.Guild.GetUser(Context.User.Id), Context.Server, Context.Guild);
            Context.Data.Update(Context.Server);
        }

        [Command("prefix"), Alias("pfx"), Priority(1)]
        [Summary("View the current prefix in use on the current server.")]
        public async Task PrefixResponseAsync()
        {
            await GetServerPrefixAsync(Context.Server);
        }

        [Command("resetprefix"), Alias("rpfx")]
        public async Task ResetPrefixAsync()
        {
            await ChangeServerPrefixAsync(Context.Guild.GetUser(Context.User.Id), Context.Server, Context.Guild, _config["prefixes:default"]);
            Context.Data.Update(Context.Server);
        }

        [Command("mobiletoggle"), Alias("mobt")]
        [Summary("Toggles Mobile Mode.")]
        public async Task ToggleMobileMode()
        {
            Context.Account.Config.ToggleMobile();
            await ReplyAsync(Context.Account.Config.Mobile ? "Mobile mode is now active." : "Mobile mode has been deactivated.");
            Context.Data.Update(Context.Account);
        }

        [Command("prefix"), Alias("pfx"), Priority(0)]
        [Summary("Change the current prefix in use on the current server.")]
        public async Task PrefixResponseAsync(string prefix)
        {
            await ChangeServerPrefixAsync(Context.Guild.GetUser(Context.User.Id), Context.Server, Context.Guild, prefix);
            Context.Data.Update(Context.Server);
        }

        [Command("overflowtoggle"), Alias("oft")]
        public async Task ToggleOverflowReponseAsync()
        {
            Context.Account.Config.ToggleOverflow();
            await ReplyAsync(Context.Account.Config.Overflow ? "All transactions above your balance will now auto-correct to your balance." : "All transactions will now prevent you from overpaying.");
        }

        //[Command("options"), Alias("opt")]
        //[Summary("View your account options.")]
        public async Task ViewOptions(int page = 1)
        {

        }

        public async Task ViewOptionsPanelAsync()
        {

        }

        public async Task GetConfigPanelAsync(OldAccount a, Server s, EmbedBuilder e)
        {
            StringBuilder sb = new StringBuilder();
            if (a.Config.Nickname.Exists())
            {
                sb.AppendLine($"**Nickname** | {a.Config.Nickname}");
                sb.AppendLine($"{EmojiIndex.Topic.Pack(a)} When referencing your account, this nickname overrides all references to your name.");
            }

            string rInfo = a.Config.ProfileCard ? "Your profile will be rendered in pixel art." : "Your profile will be rendered using an embed.";
            sb.AppendLine($"**Rendering** | {(a.Config.ProfileCard ? "Pixelated" : "Default")}");
            sb.AppendLine($"{EmojiIndex.Topic.Pack(a)} {rInfo}");

            sb.AppendLine($"**Insult Power** | {a.Config.InsultLevel}");
            sb.AppendLine($"{EmojiIndex.Topic.Pack(a)} {a.Config.GetInsultSummary()}");

            string mInfo = a.Config.Mobile ? "All emojis are unescaped to provide visibility to specific text." : "All emojis are escaped to provided a clean style.";
            sb.AppendLine($"**Portable Mode** | {a.Config.Mobile.ToToggleString()}");
            sb.AppendLine($"{EmojiIndex.Topic.Pack(a)} {mInfo}");

            string oInfo = a.Config.Overflow ? "Overspending is automatically corrected." : "You are prevented from overspending on transactions.";
            sb.AppendLine($"**Overflow** | {a.Config.Overflow.ToToggleString()}");
            sb.AppendLine($"{EmojiIndex.Topic.Pack(a)} {oInfo}");

            string vgInfo = a.Config.VerboseGimme ? "Give or Take displays extra information in relation." : "Extra information is hidden on Give or Take.";
            sb.AppendLine($"**Verbosity (Give or Take)** | {a.Config.VerboseGimme.ToToggleString()}");
            sb.AppendLine($"{EmojiIndex.Topic.Pack(a)} {vgInfo}");

            e.WithTitle("Options");
            e.WithDescription(sb.ToString());

            await ReplyAsync(embed: e.Build());
            return;
        }

        public async Task GetNotifierPanelAsync(OldAccount a, Server s, EmbedBuilder e)
        {
            var cfg = Context.Account.Config;
            var n1 = cfg.InboundMail;
            StringBuilder sb = new StringBuilder();

            string n1info = "Allows inbound mail to notify you by direct messages.";
            sb.AppendLine($"**Inbound Mail** | {n1.ToToggleString()}");
            sb.AppendLine($"{EmojiIndex.Topic.Pack(a)} {n1info}");

            string n2info = "Allows inbound updates to notify you by direct messages.";
            sb.AppendLine($"**Inbound Updates** | {cfg.InboundUpdates.ToToggleString()}");
            sb.AppendLine($"{EmojiIndex.Topic.Pack(a)} {n2info}");

            string title = "Notifications";

            e.Title = title;
            e.WithDescription(sb.ToString());

            await ReplyAsync(embed: e.Build());
            return;
        }

        public async Task GetLevelPanelAsync(OldAccount a, Server s, EmbedBuilder e)
        {
            GlobalData l = Context.Account.Data;
            var currentLevel = l.GetLevel();
            var percentage = l.LevelPercentile().ToString("#.##");
            var expOnThisLevel = l.ExperienceOnLevel(currentLevel).ToPlaceValue();
            var expToNextLevel = l.TotalExperienceToNextLevel().ToPlaceValue();
            var exp = l.Experience.ToPlaceValue();
            var remainderMax = l.RemainderExperience().ToPlaceValue();
            var remainderNext = l.RemainderToNextLevel().ToPlaceValue();

            string title = "Level Analytics";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"**Prestige**  | {l.Prestige} ({remainderMax}:XP to P{(l.Prestige + 1).ToPlaceValue()})");
            sb.AppendLine($"**Level** | {l.GetLevel()} **/** {expOnThisLevel}:XP ({(percentage.Equals("") ? "0.00" : percentage)}%)");
            sb.AppendLine($"**Raw Level** | {l.RawLevel}");
            sb.AppendLine($"**Experience** | {l.Experience.ToPlaceValue()}");

            e.WithFooter($"{currentLevel} | {exp}:XP ... {expToNextLevel}:XP | {currentLevel + 1}");
            e.Title = title;
            e.WithDescription(sb.ToString());

            await ReplyAsync(embed: e.Build());
            return;
        }

        public async Task GetWalletPanelAsync(OldAccount a, Server s, EmbedBuilder e)
        {
            AnalyzerOld wallet = Context.Account.Analytics;
            ulong spent = wallet.Expended;
            ulong held = wallet.MaxHeld;

            Emoji bal = EmojiIndex.Balance;
            string debt = $"{EmojiIndex.Debt.Pack(a)}{Context.Account.Debt.ToPlaceValue().MarkdownBold()}";
            string balance = $"{bal.Pack(a)}{Context.Account.Balance.ToPlaceValue().MarkdownBold()}";
            string sp = $"{bal.Pack(a)}{spent.ToPlaceValue().MarkdownBold()}";
            string h = $"{bal.Pack(a)}{held.ToPlaceValue().MarkdownBold()}";

            string title = "Wallet Analytics";
            string d1 = $"Balance | {balance}";
            string d2 = $"Debt | {debt}";
            string d3 = $"Expended | {sp}";
            string d4 = $"Most Held | {h}";
            string description = $"{d1}\n{d2}\n{d3}\n{d4}";

            e.Title = title;
            e.Description = description;

            await ReplyAsync(embed: e.Build());
            return;
        }
        /*
        public async Task GetCommandPanelAsync(OldAccount a, Server s, EmbedBuilder e)
        {
            var stats = Context.Account.Analytics;

            var recent = stats.Commands.OrderByDescending(x => x.Value.Date).ToList()[0];
            var rname = recent.Key;
            var date = recent.Value.Date.ToShortDateString();

            var top = stats.Commands.OrderByDescending(x => x.Value.Counter).ToList()[0];
            var tname = top.Key;
            var counter = top.Value.Counter;

            var count = stats.Commands.Count;

            string d1 = $"**Most Recent** | {rname}, {date}";
            string d2 = $"**Favorite** | {tname}, {counter} times";
            string d3 = $"**Total Different Commands Used** || {count}";

            string title = "Command Analytics";
            string description = $"{d1}\n{d2}\n{d3}";


            e.Title = title;
            e.Description = description;

            await ReplyAsync(embed: e.Build());
            return;
        }*/

        public async Task GetGimmePanelAsync(OldAccount a, Server s, EmbedBuilder e)
        {
            if (!a.TracksGimme)
            {
                a.GimmeStats = new GiveOrTakeAnalyzer();
            }
            GiveOrTakeAnalyzer got = a.GimmeStats;

            string em = EmojiIndex.Balance.Pack(a);
            e.WithTitle("Give or Take Analytics");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"**Current Streaks** | W⋅{got.WinStreak.ToPlaceValue()} ({em}{got.WinStreakAmount.ToPlaceValue()}) | L⋅{got.LossStreak.ToPlaceValue()} ({em}{got.LossStreakAmount.ToPlaceValue()})");
            sb.AppendLine($"**Play Count** | {got.PlayCount.ToPlaceValue()}");
            sb.AppendLine($"**Midas** | {got.GoldenCount.ToPlaceValue()}");
            sb.AppendLine($"**Wins** | {got.WinCount.ToPlaceValue()} ({em}{got.WinAmount.ToPlaceValue()})");
            sb.AppendLine($"**Losses** | {got.LossCount.ToPlaceValue()} ({em}{got.LossAmount.ToPlaceValue()})");
            sb.AppendLine($"**Highest Win Streak** | W⋅{got.MaxWinStreak} ({em}{got.MaxWinStreakAmount.ToPlaceValue()})");
            sb.AppendLine($"**Highest Loss Streak** | L⋅{got.MaxLossStreak} ({em}{got.MaxLossStreakAmount.ToPlaceValue()})");
            sb.AppendLine($"**Most Won in Streak** | {em}{got.MaxWinAmount.ToPlaceValue()} (W⋅{got.MaxWinAmountStreak})");
            sb.AppendLine($"**Most Lost in Streak** | {em}{got.MaxLossAmount.ToPlaceValue()} (L⋅{got.MaxLossAmountStreak})");

            e.WithDescription(sb.ToString());
            e.WithFooter($"Earnings | {EmojiIndex.Balance}{got.TotalOutcome.ToPlaceValue()}");

            await ReplyAsync(embed: e.Build());
        }

        [Command("upvotes")]
        public async Task ViewUpvotesAsync()
        {
            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithImageUrl("https://discordbots.org/api/widget/upvotes/433079994164576268.png");
            await ReplyAsync(embed: e.Build());
        }

        [Command("verbosegimmetoggle"), Alias("vgt")]
        public async Task VerboseGimmeResponseAsync()
            => await ToggleVerboseGimmeAsync(Context.Account);

        public async Task ToggleVerboseGimmeAsync(OldAccount a)
        {
            a.Config.ToggleVerboseGimme();
            await ReplyAsync(a.Config.VerboseGimme ? "Give or Take will now show extra information beneath the embed." : "Give or Take will no longer show extra information beneath the embed.");
        }

        //[Command("options"), Alias("config", "opt", "cfg")]
        public async Task OptionsPanelAsync()
            => await GetConfigPanelAsync(Context.Account, Context.Server, EmbedData.DefaultEmbed);

        [Command("levelstats"), Alias("lvlstats")]
        public async Task LevelPanelAsync()
            => await GetConfigPanelAsync(Context.Account, Context.Server, EmbedData.DefaultEmbed);

        [Command("gimmestats"), Alias("gotstats")]
        public async Task GimmePanelAsync()
            => await GetGimmePanelAsync(Context.Account, Context.Server, EmbedData.DefaultEmbed);

        [Command("balancestats"), Alias("balstats")]
        public async Task WalletPanelAsync()
            => await GetWalletPanelAsync(Context.Account, Context.Server, EmbedData.DefaultEmbed);

        [Command("account"), Alias("acc")]
        [Summary("View all of your current statistics.")]
        public async Task AccountResponseAsync(int page = 1)
        {
            SocketUser u = Context.User;
            OldAccount a = Context.Account;
            Server s = Context.Server;
            int pages = 7;

            page = page.InRange(1, pages) - 1;

            EmbedFooterBuilder f = new EmbedFooterBuilder();
            f.WithIconUrl(u.GetAvatarUrl());
            f.WithText($"{a.GetName()} | Page {page + 1} of {pages}");

            EmbedBuilder e = new EmbedBuilder();
            e.WithColor(EmbedData.GetColor("origreen"));
            e.WithFooter(f);

            if (page.Equals(0)) // Introduction Page
            {
                StringBuilder sb = new StringBuilder();
                
                string title = "Account Panel";
                sb.AppendLine($"`{Context.Server.Config.Prefix}account 2` - Options");
                sb.AppendLine($"`{Context.Server.Config.Prefix}account 3` - Notifications");
                sb.AppendLine($"`{Context.Server.Config.Prefix}account 4` - Level Stats");
                sb.AppendLine($"`{Context.Server.Config.Prefix}account 5` - Wallet Stats");
                sb.AppendLine($"`{Context.Server.Config.Prefix}account 6` - Command Stats");
                sb.AppendLine($"`{Context.Server.Config.Prefix}account 7` - Give Or Take Stats");

                e.WithTitle(title);
                e.WithDescription(sb.ToString());

                await ReplyAsync(embed: e.Build());
                return;
            }
            if (page.Equals(1)) // Configuration
            {
                await GetConfigPanelAsync(a, s, e);
                return;
            }
            if (page.Equals(2)) // Notifiers
            {
                await GetNotifierPanelAsync(a, s, e);
                return;
            }
            if (page.Equals(3)) // Level Analytics
            {
                await GetLevelPanelAsync(a, s, e);
                return;
            }
            if (page.Equals(4)) // Wallet Analytics
            {
                await GetWalletPanelAsync(a, s, e);
                return;
            }
            if (page.Equals(6))
            {
                await GetGimmePanelAsync(a, s, e);
                return;
            }
        }
    }
}
