using Newtonsoft.Json;
using System;

namespace Orikivo
{
    /// <summary>
    /// Represents a balance value at a specified date.
    /// </summary>
    public class BankStatement
    {
        [JsonProperty("balance")]
        ulong Balance { get; set; }

        [JsonProperty("date")]
        DateTime Date { get; set; }
    }
}
