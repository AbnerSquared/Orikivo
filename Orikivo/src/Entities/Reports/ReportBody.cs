﻿using Newtonsoft.Json;

namespace Orikivo
{
    public class ReportBody
    {
        internal ReportBody() { }

        public ReportBody(string title, string content, string imageUrl = null)
        {
            Title = title;
            Content = content;
            ImageUrl = imageUrl;
        }

        [JsonProperty("title")]
        public string Title { get; internal set; }

        [JsonProperty("content")]
        public string Content { get; internal set; }

        [JsonProperty("image_url")]
        public string ImageUrl { get; internal set; }
    }
}
