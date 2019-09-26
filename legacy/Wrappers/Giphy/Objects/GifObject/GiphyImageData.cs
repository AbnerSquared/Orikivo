using Newtonsoft.Json;
using Orikivo.Systems.Wrappers.Giphy.Objects.GifObject.ImageObject.Bases;

namespace Orikivo.Systems.Wrappers.Giphy.Objects.GifObject
{
    public class GiphyImageData
    {
        [JsonProperty("fixed_height")] public SizableMedia FixedHeight { get; set; }
        [JsonProperty("fixed_height_still")] public SizableStill StaticFixedHeight { get; set; }
        [JsonProperty("fixed_height_downsampled")] public SizableWebP FixedHeightDownsampled { get; set; }
        [JsonProperty("fixed_width")] public SizableMedia FixedWidth { get; set; }
        [JsonProperty("fixed_width_still")] public SizableStill StaticFixedWidth { get; set; }
        [JsonProperty("fixed_width_downsampled")] public SizableWebP FixedWidthDownsampled { get; set; }
        [JsonProperty("fixed_height_small")] public SizableWebP FixedHeightSmall { get; set; }
        [JsonProperty("fixed_height_small_small")] public SizableStill FixedHeightDoubleSmall { get; set; }
        [JsonProperty("fixed_width_small")] public SizableWebP FixedWidthSmall { get; set; }
        [JsonProperty("fixed_width_small_small")] public SizableStill FixedWidthDoubleSmall { get; set; }
        [JsonProperty("downsized")] public SizableGif Downsized { get; set; }
        [JsonProperty("downsized_still")] public SizableStill StaticDownsized { get; set; }
        [JsonProperty("downsized_large")] public SizableGif DownsizedLarge { get; set; }
        [JsonProperty("downsized_medium")] public SizableGif DownsizedMedium { get; set; }
        [JsonProperty("downsized_small")] public SizableGif DownsizedSmall { get; set; }
        [JsonProperty("original")] public SizableMedia Original { get; set; }
        [JsonProperty("original_still")] public SizableStill StaticOriginal { get; set; }
        [JsonProperty("looping")] public StaticMp4 Looping { get; set; }
        [JsonProperty("preview")] public SizableMp4 PreviewMp4 { get; set; }
        [JsonProperty("preview_gif")] public SizableGif PreviewGif { get; set; }
    }
}