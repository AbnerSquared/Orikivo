using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Orikivo.Systems.Wrappers.Steam.Services
{
    public class SteamAppInformation
    {
        //private readonly IWebsiteHandler _site = new WebsiteHandler();
        public static string SteamStoreBase = "https://store.steampowered.com/api/appdetails";
        /*
        public async Task<Dictionary<string, SteamApplicationInformationSet>> GetAppInfo(string appId)
        {
            var collection = new NameValueCollection
            {
                { "appids", appId }
            };

            var url = await _site.GetUri(new Uri($"{SteamStoreBase}{ConvertUri.ToUrlString(collection)}"));
            if (!url.IsSuccess)
            {
                throw new WebException("The Steam application failed to render.");
            }
            Console.WriteLine($"{SteamStoreBase}{ConvertUri.ToUrlString(collection)}");
            var urlJson = url.JsonData.Replace("\"pc_requirements\":[]", "\"pc_requirements\":{}");
                urlJson = urlJson.Replace("\"mac_requirements\":[]", "\"mac_requirements\":{}");
                urlJson = urlJson.Replace("\"linux_requirements\":[]", "\"linux_requirements\":{}");
            return JsonConvert.DeserializeObject<Dictionary<string, SteamApplicationInformationSet>>(urlJson);
        }
        */
    }

    public class SteamApplicationInformationSet
    {
        [JsonProperty("success")] public bool ApplicationSuccess { get; set; }
        [JsonProperty("data")] public SteamApplicationInformationData ApplicationData { get; set; }
    }

    public class SteamApplicationInformationData
    {
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("steam_appid")] public int Id { get; set; }
        [JsonProperty("required_age")] public int RequiredAge { get; set; }
        [JsonProperty("is_free")] public bool FreeCheck { get; set; }
        [JsonProperty("dlc")] public int[] Dlc { get; set; }
        [JsonProperty("controller_support")] public string ControllerSupport { get; set; }
        [JsonProperty("detailed_description")] public string Description { get; set; }
        [JsonProperty("about_the_game")] public string Summary { get; set; }
        [JsonProperty("short_description")] public string ShortDescription { get; set; }
        [JsonProperty("supported_languages")] public string Languages { get; set; }
        [JsonProperty("header_image")] public string HeaderImage { get; set; }
        [JsonProperty("website")] public string Website { get; set; }
        [JsonProperty("pc_requirements")] public SteamAppBaseRequirements PcReq { get; set; }
        [JsonProperty("mac_requirements")] public SteamAppBaseRequirements MacReq { get; set; }
        [JsonProperty("linux_requirements")] public SteamAppBaseRequirements LinuxReq { get; set; }
        [JsonProperty("developers")] public string[] Developers { get; set; }
        [JsonProperty("publishers")] public string[] Publishers { get; set; }
        [JsonProperty("demos")] public SteamAppDemoBase Demo { get; set; }
        [JsonProperty("price_overview")] public SteamAppPriceOverview Price { get; set; }
        [JsonProperty("packages")] public int[] Packages { get; set; }
        [JsonProperty("package_groups")] public SteamAppPackageGroups[] PackageGroups { get; set; }
        [JsonProperty("platforms")] public SteamAppPlatforms Platforms { get; set; }
        [JsonProperty("metacritic")] public SteamAppMetacriticBase MetacriticRating { get; set; }
        [JsonProperty("categories")] public SteamAppCategoryBase[] Categories { get; set; }
        [JsonProperty("genres")] public SteamAppGenreBase[] Genres { get; set; }
        [JsonProperty("screenshots")] public SteamAppScreenshotBase[] Screenshots { get; set; }
        [JsonProperty("movies")] public SteamAppMovieBase[] Movies { get; set; }
        [JsonProperty("recommendations")] public SteamAppRecommendationBase Recommendations { get; set; }
        [JsonProperty("achievements")] public SteamAppPartialAchievementBase Achievements { get; set; }
        [JsonProperty("release_date")] public SteamAppReleaseDateInformation ReleaseDate { get; set; }
        [JsonProperty("support_info")] public SteamAppSupportInformation SupportInformation { get; set; }
        [JsonProperty("background")] public string Background { get; set; }
    }

    public class SteamAppBaseRequirements
    {
        [JsonProperty("minimum")] public string Minimum { get; set; }
        [JsonProperty("recommended")] public string Recommended { get; set; }
    }

    public class SteamAppDemoBase
    {
        [JsonProperty("appid")] public int Id { get; set; }
        [JsonProperty("description")] public string Description { get; set; }

    }

    public class SteamAppPriceOverview
    {
        [JsonProperty("currency")] public string Currency { get; set; }
        [JsonProperty("initial")] public string Initial { get; set; }
        [JsonProperty("final")] public string Final { get; set; }
        [JsonProperty("discount_percent")] public string Discount { get; set; }
    }

    public class SteamAppPackageGroups
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("title")] public string Title { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("selection_text")] public string OnSelection { get; set; }
        [JsonProperty("save_text")] public string OnSave { get; set; }
        [JsonProperty("display_type")] public string Display { get; set; }
        [JsonProperty("is_recurring_subscription")] public string RecurringSubsciptionCheck { get; set; }
        [JsonProperty("subs")] public SteamAppSubPackageGroups[] SubPackage { get; set; }
    }

    public class SteamAppSubPackageGroups
    {
        [JsonProperty("packageid")] public int PackageId { get; set; }
        [JsonProperty("percent_savings_text")] public string PercentSavedText { get; set; }
        [JsonProperty("percent_savings")] public int PercentSaved { get; set; }
        [JsonProperty("option_text")] public string OptText { get; set; }
        [JsonProperty("option_description")] public string OptDescription { get; set; }
        [JsonProperty("can_get_free_license")] public int CanGetFreeLicenseCheck { get; set; }
        [JsonProperty("is_free_license")] public bool FreeLicenseCheck { get; set; }
        [JsonProperty("price_in_cents_with_discount")] public int PrinceInDiscount { get; set; }
    }

    public class SteamAppPlatforms
    {
        [JsonProperty("windows")] public bool OnWindows { get; set; }
        [JsonProperty("mac")] public bool OnMac { get; set; }
        [JsonProperty("linux")] public bool OnLinux { get; set; }
    }

    public class SteamAppMetacriticBase
    {
        [JsonProperty("score")] public int Score { get; set; }
        [JsonProperty("url")] public string Url { get; set; }
    }

    public class SteamAppCategoryBase
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
    }

    public class SteamAppGenreBase
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
    }

    public class SteamAppScreenshotBase
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("path_full")] public string Url { get; set; }
        [JsonProperty("path_thumbnail")] public string ThumbnailUrl { get; set; }
    }

    public class SteamAppMovieBase
    {
        [JsonProperty("highlight")] public bool HighlightCheck { get; set; }
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("thumbnail")] public string Thumbnail { get; set; }
        [JsonProperty("webm")] public SteamAppWebMBase VideoContainer { get; set; }
    }

    public class SteamAppWebMBase
    {
        [JsonProperty("480")] public string MediumQuality { get; set; }
        [JsonProperty("max")] public string MaxQuality { get; set; }
    }

    public class SteamAppRecommendationBase
    {
        [JsonProperty("total")] public int Total { get; set; }
    }

    public class SteamAppPartialAchievementBase
    {
        [JsonProperty("total")] public int Total { get; set; }
        [JsonProperty("highlighted")] public SteamAppHighlightedAchievementBase[] HighlightedAchievements { get; set; }
    }

    public class SteamAppHighlightedAchievementBase
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("path")] public string Url { get; set; }
    }

    public class SteamAppReleaseDateInformation
    {
        [JsonProperty("coming_soon")] public bool Unreleased { get; set; }
        [JsonProperty("date")] public string Date { get; set; }
    }

    public class SteamAppSupportInformation
    {
        [JsonProperty("url")] public string Url { get; set; }
        [JsonProperty("email")] public string EMail { get; set; }
    }
}