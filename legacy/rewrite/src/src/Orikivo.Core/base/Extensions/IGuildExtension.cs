using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Orikivo.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SysColor = System.Drawing.Color;

namespace Orikivo
{

    public static class SocketGuildExtension
    {
       
        public static bool TryGetServer(this SocketGuild g, DataContainer d, out Server s)
        {
            return d.TryGetServer(g, out s);
        }

        public static Color ToDiscordColor(this SysColor c)
            => new Color(c.R, c.G, c.B);

        public static bool TryGetPrimaryChatChannel(this OrikivoCommandContext ctx, SocketGuild guild, out SocketTextChannel channel)
        {
            channel = null;
            List<SocketTextChannel> channels = guild.TextChannels.ToList();
            foreach (SocketTextChannel c in channels)
            {
                if (ctx.GetClientChannelPermissions(guild, c).SendMessages)
                {
                    channel = c;
                    return true;
                }
            }
            return false;
        }

        public static bool TryGetEmote(this SocketGuild guild, string name, out Emote emote)
        {
            emote = null;
            if (!guild.Emotes.Any(x => x.HasName(name)))
            {
                return false;
            }

            emote = guild.Emotes.Where(x => x.HasName(name)).First();
            return true;
        }

        public static bool TryParseEmote(this string name, out Emote emote)
        {
            emote = null;
            if (!name.HasEmoji(out string match))
            {
                return false;
            }

            emote = Emote.Parse(match);
            return true;
        }

        public static bool TryGetEmote(this SocketGuild guild, string name, out GuildEmote emote)
        {
            emote = null;
            if (!guild.Emotes.Any(x => x.HasName(name)))
            {
                return false;
            }

            emote = guild.Emotes.Where(x => x.HasName(name)).First();
            return true;
        }

        public static async Task SendRestMessageAsync(this ISocketMessageChannel channel, RestMessage msg)
        {
            // safemessage class for rest messages to easily convert.
            string content = msg.Content ?? UnicodeIndex.Invisible.ToString();
            bool tts = msg.IsTTS;
            Embed e = null;
            if (msg.Embeds.Funct())
            {
                if (msg.Embeds.Any(x => x.IsPureEmbed()))
                {
                    e = msg.Embeds.Where(x => x.IsPureEmbed()).First();
                }
            }

            await channel.SendMessageAsync(content, tts, e);
        }

        public static bool IsPureEmbed(this Embed e)
        {
            if (e.Provider.HasValue)
            {
                return false;
            }

            return true;
        }

        public static bool HasPins(this ISocketMessageChannel channel, out IReadOnlyCollection<RestMessage> pins)
        {
            pins = channel.GetPinnedMessagesAsync().Result;
            if (!pins.Funct())
            {
                return false;
            }

            return true;
        }

        public static bool HasEmotes(this SocketGuild guild)
        {
            return guild.Emotes.Funct();
        }

        public static string GetBaseLine(this GuildEmote emote)
        {
            return $"[{emote}]({emote.Url}) {emote.Name}";
        }

        public static bool HasTextChannel(this SocketGuild g, ulong u)
            => g.TextChannels.Any(x => x.Id == u);

        public static bool HasTextChannel(this SocketGuild g, string s)
            => g.TextChannels.Any(x => x.Name.ToLower() == s.ToLower());

        public static bool HasChannel(this SocketGuild g, ulong u)
            => g.Channels.Any(x => x.Id == u);

        public static bool HasCategory(this SocketGuild g, string s)
            => g.CategoryChannels.Any(x => x.Name.ToLower() == s.ToLower());

        public static bool HasCategory(this SocketGuild g, ulong u)
            => g.CategoryChannels.Any(x => x.Id == u);

        public static bool HasChannel(this SocketGuild g, string s)
            => g.Channels.Any(x => x.Name.ToLower() == s.ToLower());

        public static bool HasVoiceChannel(this SocketGuild g, ulong u)
            => g.VoiceChannels.Any(x => x.Id == u);

        public static bool HasVoiceChannel(this SocketGuild g, string s)
            => g.VoiceChannels.Any(x => x.Name.ToLower() == s.ToLower());


        public static bool TryGetEmote(this SocketGuild guild, ulong id, out GuildEmote emote)
        {
            emote = null;
            if (!guild.Emotes.Any(x => x.HasId(id)))
            {
                return false;
            }

            emote = guild.Emotes.Where(x => x.HasId(id)).First();
            return true;
        }

        public static bool HasId(this Emote emote, ulong id)
        {
            return emote.Id == id;
        }

