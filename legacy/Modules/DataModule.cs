//using System;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Commands.Builders;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Orikivo.Static;
using Orikivo.Systems.Presets;
using Orikivo.Systems.Services;
using Orikivo.Utility;
using SysColor = System.Drawing.Color;
using Orikivo.Networking;
//using DiscordColor = Discord.Color;

namespace Orikivo.Modules
{
    //todd howard mp4 saying degenerate
    [Name("Data")]
    [Summary("Commands that offer information about raw data.")]
    [DontAutoLoad]
    public class DataModule : ModuleBase<OrikivoCommandContext>
    {
        private readonly CommandService _service;
        private readonly DiscordSocketClient _socket;
        private readonly IConfigurationRoot _config;
        private readonly GuildPrefUtility _pref;
        private OriWebClient _ori;
        public DataModule(CommandService service, IConfigurationRoot config, DiscordSocketClient socket, GuildPrefUtility pref)
        {
            _service = service;
            _config = config;
            _socket = socket;
            _pref = pref;
            _ori = new OriWebClient();
        }

        public Embed GenerateUserEmbed(SocketUser u)
        {
            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithColor(GetStatusColor(u.Status));
            var y = "✅";
            var n = "❌";

            var bot = u.IsBot ? y : n;
            var webhook = u.IsWebhook ? y : n;

            return e.Build();
        }

        public Emoji GetActivityIcon(IActivity a)
        {
            if (a == null)
            {
                return null;
            }
            if (a.Type.EqualsAny(ActivityType.Listening))
            {
                return EmojiIndex.Listening;
            }
            if (a.Type.EqualsAny(ActivityType.Playing))
            {
                return EmojiIndex.Playing;
            }
            if (a.Type.EqualsAny(ActivityType.Streaming))
            {
                return EmojiIndex.Streaming;
            }
            if (a.Type.EqualsAny(ActivityType.Watching))
            {
                return EmojiIndex.Watching;
            }

            return null;

        }

        public Color GetStatusColor(UserStatus status)
        {
            Color on = new Color(67, 181, 129);
            Color away = new Color(250, 166, 26);
            Color dnd = new Color(240, 71, 71);
            Color off = new Color(116, 127, 141);

            switch (status)
            {
                case UserStatus.AFK:
                    return away;
                case UserStatus.Idle:
                    return away;
                case UserStatus.DoNotDisturb:
                    return dnd;
                case UserStatus.Invisible:
                    return off;
                case UserStatus.Offline:
                    return off;
                default:
                    return on;
            }
        }

        public Emoji GetStatusIcon(UserStatus status)
        {
            switch (status)
            {
                case UserStatus.AFK:
                    return EmojiIndex.Away;
                case UserStatus.Idle:
                    return EmojiIndex.Away;
                case UserStatus.DoNotDisturb:
                    return EmojiIndex.Busy;
                case UserStatus.Invisible:
                    return EmojiIndex.Offline;
                case UserStatus.Offline:
                    return EmojiIndex.Offline;
                default:
                    return EmojiIndex.Online;
            }
        }

        public string GetVoiceStateIcon(IVoiceState vs)
        {
            OldAccount a = Context.Account;
            StringBuilder sb = new StringBuilder();

            if (vs.IsSuppressed)
            {
                return $"{EmojiIndex.Suppressed.Pack(a)}";
            }

            if (vs.IsDeafened || vs.IsSelfDeafened)
            {
                return $"{EmojiIndex.Deaf.Pack(a)}";
            }

            if (vs.IsMuted || vs.IsSelfMuted)
            {
                return $"{EmojiIndex.Muted.Pack(a)}";
            }

            return $"{EmojiIndex.Deaf.Pack(a)}";
        }

        public bool TryGetVoiceActivity(SocketGuildUser u, out string vc)
        {
            vc = "Not using voice chat.";
            StringBuilder sb = new StringBuilder();
            if (!u.VoiceChannel.Exists())
            {
                return false;
            }

            if (u.IsSuppressed)
            {
                sb.Append("Idling in");
            }
            else if (u.IsAnyDeafened())
            {
                sb.Append("Impaired in");
            }
            else if (u.IsAnyMuted())
            {
                sb.Append("Listening to");
            }
            else if (u.IsBot)
            {
                sb.Append("Playing audio in");
            }
            else
            {
                sb.Append("Speaking in");
            }

            sb.Append($" **{u.VoiceChannel.Name}**.");

            vc = sb.ToString();
            return true;
        }

        public Embed GenerateGuildUserEmbed(SocketGuildUser u)
        {
            const string EMPTY_AVATAR = "http://i0.kym-cdn.com/photos/images/original/000/861/034/3b4.gif";
            bool icons = false;

            EmbedAuthorBuilder a = new EmbedAuthorBuilder();

            EmbedFooterBuilder f = new EmbedFooterBuilder();
            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithColor(GetStatusColor(u.Status));
            string url = u.GetAvatarUrl() ?? EMPTY_AVATAR;
            e.WithThumbnailUrl(url);
            StringBuilder title = new StringBuilder();
            if (u.IsWebhook)
            {
                title.Append($"{EmojiIndex.Webhook} ");
            }
            if (u.IsBot)
            {
                title.Append($"{EmojiIndex.Bot} ");
            }
            if (icons)
            {
                title.Append($"{GetStatusIcon(u.Status)} ");
            }
            title.Append($"{u.Username}");

            e.WithTitle(title.ToString());

            bool vc = TryGetVoiceActivity(u, out string voicestate);

            IEnumerable<SocketRole> uRoles = u.Roles.Skip(1); // skipping 1 is because of the @everyone
            List<string> roles = new List<string>();
            if (uRoles.Funct())
            {
                foreach (SocketRole role in uRoles.OrderByDescending(x => x.Position))
                    roles.Add(role.Mention);
            }

            OldAccount acc = Context.Account;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"[{EmojiIndex.Identifier.Pack(acc)}]({url}) {u.Id}");
            sb.AppendLine($"[{EmojiIndex.FromHours(u.CreatedAt.Hour).Pack(acc)}]({url}) {u.CreatedAt.ToString("M/d/yyyy @ hh:mm tt")}");
            sb.AppendLine($"[{EmojiIndex.Role.Pack(acc)}]({url}) {(uRoles.Funct() ? uRoles.First().Mention : u.Roles.First().Mention)}");

            if (u.Activity.Exists())
            {
                sb.AppendLine($"[{GetActivityIcon(u.Activity).Pack(acc)}]({url}) {u.Activity.Summarize()}");
            }

            if (vc)
            {
                sb.AppendLine($"[{GetVoiceStateIcon(u.VoiceState)}]({url}) {voicestate}");
            }
            /*
            if (uRoles.Usable())
            {
                sb.AppendLine($"\n**Roles**");
                sb.AppendLine(string.Join("\n", roles) + '\n');
            }
            */

            e.WithDescription(sb.ToString());
            e.WithAuthor(a);

            string prefix = Context.Server.Config.GetPrefix(Context);
            //string sections = $"{prefix}avatar {u.Username} | {prefix}permissions {u.Username} | {prefix}roles {u.Username}";
            //f.WithText(sections);

            e.WithFooter(f);

            return e.Build();
        }

