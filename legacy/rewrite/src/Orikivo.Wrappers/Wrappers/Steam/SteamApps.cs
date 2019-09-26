using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Orikivo.Systems.Wrappers.Steam.Services
{
    /*
    public class SteamApps
    {
        private readonly IWebsiteHandler _site = new WebsiteHandler();
        private readonly string _key;
        public static string steamBase = "http://api.steampowered.com";
        public static string serviceBase = "/ISteamApps";
        public static string versionBase = "/v000";

        public SteamApps(string key = "")
        {
            _key = key;
        }

        public async Task<SteamAppResults> GetAppList(int methodVersion)
        {
            var methodBase = "/GetAppList";
            int[] versionList =
            {
                1, 2
            };
            if (!versionList.Contains(methodVersion))
            {
                var nullAppVersion = File.ReadAllText("SteamNullAppVersion.json");
                return JsonConvert.DeserializeObject<SteamAppResults>(nullAppVersion);
            }

            var versionType = methodVersion;

            var collection = new NameValueCollection
            {
                { "key", _key }
            };
            var url = await _site.GetUri(new Uri($"{steamBase}{serviceBase}{methodBase}{versionBase}{versionType}{ConvertUri.ToUrlString(collection)}"));
            Console.WriteLine($"{steamBase}{serviceBase}{methodBase}{versionBase}{versionType}{ConvertUri.ToUrlString(collection)}");

            if (!url.IsSuccess)
            {
                throw new WebException("The application list failed to render.");
            }

            return JsonConvert.DeserializeObject<SteamAppResults>(url.JsonData);
        }

        public async Task GetServersAtAddress(string address)
        {
        }

        public async Task UpToDateCheck(uint appid, uint version)
        {
        }
}

*/

    public class SteamAppResults
    {
        [JsonProperty("applist")] public SteamApplications Apps { get; set; }
    }

    public class SteamApplications
    {
        [JsonProperty("apps")] public SteamApplicationBase[] AppInformation { get; set; }
    }

    public class SteamApplicationBase
    {
        [JsonProperty("appid")] public int ApplicationID { get; set; }
        [JsonProperty("name")] public string ApplicationName { get; set; }
    }
}