        public static bool HasName(this Emote emote, string name)
        {
            return emote.Name == name;
        }

        public static SocketGuildUser GetGuildUser(this OrikivoCommandContext ctx)
        {
            return ctx.Guild.GetUser(ctx.User.Id);
        }

        public static bool TryGetCategory(this SocketGuild g, string s, out SocketCategoryChannel cg)
        {
            cg = null;
            if (!g.HasCategory(s))
            {
                return false;
            }

            cg = g.CategoryChannels.Where(x => x.Name.ToLower() == s.ToLower()).First();
            return true;
        }

        public static bool TryGetVoiceChannel(this SocketGuild g, string s, out SocketVoiceChannel c)
        {
            c = null;
            if (!g.HasVoiceChannel(s))
            {
                return false;
            }

            c = g.VoiceChannels.Where(x => x.Name.ToLower() == s.ToLower()).First();
            return true;
        }

        public static bool TryGetTextChannel(this SocketGuild g, string s, out SocketTextChannel c)
        {
            c = null;
            if (!g.HasTextChannel(s))
            {
                return false;
            }

            c = g.TextChannels.Where(x => x.Name.ToLower() == s.ToLower()).First();
            return true;
        }

        public static bool TryGetTextChannel(this SocketGuild g, ulong id, out SocketTextChannel c)
        {
            c = null;
            
            if (!g.HasTextChannel(id))
            {
                return false;
            }

            c = g.GetTextChannel(id);
            return true;
        }

        public static bool HasPermission(this OrikivoCommandContext ctx, ChannelPermission p)
            => ctx.GetClientPermissions().Has(p);

        public static ChannelPermissions GetClientPermissions(this OrikivoCommandContext ctx)
            => ctx.Guild.CurrentUser.GetPermissions((IGuildChannel)ctx.Channel);

        public static ChannelPermissions GetClientChannelPermissions(this OrikivoCommandContext ctx, SocketGuild guild, SocketTextChannel channel)
        {
            return guild.GetUser(ctx.Client.CurrentUser.Id).GetPermissions(channel);
        }

        public static async Task Send(this EmbedBuilder e, OrikivoCommandContext c) =>
           await c.Channel.SendMessageAsync(embed: e.Build());

        public static string CaseAs(this string s, CaseFormat casing)
        {
            switch(casing)
            {
                case CaseFormat.Uppercase:
                    return s.ToUpper();
                case CaseFormat.Lowercase:
                    return s.ToLower();
                default:
                    return s;
            }
        }
        public static bool Equals(this string s, string v, CaseFormat c)
            => s.CaseAs(c).Equals(v.CaseAs(c));
        public static bool Contains(this string s, string v, CaseFormat c)
            => s.CaseAs(c).Contains(v.CaseAs(c));
        public static bool StartsWith(this string s, string v, CaseFormat c)
            => s.CaseAs(c).StartsWith(v.CaseAs(c));
        public static bool EndsWith(this string s, string v, CaseFormat c)
            => s.CaseAs(c).EndsWith(v.CaseAs(c));
        private static IEnumerable<TSource> WhereEquals<TSource, TKey>(this IEnumerable<TSource> t, Func<TSource, TKey> k, string s, CaseFormat c = CaseFormat.Ignore)
            => t.ToDictionary(k).Where(x => x.Key.ToString().Equals(s, c)).Enumerate(x => x.Value);
        private static IEnumerable<TSource> WhereContains<TSource, TKey>(this IEnumerable<TSource> t, Func<TSource, TKey> k, string s, CaseFormat c = CaseFormat.Ignore)
            => t.ToDictionary(k).Where(x => x.Key.ToString().Contains(s, c)).Enumerate(x => x.Value);
        private static IEnumerable<TSource> WhereStartsWith<TSource, TKey>(this IEnumerable<TSource> t, Func<TSource, TKey> k, string s, CaseFormat c = CaseFormat.Ignore)
            => t.ToDictionary(k).Where(x => x.Key.ToString().StartsWith(s, c)).Enumerate(x => x.Value);
        private static IEnumerable<TSource> WhereEndsWith<TSource, TKey>(this IEnumerable<TSource> t, Func<TSource, TKey> k, string s, CaseFormat c = CaseFormat.Ignore)
            => t.ToDictionary(k).Where(x => x.Key.ToString().EndsWith(s, c)).Enumerate(x => x.Value);
        private static IEnumerable<T> WhereEquals<T>(this IEnumerable<T> t, string s, CaseFormat c = CaseFormat.Ignore)
            => t.Where(x => x.ToString().Equals(s, c));
        private static IEnumerable<T> WhereContains<T>(this IEnumerable<T> t, string s, CaseFormat c = CaseFormat.Ignore)
            => t.Where(x => x.ToString().Contains(s, c));
        private static IEnumerable<T> WhereStartsWith<T>(this IEnumerable<T> t, string s, CaseFormat c = CaseFormat.Ignore)
            => t.Where(x => x.ToString().StartsWith(s, c));
        private static IEnumerable<T> WhereEndsWith<T>(this IEnumerable<T> t, string s, CaseFormat c = CaseFormat.Ignore)
            => t.Where(x => x.ToString().EndsWith(s, c));