        [Command("avatar"), Alias("pfp")]
        [Summary("Get the avatar of a specified user.")]
        public async Task GetAvatarAsync([Remainder]SocketUser user = null)
        {
            user = user ?? Context.User;
            const string USER_AVATAR = "https://cdn.discordapp.com/avatars/{0}/{1}{2}?size=256";
            string link = null;
            if (user.AvatarId.Exists())
            {
                user.Id.Debug();
                user.AvatarId.Debug();
                Path.GetExtension(user.GetAvatarUrl()).Debug();
                link = string.Format(USER_AVATAR, user.Id, user.AvatarId, Path.GetExtension(user.GetAvatarUrl().Cut("?")));
            }
            link = link ?? user.GetDefaultAvatarUrl();

            var e = new EmbedBuilder();
            e.WithColor(EmbedData.GetColor("origreen"));
            if (!link.Exists())
            {
                e.WithColor(EmbedData.GetColor("error"));
            }
            e.WithImageUrl(link);

            await ReplyAsync(embed: e.Build());
        }

        [Command("icon")]
        [Summary("Gets the icon of a specified guild.")]
        public async Task GetIconAsync([Remainder]string guild = null)
        {
            SocketGuild g = Context.Guild;
            if (guild != null)
            {
                if (!Context.TryGetGuild(guild, out g))
                {
                    await ReplyAsync(embed: EmbedData.Throw(Context, "This server doesn't exist."));
                    return;
                }
            }
            const string GUILD_ICON = "https://cdn.discordapp.com/icons/{0}/{1}?size=256";

            string link = null;
            if (g.IconId.Exists())
            {
                link = string.Format(GUILD_ICON, g.Id, g.IconId);
            }
            link = link ?? Context.User.GetDefaultAvatarUrl();


            EmbedBuilder e = new EmbedBuilder();
            e.Color = EmbedData.GetColor("origreen");
            if (!link.Exists())
            {
                e.WithColor(EmbedData.GetColor("error"));
            }
            e.WithImageUrl(link);

            await ReplyAsync(embed: e.Build());
        }

        public List<string> GetGuildInfoList(List<SocketGuild> guilds)
        {
            string def = "↳ {0}";
            List<string> list = new List<string>();

            foreach (SocketGuild guild in guilds.OrderByDescending(x => x.Name))
            {
                list.Add(string.Format(def, guild.Name));
            }

            return list;
        }

        public List<string> GetCategoryInfoList(List<SocketCategoryChannel> channels)
        {
            string def = "↳ {0}";
            List<string> list = new List<string>();

            foreach (SocketCategoryChannel channel in channels.OrderBy(x => x.Position))
            {
                list.Add(string.Format(def, channel.Name));
            }

            return list;
        }

        public List<string> GetVoiceChannelInfoList(List<SocketVoiceChannel> channels)
        {
            string def = "↳ {0}";
            List<string> list = new List<string>();

            foreach (SocketVoiceChannel channel in channels.OrderBy(x => x.Position))
            {
                list.Add(string.Format(def, channel.Name));
            }

            return list;
        }

        public List<string> GetGuildChannelInfoList(List<SocketGuildChannel> channels)
        {
            OldAccount a = Context.Account;
            string def = "{0}";
            List<string> list = new List<string>();

            foreach (SocketGuildChannel channel in channels.OrderBy(x => x.Position))
            {
                StringBuilder sb = new StringBuilder();
                if (channel.IsTextChannel())
                {
                    sb.Append($"{EmojiIndex.Text.Pack(a)} ");
                }
                else if (channel.IsVoiceChannel())
                {
                    sb.Append($"{EmojiIndex.Deaf.Pack(a)} ");
                }
                sb.Append($"{channel.Name}");

                list.Add(string.Format(def, sb.ToString()));
            }

            return list;
        }

        public List<string> GetTextChannelInfoList(List<SocketTextChannel> channels)
        {
            string def = "↳ {0}";
            List<string> list = new List<string>();

            foreach (SocketTextChannel channel in channels.OrderBy(x => x.Position))
            {
                list.Add(string.Format(def, channel.Name));
            }

            return list;
        }

        public List<string> GetRoleInfoList(IEnumerable<SocketRole> roles, bool mention)
        {
            string def = "↳ {0}";
            List<string> list = new List<string>();

            foreach (SocketRole role in roles.OrderByDescending(x => x.Position))
            {
                list.Add(string.Format(def, mention ? role.Mention : role.Name));
            }

            return list;
        }

        public List<string> GetRoleInfoList(List<SocketRole> roles)
        {
            string def = "↳ {0}";
            List<string> list = new List<string>();

            foreach (SocketRole role in roles.OrderByDescending(x => x.Position))
            {
                list.Add(string.Format(def, role.Name));
            }

            return list;
        }



        public async Task GetUsers(SocketRole role, int page = 1)
        {
            List<SocketGuildUser> users = Context.Guild.Users.Where(x => x.HasRole(role)).OrderByDescending(x => x.Username).ToList();
            OldAccount a = Context.Account;
            EmbedBuilder e = new EmbedBuilder();
            EmbedFooterBuilder f = new EmbedFooterBuilder();
            e.Color = EmbedData.GetColor("origreen");
            e.Title = $"[{role.Name}]\n    ↳ Users";

            List<string> names = new List<string>();
            string def = "{0}";

            foreach (var user in users)
            {
                names.Add(string.Format(def, $"{(user.IsBot ? EmojiIndex.Bot.Pack(a) : EmojiIndex.User.Pack(a))}{user.Username}"));
            }

            if (users.Funct())
            {
                Embed r = EmbedData.GenerateEmbedList(names, page, e);
                await ReplyAsync(embed: r);
                return;
            }

            await ReplyAsync(embed: EmbedData.Throw(Context, "No users currently have this role."));
        }

        public async Task GetUsers(SocketTextChannel t, int page = 1)
        {
            List<SocketUser> users = t.Users.SortByName();
            OldAccount a = Context.Account;
            EmbedBuilder e = new EmbedBuilder();
            EmbedFooterBuilder f = new EmbedFooterBuilder();
            e.Color = EmbedData.GetColor("origreen");
            e.Title = $"[{t.Name}]\n    ↳ Users";

            List<string> names = new List<string>();
            string def = "{0}";

            foreach (var user in users)
            {
                names.Add(string.Format(def, $"{(user.IsBot ? EmojiIndex.Bot.Pack(a) : EmojiIndex.User.Pack(a))}{user.Username}"));
            }

            Embed r = EmbedData.GenerateEmbedList(names, page, e);
            await ReplyAsync(embed: r);
        }


        public async Task GetUsers(SocketVoiceChannel vc, int page = 1)
        {
            List<SocketUser> users = vc.Users.SortByName();

            EmbedBuilder e = new EmbedBuilder();
            EmbedFooterBuilder f = new EmbedFooterBuilder();
            e.Color = EmbedData.GetColor("origreen");
            e.Title = $"[{vc.Name}]\n    ↳ Users";

            List<string> names = new List<string>();
            string def = "↳ {0}";

            foreach (var user in users)
            {
                names.Add(string.Format(def, user.Username));
            }

            Embed r = EmbedData.GenerateEmbedList(names, page, e);
            await ReplyAsync(embed: r);
        }

        public async Task GetUsers(SocketCategoryChannel cg, int page = 1)
        {
            List<SocketUser> users = cg.Users.SortByName();

            EmbedBuilder e = new EmbedBuilder();
            EmbedFooterBuilder f = new EmbedFooterBuilder();
            e.Color = EmbedData.GetColor("origreen");
            e.Title = $"[{cg.Name}]\n    ↳ Users";

            List<string> names = new List<string>();
            string def = "↳ {0}";

            foreach (var user in users)
            {
                names.Add(string.Format(def, user.Username));
            }

            Embed r = EmbedData.GenerateEmbedList(names, page, e);
            await ReplyAsync(embed: r);
        }

