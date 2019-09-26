using Newtonsoft.Json;

namespace Orikivo.Systems.Wrappers.Tenor.Objects.GifObject
{
    public class TenorMediaData
    {
        [JsonProperty("gif")] public TenorMediaDataType Gif { get; set; }
        [JsonProperty("mediumgif")] public TenorMediaDataType MediumGif { get; set; }
        [JsonProperty("tinygif")] public TenorMediaDataType TinyGif { get; set; }
        [JsonProperty("nanogif")] public TenorMediaDataType NanoGif { get; set; }
        [JsonProperty("webm")] public TenorMediaDataType WebM { get; set; }
        [JsonProperty("tinywebm")] public TenorMediaDataType TinyWebM { get; set; }
        [JsonProperty("nanowebm")] public TenorMediaDataType NanoWebM { get; set; }
        [JsonProperty("mp4")] public TenorMediaDataType Mp4 { get; set; }
        [JsonProperty("loopedmp4")] public TenorMediaDataType LoopedMp4 { get; set; }
        [JsonProperty("tinymp4")] public TenorMediaDataType TinyMp4 { get; set; }
        [JsonProperty("nanomp4")] public TenorMediaDataType NanoMp4 { get; set; }
    }
}