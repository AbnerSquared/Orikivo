
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Text;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using Discord.WebSocket;
using Orikivo.Networking;

namespace Orikivo.Wrappers
{
    public class LockedDblWrapper : DblWrapper
    {
        private readonly string _token;
        private readonly ulong _id;
        public LockedDblWrapper(ulong id, string token) : base()
        {
            _id = id;
            _token = token;
            _webClient.WithKey(_token);
        }
        public LockedDblWrapper(DiscordSocketClient client, string token) : base()
        {
            _id = client.CurrentUser.Id;
            _token = token;
            _webClient.WithKey(_token);
        }

        public async Task<IDblClient> GetClientAsync()
        {
            DblClient c = await GetBotAsync<DblClient>(_id);
            c.Service = this;
            return c;
        }
        public async Task UpdateStatsAsync(int guildCount)
            => await UpdateStatsAsync(new GuildCountObject(guildCount));
        public async Task UpdateStatsAsync(int shardId, int shardCount, params int[] shards)
            => await UpdateStatsAsync(new ShardedGuildCountObject(shardId, shardCount, shards));
        public async Task UpdateStatsAsync(int guildCount, int shardCount)
            => await UpdateStatsAsync(new ShardedGuildCountObject(shardCount, guildCount));

        protected async Task UpdateStatsAsync(object obj)
        {
            "dbl is updating stats...".Debug();
            string json = JsonConvert.SerializeObject(obj);
            json.Debug("stat info");
            WebResponse result = await _webClient.PostAsync(Builder.WithEndpoint($"bots/{_id}/stats").Clear().Read(), json);
            result.IsSuccess.Debug();
            result.Content.ReadAsStringAsync().Result.Debug();
            "stats updated..".Debug();
        }

        public async Task<List<IDblEntity>> GetVotersAsync()
            => (await GetVotersAsync<DblEntity>()).Cast<IDblEntity>().ToList();

        public async Task<List<T>> GetVotersAsync<T>()
            => await GetAsync<List<T>>(Builder.WithEndpoint($"bots/{_id}/votes").Clear().Read());

        public async Task<bool> HasVotedAsync(ulong id)
            => (await GetAsync<DblVoted>(Builder.WithEndpoint($"bots/{_id}/check").Clear().WithQuery("userId", $"{id}").Read())).Voted;
    }

    public class DblVoted
    {
        [JsonProperty("voted")]
        internal int _voted;
        public bool Voted { get { return _voted == 1; } }
    }

    public class DblClient : DblBot, IDblClient
    {
        public async Task<List<IDblEntity>> GetVotersAsync()
            => await ((LockedDblWrapper) Service).GetVotersAsync();

        public async Task<bool> GetVoteStatusAsync(ulong id)
            => await ((LockedDblWrapper)Service).HasVotedAsync(id);

        public async Task<bool> GetWeekendStatusAsync()
            => await ((LockedDblWrapper)Service).GetWeekendAsync();
        public async Task UpdateStatsAsync(int guildCount)
            => await ((LockedDblWrapper)Service).UpdateStatsAsync(guildCount);
        public async Task UpdateStatsAsync(int[] shards)
            => await ((LockedDblWrapper)Service).UpdateStatsAsync(0, shards.Length, shards);
        public async Task UpdateStatsAsync(int shardCount, int totalShards, params int[] shards)
            => await ((LockedDblWrapper)Service).UpdateStatsAsync(shardCount, totalShards, shards);
    }

    public class DblWrapper
    {
        internal OriWebClient _webClient;
        public static string BaseUrl = "https://discordbots.org/api/";
        public static UriBuilder Builder = new UriBuilder(BaseUrl);

        public DblWrapper()
        {
            _webClient = new OriWebClient();
        }

        public async Task<IDblBotStats> GetStatsAsync(ulong id)
        {
            IDblBotStats r = await GetAsync<DblBotStats>(Builder.WithEndpoint($"bots/{id}/stats").Clear().Read());
            return r;
        }

        public async Task<IDblUser> GetUserAsync(ulong id)
            => await GetAsync<DblUser>(Builder.WithEndpoint($"users/{id}").Clear().Read());

        public async Task<IDblBot> GetBotAsync(ulong id)
            => await GetBotAsync<DblBot>(id);

