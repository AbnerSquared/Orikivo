using Newtonsoft.Json;

namespace Orikivo
{
    // cooldown reward info.. every n days it ticks
    public struct CooldownModuloInfo
    {
        [JsonProperty("modulo")]
        public int Days { get; }
        
        [JsonProperty("packets")]
        public VarPacket[] Packets { get; } // update packets for user.
    }
}
