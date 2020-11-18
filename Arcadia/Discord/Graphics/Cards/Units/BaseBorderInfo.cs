using Newtonsoft.Json;
using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    public class BaseBorderInfo
    {
        [JsonProperty("allowed")]
        public BorderAllow Allowed { get; set; }

        [JsonProperty("edge")]
        public BorderEdge Edge { get; set; } = BorderEdge.Outside;

        [JsonProperty("fill")]
        public BaseFillInfo Fill { get; set; }

        [JsonProperty("thickness")]
        public int Thickness { get; set; }
    }
}
