using Newtonsoft.Json;

namespace Orikivo.Systems.Wrappers.Giphy.Objects.GifObject
{
    public class GiphyUserData
    {
        [JsonProperty("avatar_url")] public string AvatarUrl { get; set; }
        [JsonProperty("banner_url")] public string BannerUrl { get; set; }
        [JsonProperty("profile_url")] public string ProfileUrl { get; set; }
        [JsonProperty("username")] public string Username { get; set; }
        [JsonProperty("display_name")] public string DisplayName { get; set; }
        [JsonProperty("twitter")] public string TwitterName { get; set; }
    }
}