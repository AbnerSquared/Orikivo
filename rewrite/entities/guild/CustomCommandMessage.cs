using Discord;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Orikivo
{
    public class CustomCommandMessage
    {
        [JsonConstructor]
        internal CustomCommandMessage(string url, string content, Embedder embedOptions = null)
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
            if (ext.EqualsAny("png", "jpg", "gif"))
                return Orikivo.UrlType.Image;
            if (ext.EqualsAny("mp4", "mov"))
                return Orikivo.UrlType.Video;
            if (ext.EqualsAny("mp3", "wav"))
                return Orikivo.UrlType.Audio;
            if (ext.EqualsAny("txt", "cs", "js", "html", "cpp", "py"))
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
        public Embedder EmbedOptions { get; set; }

        [JsonIgnore]
        public bool HasUrl => !string.IsNullOrWhiteSpace(Url); // instead, check if the url is validated

        public Message Build()
            => new MessageBuilder(Content, Url).Build();
    }
}
