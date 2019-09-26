using Newtonsoft.Json;
using System;

namespace Orikivo
{
    /// <summary>
    /// Used to define a money transaction.
    /// </summary>
    public class Receipt
    {
        /// <summary>
        /// The storage location of where this transaction occured.
        /// </summary>
        [JsonProperty("account")]
        public BankStorage Account { get; set; }

        [JsonProperty("old")]
        ulong Before { get; set; }

        [JsonProperty("new")]
        ulong After { get; set; }

        [JsonProperty("amount")]
        ulong Difference { get; set; }

        [JsonProperty("date")]
        DateTime Date { get; set; }
    }
}
