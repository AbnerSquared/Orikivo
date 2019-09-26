using Newtonsoft.Json;

namespace Orikivo.Systems.Wrappers.Giphy.Objects.GifObject.ImageObject.Bases
{
    public class StaticMp4
    {
        [JsonProperty("mp4")] public string Mp4 { get; set; }
    }

    public class SizableStill
    {
        [JsonProperty("url")] public string Url { get; set; }
        [JsonProperty("width")] public string Width { get; set; }
        [JsonProperty("height")] public string Height { get; set; }
    }

    public class SizableGif
    {
        [JsonProperty("url")] public string Url { get; set; }
        [JsonProperty("width")] public string Width { get; set; }
        [JsonProperty("height")] public string Height { get; set; }
        [JsonProperty("size")] public string Size { get; set; }
    }

    public class SizableMp4
    {
        [JsonProperty("mp4")] public string Mp4 { get; set; }
        [JsonProperty("mp4_size")] public string Mp4Size { get; set; }
        [JsonProperty("width")] public string Width { get; set; }
        [JsonProperty("height")] public string Height { get; set; }
    }

    public class SizableWebP
    {
        [JsonProperty("url")] public string Url { get; set; }
        [JsonProperty("width")] public string Width { get; set; }
        [JsonProperty("height")] public string Height { get; set; }
        [JsonProperty("size")] public string Size { get; set; }
        [JsonProperty("webp")] public string WebP { get; set; }
        [JsonProperty("webp_size")] public string WebPSize { get; set; }
    }

    public class SizableMedia
    {
        [JsonProperty("url")] public string Url { get; set; }
        [JsonProperty("width")] public string Width { get; set; }
        [JsonProperty("height")] public string Height { get; set; }
        [JsonProperty("size")] public string Size { get; set; }
        [JsonProperty("frames")] public string Frames { get; set; }
        [JsonProperty("mp4")] public string Mp4 { get; set; }
        [JsonProperty("mp4_size")] public string Mp4Size { get; set; }
        [JsonProperty("webp")] public string WebP { get; set; }
        [JsonProperty("webp_size")] public string WebPSize { get; set; }
    }
}