        internal async Task<T> GetBotAsync<T>(ulong id) where T : DblBot
        {
            T t = await GetAsync<T>(Builder.WithEndpoint($"bots/{id}").Clear().Read());
            if (t.Exists())
                t.Service = this;
            return t;
        }

        public async Task<ISearchResult<IDblBot>> GetBotsAsync(int count = 50, int page = 0)
        {
            DblQuery r = await GetAsync<DblQuery>(Builder.WithEndpoint("bots").Clear().Read());


            return r;
            //BotListQuery q = await GetAsync<BotListQuery>("bots");
        }

        public async Task<bool> GetWeekendAsync()
        {
            bool b = (await GetAsync<DblWeekend>(Builder.WithEndpoint("weekend").Clear().Read())).Weekend;
            return b;
        }

        protected async Task<T> GetAsync<T>(string url)
        {
            WebResponse<T> t = await _webClient.RequestAsync<T>(url);
            if (t.IsSuccess)
                return t.Data;
            return default;
        }

    }

    public class GuildCountObject
    {
        [JsonProperty("server_count")]
        public int GuildCount { get; set; }
        public GuildCountObject(int guildCount)
        {
            GuildCount = guildCount;
        }
    }

    public class ShardedGuildCountObject
    {
        public ShardedGuildCountObject(int shardCount, int guildCount)
        {
            ShardCount = shardCount;
            GuildCount = guildCount;
        }
        public ShardedGuildCountObject(int shardId, int shardCount, int[] shards)
        {
            ShardId = shardId;
            ShardCount = shardCount;
            Shards = shards;
        }

        [JsonProperty("shards")]
        public int[] Shards { get; set; }

        [JsonProperty("shard_id")]
        public int ShardId { get; set; }

        [JsonProperty("shard_count")]
        public int ShardCount { get; set; }

        [JsonProperty("server_count")]
        public int GuildCount { get; set; }
    }

    public interface IDblBotStats
    {
        IReadOnlyList<int> Shards { get; }
        int GuildCount { get; }
        int ShardCount { get; }
    }

    public class DblBotStats : ShardedObject, IDblBotStats
    {

        public int GuildCount => guildCount;

        //[JsonProperty("shards")]
        public IReadOnlyList<int> Shards => shards;

        //[JsonProperty("shard_count")]
        public int ShardCount => shardCount;

        [JsonProperty("server_count")]
        private int guildCount { get; set; }
    }

    public class ShardedObject
    {
        [JsonProperty("shards")]
        public int[] shards;
        [JsonProperty("shard_count")]
        public int shardCount;
    }

