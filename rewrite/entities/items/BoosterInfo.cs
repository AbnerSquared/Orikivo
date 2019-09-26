﻿using Newtonsoft.Json;
using System;

namespace Orikivo
{
    public class BoosterInfo
    {
        [JsonProperty("gain_rate")]
        public double GainRate { get; }

        [JsonProperty("expires_on")]
        public DateTime? ExpiresOn { get; }

        [JsonIgnore]
        public bool IsExpired => ExpiresOn.HasValue ? (ExpiresOn.Value - DateTime.UtcNow).TotalSeconds <= 0  : false;
    }
}