        public async Task GetUsers(int page = 1)
        {
            OldAccount a = Context.Account;
            SocketGuild g = Context.Guild;
            List<SocketUser> users = g.Users.SortByName();

            EmbedBuilder e = new EmbedBuilder();
            EmbedFooterBuilder f = new EmbedFooterBuilder();
            e.Color = EmbedData.GetColor("origreen");
            e.Title = $"[{g.Name}]\n    ↳ Users";

            List<string> names = new List<string>();
            string def = "{0}";

            foreach (var user in users)
            {
                names.Add(string.Format(def, $"{(user.IsBot ? EmojiIndex.Bot.Pack(a) : EmojiIndex.User.Pack(a))}{user.Username}"));
            }

            Embed r = EmbedData.GenerateEmbedList(names, page, e);
            await ReplyAsync(embed: r);

        }

        [Command("modules"), Alias("ml")]
        public async Task ViewModulesAsync(int page = 1)
        {
            List<string> commands = _service.Modules.Enumerate(x => x.GetName()).OrderBy(x => x).ToList();
            EmbedBuilder e = EmbedData.DefaultEmbed;
            EmbedFooterBuilder f = new EmbedFooterBuilder();
            e.WithTitle($"[Orikivo]\n    ↳ Modules");
            Embed r = EmbedData.GenerateEmbedList(commands, page, e);
            await ReplyAsync(embed: r);
        }

        [Command("profiles"), Alias("pfl")]
        [Summary("View a list of all users that have a working profile.")]
        public async Task GetProfilesAsync(int page = 1)
        {
            List<string> names = Context.Data.Accounts.Values.Enumerate(x => x.GetDefaultName()).OrderBy(x => x).ToList();
            EmbedBuilder e = Embedder.DefaultEmbed;
            e.WithTitle($"[Orikivo]\n    ↳ Accounts");
            await Context.Channel.PaginatedReplyAsync(names, page, e);
        }

        [Command("commands"), Alias("cl")]
        public async Task ViewCommandsAsync(int page = 1)
        {
            List<string> commands = _service.Commands.Enumerate(x => x.Name).OrderBy(x => x).ToList();
            EmbedBuilder e = EmbedData.DefaultEmbed;
            EmbedFooterBuilder f = new EmbedFooterBuilder();
            e.WithTitle($"[Orikivo]\n    ↳ Commands");
            Embed r = EmbedData.GenerateEmbedList(commands, page, e);
            await ReplyAsync(embed: r);
        }

        [Command("categories"), Alias("cgl")]
        [Summary("Gets all currently existing categories in the current or specified guild.")]
        public async Task ListCategoryChannelsAsync(int page = 1)
        {
            List<SocketCategoryChannel> channels = Context.Guild.CategoryChannels.OrderByDescending(x => x.Name).ToList();

            EmbedBuilder e = EmbedData.DefaultEmbed;
            EmbedFooterBuilder f = new EmbedFooterBuilder();
            e.WithTitle($"[{Context.Guild.Name}]\n    ↳ Categories");

            List<string> names = GetCategoryInfoList(channels);
            Embed r = EmbedData.GenerateEmbedList(names, page, e);
            await ReplyAsync(embed: r);
        }

        [Command("voicechannels"), Alias("vcl")]
        [Summary("Gets all currently existing voice channels in the current or specified guild.")]
        public async Task ListVoiceChannelsAsync(int page = 1)
        {
            List<SocketVoiceChannel> channels = Context.Guild.VoiceChannels.OrderByDescending(x => x.Name).ToList();

            EmbedBuilder e = EmbedData.DefaultEmbed;
            EmbedFooterBuilder f = new EmbedFooterBuilder();
            e.WithTitle($"[{Context.Guild.Name}]\n    ↳ Voice Channels");

            List<string> names = GetVoiceChannelInfoList(channels);
            Embed r = EmbedData.GenerateEmbedList(names, page, e);
            await ReplyAsync(embed: r);
        }

        [Command("channels"), Alias("chl"), Priority(1)]
        [Summary("Gets all currently existing text channels in the current or specified guild.")]
        public async Task ListChannelsAsync(string category, int page = 1)
        {
            if (!Context.Guild.TryGetCategory(category, out SocketCategoryChannel cg))
            {
                await ReplyAsync(embed: EmbedData.Throw(Context, "No category uses this alias."));
                return;
            }

            List<SocketGuildChannel> channels = cg.Channels.OrderByDescending(x => x.Name).ToList();

            EmbedBuilder e = EmbedData.DefaultEmbed;
            EmbedFooterBuilder f = new EmbedFooterBuilder();
            e.WithTitle($"[{cg.Name}]\n    ↳ Channels");

            List<string> names = GetGuildChannelInfoList(channels);
            Embed r = EmbedData.GenerateEmbedList(names, page, e);
            await ReplyAsync(embed: r);
        }

        [Command("channels"), Alias("chl"), Priority(0)]
        [Summary("Gets all currently existing text channels in the current or specified guild.")]
        public async Task ListChannelsAsync(int page = 1)
        {
            List<SocketTextChannel> channels = Context.Guild.TextChannels.OrderByDescending(x => x.Name).ToList();

            EmbedBuilder e = EmbedData.DefaultEmbed;
            EmbedFooterBuilder f = new EmbedFooterBuilder();
            e.WithTitle($"[{Context.Guild.Name}]\n    ↳ Text Channels");

            List<string> names = GetTextChannelInfoList(channels);
            Embed r = EmbedData.GenerateEmbedList(names, page, e);
            await ReplyAsync(embed: r);
        }

        // Allow the restoration of emojis from messages.
        // allow storing other guild's emojis into your own server.
        // allow the building of new emojis from a url.
        // allow the user to choose which server that they want to save it in, in each one that they have the ability to.

        /*[Command("emojis"), Alias("ems")]
        [Summary("Gets a coupled version of all emoticons on the server.")]
        public async Task GetEmojiGroupAsync(string guild)
        {

        }*/

        [Command("emojis"), Alias("ems")]
        [Summary("Gets a coupled version of all emoticons on the server.")]
        public async Task GetEmojiGroupAsync([Remainder] string guild = null)
        {
            try
            {
                SocketGuild g = Context.Guild;
                if (guild != null)
                {
                    if (!Context.TryGetGuild(guild, out g))
                    {
                        await ReplyAsync(embed: EmbedData.Throw(Context, "This server doesn't exist."));
                        return;
                    }
                }

                EmbedBuilder e = EmbedData.DefaultEmbed;
                EmbedFooterBuilder f = new EmbedFooterBuilder();
                f.WithText(g.Name);

                StringBuilder sb = new StringBuilder();
                foreach (Emote em in g.Emotes)
                {
                    sb.Append($"{em}");
                    //sb.Append($"[{em}]({em.Url})");
                }

                e.WithDescription(sb.ToString());
                e.WithFooter(f);

                await ReplyAsync(embed: e.Build());
            }
            catch (Exception ex)
            {
                await ReplyAsync(embed: EmbedData.CatchException(GetHashCode(), ex));
            }
        }

