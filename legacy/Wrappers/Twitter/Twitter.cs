using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo.Systems.Wrappers.Twitter
{
    public class Twitter
    {

    }

    public class TwitterUserObject
    {
        [JsonProperty("id")] public long Id { get; set; }
        [JsonProperty("id_str")] public string IdStr { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("screen_name")] public string ScreenName { get; set; }
        [JsonProperty("location")] public string Location { get; set; }
        [JsonProperty("url")] public string Url { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("derived")] public TwitterEnrichmentObject[] Derived { get; set; }
        [JsonProperty("protected")] public bool Protected { get; set; }
        [JsonProperty("verified")] public bool Verified { get; set; }
        [JsonProperty("followers_count")] public int FollowersCount { get; set; }
        [JsonProperty("friends_count")] public int FriendsCount { get; set; }
        [JsonProperty("listed_count")] public int ListedCount { get; set; }
        [JsonProperty("favourites_count")] public int FavouritesCount { get; set; }
        [JsonProperty("statuses_count")] public int StatusesCount { get; set; }
        [JsonProperty("created_at")] public string CreatedAt { get; set; }
        [JsonProperty("utc_offset")] public string UtcOffset { get; set; }
        [JsonProperty("time_zone")] public string TimeZone { get; set; }
        [JsonProperty("geo_enabled")] public bool GeoEnabled { get; set; }
        [JsonProperty("lang")] public string Lang { get; set; }
        [JsonProperty("contributors_enabled")] public bool ContributorsEnabled { get; set; }
        [JsonProperty("profile_background_color")] public string ProfileBackgroundColor { get; set; }
        [JsonProperty("profile_background_image_url")] public string ProfileBackgroundImageUrl { get; set; }
        [JsonProperty("profile_background_image_url_https")] public string ProfileBackgroundImageUrlHttps { get; set; }
        [JsonProperty("profile_background_tile")] public bool ProfileBackgroundTile { get; set; }
        [JsonProperty("profile_banner_url")] public string ProfileBannerUrl { get; set; }
        [JsonProperty("profile_image_url")] public string ProfileImageUrl { get; set; }
        [JsonProperty("profile_image_url_https")] public string ProfileImageUrlHttps { get; set; }
        [JsonProperty("profile_link_color")] public string ProfileLinkColor { get; set; }
        [JsonProperty("profile_sidebar_border_color")] public string ProfileSidebarBorderColor { get; set; }
        [JsonProperty("profile_sidebar_fill_color")] public string ProfileSidebarFillColor { get; set; }
        [JsonProperty("profile_text_color")] public string ProfileTextColor { get; set; }
        [JsonProperty("profile_use_background_image")] public bool ProfileUseBackgroundImage { get; set; }
        [JsonProperty("default_profile")] public bool DefaultProfile { get; set; }
        [JsonProperty("default_profile_image")] public bool DefaultProfileImage { get; set; }
        [JsonProperty("withheld_in_countries")] public string[] WithheldInCountries { get; set; }
        [JsonProperty("withheld_scope")] public string WithheldScope { get; set; }
    }

    public class TwitterEnrichmentObject
    {
        [JsonProperty("country")] public string Country { get; set; }
        [JsonProperty("country_code")] public string CountryCode { get; set; }
        [JsonProperty("locality")] public string Locality { get; set; }
        [JsonProperty("region")] public string Region { get; set; }
        [JsonProperty("sub_region")] public string SubRegion { get; set; }
        [JsonProperty("full_name")] public string FullName { get; set; }
        [JsonProperty("geo")] public TwitterEnrichmentGeoObject Geo { get; set; }
    }

    public class TwitterEnrichmentGeoObject
    {
        [JsonProperty("coordinates")] public double[] Coordinates { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
    }

    public class TwitterTweetObject
    {
        [JsonProperty("created_at")] public string CreatedAt { get; set; }
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("id_str")] public string IdStr { get; set; }
        [JsonProperty("text")] public string Text { get; set; }
        [JsonProperty("source")] public string Source { get; set; }
        [JsonProperty("truncated")] public string Truncated { get; set; }
        [JsonProperty("in_reply_to_status_id")] public string InReplyToStatusId { get; set; }
        [JsonProperty("in_reply_to_status_id_str")] public string InReplyToStatusIdStr { get; set; }
        [JsonProperty("in_reply_to_user_id")] public string InReplyToUserId { get; set; }
        [JsonProperty("in_reply_to_user_id_str")] public string InReplyToUserIdStr { get; set; }
        [JsonProperty("in_reply_to_screen_name")] public string InReplyToScreenName { get; set; }
        [JsonProperty("user")] public string User { get; set; }
        [JsonProperty("coordinates")] public string Coordinates { get; set; }
        [JsonProperty("place")] public string Place { get; set; }
        [JsonProperty("quoted_status_id")] public string QuotedStatusId { get; set; }
        [JsonProperty("quoted_status_id_str")] public string QuotedStatusIdStr { get; set; }
        [JsonProperty("is_quote_status")] public string IsQuoteStatus { get; set; }
        [JsonProperty("quote_status")] public string QuoteStatus { get; set; }
        [JsonProperty("retweeted_status")] public string RetweetedStatus { get; set; }
        [JsonProperty("quote_count")] public string QuoteCount { get; set; }
        [JsonProperty("reply_count")] public string ReplyCount { get; set; }
        [JsonProperty("retweet_count")] public string RetweetCount { get; set; }
        [JsonProperty("favorite_count")] public string FavoriteCount { get; set; }
        [JsonProperty("entities")] public string Entities { get; set; }
        [JsonProperty("extended_entities")] public string ExtendedEntities { get; set; }
        [JsonProperty("favorited")] public string Favorited { get; set; }
        [JsonProperty("retweeted")] public string Retweeted { get; set; }
        [JsonProperty("possibly_sensitive")] public string PossiblySensitive { get; set; }
        [JsonProperty("filter_level")] public string FilterLevel { get; set; }
        [JsonProperty("lang")] public string Lang { get; set; }
        [JsonProperty("matching_rules")] public string MatchingRules { get; set; }
        [JsonProperty("current_user_retweet")] public string CurrentUserRetweet { get; set; }
        [JsonProperty("scopes")] public string Scopes { get; set; }
        [JsonProperty("withheld_copyright")] public string WithheldCopyright { get; set; }
        [JsonProperty("withheld_in_countries")] public string WithheldInCountries { get; set; }
        [JsonProperty("withheld_scope")] public string WithheldScope { get; set; }
    }
}
