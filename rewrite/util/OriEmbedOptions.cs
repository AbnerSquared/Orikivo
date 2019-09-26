using System;
using System.Collections.Generic;
using System.Text;
using SysColor = System.Drawing.Color;
using DiscordColor = Discord.Color;
using Discord;
using Newtonsoft.Json;

namespace Orikivo
{
    // used to override certain properties.
    // instead of making an embedoptions class, try making extensions
    public class OriEmbedOptions
    {
        public static OriEmbedOptions Default
        {
            get
            {
                OriEmbedOptions embedOptions = new OriEmbedOptions();
                embedOptions.Color = (110, 250, 200);
                embedOptions.IsLocalImage = false;
                embedOptions.StampCurrentTime = false;

                return embedOptions;
            }
        }

        [JsonProperty("color")]
        public (byte r, byte g, byte b) Color { get; set; }
        //public EmbedAuthorBuilder Author { get; set; }
        //public List<EmbedFieldBuilder> Fields { get; set; }
        //public EmbedFooterBuilder Footer { get; set; }

        [JsonProperty("is_local_image")]
        public bool IsLocalImage { get; set; } // only for image spot

        [JsonProperty("stamp_current_time")]
        public bool StampCurrentTime { get; set; }

        [JsonProperty("internal_timestamp")]
        public DateTime? InternalTimestamp { get; set; }
        
        [JsonIgnore]
        public DateTime? Timestamp { get { return StampCurrentTime ? DateTime.UtcNow : InternalTimestamp; } set { InternalTimestamp = value; } }
        //public bool ShowCurrentTime { get; set; }
        //public DateTime? Timestamp { get; set; }
    }
}
