using Discord;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Orikivo
{
    public class CustomCommandMessage
    {
        [JsonConstructor]
        internal CustomCommandMessage(string url, string content, OriEmbedOptions embedOptions = null)
        {
            Url = url;
            Content = content;
            EmbedOptions = embedOptions;

        }

        public static UrlType? GetUrlType(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;
            string ext = Path.GetExtension(url).Substring(1);
            if (new List<string> { "png", "jpg", "gif" }.Contains(ext))
                return Orikivo.UrlType.Image;
            if (new List<string> { "mp4", "mov" }.Contains(ext))
                return Orikivo.UrlType.Video;
            if (new List<string> { "mp3", "wav" }.Contains(ext))
                return Orikivo.UrlType.Audio;
            if (new List<string> { "txt", "cs", "js", "html", "cpp" }.Contains(ext))
                return Orikivo.UrlType.Text;
            return Orikivo.UrlType.Empty;
        }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonIgnore]
        public UrlType? UrlType => HasUrl ? GetUrlType(Url) : null;

        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("embed_options")]
        public OriEmbedOptions EmbedOptions { get; set; }

        [JsonIgnore]
        public bool HasUrl => !string.IsNullOrWhiteSpace(Url); // instead, check if the url is validated

        public OriMessage Build()
            => new OriMessage(Content, Url, EmbedOptions);
    }
}
