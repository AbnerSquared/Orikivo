using Newtonsoft.Json;
using System;

namespace Orikivo
{
    public class SpokenContext
    {
        [JsonProperty("word")]
        public string Word { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }
    }
}
