using Newtonsoft.Json;
using System;

namespace Orikivo.Unstable
{
    public class GuildData
    {
        public GuildData() { }
        public static GuildData Empty = new GuildData { Exp = 0, ActiveExp = 0, LastMessage = null };

        [JsonConstructor]
        internal GuildData(ulong exp, DateTime? lastMessage, ulong activeExp)
        {
            Exp = exp;
            LastMessage = lastMessage;
            ActiveExp = activeExp;
        }

        [JsonProperty("exp")]
        public ulong Exp { get; private set; } // total exp earned from participation within in a guild

        [JsonProperty("last_sent")]
        public DateTime? LastMessage { get; private set; } // the last time a message was sent in this guild.
        
        [JsonProperty("active")]
        public ulong ActiveExp { get; private set; }
        // exp - (decay total from last message)
        // each time a set number of days pass w/o participation, attribute an exp decay
    }
}
