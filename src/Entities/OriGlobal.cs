using Newtonsoft.Json;
using System.Reflection;

namespace Orikivo
{
    /// <summary>
    /// The root data stored across all of Orikivo.
    /// </summary>
    public class OriGlobal
    {
        public const string DEFAULT_PREFIX = "[";

        public static readonly string ClientName = "Orikivo";
        public static readonly string ClientVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();
        public static readonly ulong DevId = 181605794159001601;
        public static readonly ulong SupportId = 456195057373020160;

        public static Drawing.RangeF PrefixLimit = new Drawing.RangeF(1, 16);

        [JsonConstructor]
        internal OriGlobal(ReportContainer reports)
        {
            Reports = reports ?? new ReportContainer();
        }

        public OriGlobal()
        {
            Reports = new ReportContainer();
        }
        
        [JsonIgnore]
        public string Prefix => DEFAULT_PREFIX;

        [JsonIgnore]
        public string Version => ClientVersion;

        [JsonProperty("reports")]
        public ReportContainer Reports { get; private set; }
    }
}
