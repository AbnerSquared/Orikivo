using Newtonsoft.Json;
using Orikivo.Systems.Wrappers.Giphy.Objects.GifObject.ImageObject;

namespace Orikivo.Systems.Wrappers.Giphy.Objects.GifObject
{
    public class GiphyData
    {
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("slug")] public string Slug { get; set; }
        [JsonProperty("url")] public string Url { get; set; }
        [JsonProperty("bitly_gif_url")] public string BitlyGifUrl { get; set; }
        [JsonProperty("bitly_url")] public string BitlyUrl { get; set; }
        [JsonProperty("embed_url")] public string EmbedUrl { get; set; }
        [JsonProperty("username")] public string Username { get; set; }
        [JsonProperty("source")] public string Source { get; set; }
        [JsonProperty("rating")] public string Rating { get; set; }
        [JsonProperty("content_url")] public string ContentUrl { get; set; }
        [JsonProperty("user")] public GiphyUserData User { get; set; }
        [JsonProperty("source_tld")] public string SourceTld { get; set; }
        [JsonProperty("source_post_url")] public string SourcePostUrl { get; set; }
        [JsonProperty("update_datetime")] public string UpdateDate { get; set; }
        [JsonProperty("create_datetime")] public string CreateDate { get; set; }
        [JsonProperty("import_datetime")] public string ImportDate { get; set; }
        [JsonProperty("trending_datetime")] public string TrendingDate { get; set; }
        [JsonProperty("images")] public GiphyImageData Images { get; set; }
        [JsonProperty("title")] public string Title { get; set; }
    }
}