        [Command("guilds"), Alias("gl")]
        [Summary("Gets all currently connected guilds in relation to me.")]
        public async Task ListGuildsAsync(int page = 1)
        {
            List<SocketGuild> guilds = Context.Client.Guilds.SortByName();

            EmbedBuilder e = EmbedData.DefaultEmbed;
            EmbedFooterBuilder f = new EmbedFooterBuilder();
            e.WithTitle($"[Orikivo]\n    ↳ Guilds");

            List<string> names = GetGuildInfoList(guilds);
            Embed r = EmbedData.GenerateEmbedList(names, page, e);
            await ReplyAsync(embed: r);
        }

        [Command("roles"), Alias("rl"), Priority(0)]
        [Summary("Gets all existing roles in a specified or current guild.")]
        public async Task ListRolesAsync(int page = 1)
        {
            SocketGuild g = Context.Guild;
            IEnumerable<SocketRole> roles = g.Roles.ByRoleName();

            EmbedBuilder e = EmbedData.DefaultEmbed;
            EmbedFooterBuilder f = new EmbedFooterBuilder();
            e.Color = EmbedData.GetColor("origreen");
            e.Title = $"[{g.Name}]\n    ↳ Roles";

            bool mention = g == Context.Guild;

            List<string> names = GetRoleInfoList(roles, mention);
            Embed r = EmbedData.GenerateEmbedList(names, page, e);
            await ReplyAsync(embed: r);
        }

        [Command("roles"), Alias("rl"), Priority(1)]
        [Summary("Gets all existing roles in a specified or current guild.")]
        public async Task ListRolesAsync(SocketGuildUser user, int page = 1)
        {
            SocketGuildUser u = user ?? Context.GetGuildUser();

            IEnumerable<SocketRole> uroles = u.Roles.ByRoleName();
            
            
            EmbedBuilder e = EmbedData.DefaultEmbed;
            EmbedFooterBuilder c = new EmbedFooterBuilder();
            e.Color = EmbedData.GetColor("origreen");
            e.Title = $"[{u.Username}]\n    ↳ Roles";
            
            List<string> names = GetRoleInfoList(uroles, true);
            Embed r = EmbedData.GenerateEmbedList(names, page, e);
            await ReplyAsync(embed: r);

        }

        [Command("users"), Alias("ul"), Priority(1)]
        [Summary("Displays a list of all users on a specified server.")]
        public async Task GetUsersAsync(int page = 1)
        {
            await GetUsers(page);
        }

        [Command("users"), Alias("ul"), Priority(0)]
        [Summary("Displays a list of all users on a specified server.")]
        public async Task GetUsersAsync(string role, int page = 1)
        {
            if (!Context.Guild.TryGetRole(role, out SocketRole r))
            {
                await ReplyAsync(embed: EmbedData.Throw(Context, "No role uses this alias."));
                return;
            }

            await GetUsers(r, page);
        }

        //[Command("audit")]
       //[Summary("View the audit log. You must the server owner, or have the permission to view audits.")]
        public async Task ViewAudit()
        {

        }

        [Command("usercount"), Alias("uc")]
        [Summary("Gets a detailed member count of a specified guild.")]
        public async Task GetGuildMemberCount([Remainder]string reference = null)
        {
            OldAccount a = Context.Account;
            SocketGuild guild = Context.Guild;
            if (reference != null)
            {
                if (!Context.TryGetGuild(reference, out guild))
                {
                    await ReplyAsync(embed: EmbedData.Throw(Context, "This server doesn't exist."));
                    return;
                }
            }

            EmbedBuilder e = new EmbedBuilder();
            e.Color = EmbedData.GetColor("origreen");
            e.WithTitle($"{guild.Name}");
            const string GUILD_ICON = "https://cdn.discordapp.com/icons/{0}/{1}?size=128";

            string link = null;
            if (guild.IconId.Exists())
            {
                link = string.Format(GUILD_ICON, guild.Id, guild.IconId);
            }
            link = link ?? Context.User.GetDefaultAvatarUrl();
            e.WithThumbnailUrl(link);

            IEnumerable<SocketGuildUser> bots = guild.Users.Where(x => x.IsBot);
            IEnumerable<SocketGuildUser> users = guild.Users.Where(x => !x.IsBot);

            int total = users.Count();
            int active = users.Where(x => !x.IsOffline()).Count();
            int online = users.Where(x => x.IsOnline()).Count();
            int away = users.Where(x => x.IsAway()).Count();
            int busy = users.Where(x => x.IsBusy()).Count();
            int offline = users.Where(x => x.IsOffline()).Count();

            int totalb = bots.Count();
            int activeb = bots.Where(x => !x.IsOffline()).Count();
            int onlineb = bots.Where(x => x.IsOnline()).Count();
            int awayb = bots.Where(x => x.IsAway()).Count();
            int busyb = bots.Where(x => x.IsBusy()).Count();
            int offlineb = bots.Where(x => x.IsOffline()).Count();

            string activeusers = ((double)active / total).ToString("##,0.00%");
            string activebots = ((double)activeb / totalb).ToString("##,0.00%");

            string onlineusers = ((double)online / total).ToString("##,0.00%");
            string onlinebots = ((double)onlineb / totalb).ToString("##,0.00%");

            string idleusers = ((double)away / total).ToString("##,0.00%");
            string idlebots = ((double)awayb / totalb).ToString("##,0.00%");

            string busyusers = ((double)busy / total).ToString("##,0.00%");
            string busybots = ((double)busyb / totalb).ToString("##,0.00%");

            string offlineusers = ((double)offline / total).ToString("##,0.00%");
            string offlinebots = ((double)offlineb / totalb).ToString("##,0.00%");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{EmojiIndex.Counter.Pack(a)} {guild.Users.Count.ToPlaceValue()}");

            sb.AppendLine($"\n{EmojiIndex.User.Pack(a)} {users.Count().ToPlaceValue()} ({activeusers})");
            if (online > 0) sb.AppendLine($"**Online** | {online.ToPlaceValue()} ({onlineusers})"); // {EmojiIndex.Online} 
            if (away > 0) sb.AppendLine($"**Idle** | {away.ToPlaceValue()} ({idleusers})"); // {EmojiIndex.Away} 
            if (busy > 0) sb.AppendLine($"**Busy** | {busy.ToPlaceValue()} ({busyusers})"); // {EmojiIndex.Busy} 

            sb.AppendLine($"\n{EmojiIndex.Bot.Pack(a)} {bots.Count().ToPlaceValue()} ({activebots})");
            if (onlineb > 0) sb.AppendLine($"**Online** | {onlineb.ToPlaceValue()} ({onlinebots})");
            if (awayb > 0) sb.AppendLine($"**Idle** | {awayb.ToPlaceValue()} ({idlebots})");
            if (busyb > 0) sb.AppendLine($"**Busy** | {busyb.ToPlaceValue()} ({busybots})");

            // Counter: online/total => :bot: 15/20
            e.WithDescription(sb.ToString());


            await ReplyAsync(embed: e.Build());
        }

        public string GetVerifiedInfo(VerificationLevel level)
        {
            switch (level)
            {
                case VerificationLevel.Extreme:
                    return "**Extreme** | ┻━┻彡 ヽ(ಠ益ಠ)ノ彡┻━┻";
                case VerificationLevel.High:
                    return "**High** | (╯°□°）╯︵ ┻━┻";
                case VerificationLevel.Medium:
                    return "**Medium** | \\(°□°)/";
                case VerificationLevel.Low:
                    return "**Low** | (ノ゜-゜)ノ";
                default:
                    return "**None** | (づ￣ ³￣)づ";
            }
        }

