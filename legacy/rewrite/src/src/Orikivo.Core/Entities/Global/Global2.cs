using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
using Discord.Commands;
using Orikivo.Networking;
namespace Orikivo
{
    public struct Uptime
    {
        public Uptime(DateTime time)
        {
            Boot = time;
        }
        public static Uptime FromBootTime(DateTime time)
        {
            return new Uptime(time);
        }
        // the time of launch
        private DateTime Boot { get; set; }

        // this class when referenced by itself should return a TimeSpan
        // displaying the duration of time it was up.
    }

    /// <summary>
    /// Represents the token resources of Orikivo.
    /// </summary>
    public class TokenCache
    {
        public string Token { get; }
        public string Giphy { get; }
        public string Steam { get; }
        public string Osu { get; }
        public string Bungie { get; }
        public string Tenor { get; }
    }


    public class Global
    {
        [JsonIgnore]
        public static DiscordSocketClient Client { get; set; }

        [JsonIgnore]
        private static OriWebClient _client;

        [JsonIgnore]
        public static OriWebClient WebClient
        {
            get
            {
                if (!_client.Exists())
                    _client = OriWebClient.Default;
               
                return _client;
            }
            set
            {
                _client = value;
            }
        }

        [JsonIgnore]
        public const string ClientName = "Orikivo";

        [JsonIgnore]
        public const string ClientVersion = "0.0.600";

        [JsonIgnore]
        public static CommandService Service { get; set; }

        // a list of bad words disabled by default when wordguard is up.
        [JsonIgnore]
        public static List<string> GlobalWordBlacklist = new List<string> { "heck" };

        [JsonIgnore]
        public static Range PrefixLimit = new Range(1, 16);

        [JsonIgnore]
        public static Range NicknameLimit = new Range(2, 32);

        [JsonIgnore]
        public const string VotingUrl = "https://discordbots.org/bot/433079994164576268/vote";

        public Version Version { get; set; }

        // however long this bot was on.
        public Uptime Uptime { get; set; }

        // the date of the last time the bot was turned on.
        public DateTime LastSessionBoot { get; set; }


        // keeps track of all reports. each report has certain flags.
        public ReportCollection Reports { get; set; }

        // keeps track of all customized clipboards
        public ClipboardCollection Clipboards { get; set; }
        public ulong ReportCounter { get; set; }

        [JsonIgnore]
        public IActivity Activity { get { return Client.CurrentUser.Activity; } set { Client.SetActivityAsync(value); } }

        [JsonIgnore]
        public UserStatus Status { get { return Client.CurrentUser.Status; } set { Client.SetStatusAsync(value); } }

        // where reports are sent.
        public ulong ReportRelay { get; set; }

        // where suggestions are sent.
        public ulong SuggestRelay { get; set; }

        public IconManager Icons { get; set; }

        public bool TryGetClipboard(ulong id, out List<Clipboard2> clipboards)
        {
            clipboards = null;
            if (Clipboards.ContainsAuthor(id))
            {
                clipboards = Clipboards.FromAuthor(id);
                return true;
            }
            return false;
        }

        public List<ulong> AuthUsers { get; private set; }
        public static ulong DevId = 181605794159001601;

        public bool IsAuthUser(SocketUser user)
            => AuthUsers.Contains(user.Id) ? true : false;

        public bool IsAuthUser(ulong id)
            => AuthUsers.Contains(id) ? true : false;
    }
}