        public static IEnumerable<TKey> Enumerate<TSource, TKey>(this IEnumerable<TSource> t, Func<TSource, TKey> k)
            => t.Select(k);
        public static IEnumerable<T> Exclude<T>(this IEnumerable<T> t, NullObjectHandling nullHandler = NullObjectHandling.Include)
        {
            switch(nullHandler)
            {
                case NullObjectHandling.Include:
                    return t;
                default:
                    return t.Where(x => x.Exists());
            }
        }
        public static IEnumerable<T> Search<T>(this IEnumerable<T> t, string v, MatchValueHandling? m = null, CaseFormat? c = null)
            => t.Search(new SearchOptions(v, m, c));
        public static IEnumerable<T> Search<T>(this IEnumerable<T> t, SearchOptions options)
        {
            switch (options.Mode)
            {
                case MatchValueHandling.Contains:
                    return t.WhereContains(options.Input, options.Casing);
                case MatchValueHandling.StartsWith:
                    return t.WhereStartsWith(options.Input, options.Casing);
                case MatchValueHandling.EndsWith:
                    return t.WhereEndsWith(options.Input, options.Casing);
                default:
                    return t.WhereEquals(options.Input, options.Casing);
            }
        }
        public static IEnumerable<TSource> Search<TSource, TKey>(this IEnumerable<TSource> t, Func<TSource, TKey> k, string v, MatchValueHandling m = MatchValueHandling.Equals, CaseFormat c = CaseFormat.Ignore)
            => t.Search(k, new SearchOptions(v, m, c));
        public static IEnumerable<TSource> Search<TSource, TKey>(this IEnumerable<TSource> t, Func<TSource, TKey> k, SearchOptions options)
        {
            switch(options.Mode)
            {
                case MatchValueHandling.Contains:
                    return t.WhereContains(k, options.Input, options.Casing);
                case MatchValueHandling.StartsWith:
                    return t.WhereStartsWith(k, options.Input, options.Casing);
                case MatchValueHandling.EndsWith:
                    return t.WhereEndsWith(k, options.Input, options.Casing);
                default:
                    return t.WhereEquals(k, options.Input, options.Casing);
            }
        }
        public static IEnumerable<T> SortBy<T>(this IEnumerable<T> t, OrderDirection d, NullObjectHandling n, bool? c = null)
            => t.SortBy(new OrderOptions(d, n, c));
        public static IEnumerable<T> SortBy<T>(this IEnumerable<T> t, OrderOptions o)
        {
            switch (o.Direction)
            {
                case OrderDirection.Ascending:
                    return t.Exclude(o.Inclusion).OrderBy(x => x, o.Compare);
                default:
                    return t.Exclude(o.Inclusion).OrderByDescending(x => x, o.Compare);
            }
        }
        public static IEnumerable<TSource> SortBy<TSource, TKey>(this IEnumerable<TSource> t, Func<TSource, TKey> k, OrderDirection d = OrderDirection.Descending, NullObjectHandling n = NullObjectHandling.Include, bool? c = false)
            => t.SortBy(k, new OrderOptions(d, n, c));
        public static IEnumerable<TSource> SortBy<TSource, TKey>(this IEnumerable<TSource> t, Func<TSource, TKey> k, OrderOptions o)
        {
            switch (o.Direction)
            {
                case OrderDirection.Ascending:
                    return t.Exclude(o.Inclusion).OrderBy(k, o.Compare);
                default:
                    return t.Exclude(o.Inclusion).OrderByDescending(k, o.Compare);
            }
        }
        public static IEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, bool compare, IComparer<TKey> comparer = null)
            => compare ? source.OrderBy(keySelector, comparer ?? KeyComparer<TKey>.Default) : source.OrderBy(keySelector);
        public static IEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, bool compare, IComparer<TKey> comparer = null)
            => compare ? source.OrderByDescending(keySelector, comparer ?? KeyComparer<TKey>.Default) : source.OrderBy(keySelector);

        /*
        /// <summary>
        /// Returns an enumerable collection using the object's selected key.
        /// </summary>


        ///<summary>
        /// Returns a search where the specified key values meet the criteria of an input.
        /// </summary>


        /// <summary>
        /// Sorts the elements of a sequence in ascending order using a Boolean check for a comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="compare">A System.Boolean`1 defining if the comparer is used.</param>
        /// <param name="comparer">An System.Collections.Generic.IComparer`1 to compare keys.</param>
        /// <returns>An System.Linq.IEnumerable`1 whose elements are sorted in ascending order according to a key.</returns>
        public static IEnumerable<TSource> OrderBy<TSource, TKey>;
            
        /// <summary>
        /// Sorts the elements of a sequence in descending order using a Boolean check for a comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="compare">A System.Boolean`1 defining if the comparer is used.</param>
        /// <param name="comparer">An System.Collections.Generic.IComparer`1 to compare keys.</param>
        /// <returns>An System.Linq.IEnumerable`1 whose elements are sorted in descending order according to a key.</returns>
        public static IEnumerable<TSource> OrderByDescending<TSource, TKey>;
             
             
        */

        // Reference base discord components.
        // Has = A boolean check.
        // By = Orders an enumerable by.
        // With = Selectively gets matching predicates.
        // With = Gets matching results based on if they even have it.
        // AnyContains = Gets matching users based on what you typed. If name contains this.ToLower()
        // AnyMatches = Gets matching users based on what you exactly typed. If name == this;

        #region unneeded
        /// <summary>
        /// Orders the sequence by IUser.Username.
        /// </summary>
        public static IEnumerable<T> ByUsername<T>(this IEnumerable<T> t, bool ascending = false) where T : IUser
            => ascending ? t.OrderBy(x => x.Username) : t.OrderByDescending(x => x.Username);

        // SocketUser Sort


        // SocketGuildUser Sorts
        public static IEnumerable<SocketGuildUser> ByJoinDate(this IEnumerable<SocketGuildUser> users, OrderDirection d = OrderDirection.Descending, NullObjectHandling n = NullObjectHandling.Include)
            => users.SortBy(x => x.JoinedAt, d, n);

        public static IEnumerable<SocketGuildUser> ByHierarchy(this IEnumerable<SocketGuildUser> users, OrderDirection d = OrderDirection.Descending, NullObjectHandling n = NullObjectHandling.Include)
            => users.SortBy(x => x.Hierarchy, d, n);

        public static IEnumerable<SocketGuildUser> ByPermissionValue(this IEnumerable<SocketGuildUser> users, OrderDirection d = OrderDirection.Descending, NullObjectHandling n = NullObjectHandling.Include)
            => users.SortBy(x => x.GuildPermissions.RawValue, d, n);

        public static IEnumerable<SocketGuildUser> ByScreenName(this IEnumerable<SocketGuildUser> users, OrderDirection d = OrderDirection.Descending, NullObjectHandling n = NullObjectHandling.Include)
            => users.SortBy(x => x.GetName(), d, n);

        public static IEnumerable<SocketGuildUser> ByAnyDeafState(this IEnumerable<SocketGuildUser> users, OrderDirection d = OrderDirection.Descending, NullObjectHandling n = NullObjectHandling.Include)
            => users.SortBy(x => x.IsAnyDeafened(), d, n);

        public static IEnumerable<SocketGuildUser> ByAnyMuteState(this IEnumerable<SocketGuildUser> users, OrderDirection d = OrderDirection.Descending, NullObjectHandling n = NullObjectHandling.Include)
            => users.SortBy(x => x.IsAnyMuted(), d, n);

        public static IEnumerable<SocketGuildUser> BySelfDeafState(this IEnumerable<SocketGuildUser> users, OrderDirection d = OrderDirection.Descending, NullObjectHandling n = NullObjectHandling.Include)
            => users.SortBy(x => x.IsSelfDeafened, d, n);

        public static IEnumerable<SocketGuildUser> BySelfMuteState(this IEnumerable<SocketGuildUser> users, OrderDirection d = OrderDirection.Descending, NullObjectHandling n = NullObjectHandling.Include)
            => users.SortBy(x => x.IsSelfMuted, d, n);

        public static IEnumerable<SocketGuildUser> ByDeafState(this IEnumerable<SocketGuildUser> users, OrderDirection d = OrderDirection.Descending, NullObjectHandling n = NullObjectHandling.Include)
            => users.SortBy(x => x.IsDeafened, d, n);

        public static IEnumerable<SocketGuildUser> ByMuteState(this IEnumerable<SocketGuildUser> users, OrderDirection d = OrderDirection.Descending, NullObjectHandling n = NullObjectHandling.Include)
            => users.SortBy(x => x.IsMuted, d, n);

        public static IEnumerable<SocketGuildUser> ByRoleCount(this IEnumerable<SocketGuildUser> users, OrderDirection d = OrderDirection.Descending, NullObjectHandling n = NullObjectHandling.Include)
            => users.SortBy(x => x.Roles.Count, d, n);

        public static IEnumerable<SocketGuildUser> ByHighestRole(this IEnumerable<SocketGuildUser> users, OrderDirection d = OrderDirection.Descending, NullObjectHandling n = NullObjectHandling.Include)
            => users.SortBy(x => x.Roles.GetHighestRole(), d, n);

        public static IEnumerable<T> ById<T>(this IEnumerable<T> t, bool ascending = false) where T : IEntity<ulong>
            => ascending ? t.OrderBy(x => x.Id) : t.OrderByDescending(x => x.Id);

        public static IEnumerable<T> ByCreationDate<T>(this IEnumerable<T> t, bool ascending = false) where T : ISnowflakeEntity
            => ascending ? t.OrderBy(x => x.CreatedAt) : t.OrderByDescending(x => x.CreatedAt);

        public static IEnumerable<T> ByActivityName<T>(this IEnumerable<T> t, bool ascending = false) where T : IPresence
            => ascending ? t.OrderBy(x => x.Activity.Type) : t.OrderByDescending(x => x.Activity.Type);

        public static IEnumerable<T> ByActivityType<T>(this IEnumerable<T> t, bool ascending = false) where T : IPresence
            => ascending ? t.OrderBy(x => x.Activity.Type) : t.OrderByDescending(x => x.Activity.Type);

        public static IEnumerable<T> ByStatus<T>(this IEnumerable<T> t, bool ascending = false) where T : IPresence
            => ascending ? t.OrderBy(x => x.Status) : t.OrderByDescending(x => x.Status);

        public static IEnumerable<T> ByGuildName<T>(this IEnumerable<T> t, bool ascending = false) where T : IGuild
            => ascending ? t.OrderBy(x => x.Name) : t.OrderByDescending(x => x.Name);

        public static IEnumerable<T> ByChannelName<T>(this IEnumerable<T> t, bool ascending = false) where T : IChannel
            => ascending ? t.OrderBy(x => x.Name) : t.OrderByDescending(x => x.Name);

        public static IEnumerable<T> ByRoleName<T>(this IEnumerable<T> t, bool ascending = false) where T : IRole
            => ascending ? t.OrderBy(x => x.Name) : t.OrderByDescending(x => x.Name);

        public static IEnumerable<T> ByPermissionValue<T>(this IEnumerable<T> t, bool ascending = false) where T : IRole
            => ascending ? t.OrderBy(x => x.Permissions.RawValue) : t.OrderByDescending(x => x.Permissions.RawValue);

        public static IEnumerable<T> ByBotStatus<T>(this IEnumerable<T> t, bool ascending = false) where T : IUser
            => ascending ? t.OrderBy(x => x.IsBot) : t.OrderByDescending(x => x.IsBot);

        public static IEnumerable<T> ByDiscriminator<T>(this IEnumerable<T> t, bool ascending = false) where T : IUser
            => ascending ? t.OrderBy(x => x.DiscriminatorValue) : t.OrderByDescending(x => x.DiscriminatorValue);

        public static IEnumerable<T> ByAvatarId<T>(this IEnumerable<T> t, bool ascending = false) where T : IUser
            => ascending ? t.OrderBy(x => x.AvatarId) : t.OrderByDescending(x => x.AvatarId);

        public static IEnumerable<T> ByVerificationLevel<T>(this IEnumerable<T> t, bool ascending = false) where T : IGuild
            => ascending ? t.OrderBy(x => x.VerificationLevel) : t.OrderByDescending(x => x.VerificationLevel);

        public static IEnumerable<T> ByVoiceRegionId<T>(this IEnumerable<T> t, bool ascending = false) where T : IGuild
            => ascending ? t.OrderBy(x => x.VoiceRegionId) : t.OrderByDescending(x => x.VoiceRegionId);

        public static IEnumerable<T> ByIconId<T>(this IEnumerable<T> t, bool ascending = false) where T : IGuild
            => ascending ? t.OrderBy(x => x.IconId) : t.OrderByDescending(x => x.IconId);

        public static IEnumerable<T> ByEmoteCount<T>(this IEnumerable<T> t, bool ascending = false) where T : IGuild
            => ascending ? t.OrderBy(x => x.Emotes.Count()) : t.OrderByDescending(x => x.Emotes.Count());

        public static IEnumerable<T> ByMfaLevel<T>(this IEnumerable<T> t, bool ascending = false) where T : IGuild
            => ascending ? t.OrderBy(x => x.MfaLevel) : t.OrderByDescending(x => x.MfaLevel);

        public static IEnumerable<T> ByExplicitFilter<T>(this IEnumerable<T> t, bool ascending = false) where T : IGuild
            => ascending ? t.OrderBy(x => x.ExplicitContentFilter) : t.OrderByDescending(x => x.ExplicitContentFilter);

        public static IEnumerable<T> ByWebhookCount<T>(this IEnumerable<T> t, bool ascending = false) where T : IGuild
            => ascending ? t.OrderBy(x => x.GetWebhooksAsync().Result.Count) : t.OrderByDescending(x => x.GetWebhooksAsync().Result.Count);

        public static IEnumerable<T> ByMemberCount<T>(this IEnumerable<T> t, bool ascending = false) where T : SocketGuild
            => ascending ? t.OrderBy(x => x.MemberCount) : t.OrderByDescending(x => x.MemberCount);

        public static IEnumerable<T> ByBitrate<T>(this IEnumerable<T> t, bool ascending = false) where T : SocketVoiceChannel
            => ascending ? t.OrderBy(x => x.Bitrate) : t.OrderByDescending(x => x.Bitrate);

        public static IEnumerable<T> ByUserLimit<T>(this IEnumerable<T> t, bool ascending = false) where T : SocketVoiceChannel
            => ascending ? t.OrderBy(x => x.UserLimit) : t.OrderByDescending(x => x.UserLimit);

        public static IEnumerable<T> ByCurrentUsers<T>(this IEnumerable<T> t, bool ascending = false) where T : SocketVoiceChannel
            => ascending ? t.OrderBy(x => x.Users.Count()) : t.OrderByDescending(x => x.Users.Count());

        public static IEnumerable<T> ByRoleCount<T>(this IEnumerable<T> t, bool ascending = false) where T : SocketGuild
            => ascending ? t.OrderBy(x => x.MemberCount) : t.OrderByDescending(x => x.MemberCount);

        public static IEnumerable<T> ByTopic<T>(this IEnumerable<T> t, bool ascending = false) where T : SocketTextChannel
            => ascending ? t.OrderBy(x => x.Topic) : t.OrderByDescending(x => x.Topic);

        //public static IEnumerable<T> By<T>(this IEnumerable<T> t, bool ascend = false)  where T : 
        //    => null;

        public static IEnumerable<T> ByChannelPosition<T>(this IEnumerable<T> t, bool ascending = false) where T : SocketGuildChannel
        => ascending ? t.OrderBy(x => x.Position) : t.OrderByDescending(x => x.Position);

        public static IEnumerable<T> ByTextCategory<T>(this IEnumerable<T> t, bool ascending = false) where T : SocketTextChannel
            => ascending ? t.OrderBy(x => x.Category) : t.OrderByDescending(x => x.Category);

        public static IEnumerable<T> ByVoiceCategory<T>(this IEnumerable<T> t, bool ascending = false) where T : SocketVoiceChannel
            => ascending ? t.OrderBy(x => x.Category) : t.OrderByDescending(x => x.Category);

        public static IEnumerable<T> ByRolePosition<T>(this IEnumerable<T> t, bool ascending = false) where T : SocketRole
            => ascending ? t.OrderBy(x => x.Position) : t.OrderByDescending(x => x.Position);

        public static IEnumerable<T> ByColor<T>(this IEnumerable<T> t, bool ascending = false) where T : SocketRole
            => ascending ? t.OrderBy(x => x.Color) : t.OrderByDescending(x => x.Color);

        public static IEnumerable<T> ByColorHex<T>(this IEnumerable<T> t, bool ascending = false) where T : SocketRole
            => ascending ? t.OrderBy(x => x.Color.GetHexCode()) : t.OrderByDescending(x => x.Color.GetHexCode());

        public static IEnumerable<T> ByColorR<T>(this IEnumerable<T> t, bool ascending = false) where T : SocketRole
            => ascending ? t.OrderBy(x => x.Color.R) : t.OrderByDescending(x => x.Color.R);

        public static IEnumerable<T> ByColorG<T>(this IEnumerable<T> t, bool ascending = false) where T : SocketRole
            => ascending ? t.OrderBy(x => x.Color.G) : t.OrderByDescending(x => x.Color.G);

        public static IEnumerable<T> ByColorB<T>(this IEnumerable<T> t, bool ascending = false) where T : SocketRole
            => ascending ? t.OrderBy(x => x.Color.B) : t.OrderByDescending(x => x.Color.B);

        public static SocketRole GetHighestRole(this IEnumerable<SocketRole> roles)
            => roles.OrderByDescending(x => x.Position).First();

        public static string GetName(this SocketGuildUser u)
            => u.Nickname ?? u.Username;
        #endregion

        public static IEnumerable<IGuildUser> GetAllUsers(this IGuild g)
        {
            g.DownloadUsersAsync();
            return g.GetUsersAsync().Result;
        }

        public static bool HasFeatures(this SocketGuild g)
            => g.Features.Funct();

        public static bool HasSystemChannel(this SocketGuild g)
            => g.SystemChannel.Exists();

        public static int GetChannelCount(this SocketGuild g)
            => g.Channels.Count();

        public static int GetTextChannelCount(this SocketGuild g)
            => g.TextChannels.Count;

        public static int GetVoiceChannelCount(this SocketGuild g)
            => g.VoiceChannels.Count;

        public static int GetCategoryCount(this SocketGuild g)
            => g.CategoryChannels.Count;

        public static int GetRoleCount(this SocketGuild g)
            => g.Roles.Count();

        public static int GetMemberCount(this IGuild g)
            => g.GetAllUsers().Count();



        public static string GetHexCode(this SysColor c)
            => string.Format("{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B);

        public static string GetHexCode(this Color c)
            => string.Format("{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B);

        #region Get Methods
        public static List<SocketGuildUser> GetUsersJoinedBefore(this SocketGuild guild, DateTimeOffset date) =>
            guild.Users.Where(x => x.JoinedAt <= date).ToList();

        public static List<SocketGuildUser> GetUsersJoinedAfter(this SocketGuild guild, DateTimeOffset date) =>
            guild.Users.Where(x => x.JoinedAt >= date).ToList();

        public static List<SocketGuildUser> GetUsersCreatedBefore(this SocketGuild guild, DateTimeOffset date) =>
            guild.Users.Where(x => x.CreatedAt <= date).ToList();

        public static List<SocketGuildUser> GetUsersCreatedAfter(this SocketGuild guild, DateTimeOffset date) =>
            guild.Users.Where(x => x.CreatedAt >= date).ToList();

        public static List<SocketGuildUser> GetUsersByStatus(this SocketGuild guild, UserStatus status) =>
            guild.Users.Where(x => x.Status == status).ToList();

        public static List<SocketGuildUser> GetUsersByActivity(this SocketGuild guild, string name) =>
            guild.Users.Where(x => x.Activity.Name.ToLower() == name.ToLower()).ToList();

        public static List<SocketGuildUser> GetUsersByActivityType(this SocketGuild guild, ActivityType type) =>
            guild.Users.Where(x => x.Activity.Type == type).ToList();
        #endregion

        public static bool HasPermission(this GuildPermissions p, string s, out GuildPermission perm)
        {
            perm = GuildPermission.CreateInstantInvite;

            foreach (GuildPermission gp in p.ToList())
            {
                if (gp.ToString().ToLower() == s.ToLower())
                {
                    perm = gp;
                    return true;
                }
            }

            return false;
        }

        public static bool HasPermission(this GuildPermissions p, ulong raw, out GuildPermission perm)
        {
            perm = GuildPermission.CreateInstantInvite;
            
            if (!Enum.TryParse($"{raw}", out GuildPermission permission))
            {
                return false;
            }
            foreach (GuildPermission gp in p.ToList())
            {
                if (gp == permission)
                {
                    perm = gp;
                    return true;
                }
            }

            return false;
        }

        public static bool HasBots(this SocketGuild g)
        {
            return g.Users.Any(x => x.IsBot);
        }

        public static bool IsOnline(this SocketGuildUser u)
        {
            return u.Status == UserStatus.Online;
        }

        public static bool IsAway(this SocketGuildUser u)
        {
            return u.Status == UserStatus.AFK || u.Status == UserStatus.Idle;
        }

        public static bool IsBusy(this SocketGuildUser u)
        {
            return u.Status == UserStatus.DoNotDisturb || u.Status == UserStatus.DoNotDisturb;
        }

        public static bool IsOffline(this SocketGuildUser u)
        {
            return u.Status == UserStatus.Offline || u.Status == UserStatus.Invisible;
        }

        public static bool TryGetUser(this SocketGuild g, string s, out SocketGuildUser u)
        {
            u = null;
            if (!g.Users.Any(x => x.Id.ToString() == s || x.ToString().ToLower() == s.ToLower() || x.Mention == s || x.Username.ToLower() == s.ToLower() || (x.Nickname.Exists() ? x.Nickname.ToLower() == s.ToLower() : x.Username.ToLower() == s.ToLower())))
                return false;

            u = g.Users.Where(x => x.Id.ToString() == s || x.ToString().ToLower() == s.ToLower() || x.Mention == s || x.Username.ToLower() == s.ToLower() || (x.Nickname.Exists() ? x.Nickname.ToLower() == s.ToLower() : x.Username.ToLower() == s.ToLower())).First();
            return true;
        }

        public static bool TryGetGuildPermission(this OrikivoCommandContext ctx, ulong u, out GuildPermission gp)
        {
            gp = GuildPermission.CreateInstantInvite;
            GuildPermissions perms = GuildPermissions.All;
            if (!perms.HasPermission(u, out gp))
            {
                return false;
            }

            return true;
        }

        public static bool TryGetGuildPermission(this OrikivoCommandContext ctx, string s, out GuildPermission gp)
        {
            gp = GuildPermission.CreateInstantInvite;
            GuildPermissions perms = GuildPermissions.All;
            if (!perms.HasPermission(s, out gp))
            {
                return false;
            }

            return true;
        }

        public static bool HasUser(this SocketGuild guild, SocketUser user) =>
            guild.Users.Contains(user);

        public static bool HasUser(this SocketGuild guild, string s) =>
            guild.Users.Any(x =>
                x.Username.ToLower() == s.ToLower() ||
                x.Nickname.ToLower() == s.ToLower() ||
                x.Mention == s);

        public static bool HasUser(this SocketGuild guild, ulong u) =>
            guild.Users.Any(x => x.Id == u);

        public static SocketUser GetUser(this SocketGuild guild, string s) =>
            guild.Users.FirstOrDefault(x =>
                x.Username.ToLower() == s.ToLower() ||
                x.Nickname.ToLower() == s.ToLower() ||
                x.Mention == s);

        public static bool MentionsUser(this SocketGuild g, string s)
            => g.Users.Any(x => x.Mention == s);

        public static bool HasRole(this SocketGuild g, ulong role)
            => g.Roles.Any(x => x.Id == role);

        public static bool HasRole(this SocketGuild g, string s)
            => g.Roles.Any(x => x.Name.ToLower() == s.ToLower() || x.Mention == s);

        public static bool MentionsRole(this SocketGuild g, string s)
            => g.Roles.Any(x => x.Mention == s);

        public static bool TryGetRole(this SocketGuild g, string s, out SocketRole role)
        {
            role = null;
            if (!g.HasRole(s))
            {
                return false;
            }
            if (g.MentionsRole(s))
            {
                role = g.Roles.Where(x => x.Mention == s).First();
                return true;
            }

            role = g.Roles.Where(x => x.Name.ToLower() == s.ToLower()).First();
            return true;
        }

        public static SocketRole GetRole(this SocketGuild guild, string s) =>
            guild.Roles.FirstOrDefault(x => x.Id.ToString() == s || x.Name.ToLower() == s.ToLower());

        public static Emote GetEmote(this SocketGuild guild, string s) =>
            guild.Emotes.FirstOrDefault(x => x.Name == s);

        public static Emote GetEmote(this SocketGuild guild, ulong u) =>
            guild.Emotes.FirstOrDefault(x => x.Id == u);

        public static SocketChannel GetChannel(this SocketGuild guild, string s) =>
            guild.Channels.FirstOrDefault(x => x.Id.ToString() == s || x.Name.ToLower() == s.ToLower());

        public static SocketTextChannel GetTextChannel(this SocketGuild guild, string s) =>
            guild.TextChannels.FirstOrDefault(x => x.Id.ToString() == s || x.Name.ToLower() == s.ToLower());

        public static SocketVoiceChannel GetVoiceChannel(this SocketGuild guild, string s) =>
            guild.VoiceChannels.FirstOrDefault(x => x.Id.ToString() == s || x.Name.ToLower() == s.ToLower());

        public static SocketCategoryChannel GetCategory(this SocketGuild guild, string s) =>
            guild.CategoryChannels.FirstOrDefault(x => x.Id.ToString() == s || x.Name.ToLower() == s.ToLower());
    }
}