        [Command("guild"), Alias("g")]
        [Summary("Gets information about a specified guild.")]
        public async Task GetGuildInfo([Remainder]string reference = null)
        {
            
            OldAccount a = Context.Account;
            SocketGuild guild = Context.Guild;
            if (reference != null)
            {
                if (!Context.TryGetGuild(reference, out guild))
                {
                    await ReplyAsync(embed: EmbedData.Throw(Context, "This server doesn't exist."));
                    return;
                }

            }

            EmbedBuilder e = new EmbedBuilder();
            EmbedFooterBuilder f = new EmbedFooterBuilder();
            e.WithColor(EmbedData.GetColor("origreen"));
            e.WithTitle($"{guild.Name}");

            const string GUILD_ICON = "https://cdn.discordapp.com/icons/{0}/{1}?size=256";

            string link = null;
            if (guild.IconId.Exists())
            {
                link = string.Format(GUILD_ICON, guild.Id, guild.IconId);
            }
            link = link ?? Context.User.GetDefaultAvatarUrl();
            e.WithThumbnailUrl(link);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"[{EmojiIndex.Identifier.Pack(a)}]({link}) {guild.Id}");
            sb.AppendLine($"[{EmojiIndex.FromHours(guild.CreatedAt.Hour).Pack(a)}]({link}) {guild.CreatedAt.ToString("M/d/yyyy @ hh:mm tt")}");
            sb.AppendLine($"[{EmojiIndex.Image.Pack(a)}]({link}) {guild.Owner}");
            sb.AppendLine($"[{EmojiIndex.Verification.Pack(a)}]({link}) {GetVerifiedInfo(guild.VerificationLevel)}");
            sb.AppendLine($"[{EmojiIndex.Region.Pack(a)}]({link}) {guild.VoiceRegionId}");
            IEnumerable<SocketGuildUser> users = guild.Users.Where(x => !x.IsBot);
            IEnumerable<SocketGuildUser> bots = guild.Users.Where(x => x.IsBot);
            string activeusers = ((double)users.Where(x => !x.IsOffline()).Count() / users.Count()).ToString("##,0.0%");
            string activebots = ((double)bots.Where(x => !x.IsOffline()).Count() / bots.Count()).ToString("##,0.0%");

            string footer = $"{EmojiIndex.User} {users.Count().ToPlaceValue()} ({activeusers})";
            if (guild.HasBots()) footer += $" | {EmojiIndex.Bot} {bots.Count().ToPlaceValue()} ({activebots})";

            f.WithText(footer);

            e.WithDescription(sb.ToString());
            e.WithFooter(f);

