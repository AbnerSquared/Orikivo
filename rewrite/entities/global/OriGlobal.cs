using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    // root orikivo data container
    /// <summary>
    /// The data stored across all of Orikivo.
    /// </summary>
    public class OriGlobal
    {
        public const string DEFAULT_PREFIX = "[";

        [JsonConstructor]
        internal OriGlobal(string prefix, ReportContainer reports)
        {
            Reports = reports ?? new ReportContainer();
        }

        public OriGlobal()
        {
        }

        public static string ClientName = "Orikivo";
        public static ulong DevId = 181605794159001601;
        public static ulong SupportGuildId = 456195057373020160;
        
        public static Range PrefixLimit = new Range(1, 16, true);
        public static string ClientVersion = "0.0.5a_dev";

        /// <summary>
        /// Returns the global prefix used for Orikivo.
        /// </summary>
        [JsonProperty("prefix")]
        public string Prefix => DEFAULT_PREFIX;

        public ReportContainer Reports { get; private set; }
    }
}
