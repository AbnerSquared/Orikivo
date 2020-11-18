using Newtonsoft.Json;
using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    public class BaseFillInfo
    {
        [JsonProperty("mode")]
        public FillMode Mode { get; set; } = FillMode.None;

        [JsonProperty("primary")]
        public Gamma? Primary { get; set; }

        [JsonProperty("secondary")]
        public Gamma? Secondary { get; set; }

        [JsonProperty("direction")]
        public Direction Direction { get; set; }

        [JsonProperty("fill_percent")]
        public float? FillPercent { get; set; }
    }
}