            await ReplyAsync(embed: e.Build());
        }

        [Command("invite")]
        [Summary("Invite Orikivo to your own server.")]
        public async Task GetInvite()
        {
            EmbedBuilder e = new EmbedBuilder();
            e.Color = EmbedData.GetColor("error");
            e.Title =
                $"**Invite me to your server!** ({_config["appearance:invite"]})\n" +
                $"**Join the community server!** ({_config["servers:dev"]})\n";

            await ReplyAsync(embed: e.Build());
        }

        [Command("user"), Alias("u")]
        [Summary("Requests and displays information about an underlying user.")]
        public async Task GetUserInfo([Remainder]SocketGuildUser u = null)
        {
            u = u ?? Context.GetGuildUser();
            Embed e = GenerateGuildUserEmbed(u);
            await ReplyAsync(embed: e);

        }

        //[Group("clipboard"), Name("clipboard"), Alias("cb")]
        //[Summary("save your funniest meme storage for this, or other stuff its up to youuuuuuuuu")]

        [Command("emoji"), Alias("e")]
        [Summary("use an emote")]
        public async Task UseEmote(ulong id)
        {
            foreach (var guild in Context.Client.Guilds)
            {
                if (guild.TryGetEmote(id, out GuildEmote em))
                {
                    await ReplyAsync($"{em}");
                    return;
                }
            }
            await ReplyAsync(embed: EmbedData.Throw(Context, "This emoticon identifier was not found in the collection."));
        }

        // send trade offers
        // items in trade offers are usage locked.
        // if the other item on their end is used, the trade is cancelled.
        // be able to accept or decline. accepting takes the item from their inventory, and adds it into yours.

        [Command("emoji"), Alias("e")]
        [Summary("use an emote")]
        public async Task UseEmote([Remainder]string name)
        {
            foreach (var guild in Context.Client.Guilds)
            {
                if (guild.TryGetEmote(name, out GuildEmote em))
                {
                    await ReplyAsync($"{em}");
                    return;
                }
            }
            await ReplyAsync(embed: EmbedData.Throw(Context, "The emoticon name provided did not exist."));
        }

        //[Command("emojilarge"), Alias("el"), Priority(0)]
        //[Summary("send the most recent emote from the last five messages.")]

        // regex info https://www.rexegg.com/regex-quickstart.html
        // <@USER_ID>
        // <@!USER_ID>
        // <#CHANNEL_ID>
        // <@&ROLE_ID>
        // <:NAME:ID>
        // <a:NAME:ID>

        [Command("emojilarge"), Alias("el"), Priority(1)]
        [Summary("use an emote from an id.")]
        public async Task UseLargeEmote(ulong id)
        {
            const string EMOJI_URL = "https://cdn.discordapp.com/emojis/{0}";
            string url = string.Format(EMOJI_URL, id);

            WebResponse state = _ori.GetAsync(url).Result;

            if (!state.IsSuccess)
            {
                await ReplyAsync(embed: EmbedData.Throw(Context, "An emoji with this ID could not be found."));
                return;
            }

            var e = EmbedData.DefaultEmbed;
            e.WithImageUrl(url);

            await ReplyAsync(embed: e.Build());
        }

        [Command("emojilarge"), Alias("el"), Priority(0)]
        [Summary("use an emote")]
        public async Task UseLargeEmote(string name)
        {
            if (name.TryParseEmote(out Emote emote))
            {
                await GenerateLargeEmojiBox(emote);
                return;
            }

            foreach (var guild in Context.Client.Guilds)
            {
                if (guild.TryGetEmote(name, out GuildEmote em))
                {
                    await GenerateLargeEmojiBox(em);
                    return;
                }
            }

            await ReplyAsync(embed: EmbedData.Throw(Context, "The emoticon name provided did not exist."));
        }

        public async Task GenerateLargeEmojiBox(Emote em)
        {
            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithImageUrl(em.Url);
            await ReplyAsync(embed: e.Build());
        }

        [Command("emoticon"), Alias("em"), Priority(1)]
        public async Task GetEmoteInfoResponseAsync(ulong id)
        {
            foreach (var guild in Context.Client.Guilds)
            {
                if (guild.TryGetEmote(id, out GuildEmote em))
                {
                    Embed e = GenerateEmoticonBox(em);
                    await ReplyAsync(embed: e);
                    return;
                }
            }

            await ReplyAsync(embed: EmbedData.Throw(Context, "The emoticon ID provided did not exist."));
        }

        [Command("emoticon"), Alias("em"), Priority(0)]
        public async Task GetEmoteInfoResponseAsync(string name)
        {
            foreach (var guild in Context.Client.Guilds)
            {
                if (guild.TryGetEmote(name, out GuildEmote em))
                {
                    Embed e = GenerateEmoticonBox(em);
                    await ReplyAsync(embed: e);
                    return;
                }
                if (name.TryParseEmote(out Emote emote))
                {
                    if (guild.Emotes.Any(x => x == emote))
                    {
                        Embed emb = GenerateEmoticonBox((GuildEmote)emote);
                        await ReplyAsync(embed: emb);
                        return;
                    }

                    Embed embed = GenerateEmoticonBox(emote);
                    await ReplyAsync(embed: embed);
                    return;
                }
            }

            await ReplyAsync(embed: EmbedData.Throw(Context, "The emoticon context provided did not exist."));
        }

        public Embed GenerateEmoticonBox(Emote em)
        {
            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithImageUrl(em.Url);

            //EmbedFooterBuilder f = new EmbedFooterBuilder();
            //f.WithText(em.ToString());
            //e.WithFooter(f);

            StringBuilder sb = new StringBuilder();

            if (em.Animated)
            {
                sb.Append($"{EmojiIndex.Animated} ");
            }
            sb.Append(em.Name);

            e.WithTitle(sb.ToString());
            e.WithDescription(GetEmojiSummary(em));
            return e.Build();
        }

        public string GetEmojiSummary(Emote em)
        {
            OldAccount a = Context.Account;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"[{EmojiIndex.Identifier.Pack(a)}]({em.Url}) {em.Id}");
            sb.AppendLine($"[{EmojiIndex.Image.Pack(a)}]({em.Url}) {em.CreatedAt.ToString("M/d/yyyy @ hh:mm tt")}");
            sb.AppendLine($"[{EmojiIndex.Permission.Pack(a)}]({em.Url}) `{em.ToString()}`");
            return sb.ToString();
        }

        public List<string> GenerateEmoteBaseList(SocketGuild guild)
        {
            List<string> list = new List<string>();
            if (guild.Emotes.Funct())
            {
                foreach (GuildEmote em in guild.Emotes)
                {
                    list.Add(em.GetBaseLine());
                }
            }

            return list;
        }

        [Command("emoticons"), Alias("eml")]
        [Summary("Get an emoji list of a specified guild. use \\s to account for spaces.")]
        public async Task GetEmoteListResponseAsync(string guild, int page = 1)
        {
            const string spacer = "\\s";
            guild = guild.Replace(spacer, " ");
            SocketGuild g = Context.Guild;
            if (guild != null)
            {
                if (!Context.TryGetGuild(guild, out g))
                {
                    await ReplyAsync(embed: EmbedData.Throw(Context, "This server doesn't exist."));
                    return;
                }

            }

            if (!g.HasEmotes())
            {
                await ReplyAsync(embed: EmbedData.Throw(Context, "This guild has no emoticons."));
                return;
            }

            List<string> list = GenerateEmoteBaseList(g);
            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithTitle(GetEmoticonBoxTitle(g));
            Embed r = EmbedData.GenerateEmbedList(list, page, e);
            await ReplyAsync(embed: r);
        }

        [Command("emoticons"), Alias("eml")]
        public async Task GetEmoteListResponseAsync(int page = 1)
        {
            SocketGuild g = Context.Guild;

            List<string> list = GenerateEmoteBaseList(g);
            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithTitle(GetEmoticonBoxTitle(g));
            Embed r = EmbedData.GenerateEmbedList(list, page, e);
            await ReplyAsync(embed: r);
        }

        public string GetEmoticonBoxTitle(SocketGuild guild)
        {
            return $"[{guild.Name}]\n    ↳ Emojis";
        }

        public async Task GetChannelInfoBox(SocketTextChannel t)
        {
            OldAccount a = Context.Account;
            EmbedBuilder e = EmbedData.DefaultEmbed;
            string title = $"{(t.IsNsfw ? $"{EmojiIndex.Nsfw} " : "")}{t.Name}";
            e.WithTitle(title);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{EmojiIndex.Identifier.Pack(a)} {t.Id}");
            sb.AppendLine($"{EmojiIndex.FromHours(t.CreatedAt.Hour).Pack(a)} {t.CreatedAt.ToString("M/d/yyyy @ hh:mm tt")}");
            if (t.Category.Exists()) sb.AppendLine($"{EmojiIndex.Category.Pack(a)} {t.Category.Name}");
            if (!string.IsNullOrWhiteSpace(t.Topic)) sb.AppendLine($"{EmojiIndex.Topic.Pack(a)} {t.Topic}");
            if (t.SlowModeInterval > 0) sb.AppendLine($"{EmojiIndex.SlowMode.Pack(a)} {t.SlowModeInterval}");

            e.WithDescription(sb.ToString());

            await ReplyAsync(embed: e.Build());
        }

        public async Task GetChannelInfoBox(SocketVoiceChannel v)
        {
            OldAccount a = Context.Account;
            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithTitle(v.Name);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{EmojiIndex.Identifier.Pack(a)} {v.Id}");
            sb.AppendLine($"{EmojiIndex.FromHours(v.CreatedAt.Hour).Pack(a)} {v.CreatedAt.ToString("M/d/yyyy @ hh:mm tt")}");
            if (v.Category.Exists()) sb.AppendLine($"{EmojiIndex.Category.Pack(a)} {v.Category.Name}");

            if (v.Users.Funct())
            {
                string talkingusers = $"({(v.Users.Where(x => x.CanSpeak()).Count().ToPlaceValue())} speaking)";
                sb.AppendLine($"\n{EmojiIndex.Deaf.Pack(a)} {v.Users.Count().ToPlaceValue()}{(v.UserLimit.HasValue ? $"**/**{v.UserLimit.Value.ToPlaceValue()}" : "")} {talkingusers}");
            }

            sb.AppendLine($"{EmojiIndex.Bitrate.Pack(a)} {v.Bitrate.ToPlaceValue()} kbps");
            e.WithDescription(sb.ToString());

            await ReplyAsync(embed: e.Build());
        }

        public async Task GetChannelInfoBox(SocketCategoryChannel c)
        {
            OldAccount a = Context.Account;
            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithTitle(c.Name);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{EmojiIndex.Identifier.Pack(a)} {c.Id}");
            sb.AppendLine($"{EmojiIndex.FromHours(c.CreatedAt.Hour).Pack(a)} {c.CreatedAt.ToString("M/d/yyyy @ hh:mm tt")}");
            sb.AppendLine($"{EmojiIndex.Channel.Pack(a)} {c.Channels.Count()}");
            e.WithDescription(sb.ToString());

            await ReplyAsync(embed: e.Build());
        }

        [Command("channel"), Alias("ch")]
        public async Task ViewTextChannelAsync([Remainder]string name = null)
        {
            SocketTextChannel t = Context.Channel as SocketTextChannel;
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (!Context.Guild.TryGetTextChannel(name, out t))
                {
                    await ReplyAsync(embed: EmbedData.Throw(Context, "No text channel uses this alias."));
                    return;
                }
            }

            await GetChannelInfoBox(t);
        }

        [Command("voicechannel"), Alias("vc")]
        public async Task ViewVoiceChannelAsync([Remainder]string name = null)
        {
            SocketVoiceChannel vc = Context.GetGuildUser().VoiceChannel;
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (!Context.Guild.TryGetVoiceChannel(name, out vc))
                {
                    await ReplyAsync(embed: EmbedData.Throw(Context, "No voice channel uses this alias."));
                    return;
                }
            }
            else
            {
                if (!vc.Exists())
                {
                    await ReplyAsync(embed: EmbedData.Throw(Context, "You aren't in any voice channels.", $"To learn more about the voice channels in this server, try using {Context.Server.Config.GetPrefix(Context)}voicechannels.", false));
                    return;
                }
            }

            await GetChannelInfoBox(vc);
        }

        [Command("category"), Alias("cg")]
        public async Task ViewCategoryAsync([Remainder]string name = null)
        {
            SocketCategoryChannel cg = (Context.Channel as SocketTextChannel).Category as SocketCategoryChannel;
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (!Context.Guild.TryGetCategory(name, out cg))
                {
                    await ReplyAsync(embed: EmbedData.Throw(Context, "No category uses this alias."));
                    return;
                }
            }
            else
            {
                if (!cg.Exists())
                {
                    await ReplyAsync(embed: EmbedData.Throw(Context, "The current channel is not placed in a category.", $"To learn more about the categories in your server, try using {Context.Server.Config.GetPrefix(Context)}categories.", false));
                }
            }

            await GetChannelInfoBox(cg);
        }

        [Command("permissions"), Alias("pl"), Priority(4)]
        [Summary("Get permissions from a permission raw value.")]
        public async Task ViewPermissionsAsync(ulong id)
        {
            // id is the mixed number. each guild permission can only be used once.
            List<GuildPermission> perms = new GuildPermissions(id).ToList();
            string names = perms.Enumerate(x => Format.Code(x.ToString())).Conjoin(" ");

            //StringBuilder sb = new StringBuilder();

            //foreach (GuildPermission p in perms)
            //{
             //   sb.AppendLine(p.ToString());
            //}

            await ReplyAsync(embed: names.ToEmbedDescription(EmbedData.DefaultEmbed.WithTitle("**Permissions**")).Build());
        }

        //[Command("permissions"), Alias("pl"), Priority(3)]
        //[Summary("Get permissions from a (r)ole, (u)ser, (c)ategory, (t)ext-channel, or (v)oice-channel. A list of permissions it allows is displayed")]
        public async Task ViewPermissionsAsync(char ctx, [Remainder]string name)
        {
            // order of context search
            // 1. Role
            // 2. User
            // 3. Category
            // 4. Channel
        }

        //upcoming - displays a list of upcoming events...?
        // once you build the event calendars lol

        [Command("permissions"), Alias("pl"), Priority(1)]
        [Summary("View permissions for a user or role, with an optional specified channel.")]
        public async Task ViewPermissionsAsync(string context, [Remainder]string channel = null)
        {
            if (!Context.Guild.TryGetRole(context, out SocketRole r))
            {
                if (!Context.Guild.TryGetUser(context, out SocketGuildUser u))
                {
                    await ReplyAsync(embed: EmbedData.Throw(Context, "No role or user uses this alias.", "Try mentioning the user or role you want to view permissions for.", false));
                    return;
                }

                await GetUserPermissions(u);
                return;
            }

            await GetRolePermissions(r);
        }

        [Command("permissions"), Alias("pl"), Priority(0)]
        [Summary("View your default permissions in this guild.")]
        public async Task ViewPermissionsAsync()
        {
            await GetUserPermissions(Context.GetGuildUser());
        }

        //__________________________________________________

        [Command("permission"), Alias("p"), Priority(1)]
        [Summary("Learn about a permission using its raw value.")]
        public async Task ViewPermissionAsync(ulong raw)
        {
            if (!Context.TryGetGuildPermission(raw, out GuildPermission gp))
            {
                string url = "https://discordapi.com/permissions.html";
                await ReplyAsync(embed: EmbedData.Throw(Context, "No permission has this value.", $"To learn about raw values for permissions, try using a [permissions calculator]({url}).", false));
                return;
            }

            await GetPermissionBoxAsync(gp);
        }

        public async Task GetPermissionBoxAsync(GuildPermission gp)
        {
            OldAccount a = Context.Account;
            EmbedBuilder e = EmbedData.DefaultEmbed;
            EmbedFooterBuilder f = new EmbedFooterBuilder();
            f.WithText($"{EmojiIndex.RawValue.Pack(a)} {(ulong)gp}");
            e.WithTitle(gp.ToString());
            e.WithDescription($"{EmojiIndex.Permission.Pack(a)} {(ulong)gp}\n{EmojiIndex.Topic.Pack(a)} {GetGuildPermissionSummary(gp)}");
            //e.WithFooter(f);

            await ReplyAsync(embed: e.Build());
        }

        public string GetGuildPermissionSummary(GuildPermission gp)
        {
            switch (gp)
            {
                case GuildPermission.ManageEmojis:
                    return "The ability to manage and edit server emojis.";
                case GuildPermission.ManageWebhooks:
                    return "The ability to configure external website updates to a specific channel.";
                case GuildPermission.ManageRoles:
                    return "This allows the user to manage and edit server roles.";
                case GuildPermission.ManageNicknames:
                    return "This grants the ability to give anyone in the server a nickname.";
                case GuildPermission.ChangeNickname:
                    return "This allows a user to change their own username.";
                case GuildPermission.UseVAD:
                    return "This permission toggles the ability to speak in voice channels without having to use Push-to-Talk.";
                case GuildPermission.MoveMembers:
                    return "This gives a user the power to move other users out of voice channels.";
                case GuildPermission.DeafenMembers:
                    return "This allows a user to deafen another user in a voice channel by force.";
                case GuildPermission.MuteMembers:
                    return "This gives a user the ability to mute another user in a voice channel.";
                case GuildPermission.Speak:
                    return "When active, the user that has this permission can speak in voice channels.";
                case GuildPermission.Connect:
                    return "This allows a user to connect to voice channels.";
                case GuildPermission.UseExternalEmojis:
                    return "When enabled, the user with this permission can send emojis outside of the server.";
                case GuildPermission.MentionEveryone:
                    return "When given, the user that has this permission can mention the entire server at any point in time.";
                case GuildPermission.ReadMessageHistory:
                    return "This allows a user to read through a channel's message history.";
                case GuildPermission.AttachFiles:
                    return "This grants a user the ability to send files into channels.";
                case GuildPermission.EmbedLinks:
                    return "When a user has this permission, every link that they send will automatically generate an embed displaying a summary about it, when possible.";
                case GuildPermission.ManageMessages:
                    return "This allows a user to pin and delete messages in a channel.";
                case GuildPermission.SendTTSMessages:
                    return "This gives a user the ability to send Text-to-Speech messages in channels.";
                case GuildPermission.SendMessages:
                    return "This allows a user to send messages in channels.";
                case GuildPermission.ViewChannel:
                    return "When given to a user, they are able to view and visibly see channels.";
                case GuildPermission.PrioritySpeaker:
                    return "When a user has this permission, when they speak in a voice channel, everyone else in the voice chat is automatically lowered.";
                case GuildPermission.ViewAuditLog:
                    return "This grants the user complete viewing permission of the audit log, a documentation of all actions taken in the server.";
                case GuildPermission.AddReactions:
                    return "Allows a user to add new reactions onto a message. Regardless of this permission, any user can still contribute to existing reactions.";
                case GuildPermission.ManageGuild:
                    return "Allows the user with this permission to edit and configure guild settings.";
                case GuildPermission.ManageChannels:
                    return "This allows the user to manage and edit channels in the server.";
                case GuildPermission.Administrator:
                    return "This grants the user an override on all permissions, allowing them to do virtually anything aside from deleting the server.";
                case GuildPermission.BanMembers:
                    return "This gives a user the ability to ban members from the server.";
                case GuildPermission.KickMembers:
                    return "This allows a user to kick members from the server.";
                default:
                    return "The user with this permission can create instant invites to any channel in the server.";
            }
        }

        [Command("permission"), Alias("p"), Priority(0)]
        [Summary("Learn about a permission that was shown in the permissions list.")]
        public async Task ViewPermissionAsync([Remainder]string name)
        {
            if (!Context.TryGetGuildPermission(name, out GuildPermission gp))
            {
                string url = "https://discordapi.com/permissions.html";
                await ReplyAsync(embed: EmbedData.Throw(Context, "No permission has this value.", $"To learn about raw values for permissions, try using a [permissions calculator]({url}).", false));
                return;
            }

            await GetPermissionBoxAsync(gp);
        }

        public EmbedBuilder GetGuildPermissionsBox(GuildPermissions p)
        {
            OldAccount a = Context.Account;
            EmbedBuilder e = EmbedData.DefaultEmbed;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{EmojiIndex.Permissions.Pack(a)} {p.RawValue}\n\n**Permissions**");
            //foreach (GuildPermission gp in p.ToList().OrderByDescending(x => (ulong)x))
            //{
            //    sb.AppendLine($"{gp}");
            //}

            string names = p.ToList().Enumerate(x => Format.Code(x.ToString())).Conjoin(" ");
            sb.Append(names);

            e.WithDescription(sb.ToString());
            return e;
        }

        // these get permissions for this channel if the user is @everyone, by default.
        // To get specific permissions for a role/user in this channel, one must be specified.
        public async Task GetCategoryPermissions(SocketCategoryChannel c, SocketGuildUser u)
        {
            OverwritePermissions? overwrite = c.GetPermissionOverwrite(u);  // tries to get overwritten permissions of a channel with a user.
        }

        public async Task GetCategoryPermissions(SocketCategoryChannel c, SocketRole r)
        {
            OverwritePermissions? overwrite = c.GetPermissionOverwrite(r);  // tries to get overwritten permissions of a channel with a role.
        }

        // these get permissions for this channel if the user is @everyone, by default.
        // To get specific permissions for a role/user in this channel, one must be specified.

        public async Task GetChannelPermissions(SocketTextChannel t, SocketGuildUser u)
        {
            OverwritePermissions? overwrite = t.GetPermissionOverwrite(u);  // tries to get overwritten permissions of a channel with a user.
        }

        public async Task GetChannelPermissions(SocketTextChannel t, SocketRole r)
        {
            OverwritePermissions? overwrite = t.GetPermissionOverwrite(r);  // tries to get overwritten permissions of a channel with a role.
        }

        // these get permissions for this channel if the user is @everyone, by default.
        // To get specific permissions for a role/user in this channel, one must be specified.

        public async Task GetVoiceChannelPermissions(SocketVoiceChannel v, SocketGuildUser u)
        {
            OverwritePermissions? overwrite = v.GetPermissionOverwrite(u); // tries to get overwritten permissions of a channel with a user.
        }

        public async Task GetVoiceChannelPermissions(SocketVoiceChannel v, SocketRole r)
        {
            OverwritePermissions? overwrite = v.GetPermissionOverwrite(r); // tries to get overwritten permissions of a channel with a role.
        }

        // This gets the permission values of a specified role.
        public async Task GetRolePermissions(SocketRole r)
        {
            EmbedBuilder e = GetGuildPermissionsBox(r.Permissions);
            e.WithTitle(r.Name);

            await ReplyAsync(embed: e.Build());
        }

        // This gets the permission Values of a guild user.
        public async Task GetUserPermissions(SocketGuildUser u)
        {
            EmbedBuilder e = GetGuildPermissionsBox(u.GuildPermissions);
            e.WithTitle(u.Username);

            await ReplyAsync(embed: e.Build());
        }

        [Command("role"), Alias("r")]
        [Summary("View more about a role in the guild.")]
        public async Task GetRoleInfo([Remainder]string role)
        {
            OldAccount a = Context.Account;
            SocketGuild g = Context.Guild;
            if (!g.TryGetRole(role, out SocketRole r))
            {
                await ReplyAsync(embed: EmbedData.Throw(Context, $"{g.Name} does not have this role.", "Maybe another server has it?"));
                // offer to build a role with the name, or check in other servers.
                return;
            }

            string hex = r.Color.GetHexCode();

            string y = "✅";
            string n = "❌";

            string everyone = r.IsEveryone ? y : n;
            string hoisted = r.IsHoisted ? y : n;
            string managed = r.IsManaged ? y : n;
            string mention = r.IsMentionable ? y : n;

            // have members of a role be searched such as [ul <role-name>
            // have role permissions be searched with [pl <role-name>

            EmbedBuilder e = new EmbedBuilder();
            EmbedFooterBuilder f = new EmbedFooterBuilder();
            int users = Context.Guild.Users.Where(x => x.HasRole(r)).Count();
            if (users > 0)
            {
                string userswithrole = ((double)Context.Guild.Users.Where(x => x.HasRole(r)).Count() / Context.Guild.Users.Count()).ToString("##,0.0%");
                f.WithText($"{EmojiIndex.User} {users.ToPlaceValue()} ({userswithrole})");
                e.WithFooter(f);
            }

            StringBuilder title = new StringBuilder();
            if (r.IsHoisted) title.Append($"{EmojiIndex.Hoisted} ");
            title.Append(r.Name);
            e.WithTitle(title.ToString());

            string url = "https://www.google.com";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"[{EmojiIndex.Identifier.Pack(a)}]({url}) {r.Id}");
            sb.AppendLine($"[{EmojiIndex.FromHours(r.CreatedAt.Hour).Pack(a)}]({url}) {r.CreatedAt.ToString("M/d/yyyy @ hh:mm tt")}");

            sb.AppendLine($"[{EmojiIndex.Color.Pack(a)}]({url}) RGB ({r.Color.R}, {r.Color.G}, {r.Color.B})");
            sb.AppendLine($"[{EmojiIndex.Hex.Pack(a)}]({url}) {hex}");

            //sb.AppendLine($"[{EmojiIndex.User.Pack()}]({url}) {r.Members.Count().ToPlaceValue()}");
            sb.AppendLine($"[{EmojiIndex.Permissions.Pack(a)}]({url}) {r.Permissions.RawValue}");
            
            e.WithColor(r.Color);

            e.WithDescription(sb.ToString());

            await ReplyAsync(embed: e.Build());
            }

        //stats stuff
        [Name("Analysis")]
        [Summary("Provides statistics on a range of objects.")]
        public class AnalyticsModule
        {
            public AnalyticsModule()
            {

            }

            //[Command("analyze")]
            public async Task AnalyzeObjectResponseAsync()
            {

            }



            /*
                    [Command("npxlacc")]
                    [Summary("A rewritten pixel profile tool.")]
                    public async Task BuildCardAsync()
                    {
                        string card = PixelEngine.GetCardAsPath(Context.User, new PixelRenderingOptions(Context.Account));
                        EmbedBuilder e = new EmbedBuilder();
                        e.WithImageUrl($"attachment://{Path.GetFileName(card)}");
                        await Context.Channel.SendFileAsync(card, embed: e.Build());
                    }
            */
        }
    }
}
