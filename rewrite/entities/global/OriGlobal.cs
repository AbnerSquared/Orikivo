using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    // root orikivo data container
    public class OriGlobal
    {
        [JsonConstructor]
        internal OriGlobal(string prefix, ReportContainer reports)
        {
            Prefix = prefix;
            Reports = reports ?? new ReportContainer();
        }

        public OriGlobal()
        {
            Prefix = DefaultPrefix;
        }

        public static string ClientName = "Orikivo";
        public static ulong DevId = 181605794159001601;
        public static ulong SupportGuildId = 456195057373020160;
        public static string DefaultPrefix = "[";
        public static Range PrefixLimit = new Range(1, 16, true);
        public static string ClientVersion = "0.0.5a_dev";

        private string _prefix;

        [JsonProperty("prefix")]
        public string Prefix
        {
            get { return Checks.NotNull(_prefix) ? _prefix : DefaultPrefix; }
            set { _prefix = (Checks.NotNull(_prefix) && PrefixLimit.ContainsValue(value.Length)) ? value : null; }
        }

        public ReportContainer Reports { get; private set; }
    }
}