    public class DblEntity : IDblEntity
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("discriminator")]
        public string Discriminator { get; set; }

        [JsonProperty("avatar")]
        internal string _avatarId;

        [JsonProperty("defAvatar")]
        internal string _defaultAvatarId;

        public string AvatarUrl { get { return string.IsNullOrWhiteSpace(_avatarId) ? $"https://discordapp.com/assets/{_defaultAvatarId}.png" : $"https://cdn.discordapp.com/avatars/{Id}/{_avatarId}.png"; } }
    }

    public class DblWeekend
    {
        [JsonProperty("is_weekend")]
        public bool Weekend { get; set; }
    }

    public class DblBot : DblEntity, IDblBot
    {
        public DblWrapper Service;

        [JsonProperty("lib")]
        public string Library { get; set; }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("shortdesc")]
        public string FlavorText { get; set; }

        [JsonProperty("longdesc")]
        public string Description { get; set; }

        [JsonProperty("date")]
        public DateTime ApprovedAt { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("owners")]
        public List<ulong> Owners { get; set; }

        [JsonProperty("guilds")]
        public List<ulong> Guilds { get; set; }

        [JsonProperty("website")]
        public string WebsiteUrl { get; set; }

        [JsonProperty("monthlypoints")]
        public int MonthlyUpvotes { get; set; }

        [JsonProperty("donatebotguildid")]
        public string DonateBotGuildId { get; set; }

        [JsonProperty("support")]
        public string SupportGuildId { get; internal set; }

        public string SupportUrl { get { return $"{"https://discord.gg/"}{SupportGuildId}"; } }

        [JsonProperty("github")]
        public string GithubUrl { get; set; }

        [JsonProperty("invite")]
        public string CustomInvite { get; internal set; }
        public string InviteUrl { get { return CustomInvite ?? new UriBuilder("https://discordapp.com/oauth2/authorize", "", ("client_id", $"{Id}"), ("scope", "bot")).Read(); } }

        [JsonProperty("vanity")]
        public string Vanity { get; internal set; }

        public string VanityUrl { get { return $"{"https://discordbots.org/bot/"}{(string.IsNullOrWhiteSpace(Vanity)?Id.ToString():Vanity)}"; } }

        [JsonProperty("certifiedBot")]
        public bool IsCertified { get; set; }

        [JsonProperty("points")]
        public int Upvotes { get; set; }

        public async Task<IDblBotStats> GetStatsAsync()
            => await Service.GetStatsAsync(Id);
    }

    public interface IDblBot : IDblEntity
    {
        string Library { get; }
        string Prefix { get; }
        string FlavorText { get; }
        string Description { get; }
        List<string> Tags { get; }
        string WebsiteUrl { get; }
        string SupportUrl { get; }
        string GithubUrl { get; }
        List<ulong> Owners { get; }
        string InviteUrl { get; }
        DateTime ApprovedAt { get; }
        bool IsCertified { get; }
        string VanityUrl { get; }
        int Upvotes { get; }
        int MonthlyUpvotes { get; }
        string DonateBotGuildId { get; }
        Task<IDblBotStats> GetStatsAsync();
    }

    public class DblUser : DblEntity, IDblUser
    {
        [JsonProperty("bio")]
        public string Biography { get; set; }

        [JsonProperty("banner")]
        public string BannerUrl { get; set; }

        [JsonProperty("social")]
        public DblUserSocial Social { get; set; }

        [JsonProperty("color")]
        public string ColorHex { get; set; }

        public Color Color { get { return ColorTranslator.FromHtml(ColorHex); } } // HexCode.ToColor();

        [JsonProperty("supporter")]
        public bool IsSupporter { get; set; }

        [JsonProperty("certifiedDev")]
        public bool IsCertified { get; set; }

        [JsonProperty("mod")]
        public bool IsMod { get; set; }

        [JsonProperty("webMod")]
        public bool IsWebMod { get; set; }

        [JsonProperty("admin")]
        public bool IsAdmin { get; set; }

        public string VanityUrl { get { return $"https://discordbots.org/user/{Id}"; } }
    }

    public class UriBuilder
    {
        public UriBuilder() { }

        public UriBuilder(string url, string endpoint = "")
        {
            Base = url;
            Endpoint = endpoint;
        }

        public UriBuilder(string url, string endpoint, string k, string v)
        {
            Base = url;
            Endpoint = endpoint;
            Add(k, v);
        }

        public UriBuilder(string url, string endpoint, params (string k, string v)[] query)
        {
            Base = url;
            Endpoint = endpoint;
            AddMany(query);
        }

        public static string ToField(string key, string value)
            => $"{key}={value}";
        public static string ToQuery(NameValueCollection query)
        {
            StringBuilder q = new StringBuilder();
            string[] fields = (from k in query.AllKeys from v in query.GetValues(k) select ToField(k, v)).ToArray();
            if (fields.Length > 0)
            {
                q.Append($"?{fields.Conjoin("&")}");
                return q.ToString();
            }
            return "";
        }

        public string Base { get; set; }
        public string Endpoint { get; set; }
        public NameValueCollection Query { get; set; } = new NameValueCollection();

        public void SetEndpoint(string endpoint)
            => Endpoint = endpoint;

        public UriBuilder WithEndpoint(string endpoint)
        {
            SetEndpoint(endpoint);
            return this;
        }

        public UriBuilder Clear()
        {
            Query = new NameValueCollection();
            return this;
        }

        public UriBuilder WithQuery(string key, string value)
        {
            Add(key, value);
            return this;
        }

        public UriBuilder WithManyQueries(params (string k, string v)[] f)
        {
            AddMany(f);
            return this;
        }

        public string ReadQuery()
            => ToQuery(Query);

        public void Add(string k, string v)
            => Query.Add(k, v);

        public void Add((string k, string v) p)
            => Add(p.k, p.v);

        public void AddMany(params (string k, string v)[] f)
        {
            foreach ((string k, string v) p in f)
                Add(p);
        }

        public UriBuilder WithQuery(NameValueCollection q)
        {
            Query = q;
            return this;
        }

        public string Read()
        {
            string r = $"{Base}{Endpoint}{ReadQuery()}";
            r.Debug();
            return r;
        }

        public Uri Build()
            => new Uri(Read());
    }

    public static class WidgetImageFormatExtender
    {
        public static string Read(this WidgetImageFormat format)
        {
            switch (format)
            {
                case WidgetImageFormat.Png:
                    return ".png";
                default:
                    return ".svg";
            }
        }
    }

    public interface IDblEntity
    {
        ulong Id { get; }
        string Username { get; }
        string Discriminator { get; }
        string AvatarUrl { get; }
    }

    public class DblUserSocial
    {
        [JsonProperty("youtube")]
        public string YoutubeId { get; internal set; }

        [JsonProperty("reddit")]
        public string RedditUsername { get; internal set; }

        [JsonProperty("twitter")]
        public string TwitterUsername { get; internal set; }

        [JsonProperty("instagram")]
        public string InstagramUsername { get; internal set; }

        [JsonProperty("github")]
        public string GithubUsername { get; internal set; }

        public string YoutubeUrl { get { return $"{"https://www.youtube.com/channel/"}{YoutubeId}"; } }
        public string RedditUrl { get { return $"{"https://www.reddit.com/u/"}{RedditUsername}"; } }
        public string TwitterUrl { get { return $"{"https://twitter.com/"}{TwitterUsername}"; } }
        public string InstagramUrl { get { return $"{"https://www.instagram.com/"}{InstagramUsername}"; } }
        public string GithubUrl { get { return $"{"https://github.com/"}{GithubUsername}"; } }
    }

    public class SmallWidgetOptions
    {
        public WidgetType Format;
        public WidgetImageFormat ImageFormat;
        public bool ShowAvatar = true;
        public Color? AvatarBackgroundColor;
        private string _avatarBackgroundColor;
        public Color? LeftColor;
        private string _leftColor;
        public Color? LeftTextColor;
        private string _leftTextColor;
        public Color? RightColor;
        private string _rightColor;
        public Color? RightTextColor;
        private string _rightTextColor;
        public ulong BotId;

        public SmallWidgetOptions() { }
        public SmallWidgetOptions(string avatarBackgroundColor = "", string leftColor = "", string leftTextColor = "", string rightColor = "", string rightTextColor = "")
        {
            _avatarBackgroundColor = avatarBackgroundColor;
            _leftColor = leftColor;
            _leftTextColor = leftTextColor;
            _rightColor = rightColor;
            _rightTextColor = rightTextColor;
        }

        public SmallWidgetOptions WithAvatarToggle(bool b)
        {
            ShowAvatar = b;
            return this;
        }
        public SmallWidgetOptions WithBotId(ulong id)
        {
            BotId = id;
            return this;
        }
        public SmallWidgetOptions WithImageFormat(WidgetImageFormat i)
        {
            ImageFormat = i;
            return this;
        }
        public SmallWidgetOptions WithType(WidgetType f)
        {
            Format = f;
            return this;
        }
        public SmallWidgetOptions WithAvatarBackgroundColor(Color c)
        {
            AvatarBackgroundColor = c;
            return this;
        }
        public SmallWidgetOptions WithLeftColor(Color c)
        {
            LeftColor = c;
            return this;
        }
        public SmallWidgetOptions WithLeftTextColor(Color c)
        {
            LeftTextColor = c;
            return this;
        }
        public SmallWidgetOptions WithRightColor(Color c)
        {
            RightColor = c;
            return this;
        }
        public SmallWidgetOptions WithRightTextColor(Color c)
        {
            RightTextColor = c;
            return this;
        }
        public string Read()
           => DblWrapper.Builder.WithEndpoint($"widget/{$"{Format}".ToLower()}/{BotId}{ImageFormat.Read()}").WithQuery(Query).Read();
        public Uri Build()
           => DblWrapper.Builder.WithEndpoint($"widget/{$"{Format}".ToLower()}/{BotId}{ImageFormat.Read()}").WithQuery(Query).Build();
        public NameValueCollection Query
        {
            get
            {
                NameValueCollection q = new NameValueCollection();
                if (ShowAvatar)
                {
                    if (AvatarBackgroundColor.HasValue)
                        q.Add("avatarbg", AvatarBackgroundColor.Value.GetHexCode());
                    else if (WidgetHelper.IsValidHex(_avatarBackgroundColor))
                        q.Add("avatarbg", _avatarBackgroundColor);

                }
                else
                    q.Add("noavatar", "true");

                if (LeftColor.HasValue)
                    q.Add("leftcolor", LeftColor.Value.GetHexCode());
                else if (WidgetHelper.IsValidHex(_leftColor))
                    q.Add("leftcolor", _leftColor);

                if (LeftTextColor.HasValue)
                    q.Add("lefttextcolor", LeftTextColor.Value.GetHexCode());
                else if (WidgetHelper.IsValidHex(_leftTextColor))
                    q.Add("lefttextcolor", _leftTextColor);

                if (RightColor.HasValue)
                    q.Add("rightcolor", RightColor.Value.GetHexCode());
                else if (WidgetHelper.IsValidHex(_rightColor))
                    q.Add("rightcolor", _rightColor);

                if (RightTextColor.HasValue)
                    q.Add("righttextcolor", RightTextColor.Value.GetHexCode());
                else if (WidgetHelper.IsValidHex(_rightTextColor))
                    q.Add("righttextcolor", _rightTextColor);
                return q;
            }
        }
    }
    public class LargeWidgetOptions
    {
        public WidgetImageFormat ImageFormat;
        public ulong BotId;
        public Color? TopColor;
        private string _topColor;
        public Color? MiddleColor;
        private string _middleColor;
        public Color? UsernameColor;
        private string _usernameColor;
        public Color? CertifiedColor;
        private string _certifiedColor;
        public Color? DataColor;
        private string _dataColor;
        public Color? LabelColor;
        private string _labelColor;
        public Color? HighlightColor;
        private string _highlightColor;

        public LargeWidgetOptions() {}
        public LargeWidgetOptions(string topColor = "", string middleColor = "", string usernameColor = "", string certifiedColor = "", string dataColor = "", string labelColor = "", string highlightColor = "")
        {
            _topColor = topColor.Replace("#", "");
            _middleColor = middleColor.Replace("#", "");
            _usernameColor = usernameColor.Replace("#", "");
            _certifiedColor = certifiedColor.Replace("#", "");
            _dataColor = dataColor.Replace("#", "");
            _labelColor = labelColor.Replace("#", "");
            _highlightColor = highlightColor.Replace("#", "");
        }

        public LargeWidgetOptions WithBotId(ulong id)
        {
            BotId = id;
            return this;
        }

        public LargeWidgetOptions WithImageFormat(WidgetImageFormat f)
        {
            ImageFormat = f;
            return this;
        }

        private void SetTopColor(Color c)
            => TopColor = c;
        public LargeWidgetOptions WithTopColor(Color c)
        {
            SetTopColor(c);
            return this;
        }

        private void SetMiddleColor(Color c)
            => MiddleColor = c;
        public LargeWidgetOptions WithMiddleColor(Color c)
        {
            SetMiddleColor(c);
            return this;
        }

        private void SetUsernameColor(Color c)
            => UsernameColor = c;
        public LargeWidgetOptions WithUsernameColor(Color c)
        {
            SetUsernameColor(c);
            return this;
        }

        private void SetCertifiedColor(Color c)
            => CertifiedColor = c;
        public LargeWidgetOptions WithCertifiedColor(Color c)
        {
            SetCertifiedColor(c);
            return this;
        }

        private void SetDataColor(Color c)
            => DataColor = c;
        public LargeWidgetOptions WithDataColor(Color c)
        {
            SetDataColor(c);
            return this;
        }

        private void SetLabelColor(Color c)
            => LabelColor = c;
        public LargeWidgetOptions WithLabelColor(Color c)
        {
            SetLabelColor(c);
            return this;
        }

        private void SetHighlightColor(Color c)
            => HighlightColor = c;
        public LargeWidgetOptions WithHighlightColor(Color c)
        {
            SetHighlightColor(c);
            return this;
        }

        public string Read()
            => DblWrapper.Builder.WithEndpoint($"widget/{BotId}{ImageFormat.Read()}").WithQuery(Query).Read();

        public Uri Build()
            => DblWrapper.Builder.WithEndpoint($"widget/{BotId}{ImageFormat.Read()}").WithQuery(Query).Build();

        public NameValueCollection Query
        {
            get
            {
                NameValueCollection q = new NameValueCollection();
                if (TopColor.HasValue)
                    q.Add("topcolor", TopColor.Value.GetHexCode());
                else if (WidgetHelper.IsValidHex(_topColor))
                    q.Add("topcolor", _topColor);
                
                if (MiddleColor.HasValue)
                    q.Add("middlecolor", MiddleColor.Value.GetHexCode());
                else if (WidgetHelper.IsValidHex(_middleColor))
                    q.Add("middlecolor", _middleColor);

                if (UsernameColor.HasValue)
                    q.Add("usernamecolor", UsernameColor.Value.GetHexCode());
                else if (WidgetHelper.IsValidHex(_usernameColor))
                    q.Add("usernamecolor", _usernameColor);

                if (CertifiedColor.HasValue)
                    q.Add("certifiedcolor", CertifiedColor.Value.GetHexCode());
                else if (WidgetHelper.IsValidHex(_certifiedColor))
                    q.Add("certifiedcolor", _certifiedColor);

                if (DataColor.HasValue)
                    q.Add("datacolor", DataColor.Value.GetHexCode());
                else if (WidgetHelper.IsValidHex(_dataColor))
                    q.Add("datacolor", _dataColor);

                if (LabelColor.HasValue)
                    q.Add("labelcolor", LabelColor.Value.GetHexCode());
                else if (WidgetHelper.IsValidHex(_labelColor))
                    q.Add("labelcolor", _labelColor);

                if (HighlightColor.HasValue)
                    q.Add("highlightcolor", HighlightColor.Value.GetHexCode());
                else if (WidgetHelper.IsValidHex(_highlightColor))
                    q.Add("highlightcolor", _highlightColor);

                return q;
            }
        }
    }
    public enum WidgetImageFormat
    {
        Svg = 1,
        Png = 2
    }
    public enum WidgetType
    {
        Status = 1,
        Servers = 2,
        Lib = 3,
        Upvotes = 4,
        Owner = 5
    }

    public interface IDblUser : IDblEntity
    {
        string VanityUrl { get; }
        string Biography { get; }
        string BannerUrl { get; }
        DblUserSocial Social { get; }
        string ColorHex { get; }
        bool IsSupporter { get; }
        bool IsCertified { get; }
        bool IsMod { get; }
        bool IsWebMod { get; }
        bool IsAdmin { get; }
    }

    public interface IDblClient : IDblBot
    {
        Task<List<IDblEntity>> GetVotersAsync();
        Task<bool> GetVoteStatusAsync(ulong id);
        Task<bool> GetWeekendStatusAsync();
        Task UpdateStatsAsync(int guildCount);
        Task UpdateStatsAsync(int[] shards);
        Task UpdateStatsAsync(int shardCount, int totalShards, params int[] shards);
    }

    public interface ISearchResult<T>
    {
        List<T> Items { get; }
        int Page { get; }
        int PageLimit { get; }
        int ItemCount { get; }
        int PageCount { get; }
    }

    public class DblQueryOptions
    {
        public int Limit;
        public int Offset;
        public string Search;
        public string Sort;
        public string Fields;
    }

    public class DblQuery : ISearchResult<IDblBot>
    {
        public List<IDblBot> Items => Results.Cast<IDblBot>().ToList();
        public int Page => (int)Math.Ceiling((double)Offset / Limit);
        public int PageLimit => Limit;
        public int ItemCount => Total;
        public int PageCount => (int)Math.Ceiling((double)Limit / Count);

        [JsonProperty("results")]
        public List<DblBot> Results;

        [JsonProperty("limit")]
        public int Limit;

        [JsonProperty("offset")]
        public int Offset;

        [JsonProperty("count")]
        public int Count;

        [JsonProperty("total")]
        public int Total;
    }
}