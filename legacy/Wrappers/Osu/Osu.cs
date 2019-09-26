using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Orikivo.Systems.Wrappers.Core.Converters;
using Orikivo.Systems.Wrappers.Core.Handlers;
using Orikivo.Systems.Wrappers.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo.Systems.Wrappers.Osu
{
    public class OsuWrapper
    {
        private readonly string _key;
        private readonly IWebsiteHandler _site = new WebsiteHandler();

        /* API Base Calls */
        public static string baseUrl = "https://osu.ppy.sh/api/";
        public static string beatmap = "get_beatmaps";
        public static string user = "get_user";
        public static string userBest = $"{user}_best";
        public static string userRecent = $"{user}_recent";
        public static string score = "get_scores";
        public static string match = "get_match";
        public static string replay = "get_replay";

        public OsuWrapper (string key = "")
        {
            _key = key;
        }

        public async Task<OsuBeatmapResult> GetBeatmaps(OsuBeatmapParameters parameters)
        {
            var collection = new NameValueCollection
            {
                { "k", $"{_key}" }
            };

            /* Parameter Check (Must be written inside a seperate class to prevent sloppy code such as this.) */
            if (parameters.Since != null)
            {
                collection.Add("since", $"{parameters.Since}");
            }
            if (parameters.BeatmapSetId != null)
            {
                collection.Add("s", $"{parameters.BeatmapSetId}");
            }
            if (parameters.BeatmapId != null)
            {
                collection.Add("b", $"{parameters.BeatmapId}");
            }
            if (parameters.User != null)
            {
                collection.Add("u", $"{parameters.User}");
            }
            if (parameters.Type != null)
            {
                collection.Add("type", $"{parameters.Type}");
            }
            if (parameters.Mode != null)
            {
                collection.Add("m", $"{parameters.Mode}");
            }
            if (parameters.IncludeConvertedBeatmaps != null)
            {
                collection.Add("a", $"{parameters.IncludeConvertedBeatmaps}");
            }
            if (parameters.BeatmapHash != null)
            {
                collection.Add("h", $"{parameters.BeatmapHash}");
            }
            if (parameters.Limit != null)
            {
                collection.Add("limit", $"{parameters.Limit}");
            }
            
            var uri = new Uri($"{baseUrl}{beatmap}{ConvertUri.ToUrlString(collection)}");
            var url = await _site.GetUri(uri);
            if (!url.IsSuccess)
            {
                throw new WebException($"Osu!API encountered an error while: {beatmap.ToUpper()}");
            }
            var jsonData = "{\n\"beatmaps\":" + url.JsonData +"}";
            return JsonConvert.DeserializeObject<OsuBeatmapResult>(jsonData);
        }

        public async Task<OsuUserResult> GetUser(OsuUserParameters parameters)
        {
            var collection = new NameValueCollection
            {
                { "k", $"{_key}" },
                { "u", $"{parameters.User}" }
            };

            /* Parameter Check (Must be written inside a seperate class to prevent sloppy code such as this.) */
            if (parameters.Mode != null)
            {
                collection.Add("m", $"{parameters.Mode}");
            }
            if (parameters.Type != null)
            {
                collection.Add("type", $"{parameters.Type}");
            }
            if (parameters.EventDays != null)
            {
                collection.Add("event_days", $"{parameters.EventDays}");
            }

            var uri = new Uri($"{baseUrl}{user}{ConvertUri.ToUrlString(collection)}");
            var url = await _site.GetUri(uri);
            if (!url.IsSuccess)
            {
                throw new WebException($"Osu!API encountered an error while: {beatmap.ToUpper()}");
            }
            var jsonData = "{\n\"users\":" + url.JsonData + "}";
            return JsonConvert.DeserializeObject<OsuUserResult>(jsonData);
        }

        public async Task<OsuScoreResult> GetScores(OsuScoreParameters parameters)
        {
            var collection = new NameValueCollection
            {
                { "k", $"{_key}" },
                { "b", $"{parameters.BeatmapId}" }
            };

            /* Parameter Check (Must be written inside a seperate class to prevent sloppy code such as this.) */
            if (parameters.User != null)
            {
                collection.Add("u", $"{parameters.User}");
            }
            if (parameters.Type != null)
            {
                collection.Add("type", $"{parameters.Type}");
            }
            if (parameters.Mode != null)
            {
                collection.Add("m", $"{parameters.Mode}");
            }
            if (parameters.Mods != null)
            {
                collection.Add("mods", $"{parameters.Mods}");
            }
            if (parameters.Limit != null)
            {
                collection.Add("limit", $"{parameters.Limit}");
            }

            var uri = new Uri($"{baseUrl}{score}{ConvertUri.ToUrlString(collection)}");
            var url = await _site.GetUri(uri);
            if (!url.IsSuccess)
            {
                throw new WebException($"Osu!API encountered an error while: {beatmap.ToUpper()}");
            }
            var jsonData = "{\n\"scores\":" + url.JsonData + "}";
            return JsonConvert.DeserializeObject<OsuScoreResult>(jsonData);
        }

        public async Task<OsuUserRecentResult> GetUserRecentBeatmaps(OsuUserRecentParameters parameters)
        {
            var collection = new NameValueCollection
            {
                { "k", $"{_key}" },
                { "u", $"{parameters.User}" }
            };

            /* Parameter Check (Must be written inside a seperate class to prevent sloppy code such as this.) */
            if (parameters.Type != null)
            {
                collection.Add("type", $"{parameters.Type}");
            }
            if (parameters.Mode != null)
            {
                collection.Add("m", $"{parameters.Mode}");
            }
            if (parameters.Limit != null)
            {
                collection.Add("limit", $"{parameters.Limit}");
            }

            var uri = new Uri($"{baseUrl}{userRecent}{ConvertUri.ToUrlString(collection)}");
            var url = await _site.GetUri(uri);
            if (!url.IsSuccess)
            {
                throw new WebException($"Osu!API encountered an error while: {beatmap.ToUpper()}");
            }
            var jsonData = "{\n\"user_recent_beatmaps\":" + url.JsonData + "}";
            return JsonConvert.DeserializeObject<OsuUserRecentResult>(jsonData);
        }

        public async Task <OsuUserBestResult> GetUserBestScores(OsuUserBestParameters parameters)
        {
            var collection = new NameValueCollection
            {
                { "k", $"{_key}" },
                { "u", $"{parameters.User}" }
            };

            /* Parameter Check (Must be written inside a seperate class to prevent sloppy code such as this.) */
            if (parameters.Type != null)
            {
                collection.Add("type", $"{parameters.Type}");
            }
            if (parameters.Mode != null)
            {
                collection.Add("m", $"{parameters.Mode}");
            }
            if (parameters.Limit != null)
            {
                collection.Add("limit", $"{parameters.Limit}");
            }

            var uri = new Uri($"{baseUrl}{userBest}{ConvertUri.ToUrlString(collection)}");
            var url = await _site.GetUri(uri);
            if (!url.IsSuccess)
            {
                throw new WebException($"Osu!API encountered an error while: {beatmap.ToUpper()}");
            }
            var jsonData = "{\n\"user_best_scores\":" + url.JsonData + "}";
            return JsonConvert.DeserializeObject<OsuUserBestResult>(jsonData);
        }

        public async Task<OsuMatchResult> GetMatch(OsuMatchParameters parameters)
        {
            var collection = new NameValueCollection
            {
                { "k", $"{_key}" },
                { "mp", $"{parameters.MatchId}" }
            };

            var uri = new Uri($"{baseUrl}{match}{ConvertUri.ToUrlString(collection)}");
            var url = await _site.GetUri(uri);
            if (!url.IsSuccess)
            {
                throw new WebException($"Osu!API encountered an error while: {beatmap.ToUpper()}");
            }
            var jsonData = "{\n\"matches\":" + url.JsonData + "}";
            return JsonConvert.DeserializeObject<OsuMatchResult>(jsonData);
        }

        public async Task<OsuReplayResult> GetReplay(OsuReplayParameters parameters)
        {
            var collection = new NameValueCollection
            {
                { "k", $"{_key}" },
                { "m", $"{parameters.Mode}" },
                { "b", $"{parameters.BeatmapId}" },
                { "u", $"{parameters.User}" },

            };

            var uri = new Uri($"{baseUrl}{replay}{ConvertUri.ToUrlString(collection)}");
            var url = await _site.GetUri(uri);
            if (!url.IsSuccess)
            {
                throw new WebException($"Osu!API encountered an error while: {beatmap.ToUpper()}");
            }
            var jsonData = "{\n\"replay\":[" + url.JsonData + "]}";
            return JsonConvert.DeserializeObject<OsuReplayResult>(jsonData);
        }
    }

    public class OsuLive
    {
        /* Game Linking URLs */
        public static string baseUrl = "osu://";
        public static string multiplayer = "mp/";
        public static string edit = "edit/";
        public static string channel = "chan/#";
        public static string directDownload = "dl/";
        public static string spectate = "spectate/";

        public static string JoinMultiplayerMatch(string matchId, string password = "")
        {
            password = (password.Equals("")) ? $"/{password}" : "";
            var link = $"{baseUrl}{multiplayer}{matchId}{password}";
            return link;
        }
        public static string EditBeatmap(string position = "<00:00:000>")
        {
            var link = $"{baseUrl}{edit}{position}";
            return link;
        }
        public static string ViewChannel(string channelName = "osu")
        {
            var link = $"{baseUrl}{channel}{channelName}";
            return link;
        }
        public static string GetMapset(string mapset = "93398")
        {
            var link = $"{baseUrl}{directDownload}{mapset}";
            return link;
        }
        public static string SpectateUser(string user = "AbnerSquared")
        {
            var link = $"{baseUrl}{spectate}{user}";
            return link;
        }
    }

    public enum OsuModifications
    {
        None = 0,
        NoFail = 1,
        Easy = 2,
        TouchDevice = 4,
        Hidden = 8,
        HardRock = 16,
        SuddenDeath = 32,
        DoubleTime = 64,
        Relax = 128,
        HalfTime = 256,
        Nightcore = 512,
        Flashlight = 1024,
        Autoplay = 2048,
        SpunOut = 4096,
        Relax2 = 8192,
        Perfect = 16384,
        Key4 = 32768,
        Key5 = 65536,
        Key6 = 131072,
        Key7 = 262144,
        Key8 = 524288,
        FadeIn = 1048576,
        Random = 2097152,
        Cinema = 4194304,
        Target = 8388608,
        Key9 = 16777216,
        KeyCoop = 33554432,
        Key1 = 67108864,
        Key3 = 134217728,
        Key2 = 268435456,
        ScoreV2 = 536870912,
        LastMod = 1073741824,
        KeyMod = Key1 | Key2 | Key3 | Key4 | Key5 | Key6 | Key7 | Key8 | Key9 | KeyCoop,
        FreeModAllowed = NoFail | Easy | Hidden | HardRock | SuddenDeath | Flashlight | FadeIn | Relax | Relax2 | SpunOut | KeyMod,
        ScoreIncreaseMods = Hidden | HardRock | DoubleTime | Flashlight | FadeIn
    }

    /* Return Types */
    public class OsuBeatmapResult
    {
        [JsonProperty("beatmaps")] public OsuBeatmapObject[] Beatmaps { get; set; }
    }

    public class OsuUserResult
    {
        [JsonProperty("users")] public OsuUserObject[] Users { get; set; }
    }

    public class OsuUserBestResult
    {
        [JsonProperty("user_best_scores")] public OsuUserBestObject[] UserBestScores { get; set; }
    }

    public class OsuUserRecentResult
    {
        [JsonProperty("user_recent_beatmaps")] public OsuUserRecentObject[] UserRecentBeatmaps { get; set; }
    }

    public class OsuScoreResult
    {
        [JsonProperty("scores")] public OsuScoreObject[] Scores { get; set; }
    }

    public class OsuMatchResult
    {
        [JsonProperty("matches")] public OsuMatchObject[] Matches { get; set; }
    }

    public class OsuReplayResult
    {
        [JsonProperty("replay")] public OsuReplayObject Replay { get; set; }
    }

    /* Osu! API Base Class Builders */
    public class OsuBeatmapParameters
    {
        public string Since { get; set; }
        public string BeatmapSetId { get; set; }
        public string BeatmapId { get; set; }
        public string User { get; set; }
        public string Type { get; set; }
        public string Mode { get; set; }
        public string IncludeConvertedBeatmaps { get; set; }
        public string BeatmapHash { get; set; }
        public string Limit { get; set; }
    }

    public class OsuBeatmapObject
    {
        [JsonProperty("approved")] public string Approved { get; set; }
        [JsonProperty("approved_date")] public string ApprovedDate { get; set; }
        [JsonProperty("last_update")] public string LastUpdate { get; set; }
        [JsonProperty("artist")] public string Artist { get; set; }
        [JsonProperty("beatmap_id")] public string BeatmapId { get; set; }
        [JsonProperty("beatmapset_id")] public string BeatmapSetId { get; set; }
        [JsonProperty("bpm")] public string Bpm { get; set; }
        [JsonProperty("creator")] public string Creator { get; set; }
        [JsonProperty("creator_id")] public string CreatorId { get; set; }
        [JsonProperty("difficultyrating")] public string DifficultyRating { get; set; }
        [JsonProperty("diff_size")] public string DiffSize { get; set; }
        [JsonProperty("diff_overall")] public string DiffOverall { get; set; }
        [JsonProperty("diff_approach")] public string DiffApproach { get; set; }
        [JsonProperty("diff_drain")] public string DiffDrain { get; set; }
        [JsonProperty("hit_length")] public string HitLength { get; set; }
        [JsonProperty("source")] public string Source { get; set; }
        [JsonProperty("genre_id")] public string GenreId { get; set; }
        [JsonProperty("language_id")] public string LanguageId { get; set; }
        [JsonProperty("title")] public string Title { get; set; }
        [JsonProperty("total_length")] public string TotalLength { get; set; }
        [JsonProperty("version")] public string Version { get; set; }
        [JsonProperty("file_md5")] public string FileMd5 { get; set; }
        [JsonProperty("mode")] public string Mode { get; set; }
        [JsonProperty("tags")] public string Tags { get; set; }
        [JsonProperty("favourite_count")] public string FavoriteCount { get; set; }
        [JsonProperty("playcount")] public string PlayCount { get; set; }
        [JsonProperty("passcount")] public string PassCount { get; set; }
        [JsonProperty("max_combo")] public string MaxCombo { get; set; }
    }

    public class OsuUserParameters
    {
        public string User { get; set; }
        public string Mode { get; set; }
        public string Type { get; set; }
        public string EventDays { get; set; }
    }

    public class OsuUserObject
    {
        [JsonProperty("user_id")] public string UserId { get; set; }
        [JsonProperty("username")] public string Username { get; set; }
        [JsonProperty("count300")] public string Count300 { get; set; }
        [JsonProperty("count100")] public string Count100 { get; set; }
        [JsonProperty("count50")] public string Count50 { get; set; }
        [JsonProperty("playcount")] public string PlayCount { get; set; }
        [JsonProperty("ranked_score")] public string RankedScore { get; set; }
        [JsonProperty("total_score")] public string TotalScore { get; set; }
        [JsonProperty("pp_rank")] public string PpRank { get; set; }
        [JsonProperty("level")] public string Level { get; set; }
        [JsonProperty("pp_raw")] public string PpRaw { get; set; }
        [JsonProperty("accuracy")] public string Accuracy { get; set; }
        [JsonProperty("count_rank_ss")] public string CountRankSs { get; set; }
        [JsonProperty("count_rank_ssh")] public string CountRankSsH { get; set; }
        [JsonProperty("count_rank_s")] public string CountRankS { get; set; }
        [JsonProperty("count_rank_sh")] public string CountRankSH { get; set; }
        [JsonProperty("count_rank_a")] public string CountRankA { get; set; }
        [JsonProperty("country")] public string Country { get; set; }
        [JsonProperty("total_seconds_played")] public string TotalSecondsPlayed { get; set; }
        [JsonProperty("pp_country_rank")] public string PpCountryRank { get; set; }
        [JsonProperty("events")] public UserEventObject[] Events { get; set; }
    }

    public class UserEventObject
    {
        [JsonProperty("display_html")] public string DisplayHtml { get; set; }
        [JsonProperty("beatmap_id")] public string BeatmapId { get; set; }
        [JsonProperty("beatmapset_id")] public string BeatmapSetId { get; set; }
        [JsonProperty("date")] public string Date { get; set; }
        [JsonProperty("epicfactor")] public string EpicFactor { get; set; }
    }

    public class OsuUserBestParameters
    {
        public string User { get; set; }
        public string Mode { get; set; }
        public string Limit { get; set; }
        public string Type { get; set; }
    }

    public class OsuUserBestObject
    {
        [JsonProperty("beatmap_id")] public string BeatmapId { get; set; }
        [JsonProperty("score")] public string Score { get; set; }
        [JsonProperty("maxcombo")] public string MaxCombo { get; set; }
        [JsonProperty("count300")] public string Count300 { get; set; }
        [JsonProperty("count100")] public string Count100 { get; set; }
        [JsonProperty("count50")] public string Count50 { get; set; }
        [JsonProperty("countmiss")] public string CountMiss { get; set; }
        [JsonProperty("countkatu")] public string CountKatu { get; set; }
        [JsonProperty("countgeki")] public string CountGeki { get; set; }
        [JsonProperty("perfect")] public string Perfect { get; set; }
        [JsonProperty("enabled_mods")] public string EnabledMods { get; set; }
        [JsonProperty("user_id")] public string UserId { get; set; }
        [JsonProperty("date")] public string Date { get; set; }
        [JsonProperty("rank")] public string Rank { get; set; }
        [JsonProperty("pp")] public string Pp { get; set; }
    }

    public class OsuUserRecentParameters
    {
        public string User { get; set; }
        public string Mode { get; set; }
        public string Limit { get; set; }
        public string Type { get; set; }
    }

    public class OsuUserRecentObject
    {
        [JsonProperty("beatmap_id")] public string BeatmapId { get; set; }
        [JsonProperty("score")] public string Score { get; set; }
        [JsonProperty("maxcombo")] public string MaxCombo { get; set; }
        [JsonProperty("count300")] public string Count300 { get; set; }
        [JsonProperty("count100")] public string Count100 { get; set; }
        [JsonProperty("count50")] public string Count50 { get; set; }
        [JsonProperty("countmiss")] public string CountMiss { get; set; }
        [JsonProperty("countkatu")] public string CountKatu { get; set; }
        [JsonProperty("countgeki")] public string CountGeki { get; set; }
        [JsonProperty("perfect")] public string Perfect { get; set; }
        [JsonProperty("enabled_mods")] public string EnabledMods { get; set; }
        [JsonProperty("user_id")] public string UserId { get; set; }
        [JsonProperty("date")] public string Date { get; set; }
        [JsonProperty("rank")] public string Rank { get; set; }
    }

    public class OsuScoreParameters
    {
        public string BeatmapId { get; set; }
        public string User { get; set; }
        public string Mode { get; set; }
        public string Mods { get; set; }
        public string Type { get; set; }
        public string Limit { get; set; }
    }

    public class OsuScoreObject
    {
        [JsonProperty("score_id")] public string ScoreId { get; set; }
        [JsonProperty("score")] public string Score { get; set; }
        [JsonProperty("username")] public string Username { get; set; }
        [JsonProperty("count300")] public string Count300 { get; set; }
        [JsonProperty("count100")] public string Count100 { get; set; }
        [JsonProperty("count50")] public string Count50 { get; set; }
        [JsonProperty("countmiss")] public string CountMiss { get; set; }
        [JsonProperty("maxcombo")] public string MaxCombo { get; set; }
        [JsonProperty("countkatu")] public string CountKatu { get; set; }
        [JsonProperty("countgeki")] public string CountGeki { get; set; }
        [JsonProperty("perfect")] public string Perfect { get; set; }
        [JsonProperty("enabled_mods")] public string EnabledMods { get; set; }
        [JsonProperty("user_id")] public string UserId { get; set; }
        [JsonProperty("date")] public string Date { get; set; }
        [JsonProperty("rank")] public string Rank { get; set; }
        [JsonProperty("pp")] public string Pp { get; set; }
        [JsonProperty("replay_available")] public string ReplayAvailable { get; set; }
    }

    public class OsuMatchParameters
    {
        public string MatchId { get; set; }
    }

    public class OsuMatchObject
    {
        [JsonProperty("match")] public MatchInfoObject Match { get; set; }
        [JsonProperty("games")] public MatchGameObject[] Games { get; set; }
    }

    public class MatchInfoObject
    {
        [JsonProperty("match_id")] public string MatchId { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("start_time")] public string StartTime { get; set; }
        [JsonProperty("end_time")] public string EndTime { get; set; }
    }

    public class MatchGameObject
    {
        [JsonProperty("game_id")] public string GameId { get; set; }
        [JsonProperty("start_time")] public string StartTime { get; set; }
        [JsonProperty("end_time")] public string EndTime { get; set; }
        [JsonProperty("beatmap_id")] public string BeatmapId { get; set; }
        [JsonProperty("play_mode")] public string PlayMode { get; set; }
        [JsonProperty("match_type")] public string MatchType { get; set; }
        [JsonProperty("scoring_type")] public string ScoringType { get; set; }
        [JsonProperty("team_type")] public string TeamType { get; set; }
        [JsonProperty("mods")] public string Mods { get; set; }
        [JsonProperty("scores")] public MatchGameScoreObject[] Scores { get; set; }
    }

    public class MatchGameScoreObject
    {
        [JsonProperty("slot")] public string Slot { get; set; }
        [JsonProperty("team")] public string Team { get; set; }
        [JsonProperty("user_id")] public string UserId { get; set; }
        [JsonProperty("score")] public string Score { get; set; }
        [JsonProperty("maxcombo")] public string MaxCombo { get; set; }
        [JsonProperty("rank")] public string Rank { get; set; }
        [JsonProperty("count50")] public string Count50 { get; set; }
        [JsonProperty("count100")] public string Count100 { get; set; }
        [JsonProperty("count300")] public string Count300 { get; set; }
        [JsonProperty("countmiss")] public string CountMiss { get; set; }
        [JsonProperty("countgeki")] public string CountGeki { get; set; }
        [JsonProperty("countkatu")] public string CountKatu { get; set; }
        [JsonProperty("perfect")] public string Perfect { get; set; }
        [JsonProperty("pass")] public string Pass { get; set; }
    }

    public class OsuReplayParameters
    {
        public string Mode { get; set; }
        public string BeatmapId { get; set; }
        public string User { get; set; }
    }

    public class OsuReplayObject
    {
        [JsonProperty("content")] public string Content { get; set; }
        [JsonProperty("encoding")] public string Encoding { get; set; }
    }
}
