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

namespace Orikivo.Systems.Wrappers.E926
{
    public class E926Wrapper
    {
        private readonly IWebsiteHandler _site = new WebsiteHandler();
        const string baseUri = "https://e926.net/";
        const string POST_COLLECTION = "post/index.json";
        const string POST_RESULT = "post/show.json";

        public async Task<E926BaseResults> GetPost(ulong id)
        {
            var collection = new NameValueCollection
            {
                {"id", $"{id}"}
            };
            var url = await _site.GetUri(new Uri($"{baseUri}{POST_RESULT}{ConvertUri.ToUrlString(collection)}"));
            if (!url.IsSuccess)
            {
                throw new WebException($"E926 failed to collect information on ID#{id}");
            }
            Console.WriteLine($"{baseUri}{POST_RESULT}{ConvertUri.ToUrlString(collection)}");
            return JsonConvert.DeserializeObject<E926BaseResults>("{\nposts:[" + url.JsonData + "]}");
        }

            public async Task<E926BaseResults> GetPostCollection(string tag, int limit = 10)
        {
            var collection = new NameValueCollection
            {
                { "tags", tag }
            };
            if (limit < -1)
            {
                collection.Add("limit", limit.ToString());
            }

            var url = await _site.GetUri(new Uri($"{baseUri}{POST_COLLECTION}{ConvertUri.ToUrlString(collection)}"));
            if (!url.IsSuccess)
            {
                throw new WebException("E926 failed to collect search results.");
            }
            Console.WriteLine($"{baseUri}{POST_COLLECTION}{ConvertUri.ToUrlString(collection)}");
            return JsonConvert.DeserializeObject<E926BaseResults>("{\nposts:" + url.JsonData + "}");
        }
        
        /*public async Task GetRandomPopular()
        {
            var collection = new NameValueCollection
            {
                { "tags", tagList }
            };

        }
         */
    }
    
    public class E926BaseResults
    {
        [JsonProperty("posts")] public E926PostResults[] Posts { get; set; }
    }

    public class E926PostResults
    {
        [JsonProperty("id")] public int Id {get; set;}
        [JsonProperty("author")] public string Author { get; set; }
        [JsonProperty("creator_id")] public int CreatorId { get; set; }
        [JsonProperty("created_at")] public E926CreatedAtResults CreatedAt { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("source")] public string Source { get; set; }
        [JsonProperty("sources")] public string[] Sources { get; set; }
        [JsonProperty("tags")] public string Tags { get; set; }
        [JsonProperty("locked_tags")] public string LockedTags { get; set; }
        [JsonProperty("artist")] public string[] Artist { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("fav_count")] public int FavCount { get; set; }
        [JsonProperty("score")] public int Score { get; set; }
        [JsonProperty("rating")] public string Rating { get; set; }
        [JsonProperty("parent_id")] public int? ParentId { get; set; }
        [JsonProperty("has_children")] public bool HasChildren { get; set; }
        [JsonProperty("children")] public string Children { get; set; }
        [JsonProperty("has_notes")] public bool HasNotes { get; set; }
        [JsonProperty("has_comments")] public bool HasComments { get; set; }
        [JsonProperty("md5")] public string Md5 { get; set; }
        [JsonProperty("file_url")] public string FileUrl { get; set; }
        [JsonProperty("file_ext")] public string FileExt { get; set; }
        [JsonProperty("file_size")] public int FileSize { get; set; }
        [JsonProperty("width")] public int Width { get; set; }
        [JsonProperty("height")] public int Height { get; set; }
        [JsonProperty("sample_url")] public string SampleUrl { get; set; }
        [JsonProperty("sample_width")] public int SampleWidth { get; set; }
        [JsonProperty("sample_height")] public int SampleHeight { get; set; }
        [JsonProperty("preview_url")] public string PreviewUrl { get; set; }
        [JsonProperty("preview_width")] public int PreviewWidth { get; set; }
        [JsonProperty("preview_height")] public int PreviewHeight { get; set; }
        [JsonProperty("delreason")] public string DelReason { get; set; }
    }

    public class E926CreatedAtResults
    {
        [JsonProperty("json_class")] public string JsonClass { get; set; }
        [JsonProperty("s")] public int S { get; set; }
        [JsonProperty("n")] public int N { get; set; }
    }
}
