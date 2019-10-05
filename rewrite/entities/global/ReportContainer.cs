using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    // used to keep track of reports and vice versa.
    public class ReportContainer
    {
        [JsonConstructor]
        internal ReportContainer(int reportIndex, List<ReportInfo> reports)
        {
            ReportIndex = reportIndex;
            Reports = reports ?? new List<ReportInfo>();
        }

        internal ReportContainer()
        {
            ReportIndex = 0;
            Reports = new List<ReportInfo>();
        }

        public int Add(OriUser user, OverloadDisplayInfo overload, ReportBodyInfo info, params ReportTag[] tags)
        {
            ReportIndex += 1;
            Reports.Add(new ReportInfo(ReportIndex, overload, user, info, tags));
            return ReportIndex; // the new report id.
        }

        public void Close(int id, string reason = null)
            => this[id].Close(reason);

        [JsonProperty("cases")]
        public int ReportIndex { get; private set; }
        [JsonProperty("reports")]
        public List<ReportInfo> Reports { get; private set; }

        [JsonIgnore]
        public int Count => Reports.Count;

        public bool Contains(int id)
            => Reports.Any(x => x.Id == id);

        public ReportInfo this[int id]
        {
            get
            {
                return Reports.FirstOrDefault(x => x.Id == id);
            }
        }
    }
}
